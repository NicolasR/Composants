/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
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
  /// Extension du contr�le <see cref="CheckedListBox"/> pour l'affichage des items
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
    /// Ajoute une valeur enti�re et sa description
    /// </summary>
    /// <param name="value">valeur</param>
    /// <param name="text">description</param>
    /// <returns>l'item cr�� et ajout�</returns>
    public EnumFlagsCheckedListBoxItem Add( int value, string text ) {
      EnumFlagsCheckedListBoxItem item = new EnumFlagsCheckedListBoxItem( value, text );
      Items.Add( item );
      return item;
    }

    /// <summary>
    /// Ajoute un item � la liste des valeurs
    /// </summary>
    /// <param name="item">ietm � ajouter</param>
    /// <returns>l'item ajout�</returns>
    public EnumFlagsCheckedListBoxItem Add( EnumFlagsCheckedListBoxItem item ) {
      Items.Add( item );
      return item;
    }

    /// <summary>
    /// Surveille la modification des �l�ments coch�s
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnItemCheck( ItemCheckEventArgs e ) {
      base.OnItemCheck( e );

      // ne pas propager les modifications si une mise � jour est d�j� en cours
      if ( isUpdatingCheckStates ) return;

      // obtenir l'item modifi� item
      EnumFlagsCheckedListBoxItem item = Items[ e.Index ] as EnumFlagsCheckedListBoxItem;

      // mettre � jour les autres �l�ments
      UpdateCheckedItems( item, e.NewValue );
    }

    /// <summary>
    /// Cocher ou non les items selon une valeur donn�e
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
    /// Mise � jour des �l�ments de la list box
    /// </summary>
    /// <param name="composite">item qui vient d'�tre modifi�</param>
    /// <param name="checkState">�tat coch� ou non de l'item</param>
    protected void UpdateCheckedItems( EnumFlagsCheckedListBoxItem composite, CheckState checkState ) {

      // appeler directement si la valeur est 0
      if ( composite.value == 0 )
        UpdateCheckedItems( 0 );

      // obtenir la valeur d�termin�e par tous les items coch�s
      int sum = GetCurrentValue();

      // combiner ou effacer les bits de l'item modifi�
      if ( checkState == CheckState.Unchecked )
        sum = sum & (~composite.value);
      else
        sum |= composite.value;

      // mettre � jour tous les items en fonction de la valeur obtenue
      UpdateCheckedItems( sum );
    }

    /// <summary>
    /// Calcule la valeur produite par tous les items coch�s
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
    /// Obtient ou d�termine la valeur d'�num�ration
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
  //                                    El�ments de la list box                                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Donn�es associ�e � un �l�ment de la bo�te liste
  /// </summary>
  [ToolboxItem( false )]
  public class EnumFlagsCheckedListBoxItem {

    /// <summary>
    /// Vecteur de bits associ� au membre de l'�num�ration
    /// </summary>
    public int value;

    /// <summary>
    /// Description du membre de l'�num�ration
    /// </summary>
    public string caption;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="value">valeur enti�re d'un membre de l'�num�ration</param>
    /// <param name="caption">description du membre de l'�num�ration</param>
    public EnumFlagsCheckedListBoxItem( int value, string caption ) {
      this.value = value;
      this.caption = caption;
    }

    /// <summary>
    /// Conversion en cha�ne
    /// </summary>
    /// <returns>la conversion en cha�ne de l'item</returns>
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

    // la bo�te d'affichage
    private EnumFlagsCheckedListBox box;

    /// <summary>
    /// Constructeur
    /// </summary>
    public EnumFlagsEditor() {
      box = new EnumFlagsCheckedListBox();
      box.BorderStyle = BorderStyle.None;
    }

    /// <summary>
    /// Red�finition de la m�thode EditValue pour afficher l'�diteur
    /// </summary>
    /// <param name="context">contexte apparent du type</param>
    /// <param name="provider">fournisseur de services pour le design</param>
    /// <param name="value">valeur � �diter</param>
    /// <returns>valeur obtenue apr�s �dition</returns>
    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value ) {
      if ( context == null || context.Instance == null || provider == null ) return value;

      // r�cup�rer le service pour l'affichage
      IWindowsFormsEditorService service = (IWindowsFormsEditorService) provider.GetService( typeof( IWindowsFormsEditorService ) );
      if ( service == null ) return value;

      // pr�parer l'affichage
      box.EnumValue = (Enum) Convert.ChangeType( value, context.PropertyDescriptor.PropertyType );

      // afficher le dialogue
      service.DropDownControl( box );
      return box.EnumValue;
    }

    /// <summary>
    /// D�termine le style d'affichage
    /// </summary>
    /// <param name="context">contexte apparent du type</param>
    /// <returns>le style d'affichage pour l'�dition de la valeur</returns>
    public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context ) {
      return UITypeEditorEditStyle.DropDown;
    }
  }
}
