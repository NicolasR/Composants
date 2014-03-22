/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Crédits : 
 * - Jeroen Landheer pour Code Project
 * - Microsoft Corp.
 * 
 * Version complète assurant la compatibilité de toutes les versions de Windows
 * 
 * 10 12 2008 : version initiale
 * 12 01 2010 : protection de la propriété ReadyState contre les exception intempestives
 * 11 05 2010 : amélioration de la valeur de la propriété ActualTitle
 * 27 05 2010 : protection de la propriété StatusText contre les exception intempestives
 * 31 05 2010 : protection de la propriété Document contre les exception UnautorizedAccessException (vs 2010)
 * 01 06 2010 : modification de ActualUrl pour corriger des problèmes de codage UTF-8/UTF-16 en VS 2010
 * 07 02 2011 : adjonction de StateChanged, ActualProgressState, ActualReadyState
 * 17 02 2011 : adjonction des fonctionnalités de déclenchement des dialogues usuels IE
 * 17 02 2011 : adjonction des fonctionnalités liées au zoom optique
 * 19 02 2011 : adjonction des fonctionnalités de recherche de chaînes dans une page
 * 02 07 2011 : adjonction de la prise en charge de la validatiob de la commande contextuelle des onglets
 */                                                                        // <wao never.end>

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;
using Psl.Applications;
using Psl.Windows;
using Microsoft.Win32;
using System.Diagnostics;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                       Flags NWMF                                            //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////
  partial class WebBrowserEx {

    /// <summary>
    /// Flags transmis via l'événement NewWindoW3 pour le descripteur <see cref="NewWindowExEventArgs"/>
    /// </summary>
    /// <remarks>
    /// Dans le cas du composant de navigation, seul le flag <see cref="NWMF_FORCETAB"/> est en fait à tester
    /// pour savoir s'il faut ouvrir un nouvel onglet ou non.
    /// <br/>
    /// Toutefois, ce flag n'est positionné que via la commande de menu contextuel "ouvrir dans un nouvel onglet",
    /// la quelle commande n'est validée qu'à condition que l'application hôte soit enregistrée dans le 
    /// registre de Windows comme prenant en charge la navigation multi-onglets : basculer la propriété de classe
    /// <see cref="IsTabEnabled"/> à true.
    /// </remarks>
    public enum NWMF {

      /// <summary>
      /// Aucun flag armé
      /// </summary>
      NWMF_NONE = 0x00000000,

      /// <summary>
      /// La page est en cours de déchargement
      /// </summary>
      NWMF_UNLOADING = 0x00000001,

      /// <summary>
      /// L'ouverture résulte d'une action de l'utilisateur
      /// </summary>
      NWMF_USERINITED = 0x00000002,

      /// <summary>
      /// Indique si l'ouverture est la première (dans le cas où le flag NWMF_USERINITED est armé)
      /// </summary>
      NWMF_FIRST = 0x00000004,

      /// <summary>
      /// La touche "override" (ALT) était enfoncée
      /// </summary>
      NWMF_OVERRIDEKEY = 0x00000008,

      /// <summary>
      /// La nouvelle fenêtre à charger provient d'une requête d'affichage de l'aide
      /// </summary>
      NWMF_SHOWHELP = 0x00000010,

      /// <summary>
      /// La fenêtre est une boîte de dialogue qui affiche un contenu html
      /// </summary>
      NWMF_HTMLDIALOG = 0x00000020,

      /// <summary>
      /// La fenêtre à ouvrir est appelée depuis une boîte de dialogue à contenu html
      /// </summary>
      NWMF_FROMDIALOGCHILD = 0x00000040,

      /// <summary>
      /// La fenêtre est réellement requise par l'utilisateur 
      /// </summary>
      NWMF_USERREQUESTED = 0x00000080,

      /// <summary>
      /// Résult d'une commande de rafraîchissement
      /// </summary>
      NWMF_USERALLOWED = 0x00000100,

      /// <summary>
      /// La nouvelle fenêtre devrait être forcée dans une nouvelle fenêtre plutôt que dans un nouvel onglet
      /// </summary>
      NWMF_FORCEWINDOW = 0x00010000,

      /// <summary>
      /// La nouvelle fenêtre devrait être ouverte dans un nouvel onglet
      /// </summary>
      NWMF_FORCETAB = 0x00020000,

      /// <summary>
      /// La fenêtre devrait être ouverte comme une fenêtre, sauf si NWMF_FORCETAB est armé
      /// </summary>
      NWMF_SUGGESTWINDOW = 0x00040000,

      /// <summary>
      /// La fenêtre devrait être ouverte comme un onglet, sauf si NWMF_FORCEWINDOW est armé
      /// </summary>
      NWMF_SUGGESTTAB = 0x00080000,

      /// <summary>
      /// L'événement est déclenché depuis un onglet inactif
      /// </summary>
      NWMF_INACTIVETAB = 0x00100000
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Descripteur NewWindowExEventArgs                                //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////
                                                                               // <wao NewWindowExEventArgs.begin>
  /// <summary>
  /// Descripteur pour l'événement <see cref="WebBrowserEx.NewWindowEx"/>.
  /// </summary>
  public class NewWindowExEventArgs : CancelEventArgs {

                                                                             // <wao never.begin>
    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="automationObject">objet automation prenant en charge la navigation</param>
    /// <param name="url">url cible de la navigation</param>
    /// <param name="urlContext">url du contexte depuis lequel la navigation est déclenchée</param>
    /// <param name="flags">flags précisant la source et le contexte de l'opération</param>
    public NewWindowExEventArgs( object automationObject = null, string url = "", string urlContext = "", WebBrowserEx.NWMF flags = WebBrowserEx.NWMF.NWMF_NONE ) {
      AutomationObject = automationObject;
      Url = url;
      UrlContext = urlContext;
      Flags = flags;
    }

    /// <summary>
    /// Url cible de la navigation
    /// </summary>
    public string Url { get; private set; }

    /// <summary>
    /// Url du contexte d'où provient la demande de navigation
    /// </summary>
    public string UrlContext { get; private set; }

    /// <summary>
    /// Flags spécifiant les circonstances de l'ouverture de la fenêtre
    /// </summary>
    public WebBrowserEx.NWMF Flags { get; private set; }
                                                                            // <wao never.end>

    /// <summary>
    /// Obtient ou détermine l'objet automation prenant en charge la navigation
    /// </summary>
    /// <remarks>
    /// Pour que la navigation s'effectue dans une nouvelle fenêtre externe, ne pas affecter cette propriété.
    /// Pour provoquer une navigation au sein de l'application (par exemple pour une navigation multi-onglets),
    /// instancier un nouvel objet <see cref="WebBrowserEx"/>, puis affecter la valeur de la propriété 
    /// <see cref="WebBrowserEx.AutomationObject"/> à AutomationObject.
    /// </remarks>
    public object AutomationObject { get; set ;}
  }                                                                            // <wao NewWindowExEventArgs.end>

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Délégué NewWindowExEventHandler                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Type de délégué pour l'événement <see cref="WebBrowserEx.NewWindowEx"/>.
  /// </summary>
  /// <param name="sender">source de l'événement</param>
  /// <param name="e">descripteur de l'événement</param>
  public delegate void NewWindowExEventHandler( object sender, NewWindowExEventArgs e ) ;

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension du contrôle <see cref="WebBrowser"/> autorisant une navigation multi-onglets.
  /// </summary>
  [
  PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"), 
  PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"),
  Description( "Extension du contrôle autorisant une navigation multi-onglets" )
  ]
  public partial class WebBrowserEx : WebBrowser {

    //
    // Champs
    //

    private NativeMethods.IWebBrowser2 axIWebBrowser2;
    private AxHost.ConnectionPointCookie cookie;
    private WebBrowserExSink sink;

    private ProgressData lastProgressState = new ProgressData();
    private int lastZoomRange = 100;

    //
    // Initialisation, finalisation et connexions à l'automation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public WebBrowserEx() {
      ScriptErrorsSuppressed = true;
    }

    /// <summary>
    /// Appelée par l'infrastructure lorsque le contrôle ActiveX sous-jacent est créé. 
    /// </summary>
    /// <param name="nativeActiveXObject">objet ActiveX natif assurant la navigation</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void AttachInterfaces( object nativeActiveXObject ) {
      this.axIWebBrowser2 = (NativeMethods.IWebBrowser2) nativeActiveXObject;
      base.AttachInterfaces( nativeActiveXObject );
    }

    /// <summary>
    /// Appelée par l'infrastructure lorsque le contrôle ActiveX sous-jacent est détruit. 
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void DetachInterfaces() {
      this.axIWebBrowser2 = null;
      base.DetachInterfaces();
    }

    /// <summary>
    /// Appelée par l'infrastructure pour permettre la connexion du puits d'événements
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void CreateSink() {
      base.CreateSink();
      sink = new WebBrowserExSink( this );
      cookie = new AxHost.ConnectionPointCookie( ActiveXInstance, sink, typeof( NativeMethods.DWebBrowserEvents2 ) );
    }

    /// <summary>
    /// Appelée par l'infrastructure pour permettre la déconnexion du puits d'événements
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void DetachSink() {
      if ( cookie != null ) cookie.Disconnect();
      cookie = null;
      sink = null;
      base.DetachSink();
    }

    //
    // Contrôles
    //

    private void DoCheckDocument( string operation ) {
      if ( Document == null ) throw new InvalidOperationException( operation + " requiert qu'une page html soit chargée dans le navigateur" );
    }

    private NativeMethods.IOleCommandTarget DoCheckOleCommandTarget( string operation ) {
      NativeMethods.IOleCommandTarget result = axIWebBrowser2.Document as NativeMethods.IOleCommandTarget;
      if ( result == null ) throw new InvalidOperationException( operation + " requiert qu'une page html soit chargée dans le navigateur" );
      return result;
    }

    private void DoCheckString( string value, string operation, string paramName = "text" ) {
      if ( string.IsNullOrEmpty( value ) ) throw new ArgumentException( operation + " : la chaîne de caractères ne peut être ni null ni vide", paramName ); 
    }

    //
    // Service
    //

    // Les méthodes de recherche sont programmées en style dynamic pour éviter la référence à 
    // la librairie Microsoft.mshtml.dll dans la librairie de base Psl.Core
    // La documentation des interfaces, objets et méthodes est celle de IHTMLDocument2

    // méthode centralisée pour atteindre une occurrence, et éventuellement la sélectionner ou la colorier
    private bool DoFindText( string text, ref dynamic anchor, bool next, bool forward, bool select, Color color, bool wholeWord, bool matchCase ) {

      // déterminer l'ancre de base pour la recherche : corps de la page ou sélection en cours
      if ( anchor == null ) {
        dynamic document = Document.DomDocument;         // IHTMLDocument2 document = Document.DomDocument as IHTMLDocument2;
        if ( next )
          anchor = document.selection.createRange();     // IHTMLTxtRange anchor = document.selection.createRange() as IHTMLTxtRange;
        else {
          anchor = document.body.createTextRange();      // IHTMLTxtRange anchor = document.body.createTextRange() as IHTMLTxtRange;
          anchor.collapse( forward );
        }
      }

      // préparer l'ancre pour le point de départ de la recherche
      anchor.collapse( !forward );

      // déterminer l'étendue de la recherche : positive vers la fin, négative vers le début
      int scope = forward ? int.MaxValue : int.MinValue;

      // préparer les flags d'options de la recherche
      int flags = 0;
      if ( wholeWord ) flags |= 0x00000002;
      if ( matchCase ) flags |= 0x00000004;

      // effectuer la recherche
      bool found = anchor.findText( text, scope, flags );
      if ( !found ) return false;

      // sélectionner l'occurrence ou colorier son arrière-plan
      if ( select ) 
        anchor.select();
      else 
        anchor.execCommand( "BackColor", false, color );

      // occurrence trouvée
      return true;
    }

    // rechercher une occurrence
    private bool DoFindText( string text, bool next, bool forward, bool select, Color color, bool wholeWord, bool matchCase ) {
      dynamic anchor = null;                      // IHTMLTxtRange anchor = null
      return DoFindText( text, ref anchor, next, forward, select, color, wholeWord, matchCase );
    }

    // colorier toutes les occurrences
    private int DoFindText( string text, Color color, bool wholeWord, bool matchCase ) {
      dynamic anchor = null;                     // IHTMLTxtRange anchor
      int count = 0;

      // parcourir les occurrences
      do {
        if ( !DoFindText( text, ref anchor, anchor != null, true, false, color, wholeWord, matchCase ) ) break;
        count++;
      } while ( true );

      // nombre d'occurrences trouvées
      return count;
    }

    //
    // Déclenchement centralisé des événements
    //
                                                                               
    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.DocumentCompleted"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]           // <wao never>
    protected override void OnDocumentCompleted( WebBrowserDocumentCompletedEventArgs e ) {  // <wao OnDocumentCompleted.begin>
      try {
        base.OnDocumentCompleted( e );
        OnStateChanged( EventArgs.Empty );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }                                                                          // <wao OnDocumentCompleted.end>

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.Navigating"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnNavigating( WebBrowserNavigatingEventArgs e ) {
      try {
        base.OnNavigating( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.Navigated"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnNavigated( WebBrowserNavigatedEventArgs e ) {
      try {
        base.OnNavigated( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.ProgressChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnProgressChanged( WebBrowserProgressChangedEventArgs e ) {
      try {
        lastProgressState = new ProgressData( 0, (int) e.CurrentProgress, (int) e.MaximumProgress, 1 );
        base.OnProgressChanged( e );
        OnStateChanged( EventArgs.Empty );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.StatusTextChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnStatusTextChanged( EventArgs e ) {
      try {
        base.OnStatusTextChanged( e );
        OnStateChanged( EventArgs.Empty );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.DocumentTitleChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDocumentTitleChanged( EventArgs e ) {
      try {
        base.OnDocumentTitleChanged( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.EncryptionLevelChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnEncryptionLevelChanged( EventArgs e ) {
      try {
        base.OnEncryptionLevelChanged( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement de l'événement <see cref="WebBrowser.FileDownload"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont interceptées et relayées via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnFileDownload( EventArgs e ) {
      try {
        base.OnFileDownload( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="NewWindowEx"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected virtual void OnNewWindowEx( NewWindowExEventArgs e ) {
      try {
        if ( NewWindowEx != null ) NewWindowEx( this, e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="StateChanged"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected virtual void OnStateChanged( EventArgs e ) {
      try {
        if ( StateChanged != null ) StateChanged( this, e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    //
    // Fonctionnalités redéclarées ou redéfinies
    //

    /// <summary>
    /// Obtient la référence sur le document chargé dans le contrôle <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette redéclaration est simplement destinée à absorber d'éventuelles exceptions qui
    /// se produisent lorsque le navigateur n'est pas accessible. 
    /// </remarks>
    [Browsable( false )]
    public new HtmlDocument Document {
      get {
        try {
          return base.Document;
        }
        catch ( System.UnauthorizedAccessException ) { return null; }
      }
    }

    /// <summary>
    /// Obtient une valeur indiquant l'état actuel du contrôle <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette redéclaration est simplement destinée à absorber d'éventuelles exceptions qui
    /// se produisent lorsque le navigateur n'est pas accessible. 
    /// </remarks>
    [Browsable( false )]
    public new WebBrowserReadyState ReadyState {
      get {
        try {
          return base.ReadyState;
        }
        catch { return WebBrowserReadyState.Uninitialized; }
      }
    }

    /// <summary>
    /// Obtient le status text du contrôle <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette redéclaration est simplement destinée à absorber d'éventuelles exceptions qui
    /// se produisent lorsque le navigateur n'est pas accessible. 
    /// </remarks>
    [Browsable( false )]
    public new string StatusText {
      get {
        try {
          return base.StatusText;
        }
        catch { return string.Empty; }
      }
    }

    /// <summary>
    /// Obtient ou détermine si le navigateur supprime les boîtes de dialogue concernant les erreurs de scripts.
    /// </summary>
    [
     Category( "Behavior" ), 
     DefaultValue( true ),
     Description( "Obtient ou détermine si le navigateur supprime boîtes de dialogue concernant les erreurs de scripts" ),
    ]
    public new bool ScriptErrorsSuppressed {
      get { return base.ScriptErrorsSuppressed ; }
      set { base.ScriptErrorsSuppressed = value ; }
    } 

    //
    // Fonctionnalités ajoutées à WebBrowser
    //

    /// <summary>
    /// Affiche le dialogue des options Internet Explorer
    /// </summary>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement chargée dans le navigateur</exception>
    public void ShowInternetOptions() {
      NativeMethods.IOleCommandTarget target = DoCheckOleCommandTarget( "ShowInternetOptions" );

      object nothing = new object();
      try {
        target.Exec( ref NativeMethods.ExtraWebBrowserOleCommandGuid, (uint) NativeMethods.ExtraWebBrowserOleCommand.Options, (uint) NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref nothing, ref nothing );
      }
      catch ( Exception ex ) {
        if ( NativeMethods.IsNotIEStandardException( ex ) ) throw;
      }
    }

    /// <summary>
    /// Affiche la source de la page actuellement chargée dans NotePad
    /// </summary>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement chargée dans le navigateur</exception>
    public void ViewSource() {
      NativeMethods.IOleCommandTarget target = DoCheckOleCommandTarget( "ViewSource" );

      object nothing = new object();
      try {
        target.Exec( ref NativeMethods.ExtraWebBrowserOleCommandGuid, (uint) NativeMethods.ExtraWebBrowserOleCommand.ViewSource, (uint) NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref nothing, ref nothing );
      }
      catch ( Exception ex ) {
        if ( NativeMethods.IsNotIEStandardException( ex ) ) throw;
      }
    }

    /*
    /// <summary>
    /// Affiche de dialogue de recherche d'une chaîne dans une page
    /// </summary>
    /// <remarks>
    /// Ce dialogue est lié à l'instance de WebBrowser pour le compte de laquelle le dialogue a été ouvert.
    /// Il n'est pas adapté au contexte multi-onglets car le dialogue ne s'applique pas à la page actuellement visible.
    /// </remarks>
    public void ShowFindDialog() {
      object nullObjectArray = null;
      axIWebBrowser2.ExecWB( NativeMethods.OLECMDID.OLECMDID_FIND, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero );
    }
    */

    /// <summary>
    /// Recherche et sélectionne une chaîne dans le texte de la page courante.
    /// </summary>
    /// <param name="text">chaîne à rechercher</param>
    /// <param name="next">true pour forcer la recherche de l'occurrence suivante</param>
    /// <param name="forward">true pour rechercher l'occurrence suivante, false pour rechercher l'occurrence précédente</param>
    /// <param name="wholeWord">true pour une recherche en mots entiers seulement</param>
    /// <param name="matchCase">true pour recherche sensible à la casse</param>
    /// <returns>true si une occurrence de la chaîne a été trouvée</returns>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement chargée dans le navigateur</exception>
    /// <exception cref="ArgumentException">la chaîne à rechercher <paramref name="text"/> est null ou vide</exception>
    public bool FindText( string text, bool next = true, bool forward = true, bool wholeWord = false, bool matchCase = false ) {
      DoCheckDocument( "FindText" );
      DoCheckString( text, "FindText" );
      return DoFindText( text, next, forward, true, Color.White, wholeWord, matchCase );
    }

    /// <summary>
    /// Recherche et colorie l'arrière-plan de toutes les occurrences d'une chaîne dans la page courante. 
    /// </summary>
    /// <param name="text">chaîne à rechercher</param>
    /// <param name="color">couleur d'arrière-plan pour signaler les occurrences trouvées</param>
    /// <param name="wholeWord">true pour une recherche en mots entiers seulement</param>
    /// <param name="matchCase">true pour recherche sensible à la casse</param>
    /// <returns>le nombre d'occurrences trouvées</returns>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement chargée dans le navigateur</exception>
    /// <exception cref="ArgumentException">la chaîne à rechercher <paramref name="text"/> est null ou vide</exception>
    public int FindText( string text, Color color, bool wholeWord = false, bool matchCase = false ) {
      DoCheckDocument( "FindText" );
      DoCheckString( text, "FindText" );
      return DoFindText( text, color, wholeWord, matchCase );
    }

    //
    // Propriétés ajoutées à WebBrowser
    //

    /// <summary>
    /// Obtient l'objet automation associé au navigateur
    /// </summary>
    [Browsable( false )]
    public object AutomationObject {
      get { return axIWebBrowser2.Application; }
    }

    /// <summary>
    /// Obtient l'url apparente de la page selon son genre (fichier local ou fichier distant)
    /// </summary>
    /// <remarks>
    /// Retourne <see cref="Uri.LocalPath"/> si c'est un fichier local, <see cref="Uri.ToString()"/> sinon. 
    /// <br/>
    /// En vs 2010, <see cref="Uri.AbsoluteUri"/> est codé en UTF-8 et pose problème avec
    /// les url comportant des caractères accentués. 
    /// </remarks>
    [Browsable( false )]
    public string ActualUrl {
      get { return Url == null ? string.Empty : Url.IsFile ? Url.LocalPath : Url.ToString(); }
    //get { return Url == null ? string.Empty : Url.IsFile ? Url.LocalPath : Url.AbsoluteUri; } // vs2008
    }
    
    /// <summary>
    /// Obtient le titre apparent de la page selon son genre (fichier local ou fichier distant)
    /// </summary>
    [Browsable( false )]
    public string ActualTitle {
      get { return Url == null ? string.Empty : Document == null || DocumentTitle == string.Empty ? Path.GetFileName( Url.LocalPath ) : DocumentTitle; }
    }

    /// <summary>
    /// Obtient l'url de la favicon de la page courante, ou chaîne vide si aucune favicon n'est déterminée
    /// </summary>
    [Browsable( false )]
    public string ActualFaviconUrl {
      get { return GetFaviconUrl(); }
    }

    /// <summary>
    /// Obtient le "StatusText" le plus pertinent pour l'état courant du navigateur
    /// </summary>
    [Browsable( false )]
    public string ActualStatusText {
      get {
        string result = StatusText;
        if ( string.IsNullOrEmpty( result ) || result == "Terminé" ) result = ActualUrl;
        return result;
      }
    }

    /// <summary>
    /// Obtient la chaîne d'état de disponibilité du navigateur
    /// </summary>
    [Browsable( false )]
    public string ActualReadyState {
      get {
        switch ( ReadyState ) {
          case WebBrowserReadyState.Uninitialized: return "Non initialisé";
          case WebBrowserReadyState.Loading: return "Chargement";
          case WebBrowserReadyState.Loaded: return "Chargé";
          case WebBrowserReadyState.Interactive: return "Interactif";
          case WebBrowserReadyState.Complete: return "Terminé";
          default: return ReadyState.ToString();
        }
      }
    }

    /// <summary>
    /// Obtient l'état courant de la jauge de progression
    /// </summary>
    [Browsable( false )]
    public ProgressData ActualProgressState {
      get { return lastProgressState; }
    }

    /// <summary>
    /// Obtient l'intervalle possible pour le zoom optique <see cref="OpticalZoom"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">si aucune page n'est chargée ou si l'opération n'est pas possible dans l'état courant du navigateur</exception>
    [Browsable( false )]
    public Range OpticalZoomRange {
      get {
        DoCheckDocument( "OpticalZoomRange" );
        object data = 0;
        object result = 0;
        try {
          axIWebBrowser2.ExecWB( NativeMethods.OLECMDID.OLECMDID_OPTICAL_GETZOOMRANGE, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref data, ref result );
        }
        catch ( COMException ex ) {
          throw new InvalidOperationException( "L'accès à la plage de variation du zoom optique n'est pas possible", ex );
        }

        Point split = Win.Util.LParamToPoint( (int) result );
        return new Range( split.X, split.Y );
      }
    }

    /// <summary>
    /// Obtient ou détermine la valeur du zoom optique
    /// </summary>
    /// <remarks>
    /// Comme il n'y a pas de moyen (que je connaisse) pour connaître le facteur de zoom optique courant,
    /// la propriété OpticalZoom tient à jour un champ local pour mémoriser le dernier facteur de zoom optique connu. 
    /// </remarks>
    /// <exception cref="InvalidOperationException">si aucune page n'est chargée ou si l'opération n'est pas possible dans l'état courant du navigateur</exception>
    [
     Browsable( false ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public int OpticalZoom {
      get {
        return lastZoomRange;
      }
      set {
        DoCheckDocument( "OpticalZoomRange" );
        lastZoomRange = value;
        object factor = value;
        object result = new object();
        try {
          axIWebBrowser2.ExecWB( NativeMethods.OLECMDID.OLECMDID_OPTICAL_ZOOM, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref factor, ref result );
        }
        catch ( COMException ex ) {
          throw new InvalidOperationException( "La modification du zoom optique n'est pas possible", ex );
        }
        OnStateChanged( EventArgs.Empty );
      }
    }

    //
    // Evénements ajoutés à WebBrowser
    //

    /// <summary>
    /// Déclenché lorsque la navigation va provoquer l'ouverture d'une nouvelle fenêtre.
    /// </summary>
    /// <seealso cref="NewWindowExEventArgs"/>
    [Description( "Déclenché lorsque la navigation va provoquer l'ouverture d'une nouvelle fenêtre" )]
    public event NewWindowExEventHandler NewWindowEx;

    /// <summary>
    /// Déclenché lorsqu'une information concernant l'état courant du navigateur a changé
    /// </summary>
    [Description( "Déclenché lorsqu'une information concernant l'état courant du navigateur a changé" )]
    public event EventHandler StateChanged;

  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                             Calque pour le puits d'événements                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class WebBrowserEx {

    //
    //  Classe emboîtée WebBrowserExSink
    //

    /// <summary>
    /// Classe emboîtée pour le puits d'événements DWebBrowserEvents2Reduced.
    /// </summary>
    internal class WebBrowserExSink : NativeMethods.DWebBrowserEvents2 {

      /// <summary>
      /// Lien sur le <see cref="WebBrowserEx"/>
      /// </summary>
      private WebBrowserEx browser;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="browser">lien sur le <see cref="WebBrowserEx"/></param>
      public WebBrowserExSink( WebBrowserEx browser ) { 
        this.browser = browser; 
      }

      /// <summary>
      /// Déclenché avant la navigation dans une nouvelle fenêtre.
      /// </summary>
      /// <remarks>
      /// Cet événement est déclenché dans toutes les versions, y compris Windows XP SP1 et antérieur.
      /// <br/>
      /// Il est débrayé depuis le 02 07 2011 au profit de <see cref="NewWindow3"/>.
      /// </remarks>
      /// <param name="pDisp">objet ActiveX associé au navigateur</param>
      /// <param name="cancel">basculer à true pour annuler la navigation</param>
      public void NewWindow2( ref object pDisp, ref bool cancel ) {
        //NewWindowExEventArgs args = new NewWindowExEventArgs( pDisp );
        //browser.OnNewWindowEx( args );
        //cancel = args.Cancel;
        //pDisp = args.AutomationObject;
      }

      /// <summary>
      /// Déclenché avant la navigation dans une nouvelle fenêtre.
      /// </summary>
      /// <remarks>
      /// Evénement déclenché à partir de Windows XP SP2 et ultérieur
      /// </remarks>
      /// <param name="pDisp">objet ActiveX associé au navigateur</param>
      /// <param name="cancel">basculer à true pour annuler la navigation</param>
      /// <param name="dwFlags">flags précisant l'opération liée à la fenêtre</param>
      /// <param name="bstrUrlContext">url de la page depuis laquelle l'ouverture est requise</param>
      /// <param name="bstrUrl">url cible de la navigation</param>
      public void NewWindow3( ref object pDisp, ref bool cancel, uint dwFlags, string bstrUrlContext, string bstrUrl ) {
        NewWindowExEventArgs args = new NewWindowExEventArgs( pDisp, bstrUrl, bstrUrlContext, (WebBrowserEx.NWMF) dwFlags );
        browser.OnNewWindowEx( args );
        cancel = args.Cancel;
        pDisp = args.AutomationObject;
      }

      public void BeforeNavigate2( object pDisp, ref object URL, ref object flags, ref object targetFrameName, ref object postData, ref object headers, ref bool cancel ) { }

      public void DownloadBegin() { }
      public void DownloadComplete() { }
      public void WindowClosing( bool isChildWindow, ref bool cancel ) { }
      public void OnQuit() { }
      public void StatusTextChange( string text ) { }
      public void ProgressChange( int progress, int progressMax ) { }
      public void TitleChange( string text ) { }
      public void PropertyChange( string szProperty ) { }
      public void NavigateComplete2( object pDisp, ref object URL ) { }
      public void DocumentComplete( object pDisp, ref object URL ) { }
      public void OnVisible( bool visible ) { }
      public void OnToolBar( bool toolBar ) { }
      public void OnMenuBar( bool menuBar ) { }
      public void OnStatusBar( bool statusBar ) { }
      public void OnFullScreen( bool fullScreen ) { }
      public void OnTheaterMode( bool theaterMode ) { }
      public void WindowSetResizable( bool resizable ) { }
      public void WindowSetLeft( int left ) { }
      public void WindowSetTop( int top ) { }
      public void WindowSetWidth( int width ) { }
      public void WindowSetHeight( int height ) { }
      public void SetSecureLockIcon( int secureLockIcon ) { }
      public void FileDownload( ref bool cancel ) { }
      public void NavigateError( object pDisp, ref object URL, ref object frame, ref object statusCode, ref bool cancel ) { }
      public void PrintTemplateInstantiation( object pDisp ) { }
      public void PrintTemplateTeardown( object pDisp ) { }
      public void UpdatePageStatus( object pDisp, ref object nPage, ref object fDone ) { }
      public void PrivacyImpactedStateChange( bool bImpacted ) { }
      public void CommandStateChange( int Command, bool Enable ) { }
      public void ClientToHostWindow( ref int CX, ref int CY ) { }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                         Calque pour l'accès à l'url de la favicon                           //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class WebBrowserEx {

    /// <summary>
    /// Détermine l'url de la favicon du site de la page actuelle.
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut être déterminée que si une page est chargée, 
    /// plus précisément si la propriété <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// Cette url est spécifiée comme la fichier "favicon.ico" à la racine du site<see cref="HtmlDocument.Domain"/>).
    /// <br/>
    /// Cette méthode ne vérifie pas si l'icône existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon, ou chaîne vide si l'url ne peut être déterminée</returns>
    protected string GetSiteFaviconUrl() {
      try {
        if ( Document == null ) return string.Empty;
        string domain = Document.Domain;
        if ( string.IsNullOrEmpty( domain ) ) return string.Empty;
        string result = domain + "/favicon.ico";
        if ( !result.ToLower().StartsWith( "http://" ) ) result = "http://" + result;
        return result;
      }
      catch { return string.Empty; }
    }

    /// <summary>
    /// Détermine l'url de la favicon associée à la page courante ou, à défaut, au site de la page courante.
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut être déterminée que si une page est chargée, 
    /// plus précisément si la propriété <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// La favicon d'une page est spécifiée via une balise "link" munie d'un attribut "rel" ayant la valeur
    /// "icon" ou "shortcut icon", l'icône étant alors spécifié via un attribut "href".
    /// <br/>
    /// La méthode prend en considération la première spécification de favicon trouvée et ignore d'éventuelles
    /// autres balises spécifiant une favicon. 
    /// <br/>
    /// Si la page ne contient pas de balise spécifiant une favicon, la méthode retourne la favicon
    /// du site de la page via <see cref="GetSiteFaviconUrl"/>.
    /// <br/>
    /// Cette méthode ne vérifie pas si l'icône existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon de la page, ou à défaut celle de la favicon du site, ou chaîne vide si aucune favicn ne peut être déterminée</returns>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected string GetPageFaviconUrl() {
      string result = string.Empty; ;

      // s'assurer qu'une page est chargée
      HtmlDocument html = Document;
      if ( html == null ) return result;

      // obtenir les éléments de partie "head"
      HtmlElementCollection heads = html.GetElementsByTagName( "head" );
      if ( heads.Count == 0 ) return result;

      // obtenir la collection des balises "link" de la partie "head"
      HtmlElement head = heads[ 0 ] ;
      HtmlElementCollection links = html.GetElementsByTagName( "link" );
      if ( links.Count == 0 ) return result;
      
      // rechercher la première balise "link" avec l'attribut "rel" qui soit "icon" ou "shorcut icon"
      foreach ( HtmlElement link in links ) {
        string rel = link.GetAttribute( "rel" ).ToLower();
        if ( rel != "icon" && rel != "shortcut icon" ) continue;
        string url = link.GetAttribute( "href" );
        if ( string.IsNullOrEmpty( url ) ) continue;
        result = url;
        break;
      }

      // aucune balise appropriée n'a été trouvée
      if ( string.IsNullOrEmpty( result ) ) return result;

      // élaborer l'url complète à partir de l'attribut href fourni
      Uri uri = new Uri( Document.Url, result );
      result = uri.AbsoluteUri ;
      return result;
    }
                                                                               // <wao GetFaviconUrl.begin>
    /// <summary>
    /// Détermine l'url de la favicon à associer à une page
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut être déterminée que si une page est chargée, 
    /// plus précisément si la propriété <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// L'url de la favicon associée à une page par cette méthode est en priorité l'url de la favicon
    /// propre de la page telle de calculée par <see cref="GetPageFaviconUrl"/>.
    /// <br/>
    /// Si cette méthode ne trouve aucune url de favicon, l'url de favicon retournée sera 
    /// l'url de la favicon du site telle que calculée par <see cref="GetSiteFaviconUrl"/>.
    /// <br/>
    /// La méthode ne vérifie pas si l'icône existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon, ou chaîne vide si aucune url de favicon ne peut être déterminée</returns>
    [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    protected string GetFaviconUrl() {
      string result = GetPageFaviconUrl();
      if ( string.IsNullOrEmpty( result ) ) result = GetSiteFaviconUrl();
      return result;
    }                                                                          // <wao GetFaviconUrl.end>
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                       Calque pour la validation des commandes d'onglets                     //
  //                                                                                             //
  // Documentation :                                                                             //
  // http://code.msdn.microsoft.com/VBTabbedWebBrowser-f0c0a525                                  //
  // http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx                                //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class WebBrowserEx {

    // clé de registre Windows pour enregistrer les applications prenant en charge la navigation multi-onglets
    private const string TabFeatureRegistryKey = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_TABBED_BROWSING";

    /// <summary>
    /// Obtient ou détermine si la navigation multi-onglets est validée pour l'application en cours. 
    /// </summary>
    public static bool IsTabEnabled {
      [PermissionSetAttribute( SecurityAction.LinkDemand, Name = "FullTrust" )]
      get {
        RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey( TabFeatureRegistryKey );
        if ( key == null ) return false;
        string processName = Process.GetCurrentProcess().ProcessName + ".exe";
        int keyValue = (int) key.GetValue( processName, 0 );
        return keyValue == 1;
      }
      [PermissionSetAttribute( SecurityAction.LinkDemand, Name = "FullTrust" )]
      set {
        RegistryKey key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey( TabFeatureRegistryKey );
        string processName = Process.GetCurrentProcess().ProcessName + ".exe";
        int keyValue = (int) key.GetValue( processName, 0 );
        bool isEnabled = keyValue == 1;
        if ( isEnabled != value ) key.SetValue( processName, value ? 1 : 0 );
      }
    }
  }

}
