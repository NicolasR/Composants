/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Fenêtre d'afichage du rapport du gestionnaire des plugins dynamiques
 * 
 * 31 01 2008 : version initiale pour aihm 2007-2008
 * 20 04 2009 : petite amélioration dans l'affichage du statut des librairies
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Psl.Applications.Manager {

  // <exclude
  /// <summary>
  /// Fenêtre d'affichage du rapport
  /// </summary>
  internal partial class PluginManagerForm : Form {

    /// <summary>
    /// Référence sur l'instance du plugin manager
    /// </summary>
    private PluginManager manager = null;

    private const int icoDllBleu = 5;
    private const int icoPlugin = 6;
    private const int icoInfo = 10;
    private const int icoWarning = 11;
    private const int icoStop = 12;
    private const int icoRondRouge = 13;
    private const int icoRondVert = 14;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="manager">référence sur l'instance du gestionnaire de plugins</param>
    /// <param name="version">version courante du gestionnaire</param>
    /// <param name="modal">si true, pour affichage modal, si false, pour fenêtre de progression</param>
    internal PluginManagerForm( PluginManager manager, string version, bool modal ) {
      InitializeComponent();
      this.manager = manager;
      if (modal)
        DoUpdateReport();
      else
        Visible = true;
      PluginManagerForm_ResizeEnd( this, EventArgs.Empty );
      Refresh();
    }

    //
    // Méthodes de service
    //

    /// <summary>
    /// Ajoute une librarie à une liste d'affichage des librairies.
    /// </summary>
    /// <param name="list">liste hôte</param>
    /// <param name="file">descripteur des informations associées à la librairie</param>
    private void DoAddLibrary( ListView list, FileData file ) {

      // déterminer l'index de l'icône
      int iconIndex = file.IsFiltered ? icoPlugin : icoDllBleu;

      // déterminer l'index image de l'état
      int stateIndex = 0;
      if ( file.IsFiltered )
        stateIndex = file.IsErrored ? icoStop : file.IsWarned ? icoWarning : file.IsInfoed ? icoInfo : file.IsLoaded ? file.IsInstalled ? icoRondVert : icoRondRouge : icoWarning;
      else
        stateIndex = file.IsErrored ? icoStop : file.IsWarned ? icoWarning : file.IsInfoed ? icoInfo : file.IsLoaded ? -1 : icoWarning;

      // ajouter l'itemReport
      ListViewItem item = list.Items.Add( file.FileName );
      item.ImageIndex = stateIndex;
      item.StateImageIndex = iconIndex;
      item.Tag = file;
    }

    private ListViewItem DoAddProperty( ListView list, string name, string value, bool bold, Color color, object tag ) {
      if ( string.IsNullOrEmpty( value ) ) return null;
      ListViewItem item = list.Items.Add( name );
      item.SubItems.Add( value );
      if ( bold ) item.Font = new Font( item.Font, FontStyle.Bold );
      item.ForeColor = color;
      item.Tag = tag;
      return item;
    }

    /// <summary>
    /// Ajouter une propriété à une liste de propriétés
    /// </summary>
    /// <param name="list">liste hôte</param>
    /// <param name="name">nom de la propriété</param>
    /// <param name="value">valeur de la propriété</param>
    private ListViewItem DoAddProperty( ListView list, string name, string value ) {
      return DoAddProperty( list, name, value, false, list.ForeColor, null );
    }

    /// <summary>
    /// Mise à jour des informations de l'onglet du gestionnaire des plugins
    /// </summary>
    private void DoUpdateManagerData() {
      properties.BeginUpdate();
      try {
        properties.Items.Clear();

        DoAddProperty( properties, "Répertoire de base", manager.BaseFolder );
        DoAddProperty( properties, "Filtre des librairies", manager.FileFilter );
        DoAddProperty( properties, "Librairies filtrées", string.Format( "{0}  librairie(s) filtrée(s) détectée(s)", manager.FilteredCount ) );
        DoAddProperty( properties, "Librairies chargées", string.Format( "{0}  librairie(s) chargée(s) par le gestionnaire", manager.LoadedCount ) );
        //DoAddProperty( properties, "Plugins installables", string.Format( "{0}  installateur(s) de plugins détecté(s)", manager.InstallableCount ) );
        DoAddProperty( properties, "Plugins installés", string.Format( "{0}  plugin(s) effectivement installé(s)", manager.InstalledCount ) );
        DoAddProperty( properties, "Diagnostics", manager.MessageCount == 0 ? "aucun diagnostic détecté" : string.Format( "{0}  diagnostic(s) détecté(s)", manager.MessageCount ) );
        propertiesHeaderValues.Width = -2;
      }
      finally { properties.EndUpdate(); }

      string installed = manager.InstalledCount == 0 ? "aucun plugin installé" : string.Format( "{0}  plugin(s) installé(s)", manager.InstalledCount );
      string errored = manager.MessageCount == 0 ? "" : string.Format( "{0}  diagnostic(s)", manager.MessageCount );
      statusLabel.Text = installed + ",  " + errored;
    }

    /// <summary>
    /// Mise à jour des onglets relatifs à la librairie actuellement sélectionnée
    /// </summary>
    private void DoUpdateLibraryData() {
      library.Items.Clear();
      pages.SelectedTab = pageLibrary;

      if ( libraries.SelectedItems.Count != 1 ) return;

      FileData file = (FileData) libraries.SelectedItems[ 0 ].Tag;

      DoAddProperty( library, "Librairie", file.FileName );
      DoAddProperty( library, "Répertoire", Path.GetDirectoryName( file.FullName ) );
      DoAddProperty( library, "Statut", file.IsFiltered ? "librairie de plugin à charger dynamiquement" : "librairie non prise en charge par le gestionnaire de plugins dynamiques" );

      switch ( file.LoadState ) {

        case LoadStates.isMgrLoaded:
          DoAddProperty( library, "Chargement", "librairie chargée par le gestionnaire de plugins dynamiques" );
          DoAddProperty( library, "Installateurs", file.InstallCount == 0 ? "aucun installateur correct de plugin détecté" : string.Format( "{0}  plugin(s) installé(s)", file.InstallCount ) );

          foreach ( string installer in file.Installers )
            DoAddProperty( library, "", installer );
          break;

        case LoadStates.isPreLoaded:
          DoAddProperty( library, "Chargement", "librairie chargée par le framework net avant le chargement des plugins" );
          DoAddProperty( library, "Installateurs", "pas de détection dans les librairies chargées par le framework net" );
          break;

        case LoadStates.isPostLoaded:
          DoAddProperty( library, "Chargement", "librairie chargée par le framework net par l'effet du chargement d'un plugin" );
          DoAddProperty( library, "Installateurs", "pas de détection dans les librairies chargées par le framework net" );
          break;

        case LoadStates.isExeLoaded:
          DoAddProperty( library, "Chargement", "librairie chargée par le framework net pendant l'exécution de l'application" );
          DoAddProperty( library, "Installateurs", "pas de détection dans les librairies chargées par le framework net" );
          break;

        case LoadStates.isNotLoaded:
          DoAddProperty( library, "Chargement", "librairie non chargée" );
          break;
      }

      if (file.Messages.Count > 0 ) {
        DoAddProperty( library, "Diagnostics", string.Format( "{0}  diagnostic(s) détecté(s), voir le détail dans l'onglet des diagnostics", file.Messages.Count ) );

        foreach ( EPluginManager message in file.Messages ) {
          Color color = message.Severity == Severity.Error ? Color.Red : Color.Blue;
          string title = message.Severity == Severity.Error ? "Diagnostic" : message.Severity == Severity.Warning ? "Avertissement" : "Information";
          string inner = message.InnerException == null ? string.Empty : ">>> double-cliquer pour afficher le détail de l'exception";

          DoAddProperty( library, string.Empty, " " );
          DoAddProperty( library, title         , message.Message  , true , color, message.InnerException );
          DoAddProperty( library, "Exception"   , inner            , false, color, message.InnerException );
          DoAddProperty( library, "Phase"       , message.Phase    , false, color, message.InnerException );
          DoAddProperty( library, "Installateur", message.Installer, false, color, message.InnerException );
        }
      }

      libraryHeaderValues.Width = -2;
    }

    /// <summary>
    /// Mise à jour de l'ensemble de la fenêtre du rapport.
    /// </summary>
    private void DoUpdateReport() {

      // nettoyer l'affichage de la progression
      statusProgress.Visible = false;
      statusLabel.Text = "";

      libraries.BeginUpdate();
      try {
        libraries.Items.Clear();
        foreach ( FileData file in manager.Files.Values )
          if ( btShowAll.Checked || file.IsFiltered || file.Messages.Count != 0 )
            DoAddLibrary( libraries, file );
        PluginManagerForm_ResizeEnd( this, EventArgs.Empty );
      } finally { libraries.EndUpdate(); }
      libraries.Update();

      DoUpdateManagerData();
      pages.SelectedTab = pageManager;
    }

    //
    // Gestion de la fenêtre de progression
    //

    /// <summary>
    /// Déterminer le maximum pour la jauge de progression
    /// </summary>
    /// <param name="value">valeur maximale de la jauge</param>
    internal void SetMaximum( int value ) {
      statusProgress.Maximum = value;
      //statusProgress.Invalidate();
    }

    /// <summary>
    /// Détermine une étape de progression comme chargement d'une librairie
    /// </summary>
    /// <param name="file">descripteur des informations associées à la librairie</param>
    internal void Progress( FileData file ) {
      DoAddLibrary( libraries, file );
      libraries.Update();
      statusLabel.Text = file.FileName == "" ? "" : "Chargement de : " + file.FileName;
      statusLabel.Update();
      statusProgress.Value = libraries.Items.Count;
      statusProgress.Update();
      Application.DoEvents();
    }

    //
    // Commandes
    //

    private void btOk_Click( object sender, EventArgs e ) {
      Close();
    }

    private void btShowAll_Click( object sender, EventArgs e ) {
      btShowAll.Checked = !btShowAll.Checked;
      DoUpdateReport();
    }

    //
    // Evénements
    //

    private void libraries_SelectedIndexChanged( object sender, EventArgs e ) {
      DoUpdateLibraryData();
    }

    private void library_MouseDoubleClick( object sender, MouseEventArgs e ) {
      ListView list = sender as ListView;
      if ( list == null || list.SelectedItems.Count != 1 ) return;
      ListViewItem item = list.SelectedItems[ 0 ];
      Exception exception = item.Tag as Exception;
      if ( exception == null ) return;
      ExceptionBox.Show( this, exception, "Exception interceptée par le manager de plugins" );
    }

    private void PluginManagerForm_FormClosed( object sender, FormClosedEventArgs e ) {
      Dispose();
    }

    private void PluginManagerForm_ResizeEnd( object sender, EventArgs e ) {
      listHeaderName.Width = -2;
      libraryHeaderValues.Width = -2;
      propertiesHeaderValues.Width = -2;
    }
  }
}