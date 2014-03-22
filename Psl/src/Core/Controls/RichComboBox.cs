/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 11 02 2011 : amélioration de l'affichage de l'image de la boîte d'édition
 */                                                                            // <wao never.end>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Psl.Windows;
using Psl.Drawing;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichComboBox                                   //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /*
  public class RichComboBox : ComboBox                                                             // <wao RichComboBox_abrégé.+header>
   */

  /// <summary>
  /// Extension de ComboBox pour la prise en charge d'items enrichis.
  /// </summary>
  [
  ToolboxItem( true ),
  ToolboxBitmap( typeof( RichComboBox ), "RichComboBox.bmp" ),
  DefaultProperty( "Items" ),
  DefaultEvent( "SelectedIndexChanged" ),
  Description( "Extension de ComboBox pour la prise en charge d'items enrichis" ),
  ]
  public partial class RichComboBox : ComboBox, IRichControlHost, IRichListPainterHost {           // <wao spécif>

    //
    // Paramètres par défaut
    //

    /// <summary>
    /// Hauteur par défaut exprimée en pixels d'un item
    /// </summary>
    public const int DefaultItemHeight = 15;

    /// <summary>
    /// Largeur par défaut exprimée en pixels d'un niveau d'indentation
    /// </summary>
    public const int DefaultIndentWidth = 10;

    /// <summary>
    /// Mode de peinture par défaut
    /// </summary>
    public const DrawMode DefaultDrawMode = DrawMode.OwnerDrawFixed;

    /// <summary>
    /// Rendu par défaut pour la propriété <see cref="FlatStyle"/>
    /// </summary>
    public const FlatStyle DefaultFlatStyle = FlatStyle.Standard;

    //
    // Champs
    //

    // collection des items
    private RichItemCollection items;

    // largeur des indentations en pixels
    private int indentWidth;

    // font par défaut pour les éléments ou null
    private Font itemFont = null;

    // couleur des textes par défaut pour les éléments
    private Color itemForeColor = Color.Empty;

    // couleur d'arrière-plan par défaut pour les items
    private Color itemBackColor = Color.Empty ;

    // détermine si les images doivent être affichées
    private bool imageShow = true;

    // détermine si l'image TextImage est gérée automatiquement (alignée sur celle de l'item sléectionné)
    private bool textImageSelectedItem = true;

    // liste d'images + image courante dans la boîte d'édition
    private readonly ImageIndexer textImage = new ImageIndexer();

    // menu contextuel associé à l'image
    private ContextMenuStrip textImageStripMenu;

    // la fenêtre de dessin du texte courant de la combo
    private NativeChildEdit editWindow;

    //
    // Constructeur                                                                      // <wao code.&comgroup RichComboBox_abrégé.+comitem>
    //

    /// <summary>
    /// Constructeur                                                                     
    /// </summary>
    public RichComboBox() {                                                              // <wao code.&body RichComboBox_abrégé.+body>
      editWindow = new NativeChildEdit( this );
      items = new RichItemCollection( this );
      Font = Control.DefaultFont;
      FlatStyle = DefaultFlatStyle;
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
    // Service
    //

    /// <summary>
    /// Obtient le mode actuel de l'orientation bidi
    /// </summary>
    /// <returns>true si l'orientation est de droite à gauche, false sinon</returns>
    [Browsable(false)]
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

      // forcer la recréation de l'image de boîte d'édition
      DoInvalidateEditImage();

    }

    /// <summary>
    /// Notifie un changement de l'image associée au texte en édition
    /// </summary>
    protected void InvalidateTextImage() {
      DoInvalidateEditImage();
    }

    /// <summary>
    /// Provoque l'invalidation complète des caches et de l'affichage
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

      // provoquer la réfection complète de la peinture
      if ( editWindow != null && editWindow.Handle != IntPtr.Zero )
        Win.RedrawWindow(
          editWindow.Handle,
          IntPtr.Zero,
          IntPtr.Zero,
          Win.RDW_INVALIDATE | Win.RDW_FRAME | Win.RDW_ERASE | Win.RDW_ALLCHILDREN
          );
    }

    //
    // Délégations de la fenêtre native d'édition
    //

    /// <summary>
    /// Traite l'enfoncement d'un bouton souris sur la fenêtre d'édition
    /// </summary>
    /// <param name="m">message intercepté par WM_LBUTTONDOWN ou WM_LBUTTONRIGHT</param>
    /// <param name="button">indique le bouton concerné</param>
    /// <returns>true si le message a été pris en charge, false sinon</returns>
    protected virtual bool HandleEditButtonDown( ref Message m, MouseButtons button ) {
      if (!Enabled) return false ;

      Point location = new Point( Win.Util.SignedLOWORD( (int) m.LParam ), Win.Util.SignedHIWORD( (int) m.LParam ) );
      if ( DoGetHitTestAt( location ) == RichControlHitTest.Image ) {

        HandledEventArgs args = new HandledEventArgs( false );
        if ( button == MouseButtons.Left )
          OnClickImageLeft( args );
        else
          OnClickImageRight( args );

        if ( args.Handled ) return false; // true;

        if ( button == MouseButtons.Right && textImageStripMenu != null ) {
          textImageStripMenu.Show( this, location );
          return false; // true;
        }

        return false; // args.Handled;
      }

      return false;
    }

    /*
    protected virtual bool HandleEditButtonDown( ref Message m, MouseButtons button ) {
      if ( !Enabled ) return false;

      Point location = new Point( Win.Util.SignedLOWORD( (int) m.LParam ), Win.Util.SignedHIWORD( (int) m.LParam ) );
      if ( DoGetHitTestAt( location ) != RichControlHitTest.Image ) return false;

      HandledEventArgs args = new HandledEventArgs( false );
      if ( button == MouseButtons.Left )
        OnClickImageLeft( args );
      else
        OnClickImageRight( args );

      if ( args.Handled ) return true;

      if ( button == MouseButtons.Right && textImageStripMenu != null ) {
        textImageStripMenu.Show( this, location );
        return true;
      }

      return args.Handled;
    }
    */

    //
    // Méthode redéfinies
    //

    /// <summary>
    /// Redéfinition de la méthode WndProc pour traiter certains messages
    /// </summary>
    /// <param name="m">message</param>
    protected override void WndProc( ref Message m ) {
      //if (m.Msg != Win.CB_GETCURSEL) Psl.Tracker.Tracker.Track( "RichCombo.WndProc : message : " + m.Msg.ToString( "X" ) );
      switch ( m.Msg ) {

        case Win.WM_CTLCOLORLISTBOX:
          if ( DrawMode != DrawMode.Normal ) {
            Win.SetTextColor( m.WParam, ColorTranslator.ToWin32( ItemForeColor ) );
            Win.SetBkColor( m.WParam, ColorTranslator.ToWin32( ItemBackColor ) );
            IntPtr backBrush = Win.CreateSolidBrush( ColorTranslator.ToWin32( ItemBackColor ) );
            m.Result = backBrush;
            return;
          }
          base.WndProc( ref m );
          break;

        case Win.CB_RESETCONTENT:
          if ( textImageSelectedItem && DropDownStyle == ComboBoxStyle.DropDownList ) {
            textImage.ImageToken = null;
            InvalidateTextImage();
          }
          base.WndProc( ref m );
          break;

        default:
          base.WndProc( ref m );
          break;
      }
    }

    //protected override void OnHandleDestroyed( EventArgs e ) {
    //  base.OnHandleDestroyed( e );
    //}

    /// <summary>
    /// Traitements à appliquer lorsque le handle du contrôle est créé
    /// </summary>
    /// <remarks>
    /// Laisser d'abord la combo standard faire son travail, ensuite compléter ou modifier
    /// </remarks>
    /// <param name="e">descripteur d'événement</param>
    protected override void OnHandleCreated( EventArgs e ) {
      base.OnHandleCreated( e );
      DoNativeEditAssignHandle( editWindow );
      InvalidateCaches();
    }

    /// <summary>
    /// Mise à jour de la boîte d'édition quand la sélection change.
    /// </summary>
    /// <param name="e">descripteur d'événement</param>
    protected override void OnSelectedIndexChanged( EventArgs e ) {
      if ( textImageSelectedItem ) {
        textImage.ImageToken = SelectedIndex < 0 ? null : SelectedItem.ImageToken;
        InvalidateTextImage();
      }
      base.OnSelectedIndexChanged( e );
    }

    /// <summary>
    /// Redéfinition permettant d'exposer l'événement <see cref="InputKey"/>
    /// </summary>
    /// <remarks>
    /// Il s'agit en particulier de parvenir à récupérer les touches Return et Escape pour
    /// les soustaire au protocole "strip" aligné sur celui des menus
    /// </remarks>
    /// <param name="keyData">code de touche</param>
    /// <returns>true la touche est à considérer comme un caractère d'entrée du contrôle</returns>
    protected override bool IsInputKey( Keys keyData ) {
      InputKeyEventArgs args = new InputKeyEventArgs( keyData, base.IsInputKey( keyData ) );
      OnInputKey( args );
      return args.IsInputKey;
    }

    /// <summary>
    /// Redéfinition permettant d'éviter le filtrage autoritaire de certains caractères
    /// </summary>
    /// <remarks>
    /// Associée à la méthode <see cref="IsInputKey"/> (donc à l'événement <see cref="InputKey"/>,
    /// cette redéfinition contribue à soustraire les caractères Return et Escape au protocole
    /// des contrôles "strip" aligné sur celui des menus
    /// </remarks>
    /// <param name="msg">message Windows</param>
    /// <param name="keyData">code de touche</param>
    /// <returns>false si la touche n'a pas été prise en charge comme caractère de commande</returns>
    protected override bool ProcessCmdKey( ref Message msg, Keys keyData ) {
      return IsInputKey( keyData ) ? false : base.ProcessCmdKey( ref msg, keyData );
    }

    /// <summary>
    /// Traitement à effectuer lorsque la font du contrôle est modifiée
    /// </summary>
    /// <remarks>
    /// L'invalidation est liée au fait que la font du contrôle constitue la valeur par défaut 
    /// pour la font par défaut des items si <see cref="ItemFont"/> est null.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnFontChanged( EventArgs e ) {
      base.OnFontChanged( e );
      InvalidateAll();
    }

    //
    // Déclenchement centralisé des événements
    //

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ClickImageLeft"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnClickImageLeft( HandledEventArgs e ) {
      if ( ClickImageLeft == null ) return;
      e.Handled = true;
      ClickImageLeft( this, e );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ClickImageRight"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnClickImageRight( HandledEventArgs e ) {
      if ( ClickImageRight == null ) return;
      e.Handled = true;
      ClickImageRight( this, e );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="InputKey"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnInputKey( InputKeyEventArgs e ) {
      if ( InputKey != null ) InputKey( this, e );
    }

    //
    // Fonctionnalités exposées                                                          // <wao code.&comgroup>
    //

    /// <summary>
    /// Obtient la localisation associée à un point
    /// </summary>
    /// <param name="location">coordonnées client du point</param>
    /// <returns>la localisation du point dans la boîte d'édition</returns>
    public RichControlHitTest GetHitTestAt( Point location ) {                             // <wao code.&body>
      return DoGetHitTestAt( location );
    }

    //
    // Propriétés exposées                                                               // <wao code.&comgroup RichComboBox_abrégé.+comitem>
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
    TypeConverterAttribute( typeof( RichItemCollection ) ),
    Description( "Obtient une référence sur la collection des éléments" ),
    ]
    public new RichItemCollection Items {                                                // <wao code.&body:ro RichComboBox_abrégé.+body:ro>
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
    public new RichItem SelectedItem {                                                   // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
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
    public ImageList ImageList {                                                         // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImage.ImageList; }
      set {
        if ( value == textImage.ImageList ) return;
        textImage.ImageList = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Obtient ou détermine l'image (type Image) associée à la boîte d'édition
    /// </summary>
    [
     Browsable( false ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
     Description( "Obtient ou détermine l'image (type Image) associée à la boîte d'édition" ),
    ]
    public Image TextImage {                                                             // <wao code.&body:rw>
      get { return textImage.Image; }
      set { TextImageObject = value; }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image de la boîte d'édition en tant qu'objet Image
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( null ),
     RefreshProperties( RefreshProperties.Repaint ),
     Description( "Obtient ou détermine le token d'image de la boîte d'édition en tant qu'objet Image" ),
    ]
    public Image TextImageObject {                                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImage.ImageObject; }
      set {
        if ( value == textImage.ImageToken ) return;
        textImage.ImageObject = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image de la boîte d'édition en tant qu'index dans la liste d'images
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( -1 ),
     RelatedImageList( "ImageList" ),
     TypeConverter( "System.Windows.Forms.NoneExcludedImageIndexConverter, " + AssemblyRef.SystemWindowsForms ),
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ), 
     RefreshProperties(RefreshProperties.Repaint),
     Description( "Obtient ou détermine le token d'image de la boîte d'édition en tant qu'index dans la liste d'images" ),
    ]
    public int TextImageIndex {                                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImage.ImageIndex; }
      set {
        if ( value == textImage.ImageIndex ) return;
        textImage.ImageIndex = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image de la boîte d'édition en tant que clé dans la liste d'images
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( "" ),
     RelatedImageList( "ImageList" ),
     TypeConverter( typeof( ImageKeyConverter ) ), 
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
     RefreshProperties( RefreshProperties.Repaint ),
     Description( "Obtient ou détermine le token d'image de la boîte d'édition en tant que clé dans la liste d'images" ),
    ]
    public string TextImageKey {                                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImage.ImageKey; }
      set {
        if ( value == textImage.ImageKey ) return;
        textImage.ImageKey = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image associé à la boîte d'édition
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine le token d'image associé à la boîte d'édition" ),
    ]
    public object TextImageToken {                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImage.ImageToken; }
      set {
        if ( value == textImage.ImageToken ) return;
        textImage.ImageToken = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou détermine si l'image de la boîte de saisie est automatiquement celle de l'item sélectionné.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    Description( "Obtient ou détermine si l'image de la boîte d'édition est automatiquement celle de l'item sélectionné" )
    ]
    public bool TextImageSelectedItem {                                        // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get {
        return textImageSelectedItem;
      }
      set {
        textImageSelectedItem = value;
      }
    }

    /// <summary>
    /// Obtient ou détermine le menu contextuel associé à la zone image de la boîte d'édition.
    /// </summary>
    [
     Category( "Behavior" ),
     DefaultValue( null ),
     Description( "Obtient ou détermine le menu contextuel associé à la zone image de la boîte d'édition" ),
    ]
    public ContextMenuStrip TextImageStripMenu {                                 // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return textImageStripMenu; }
      set { textImageStripMenu = value; }
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeFont() {
      return Font != null && !Font.Equals( Control.DefaultFont );
    }

    /// <summary>
    /// Indique si la propriété <see cref="ItemFont"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemFont() {
      return itemFont != null;
    }

    /// <summary>
    /// Obtient ou détermine la font par défaut des éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la font par défaut des éléments" ),
    ]
    public Font ItemFont {                                                               // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return itemFont == null ? DefaultFont : itemFont; }
      set {
        if ( value == null || value.Equals( DefaultFont ) ) value = null;
        if ( value == itemFont ) return;
        itemFont = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Indique si la propriété <see cref="ItemBackColor"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemBackColor() {
      return itemBackColor != Color.Empty;
    }

    /// <summary>
    /// Obtient ou détermine la couleur d'arrière-plan pour les éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la couleur d'arrière-plan pour les éléments" ),
    ]
    public Color ItemBackColor {                                                         // <wao code.&body:rw>
      get { return itemBackColor == Color.Empty ? BackColor : itemBackColor; }
      set {
        if ( value == BackColor ) value = Color.Empty;
        if ( value == itemBackColor ) return;
        itemBackColor = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Indique si la propriété <see cref="ItemForeColor"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemForeColor() {
      return itemForeColor != Color.Empty;
    }

    /// <summary>
    /// Obtient ou détermine la couleur d'avant-plan pour les éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la couleur d'avant-plan pour les éléments" ),
    ]
    public Color ItemForeColor {                                                         // <wao code.&body:rw>
      get { return itemForeColor == Color.Empty ? ForeColor : itemForeColor; }
      set {
        if ( value == ForeColor ) value = Color.Empty;
        if ( value == itemForeColor ) return;
        itemForeColor = value;
        InvalidateAll();
      }
    }

    //
    // Evénements                                                                        // <wao code.&comgroup RichComboBox_abrégé.+comitem>
    //

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageLeft;                                     // <wao spécif RichComboBox_abrégé>

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de droite se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de droite se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageRight;                                    // <wao spécif RichComboBox_abrégé>

    /// <summary>
    /// Se produit lorsqu'il faut déterminer si un caractère est un caractère d'entrée
    /// </summary>
    [
    Description( "Se produit lorsqu'il faut déterminer si un caractère est un caractère d'entrée" ),
    ]
    public event InputKeyEventHandler InputKey;                                          // <wao spécif>
  }                                                                                      // <wao spécif>

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichComboBox                                   //
  //                                  Peinture de la boîte liste                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {
  
    // référence sur le peintre des items de la liste *** géré par nécessité ***
    private IRichListPainter richListPainter ;

    /// <summary>
    /// Obtient un peintre pour les items de la liste déroulante
    /// </summary>
    /// <returns>la référence sur le peintre</returns>
    protected virtual IRichListPainter CreateRichListPainter() {
      return new RichListPainter( this ) ;
    }

    /// <summary>
    /// Obtient la réfence sur le peintre courant des items de la liste
    /// </summary>
    /// <remarks>
    /// La référence sur le peintre courant est gérée par nécessité.
    /// </remarks>
    protected IRichListPainter RichListPainter {
      get {
        if ( richListPainter == null ) 
          richListPainter = CreateRichListPainter();
        return richListPainter;
      }
    }

    /// <summary>
    /// Détermine le rectangle d'affichage d'un item en mode OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMeasureItem( MeasureItemEventArgs e ) {
      if ( DataSource == null && 0 <= e.Index && e.Index < Items.Count ) 
        RichListPainter.MeasureRichItem( e, Items[ e.Index ] );
      base.OnMeasureItem( e );
    }

    /// <summary>
    /// Effectue la peinture d'un item en mode OwnderDrawFixed ou OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDrawItem( DrawItemEventArgs e ) {
      DrawItemEventArgs args = new DrawItemEventArgs( e.Graphics, e.Font, e.Bounds, e.Index, e.State, itemForeColor, ItemBackColor );
      if ( DataSource == null && 0 <= e.Index && e.Index < Items.Count ) 
        RichListPainter.DrawRichItem( args, Items[ e.Index ] );
      base.OnDrawItem( args );
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichComboBox                                   //
  //                                 Peinture de la boîte d'édition                              //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {

    // référence sur le peintre des items de la liste *** géré par nécessité ***
    private IRichEditPainter richEditPainter ;

    /// <summary>
    /// Obtient un peintre pour les items de la liste déroulante
    /// </summary>
    /// <returns>la référence sur le peintre</returns>
    protected virtual IRichEditPainter CreateRichEditPainter() {
      return new RichEditPainter( this ) ;
    }

    /// <summary>
    /// Obtient la réfence sur le peintre courant des items de la liste
    /// </summary>
    /// <remarks>
    /// La référence sur le peintre courant est gérée par nécessité.
    /// </remarks>
    protected IRichEditPainter RichEditPainter {
      get {
        if ( richEditPainter == null ) 
          richEditPainter = CreateRichEditPainter();
        return richEditPainter;
      }
    }

    /// <summary>
    /// Sous-classer la fenêtre d'édition
    /// </summary>
    private void DoNativeEditAssignHandle( NativeChildEdit native ) {
      if ( native == null ) return;

      // libérer le handle de la fenêtre d'édition courante
      if ( !native.Handle.Equals( IntPtr.Zero ) ) native.ReleaseHandle();

      // assigner le handle actuel de la fenêtre d'édition
      Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );
      native.AssignHandle( infos.hwndEdit );
    }

    /// <summary>
    /// Obtient la localisation d'un point relativement aux zones de la boîte d'édition
    /// </summary>
    /// <param name="location">coordonnées du point en coordonnées client</param>
    /// <returns>l'indication de la zone de la boîte liste concernée</returns>
    protected virtual RichControlHitTest DoGetHitTestAt( Point location ) {
      if ( !ClientRectangle.Contains( location ) ) return RichControlHitTest.None;

       // récupérer les marges au niveau du composant
      ulong margins = 0;
      if (editWindow != null && editWindow.Handle != IntPtr.Zero ) margins = (ulong) Win.SendMessage( editWindow.Handle, Win.EM_GETMARGINS, 0, 0 );
      int leftMargin = (int) (margins & 0x0000FFFF);
      int rightMargin = (int) ((margins & 0xFFFF0000) >> 16);

      bool inLeftPart = location.X < (IsRightToLeft ? ClientRectangle.Right - rightMargin : leftMargin);
      return inLeftPart && !IsRightToLeft || !inLeftPart && IsRightToLeft ? RichControlHitTest.Image : RichControlHitTest.Text;
    }

    /// <summary>
    /// Invalide les informations gérées en cache au niveau du peintre de la fenêtre d'édition
    /// </summary>
    protected void DoInvalidateEditImage() {
      Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );
      RichEditPainter.InvalidateEditImage( infos.hwndEdit );
    }

    /// <summary>
    /// Prend en charge un message Wm_Paint pour la fenêtre d'édition
    /// </summary>
    /// <param name="m">message</param>
    private void HandleEditPaint( ref Message m ) {
      //Psl.Tracker.Tracker.Track( "RichComboBox : HandleEditPaint" );
      if ( DrawMode == DrawMode.Normal ) return;

      // obtenir une surface de dessin
      using ( Graphics graphics = Graphics.FromHwnd( m.HWnd ) ) {

        // infos combo
        Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );

        // déterminer le rectangle de l'item
        RectangleF itemRect = DropDownStyle == ComboBoxStyle.DropDownList ?
          RectangleF.FromLTRB( infos.rcItem.left, infos.rcItem.top, infos.rcItem.right, infos.rcItem.bottom ) :
          graphics.VisibleClipBounds;

        switch ( DropDownStyle ) {

          case ComboBoxStyle.Simple :
          case ComboBoxStyle.DropDown :
            RichEditPainter.DrawImage( graphics, itemRect, TextImage );
            break;

          case ComboBoxStyle.DropDownList :
            RichEditPainter.DrawContent( graphics, itemRect, TextImage, Text, Focused && !DroppedDown );
            break;
        }
      }
    }

  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichComboBox                                   //
  //                      fenêtre native pour sous-classer la fenêtre d'édition                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {

    private class NativeChildEdit : NativeWindow {

      // Combo propriétaire de la fenêtre d'édition
      private RichComboBox owner = null;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="owner">composant <see cref="RichComboBox" />propriétaire</param>
      internal NativeChildEdit( RichComboBox owner ) {
        this.owner = owner;
      }

      protected override void WndProc( ref Message m ) {
        //Psl.Tracker.Tracker.Track( "RichCombo.NativeWindow.WndProc : message : " + m.Msg.ToString( "X" ) );
        switch ( m.Msg ) {

          // workaround pour forcer le réaffichage de l'image
          case Win.WM_KEYUP:
          case Win.WM_MOUSEFIRST:
            base.WndProc( ref m );
            if ( owner.DropDownStyle != ComboBoxStyle.DropDownList ) owner.HandleEditPaint( ref m );
            break;

          case Win.WM_PAINT:
            base.WndProc( ref m );
            owner.HandleEditPaint( ref m );
            break;

          case Win.WM_SETCURSOR:
            if ( owner.DoGetHitTestAt( owner.PointToClient( Control.MousePosition ) ) == RichControlHitTest.Image )
              Win.SetCursor( Cursors.Arrow.Handle );
            else
              base.WndProc( ref m );
            break;

          case Win.WM_LBUTTONDOWN:
            if ( !owner.HandleEditButtonDown( ref m, MouseButtons.Left ) )
              base.WndProc( ref m );
            break;

          case Win.WM_RBUTTONDOWN:
            if ( !owner.HandleEditButtonDown( ref m, MouseButtons.Right ) )
              base.WndProc( ref m );
            break;

          default:
            base.WndProc( ref m );
            break;
        }
      }
    }
  }
}
