/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 12 2000 : version initiale
 * 14 12 2001 : adaptation au contexte 2001-2002
 * 08 12 2002 : r�solution du probl�me des images
 * 27 11 2003 : migration en vjs
 * 23 04 2005 : portage en cs et refonte
 * 19 05 2005 : cr�ation s�par�e du composant Archiver dans Archiver_plugin
 * 19 05 2005 : acc�s � l'archiveur via IArchiverManager
 * 20 05 2005 : mise � niveau pour le protocole des actions
 * 02 06 2005 : correction merge order dans le sous menu "barres d'outils"
 * 04 06 2006 : correction du menu "� propos" pour conformit� � la sp�cification 2006
 * 30 01 2007 : portage et adaptation pour net 2.0
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 08 03 2008 : suppression des appels � doUpdateState inutiles
 * 08 03 2008 : mise � niveau pour la gestion de la propri�t� Checked des actions
 * 05 10 2009 : adjonction de l'option d'installation silencieuse
 * 07 10 2009 : adjonction de l'extension � la base de nom dans l'�v�nement ApplicationOpened
 * 13 06 2010 : adjonction du tag "-archive:" pour d�terminer le fichier d'archive par la ligne de commande
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace Psl.Applications {

  /// <summary>
  /// Cluster pour l'ihm d'un plugin d'archivage
  /// </summary>
  /// <remarks>
  /// <p>Cluster associ� � l'interface utilisateur pour l'archivage de l'�tat d'une application :
  /// commandes de menus, barres d'outils et dialogues d'acquisition des noms de fichiers.</p>
  ///  
  /// <p>Ce cluster sous-traite l'archivage proprement dit � un composant <see cref="Archiver"/>. 
  /// On peut contr�ler manuellement ce cluster via l'interface <see cref="IArchiverControl"/>.</p>
  ///   
  /// <p>Ce cluster permet de lire une archive, d'enregistrer (et d'enregistrer sous)
  /// l'�tat courant de l'application.</p>
  ///   
  /// <p>Par d�faut, le cluster fonctionne en mode automatique :<br/> 
  /// - lecture        de l'archive sur l'�v�nement <see cref="ApplicationState.ApplicationOpen"/><br/> 
  /// - enregistrement de l'archive sur l'�v�nement <see cref="ApplicationState.ApplicationClose"/></p>
  ///  
  /// <p>Notes :</p>
  /// <p>(1) Ce cluster suppose que la gestion des �v�nements de l'application
  /// est correctement mise en oeuvre.</p>
  ///  
  /// <p>(2) Ce cluster verrouille toutes les commandes tant que l'�v�nement <see cref="ApplicationState.ApplicationOpened"/>
  /// n'a pas �t� re�u et que la passerelle d'archivage n'est pas op�rationnelle.</p>
  /// </remarks>
  [ToolboxItem(false)]
  public partial class ArchiverCluster : UserControl {

    #region Champs propres pour l'impl�mentation

    /// <summary>
    /// Identification de la version du cluster de l'archiveur.
    /// </summary>
    private const string version = "13 juin 2010" ;

    /// <summary>
    /// Acc�s au service de messagerie
    /// </summary>
    protected IStatusService  status = null ;

    /// <summary>
    /// Composant d'archivage (interface de gestion)
    /// </summary>
    protected IArchiverManager archiver = null ;
  
    /// <summary>
    /// Nom du fichier d'archive en cours
    /// </summary>
    protected string  archiveName     = ""    ;
  
    /// <summary>
    /// Contr�le de l'installation
    /// </summary>
    private bool isInstalled = false ;

    /// <summary>
    /// Option d'installation silencieuse (sans menus ni barre d'outils) 
    /// </summary>
    private bool allowSilent = false;

    #endregion

    #region Initialisation / terminaison

    /// <summary>
    /// Constructeur.
    /// </summary>
    public ArchiverCluster() {
      InitializeComponent();
    }

    /// <summary>
    /// Constructeur avec option d'installation silencieuse
    /// </summary>
    /// <param name="allowSilent">si true, ne pas �mettre de diagnostic si les menus et barres d'outils ne sont pas enregistr�s</param>
    public ArchiverCluster( bool allowSilent ) {
      InitializeComponent();
      this.allowSilent = allowSilent;
    }

    /// <summary>
    /// Initialisation sur <see cref="ApplicationState.ApplicationOpen"/>.
    /// </summary>
    protected void DoSetupOpen() {
      try {
        archiver = Registry.MainArchiverManager ;
        status = (IStatusService) Registry.GetIf(MainKeys.KeyMainStatus);
        bool mergeInMenu = !allowSilent || Registry.Has( MainKeys.KeyMainMenu );
        bool mergeInTools = !allowSilent || Registry.Has( MainKeys.KeyMainTools );
        if (mergeInMenu) Registry.MergeInMainMenu( menu ) ;
        if (mergeInTools) Registry.MergeInMainTools( archiverTools ) ;
        isInstalled = true ;
      } 
      catch (Exception x) { 
        ExceptionBox.Show( x, "Erreur d'installation de l'archiveur", "Le cluster d'archivage n'a pas pu s'installer" ) ;        
      }
    }

    #endregion

    #region R�affichage du cluster

    /// <summary>
    /// Mise � jour de l'�tat de l'application.
    /// </summary>
    /// <param name="left">texte associ� au volet gauche</param>
    /// <param name="infos">texte associ� au volet d'infos</param>
    protected void DoReportStatus( string left, string infos ) {
      if ( status == null ) return;
      status.TextLeft = left ;
      status.TextInfos = infos;
    }

    /// <summary>
    /// Affichage centralis� correspondant � l'�tat courant du cluster
    /// </summary>
    protected void DoUpdateState() {
      acArchiverLoad   .Enabled = isInstalled                     ;
      acArchiverSave   .Enabled = isInstalled                     ;
      acArchiverSaveAs .Enabled = isInstalled                     ;
      archiverTools            .Visible = acArchiverToolsVisible.Checked  ;
    }

    #endregion

    #region Service 

    /// <summary>
    /// Contr�le de l'�tat d'installation du cluster
    /// </summary>
    /// <param name="doThrow">si true, d�clencher une exception en cas d'�chec du contr�le</param>
    /// <param name="opName"></param>
    /// <returns>true si l'installation est effectu�e</returns>
    private bool DoCheckState( bool doThrow, string opName ) {
      if (isInstalled) return true ;
        Exception exception = new EArchiver( "L'archiver n'est pas correctement initialis�, op�ration abandonn�e : " + opName );
        if ( doThrow )
          throw exception;
        else
          ExceptionBox.Show( exception );
      return false ;
    }

    /// <summary>
    /// Affichage d'un messages d'exception
    /// </summary>
    /// <param name="exception">exception dont le messages doit �tre affich�</param>
    private void DoReport( Exception exception ) {
      ExceptionBox.Show( exception ) ;
    }

    /// <summary>
    /// Obtient la base du nom du fichier d'archivage.
    /// </summary>
    /// <remarks>
    /// La base du nom du fichier d'archivage est obtenue comme suit :
    /// <br/>
    /// 1) si la ligne de commande comporte un argument "-archive:"
    /// et si la valeur de cet argument n'est pas vide,
    /// la base du nom est la valeur de ce param�tre;
    /// <br/>
    /// 2) sinon, la base du nom est le nom de l'ex�cutable o� l'extension
    /// ".exe" est remplac�e par ".archiver".
    /// </remarks>
    /// <returns>la base du nom du fichier d'archivage</returns>
    private string DoGetBaseFileName() {

      string  name = string.Empty;

      string[] options = Environment.GetCommandLineArgs();
      for ( int ix = 1 ; ix < options.Length ; ix++ ) {
        string option = options[ ix ];
        string lower = option.ToLower();
        if ( !lower.StartsWith( "-archive:" ) ) continue;
        name = option.Substring( 9, option.Length - 9 ).Trim( ' ', '"' );
        break;
      }

      if ( name == string.Empty ) name = ArchiverConst.DefaultName + "." + ArchiverConst.DefaultExtension;
      return name;
    }

 	  /// <summary>
    /// Cette m�thode �labore dans le champ archiveName le nom d'un
    /// fichier d'archive en perfectionnant la base fname fournie en argument et
    /// en contr�lant la validit� de ce nom compte tenu de l'objectif (lecture ou
    /// �criture) sp�cifi�e via le param�tre reading. 
    /// 
    /// Le cas �ch�ant, cette m�thode sollicitera l'utilisateur par un dialogue
    /// appropri� pour obtenir un nom de fichier acceptable, auquel cas l'utilisateur
    /// aura la facult� d'annuler la commande en cours.
	  /// </summary>
	  /// <param name="fname">base initiale d'�laboration du nom de fichier</param>
	  /// <param name="reading">true si en vue d'une ouvrture, false en vue d'un enregistrement</param>
    protected void MakeArchiveName( string fname, bool reading ) {
      bool    fileExists = false ;
      bool    pathExists = false ;
      bool    ask    ;
      string  wName  ;
      string  wPath  ;
  		
      // Dialogue � effectuer si fname est cha�ne vide		
      ask = fname == "" ;
  		
      // Base d�part pour l'�laboration d'un nom de fichier
      wName = fname == "" ? 
        archiveName == "" ? 
        ArchiverConst.DefaultName : 
      archiveName :
        fname ;
  		
      // Perfectionnement du nom de fichier : r�pertoire et extension
      if (! Path.HasExtension( wName )) wName = Path.ChangeExtension( wName, ArchiverConst.DefaultExtension ) ;
      wName = Path.GetFullPath     ( wName ) ;
      wPath = Path.GetDirectoryName( wName ) ;
      wName = Path.GetFileName     ( wName ) ;
  		
      // Contr�le de l'existence d'un fichier associ� au nom �labor�
      fileExists = File.Exists( Path.Combine( wPath, wName ) ) ;
      if (! fileExists) {
        String startPath = Application.StartupPath ;
        fileExists = File.Exists( Path.Combine( startPath, wName ) ) ;
        if (fileExists) wPath = startPath ;
      }
  		
      // Contr�le de l'existence du chemin associ� au nom �labor�
      pathExists = fileExists ? true : Directory.Exists( wPath ) ;
  		
      // Dialogue �ventuel d'acquisition ou de confirmation du nom du fichier
      if (reading) {
        if (ask || ! fileExists) {
          openDialog.FileName = wName ;
          if (pathExists) openDialog.InitialDirectory = wPath ;

          //openCreate = false ;
          if (openDialog.ShowDialog() == DialogResult.Cancel /*&& ! openCreate*/) 
            throw new ECancelled() ;
          archiveName = openDialog.FileName ;
        } else archiveName = Path.Combine( wPath, wName ) ;
      } else 
        if (ask || ! pathExists) {
        saveDialog.FileName = wName ;
        if (pathExists) saveDialog.InitialDirectory = wPath ;
        if (saveDialog.ShowDialog() == DialogResult.Cancel) throw new ECancelled() ;
        archiveName = saveDialog.FileName ;
      } else archiveName = Path.Combine( wPath, wName ) ;
    }
	
    /// <summary>
    /// Archivage �ventuel de l'�tat courant de la soute (service).
    /// </summary>
    protected void DoArchiveSaveIf() {
      DialogResult response = MessageBox.Show( "Voulez-vous archiver l'�tat actuel ?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question );
      if (response == DialogResult.Yes) DoArchive( archiveName, ArchiverConst.Writing ) ;
    }
	
    /// <summary>
    /// R�alisation de l'archivage
    /// </summary>
    /// <param name="fname">base du nom du fichier de l'archive</param>
    /// <param name="reading">true pour une lecture, false pour une �criture</param>
    protected void DoArchive( string fname, bool reading ) {
      string sens = reading ? "lecture" : "�criture" ;

      try {
			
        // Contr�le de l'initialisation
        if (!DoCheckState( false, sens )) {
          DoReportStatus( "archivage", sens + " annul�e : " + fname );
          return;
        }

        // Enregistrement �ventuel de la soute en cours
        // if (reading) DoArchiveSaveIf() ;
			
        // R�alisation bi-directionnelle de l'archivage
        try {
          MakeArchiveName( fname, reading ) ;
          DoReportStatus( "archivage", sens + " en cours... " + archiveName ) ;
          archiver.ArchiveFull( archiveName, ArchiverConst.Stamp, reading ) ;
          DoReportStatus( "archivage", sens + " termin�e : " + archiveName ) ;
        } 
				
          // Interception et relance pour perfectionnement des messages affich�s
        catch (ECancelled) { throw ; }
        catch (Exception e) { 
          DoReportStatus( "archivage", sens + " anomalie : " + archiveName ) ;
          throw new EArchiver(
            (reading ? "La lecture" : "L'�criture") + " de l'archive n'a pu �tre men�e � bien\r\n" +
            "fichier : " + archiveName,  
            e 
            ) ;
        }
      }
			
        // Interception et relance pour affichage de l'�tat courant 
      catch (ECancelled) {
        DoReportStatus( "archivage", sens + " annul�e : " + fname ) ;
        throw ;
      } 
    }
  
    #endregion


    #region Actions

    private void acArchiverAbout_Execute(object sender, EventArgs e) {
      Information.ShowDialog( "Plugin d'archivage", "standard", version );
    }

    private void acArchiverLoad_Execute(object sender, EventArgs e) {
      DoArchive( "", ArchiverConst.Reading ) ; 
    }

    private void acArchiverSave_Execute(object sender, EventArgs e) {
      DoArchive( archiveName, ArchiverConst.Writing ) ; 
    }

    private void acArchiverSaveAs_Execute(Object sender, EventArgs e) {
      DoArchive( "", ArchiverConst.Writing ) ;
    }

    #endregion

    #region Ev�nements de l'application

    private void applicationEvents_ApplicationOpen(object sender, System.EventArgs e) {
      try { DoSetupOpen() ; }
      catch (Exception x) { 
        ExceptionBox.Show( x ) ; 
      }
    }

    private void applicationEvents_ApplicationOpened(object sender, System.EventArgs e) {
      try { DoArchive( DoGetBaseFileName(), ArchiverConst.Reading ) ; } 
      catch (ECancelled) {}
      catch (Exception eX) { DoReport( eX ) ; }
    }

    private void applicationEvents_ApplicationClosing( object sender, FormClosingEventArgs e ) {
      if (e.Cancel) return ;
      try { 
        DoArchive( archiveName, ArchiverConst.Writing ) ; 
      } 
      catch (ECancelled) { e.Cancel = true ; }
      catch (Exception eX) { e.Cancel = true ; DoReport( eX ) ; }
    }

    private void applicationEvents_Archive(IArchiver sender) {
      sender.PushSection( "Psl.Applications.ArchiverCluster" ) ;
      try {
        sender.ArchiveProperty( "ShowTools", acArchiverToolsVisible, "Checked", false ) ;
      } finally { sender.PopSection() ; }
    }

    private void applicationEvents_ApplicationIdle(object sender, System.EventArgs e) {
      DoUpdateState() ;
    }

    #endregion

    #region Ev�nements locaux du cluster


    /// <summary>
    /// Contr�le de la validit� d'un fichier d'archive � lire.
    /// </summary>
    /// 
    /// <remarks>
    /// Ce gestionnaire correspond � l'�v�nement FileOk du composant de dialogue
    /// d'acquisition d'un nom de fichier en lecture <see cref="OpenFileDialog"/>. 
    /// <br/>                                                
    /// Si le fichier existe, le contr�le consiste � v�rifier le timbrage 
    /// d'authentification via la m�thode <see cref="IArchiverManager.ArchiveCheck"/> du 
    /// streamer ; si le timbrage est incorrect, la fermeture du dialogue est
    /// bloqu�e pour obliger l'utilisateur � choisir un autre fichier ou � annuler
    /// <br/>
    /// Si le fichier n'existe pas, un dialogue demande � l'utilisateur confirmation
    /// de la cr�ation d'une nouvelle archive (lecture des valeurs par d�faut). 
    /// </remarks>
    /// 
    /// <param name="source">object � consid�rer comme la source de l'�v�nement</param>
    /// <param name="e">descripteur d'�v�nement incluant un champ cancelled</param>
    private void openDialog_FileOk( object source, CancelEventArgs e ) {
      string fname = openDialog.FileName;
      e.Cancel = false;
      if ( !File.Exists( fname ) ) return ;

      try { archiver.ArchiveCheck( fname, ArchiverConst.Stamp ); }
      catch ( Exception eX ) { 
        e.Cancel = true ;
        DoReport( eX ); 
      }
    }

    #endregion

  }

  /// <summary>
  /// Constantes publi�es du plugin d'archivage
  /// </summary>
  public class ArchiverConst {

    /// <summary>
    /// Nom par d�faut pour un fichier d'archivage
    /// </summary>
    public static string DefaultName = Path.GetFileNameWithoutExtension( Application.ExecutablePath );

    /// <summary>
    /// Extension par d�faut pour un fichier d'archivage
    /// </summary>
    public static string DefaultExtension  = "archiver" ;

    /// <summary>
    /// Timbrage d'un fichier d'archive
    /// </summary>
    public static string  Stamp = "*** Fichier d'archive Psl - version du 25 04 2005 ***" ; 

    /// <summary>
    /// Sens d'archivage : lecture
    /// </summary>
    public const bool Reading  = true  ;
	
    /// <summary>
    /// Sens d'archivage : �criture
    /// </summary>
    public const bool Writing = false ;
	
  }
   
  /// <summary>
  /// Interface de contr�le du plugin d'archivage
  /// </summary>
  public interface IArchiverControl {

    /// <summary>
    /// Obtient ou d�termine si l'archivage doit �tre effectu� automatiquement
    /// </summary>
    /// Cette propri�t� correspond � une option de fonctionnement de l'archiveur :<br/>
    /// - si true  :<br/>
    ///   a) archivage automatique en lecture  sur <see cref="ApplicationState.ApplicationOpened"/> <br/>
    ///   b) archivage automatique en �criture sur <see cref="ApplicationState.ApplicationClosing"/> <br/>
    /// - si false : le d�clenchement des archivages s'effectue "manuellement" <br/>
    ///   via les m�thodes <see cref="Archive"/> et <see cref="OnClosingApplication"/> <br/>
    bool ArchiveAuto{ get ; set ; }

	  /// <summary>
	  /// R�alisation d'un archivage, toutes exceptions trait�es.
	  /// </summary>
	  /// <param name="fname">Nom complet du fichier d'archivage</param>
	  /// <param name="reading">True pour une lecture, false pour une �criture</param>
    void Archive( string fname, bool reading ) ;
  
    /// <summary>
    /// R�alisation d'un archivage, sans traitement des exceptions.
    /// </summary>
    /// Cette m�thode laisse ressortir les exceptions qui pourraient survenir
    /// pendant l'archivage.
    ///
    ///<param name="fname">nom du fichier d'archivage ou cha�ne vide (nom par d�faut)</param> 
    ///<param name="reading">si true, archivage en lecture, sinon archivage en �criture</param>
    void ArchiveAsCommand( string fname, bool reading ) ;
  
    /// <summary>
    /// R�aliser un archivage, toutes exceptions trait�es.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est � appeler lors du protocole de fermeture de l'application. 
    /// Elle inclut la gestion d'�v�nement du type <see cref="CancelEventArgs"/> conduisant, le cas
    /// �ch�ant, � interrompre le protocole de fermeture. 
    /// Implicitement, l'archivage s'effectue en �criture. 
    /// </remarks>
    /// <param name="source">objet � consid�rer comme la source de la demande d'archivage</param>
    /// <param name="e">descripteur d'�v�nement</param>
    void OnClosingApplication( object source, CancelEventArgs e  ) ;
  }

}
