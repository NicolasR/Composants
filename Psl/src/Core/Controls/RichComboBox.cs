/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 21 05 2009 : version initiale
 * 11 02 2011 : am�lioration de l'affichage de l'image de la bo�te d'�dition
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
  public class RichComboBox : ComboBox                                                             // <wao RichComboBox_abr�g�.+header>
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
  public partial class RichComboBox : ComboBox, IRichControlHost, IRichListPainterHost {           // <wao sp�cif>

    //
    // Param�tres par d�faut
    //

    /// <summary>
    /// Hauteur par d�faut exprim�e en pixels d'un item
    /// </summary>
    public const int DefaultItemHeight = 15;

    /// <summary>
    /// Largeur par d�faut exprim�e en pixels d'un niveau d'indentation
    /// </summary>
    public const int DefaultIndentWidth = 10;

    /// <summary>
    /// Mode de peinture par d�faut
    /// </summary>
    public const DrawMode DefaultDrawMode = DrawMode.OwnerDrawFixed;

    /// <summary>
    /// Rendu par d�faut pour la propri�t� <see cref="FlatStyle"/>
    /// </summary>
    public const FlatStyle DefaultFlatStyle = FlatStyle.Standard;

    //
    // Champs
    //

    // collection des items
    private RichItemCollection items;

    // largeur des indentations en pixels
    private int indentWidth;

    // font par d�faut pour les �l�ments ou null
    private Font itemFont = null;

    // couleur des textes par d�faut pour les �l�ments
    private Color itemForeColor = Color.Empty;

    // couleur d'arri�re-plan par d�faut pour les items
    private Color itemBackColor = Color.Empty ;

    // d�termine si les images doivent �tre affich�es
    private bool imageShow = true;

    // d�termine si l'image TextImage est g�r�e automatiquement (align�e sur celle de l'item sl�ectionn�)
    private bool textImageSelectedItem = true;

    // liste d'images + image courante dans la bo�te d'�dition
    private readonly ImageIndexer textImage = new ImageIndexer();

    // menu contextuel associ� � l'image
    private ContextMenuStrip textImageStripMenu;

    // la fen�tre de dessin du texte courant de la combo
    private NativeChildEdit editWindow;

    //
    // Constructeur                                                                      // <wao code.&comgroup RichComboBox_abr�g�.+comitem>
    //

    /// <summary>
    /// Constructeur                                                                     
    /// </summary>
    public RichComboBox() {                                                              // <wao code.&body RichComboBox_abr�g�.+body>
      editWindow = new NativeChildEdit( this );
      items = new RichItemCollection( this );
      Font = Control.DefaultFont;
      FlatStyle = DefaultFlatStyle;
      DrawMode = DefaultDrawMode;
      ItemHeight = DefaultItemHeight;
      IndentWidth = DefaultIndentWidth;
    }

    //
    // Gestion des �l�ments (interface IRichItemHost)
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
    /// <returns>true si l'orientation est de droite � gauche, false sinon</returns>
    [Browsable(false)]
    public bool IsRightToLeft {
      get {
        return (RightToLeft == RightToLeft.Yes)
          || (RightToLeft == RightToLeft.Inherit && Parent != null && Parent.RightToLeft == RightToLeft.Yes);
      }
    }

    //
    // Mises � jour
    //

    /// <summary>
    /// Provoque l'invalidation de toutes les valeurs g�r�es en cache
    /// </summary>
    protected void InvalidateCaches() {

      // forcer les items � mettre � jour leur font
      foreach ( RichItem item in Items ) {
        item.InvalidateFont();
      }

      // forcer la recr�ation de l'image de bo�te d'�dition
      DoInvalidateEditImage();

    }

    /// <summary>
    /// Notifie un changement de l'image associ�e au texte en �dition
    /// </summary>
    protected void InvalidateTextImage() {
      DoInvalidateEditImage();
    }

    /// <summary>
    /// Provoque l'invalidation compl�te des caches et de l'affichage
    /// </summary>
    /// <remarks>
    /// Control.Invalidate(true) doesn't invalidate the non-client region
    /// </remarks>
    protected void InvalidateAll() {

      // invalider toutes les valeurs en caches
      InvalidateCaches();

      // provoquer la r�fection compl�te de la peinture
      if ( IsHandleCreated )
        Win.RedrawWindow(
          Handle,
          IntPtr.Zero,
          IntPtr.Zero,
          Win.RDW_INVALIDATE | Win.RDW_FRAME | Win.RDW_ERASE | Win.RDW_ALLCHILDREN
          );

      // provoquer la r�fection compl�te de la peinture
      if ( editWindow != null && editWindow.Handle != IntPtr.Zero )
        Win.RedrawWindow(
          editWindow.Handle,
          IntPtr.Zero,
          IntPtr.Zero,
          Win.RDW_INVALIDATE | Win.RDW_FRAME | Win.RDW_ERASE | Win.RDW_ALLCHILDREN
          );
    }

    //
    // D�l�gations de la fen�tre native d'�dition
    //

    /// <summary>
    /// Traite l'enfoncement d'un bouton souris sur la fen�tre d'�dition
    /// </summary>
    /// <param name="m">message intercept� par WM_LBUTTONDOWN ou WM_LBUTTONRIGHT</param>
    /// <param name="button">indique le bouton concern�</param>
    /// <returns>true si le message a �t� pris en charge, false sinon</returns>
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
    // M�thode red�finies
    //

    /// <summary>
    /// Red�finition de la m�thode WndProc pour traiter certains messages
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
    /// Traitements � appliquer lorsque le handle du contr�le est cr��
    /// </summary>
    /// <remarks>
    /// Laisser d'abord la combo standard faire son travail, ensuite compl�ter ou modifier
    /// </remarks>
    /// <param name="e">descripteur d'�v�nement</param>
    protected override void OnHandleCreated( EventArgs e ) {
      base.OnHandleCreated( e );
      DoNativeEditAssignHandle( editWindow );
      InvalidateCaches();
    }

    /// <summary>
    /// Mise � jour de la bo�te d'�dition quand la s�lection change.
    /// </summary>
    /// <param name="e">descripteur d'�v�nement</param>
    protected override void OnSelectedIndexChanged( EventArgs e ) {
      if ( textImageSelectedItem ) {
        textImage.ImageToken = SelectedIndex < 0 ? null : SelectedItem.ImageToken;
        InvalidateTextImage();
      }
      base.OnSelectedIndexChanged( e );
    }

    /// <summary>
    /// Red�finition permettant d'exposer l'�v�nement <see cref="InputKey"/>
    /// </summary>
    /// <remarks>
    /// Il s'agit en particulier de parvenir � r�cup�rer les touches Return et Escape pour
    /// les soustaire au protocole "strip" align� sur celui des menus
    /// </remarks>
    /// <param name="keyData">code de touche</param>
    /// <returns>true la touche est � consid�rer comme un caract�re d'entr�e du contr�le</returns>
    protected override bool IsInputKey( Keys keyData ) {
      InputKeyEventArgs args = new InputKeyEventArgs( keyData, base.IsInputKey( keyData ) );
      OnInputKey( args );
      return args.IsInputKey;
    }

    /// <summary>
    /// Red�finition permettant d'�viter le filtrage autoritaire de certains caract�res
    /// </summary>
    /// <remarks>
    /// Associ�e � la m�thode <see cref="IsInputKey"/> (donc � l'�v�nement <see cref="InputKey"/>,
    /// cette red�finition contribue � soustraire les caract�res Return et Escape au protocole
    /// des contr�les "strip" align� sur celui des menus
    /// </remarks>
    /// <param name="msg">message Windows</param>
    /// <param name="keyData">code de touche</param>
    /// <returns>false si la touche n'a pas �t� prise en charge comme caract�re de commande</returns>
    protected override bool ProcessCmdKey( ref Message msg, Keys keyData ) {
      return IsInputKey( keyData ) ? false : base.ProcessCmdKey( ref msg, keyData );
    }

    /// <summary>
    /// Traitement � effectuer lorsque la font du contr�le est modifi�e
    /// </summary>
    /// <remarks>
    /// L'invalidation est li�e au fait que la font du contr�le constitue la valeur par d�faut 
    /// pour la font par d�faut des items si <see cref="ItemFont"/> est null.
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnFontChanged( EventArgs e ) {
      base.OnFontChanged( e );
      InvalidateAll();
    }

    //
    // D�clenchement centralis� des �v�nements
    //

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement <see cref="ClickImageLeft"/>.
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected virtual void OnClickImageLeft( HandledEventArgs e ) {
      if ( ClickImageLeft == null ) return;
      e.Handled = true;
      ClickImageLeft( this, e );
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement <see cref="ClickImageRight"/>.
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected virtual void OnClickImageRight( HandledEventArgs e ) {
      if ( ClickImageRight == null ) return;
      e.Handled = true;
      ClickImageRight( this, e );
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement <see cref="InputKey"/>.
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected virtual void OnInputKey( InputKeyEventArgs e ) {
      if ( InputKey != null ) InputKey( this, e );
    }

    //
    // Fonctionnalit�s expos�es                                                          // <wao code.&comgroup>
    //

    /// <summary>
    /// Obtient la localisation associ�e � un point
    /// </summary>
    /// <param name="location">coordonn�es client du point</param>
    /// <returns>la localisation du point dans la bo�te d'�dition</returns>
    public RichControlHitTest GetHitTestAt( Point location ) {                             // <wao code.&body>
      return DoGetHitTestAt( location );
    }

    //
    // Propri�t�s expos�es                                                               // <wao code.&comgroup RichComboBox_abr�g�.+comitem>
    //

    /// <summary>
    /// Obtient ou d�termine le mode de peinture du contr�le
    /// </summary>
    /// <remarks>
    /// Propri�t� red�clar�e pour ajuster la valeur par d�faut de la s�rialisation
    /// </remarks>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultDrawMode ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou d�termine le mode de peinture du contr�le" ),
    ]
    public new DrawMode DrawMode {
      get { return base.DrawMode; }
      set { base.DrawMode = value; }
    }

    /// <summary>
    /// Obtient une r�f�rence sur la collection des �l�ments.
    /// </summary>
    [
    Category( "Data" ),
    Localizable( true ),
    MergableProperty( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Editor( "System.ComponentModel.Design.CollectionEditor, " + Psl.AssemblyRef.SystemDesign, typeof( UITypeEditor ) ), // Editor( typeof( CollectionEditor ), typeof( UITypeEditor ) ),
    TypeConverterAttribute( typeof( RichItemCollection ) ),
    Description( "Obtient une r�f�rence sur la collection des �l�ments" ),
    ]
    public new RichItemCollection Items {                                                // <wao code.&body:ro RichComboBox_abr�g�.+body:ro>
      get { return items; }
    }

    /// <summary>
    /// Obtient ou d�termine l'�l�ment actuellement s�lectionn�.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine l'�l�ment actuellement s�lectionn�" ),
    ]
    public new RichItem SelectedItem {                                                   // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return base.SelectedItem as RichItem; }
      set { base.SelectedItem = value; }
    }

    /// <summary>
    /// Obtient ou d�termine si les images doivent �tre affich�es.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou d�termine si les images doivent �tre affich�es" ),
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
    /// Obtient ou d�termine la liste d'image associ�e aux �l�ments de la collection.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( null ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou d�termine la liste d'image associ�e aux �l�ments de la collection" )
    ]
    public ImageList ImageList {                                                         // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImage.ImageList; }
      set {
        if ( value == textImage.ImageList ) return;
        textImage.ImageList = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Obtient ou d�termine l'image (type Image) associ�e � la bo�te d'�dition
    /// </summary>
    [
     Browsable( false ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
     Description( "Obtient ou d�termine l'image (type Image) associ�e � la bo�te d'�dition" ),
    ]
    public Image TextImage {                                                             // <wao code.&body:rw>
      get { return textImage.Image; }
      set { TextImageObject = value; }
    }

    /// <summary>
    /// Obtient ou d�termine le token d'image de la bo�te d'�dition en tant qu'objet Image
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( null ),
     RefreshProperties( RefreshProperties.Repaint ),
     Description( "Obtient ou d�termine le token d'image de la bo�te d'�dition en tant qu'objet Image" ),
    ]
    public Image TextImageObject {                                                             // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImage.ImageObject; }
      set {
        if ( value == textImage.ImageToken ) return;
        textImage.ImageObject = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou d�termine le token d'image de la bo�te d'�dition en tant qu'index dans la liste d'images
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( -1 ),
     RelatedImageList( "ImageList" ),
     TypeConverter( "System.Windows.Forms.NoneExcludedImageIndexConverter, " + AssemblyRef.SystemWindowsForms ),
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ), 
     RefreshProperties(RefreshProperties.Repaint),
     Description( "Obtient ou d�termine le token d'image de la bo�te d'�dition en tant qu'index dans la liste d'images" ),
    ]
    public int TextImageIndex {                                                             // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImage.ImageIndex; }
      set {
        if ( value == textImage.ImageIndex ) return;
        textImage.ImageIndex = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou d�termine le token d'image de la bo�te d'�dition en tant que cl� dans la liste d'images
    /// </summary>
    [
     Category( "Behavior" ),
     Browsable( true ),
     DefaultValue( "" ),
     RelatedImageList( "ImageList" ),
     TypeConverter( typeof( ImageKeyConverter ) ), 
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
     RefreshProperties( RefreshProperties.Repaint ),
     Description( "Obtient ou d�termine le token d'image de la bo�te d'�dition en tant que cl� dans la liste d'images" ),
    ]
    public string TextImageKey {                                                             // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImage.ImageKey; }
      set {
        if ( value == textImage.ImageKey ) return;
        textImage.ImageKey = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou d�termine le token d'image associ� � la bo�te d'�dition
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine le token d'image associ� � la bo�te d'�dition" ),
    ]
    public object TextImageToken {                                             // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImage.ImageToken; }
      set {
        if ( value == textImage.ImageToken ) return;
        textImage.ImageToken = value;
        InvalidateTextImage();
      }
    }

    /// <summary>
    /// Obtient ou d�termine si l'image de la bo�te de saisie est automatiquement celle de l'item s�lectionn�.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    Description( "Obtient ou d�termine si l'image de la bo�te d'�dition est automatiquement celle de l'item s�lectionn�" )
    ]
    public bool TextImageSelectedItem {                                        // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get {
        return textImageSelectedItem;
      }
      set {
        textImageSelectedItem = value;
      }
    }

    /// <summary>
    /// Obtient ou d�termine le menu contextuel associ� � la zone image de la bo�te d'�dition.
    /// </summary>
    [
     Category( "Behavior" ),
     DefaultValue( null ),
     Description( "Obtient ou d�termine le menu contextuel associ� � la zone image de la bo�te d'�dition" ),
    ]
    public ContextMenuStrip TextImageStripMenu {                                 // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return textImageStripMenu; }
      set { textImageStripMenu = value; }
    }

    /// <summary>
    /// Obtient ou d�termine la largeur de l'indentation (exprim�e en pixels) d'un niveau d'indentation.
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultIndentWidth ),
    Description( "Obtient ou d�termine la largeur de l'indentation (exprim�e en pixels) d'un niveau d'indentation" )
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
    /// Obtient ou d�termine la hauteur fixe (exprim�e en pixels) d'un item
    /// </summary>
    [
    Category( "Behavior" ),
    DefaultValue( DefaultItemHeight ),
    Description( "Obtient ou d�termine la hauteur fixe (exprim�e en pixels) d'un item" )
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
    /// Indique si la propri�t� <see cref="Font"/> doit �tre s�rialis�e
    /// </summary>
    /// <remarks>
    /// Cette m�thode est d�clar�e public pour permettre la r�alisation de wrappers
    /// </remarks>
    /// <returns>true si la propri�t� doit �tre s�rialis�e, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeFont() {
      return Font != null && !Font.Equals( Control.DefaultFont );
    }

    /// <summary>
    /// Indique si la propri�t� <see cref="ItemFont"/> doit �tre s�rialis�e
    /// </summary>
    /// <remarks>
    /// Cette m�thode est d�clar�e public pour permettre la r�alisation de wrappers
    /// </remarks>
    /// <returns>true si la propri�t� doit �tre s�rialis�e, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemFont() {
      return itemFont != null;
    }

    /// <summary>
    /// Obtient ou d�termine la font par d�faut des �l�ments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou d�termine la font par d�faut des �l�ments" ),
    ]
    public Font ItemFont {                                                               // <wao code.&body:rw RichComboBox_abr�g�.+body:rw>
      get { return itemFont == null ? DefaultFont : itemFont; }
      set {
        if ( value == null || value.Equals( DefaultFont ) ) value = null;
        if ( value == itemFont ) return;
        itemFont = value;
        InvalidateAll();
      }
    }

    /// <summary>
    /// Indique si la propri�t� <see cref="ItemBackColor"/> doit �tre s�rialis�e
    /// </summary>
    /// <remarks>
    /// Cette m�thode est d�clar�e public pour permettre la r�alisation de wrappers
    /// </remarks>
    /// <returns>true si la propri�t� doit �tre s�rialis�e, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemBackColor() {
      return itemBackColor != Color.Empty;
    }

    /// <summary>
    /// Obtient ou d�termine la couleur d'arri�re-plan pour les �l�ments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou d�termine la couleur d'arri�re-plan pour les �l�ments" ),
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
    /// Indique si la propri�t� <see cref="ItemForeColor"/> doit �tre s�rialis�e
    /// </summary>
    /// <remarks>
    /// Cette m�thode est d�clar�e public pour permettre la r�alisation de wrappers
    /// </remarks>
    /// <returns>true si la propri�t� doit �tre s�rialis�e, false sinon</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShouldSerializeItemForeColor() {
      return itemForeColor != Color.Empty;
    }

    /// <summary>
    /// Obtient ou d�termine la couleur d'avant-plan pour les �l�ments.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou d�termine la couleur d'avant-plan pour les �l�ments" ),
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
    // Ev�nements                                                                        // <wao code.&comgroup RichComboBox_abr�g�.+comitem>
    //

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de gauche se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageLeft;                                     // <wao sp�cif RichComboBox_abr�g�>

    /// <summary>
    /// Se produit lorsqu'un clic souris bouton de droite se produit sur l'image.
    /// </summary>
    [
    Description( "Se produit lorsqu'un clic souris bouton de droite se produit sur l'image")
    ]
    public event HandledEventHandler ClickImageRight;                                    // <wao sp�cif RichComboBox_abr�g�>

    /// <summary>
    /// Se produit lorsqu'il faut d�terminer si un caract�re est un caract�re d'entr�e
    /// </summary>
    [
    Description( "Se produit lorsqu'il faut d�terminer si un caract�re est un caract�re d'entr�e" ),
    ]
    public event InputKeyEventHandler InputKey;                                          // <wao sp�cif>
  }                                                                                      // <wao sp�cif>

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                    Composant RichComboBox                                   //
  //                                  Peinture de la bo�te liste                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {
  
    // r�f�rence sur le peintre des items de la liste *** g�r� par n�cessit� ***
    private IRichListPainter richListPainter ;

    /// <summary>
    /// Obtient un peintre pour les items de la liste d�roulante
    /// </summary>
    /// <returns>la r�f�rence sur le peintre</returns>
    protected virtual IRichListPainter CreateRichListPainter() {
      return new RichListPainter( this ) ;
    }

    /// <summary>
    /// Obtient la r�fence sur le peintre courant des items de la liste
    /// </summary>
    /// <remarks>
    /// La r�f�rence sur le peintre courant est g�r�e par n�cessit�.
    /// </remarks>
    protected IRichListPainter RichListPainter {
      get {
        if ( richListPainter == null ) 
          richListPainter = CreateRichListPainter();
        return richListPainter;
      }
    }

    /// <summary>
    /// D�termine le rectangle d'affichage d'un item en mode OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnMeasureItem( MeasureItemEventArgs e ) {
      if ( DataSource == null && 0 <= e.Index && e.Index < Items.Count ) 
        RichListPainter.MeasureRichItem( e, Items[ e.Index ] );
      base.OnMeasureItem( e );
    }

    /// <summary>
    /// Effectue la peinture d'un item en mode OwnderDrawFixed ou OwnerDrawVariable
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
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
  //                                 Peinture de la bo�te d'�dition                              //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {

    // r�f�rence sur le peintre des items de la liste *** g�r� par n�cessit� ***
    private IRichEditPainter richEditPainter ;

    /// <summary>
    /// Obtient un peintre pour les items de la liste d�roulante
    /// </summary>
    /// <returns>la r�f�rence sur le peintre</returns>
    protected virtual IRichEditPainter CreateRichEditPainter() {
      return new RichEditPainter( this ) ;
    }

    /// <summary>
    /// Obtient la r�fence sur le peintre courant des items de la liste
    /// </summary>
    /// <remarks>
    /// La r�f�rence sur le peintre courant est g�r�e par n�cessit�.
    /// </remarks>
    protected IRichEditPainter RichEditPainter {
      get {
        if ( richEditPainter == null ) 
          richEditPainter = CreateRichEditPainter();
        return richEditPainter;
      }
    }

    /// <summary>
    /// Sous-classer la fen�tre d'�dition
    /// </summary>
    private void DoNativeEditAssignHandle( NativeChildEdit native ) {
      if ( native == null ) return;

      // lib�rer le handle de la fen�tre d'�dition courante
      if ( !native.Handle.Equals( IntPtr.Zero ) ) native.ReleaseHandle();

      // assigner le handle actuel de la fen�tre d'�dition
      Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );
      native.AssignHandle( infos.hwndEdit );
    }

    /// <summary>
    /// Obtient la localisation d'un point relativement aux zones de la bo�te d'�dition
    /// </summary>
    /// <param name="location">coordonn�es du point en coordonn�es client</param>
    /// <returns>l'indication de la zone de la bo�te liste concern�e</returns>
    protected virtual RichControlHitTest DoGetHitTestAt( Point location ) {
      if ( !ClientRectangle.Contains( location ) ) return RichControlHitTest.None;

       // r�cup�rer les marges au niveau du composant
      ulong margins = 0;
      if (editWindow != null && editWindow.Handle != IntPtr.Zero ) margins = (ulong) Win.SendMessage( editWindow.Handle, Win.EM_GETMARGINS, 0, 0 );
      int leftMargin = (int) (margins & 0x0000FFFF);
      int rightMargin = (int) ((margins & 0xFFFF0000) >> 16);

      bool inLeftPart = location.X < (IsRightToLeft ? ClientRectangle.Right - rightMargin : leftMargin);
      return inLeftPart && !IsRightToLeft || !inLeftPart && IsRightToLeft ? RichControlHitTest.Image : RichControlHitTest.Text;
    }

    /// <summary>
    /// Invalide les informations g�r�es en cache au niveau du peintre de la fen�tre d'�dition
    /// </summary>
    protected void DoInvalidateEditImage() {
      Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );
      RichEditPainter.InvalidateEditImage( infos.hwndEdit );
    }

    /// <summary>
    /// Prend en charge un message Wm_Paint pour la fen�tre d'�dition
    /// </summary>
    /// <param name="m">message</param>
    private void HandleEditPaint( ref Message m ) {
      //Psl.Tracker.Tracker.Track( "RichComboBox : HandleEditPaint" );
      if ( DrawMode == DrawMode.Normal ) return;

      // obtenir une surface de dessin
      using ( Graphics graphics = Graphics.FromHwnd( m.HWnd ) ) {

        // infos combo
        Win.ComboBoxInfo infos = Win.GetComboBoxInfos( Handle );

        // d�terminer le rectangle de l'item
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
  //                      fen�tre native pour sous-classer la fen�tre d'�dition                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class RichComboBox {

    private class NativeChildEdit : NativeWindow {

      // Combo propri�taire de la fen�tre d'�dition
      private RichComboBox owner = null;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="owner">composant <see cref="RichComboBox" />propri�taire</param>
      internal NativeChildEdit( RichComboBox owner ) {
        this.owner = owner;
      }

      protected override void WndProc( ref Message m ) {
        //Psl.Tracker.Tracker.Track( "RichCombo.NativeWindow.WndProc : message : " + m.Msg.ToString( "X" ) );
        switch ( m.Msg ) {

          // workaround pour forcer le r�affichage de l'image
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
