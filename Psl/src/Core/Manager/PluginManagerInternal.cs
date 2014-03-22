/*
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Gestionnaire des plugins dynamiques
 * 
 * 31 01 2008 : version initiale pour aihm 2007-2008
 * 22 04 2009 : amélioration du contrôle des librairies
 * 30 01 2010 : adjonction du chargement des plugins spéciaux
 * 01 06 2010 : adjonction du hook VerboseOff pour éviter l'affichage modal de la fenêtre en mode verbose
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Psl.Applications.Manager {

  #region Classes de service

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                         EPluginManager                                      //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Enumération associée au niveau de sévérité des diagnostics.
  /// </summary>
  [Flags]
  public enum Severity {

    /// <summary>
    /// Aucun flag de sévérité armé
    /// </summary>
    None = 0x00000000,

    /// <summary>
    /// Niveau de sévérité : pour information
    /// </summary>
    Info = 0x00000001,

    /// <summary>
    /// Niveau de sévérité : avertissement
    /// </summary>
    Warning = 0x00000002,

    /// <summary>
    /// Niveau de sévérité : erreur
    /// </summary>
    Error = 0x00000004,

    /// <summary>
    /// Tous les flags de sévérité armés
    /// </summary>
    All = 0x0000000F
  }

  /// <summary>
  /// Classe pour les exceptions propres du gestionnaire des plugins.
  /// </summary>
  public class EPluginManager : Exception {

    private Severity severity  ;
    private string   phase     ;
    private string   installer ;

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="severity">niveau de sévérité de l'erreur</param>
    /// <param name="phase">cause de l'exception</param>
    /// <param name="installer">nom de la classe germe d'installation</param>
    /// <param name="message">mesage associé à l'exception</param>
    /// <param name="arg0">premier argument</param>
    /// <param name="arg1">second argument</param>
    public EPluginManager( Severity severity, string phase, string installer, string message, object arg0, object arg1 )
      : this( severity, phase, installer, string.Format( message, arg0, arg1 ), (Exception) null ) { }

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="severity">niveau de sévérité de l'erreur</param>
    /// <param name="phase">cause de l'exception</param>
    /// <param name="installer">nom de la classe germe d'installation</param>
    /// <param name="message">mesage associé à l'exception</param>
    /// <param name="arg0">premier argument</param>
    public EPluginManager( Severity severity, string phase, string installer, string message, object arg0 )
      : this( severity, phase, installer, string.Format( message, arg0 ), (Exception) null ) { }

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="severity">niveau de sévérité de l'erreur</param>
    /// <param name="phase">cause de l'exception</param>
    /// <param name="installer">nom de la classe germe d'installation</param>
    /// <param name="message">mesage associé à l'exception</param>
    public EPluginManager( Severity severity, string phase, string installer, string message )
      : this( severity, phase, installer, message, (Exception) null ) { }

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="severity">niveau de sévérité de l'erreur</param>
    /// <param name="phase">cause de l'exception</param>
    /// <param name="installer">nom de la classe germe d'installation</param>
    /// <param name="message">mesage associé à l'exception</param>
    /// <param name="inner">objet exception intercepté</param>
    public EPluginManager( Severity severity, string phase, string installer, string message, Exception inner )
      : base( message, inner ) {
      this.severity = severity;
      this.phase = phase;
      this.installer = installer;
    }

    /// <summary>
    /// Obtient le niveau de sévérité de l'exception
    /// </summary>
    public Severity Severity { get { return severity; } }

    /// <summary>
    /// Obtient la cause de l'exception
    /// </summary>
    public string Phase { get { return phase; } }

    /// <summary>
    /// Obtient le nom de la classe germe d'installation concernée
    /// </summary>
    public string Installer { get { return installer; } }
  }

  /// <summary>
  /// Classe des exceptions à supprimer lors de chaque élaboration du rapport
  /// </summary>
  public class EPluginManagerClearable : EPluginManager {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="severity">niveau de sévérité de l'erreur</param>
    /// <param name="phase">cause de l'exception</param>
    /// <param name="installer">nom de la classe germe d'installation</param>
    /// <param name="message">mesage associé à l'exception</param>
    /// <param name="inner">objet exception intercepté</param>
    public EPluginManagerClearable( Severity severity, string phase, string installer, string message, Exception inner )
      : base( severity, phase, installer, message, inner ) {
    }
  }


  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                         FileData                                            //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Enumération associée au classement des librairies dll.
  /// </summary>
  internal enum LoadStates {
    isNotLoaded,
    isPreLoaded,
    isPostLoaded,
    isExeLoaded,
    isMgrLoaded,
  }

  /// <summary>
  /// Recueil des informations associées à une librairie dll.
  /// </summary>
  internal class FileData {

    private string fullName = string.Empty;
    private string fileName = string.Empty;

    private LoadStates loadState = LoadStates.isNotLoaded;
    private bool isFiltered = false;

    private List<string> installers = null;

    private List<EPluginManager> messages = new List<EPluginManager>();

    /// <summary>
    /// Constructeur du descripteur.
    /// </summary>
    /// <param name="fullName">nom complet absolu de la librairie</param>
    /// <param name="loadState">classement de la librairie</param>
    internal FileData( string fullName, LoadStates loadState ) {
      this.fullName = fullName;
      this.loadState = loadState;
      this.fileName = Path.GetFileName( fullName );
    }

    internal string FullName { get { return fullName; } }
    internal string FileName { get { return fileName; } }

    internal LoadStates LoadState { get { return loadState; } set { loadState = value; } }
    internal bool IsFiltered { get { return isFiltered; } set { isFiltered = value; } }
    internal bool IsLoadable { get { return loadState == LoadStates.isNotLoaded && isFiltered && !IsErrored; } }
    internal bool IsLoaded { get { return loadState != LoadStates.isNotLoaded; } }

    internal bool IsInstalled { get { return installers != null && installers.Count > 0; } }
    internal int InstallCount { get { return installers == null ? 0 : installers.Count; } }

    internal bool IsInfoed { get { return GetSeverityCount(Severity.Info ) > 0; } }
    internal bool IsWarned { get { return GetSeverityCount(Severity.Warning ) > 0; } }
    internal bool IsErrored { get { return GetSeverityCount(Severity.Error ) > 0; } }
    internal List<EPluginManager> Messages { get { return messages; } }

    internal IEnumerable<string> Installers {
      get {
        return installers == null ? new EnumeratorEmpty<string>() : (IEnumerable<string>) installers;
      }
    }

    internal int GetSeverityCount( Severity flags ) {
      int result = 0;
      foreach ( EPluginManager message in messages )
        if ( (message.Severity & flags) != 0 ) result++;
      return result;
    }

    /// <summary>
    /// Ajouter l'identification d'un installateur à la liste des installateurs.
    /// </summary>
    /// <param name="name">identification de l'installateur à ajouter</param>
    internal void AddInstaller( string name ) {
      if ( installers == null ) installers = new List<string>();
      installers.Add( name );
    }

    /// <summary>
    /// Enumérateur pour une collection vide
    /// </summary>
    /// <typeparam name="T">type des éléments à énumérer</typeparam>
    protected struct EnumeratorEmpty<T> : IEnumerable<T>, IEnumerator<T> {
      public T Current { get { throw new InvalidOperationException(); } }
      public bool MoveNext() { return false; }
      public void Reset() { throw new NotSupportedException(); }
      public void Dispose() { }
      object System.Collections.IEnumerator.Current { get { return Current; } }

      public IEnumerator<T> GetEnumerator() {
        return this;
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }
    }
  }

  #endregion

  #region Implémentation du gestionnaire des plugins dynamiques

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                         PluginManager                                       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Implémentation du gestionnaire des plugins dynamiques
  /// </summary>
  internal partial class PluginManager {

    //
    // Champs et propriétés de classe
    //

    /// <summary>
    /// Identification de la version du plugin manager
    /// </summary>
    private const string version = "30 janvier 2010";

    /// <summary>
    /// Filtre des plugins spéciaux
    /// </summary>
    private const string spyFilter = "*.Spy.Plugin.dll";

    /// <summary>
    /// Référence unique sur l'instance courante du gestionnaire de plugins.
    /// </summary>
    private static PluginManager manager = null;

    /// <summary>
    /// Mémorisation de la valeur de la propriété <see cref="VerboseOff"/>
    /// </summary>
    private static bool verboseOff = false;

    /// <summary>
    /// Accès à l'instance courante du gestionnaire de plugin.
    /// </summary>
    /// <remarks>
    /// Le gestionnaire de plugins est instancié par nécessité.
    /// </remarks>
    public static PluginManager Manager {
      get {
        if ( manager == null ) manager = new PluginManager();
        return manager;
      }
    }

    /// <summary>
    /// Indique si le mode bavard de la méthode <see cref="LoadPlugins"/> doit être inhibé
    /// </summary>
    /// <remarks>
    /// Hook permettant de forcer la clôture automatique de la fenêtre s'il n'y a pas de diagnostics
    /// </remarks>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static bool VerboseOff {
      get { return verboseOff; }
      set { verboseOff = value; }
    }

    //
    // Champs d'instance
    //

    private string fileFilter = "";
    private string baseFolder = Application.StartupPath;

    private int filteredCount = 0;
    private int loadedCount = 0;

    private int installedCount = 0;
    private int messageCount = 0;
    private Severity highestSeverity = Severity.None;

    private Dictionary<string, FileData> files = new Dictionary<string, FileData>();
    private Dictionary<string, FileData> installers = new Dictionary<string, FileData>();

    //
    // Propriétés exposées
    //

    internal Dictionary<string, FileData> Files { get { return files; } }

    internal string FileFilter { get { return fileFilter; } }
    internal string BaseFolder { get { return baseFolder; } }
    internal int FilteredCount { get { return filteredCount; } }
    internal int LoadedCount { get { return loadedCount; } }
    internal int InstalledCount { get { return installedCount; } }
    internal int MessageCount { get { return messageCount; } }

    //
    // Gestion générale
    //

    /// <summary>
    /// Constructeur.
    /// </summary>
    public PluginManager() {
      ApplicationState.ApplicationOpened += OnApplicationOpened;
    }

    /// <summary>
    /// Méthode abonnée à l'événement ApplicationOpen.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnApplicationOpened( object sender, EventArgs e ) {
      GenerateMenu();
    }

    //
    // Chargement des plugins
    //

    private void DoRemoveMessages() {
      foreach ( FileData file in files.Values )
        for ( int ix = file.Messages.Count - 1 ; ix >= 0 ; ix-- )
          if ( file.Messages[ ix ] is EPluginManagerClearable )
            file.Messages.RemoveAt( ix );
    }

    /// <summary>
    /// Parcourt les descripteurs de dll et de fichiers pour calculer les statistiques
    /// </summary>
    internal void DoMakeStats() {
      filteredCount = 0;
      loadedCount = 0;
      installedCount = 0;
      messageCount = 0;
      highestSeverity = Severity.None;

      foreach ( FileData file in files.Values ) {
        if ( file.IsFiltered ) filteredCount++;
        if ( file.IsLoaded && file.IsFiltered ) loadedCount++;
        if ( file.IsInstalled ) installedCount += file.InstallCount;

        if ( file.Messages.Count > 0 ) {
          messageCount += file.Messages.Count;
          foreach ( EPluginManager message in file.Messages )
            if ( message.Severity > highestSeverity ) highestSeverity = message.Severity;
        }
      }
    }

    /// <summary>
    /// Recense les librairies actuellement chargées
    /// </summary>
    /// <param name="justLoadedState">indicateur de chargement pour une librairie qui vient d'être chargée</param>
    private void DoGetLoadedDll( LoadStates justLoadedState ) {
      FileData file = null;
      Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

      foreach ( Assembly assembly in assemblies ) {
        if ( assembly.Location.EndsWith( "exe", true, System.Globalization.CultureInfo.CurrentCulture ) ) continue;

        if ( files.TryGetValue( assembly.Location, out file ) ) {
          if ( !file.IsLoaded ) file.LoadState = justLoadedState;
          continue;
        }

        files.Add( assembly.Location, new FileData( assembly.Location, justLoadedState ) );
      }
    }

    /// <summary>
    /// Relevé de toutes les librairies figurant dans un répertoire
    /// </summary>
    /// <param name="folder">répertoire où effectuer le relevé</param>
    private void DoGetAllDll( string folder ) {
      string[] fullNames = Directory.GetFiles( folder, "*.dll" );
      FileData file = null;
      foreach ( string fullName in fullNames ) {
        if ( !fullName.EndsWith( ".dll", true, System.Globalization.CultureInfo.CurrentCulture ) ) continue;
        if ( !files.TryGetValue( fullName, out file ) ) {
          file = new FileData( fullName, LoadStates.isNotLoaded );
          files.Add( fullName, file );
        }
      }
    }

    /// <summary>
    /// Recherche des librairies à charger selon le filtre de fichiers FileFilter.
    /// </summary>
    /// <param name="folder">répertoire de la recherche</param>
    /// <param name="filter">filtre de recherche</param>
    private void DoGetFiltered( string folder, string filter) {
      string[] fullNames = Directory.GetFiles( folder, filter );
      FileData file = null;
      foreach ( string fullName in fullNames ) {
        if ( !files.TryGetValue( fullName, out file ) ) {
          file = new FileData( fullName, LoadStates.isNotLoaded );
          files.Add( fullName, file );
        }
        file.IsFiltered = true;
      }
    }

    /// <summary>
    /// Bilan de contrôle des librairies chargées ou non chargées, et des librairies de plugins
    /// </summary>
    private void DoCheckFiles() {
      foreach ( FileData file in files.Values ) {
        if ( file.Messages.Count != 0 ) continue;

        if ( !file.IsLoaded ) {
          file.Messages.Add( new EPluginManagerClearable( Severity.Info, "Contrôle des librairies", string.Empty, "La librairie figure dans le répertoire de l'exécutable mais elle n'est pas chargée dans l'application", null ) );
          continue ;
        }

        if ( file.IsFiltered && file.LoadState != LoadStates.isMgrLoaded ) {
          file.Messages.Add( new EPluginManager( Severity.Warning, "Contrôle des librairies", string.Empty, "La librairie a été chargée par le framework net bien qu'il s'agisse d'une librairie devant être chargée dynamiquement par le gestionnaire de plugins" ) );
          continue;
        }

        if ( file.IsFiltered && file.LoadState == LoadStates.isMgrLoaded && file.InstallCount == 0 ) {
          file.Messages.Add( new EPluginManager( Severity.Warning, "Contrôle des librairies", string.Empty, "La librairie se présente comme un plugin à charger dynamiquement, mais aucun installateur, c'est-à-dire aucune classe taggée par l'attribut [PslPluginInstaller], n'a été trouvé dans la librairie" ) );
          continue;
        }
      }
    }

    /// <summary>
    /// Contrôle et installation d'un installateur de plugin
    /// </summary>
    /// <param name="file">descripteur des informations associées à la librairie</param>
    /// <param name="installer">descripteur réflexig du type associé à la classe de l'installateur</param>
    private void DoPluginInstall(FileData file, Type installer) {

      FileData previous = null;
      if ( installers.TryGetValue( installer.FullName, out previous ) ) 
        throw new EPluginManager( Severity.Error, "Validation d'un installateur", installer.FullName, "Un installateur de même nom existe déjà dans la librairie {0}", previous.FileName );

      installers.Add( installer.FullName, file );

      BindingFlags flags = BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

      if (installer.GetMethod( "Install", flags ) == null)
        throw new EPluginManager( Severity.Error, "Validation d'un installateur", installer.FullName, "L'installateur ne comporte aucune méthode Install" );

      MethodInfo install = installer.GetMethod( "Install", flags, null, new Type[ 0 ], null );
      if ( install == null )
        throw new EPluginManager( Severity.Error, "Validation d'un installateur", installer.FullName, "L'installateur ne comporte pas de méthode Install sans paramètres" );

      if ( !install.IsStatic )
        throw new EPluginManager( Severity.Error, "Validation d'un installateur", installer.FullName, "La méthode Install sans paramètres trouvée n'est pas une méthode de classe" );

      try { install.Invoke( null, null ); }
      catch ( Exception ex ) { throw new EPluginManager( Severity.Error, "Application d'un installateur", installer.FullName, "Exception déclenchée pendant l'application de la méthode Install d'un installateur", ex ); }

      file.AddInstaller( installer.FullName );
    }

    /// <summary>
    /// Chargement et installation d'un plugin
    /// </summary>
    /// <param name="file">descripteur des informations associées à la librairie</param>
    private void DoPluginLoad(FileData file) {
      try {
        Assembly assembly = Assembly.LoadFrom( file.FullName );
        file.LoadState = LoadStates.isMgrLoaded;

        Type[] types = assembly.GetTypes();

        foreach ( Type type in types ) {
          if ( !Attribute.IsDefined( type, typeof( PslPluginInstaller ), true ) ) continue;
          try   { DoPluginInstall( file, type ); }
          catch ( EPluginManager ex ) { file.Messages.Add( ex ); }
        }
      }
      catch ( Exception ex ) { file.Messages.Add( new EPluginManager( Severity.Error, "Traitement d'une librairie", string.Empty, "Exception pendant le traitement de la librairie", ex ) ); }
    }

    /// <summary>
    /// Déclenchement du chargement des plugins
    /// </summary>
    /// <param name="verbose">si true, affichage de la fenêtre de progression</param>
    /// <param name="fileFilter">filtre de fichiers pour détecter les librairies à charger</param>
    /// <returns>true si aucun erreur n'a été détectée</returns>
    internal bool LoadPlugins( bool verbose, string fileFilter ) {
      this.fileFilter = fileFilter;

      using ( PluginManagerForm box = verbose ? new PluginManagerForm( this, version, false ) : null ) {
        try {
          DoGetLoadedDll( LoadStates.isPreLoaded );
          DoGetAllDll( baseFolder );
          DoGetFiltered( baseFolder, fileFilter );
          DoGetFiltered( baseFolder, spyFilter );

          if ( verbose ) {
            int loadable = 0;
            foreach ( FileData file in files.Values )
              if ( file.IsLoadable ) loadable++;
            box.SetMaximum( loadable );
          }

          foreach ( FileData file in files.Values ) {
            if ( !file.IsLoadable ) continue;
            if (verbose) box.Progress( file );
            DoPluginLoad( file );
          }
        } catch ( Exception ex ) {
          ExceptionBox.Show( ex, "Exception pendant le processus de chargement" );
        }
      }

      DoGetLoadedDll( LoadStates.isPostLoaded );
      DoCheckFiles();
      DoMakeStats();

      if ( (verbose && !verboseOff) || highestSeverity > Severity.Info ) ShowReport( false );
      return messageCount == 0;
    }

    /// <summary>
    /// Déclenche l'affichage modal de la fenêtre de rapport.
    /// </summary>
    /// <param name="checkDll">si true, recenser les dll actuellement chargées</param>
    internal void ShowReport( bool checkDll ) {

      if ( checkDll ) {
        DoRemoveMessages();
        DoGetLoadedDll( LoadStates.isExeLoaded );
        DoCheckFiles();
        DoMakeStats();
      }

      using ( PluginManagerForm box = new PluginManagerForm( this, version, true ) ) {
        box.ShowDialog();
      }
    }
  }

  #endregion
}
