/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 23 03 2011 : adjonction de la propriété AutoRightSelect
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using Psl.Controls;
using Psl.Windows;
using Psl.Drawing;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichListBox                                    //
  //                                Base de l'extension de ListBox                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension de <see cref="ListBox"/> pour une collection d'éléments riches <see cref="RichItem"/>
  /// </summary>
  [
  ToolboxItem( true ),
  ToolboxBitmap( typeof( RichListBox ), "RichListBox.bmp" ),
  DefaultProperty( "Items" ),
  DefaultEvent( "SelectedIndexChanged" ),
  Description( "Extension de ListBox pour la prise en charge d'items enrichis" ),
  ]
  public partial class RichListBox : ListBox, IRichControlHost, IRichListPainterHost {   // <wao code.&header>

    //
    // Paramètres par défaut
    //

    /// <summary>
    /// Largeur par défaut (exprimée en pixels) d'un niveau d'indentation
    /// </summary>
    public const int DefaultIndentWidth = 10;

    /// <summary>
    /// Mode de peinture par défaut
    /// </summary>
    public const DrawMode DefaultDrawMode = DrawMode.OwnerDrawFixed;

    //
    // Champs
    //

    // collection des items
    private RichItemCollection items;

    // largeur des indentations en pixels
    private int indentWidth;

    // liste d'images 
    private ImageList imageList;

    // détermine si les images doivent être affichées
    private bool imageShow = true;

    // Constructeur                                                                      // <wao code.&comgroup>

    /// <summary>
    /// Constructeur                                                                     
    /// </summary>
    public RichListBox() {                                                               // <wao code.&body>
      items = new RichItemCollection( this );
      Font = Control.DefaultFont;
      DrawMode = DefaultDrawMode;
      ItemHeight = DefaultItemHeight;
      IndentWidth = DefaultIndentWidth;
    }

    //
    // Gestion des éléments (interface IRichItemHost)
    //

    IList IRichControlHost.Items {
      get { return base.Items; }
    }

    void IRichControlHost.OnItemChanged( RichItem item ) {
      if ( item == null ) return;

      int index = Items.IndexOf( item );
      if ( index != -1 )
        RefreshItem( index );
    }

    //
    // Peinture des éléments (interface IRichControlHost)
    //

    Font IRichControlHost.ItemFont {
      get { return Font; }
    }

    Color IRichControlHost.ItemBackColor {
      get { return BackColor; }
    }

    Color IRichControlHost.ItemForeColor {
      get { return ForeColor; }
    }

    //
    // Service
    //

    /// <summary>
    /// Obtient le mode actuel de l'orientation bidi
    /// </summary>
    /// <returns>true si l'orientation est de droite à gauche, false sinon</returns>
    [Browsable( false )]
    public bool IsRightToLeft {
      get {
        return (RightToLeft == RightToLeft.Yes)
          || (RightToLeft == RightToLeft.Inherit && Parent != null && Parent.RightToLeft == RightToLeft.Yes);
      }
    }

    //
    // Mises à jour
    //

    /// <summary>
    /// Provoque l'invalidation de toutes les valeurs gérées en cache
    /// </summary>
    protected void InvalidateCaches() {

      // forcer les items à mettre à jour leur font
      foreach ( RichItem item in Items ) {
        item.InvalidateFont();
      }
    }

    /// <summary>
    /// Invalide l'ensemble du contrôle, y compris les caches et la zone non-client
    /// </summary>
    /// <remarks>
    /// Control.Invalidate(true) doesn't invalidate the non-client region
    /// </remarks>
    protected void InvalidateAll() {

      // invalider toutes les valeurs en caches
      InvalidateCaches();

      // provoquer la réfection complète de la peinture
      if ( IsHandleCreated )
        Win.RedrawWindow(
          Handle,
          IntPtr.Zero,
          IntPtr.Zero,
          Win.RDW_INVALIDATE | Win.RDW_FRAME | Win.RDW_ERASE | Win.RDW_ALLCHILDREN
          );
    }

    //
    // Propriétés exposées                                                               // <wao code.&comgroup>
    //

    /// <summary>
    /// Obtient ou détermine le mode de peinture du contrôle
    /// </summary>
    /// <remarks>
    /// Propriété redéclarée pour ajuster la valeur par défaut de la sérialisation
    /// </remarks>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultDrawMode ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine le mode de peinture du contrôle" ),
    ]
    public new DrawMode DrawMode {
      get { return base.DrawMode; }
      set { base.DrawMode = value; }
    }

    /// <summary>
    /// Obtient une référence sur la collection des éléments.
    /// </summary>
    [
    Category( "Data" ),
    Localizable( true ),
    MergableProperty( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Editor( "System.ComponentModel.Design.CollectionEditor, " + Psl.AssemblyRef.SystemDesign, typeof( UITypeEditor ) ), // Editor( typeof( CollectionEditor ), typeof( UITypeEditor ) ),
    TypeConverter( typeof( RichItemCollection ) ),
    Description( "Obtient une référence sur la collection des éléments" ),
    ]
    public new RichItemCollection Items {                                                // <wao code.&body:rw>
      get { return items; }
    }

    /// <summary>
    /// Obtient ou détermine l'élément actuellement sélectionné.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine l'élément actuellement sélectionné" ),
    ]
    public new RichItem SelectedItem {                                                   // <wao code.&body:rw>
      get { return base.SelectedItem as RichItem; }
      set { base.SelectedItem = value; }
    }

    /// <summary>
    /// Obtient ou détermine si les images doivent être affichées.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine si les images doivent être affichées" ),
    ]
    public bool ImageShow {                                                              // <wao code.&body:rw>
      get { return imageShow; }
      set {
        if ( value == imageShow ) return;
        imageShow = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Obtient ou détermine la liste d'image associée aux éléments de la collection.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( null ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine la liste d'image associée aux éléments de la collection" )
    ]
    public ImageList ImageList {                                                         // <wao code.&body:rw>
      get { return imageList; }
      set {
        if ( value == imageList ) return;
        imageList = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Obtient ou détermine la largeur de l'indentation (exprimée en pixels) d'un niveau d'indentation.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultIndentWidth ),
    Description( "Obtient ou détermine la largeur de l'indentation (exprimée en pixels) d'un niveau d'indentation" )
    ]
    public int IndentWidth {                                                             // <wao code.&body:rw>
      get { return indentWidth; }
      set {
        if ( value == indentWidth ) return;
        indentWidth = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Obtient ou détermine la hauteur fixe (exprimée en pixels) d'un item
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultItemHeight ),
    Description( "Obtient ou détermine la hauteur fixe (exprimée en pixels) d'un item" )
    ]
    public new int ItemHeight {
      get { return base.ItemHeight; }
      set {
        if ( value == ItemHeight ) return;
        Invalidate();
        base.ItemHeight = value;
      }
    }

    /// <summary>
    /// Indique si la propriété <see cref="Font"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public virtual bool ShouldSerializeFont() {
      return Font != null && !Font.Equals( Control.DefaultFont );
    }
  }                                                                                      // <wao code.&ender>

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichListBox                                    //
  //                                     Peinture de la liste                                    //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichListBox {
  
    // référence sur le peintre des items de la liste *** géré par nécessité ***
    private IRichListPainter richItemPainter ;

    /// <summary>
    /// Obtient un peintre pour les items de la liste
    /// </summary>
    /// <returns>la référence sur le peintre</returns>
    protected virtual IRichListPainter CreateRichItemPainter() {
      return new RichListPainter( this ) ;
    }

    /// <summary>
    /// Obtient la réfence sur le peintre courant des items de la liste
    /// </summary>
    /// <remarks>
    /// La référence sur le peintre courant est gérée par nécessité.
    /// </remarks>
    protected IRichListPainter RichItemPainter {
      get {
        if ( richItemPainter == null )
          richItemPainter = CreateRichItemPainter();
        return richItemPainter;
      }
    }

    /// <summary>
    /// Détermine le rectangle d'affichage d'un item en mode OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMeasureItem( MeasureItemEventArgs e ) {
      if ( DataSource == null && 0 <= e.Index && e.Index < Items.Count ) 
        RichItemPainter.MeasureRichItem( e, Items[ e.Index ] );
      base.OnMeasureItem( e );
    }

    /// <summary>
    /// Effectue la peinture d'un item en mode OwnderDrawFixed ou OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDrawItem( DrawItemEventArgs e ) {
      DrawItemEventArgs args = new DrawItemEventArgs( e.Graphics, e.Font, e.Bounds, e.Index, e.State, ForeColor, BackColor );
      if ( DataSource == null && 0 <= e.Index && e.Index < Items.Count ) 
        RichItemPainter.DrawRichItem( args, Items[ e.Index ] );
      base.OnDrawItem( args );
    }
  }

  // Adjonction du 23 03 2011
  partial class RichListBox {

    private bool autoRightSelect = true ;

    /// <summary>
    /// Obtient ou détermine si l'élément cible d'un clic souris droit est automatiquement sélectionné.
    /// </summary>
    [
     DefaultValue( true ),
     Description( "Obtient ou détermine si l'élément cible d'un clic souris droit est automatiquement sélectionné" ),
    ]
    public bool AutoRightSelect {
      get { return autoRightSelect; }
      set { autoRightSelect = value; }
    }

    /// <summary>
    /// Redéfinition de <see cref="Control.OnMouseUp"/> pour effectuer la sélection sur clic droit.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    /// <seealso cref="AutoRightSelect"/>
    protected override void OnMouseUp( MouseEventArgs e ) {
      
      if ( e.Button == MouseButtons.Right ) {
        int index = IndexFromPoint( e.Location );
        if ( index == -1 ) return;
        bool selected = GetSelected( index );
        if ( !selected ) {
          SelectedIndices.Clear();
          SelectedIndex = index;
        }
      }

      base.OnMouseUp( e );
    }
  }

}
