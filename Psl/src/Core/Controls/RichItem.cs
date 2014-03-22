/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 11 02 2011 : alignement de stratégie des sérialisation des tokens d'image sur celle de RichComboBox
 */                                                                            // <wao never.end>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Psl.Drawing;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                               Convertisseur pour les éléments                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  public partial class RichItem {

    /// <summary>
    /// Convertisseur de type pour les éléments <see cref="RichItem"/> de la liste.
    /// </summary>
    public class RichItemConverter : ExpandableObjectConverter {

      /// <summary>
      /// Indique si une instance de <see cref="RichItem"/> peut être convertie dans un type donné.
      /// </summary>
      /// <param name="context">contexte dans lequel une conversion est requise</param>
      /// <param name="destinationType">type cible de la conversion</param>
      /// <returns>true si la conversion est possible</returns>
      public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType ) {
        if ( destinationType == typeof( InstanceDescriptor ) )
          return true;
        else
          return base.CanConvertTo( context, destinationType );
      }

      /// <summary>
      /// Convertit une instance de <see cref="RichItem"/> en un type donné .
      /// </summary>
      /// <param name="context">contexte dans lequel une conversion est requise</param>
      /// <param name="culture">culture dans laquelle effectuer la conversion</param>
      /// <param name="value">valuer à convertir</param>
      /// <param name="destinationType">type de destination</param>
      /// <returns>référence sur l'objet produit par la conversion</returns>
      public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType ) {
        if ( destinationType == typeof( InstanceDescriptor ) ) {
          Type valueType = value.GetType();
          ConstructorInfo constructor = valueType.GetConstructor( System.Type.EmptyTypes );
          return new InstanceDescriptor( constructor, null, false );
        }
        else
          return base.ConvertTo( context, culture, value, destinationType );
      }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                       Les éléments RichItem d'une RichItemCollection                        //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /*
  public class RichItem                                                        // <wao RichItem_abrégé.+header>
   */

  /// <summary>
  /// Classe regroupant les propriété d'un élément de collection <see cref="RichItemCollection"/>.
  /// </summary>
  /// <remarks>
  /// <p>
  /// Les "tokens d'images" sont typés <see cref="object"/> et permettent de spécifier une image
  /// de trois manières : (1) comme un objet de type <see cref="Image"/>, (2) comme une clé dans la 
  /// liste d'images <see cref="ImageList"/>, (3) comme un index dans la liste d'images <see cref="ImageList"/>.
  /// Les valeurs null, -1 ou chaîne vide sont équivalentes et indiquent qu'aucune image n'est associée
  /// à l'élément.
  /// </p>
  /// </remarks>
  [
  ToolboxItem( false ),
  DesignTimeVisible( false ),
  Serializable(),
  TypeConverterAttribute( typeof( RichItemConverter ) )
  ]
  public partial class RichItem : ISerializable, IImageListProvider {          // <wao RichItem>

    //
    // Constantes
    //

    /// <summary>
    /// Valeur de la propriété <see cref="FontName"/> indiquant que la valeur doit être héritée de l'hôte
    /// </summary>
    public const string InheritFontName = "";

    /// <summary>
    /// Valeur de la propriété <see cref="FontSize"/> indiquant que la valeur doit être héritée de l'hôte
    /// </summary>
    public const float InheritFontSize = 0F;

    /// <summary>
    /// Valeur de la propriété <see cref="FontStyle"/> indiquant que la valeur doit être héritée de l'hôte
    /// </summary>
    public const FontStyle InheritFontStyle = (FontStyle) (-1);

    /// <summary>
    /// Valeur de la propriété <see cref="BackColor"/> indiquant que la valeur doit être héritée de l'hôte
    /// </summary>
    public static readonly Color InheritBackColor = Color.Empty;

    /// <summary>
    /// Valeur de la propriété <see cref="ForeColor"/> indiquant que la valeur doit être héritée de l'hôte
    /// </summary>
    public static readonly Color InheritForeColor = Color.Empty;

    //
    // Champs d'instance associés aux propriétés
    //

    // lien sur la collection propriétaire de l'item
    private IRichCollectionOwner owner;

    // libellé de l'élément
    private string text = string.Empty;

    // données éventuelles associées à l'élément
    private object data = null;

    // données complémentaires éventuelles
    private object tag = null;

    // niveau d'indentation
    private int indentLevel = 0;

    // gestion de l'image
    private ImageIndexer imageIndexer;

    // nom de la font personnalisée ou chaîne vide
    private string fontName = InheritFontName;

    // taille propre à l'item
    private float fontSize = InheritFontSize; 

    // style propre à l'item
    private FontStyle fontStyle = InheritFontStyle;

    // couleur d'affichage du texte
    private Color foreColor = Color.Empty;

    // couleur de fond du texte
    private Color backColor = Color.Empty;

    //
    // Champs internes d'implémentation
    //

    // font courante pour l'affichage du texte *** gestion en cache ***
    private Font font = null;

    //                                                                                 
    // Constructeurs                                                                     // <wao RichItem.+comitem RichItem_abrégé.+comitem>
    //

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <remarks>
    /// Ce constructeur est le constructeur de référence.
    /// </remarks>
    public RichItem() {                                                                  // <wao RichItem.+body RichItem_abrégé.+body>
      imageIndexer = new ImageIndexer( this );
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libellé à afficher pour l'élément</param>
    public RichItem( string text )                                                       // <wao RichItem.+body RichItem_abrégé.+body>
      : this() {
      Text = text;
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libellé à afficher pour l'élément</param>
    /// <param name="imageToken">token de l'image associée à l'item</param>
    public RichItem( string text, object imageToken )                                    // <wao RichItem.+body RichItem(string_object) RichItem_abrégé.+body>
      : this() {
      Text = text;
      DoSetImageToken( imageToken );
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libellé à afficher pour l'élément</param>
    /// <param name="imageToken">token de l'image associée à l'item</param>
    /// <param name="data">référence sur des données quelconques associées à l'item</param>                                                      
    public RichItem( string text, object imageToken, object data )                       // <wao RichItem.+body RichItem_abrégé.+body>
      : this() {
      Text = text;
      DoSetImageToken( imageToken );
      Data = data;
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libellé à afficher pour l'élément</param>
    /// <param name="imageToken">token de l'image associée à l'item</param>
    /// <param name="data">donnée à associer à l'élément</param>
    /// <param name="indentLevel">niveau d'indentation de l'élément</param>
    public RichItem( string text, object imageToken, object data, int indentLevel )      // <wao RichItem.+body>
      : this() {
      Text = text;
      DoSetImageToken( imageToken );
      IndentLevel = indentLevel;
      Data = data;
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="data">donnée à associer à l'élément</param>
    public RichItem( object data )                                                       // <wao RichItem.+body>
      : this() {
      Text = data == null ? null : data.ToString();
      Data = data;
    }

    //
    // Valeurs par défaut
    //

    /// <summary>
    /// Obtient la valeur par défaut de la propriété <see cref="Font"/>.
    /// </summary>
    protected virtual Font DefaultFont {
      get { return IsHosted ? Host.ItemFont : Control.DefaultFont; ; }
    }

    /// <summary>
    /// Obtient la valeur par défaut de la propriété <see cref="BackColor"/>.
    /// </summary>
    protected virtual Color DefaultBackColor {
      get { return IsHosted ? Host.ItemBackColor : SystemColors.Window; }
    }

    /// <summary>
    /// Obtient la valeur par défaut de la propriété <see cref="BackColor"/>.
    /// </summary>
    protected virtual Color DefaultForeColor {
      get { return IsHosted ? Host.ItemForeColor : Color.FromKnownColor( KnownColor.WindowText ); }
    }

    //
    // Liaison à l'hôte
    //

    /// <summary>
    /// Infrastructure. Lie ou délie l'élément de sa collection propriétaire
    /// </summary>
    /// <remarks>
    /// Cette méthode est une méthode d'implémentation qui ne doit être invoquée que par
    /// la collection propriétaire pour mettre à jour le lien d'un élément sur la collection. 
    /// </remarks>
    /// <param name="newOwner">hôte pour l'affichage</param>
    internal void InternalWireToOwner( IRichCollectionOwner newOwner ) {
      InvalidateFont();
      owner = newOwner;
    }

    /// <summary>
    /// Invalide la font mémorisée en cache
    /// </summary>
    /// <remarks>
    /// Méthode appelée par l'hôte d'affichage pour forcer l'actualiation de la font de l'item
    /// </remarks>
    internal protected void InvalidateFont() {
      font = null;
    }

    //
    // Implémentation de ISerializable
    //

    /// <summary>
    /// Constructeur de désérialisation
    /// </summary>
    /// <param name="info">informations sur l'objet à désérialiser</param>
    /// <param name="context">constexte de désérialisation</param>
    protected RichItem( SerializationInfo info, StreamingContext context ) {
      text = (string) info.GetValue( "Text", typeof( string ) );
      data = (object) info.GetValue( "Data", typeof( object ) );
      tag = (object) info.GetValue( "Tag", typeof( object ) );
      indentLevel = (int) info.GetValue( "IndentLevel", typeof( int ) );
      fontName = (string) info.GetValue( "FontName", typeof( string ) );
      fontSize = (float) info.GetValue( "FontSize", typeof( float ) );
      fontStyle = (FontStyle) info.GetValue( "FontStyle", typeof( int ) ); // à cause de -1
      imageIndexer.ImageToken = info.GetValue( "ImageToken", typeof( object ) );
      foreColor = (Color) info.GetValue( "ForeColor", typeof( Color ) );
      backColor = (Color) info.GetValue( "BackColor", typeof( Color ) );
    }

    /// <summary>
    /// Obtient les informations de sérialisation de l'objet
    /// </summary>
    /// <param name="info">informations sur l'objet à désérialiser</param>
    /// <param name="context">constexte de désérialisation</param>
    public void GetObjectData( SerializationInfo info, StreamingContext context ) {
      info.AddValue( "Text", text );
      info.AddValue( "Data", data );
      info.AddValue( "Tag", tag );
      info.AddValue( "IndentLevel", indentLevel );
      info.AddValue( "FontName", fontName );
      info.AddValue( "FontSize", fontSize );
      info.AddValue( "FontStyle", (int) fontStyle );
      info.AddValue( "ForeColor", foreColor );
      info.AddValue( "BackColor", backColor );
      info.AddValue( "ImageToken", imageIndexer.ImageToken );
    }

    //
    // Service
    //

    /// <summary>
    /// Permet de notifier à l'hôte d'affichage que l'item a changé
    /// </summary>
    protected virtual void OnItemChanged() {
      if ( Host != null ) Host.OnItemChanged( this );
    }

    /// <summary>
    /// Provoque l'invalidation de la font et la notification de changement de l'item
    /// </summary>
    protected void DoInvalidateFont() {
      font = null;
      OnItemChanged();
    }

    /// <summary>
    /// Met à jour si nécessaire la font mémorisée en cache
    /// </summary>
    /// <remarks>
    /// Prend en charge la traduction des valeurs par défaut des attributs de la font
    /// </remarks>
    protected void DoUpdateFont() {
      if ( font != null ) return;

      string name = fontName == InheritFontName ? DefaultFont.Name : fontName;
      float size = fontSize == InheritFontSize ? DefaultFont.SizeInPoints : fontSize;
      FontStyle style = fontStyle == InheritFontStyle ? DefaultFont.Style : fontStyle;
      font = new Font( name, size, style ); 
    }

    /// <summary>
    /// Eclate les attributs d'une font selon son nom, sa taille et son style
    /// </summary>
    /// <remarks>
    /// Réduit la font à ses attributs et effectue la réduction pour les valeurs par défaut
    /// </remarks>
    /// <param name="value">font affectée à l'item</param>
    protected void DoSetFont( Font value ) {
      if ( value == null ) value = DefaultFont;
      if ( value.Equals( font ) ) return;

      fontName = value.Name == DefaultFont.Name ? InheritFontName : value.Name;
      fontSize = value.SizeInPoints == DefaultFont.SizeInPoints ? InheritFontSize : value.SizeInPoints;
      fontStyle = value.Style == DefaultFont.Style ? InheritFontStyle : value.Style;

      font = value;
      OnItemChanged();
    }

    /// <summary>
    /// Modifie le nom de la font d'affichage de l'item
    /// </summary>
    /// <remarks>
    /// Prend en charge la réduction de la valeur par défaut
    /// </remarks>
    /// <param name="value">nouveau nom de font</param>
    protected void DoSetFontName( string value ) {
      if ( value == null || value == DefaultFont.Name ) value = string.Empty;
      if ( value == fontName ) return;
      fontName = value;
      DoInvalidateFont();
    }

    /// <summary>
    /// Modifie la taille de la font d'affichage de l'item
    /// </summary>
    /// <remarks>
    /// Prend en charge la réduction de la valeur par défaut
    /// </remarks>
    /// <param name="value">nouvelle taille de font</param>
    protected void DoSetFontSize( float value ) {
      if ( value <= 0 || value == DefaultFont.SizeInPoints ) value = InheritFontSize;
      if ( value == fontSize ) return;
      fontSize = value;
      DoInvalidateFont();
    }

    /// <summary>
    /// Modifie le style de la font d'affichage de l'item
    /// </summary>
    /// <remarks>
    /// Prend en charge la réduction de la valeur par défaut
    /// </remarks>
    /// <param name="value">nouveau style de font</param>
    protected void DoSetFontStyle( FontStyle value ) {
      if ( ((int) value) < 0 || value == DefaultFont.Style ) value = InheritFontStyle;
      if ( value == fontStyle ) return;
      fontStyle = value;
      DoInvalidateFont();
    }

    /// <summary>
    /// Retourne true si un style est armé dans la font, en tenant compte de la font par défaut
    /// </summary>
    /// <param name="style">style concerné</param>
    /// <returns>true si le style est armé</returns>
    protected bool DoGetFontStyle( FontStyle style ) {
      FontStyle actualStyle = fontStyle == InheritFontStyle ? DefaultFont.Style : fontStyle;
      return (actualStyle & style) == style;
    }

    /// <summary>
    /// Ajoute ou retire un flag du style <see cref="FontStyle"/> de la font.
    /// </summary>
    /// <param name="styleFlag">flag concerné</param>
    /// <param name="value">true pour ajouter le flag, false pour le retirer</param>
    protected void DoSetFontStyle( FontStyle styleFlag, bool value ) {
      FontStyle actualStyle = fontStyle == InheritFontStyle ? DefaultFont.Style : fontStyle;
      DoSetFontStyle( value ? actualStyle | styleFlag : actualStyle & ~styleFlag );
    }

    /// <summary>
    /// Détermine l'image associée à l'item via un objet "image token" <see cref="ImageIndexer"/>
    /// </summary>
    /// <param name="kind">genre du token (index, clé ou objet image)</param>
    /// <param name="value">valeur du token d'image</param>
    /// <returns>true si le token changé, false sinon</returns>
    protected bool DoSetImageToken( ImageIndexer.Kind kind, object value ) {
      bool changed = imageIndexer.SetImageToken( kind, value );
      if ( changed ) OnItemChanged();
      return changed;
    }

    /// <summary>
    /// Détermine l'image associée à l'item via un objet "image token" <see cref="ImageIndexer"/>
    /// </summary>
    /// <param name="value">valeur du token d'image</param>
    /// <returns>true si le token changé, false sinon</returns>
    protected bool DoSetImageToken( object value ) {
      bool changed = imageIndexer.SetImageToken( value );
      if ( changed ) OnItemChanged();
      return changed;
    }

    //
    // Fonctionnalités exposées                                                          
    //

    /// <summary>
    /// Obtient la chaîne à associer à l'item comma valeur de la propriété <see cref="Text"/>.
    /// </summary>
    /// <returns>la chaîne asociée à l'item</returns>
    public override string ToString() {
      return Text;
    }

    //
    // Propriétés exposées                                                     // <wao RichItem_abrégé.+comitem>
    //

    /// <summary>
    /// Obtient la référence sur la collection propriétaire du <see cref="RichItem"/>
    /// </summary>
    /// <remarks>
    /// Permet d'atteindre la liste d'image et les valeurs par défaut.
    /// </remarks>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public IRichCollectionOwner Owner {
      get { return owner; }
    }

    /// <summary>
    /// Indique si l'item est actuellement associé à une collection propriétaire.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public bool IsOwned {
      get { return owner != null; }
    }

    /// <summary>
    /// Obtient le contrôle hôte de la collection à laquelle l'item appartient.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public IRichControlHost Host {
      get { return IsOwned ? owner.Host : null; }
    }

    /// <summary>
    /// Indique si l'item est actuellement hébergé par une collection et un contrôle hôte.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public bool IsHosted {
      get { return IsOwned && owner.IsHosted; }
    }

    /// <summary>
    /// Obtient la liste d'image du contrôle.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public ImageList ImageList {
      get { return IsHosted ? Host.ImageList : null; }
    }

    // Données propres de l'élément                                                      // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou détermine le texte de l'élément.
    /// </summary>
    /// <remarks>
    /// Si le texte est vide et si <see cref="Data"/> n'est pas null, retourne Data.ToString(). 
    /// </remarks>
    [
    DefaultValue( "" ),
    Localizable( true ),
    Description( "Libellé de l'élément" ),
    ]
    public string Text {                                                                 // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get {
        return text == string.Empty && data != null ? data.ToString() : text;
      }
      set {
        if ( value == null ) value = string.Empty;
        text = value;
        OnItemChanged();
      }
    }

    /// <summary>
    /// Donnée éventuelle associée au noeud.
    /// </summary>
    [
    DefaultValue( null ),
    TypeConverter( typeof( StringConverter ) ),
    Description( "Donnée éventuelle associée à l'élément" ),
    ]
    public object Data {                                                                 // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return data; }
      set { data = value; }
    }

    /// <summary>
    /// Donnée complémentaire éventuelle associée à l'élément.
    /// </summary>
    [
    DefaultValue( null ),
    TypeConverter( typeof(StringConverter) ),
    Description( "Donnée complémentaire éventuelle associée à l'élément" ),
    ]
    public object Tag {                                                                  // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return tag; }
      set { tag = value; }
    }

    // Niveau d'indentation de l'élément                                                 // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou détermine le niveau d'indentation de l'élément.
    /// </summary>
    /// <remarks>
    /// L'indentation réelle est liée à la propriété IndentWidth du contrôle hôte.
    /// </remarks>
    [
    DefaultValue( 0 ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    Description( "Obtient ou détermine le niveau d'indentation de l'élément" ),
    ]
    public int IndentLevel {                                                             // <wao RichItem.+body:rw>
      get { return indentLevel; }
      set {
        if ( value < 0 ) value = 0;
        indentLevel = value;
        OnItemChanged();
      }
    }

    // Image associée à l'élément                                                        // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient l'image associée à l'élément.
    /// </summary>
    /// <remarks>
    /// L'obtention de cette image est liée à la liste d'image du contrôle hôte 
    /// si l'image n'est pas déterminée par un object image (propriété <see cref="ImageObject"/>).
    /// </remarks>
    [
    Browsable(false),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public Image Image {
      get { return imageIndexer.Image; }
    }

    /// <summary>
    /// Obtient ou détermine l'image de l'élément comme un index dans la liste d'image du contrôle hôte.
    /// </summary>
    [
    DefaultValue( -1 ),
    RefreshProperties( RefreshProperties.Repaint ),
    TypeConverterAttribute( typeof( ImageIndexConverter ) ),
    Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
    Description( "Obtient ou détermine l'image de l'élément par un index dans la liste d'images" ),
    ]
    public int ImageIndex {                                                              // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return imageIndexer.ImageIndex; }
      set { DoSetImageToken( ImageIndexer.Kind.Index, value ); }
    }

    /// <summary>
    /// Obtient ou détermine l'image de l'élément comme une clé dans la liste d'image du contrôle hôte.
    /// </summary>
    [
     DefaultValue( "" ),
     Localizable( true ),
     RefreshProperties( RefreshProperties.Repaint ),
     TypeConverter( typeof( ImageKeyConverter ) ),
     //TypeConverterAttribute( typeof( Psl.Converters.ImageKeyWithNoneConverter ) ),
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
     Description( "Obtient ou détermine l'image de l'élément par une clé dans la liste d'images" ),
    ]
    public string ImageKey {                                                             // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return imageIndexer.ImageKey; }
      set { DoSetImageToken( ImageIndexer.Kind.Key, value ); }
    }

    /// <summary>
    /// Obtient ou détermine l'image de l'élément comme un objet <see cref="Image"/>.
    /// </summary>
    [
    DefaultValue( null ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine directement l'image de l'élément" )
    ]
    public Image ImageObject {                                                           // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return imageIndexer.ImageObject; }
      set { DoSetImageToken( ImageIndexer.Kind.Image, value ); }
    }

    /// <summary>
    /// Obtient ou détermine l'image de l'élément via un "token d'image"
    /// </summary>
    /// <remarks>
    /// Les "tokens" d'image peuvent être :<br/>
    /// - un index dans une liste d'image (propriété <see cref="ImageIndex"/>);
    /// - un clé dans une liste d'images (propriété <see cref="ImageKey"/>);
    /// - un objet image de type <see cref="Image"/> (propriété <see cref="ImageObject"/>).
    /// </remarks>
    [
     Browsable(false),
     DefaultValue( null ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public object ImageToken {                                                           // <wao RichItem.+body:rw RichItem_abrégé.+body:rw>
      get { return imageIndexer.ImageToken; }
      set { DoSetImageToken( value ); }
    }

    // Font et mise en forme de l'élément                                                // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou détermine la font d'affichage de l'élément.
    /// </summary>
    /// <remarks>
    /// En lecture, cette propriété détermine la font à partir des attributs
    /// (propriétés <see cref="FontName"/>, <see cref="FontSize"/> et <see cref="FontStyle"/>.
    /// <br/>
    /// En écriture, cette propriété prélève les attributs de la font assignée pour les
    /// mémorise de manière éclatée dans les propriétés <see cref="FontName"/>, 
    /// <see cref="FontSize"/> et <see cref="FontStyle"/>.
    /// </remarks>
    [
    Browsable(false),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public Font Font {                                                                   // <wao RichItem.+body:rw>
      get {
        DoUpdateFont();
        return font; 
      }
      set { DoSetFont( value ); }
    }

    /// <summary>
    /// Obtient ou détermine le nom de la font associée à l'item.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( InheritFontName ),
    TypeConverter(typeof(Psl.Converters.FontNameWithInheritConverter)), 
    Editor("System.Drawing.Design.FontNameEditor, " + AssemblyRef.SystemDrawingDesign, typeof(UITypeEditor)), 
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    Description( "Obtient ou détermine le nom de la font associée à l'item" ),
    ]
    public string FontName {                                                             // <wao RichItem.+body:rw>
      get { return fontName; }
      set { DoSetFontName( value ); }
    }

    /// <summary>
    /// Obtient ou détermine la taille (en points) de la font associée à l'item.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( InheritFontSize ),
    Description( "Obtient ou détermine la taille en points de la police personnalisée de l'élément" ),
    ]
    public float FontSize {                                                              // <wao RichItem.+body:rw>
      get { return fontSize; }
      set { DoSetFontSize( value ); }
    }

    /// <summary>
    /// Indique si la propriété <see cref="FontStyle"/> doit être sérialisée.
    /// </summary>
    /// <returns>true si la propriété <see cref="FontStyle"/>doit être sérialisée</returns>
    private bool ShouldSerializeFontStyle() {
      return fontStyle != InheritFontStyle;
    }

    /// <summary>
    /// Obtient ou détermine le style de la font associée à l'item.
    /// </summary>
    /// <remarks>
    /// Les différents aspects du style peuvent être aussi déterminés ou obtenus via
    /// les propriétés <see cref="Bold"/>, <see cref="Italic"/>, <see cref="Underline"/>
    /// et <see cref="Strikeout"/>.
    /// </remarks>
    [
    Browsable( false ),
    TypeConverterAttribute( typeof( Psl.Converters.EnumFlagsConverter ) ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    Description( "Obtient ou détermine la mise en forme" ),
    ]
    public FontStyle FontStyle {                                                         // <wao RichItem.+body:rw>
      get { return fontStyle; }
      set { DoSetFontStyle( value ); }
    }

    /// <summary>
    /// Obtient ou détermine la graisse de la font de l'élément.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine si l'élément doit être affiché avec le style gras" ),
    ]
    public bool Bold {                                                                   // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Bold ); }
      set { DoSetFontStyle( FontStyle.Bold, value ); }
    }

    /// <summary>
    /// Obtient ou détermine la mise en forme italique de la font de l'élément.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine si l'élément doit être affiché avec le style italique" ),
    ]
    public bool Italic {                                                                 // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Italic ); }
      set { DoSetFontStyle( FontStyle.Italic, value ); }
    }

    /// <summary>
    /// Obtient ou détermine la mise en forme souligné de la font de l'élément.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine si l'élément doit être affiché avec le style souligné" ),
    ]
    public bool Underline {                                                              // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Underline ); }
      set { DoSetFontStyle( FontStyle.Underline, value ); }
    }

    /// <summary>
    /// Obtient ou détermine si l'élément doit être affiché avec le style barré
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine si l'élément doit être affiché avec le style barré" ),
    ]
    public bool Strikeout {                                                              // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Strikeout ); }
      set { DoSetFontStyle( FontStyle.Strikeout, value ); }
    }

    /// <summary>
    /// Indique si la propriété <see cref="ForeColor"/> doit être sérialisée.
    /// </summary>
    /// <returns>true si la propriété doit être sérialisée</returns>
    private bool ShouldSerializeForeColor() {
      return foreColor != InheritForeColor ; }

    /// <summary>
    /// Obtient ou détermine la couleur d'affichage du texte.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou détermine la couleur d'affichage du texte" ),
    ]
    public Color ForeColor {                                                             // <wao RichItem.+body:rw>
      get { return foreColor == InheritForeColor ? DefaultForeColor : foreColor; }
      set {
        if ( value == DefaultForeColor ) value = InheritForeColor;
        if ( value == foreColor ) return;
        foreColor = value;
        OnItemChanged();
      }
    }

    /// <summary>
    /// Indique si la propriété <see cref="BackColor"/> doit être sérialisée.
    /// </summary>
    /// <returns>true si la propriété doit être sérialisée</returns>
    private bool ShouldSerializeBackColor() {
      return backColor != InheritBackColor;
    }

    /// <summary>
    /// Obtient ou détermine la couleur de fond pour l'affichage du texte.
    /// </summary>
    [
    Browsable( true ),
    Localizable( true ),
    Description( "Obtient ou détermine la couleur de fond pour l'affichage du texte" ),
    ]
    public Color BackColor {                                                             // <wao RichItem.+body:rw>
      get { return backColor == InheritBackColor ? DefaultBackColor : backColor; }
      set {
        if ( value == DefaultBackColor ) value = InheritBackColor;
        if ( value == backColor ) return;
        backColor = value;
        OnItemChanged();
      }
    }
  }                                                                                      // <wao RichItem RichItem_abrégé>
}
