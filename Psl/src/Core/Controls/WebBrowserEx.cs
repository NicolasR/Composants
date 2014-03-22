/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * Cr�dits : 
 * - Jeroen Landheer pour Code Project
 * - Microsoft Corp.
 * 
 * Version compl�te assurant la compatibilit� de toutes les versions de Windows
 * 
 * 10 12 2008 : version initiale
 * 12 01 2010 : protection de la propri�t� ReadyState contre les exception intempestives
 * 11 05 2010 : am�lioration de la valeur de la propri�t� ActualTitle
 * 27 05 2010 : protection de la propri�t� StatusText contre les exception intempestives
 * 31 05 2010 : protection de la propri�t� Document contre les exception UnautorizedAccessException (vs 2010)
 * 01 06 2010 : modification de ActualUrl pour corriger des probl�mes de codage UTF-8/UTF-16 en VS 2010
 * 07 02 2011 : adjonction de StateChanged, ActualProgressState, ActualReadyState
 * 17 02 2011 : adjonction des fonctionnalit�s de d�clenchement des dialogues usuels IE
 * 17 02 2011 : adjonction des fonctionnalit�s li�es au zoom optique
 * 19 02 2011 : adjonction des fonctionnalit�s de recherche de cha�nes dans une page
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
    /// Flags transmis via l'�v�nement NewWindoW3 pour le descripteur <see cref="NewWindowExEventArgs"/>
    /// </summary>
    /// <remarks>
    /// Dans le cas du composant de navigation, seul le flag <see cref="NWMF_FORCETAB"/> est en fait � tester
    /// pour savoir s'il faut ouvrir un nouvel onglet ou non.
    /// <br/>
    /// Toutefois, ce flag n'est positionn� que via la commande de menu contextuel "ouvrir dans un nouvel onglet",
    /// la quelle commande n'est valid�e qu'� condition que l'application h�te soit enregistr�e dans le 
    /// registre de Windows comme prenant en charge la navigation multi-onglets : basculer la propri�t� de classe
    /// <see cref="IsTabEnabled"/> � true.
    /// </remarks>
    public enum NWMF {

      /// <summary>
      /// Aucun flag arm�
      /// </summary>
      NWMF_NONE = 0x00000000,

      /// <summary>
      /// La page est en cours de d�chargement
      /// </summary>
      NWMF_UNLOADING = 0x00000001,

      /// <summary>
      /// L'ouverture r�sulte d'une action de l'utilisateur
      /// </summary>
      NWMF_USERINITED = 0x00000002,

      /// <summary>
      /// Indique si l'ouverture est la premi�re (dans le cas o� le flag NWMF_USERINITED est arm�)
      /// </summary>
      NWMF_FIRST = 0x00000004,

      /// <summary>
      /// La touche "override" (ALT) �tait enfonc�e
      /// </summary>
      NWMF_OVERRIDEKEY = 0x00000008,

      /// <summary>
      /// La nouvelle fen�tre � charger provient d'une requ�te d'affichage de l'aide
      /// </summary>
      NWMF_SHOWHELP = 0x00000010,

      /// <summary>
      /// La fen�tre est une bo�te de dialogue qui affiche un contenu html
      /// </summary>
      NWMF_HTMLDIALOG = 0x00000020,

      /// <summary>
      /// La fen�tre � ouvrir est appel�e depuis une bo�te de dialogue � contenu html
      /// </summary>
      NWMF_FROMDIALOGCHILD = 0x00000040,

      /// <summary>
      /// La fen�tre est r�ellement requise par l'utilisateur 
      /// </summary>
      NWMF_USERREQUESTED = 0x00000080,

      /// <summary>
      /// R�sult d'une commande de rafra�chissement
      /// </summary>
      NWMF_USERALLOWED = 0x00000100,

      /// <summary>
      /// La nouvelle fen�tre devrait �tre forc�e dans une nouvelle fen�tre plut�t que dans un nouvel onglet
      /// </summary>
      NWMF_FORCEWINDOW = 0x00010000,

      /// <summary>
      /// La nouvelle fen�tre devrait �tre ouverte dans un nouvel onglet
      /// </summary>
      NWMF_FORCETAB = 0x00020000,

      /// <summary>
      /// La fen�tre devrait �tre ouverte comme une fen�tre, sauf si NWMF_FORCETAB est arm�
      /// </summary>
      NWMF_SUGGESTWINDOW = 0x00040000,

      /// <summary>
      /// La fen�tre devrait �tre ouverte comme un onglet, sauf si NWMF_FORCEWINDOW est arm�
      /// </summary>
      NWMF_SUGGESTTAB = 0x00080000,

      /// <summary>
      /// L'�v�nement est d�clench� depuis un onglet inactif
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
  /// Descripteur pour l'�v�nement <see cref="WebBrowserEx.NewWindowEx"/>.
  /// </summary>
  public class NewWindowExEventArgs : CancelEventArgs {

                                                                             // <wao never.begin>
    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="automationObject">objet automation prenant en charge la navigation</param>
    /// <param name="url">url cible de la navigation</param>
    /// <param name="urlContext">url du contexte depuis lequel la navigation est d�clench�e</param>
    /// <param name="flags">flags pr�cisant la source et le contexte de l'op�ration</param>
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
    /// Url du contexte d'o� provient la demande de navigation
    /// </summary>
    public string UrlContext { get; private set; }

    /// <summary>
    /// Flags sp�cifiant les circonstances de l'ouverture de la fen�tre
    /// </summary>
    public WebBrowserEx.NWMF Flags { get; private set; }
                                                                            // <wao never.end>

    /// <summary>
    /// Obtient ou d�termine l'objet automation prenant en charge la navigation
    /// </summary>
    /// <remarks>
    /// Pour que la navigation s'effectue dans une nouvelle fen�tre externe, ne pas affecter cette propri�t�.
    /// Pour provoquer une navigation au sein de l'application (par exemple pour une navigation multi-onglets),
    /// instancier un nouvel objet <see cref="WebBrowserEx"/>, puis affecter la valeur de la propri�t� 
    /// <see cref="WebBrowserEx.AutomationObject"/> � AutomationObject.
    /// </remarks>
    public object AutomationObject { get; set ;}
  }                                                                            // <wao NewWindowExEventArgs.end>

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             D�l�gu� NewWindowExEventHandler                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Type de d�l�gu� pour l'�v�nement <see cref="WebBrowserEx.NewWindowEx"/>.
  /// </summary>
  /// <param name="sender">source de l'�v�nement</param>
  /// <param name="e">descripteur de l'�v�nement</param>
  public delegate void NewWindowExEventHandler( object sender, NewWindowExEventArgs e ) ;

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Extension du contr�le <see cref="WebBrowser"/> autorisant une navigation multi-onglets.
  /// </summary>
  [
  PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust"), 
  PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust"),
  Description( "Extension du contr�le autorisant une navigation multi-onglets" )
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
    // Initialisation, finalisation et connexions � l'automation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public WebBrowserEx() {
      ScriptErrorsSuppressed = true;
    }

    /// <summary>
    /// Appel�e par l'infrastructure lorsque le contr�le ActiveX sous-jacent est cr��. 
    /// </summary>
    /// <param name="nativeActiveXObject">objet ActiveX natif assurant la navigation</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void AttachInterfaces( object nativeActiveXObject ) {
      this.axIWebBrowser2 = (NativeMethods.IWebBrowser2) nativeActiveXObject;
      base.AttachInterfaces( nativeActiveXObject );
    }

    /// <summary>
    /// Appel�e par l'infrastructure lorsque le contr�le ActiveX sous-jacent est d�truit. 
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void DetachInterfaces() {
      this.axIWebBrowser2 = null;
      base.DetachInterfaces();
    }

    /// <summary>
    /// Appel�e par l'infrastructure pour permettre la connexion du puits d'�v�nements
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void CreateSink() {
      base.CreateSink();
      sink = new WebBrowserExSink( this );
      cookie = new AxHost.ConnectionPointCookie( ActiveXInstance, sink, typeof( NativeMethods.DWebBrowserEvents2 ) );
    }

    /// <summary>
    /// Appel�e par l'infrastructure pour permettre la d�connexion du puits d'�v�nements
    /// </summary>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void DetachSink() {
      if ( cookie != null ) cookie.Disconnect();
      cookie = null;
      sink = null;
      base.DetachSink();
    }

    //
    // Contr�les
    //

    private void DoCheckDocument( string operation ) {
      if ( Document == null ) throw new InvalidOperationException( operation + " requiert qu'une page html soit charg�e dans le navigateur" );
    }

    private NativeMethods.IOleCommandTarget DoCheckOleCommandTarget( string operation ) {
      NativeMethods.IOleCommandTarget result = axIWebBrowser2.Document as NativeMethods.IOleCommandTarget;
      if ( result == null ) throw new InvalidOperationException( operation + " requiert qu'une page html soit charg�e dans le navigateur" );
      return result;
    }

    private void DoCheckString( string value, string operation, string paramName = "text" ) {
      if ( string.IsNullOrEmpty( value ) ) throw new ArgumentException( operation + " : la cha�ne de caract�res ne peut �tre ni null ni vide", paramName ); 
    }

    //
    // Service
    //

    // Les m�thodes de recherche sont programm�es en style dynamic pour �viter la r�f�rence � 
    // la librairie Microsoft.mshtml.dll dans la librairie de base Psl.Core
    // La documentation des interfaces, objets et m�thodes est celle de IHTMLDocument2

    // m�thode centralis�e pour atteindre une occurrence, et �ventuellement la s�lectionner ou la colorier
    private bool DoFindText( string text, ref dynamic anchor, bool next, bool forward, bool select, Color color, bool wholeWord, bool matchCase ) {

      // d�terminer l'ancre de base pour la recherche : corps de la page ou s�lection en cours
      if ( anchor == null ) {
        dynamic document = Document.DomDocument;         // IHTMLDocument2 document = Document.DomDocument as IHTMLDocument2;
        if ( next )
          anchor = document.selection.createRange();     // IHTMLTxtRange anchor = document.selection.createRange() as IHTMLTxtRange;
        else {
          anchor = document.body.createTextRange();      // IHTMLTxtRange anchor = document.body.createTextRange() as IHTMLTxtRange;
          anchor.collapse( forward );
        }
      }

      // pr�parer l'ancre pour le point de d�part de la recherche
      anchor.collapse( !forward );

      // d�terminer l'�tendue de la recherche : positive vers la fin, n�gative vers le d�but
      int scope = forward ? int.MaxValue : int.MinValue;

      // pr�parer les flags d'options de la recherche
      int flags = 0;
      if ( wholeWord ) flags |= 0x00000002;
      if ( matchCase ) flags |= 0x00000004;

      // effectuer la recherche
      bool found = anchor.findText( text, scope, flags );
      if ( !found ) return false;

      // s�lectionner l'occurrence ou colorier son arri�re-plan
      if ( select ) 
        anchor.select();
      else 
        anchor.execCommand( "BackColor", false, color );

      // occurrence trouv�e
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

      // nombre d'occurrences trouv�es
      return count;
    }

    //
    // D�clenchement centralis� des �v�nements
    //
                                                                               
    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.DocumentCompleted"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]           // <wao never>
    protected override void OnDocumentCompleted( WebBrowserDocumentCompletedEventArgs e ) {  // <wao OnDocumentCompleted.begin>
      try {
        base.OnDocumentCompleted( e );
        OnStateChanged( EventArgs.Empty );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }                                                                          // <wao OnDocumentCompleted.end>

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.Navigating"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnNavigating( WebBrowserNavigatingEventArgs e ) {
      try {
        base.OnNavigating( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.Navigated"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnNavigated( WebBrowserNavigatedEventArgs e ) {
      try {
        base.OnNavigated( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.ProgressChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
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
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.StatusTextChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected override void OnStatusTextChanged( EventArgs e ) {
      try {
        base.OnStatusTextChanged( e );
        OnStateChanged( EventArgs.Empty );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.DocumentTitleChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnDocumentTitleChanged( EventArgs e ) {
      try {
        base.OnDocumentTitleChanged( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.EncryptionLevelChanged"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnEncryptionLevelChanged( EventArgs e ) {
      try {
        base.OnEncryptionLevelChanged( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement de l'�v�nement <see cref="WebBrowser.FileDownload"/>
    /// </summary>
    /// <remarks>
    /// Les exceptions sont intercept�es et relay�es via <see cref="System.Windows.Forms.Application.ThreadException"/>
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnFileDownload( EventArgs e ) {
      try {
        base.OnFileDownload( e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement <see cref="NewWindowEx"/>.
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected virtual void OnNewWindowEx( NewWindowExEventArgs e ) {
      try {
        if ( NewWindowEx != null ) NewWindowEx( this, e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement <see cref="StateChanged"/>.
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected virtual void OnStateChanged( EventArgs e ) {
      try {
        if ( StateChanged != null ) StateChanged( this, e );
      }
      catch ( Exception x ) { Application.OnThreadException( x ); }
    }

    //
    // Fonctionnalit�s red�clar�es ou red�finies
    //

    /// <summary>
    /// Obtient la r�f�rence sur le document charg� dans le contr�le <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette red�claration est simplement destin�e � absorber d'�ventuelles exceptions qui
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
    /// Obtient une valeur indiquant l'�tat actuel du contr�le <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette red�claration est simplement destin�e � absorber d'�ventuelles exceptions qui
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
    /// Obtient le status text du contr�le <see cref="WebBrowser"/>
    /// </summary>
    /// <remarks>
    /// Cette red�claration est simplement destin�e � absorber d'�ventuelles exceptions qui
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
    /// Obtient ou d�termine si le navigateur supprime les bo�tes de dialogue concernant les erreurs de scripts.
    /// </summary>
    [
     Category( "Behavior" ), 
     DefaultValue( true ),
     Description( "Obtient ou d�termine si le navigateur supprime bo�tes de dialogue concernant les erreurs de scripts" ),
    ]
    public new bool ScriptErrorsSuppressed {
      get { return base.ScriptErrorsSuppressed ; }
      set { base.ScriptErrorsSuppressed = value ; }
    } 

    //
    // Fonctionnalit�s ajout�es � WebBrowser
    //

    /// <summary>
    /// Affiche le dialogue des options Internet Explorer
    /// </summary>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement charg�e dans le navigateur</exception>
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
    /// Affiche la source de la page actuellement charg�e dans NotePad
    /// </summary>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement charg�e dans le navigateur</exception>
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
    /// Affiche de dialogue de recherche d'une cha�ne dans une page
    /// </summary>
    /// <remarks>
    /// Ce dialogue est li� � l'instance de WebBrowser pour le compte de laquelle le dialogue a �t� ouvert.
    /// Il n'est pas adapt� au contexte multi-onglets car le dialogue ne s'applique pas � la page actuellement visible.
    /// </remarks>
    public void ShowFindDialog() {
      object nullObjectArray = null;
      axIWebBrowser2.ExecWB( NativeMethods.OLECMDID.OLECMDID_FIND, NativeMethods.OLECMDEXECOPT.OLECMDEXECOPT_PROMPTUSER, ref nullObjectArray, IntPtr.Zero );
    }
    */

    /// <summary>
    /// Recherche et s�lectionne une cha�ne dans le texte de la page courante.
    /// </summary>
    /// <param name="text">cha�ne � rechercher</param>
    /// <param name="next">true pour forcer la recherche de l'occurrence suivante</param>
    /// <param name="forward">true pour rechercher l'occurrence suivante, false pour rechercher l'occurrence pr�c�dente</param>
    /// <param name="wholeWord">true pour une recherche en mots entiers seulement</param>
    /// <param name="matchCase">true pour recherche sensible � la casse</param>
    /// <returns>true si une occurrence de la cha�ne a �t� trouv�e</returns>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement charg�e dans le navigateur</exception>
    /// <exception cref="ArgumentException">la cha�ne � rechercher <paramref name="text"/> est null ou vide</exception>
    public bool FindText( string text, bool next = true, bool forward = true, bool wholeWord = false, bool matchCase = false ) {
      DoCheckDocument( "FindText" );
      DoCheckString( text, "FindText" );
      return DoFindText( text, next, forward, true, Color.White, wholeWord, matchCase );
    }

    /// <summary>
    /// Recherche et colorie l'arri�re-plan de toutes les occurrences d'une cha�ne dans la page courante. 
    /// </summary>
    /// <param name="text">cha�ne � rechercher</param>
    /// <param name="color">couleur d'arri�re-plan pour signaler les occurrences trouv�es</param>
    /// <param name="wholeWord">true pour une recherche en mots entiers seulement</param>
    /// <param name="matchCase">true pour recherche sensible � la casse</param>
    /// <returns>le nombre d'occurrences trouv�es</returns>
    /// <exception cref="InvalidOperationException">si une page html n'est pas actuellement charg�e dans le navigateur</exception>
    /// <exception cref="ArgumentException">la cha�ne � rechercher <paramref name="text"/> est null ou vide</exception>
    public int FindText( string text, Color color, bool wholeWord = false, bool matchCase = false ) {
      DoCheckDocument( "FindText" );
      DoCheckString( text, "FindText" );
      return DoFindText( text, color, wholeWord, matchCase );
    }

    //
    // Propri�t�s ajout�es � WebBrowser
    //

    /// <summary>
    /// Obtient l'objet automation associ� au navigateur
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
    /// En vs 2010, <see cref="Uri.AbsoluteUri"/> est cod� en UTF-8 et pose probl�me avec
    /// les url comportant des caract�res accentu�s. 
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
    /// Obtient l'url de la favicon de la page courante, ou cha�ne vide si aucune favicon n'est d�termin�e
    /// </summary>
    [Browsable( false )]
    public string ActualFaviconUrl {
      get { return GetFaviconUrl(); }
    }

    /// <summary>
    /// Obtient le "StatusText" le plus pertinent pour l'�tat courant du navigateur
    /// </summary>
    [Browsable( false )]
    public string ActualStatusText {
      get {
        string result = StatusText;
        if ( string.IsNullOrEmpty( result ) || result == "Termin�" ) result = ActualUrl;
        return result;
      }
    }

    /// <summary>
    /// Obtient la cha�ne d'�tat de disponibilit� du navigateur
    /// </summary>
    [Browsable( false )]
    public string ActualReadyState {
      get {
        switch ( ReadyState ) {
          case WebBrowserReadyState.Uninitialized: return "Non initialis�";
          case WebBrowserReadyState.Loading: return "Chargement";
          case WebBrowserReadyState.Loaded: return "Charg�";
          case WebBrowserReadyState.Interactive: return "Interactif";
          case WebBrowserReadyState.Complete: return "Termin�";
          default: return ReadyState.ToString();
        }
      }
    }

    /// <summary>
    /// Obtient l'�tat courant de la jauge de progression
    /// </summary>
    [Browsable( false )]
    public ProgressData ActualProgressState {
      get { return lastProgressState; }
    }

    /// <summary>
    /// Obtient l'intervalle possible pour le zoom optique <see cref="OpticalZoom"/>
    /// </summary>
    /// <exception cref="InvalidOperationException">si aucune page n'est charg�e ou si l'op�ration n'est pas possible dans l'�tat courant du navigateur</exception>
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
          throw new InvalidOperationException( "L'acc�s � la plage de variation du zoom optique n'est pas possible", ex );
        }

        Point split = Win.Util.LParamToPoint( (int) result );
        return new Range( split.X, split.Y );
      }
    }

    /// <summary>
    /// Obtient ou d�termine la valeur du zoom optique
    /// </summary>
    /// <remarks>
    /// Comme il n'y a pas de moyen (que je connaisse) pour conna�tre le facteur de zoom optique courant,
    /// la propri�t� OpticalZoom tient � jour un champ local pour m�moriser le dernier facteur de zoom optique connu. 
    /// </remarks>
    /// <exception cref="InvalidOperationException">si aucune page n'est charg�e ou si l'op�ration n'est pas possible dans l'�tat courant du navigateur</exception>
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
    // Ev�nements ajout�s � WebBrowser
    //

    /// <summary>
    /// D�clench� lorsque la navigation va provoquer l'ouverture d'une nouvelle fen�tre.
    /// </summary>
    /// <seealso cref="NewWindowExEventArgs"/>
    [Description( "D�clench� lorsque la navigation va provoquer l'ouverture d'une nouvelle fen�tre" )]
    public event NewWindowExEventHandler NewWindowEx;

    /// <summary>
    /// D�clench� lorsqu'une information concernant l'�tat courant du navigateur a chang�
    /// </summary>
    [Description( "D�clench� lorsqu'une information concernant l'�tat courant du navigateur a chang�" )]
    public event EventHandler StateChanged;

  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Extension de la classe WebBrowser                               //
  //                             Calque pour le puits d'�v�nements                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class WebBrowserEx {

    //
    //  Classe embo�t�e WebBrowserExSink
    //

    /// <summary>
    /// Classe embo�t�e pour le puits d'�v�nements DWebBrowserEvents2Reduced.
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
      /// D�clench� avant la navigation dans une nouvelle fen�tre.
      /// </summary>
      /// <remarks>
      /// Cet �v�nement est d�clench� dans toutes les versions, y compris Windows XP SP1 et ant�rieur.
      /// <br/>
      /// Il est d�bray� depuis le 02 07 2011 au profit de <see cref="NewWindow3"/>.
      /// </remarks>
      /// <param name="pDisp">objet ActiveX associ� au navigateur</param>
      /// <param name="cancel">basculer � true pour annuler la navigation</param>
      public void NewWindow2( ref object pDisp, ref bool cancel ) {
        //NewWindowExEventArgs args = new NewWindowExEventArgs( pDisp );
        //browser.OnNewWindowEx( args );
        //cancel = args.Cancel;
        //pDisp = args.AutomationObject;
      }

      /// <summary>
      /// D�clench� avant la navigation dans une nouvelle fen�tre.
      /// </summary>
      /// <remarks>
      /// Ev�nement d�clench� � partir de Windows XP SP2 et ult�rieur
      /// </remarks>
      /// <param name="pDisp">objet ActiveX associ� au navigateur</param>
      /// <param name="cancel">basculer � true pour annuler la navigation</param>
      /// <param name="dwFlags">flags pr�cisant l'op�ration li�e � la fen�tre</param>
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
  //                         Calque pour l'acc�s � l'url de la favicon                           //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class WebBrowserEx {

    /// <summary>
    /// D�termine l'url de la favicon du site de la page actuelle.
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut �tre d�termin�e que si une page est charg�e, 
    /// plus pr�cis�ment si la propri�t� <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// Cette url est sp�cifi�e comme la fichier "favicon.ico" � la racine du site<see cref="HtmlDocument.Domain"/>).
    /// <br/>
    /// Cette m�thode ne v�rifie pas si l'ic�ne existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon, ou cha�ne vide si l'url ne peut �tre d�termin�e</returns>
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
    /// D�termine l'url de la favicon associ�e � la page courante ou, � d�faut, au site de la page courante.
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut �tre d�termin�e que si une page est charg�e, 
    /// plus pr�cis�ment si la propri�t� <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// La favicon d'une page est sp�cifi�e via une balise "link" munie d'un attribut "rel" ayant la valeur
    /// "icon" ou "shortcut icon", l'ic�ne �tant alors sp�cifi� via un attribut "href".
    /// <br/>
    /// La m�thode prend en consid�ration la premi�re sp�cification de favicon trouv�e et ignore d'�ventuelles
    /// autres balises sp�cifiant une favicon. 
    /// <br/>
    /// Si la page ne contient pas de balise sp�cifiant une favicon, la m�thode retourne la favicon
    /// du site de la page via <see cref="GetSiteFaviconUrl"/>.
    /// <br/>
    /// Cette m�thode ne v�rifie pas si l'ic�ne existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon de la page, ou � d�faut celle de la favicon du site, ou cha�ne vide si aucune favicn ne peut �tre d�termin�e</returns>
    [PermissionSet( SecurityAction.LinkDemand, Name = "FullTrust" )]
    protected string GetPageFaviconUrl() {
      string result = string.Empty; ;

      // s'assurer qu'une page est charg�e
      HtmlDocument html = Document;
      if ( html == null ) return result;

      // obtenir les �l�ments de partie "head"
      HtmlElementCollection heads = html.GetElementsByTagName( "head" );
      if ( heads.Count == 0 ) return result;

      // obtenir la collection des balises "link" de la partie "head"
      HtmlElement head = heads[ 0 ] ;
      HtmlElementCollection links = html.GetElementsByTagName( "link" );
      if ( links.Count == 0 ) return result;
      
      // rechercher la premi�re balise "link" avec l'attribut "rel" qui soit "icon" ou "shorcut icon"
      foreach ( HtmlElement link in links ) {
        string rel = link.GetAttribute( "rel" ).ToLower();
        if ( rel != "icon" && rel != "shortcut icon" ) continue;
        string url = link.GetAttribute( "href" );
        if ( string.IsNullOrEmpty( url ) ) continue;
        result = url;
        break;
      }

      // aucune balise appropri�e n'a �t� trouv�e
      if ( string.IsNullOrEmpty( result ) ) return result;

      // �laborer l'url compl�te � partir de l'attribut href fourni
      Uri uri = new Uri( Document.Url, result );
      result = uri.AbsoluteUri ;
      return result;
    }
                                                                               // <wao GetFaviconUrl.begin>
    /// <summary>
    /// D�termine l'url de la favicon � associer � une page
    /// </summary>
    /// <remarks>
    /// L'url de la favicon ne peut �tre d�termin�e que si une page est charg�e, 
    /// plus pr�cis�ment si la propri�t� <see cref="WebBrowser.Document"/> n'est pas <c>null</c>.
    /// <br/>
    /// L'url de la favicon associ�e � une page par cette m�thode est en priorit� l'url de la favicon
    /// propre de la page telle de calcul�e par <see cref="GetPageFaviconUrl"/>.
    /// <br/>
    /// Si cette m�thode ne trouve aucune url de favicon, l'url de favicon retourn�e sera 
    /// l'url de la favicon du site telle que calcul�e par <see cref="GetSiteFaviconUrl"/>.
    /// <br/>
    /// La m�thode ne v�rifie pas si l'ic�ne existe sur le site : elle calcule seulement son url.
    /// </remarks>
    /// <returns>l'url de la favicon, ou cha�ne vide si aucune url de favicon ne peut �tre d�termin�e</returns>
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

    // cl� de registre Windows pour enregistrer les applications prenant en charge la navigation multi-onglets
    private const string TabFeatureRegistryKey = "Software\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_TABBED_BROWSING";

    /// <summary>
    /// Obtient ou d�termine si la navigation multi-onglets est valid�e pour l'application en cours. 
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
