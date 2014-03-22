/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * Credit : G. Himangi, Code Project
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                           // <wao never.end>

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Psl.Design {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                         ListBox d'affichage                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension du contrôle <see cref="CheckedListBox"/> pour l'affichage des items
  /// </summary>
  [ToolboxItem(false)]
  public class EnumFlagsCheckedListBox : CheckedListBox {

    private bool isUpdatingCheckStates = false;
    private Type enumType;
    private Enum enumValue;

    /// <summary>
    /// Constructeur
    /// </summary>
    public EnumFlagsCheckedListBox() {
      CheckOnClick = true;
    }

    /// <summary>
    /// Ajoute une valeur entière et sa description
    /// </summary>
    /// <param name="value">valeur</param>
    /// <param name="text">description</param>
    /// <returns>l'item créé et ajouté</returns>
    public EnumFlagsCheckedListBoxItem Add( int value, string text ) {
      EnumFlagsCheckedListBoxItem item = new EnumFlagsCheckedListBoxItem( value, text );
      Items.Add( item );
      return item;
    }

    /// <summary>
    /// Ajoute un item à la liste des valeurs
    /// </summary>
    /// <param name="item">ietm à ajouter</param>
    /// <returns>l'item ajouté</returns>
    public EnumFlagsCheckedListBoxItem Add( EnumFlagsCheckedListBoxItem item ) {
      Items.Add( item );
      return item;
    }

    /// <summary>
    /// Surveille la modification des éléments cochés
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnItemCheck( ItemCheckEventArgs e ) {
      base.OnItemCheck( e );

      // ne pas propager les modifications si une mise à jour est déjà en cours
      if ( isUpdatingCheckStates ) return;

      // obtenir l'item modifié item
      EnumFlagsCheckedListBoxItem item = Items[ e.Index ] as EnumFlagsCheckedListBoxItem;

      // mettre à jour les autres éléments
      UpdateCheckedItems( item, e.NewValue );
    }

    /// <summary>
    /// Cocher ou non les items selon une valeur donnée
    /// </summary>
    /// <param name="value">vecteur de bits</param>
    protected void UpdateCheckedItems( int value ) {
      isUpdatingCheckStates = true;

      // parcourir tous les items
      for ( int i = 0 ; i < Items.Count ; i++ ) {
        EnumFlagsCheckedListBoxItem item = Items[ i ] as EnumFlagsCheckedListBoxItem;

        if ( item.value == 0 ) 
          SetItemChecked( i, value == 0 );
        else {
          bool isPresent = (item.value & value) == item.value && item.value != 0;
          SetItemChecked( i, isPresent );
        }
      }

      isUpdatingCheckStates = false;
    }

    /// <summary>
    /// Mise à jour des éléments de la list box
    /// </summary>
    /// <param name="composite">item qui vient d'être modifié</param>
    /// <param name="checkState">état coché ou non de l'item</param>
    protected void UpdateCheckedItems( EnumFlagsCheckedListBoxItem composite, CheckState checkState ) {

      // appeler directement si la valeur est 0
      if ( composite.value == 0 )
        UpdateCheckedItems( 0 );

      // obtenir la valeur déterminée par tous les items cochés
      int sum = GetCurrentValue();

      // combiner ou effacer les bits de l'item modifié
      if ( checkState == CheckState.Unchecked )
        sum = sum & (~composite.value);
      else
        sum |= composite.value;

      // mettre à jour tous les items en fonction de la valeur obtenue
      UpdateCheckedItems( sum );
    }

    /// <summary>
    /// Calcule la valeur produite par tous les items cochés
    /// </summary>
    /// <returns>la valeur obtenue</returns>
    public int GetCurrentValue() {
      int sum = 0;

      for ( int i = 0 ; i < Items.Count ; i++ ) 
        if ( GetItemChecked( i ) ) sum |= (Items[ i ] as EnumFlagsCheckedListBoxItem).value;

      return sum;
    }

    // Adds items to the checklistbox based on the members of the enum
    private void FillEnumMembers() {
      foreach ( string name in Enum.GetNames( enumType ) ) {
        object val = Enum.Parse( enumType, name );
        int intVal = (int) Convert.ChangeType( val, typeof( int ) );

        Add( intVal, name );
      }
    }

    // Checks/unchecks items based on the current value of the enum variable
    private void ApplyEnumValue() {
      int intVal = (int) Convert.ChangeType( enumValue, typeof( int ) );
      UpdateCheckedItems( intVal );
    }

    /// <summary>
    /// Obtient ou détermine la valeur d'énumération
    /// </summary>
    [DesignerSerializationVisibilityAttribute( DesignerSerializationVisibility.Hidden )]
    public Enum EnumValue {
      get {
        object e = Enum.ToObject( enumType, GetCurrentValue() );
        return (Enum) e;
      }
      set {
        Items.Clear();
        enumValue = value; // Store the current enum value
        enumType = value.GetType(); // Store enum type
        FillEnumMembers(); // Add items for enum members
        ApplyEnumValue(); // Check/uncheck items depending on enum value
      }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Eléments de la list box                                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Données associée à un élément de la boîte liste
  /// </summary>
  [ToolboxItem( false )]
  public class EnumFlagsCheckedListBoxItem {

    /// <summary>
    /// Vecteur de bits associé au membre de l'énumération
    /// </summary>
    public int value;

    /// <summary>
    /// Description du membre de l'énumération
    /// </summary>
    public string caption;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="value">valeur entière d'un membre de l'énumération</param>
    /// <param name="caption">description du membre de l'énumération</param>
    public EnumFlagsCheckedListBoxItem( int value, string caption ) {
      this.value = value;
      this.caption = caption;
    }

    /// <summary>
    /// Conversion en chaîne
    /// </summary>
    /// <returns>la conversion en chaîne de l'item</returns>
    public override string ToString() {
      return caption;
    }

    /// <summary>
    /// Retourne true si la valeur comporte un seul bit
    /// </summary>
    public bool IsFlag {
      get { return ((value & (value - 1)) == 0); }
    }

    /// <summary>
    /// Retourne true si cette valeur est un membre de la valeur composite
    /// </summary>
    /// <param name="composite">valeur composite</param>
    /// <returns>true si cette valeur est un membre de la valeur composite</returns>
    public bool IsMemberFlag( EnumFlagsCheckedListBoxItem composite ) {
      return (IsFlag && ((value & composite.value) == value));
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                               Editeur pour le mode conception                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Editeur pour le mode design
  /// </summary>
  public class EnumFlagsEditor : UITypeEditor {

    // la boîte d'affichage
    private EnumFlagsCheckedListBox box;

    /// <summary>
    /// Constructeur
    /// </summary>
    public EnumFlagsEditor() {
      box = new EnumFlagsCheckedListBox();
      box.BorderStyle = BorderStyle.None;
    }

    /// <summary>
    /// Redéfinition de la méthode EditValue pour afficher l'éditeur
    /// </summary>
    /// <param name="context">contexte apparent du type</param>
    /// <param name="provider">fournisseur de services pour le design</param>
    /// <param name="value">valeur à éditer</param>
    /// <returns>valeur obtenue après édition</returns>
    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value ) {
      if ( context == null || context.Instance == null || provider == null ) return value;

      // récupérer le service pour l'affichage
      IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService( typeof( IWindowsFormsEditorService ) );
      if ( service == null ) return value;

      // préparer l'affichage
      box.EnumValue = (Enum) Convert.ChangeType( value, context.PropertyDescriptor.PropertyType );

      // afficher le dialogue
      service.DropDownControl( box );
      return box.EnumValue;
    }

    /// <summary>
    /// Détermine le style d'affichage
    /// </summary>
    /// <param name="context">contexte apparent du type</param>
    /// <returns>le style d'affichage pour l'édition de la valeur</returns>
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context ) {
      return UITypeEditorEditStyle.DropDown;
    }
  }
}
