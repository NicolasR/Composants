/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 06 12 2008 : version initiale
 * 25 04 2009 : amélioration du filtrage dans ToolTipText et Text
 * 26 11 2010 : amélioration de l'activation du client même si le client n'est pas focusable
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Psl.Controls {

  /// <summary>
  /// Page d'un contrôle <see cref="TabDocker"/>.
  /// </summary>
  /// <remarks>
  /// Une page d'un contrôle <see cref="TabDocker"/> fonctionne comme un conteneur volatile 
  /// à l'égard d'un contrôle <see cref="Client"/>. 
  /// <br/>
  /// Le premier contrôle enfant ajouté à la page est considéré par défaut comme le <see cref="Client"/>
  /// de la page. 
  /// <br/>
  /// La page se détruit automatiquement quand on lui retire le dernier contrôle enfant qu'elle contenait.
  /// </remarks>
  [
    ToolboxItem( false ),
    DesignTimeVisible( false ), 
    DefaultProperty( "Text" ), 
    DefaultEvent( "Click" ),
    Designer( typeof( Psl.Design.TabDockerPageDesigner ) ), // Designer( "Psl.Design.TabDockerDesigner, " + Psl.AssemblyRef.PslCoreDesign ),
  ]
  public partial class TabDockerPage : TabPage {

    //
    // Champs
    //

    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Référence sur le client courant de la page.
    /// </summary>
    private Control client = null;

    /// <summary>
    /// Libellé complet de la page hors toute troncature
    /// </summary>
    private string textFull = string.Empty;

    /// <summary>
    /// Mémorisation de l'option <see cref="AutoEllipsis"/>
    /// </summary>
    private bool autoEllipsis = true;


    /// <summary>
    /// Mémorisation de l'option <see cref="AutoEllipsisLength"/>
    /// </summary>
    private int autoEllipsisLength = 30;

    //
    // Gestion générale
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public TabDockerPage() {
      InitializeComponent();
    }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="text">libellé de la page</param>
    public TabDockerPage(string text) : this() {
      Text = text; // pour effectuer la réduction 
    }

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
      AutoEllipsis = true;
      AutoEllipsisLength = 30;
      AutoToolTip = true;
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

    //
    // Service
    //

    internal void ActivatePage() {
      if ( DesignMode ) return;
      if ( client == null ) return;
      IContainerControl container = this.GetFirstContainerControl();
      if ( container == null ) return;
      container.ActiveControl = client;
    }

    /// <summary>
    /// Effectue la réduction du texte selon <see cref="AutoEllipsis"/> et <see cref="AutoEllipsisLength"/>.
    /// </summary>
    /// <param name="text">libellé complet de la page</param>
    /// <returns>libellé éventuellement réduit à afficher</returns>
    private string DoAutoEllipsis( string text ) {
      if ( !AutoEllipsis || AutoEllipsisLength <= 0 || text == null || text.Length <= AutoEllipsisLength ) return text;
      return text.Substring( 0, AutoEllipsisLength ) + "...";
    }

    //
    // Redéfinition de méthodes et de propriétés héritées
    //

    /// <summary>
    /// Surveille les contrôles enfants ajoutés pour tenir à jour le <see cref="Client"/> de la page.
    /// </summary>
    /// <remarks>
    /// Lorsque le client est null, le premier contrôle ajouté devient le client.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnControlAdded( ControlEventArgs e ) {
      base.OnControlAdded( e );
      if ( client == null ) client = e.Control;
    }

    /// <summary>
    /// Surveille les contrôles enfants supprimés pour tenir à jour le <see cref="Client"/> de la page.
    /// </summary>
    /// <remarks>
    /// La page se détruit automatiquement quant le client est détruit ou quand il n'y a plus de contrôles dans la page.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnControlRemoved( ControlEventArgs e ) {
      if ( client == e.Control ) client = null;
      base.OnControlRemoved( e );
      if ( !Disposing && (client == null) || (Controls.Count == 0) ) Dispose();
    }

    /// <summary>
    /// Transmet le focus au <see cref="Client"/> si <see cref="TabDocker.AutoFocusClient"/> est armé.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnGotFocus( EventArgs e ) {
      base.OnGotFocus( e );
      TabDocker parent = Parent as TabDocker;
      if ( parent == null ) return;
      if ( parent.AutoFocusClient && client != null ) ActivatePage();
    }

    //
    // Interface d'utilisation externe
    //

    /// <summary>
    /// Obtient ou détermine le client courant de la page.
    /// </summary>
    /// <exception cref="ArgumentException">si le contrôle affecté à la propriété ne figure pas parmi les contrôles enfants</exception>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
    ]
    public Control Client {
      get { return client; }
      set {
        if ( value == null ) Dispose();
        if ( value == client ) return;
        if ( !Controls.Contains( value ) ) Controls.Add( value );
        client = value;
      }
    }

    /// <summary>
    /// Obtient ou détermine si le libellé de la page est automatiquement réduit s'il dépasse le nombre de caractères spécifié par <see cref="AutoEllipsisLength"/>.
    /// </summary>
    [ 
      DefaultValue(true),
      RefreshProperties(RefreshProperties.All ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine si le libellé de la page est automatiquement tronqué s'il dépasse le nombre de caractères spécifié par AutoEllipsisLength")
    ]
    public bool AutoEllipsis {
      get { return autoEllipsis; }
      set {
        if ( value == autoEllipsis ) return;
        autoEllipsis = value;
        Text = textFull;
      }
    }

    /// <summary>
    /// Obtient ou détermine le nombre de caractères à partir duquel le libellé est tronqué si <see cref="AutoEllipsis"/> est true.
    /// </summary>
    [ 
      DefaultValue(30),
      RefreshProperties(RefreshProperties.All ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine le nombre de caractères à partir duquel le libellé est tronqué si AutoEllipsis est true")
    ]
    public int AutoEllipsisLength {
      get { return autoEllipsisLength; }
      set {
        if ( value == autoEllipsisLength ) return;
        autoEllipsisLength = value;
        Text = textFull;
      }
    }

    /// <summary>
    /// Obtient ou détermine si le libellé complet de la page est automatiquement considéré comme un <see cref="TabPage.ToolTipText"/>.
    /// </summary>
    [ 
      DefaultValue(true),
      RefreshProperties(RefreshProperties.All ),
      Category( "Behavior" ),
      Description( "Obtient ou détermine si le libellé complet de la page est automatiquement considéré comme un ToolTipText")
    ]
    public bool AutoToolTip { get; set; }

    /// <summary>
    /// Obtient ou détermine la bulle d'aide associée à l'onglet.
    /// </summary>
    [
      Browsable( true ),
      DefaultValue( "(auto)" ),
      Localizable( true ),
      Description( "Obtient ou détermine la bulle d'aide affichée lorsque la souris passe au-dessus de l'onglet associé à la page" )
    ]
    public new string ToolTipText {
      get { return DesignMode && AutoToolTip ? "(auto)" : base.ToolTipText; }
      set {
        if ( ToolTipText == value ) return;
        base.ToolTipText = value; 
      }
    }

    /// <summary>
    /// Obtient ou détermine le libellé de l'onglet en appliquant éventuellement une troncature.
    /// </summary>
    /// <seealso cref="AutoEllipsis"/>
    /// <see cref="TextFull"/>
    [
      Browsable( false ),
      //Localizable( true ),
      //RefreshProperties(RefreshProperties.All ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
      //EditorBrowsable( EditorBrowsableState.Always ),
      //Description( "Obtient ou détermine le libellé éventuellement tronqué de l'onglet" )
    ]
    public override string Text {
      get {
        return base.Text;
      }
      set {
        if ( value == null ) value = string.Empty;
        textFull = value;
        ToolTipText = value;
        string text = DoAutoEllipsis( value );
        if ( text == Text ) return;
        base.Text = text;
      }
    }

    /// <summary>
    /// Obtient ou détermine le libellé complet de l'onglet.
    /// </summary>
    /// <seealso cref="AutoEllipsis"/>
    /// <seealso cref="Text"/>
    [
      Browsable( true ),
      Localizable( true ),
      DefaultValue( "" ),
      DisplayName( "Text" ),
      RefreshProperties(RefreshProperties.All ),
      EditorBrowsable( EditorBrowsableState.Always ),
      Description( "Obtient ou détermine le libellé complet de l'onglet" )
    ]
    public virtual string TextFull {
      get {
        return textFull;
      }
      set {
        Text = value;
      }
    }
  }
}
