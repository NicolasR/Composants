/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 01 01 2009 : version initiale
 * 26 04 2009 : amélioration du filtrage dans les propriétés Text et ToolTipText des StatusReporterLabel
 * 26 04 2009 : légère réduction de la hauteur de la jauge de progression par souci esthétique
 * 16 02 2011 : amélioration de l'affichage de la jauge pour qu'elle parvienne à sa valeur maximale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Controls {

  /// <summary>
  /// Barre d'état pré-configurée implémentant l'interface <see cref="IStatusService"/>.
  /// </summary>
  [
  Description( "Barre d'état pré-configurée implémentant l'interface IStatusService" )
  ]
  public partial class StatusReporter : StatusStrip, IStatusService {

    //
    // Champs
    //

    private IContainer components = null;
    private StatusReporterLabel statusLeft;
    private StatusReporterLabel statusMiddle;
    private StatusReporterLabel statusRight;
    private StatusReporterLabel statusInfos;
    private StatusReporterProgressBar statusProgress;

    //
    // Gestion générale
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public StatusReporter() {
      InitializeComponent();
    }

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      ConfigureComponent();
    }

    /// <summary>
    /// Préconfiguration du composant avec les volets standard
    /// </summary>
    private void ConfigureComponent() {
      if ( Items.Count != 0 ) return;

      statusLeft = new Psl.Controls.StatusReporterLabel( "Left", this );
      statusMiddle = new Psl.Controls.StatusReporterLabel( "Middle", this );
      statusRight = new Psl.Controls.StatusReporterLabel( "Right", this );
      statusInfos = new Psl.Controls.StatusReporterLabel( "Infos", this );
      statusProgress = new Psl.Controls.StatusReporterProgressBar( "Progress", this );

      this.SuspendLayout();
      Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            statusLeft,
            statusMiddle,
            statusRight,
            statusInfos,
            statusProgress} );

      ShowItemToolTips = true;
      MinimumSize = new Size( 0, 25 );

      // statusLeft
      statusLeft.AutoSize = false;
      statusLeft.Size = new Size( 100, 20 );

      // statusMiddle
      statusMiddle.AutoSize = false;
      statusMiddle.Size = new Size( 100, 20 );

      // statusRight
      statusRight.Size = new Size( 4, 20 );

      // statusInfos
      statusInfos.Size = new Size( 451, 20 );
      statusInfos.Spring = true;
      statusInfos.TextAlign = ContentAlignment.MiddleLeft;

      // statusProgress
      statusProgress.Size = new Size( 80, 12 );
      statusProgress.Visible = false;
      statusProgress.AutoSize = false;
      statusProgress.Margin = new Padding( 4, 6, 1, 6 );

      ResumeLayout( false );
      PerformLayout();
    }

    //
    // Service
    //

    /// <summary>
    /// Obtient le <see cref="StatusReporterLabel"/> associé à la clé d'énumération.
    /// </summary>
    /// <param name="labelEnumOrName">clé d'énumération</param>
    /// <returns>le volet associé à la clé d'énumération</returns>
    protected StatusReporterLabel DoTryGetLabel( StatusLabels labelEnumOrName ) {
      StatusReporterLabel result = null;

      // aiguiller et filtrer selon la clé d'énumération
      switch ( labelEnumOrName ) {
        case StatusLabels.Left: result = statusLeft; break;
        case StatusLabels.Middle: result = statusMiddle; break;
        case StatusLabels.Right: result = statusRight; break;
        case StatusLabels.Infos: result = statusInfos; break;
        default: throw new ArgumentException( "La valeur ne figure pas dans l'énumération", "labelEnumOrName" );
      }

      // contrôler que le volet est disponible
      if ( !result.Displayed ) throw new ArgumentException( "Le volet standard " + labelEnumOrName.ToString() + " n'est pas actuellement disponible (Displayed==false)", "labelEnumOrName" );
      return result;
    }

    /// <summary>
    /// Obtient le <see cref="ToolStripStatusLabel"/> associé au nom de volet.
    /// </summary>
    /// <param name="labelEnumOrName">nom du volet</param>
    /// <returns>le volet associé au nom de volet</returns>
    protected ToolStripStatusLabel DoTryGetLabel( string labelEnumOrName ) {

      // effectuer la recherche danbs la collection des volets
      ToolStripItem item = Items[ labelEnumOrName ] ;
      if (item == null) throw new ArgumentException( string.Format( "La collection des labels affichés dans la barre d'état ne comporte aucun élément nommé \"{0}\"", labelEnumOrName ), "labelEnumOrName" ) ;

      // contrôler qu'il s'agit d'un ToolStripStatusLabel
      ToolStripStatusLabel result = item as ToolStripStatusLabel;
      if (result == null) throw new ArgumentException( string.Format( "L'élément de la barre d'état nommé \"{0}\" n'est pas un Label", labelEnumOrName), "labelEnumOrName" ) ;
      return result;
    }

    /// <summary>
    /// Obtient le <see cref="ToolStripStatusLabel"/> associé à une clé d'énumération ou à un nom de volet.
    /// </summary>
    /// <param name="labelEnumOrName">identification du label</param>
    /// <returns>référence sur le <see cref="ToolStripStatusLabel"/></returns>
    protected ToolStripStatusLabel DoTryGetLabel( object labelEnumOrName ) {
      if (labelEnumOrName == null) throw new ArgumentNullException( "labelEnumOrName" ) ;

      if ( labelEnumOrName is StatusLabels )
        return DoTryGetLabel( (StatusLabels) labelEnumOrName );
      if ( labelEnumOrName is string )
        return DoTryGetLabel( labelEnumOrName as string );
      else
        throw new ArgumentException( "L'identification d'un volet doit être un membre de l'énumération StatusLabels ou un nom de type string", "labelEnumOrName" );
    }

    /// <summary>
    /// Modifier l'image d'un label
    /// </summary>
    /// <param name="label">label dont l'image est à modifier</param>
    /// <param name="imageIndexOrKey">index ou clé de l'image dans la liste d'image</param>
    protected void DoSetImage( ToolStripStatusLabel label, object imageIndexOrKey ) {

      // contrôler la validité de l'argument
      if ( imageIndexOrKey == null ) imageIndexOrKey = string.Empty;
      bool isIndex = imageIndexOrKey is int;
      if ( !isIndex && !(imageIndexOrKey is string) )
        throw new ArgumentException( "L'identification d'une image doit être un entier ou une chaîne", "imageIndexOrKey" );

      // traitement selon le type de label
      StatusReporterLabel labelInternal = label as StatusReporterLabel;
      if ( labelInternal != null )
        if ( isIndex )
          labelInternal.ImageIndex = (int) imageIndexOrKey;
        else
          labelInternal.ImageKey = imageIndexOrKey as string;
      else {
        label.ImageKey = string.Empty;
        label.ImageIndex = -1;
        if ( label.Image != null ) {
          label.Image.Dispose();
          label.Image = null;
        }
        if ( isIndex )
          label.ImageIndex = (int) imageIndexOrKey;
        else
          label.ImageKey = imageIndexOrKey as string;
      }
    }

    /// <summary>
    /// Modifier l'image d'un label
    /// </summary>
    /// <param name="label">label dont l'image est à modifier</param>
    /// <param name="image">objet image</param>
    protected void DoSetImage( ToolStripStatusLabel label, Image image ) {

      // traitement selon le type de label
      StatusReporterLabel labelInternal = label as StatusReporterLabel;
      if ( labelInternal != null )
        labelInternal.Image = image ;
      else {
        label.ImageKey = string.Empty;
        label.ImageIndex = -1;
        if ( label.Image != null ) {
          label.Image.Dispose();
          label.Image = null;
        }
        label.Image = image;
      }
    }

    /// <summary>
    /// Extension de la classe <see cref="ToolStripItemCollection"/> pour sérialiser la collection
    /// </summary>
    /// <remarks>
    /// Les volets internes standard ne sont pas inclus dans la collection sérialisée standard.
    /// Cette classe est simplement destinée à permettre la sérialisation de la référence de tous les volets 
    /// de la barre d'état dans l'odre produit par l'utilisateur. 
    /// </remarks>
    public class ToolStripItemCollectionEx : ToolStripItemCollection {

      private StatusStrip owner = null;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="owner">composant propriétaire de la collection</param>
      /// <param name="value">éléments à inclure initialement dans la collection</param>
      public ToolStripItemCollectionEx( StatusStrip owner, ToolStripItem[] value ) : base( owner, value ) {
        this.owner = owner;
      }

      /// <summary>
      /// Redéclaration destinée à intercepter la désérialisation
      /// </summary>
      /// <param name="toolStripItems">éléments à placer dans la collection des items</param>
      public new void AddRange( ToolStripItem[] toolStripItems ) {
        owner.Items.Clear();
        owner.Items.AddRange( toolStripItems );
      }
    }

    /// <summary>
    /// Collection ordonnée des volets effectivement affichés
    /// </summary>
    /// <remarks>
    /// La sérialisation correcte de cette propriété impose (mais pourquoi ?) que son
    /// identificateur commence par T à Z. 
    /// </remarks>
    [
      Browsable( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    ]
    public ToolStripItemCollectionEx ZDisplayed {
      get {
        ToolStripItem[] items = new ToolStripItem[ Items.Count ];
        Items.CopyTo( items, 0 );
        return new ToolStripItemCollectionEx( this, items );
      }
    }

    //
    // Propriétés et fonctionnalités exposées
    //

    /// <summary>
    /// Obtient le volet fixe de gauche.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Category( "Behavior" ),
      Description( "Obtient le volet fixe de gauche" )
    ]
    public StatusReporterLabel StatusLeft {
      get { return statusLeft; }
      set { }
    }

    /// <summary>
    /// Obtient le volet fixe médian.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Category( "Behavior" ),
      Description( "Obtient le volet fixe médian" )
    ]
    public StatusReporterLabel StatusMiddle {
      get { return statusMiddle; }
      set { }
    }

    /// <summary>
    /// Obtient le volet élastique médian.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Category( "Behavior" ),
      Description( "Obtient le volet élastique médian" )
    ]
    public StatusReporterLabel StatusRight {
      get { return statusRight; }
      set { }
    }

    /// <summary>
    /// Obtient le volet élastique de droite.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Category( "Behavior" ),
      Description( "Obtient le volet élastique de droite" )
    ]
    public StatusReporterLabel StatusInfos {
      get { return statusInfos; }
      set { }
    }

    /// <summary>
    /// Obtient la jauge de progression.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Category( "Behavior" ),
      Description( "Obtient la jauge de progression" )
    ]
    public StatusReporterProgressBar StatusProgress {
      get { return statusProgress; }
      set { }
    }

    /// <summary>
    /// Obtient ou détermine le texte d'un volet de texte
    /// </summary>
    /// <param name="labelEnumOrName">clé d'énumération <see cref="StatusLabels"/>ou nom du volet</param>
    /// <returns>le texte du label</returns>
    [
      Browsable( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]   
    public string this[ object labelEnumOrName ] {
      get { return DoTryGetLabel( labelEnumOrName ).Text; }
      set { DoTryGetLabel( labelEnumOrName ).Text = value; }
    }

    /// <summary>
    /// Obtient ou détermine le texte du volet fixe de gauche.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine le texte du volet fixe de gauche" )
    ]
    public string TextLeft {
      get { return statusLeft.Text; }
      set { DoTryGetLabel( StatusLabels.Left ).Text = value; }
    }

    /// <summary>
    /// Obtient ou détermine le texte du volet fixe médian.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine le texte du volet fixe médian" )
    ]
    public string TextMiddle {
      get { return statusMiddle.Text; }
      set { DoTryGetLabel( StatusLabels.Middle ).Text = value; }
    }

    /// <summary>
    /// Obtient ou détermine le texte du volet élastique médian.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine le texte du volet élastique médian" )
    ]
    public string TextRight {
      get { return statusRight.Text; }
      set { DoTryGetLabel( StatusLabels.Right ).Text = value; }
    }

    /// <summary>
    /// Obtient ou détermine le texte du volet élastique de droite.
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine le texte du volet élastique de droite" )
    ]
    public string TextInfos {
      get { return statusInfos.Text; }
      set { DoTryGetLabel( StatusLabels.Infos ).Text = value; }
    }

    /// <summary>
    /// Liste d'images associée à la barre d'état
    /// </summary>
    /// <remarks>
    /// Cette propriété n'est pas exposée en mode conception dans le composant <see cref="StatusStrip"/>
    /// </remarks>
    [
      Browsable(true),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible )
    ]
    public new ImageList ImageList {
      get { return base.ImageList; }
      set { base.ImageList = value; }
    }

    /// <summary>
    /// Détermine l'image associée à un label
    /// </summary>
    /// <param name="labelEnumOrName">clé d'énumération <see cref="StatusLabels"/>ou nom du label</param>
    /// <param name="imageIndexOrKey">index ou clé de l'image dans la liste d'images <see cref="ImageList"/></param>
    public void SetImage( object labelEnumOrName, object imageIndexOrKey ) {
      DoSetImage( DoTryGetLabel( labelEnumOrName ), imageIndexOrKey );
    }

    /// <summary>
    /// Détermine l'image isolée à associer à un label
    /// </summary>
    /// <param name="labelEnumOrName">code énuméré <see cref="StatusLabels"/>ou nom du label</param>
    /// <param name="image">image à associer au volet</param>
    public void SetImage( object labelEnumOrName, Image image ) {
      DoSetImage( DoTryGetLabel( labelEnumOrName ), image );
    }

    /// <summary>
    /// Met à jour les trois volets <see cref="StatusLeft"/>, <see cref="StatusMiddle"/> et <see cref="StatusInfos"/>.
    /// </summary>
    /// <param name="left">texte à affecter au volet <see cref="StatusLeft"/></param>
    /// <param name="middle">texte à affecter au volet <see cref="StatusMiddle"/></param>
    /// <param name="infos">texte à affecter au volet <see cref="StatusInfos"/></param>
    public void StatusUpdate( string left, string middle, string infos ) {
      TextLeft = left;
      TextMiddle = middle;
      TextInfos = infos;
    }

    /// <summary>
    /// Initialise les valeurs de la jauge de progression.
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    public void ProgressInitialize( int minimum, int maximum ) {
      statusProgress.SetProgressValues( minimum, minimum, maximum, statusProgress.Step );
    }

    /// <summary>
    /// Initialise les valeurs de la jauge de progression et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    /// <param name="step">valeur de l'incrément à chaque étape</param>
    public void ProgressInitialize( int minimum, int maximum, int step ) {
      statusProgress.SetProgressValues( minimum, minimum, maximum, step );
    }

    /// <summary>
    /// Met à jour les valeurs de la jauge et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="value">valeur courante de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    public void ProgressUpdate( int minimum, int value, int maximum ) {
      statusProgress.SetProgressValues( minimum, value, maximum, statusProgress.Step );
    }

    /// <summary>
    /// Met à jour les valeurs de la jauge et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="data">données fournissant les bornes et la valeur courante de la jauge</param>
    public void ProgressUpdate( ProgressData data ) {
      statusProgress.SetProgressValues( data.Minimum, data.Value, data.Maximum, data.Step );
    }

    /// <summary>
    /// Augmente la valeur de la jauge d'un pas.
    /// </summary>
    /// <remarks>
    /// Le pas d'incrémentation de la valeur de la jauge peut être fixé via <see cref="ProgressInitialize(int,int,int)"/>
    /// </remarks>
    public void ProgressStep() {
      statusProgress.PerformStep();
    }

    /// <summary>
    /// Masque la jauge de progression.
    /// </summary>
    public void ProgressFinish() {
      statusProgress.SetProgressValues( statusProgress.Minimum, statusProgress.Minimum, statusProgress.Maximum, statusProgress.Step );
    }

    /// <summary>
    /// Obtient ou détermine la valeur courante de la jauge de progression.
    /// </summary>
    [
      Browsable( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public int ProgresseValue {
      get { return statusProgress.Value; }
      set { statusProgress.SetProgressValues( statusProgress.Minimum, value, statusProgress.Maximum, statusProgress.Step ); }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                      StatusReporterLabel                                    //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension prenant en charge les automatismes requis par <see cref="StatusReporter"/>
  /// </summary>
  [ToolboxBitmap( typeof( ToolStripStatusLabel ), "ToolStripStatusLabel.bmp" )]
  public class StatusReporterLabel : ToolStripStatusLabel {

    private StatusReporter home = null;

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    /// <remarks>
    /// Les propriétés initialisées dans le constructeur doivent rester identiques aux valeurs
    /// par défaut des propriétés
    /// </remarks>
    protected StatusReporterLabel() {
      AutoToolTip = true;
      BorderSides = ToolStripStatusLabelBorderSides.All;
      ImageTransparentColor = Color.Magenta;
      ImageAlign = ContentAlignment.MiddleLeft;
      TextAlign = ContentAlignment.MiddleCenter;
    }

    /// <summary>
    /// Constructeur pour un volet interne
    /// </summary>
    /// <param name="name">nom du volet</param>
    /// <param name="home">composant <see cref="StatusReporter"/> propriétaire</param>
    internal StatusReporterLabel( string name, StatusReporter home ) : this() {
      base.Name = name;
      this.home = home;
    }

    /// <summary>
    /// Mise à jour de l'état de visibilité d'un volet
    /// </summary>
    /// <remarks>
    /// Escamote les volets vides en <see cref="ToolStripItem.AutoSize"/> qui ont seulement des bordures
    /// </remarks>
    private void DoVisibleUpdate() {
      if ( Spring || !AutoSize || BorderSides == ToolStripStatusLabelBorderSides.None ) return;
      bool visible = Image != null || !string.IsNullOrEmpty( Text );
      if ( Visible != visible ) Visible = visible;
    }

    /// <summary>
    /// Suppression de l'image actuellement associée au volet
    /// </summary>
    /// <remarks>
    /// Les images qui ne sont pas associées à une liste d'images sont automatiquement détruites
    /// </remarks>
    private void DoClearImage() {
      base.ImageKey = string.Empty;
      base.ImageIndex = -1;
      if ( base.Image != null ) {
        base.Image.Dispose();
        base.Image = null;
      }
    }

    /// <summary>
    /// Redéfinition permettant la mise à jour de l'état de visibilité des volets
    /// </summary>
    /// <param name="e">descripteur de l'événément</param>
    protected override void OnLayout( LayoutEventArgs e ) {
      DoVisibleUpdate();
      base.OnLayout( e );
    }

    /// <summary>
    /// Redéclaration permettant d'exposer la propriété.
    /// </summary>
    /// <remarks>
    /// Le setter est vide de manière à empêcher le changement du nom
    /// </remarks>
    [
      Browsable( true ),
      DisplayName( "(Name)" ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
      Description( "Obtient le nom du volet" ),
      Category( "Design" )
    ]
    public new string Name {
      get { return base.Name; }
      set { }
    }

    /// <summary>
    /// Obtient ou détermine si le texte de l'info-bulle est celui de la propriété Text ou celui de la propriété ToolTipText
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( true ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine si le texte de l'info-bulle est celui de la propriété Text ou celui de la propriété ToolTipText" ),
      CategoryAttribute( "Behavior" ) 
    ]
    public new bool AutoToolTip {
      get { return base.AutoToolTip; }
      set { base.AutoToolTip = value; }
    }

    /// <summary>
    /// Obtient ou détermine la placement du texte dans le volet
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( ContentAlignment.MiddleCenter ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la placement du texte dans le volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new ContentAlignment TextAlign {
      get { return base.TextAlign; }
      set { base.TextAlign = value; }
    }

    /// <summary>
    /// Obtient ou détermine la placement de l'image dans le volet
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( ContentAlignment.MiddleLeft ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la placement de l'image dans le volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new ContentAlignment ImageAlign {
      get { return base.ImageAlign; }
      set { base.ImageAlign = value; }
    }

    /// <summary>
    /// Obtient ou détermine l'image associée au volet
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( null ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine l'image associée au volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public override Image Image {
      get { return base.Image; }
      set {
        DoClearImage();
        base.Image = value;
        //DoVisibleUpdate();
      }
    }

    /// <summary>
    /// Obtient ou détermine la clé de l'image associée au volet
    /// </summary>
    [
      Browsable(true),
      DefaultValue( "" ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la clé de l'image associée au volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new string ImageKey {
      get { return base.ImageKey; }
      set { 
        DoClearImage();
        base.ImageKey = value; 
      }
    }

    /// <summary>
    /// Obtient ou détermine l'index de l'image associée au volet
    /// </summary>
    [
      Browsable(true),
      DefaultValue( -1 ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine l'index de l'image associée au volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new int ImageIndex {
      get { return base.ImageIndex; }
      set { 
        DoClearImage();
        base.ImageIndex = value; 
      }
    }

    /// <summary>
    /// Obtient ou détermine la couleur transparente de l'image
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( typeof(Color), "Magenta" ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la couleur transparente de l'image" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new Color ImageTransparentColor {
      get { return base.ImageTransparentColor; }
      set { base.ImageTransparentColor = value; }
    }

    /// <summary>
    /// Obtient ou détermine l'apparence des bordures du volet
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( ToolStripStatusLabelBorderSides.All ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine l'apparence des bordures du volet" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public new ToolStripStatusLabelBorderSides BorderSides {
      get { return base.BorderSides; }
      set { base.BorderSides = value; }
    }

    /// <summary>
    /// Obtient ou détermine si le volet standard est inclus dans les volets affichables
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Description( "Obtient ou détermine si le volet standard est inclus dans les volets affichables" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public bool Displayed {
      get {
        return Owner != null && Owner.Items.Contains( this );
      }
      set {
        if ( value == Displayed ) return;
        if ( value ) {
          if ( home == null ) return;
          home.Items.Add( this );
        }
        else {
          if ( Owner == null ) return;
          Owner.Items.Remove( this );
        }
      }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                  StatusReporterProgressBar                                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension prenant en charge les automatismes requis par <see cref="StatusReporter"/>
  /// </summary>
  [ToolboxBitmap( typeof( ToolStripProgressBar ), "ToolStripProgressBar.bmp" )]
  public class StatusReporterProgressBar : ToolStripProgressBar {

    private StatusReporter home = null;

    /// <summary>
    /// Constructeur par défaut
    /// </summary>
    protected StatusReporterProgressBar() {
    }

    /// <summary>
    /// Constructeur pour un volet interne
    /// </summary>
    /// <param name="name">nom du volet</param>
    /// <param name="home">composant <see cref="StatusReporter"/> propriétaire</param>
    internal StatusReporterProgressBar( string name, StatusReporter home ) : this() {
      this.home = home;
      base.Name = name;
    }

    /// <summary>
    /// Mise à jour de la jauge de progression
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="value">valeur courante de la jauge</param>
    /// <param name="maximum">valeur maximale de la jauge</param>
    /// <param name="step">incrément de la valeur dans une modification de la valeur via <see cref="StatusReporter.ProgressStep()"/></param>
    public void SetProgressValues( int minimum, int value, int maximum, int step ) {
      //System.Diagnostics.Trace.TraceInformation( "Jauge valeurs : {0}-{1}-{2}", minimum, value, maximum );

      // écrêtage
      minimum = minimum < 0 ? 0 : minimum;
      value = value < minimum ? minimum : value > maximum ? maximum + 1 : value;
      step = step < 1 ? 1 : step;

      // visibilité de la jauge
      bool visible = minimum < maximum && value <= maximum;
      if ( visible != Visible ) Visible = visible;
      if ( !visible ) return;

      // mise à jour des valeurs courantes
      if ( base.Minimum != minimum ) base.Minimum = minimum;
      if ( base.Maximum != maximum ) base.Maximum = maximum;
      if ( base.Value   != value   ) base.Value   = value;
      if ( base.Step    != step    ) base.Step    = step;

      //System.Diagnostics.Trace.TraceInformation( "Jauge visible : {0}-{1}-{2}", base.Minimum, base.Value, base.Maximum );
    }

    /// <summary>
    /// Redéclaration permettant d'exposer la propriété.
    /// </summary>
    /// <remarks>
    /// Le setter est vide de manière à empêcher le changement du nom
    /// </remarks>
    [
      Browsable( true ),
      DisplayName( "(Name)" ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden ),
      Description( "Obtient le nom du volet" ),
      Category( "Design" )
    ]
    public new string Name {
      get { return base.Name; }
      set { }
    }

    /// <summary>
    /// Redéclaration permettant une gestion homogène de la jauge
    /// </summary>
    public new void PerformStep() {
      SetProgressValues( Minimum, Value + Step, Maximum, Step );
    }

    /// <summary>
    /// Obtient ou détermine la borne minimale de la jauge de progression
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( 0 ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la borne minimale de la jauge de progression"),
      Category( "Design" ) 
    ]
    public new int Minimum {
      get { return base.Minimum; }
      set { SetProgressValues( value, Value, Maximum, Step ); }
    }

    /// <summary>
    /// Obtient ou détermine la borne maximale de la jauge de progression
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( 100 ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la borne maximale de la jauge de progression"),
      Category( "Design" ) 
    ]
    public new int Maximum {
      get { return base.Maximum; }
      set { SetProgressValues( Minimum, Value, value, Step ); }
    }

    /// <summary>
    /// Obtient ou détermine la valeur courante de la jauge de progression
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( 0 ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Visible ),
      Description( "Obtient ou détermine la valeur courante de la jauge de progression"),
      Category( "Design" ) 
    ]
    public new int Value {
      get { return base.Value; }
      set { SetProgressValues( Minimum, value, Maximum, Step ); }
    }

    /// <summary>
    /// Obtient ou détermine si le volet standard est inclus dans les volets affichables
    /// </summary>
    [
      Browsable( true ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Description( "Obtient ou détermine si le volet standard est inclus dans les volets affichables" ),
      CategoryAttribute( "Appearance" ) 
    ]
    public bool Displayed {
      get {
        return Owner != null && Owner.Items.Contains( this );
      }
      set {
        if ( value == Displayed ) return;
        if ( value ) {
          if ( home == null ) return;
          home.Items.Add( this );
        }
        else {
          if ( Owner == null ) return;
          Owner.Items.Remove( this );
        }
      }
    }
  }

}
