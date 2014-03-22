/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 10 05 2010 : ré-exposition des propriétés et événements liés au protocole drag-and-drop (cf. ToolStripComboBoxHost)
 * 16 05 2010 : masquage des propriétés Image, ImageKey et ImageIndex de ToolStripItem, redirigées sur TextImageToken
 * 
 * Note du 2010 05 10
 * Lorsqu'on recompile la librairie au sein d'une solution qui utilise des composants enveloppés
 * dans des ToolStripControlHost (ou dérivés), les barres d'outils ToolStrip comportant de tels 
 * composants se figent à l'issue de la recompilation. Il semble que ce soit une bogue connue,
 * et qu'il n'y ait rien d'autre à faire que fermer puis rouvrir le concepteur concerné.
 * 
 */                                                                            // <wao never.end>

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Psl.Windows;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Wrapper ToolStrip pour RichComboBox                             //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Boîte combinée riche intégrable dans les composants "strip"
  /// </summary>
  [
  DefaultProperty( "Items" ),
  DefaultEvent( "SelectedIndexChanged" ),
  ToolboxBitmap( typeof( RichComboBox ), "RichComboBox.bmp" ),
  ToolStripItemDesignerAvailability( ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip | ToolStripItemDesignerAvailability.StatusStrip ),
  ]
  public partial class ToolStripRichComboBox : ToolStripComboBoxHost {

    // clés pour le relai des événements
    internal static readonly object EventClickImageLeft = new object();
    internal static readonly object EventClickImageRight = new object();
    internal static readonly object EventInputKey = new object();

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Constructeur apportant la <see cref="RichComboBox"/> embarquée
    /// </summary>
    /// <param name="combo">référence non null sur la boîte combinée</param>
    public ToolStripRichComboBox( RichComboBox combo ) : base( combo ) {
      ComboBox.FlatStyle = FlatStyle.Popup;
      ((RichComboBoxStrip) ComboBox).Owner = this;
    }

    /// <summary>
    /// Constructeur pour embarquer une <see cref="RichComboBox"/> standard
    /// </summary>
    public ToolStripRichComboBox() : this( new RichComboBoxStrip() ) {
    }

    //
    // Mécanique de gestion du relai d'événements
    //

    /// <summary>
    /// Recherche et déclenche un événement de type <see cref="HandledEventHandler"/>
    /// </summary>
    /// <param name="key">clé associée à l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    protected void RaiseEvent( object key, HandledEventArgs e ) {
      HandledEventHandler handler = (HandledEventHandler) base.Events[ key ];
      if ( handler != null ) handler( this, e );
    }

    /// <summary>
    /// Recherche et déclenche un événement de type <see cref="InputKeyEventHandler"/>
    /// </summary>
    /// <param name="key">clé associée à l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    protected void RaiseEvent( object key, InputKeyEventArgs e ) {
      InputKeyEventHandler handler = (InputKeyEventHandler) base.Events[ key ];
      if ( handler != null ) handler( this, e );
    }

    private void HandleClickImageLeft( object sender, HandledEventArgs e ) {
      OnClickImageLeft( e );
    }

    private void HandleClickImageRight( object sender, HandledEventArgs e ) {
      OnClickImageRight( e );
    }

    private void HandleInputKey( object sender, InputKeyEventArgs e ) {
      OnInputKey( e );
    }

    /// <summary>
    /// Abonnement aux événements relayé par le wrapper
    /// </summary>
    /// <param name="control">contrôle embraqué</param>
    protected override void OnSubscribeControlEvents( Control control ) {
      RichComboBox comboBox = control as RichComboBox;
      if ( comboBox != null ) {
        comboBox.ClickImageLeft += new HandledEventHandler( HandleClickImageLeft );
        comboBox.ClickImageLeft += new HandledEventHandler( HandleClickImageRight );
        comboBox.InputKey += new InputKeyEventHandler( HandleInputKey );
      }
      base.OnSubscribeControlEvents( control );
    }

    /// <summary>
    /// Désabonnement des événements relayé par le wrapper
    /// </summary>
    /// <param name="control">contrôle embraqué</param>
    protected override void OnUnsubscribeControlEvents( Control control ) {
      RichComboBox comboBox = control as RichComboBox;
      if ( comboBox != null ) {
        comboBox.ClickImageLeft -= new HandledEventHandler( HandleClickImageLeft );
        comboBox.ClickImageLeft -= new HandledEventHandler( HandleClickImageRight );
        comboBox.InputKey -= new InputKeyEventHandler( HandleInputKey );
      }
      base.OnUnsubscribeControlEvents( control );
    }

    //
    // Déclenchement centralisé des événements relayés
    //

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ClickImageLeft"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnClickImageLeft( HandledEventArgs e ) {
      RaiseEvent( EventClickImageLeft, e );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ClickImageRight"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnClickImageRight( HandledEventArgs e ) {
      RaiseEvent( EventClickImageRight, e );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="InputKey"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnInputKey( InputKeyEventArgs e ) {
      RaiseEvent( EventInputKey, e );
    }

    //
    // Reali de fonctionnalités
    //

    /// <summary>
    /// Obtient la localisation associée à un point
    /// </summary>
    /// <param name="location">coordonnées client du point</param>
    /// <returns>la localisation du point dans la boîte d'édition</returns>
    public RichControlHitTest GetHitTestAt( Point location ) {
      return ComboBox.GetHitTestAt( location );
    }

    //
    // Propriétés propres au wrapper
    //

    /// <summary>
    /// Obtient la référence sur la <see cref="RichComboBox"/> embarquée
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    ]
    public new RichComboBox ComboBox {
      get { return Control as RichComboBox; }
    }

    //
    // Relai des propriétés
    //

    /// <summary>
    /// Obtient ou détermine le mode de peinture du contrôle
    /// </summary>
    /// <remarks>
    /// Propriété redéclarée pour ajuster la valeur par défaut de la sérialisation
    /// </remarks>
    [
    Category( "Behavior" ),
    DefaultValue( RichComboBox.DefaultDrawMode ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine le mode de peinture du contrôle" ),
    ]
    public new DrawMode DrawMode {
      get { return ComboBox.DrawMode; }
      set { ComboBox.DrawMode = value; }
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
    public new RichItemCollection Items {
      get { return ComboBox.Items; }
    }

    /// <summary>
    /// Obtient ou détermine l'élément actuellement sélectionné.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine l'élément actuellement sélectionné" ),
    ]
    public new RichItem SelectedItem {
      get { return ComboBox.SelectedItem; }
      set { ComboBox.SelectedItem = value; }
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
    public bool ImageShow {
      get { return ComboBox.ImageShow; }
      set { ComboBox.ImageShow = value; }
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
    public ImageList ImageList {
      get { return ComboBox.ImageList; }
      set { ComboBox.ImageList = value; }
    }

    /// <summary>
    /// Obtient ou détermine l'image associée à la boîte d'édition
    /// </summary>
    /// <remarks>
    /// Cette propriété masque la propriété héritée et la redirige sur la propriété <see cref="TextImageToken"/>
    /// </remarks>
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine l'image associée à la boîte d'édition" )
    ]
    public new Image Image {
      get { return ComboBox.TextImage; }
      set { ComboBox.TextImageObject = value; }
    }

    /// <summary>
    /// Obtient ou détermine l'index de l'image associée à la boîte d'édition
    /// </summary>
    /// <remarks>
    /// Cette propriété masque la propriété héritée et la redirige sur la propriété <see cref="TextImageToken"/>
    /// </remarks>
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine l'index de l'image associée à la boîte d'édition" )
    ]
    public new int ImageIndex {
      get { return ComboBox.TextImageIndex; }
      set { ComboBox.TextImageIndex = value; }
    }

    /// <summary>
    /// Obtient ou détermine la clé de l'image associée à la boîte d'édition
    /// </summary>
    /// <remarks>
    /// Cette propriété masque la propriété héritée et la redirige sur la propriété <see cref="TextImageToken"/>
    /// </remarks>
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine la clé de l'image associée à la boîte d'édition" )
    ]
    public new string ImageKey {
      get { return ComboBox.TextImageKey; }
      set { ComboBox.TextImageKey = value; }
    }

    /// <summary>
    /// Obtient ou détermine l'image (type Image) associée à la boîte d'édition
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( null ),
     RefreshProperties( RefreshProperties.Repaint ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
     Description( "Obtient ou détermine l'image (type Image) associée à la boîte d'édition" ),
    ]
    public Image TextImage {                                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return ComboBox.TextImage; }
      set { ComboBox.TextImageObject = value; }
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
    public Image TextImageObject {                                                             // <wao code.&body:rw>
      get { return ComboBox.TextImageObject; }
      set { ComboBox.TextImageObject = value; }
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
     RefreshProperties( RefreshProperties.Repaint ),
     Description( "Obtient ou détermine le token d'image de la boîte d'édition en tant qu'index dans la liste d'images" ),
    ]
    public int TextImageIndex {                                                             // <wao code.&body:rw RichComboBox_abrégé.+body:rw>
      get { return ComboBox.TextImageIndex; }
      set { ComboBox.TextImageIndex = value; }
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
      get { return ComboBox.TextImageKey; }
      set { ComboBox.TextImageKey = value; }
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
      get { return ComboBox.TextImageToken; }
      set { ComboBox.TextImageToken = value; }
    }

    /// <summary>
    /// Obtient ou détermine si l'image de la boîte de saisie est gérée automatiquement comme étant celle de l'item sélectionné.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    Description( "Obtient ou détermine si l'image de la boîte de saisie est gérée automatiquement comme étant celle de l'item sélectionné" )
    ]
    public bool TextImageSelectedItem {
      get { return ComboBox.TextImageSelectedItem; }
      set { ComboBox.TextImageSelectedItem = value; }
    }

    /// <summary>
    /// Obtient ou détermine le menu contextuel associé à la zone image de la boîte d'édition.
    /// </summary>
    [
    DefaultValue( null ),
    Description( "Obtient ou détermine le menu contextuel associé à la zone image de la boîte d'édition" ),
    ]                                                                                    
    public ContextMenuStrip TextImageStripMenu {
      get { return ComboBox.TextImageStripMenu; }
      set { ComboBox.TextImageStripMenu = value; }
    }

    /// <summary>
    /// Obtient ou détermine la largeur de l'indentation (exprimée en pixels) d'un niveau d'indentation.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( RichComboBox.DefaultIndentWidth ),
    Description( "Obtient ou détermine la largeur de l'indentation (exprimée en pixels) d'un niveau d'indentation" )
    ]
    public int IndentWidth {
      get { return ComboBox.IndentWidth; }
      set { ComboBox.IndentWidth = value; }
    }

    /// <summary>
    /// Obtient ou détermine la hauteur fixe (exprimée en pixels) d'un item
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( RichComboBox.DefaultItemHeight ),
    Description( "Obtient ou détermine la hauteur fixe (exprimée en pixels) d'un item" )
    ]
    public new int ItemHeight {
      get { return ComboBox.ItemHeight; }
      set { ComboBox.ItemHeight = value; }
    }

    /// <summary>
    /// Indique si la propriété <see cref="Font"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool ShouldSerializeFont() {
      return ComboBox.ShouldSerializeFont();
    }

    /// <summary>
    /// Obtient ou détermine la font asociée au contrôle
    /// </summary>
    [
    Category( "Appearance" ),
    Localizable( true ),
    Description( "Obtient ou détermine la font asociée au contrôle" ),
    ]
    public override Font Font {
      get { return ComboBox.Font; }
      set { ComboBox.Font = value; }
    } 

    /// <summary>
    /// Indique si la propriété <see cref="ItemFont"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual bool ShouldSerializeItemFont() {
      return ComboBox.ShouldSerializeItemFont();
    }

    /// <summary>
    /// Obtient ou détermine la font par défaut des éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la font par défaut des éléments" ),
    ]
    public Font ItemFont {
      get { return ComboBox.ItemFont; }
      set { ComboBox.ItemFont = value; }
    }

    /// <summary>
    /// Indique si la propriété <see cref="ItemBackColor"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual bool ShouldSerializeItemBackColor() {
      return ComboBox.ShouldSerializeItemBackColor();
    }

    /// <summary>
    /// Obtient ou détermine la couleur d'arrière-plan pour les éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la couleur d'arrière-plan pour les éléments" ),
    ]
    public Color ItemBackColor {
      get { return ComboBox.ItemBackColor; }
      set { ComboBox.ItemBackColor = value; }
    }

    /// <summary>
    /// Indique si la propriété <see cref="ItemForeColor"/> doit être sérialisée
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclarée public pour permettre la réalisation de wrappers
    /// </remarks>
    /// <returns>true si la propriété doit être sérialisée, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected virtual bool ShouldSerializeItemForeColor() {
      return ComboBox.ShouldSerializeItemForeColor();
    }

    /// <summary>
    /// Obtient ou détermine la couleur d'avant-plan pour les éléments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la couleur d'avant-plan pour les éléments" ),
    ]
    public Color ItemForeColor {
      get { return ComboBox.ItemForeColor; }
      set { ComboBox.ItemForeColor = value; }
    }

    //
    // Evénements
    //

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageLeft {
      add { Events.AddHandler( EventClickImageLeft, value ); }
      remove { Events.RemoveHandler( EventClickImageLeft, value ); }
    }

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de droite se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de droite se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageRight {
      add { Events.AddHandler( EventClickImageRight, value ); }
      remove { Events.RemoveHandler( EventClickImageRight, value ); }
    }

    /// <summary>
    /// Se produit lorsqu'il faut déterminer si un caractère est un caractère d'entrée
    /// </summary>
    [
    Description( "Se produit lorsqu'il faut déterminer si un caractère est un caractère d'entrée" ),
    ]
    public event InputKeyEventHandler InputKey {
      add { Events.AddHandler( EventInputKey, value ); }
      remove { Events.RemoveHandler( EventInputKey, value ); }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                         Extension de RichComboBox pour le flat adpater                      //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class ToolStripRichComboBox {

    /// <summary>
    /// Extension de <see cref="RichTextBox"/> prenant en charge l'adaptateur de peinture "flat"
    /// </summary>
    protected class RichComboBoxStrip : RichComboBox {

      // référence sur le wrapper strip
      private ToolStripRichComboBox owner;

      // référence sur l'adaptateur flat *** gérée par nécessité ***
      private FlatComboAdapter adapter;

      /// <summary>
      /// Constructeur
      /// </summary>
      public RichComboBoxStrip() {
        FlatStyle = FlatStyle.Popup;
        SetStyle( ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true );
      }

      /// <summary>
      /// Obtient ou détermine la référence sur le wrapper strip
      /// </summary>
      public ToolStripRichComboBox Owner {
        get { return owner; }
        set { owner = value; }
      }

      /// <summary>
      /// Obtient la référence sur l'adaptateur flat
      /// </summary>
      /// <remarks>
      /// L'adaptateur est instancié par nécessité
      /// </remarks>
      protected FlatComboAdapter FlatComboAdapter {
        get {
          if ( adapter == null || !adapter.IsValid( this ) )
            adapter = CreateFlatComboAdapter();
          return adapter;
        }
      }

      /// <summary>
      /// Instancie un adaptateur flat
      /// </summary>
      /// <returns>la référence sur l'adaptateur flat instancié</returns>
      protected virtual FlatComboAdapter CreateFlatComboAdapter() {
        return new FlatComboAdapterStrip( this, Owner );
      }

      /// <summary>
      /// Redéfinition de la procédure de traitement des messages
      /// </summary>
      /// <param name="m">message</param>
      protected override void WndProc( ref Message m ) {
        switch ( m.Msg ) {

          case Win.WM_PAINT:
            if ( GetStyle( ControlStyles.UserPaint ) == false && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup) ) {
              DefWndProc( ref m );
              using ( Graphics graphics = Graphics.FromHwnd( m.HWnd ) ) {
                FlatComboAdapter.DrawFlatCombo( graphics );
              }
            }
            else
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
