/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 21 05 2009 : version initiale
 * 11 02 2011 : alignement de strat�gie des s�rialisation des tokens d'image sur celle de RichComboBox
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
  //                               Convertisseur pour les �l�ments                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  public partial class RichItem {

    /// <summary>
    /// Convertisseur de type pour les �l�ments <see cref="RichItem"/> de la liste.
    /// </summary>
    public class RichItemConverter : ExpandableObjectConverter {

      /// <summary>
      /// Indique si une instance de <see cref="RichItem"/> peut �tre convertie dans un type donn�.
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
      /// Convertit une instance de <see cref="RichItem"/> en un type donn� .
      /// </summary>
      /// <param name="context">contexte dans lequel une conversion est requise</param>
      /// <param name="culture">culture dans laquelle effectuer la conversion</param>
      /// <param name="value">valuer � convertir</param>
      /// <param name="destinationType">type de destination</param>
      /// <returns>r�f�rence sur l'objet produit par la conversion</returns>
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
  //                       Les �l�ments RichItem d'une RichItemCollection                        //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /*
  public class RichItem                                                        // <wao RichItem_abr�g�.+header>
   */

  /// <summary>
  /// Classe regroupant les propri�t� d'un �l�ment de collection <see cref="RichItemCollection"/>.
  /// </summary>
  /// <remarks>
  /// <p>
  /// Les "tokens d'images" sont typ�s <see cref="object"/> et permettent de sp�cifier une image
  /// de trois mani�res : (1) comme un objet de type <see cref="Image"/>, (2) comme une cl� dans la 
  /// liste d'images <see cref="ImageList"/>, (3) comme un index dans la liste d'images <see cref="ImageList"/>.
  /// Les valeurs null, -1 ou cha�ne vide sont �quivalentes et indiquent qu'aucune image n'est associ�e
  /// � l'�l�ment.
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
    /// Valeur de la propri�t� <see cref="FontName"/> indiquant que la valeur doit �tre h�rit�e de l'h�te
    /// </summary>
    public const string InheritFontName = "";

    /// <summary>
    /// Valeur de la propri�t� <see cref="FontSize"/> indiquant que la valeur doit �tre h�rit�e de l'h�te
    /// </summary>
    public const float InheritFontSize = 0F;

    /// <summary>
    /// Valeur de la propri�t� <see cref="FontStyle"/> indiquant que la valeur doit �tre h�rit�e de l'h�te
    /// </summary>
    public const FontStyle InheritFontStyle = (FontStyle) (-1);

    /// <summary>
    /// Valeur de la propri�t� <see cref="BackColor"/> indiquant que la valeur doit �tre h�rit�e de l'h�te
    /// </summary>
    public static readonly Color InheritBackColor = Color.Empty;

    /// <summary>
    /// Valeur de la propri�t� <see cref="ForeColor"/> indiquant que la valeur doit �tre h�rit�e de l'h�te
    /// </summary>
    public static readonly Color InheritForeColor = Color.Empty;

    //
    // Champs d'instance associ�s aux propri�t�s
    //

    // lien sur la collection propri�taire de l'item
    private IRichCollectionOwner owner;

    // libell� de l'�l�ment
    private string text = string.Empty;

    // donn�es �ventuelles associ�es � l'�l�ment
    private object data = null;

    // donn�es compl�mentaires �ventuelles
    private object tag = null;

    // niveau d'indentation
    private int indentLevel = 0;

    // gestion de l'image
    private ImageIndexer imageIndexer;

    // nom de la font personnalis�e ou cha�ne vide
    private string fontName = InheritFontName;

    // taille propre � l'item
    private float fontSize = InheritFontSize; 

    // style propre � l'item
    private FontStyle fontStyle = InheritFontStyle;

    // couleur d'affichage du texte
    private Color foreColor = Color.Empty;

    // couleur de fond du texte
    private Color backColor = Color.Empty;

    //
    // Champs internes d'impl�mentation
    //

    // font courante pour l'affichage du texte *** gestion en cache ***
    private Font font = null;

    //                                                                                 
    // Constructeurs                                                                     // <wao RichItem.+comitem RichItem_abr�g�.+comitem>
    //

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <remarks>
    /// Ce constructeur est le constructeur de r�f�rence.
    /// </remarks>
    public RichItem() {                                                                  // <wao RichItem.+body RichItem_abr�g�.+body>
      imageIndexer = new ImageIndexer( this );
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libell� � afficher pour l'�l�ment</param>
    public RichItem( string text )                                                       // <wao RichItem.+body RichItem_abr�g�.+body>
      : this() {
      Text = text;
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libell� � afficher pour l'�l�ment</param>
    /// <param name="imageToken">token de l'image associ�e � l'item</param>
    public RichItem( string text, object imageToken )                                    // <wao RichItem.+body RichItem(string_object) RichItem_abr�g�.+body>
      : this() {
      Text = text;
      DoSetImageToken( imageToken );
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libell� � afficher pour l'�l�ment</param>
    /// <param name="imageToken">token de l'image associ�e � l'item</param>
    /// <param name="data">r�f�rence sur des donn�es quelconques associ�es � l'item</param>                                                      
    public RichItem( string text, object imageToken, object data )                       // <wao RichItem.+body RichItem_abr�g�.+body>
      : this() {
      Text = text;
      DoSetImageToken( imageToken );
      Data = data;
    }

    /// <summary>
    /// Initialise une instance de la classe <see cref="RichItem"/>.
    /// </summary>
    /// <param name="text">libell� � afficher pour l'�l�ment</param>
    /// <param name="imageToken">token de l'image associ�e � l'item</param>
    /// <param name="data">donn�e � associer � l'�l�ment</param>
    /// <param name="indentLevel">niveau d'indentation de l'�l�ment</param>
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
    /// <param name="data">donn�e � associer � l'�l�ment</param>
    public RichItem( object data )                                                       // <wao RichItem.+body>
      : this() {
      Text = data == null ? null : data.ToString();
      Data = data;
    }

    //
    // Valeurs par d�faut
    //

    /// <summary>
    /// Obtient la valeur par d�faut de la propri�t� <see cref="Font"/>.
    /// </summary>
    protected virtual Font DefaultFont {
      get { return IsHosted ? Host.ItemFont : Control.DefaultFont; ; }
    }

    /// <summary>
    /// Obtient la valeur par d�faut de la propri�t� <see cref="BackColor"/>.
    /// </summary>
    protected virtual Color DefaultBackColor {
      get { return IsHosted ? Host.ItemBackColor : SystemColors.Window; }
    }

    /// <summary>
    /// Obtient la valeur par d�faut de la propri�t� <see cref="BackColor"/>.
    /// </summary>
    protected virtual Color DefaultForeColor {
      get { return IsHosted ? Host.ItemForeColor : Color.FromKnownColor( KnownColor.WindowText ); }
    }

    //
    // Liaison � l'h�te
    //

    /// <summary>
    /// Infrastructure. Lie ou d�lie l'�l�ment de sa collection propri�taire
    /// </summary>
    /// <remarks>
    /// Cette m�thode est une m�thode d'impl�mentation qui ne doit �tre invoqu�e que par
    /// la collection propri�taire pour mettre � jour le lien d'un �l�ment sur la collection. 
    /// </remarks>
    /// <param name="newOwner">h�te pour l'affichage</param>
    internal void InternalWireToOwner( IRichCollectionOwner newOwner ) {
      InvalidateFont();
      owner = newOwner;
    }

    /// <summary>
    /// Invalide la font m�moris�e en cache
    /// </summary>
    /// <remarks>
    /// M�thode appel�e par l'h�te d'affichage pour forcer l'actualiation de la font de l'item
    /// </remarks>
    internal protected void InvalidateFont() {
      font = null;
    }

    //
    // Impl�mentation de ISerializable
    //

    /// <summary>
    /// Constructeur de d�s�rialisation
    /// </summary>
    /// <param name="info">informations sur l'objet � d�s�rialiser</param>
    /// <param name="context">constexte de d�s�rialisation</param>
    protected RichItem( SerializationInfo info, StreamingContext context ) {
      text = (string) info.GetValue( "Text", typeof( string ) );
      data = (object) info.GetValue( "Data", typeof( object ) );
      tag = (object) info.GetValue( "Tag", typeof( object ) );
      indentLevel = (int) info.GetValue( "IndentLevel", typeof( int ) );
      fontName = (string) info.GetValue( "FontName", typeof( string ) );
      fontSize = (float) info.GetValue( "FontSize", typeof( float ) );
      fontStyle = (FontStyle) info.GetValue( "FontStyle", typeof( int ) ); // � cause de -1
      imageIndexer.ImageToken = info.GetValue( "ImageToken", typeof( object ) );
      foreColor = (Color) info.GetValue( "ForeColor", typeof( Color ) );
      backColor = (Color) info.GetValue( "BackColor", typeof( Color ) );
    }

    /// <summary>
    /// Obtient les informations de s�rialisation de l'objet
    /// </summary>
    /// <param name="info">informations sur l'objet � d�s�rialiser</param>
    /// <param name="context">constexte de d�s�rialisation</param>
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
    /// Permet de notifier � l'h�te d'affichage que l'item a chang�
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
    /// Met � jour si n�cessaire la font m�moris�e en cache
    /// </summary>
    /// <remarks>
    /// Prend en charge la traduction des valeurs par d�faut des attributs de la font
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
    /// R�duit la font � ses attributs et effectue la r�duction pour les valeurs par d�faut
    /// </remarks>
    /// <param name="value">font affect�e � l'item</param>
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
    /// Prend en charge la r�duction de la valeur par d�faut
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
    /// Prend en charge la r�duction de la valeur par d�faut
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
    /// Prend en charge la r�duction de la valeur par d�faut
    /// </remarks>
    /// <param name="value">nouveau style de font</param>
    protected void DoSetFontStyle( FontStyle value ) {
      if ( ((int) value) < 0 || value == DefaultFont.Style ) value = InheritFontStyle;
      if ( value == fontStyle ) return;
      fontStyle = value;
      DoInvalidateFont();
    }

    /// <summary>
    /// Retourne true si un style est arm� dans la font, en tenant compte de la font par d�faut
    /// </summary>
    /// <param name="style">style concern�</param>
    /// <returns>true si le style est arm�</returns>
    protected bool DoGetFontStyle( FontStyle style ) {
      FontStyle actualStyle = fontStyle == InheritFontStyle ? DefaultFont.Style : fontStyle;
      return (actualStyle & style) == style;
    }

    /// <summary>
    /// Ajoute ou retire un flag du style <see cref="FontStyle"/> de la font.
    /// </summary>
    /// <param name="styleFlag">flag concern�</param>
    /// <param name="value">true pour ajouter le flag, false pour le retirer</param>
    protected void DoSetFontStyle( FontStyle styleFlag, bool value ) {
      FontStyle actualStyle = fontStyle == InheritFontStyle ? DefaultFont.Style : fontStyle;
      DoSetFontStyle( value ? actualStyle | styleFlag : actualStyle & ~styleFlag );
    }

    /// <summary>
    /// D�termine l'image associ�e � l'item via un objet "image token" <see cref="ImageIndexer"/>
    /// </summary>
    /// <param name="kind">genre du token (index, cl� ou objet image)</param>
    /// <param name="value">valeur du token d'image</param>
    /// <returns>true si le token chang�, false sinon</returns>
    protected bool DoSetImageToken( ImageIndexer.Kind kind, object value ) {
      bool changed = imageIndexer.SetImageToken( kind, value );
      if ( changed ) OnItemChanged();
      return changed;
    }

    /// <summary>
    /// D�termine l'image associ�e � l'item via un objet "image token" <see cref="ImageIndexer"/>
    /// </summary>
    /// <param name="value">valeur du token d'image</param>
    /// <returns>true si le token chang�, false sinon</returns>
    protected bool DoSetImageToken( object value ) {
      bool changed = imageIndexer.SetImageToken( value );
      if ( changed ) OnItemChanged();
      return changed;
    }

    //
    // Fonctionnalit�s expos�es                                                          
    //

    /// <summary>
    /// Obtient la cha�ne � associer � l'item comma valeur de la propri�t� <see cref="Text"/>.
    /// </summary>
    /// <returns>la cha�ne asoci�e � l'item</returns>
    public override string ToString() {
      return Text;
    }

    //
    // Propri�t�s expos�es                                                     // <wao RichItem_abr�g�.+comitem>
    //

    /// <summary>
    /// Obtient la r�f�rence sur la collection propri�taire du <see cref="RichItem"/>
    /// </summary>
    /// <remarks>
    /// Permet d'atteindre la liste d'image et les valeurs par d�faut.
    /// </remarks>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public IRichCollectionOwner Owner {
      get { return owner; }
    }

    /// <summary>
    /// Indique si l'item est actuellement associ� � une collection propri�taire.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public bool IsOwned {
      get { return owner != null; }
    }

    /// <summary>
    /// Obtient le contr�le h�te de la collection � laquelle l'item appartient.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public IRichControlHost Host {
      get { return IsOwned ? owner.Host : null; }
    }

    /// <summary>
    /// Indique si l'item est actuellement h�berg� par une collection et un contr�le h�te.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public bool IsHosted {
      get { return IsOwned && owner.IsHosted; }
    }

    /// <summary>
    /// Obtient la liste d'image du contr�le.
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public ImageList ImageList {
      get { return IsHosted ? Host.ImageList : null; }
    }

    // Donn�es propres de l'�l�ment                                                      // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou d�termine le texte de l'�l�ment.
    /// </summary>
    /// <remarks>
    /// Si le texte est vide et si <see cref="Data"/> n'est pas null, retourne Data.ToString(). 
    /// </remarks>
    [
    DefaultValue( "" ),
    Localizable( true ),
    Description( "Libell� de l'�l�ment" ),
    ]
    public string Text {                                                                 // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
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
    /// Donn�e �ventuelle associ�e au noeud.
    /// </summary>
    [
    DefaultValue( null ),
    TypeConverter( typeof( StringConverter ) ),
    Description( "Donn�e �ventuelle associ�e � l'�l�ment" ),
    ]
    public object Data {                                                                 // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return data; }
      set { data = value; }
    }

    /// <summary>
    /// Donn�e compl�mentaire �ventuelle associ�e � l'�l�ment.
    /// </summary>
    [
    DefaultValue( null ),
    TypeConverter( typeof(StringConverter) ),
    Description( "Donn�e compl�mentaire �ventuelle associ�e � l'�l�ment" ),
    ]
    public object Tag {                                                                  // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return tag; }
      set { tag = value; }
    }

    // Niveau d'indentation de l'�l�ment                                                 // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou d�termine le niveau d'indentation de l'�l�ment.
    /// </summary>
    /// <remarks>
    /// L'indentation r�elle est li�e � la propri�t� IndentWidth du contr�le h�te.
    /// </remarks>
    [
    DefaultValue( 0 ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    Description( "Obtient ou d�termine le niveau d'indentation de l'�l�ment" ),
    ]
    public int IndentLevel {                                                             // <wao RichItem.+body:rw>
      get { return indentLevel; }
      set {
        if ( value < 0 ) value = 0;
        indentLevel = value;
        OnItemChanged();
      }
    }

    // Image associ�e � l'�l�ment                                                        // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient l'image associ�e � l'�l�ment.
    /// </summary>
    /// <remarks>
    /// L'obtention de cette image est li�e � la liste d'image du contr�le h�te 
    /// si l'image n'est pas d�termin�e par un object image (propri�t� <see cref="ImageObject"/>).
    /// </remarks>
    [
    Browsable(false),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public Image Image {
      get { return imageIndexer.Image; }
    }

    /// <summary>
    /// Obtient ou d�termine l'image de l'�l�ment comme un index dans la liste d'image du contr�le h�te.
    /// </summary>
    [
    DefaultValue( -1 ),
    RefreshProperties( RefreshProperties.Repaint ),
    TypeConverterAttribute( typeof( ImageIndexConverter ) ),
    Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
    Description( "Obtient ou d�termine l'image de l'�l�ment par un index dans la liste d'images" ),
    ]
    public int ImageIndex {                                                              // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return imageIndexer.ImageIndex; }
      set { DoSetImageToken( ImageIndexer.Kind.Index, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine l'image de l'�l�ment comme une cl� dans la liste d'image du contr�le h�te.
    /// </summary>
    [
     DefaultValue( "" ),
     Localizable( true ),
     RefreshProperties( RefreshProperties.Repaint ),
     TypeConverter( typeof( ImageKeyConverter ) ),
     //TypeConverterAttribute( typeof( Psl.Converters.ImageKeyWithNoneConverter ) ),
     Editor( "System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
     Description( "Obtient ou d�termine l'image de l'�l�ment par une cl� dans la liste d'images" ),
    ]
    public string ImageKey {                                                             // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return imageIndexer.ImageKey; }
      set { DoSetImageToken( ImageIndexer.Kind.Key, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine l'image de l'�l�ment comme un objet <see cref="Image"/>.
    /// </summary>
    [
    DefaultValue( null ),
    RefreshProperties( RefreshProperties.Repaint ),
    Description( "Obtient ou d�termine directement l'image de l'�l�ment" )
    ]
    public Image ImageObject {                                                           // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return imageIndexer.ImageObject; }
      set { DoSetImageToken( ImageIndexer.Kind.Image, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine l'image de l'�l�ment via un "token d'image"
    /// </summary>
    /// <remarks>
    /// Les "tokens" d'image peuvent �tre :<br/>
    /// - un index dans une liste d'image (propri�t� <see cref="ImageIndex"/>);
    /// - un cl� dans une liste d'images (propri�t� <see cref="ImageKey"/>);
    /// - un objet image de type <see cref="Image"/> (propri�t� <see cref="ImageObject"/>).
    /// </remarks>
    [
     Browsable(false),
     DefaultValue( null ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public object ImageToken {                                                           // <wao RichItem.+body:rw RichItem_abr�g�.+body:rw>
      get { return imageIndexer.ImageToken; }
      set { DoSetImageToken( value ); }
    }

    // Font et mise en forme de l'�l�ment                                                // <wao RichItem.+comitem>

    /// <summary>
    /// Obtient ou d�termine la font d'affichage de l'�l�ment.
    /// </summary>
    /// <remarks>
    /// En lecture, cette propri�t� d�termine la font � partir des attributs
    /// (propri�t�s <see cref="FontName"/>, <see cref="FontSize"/> et <see cref="FontStyle"/>.
    /// <br/>
    /// En �criture, cette propri�t� pr�l�ve les attributs de la font assign�e pour les
    /// m�morise de mani�re �clat�e dans les propri�t�s <see cref="FontName"/>, 
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
    /// Obtient ou d�termine le nom de la font associ�e � l'item.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( InheritFontName ),
    TypeConverter(typeof(Psl.Converters.FontNameWithInheritConverter)), 
    Editor("System.Drawing.Design.FontNameEditor, " + AssemblyRef.SystemDrawingDesign, typeof(UITypeEditor)), 
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    Description( "Obtient ou d�termine le nom de la font associ�e � l'item" ),
    ]
    public string FontName {                                                             // <wao RichItem.+body:rw>
      get { return fontName; }
      set { DoSetFontName( value ); }
    }

    /// <summary>
    /// Obtient ou d�termine la taille (en points) de la font associ�e � l'item.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( InheritFontSize ),
    Description( "Obtient ou d�termine la taille en points de la police personnalis�e de l'�l�ment" ),
    ]
    public float FontSize {                                                              // <wao RichItem.+body:rw>
      get { return fontSize; }
      set { DoSetFontSize( value ); }
    }

    /// <summary>
    /// Indique si la propri�t� <see cref="FontStyle"/> doit �tre s�rialis�e.
    /// </summary>
    /// <returns>true si la propri�t� <see cref="FontStyle"/>doit �tre s�rialis�e</returns>
    private bool ShouldSerializeFontStyle() {
      return fontStyle != InheritFontStyle;
    }

    /// <summary>
    /// Obtient ou d�termine le style de la font associ�e � l'item.
    /// </summary>
    /// <remarks>
    /// Les diff�rents aspects du style peuvent �tre aussi d�termin�s ou obtenus via
    /// les propri�t�s <see cref="Bold"/>, <see cref="Italic"/>, <see cref="Underline"/>
    /// et <see cref="Strikeout"/>.
    /// </remarks>
    [
    Browsable( false ),
    TypeConverterAttribute( typeof( Psl.Converters.EnumFlagsConverter ) ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Visible ),
    Description( "Obtient ou d�termine la mise en forme" ),
    ]
    public FontStyle FontStyle {                                                         // <wao RichItem.+body:rw>
      get { return fontStyle; }
      set { DoSetFontStyle( value ); }
    }

    /// <summary>
    /// Obtient ou d�termine la graisse de la font de l'�l�ment.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine si l'�l�ment doit �tre affich� avec le style gras" ),
    ]
    public bool Bold {                                                                   // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Bold ); }
      set { DoSetFontStyle( FontStyle.Bold, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine la mise en forme italique de la font de l'�l�ment.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine si l'�l�ment doit �tre affich� avec le style italique" ),
    ]
    public bool Italic {                                                                 // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Italic ); }
      set { DoSetFontStyle( FontStyle.Italic, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine la mise en forme soulign� de la font de l'�l�ment.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine si l'�l�ment doit �tre affich� avec le style soulign�" ),
    ]
    public bool Underline {                                                              // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Underline ); }
      set { DoSetFontStyle( FontStyle.Underline, value ); }
    }

    /// <summary>
    /// Obtient ou d�termine si l'�l�ment doit �tre affich� avec le style barr�
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( false ),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou d�termine si l'�l�ment doit �tre affich� avec le style barr�" ),
    ]
    public bool Strikeout {                                                              // <wao RichItem.+body:rw>
      get { return DoGetFontStyle( FontStyle.Strikeout ); }
      set { DoSetFontStyle( FontStyle.Strikeout, value ); }
    }

    /// <summary>
    /// Indique si la propri�t� <see cref="ForeColor"/> doit �tre s�rialis�e.
    /// </summary>
    /// <returns>true si la propri�t� doit �tre s�rialis�e</returns>
    private bool ShouldSerializeForeColor() {
      return foreColor != InheritForeColor ; }

    /// <summary>
    /// Obtient ou d�termine la couleur d'affichage du texte.
    /// </summary>
    [
    Browsable( true ),
    Description( "Obtient ou d�termine la couleur d'affichage du texte" ),
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
    /// Indique si la propri�t� <see cref="BackColor"/> doit �tre s�rialis�e.
    /// </summary>
    /// <returns>true si la propri�t� doit �tre s�rialis�e</returns>
    private bool ShouldSerializeBackColor() {
      return backColor != InheritBackColor;
    }

    /// <summary>
    /// Obtient ou d�termine la couleur de fond pour l'affichage du texte.
    /// </summary>
    [
    Browsable( true ),
    Localizable( true ),
    Description( "Obtient ou d�termine la couleur de fond pour l'affichage du texte" ),
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
  }                                                                                      // <wao RichItem RichItem_abr�g�>
}
