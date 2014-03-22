namespace Psl.Applications {
  partial class ArchiverCluster {
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ArchiverCluster));
      this.openDialog = new System.Windows.Forms.OpenFileDialog();
      this.saveDialog = new System.Windows.Forms.SaveFileDialog();
      this.menu = new System.Windows.Forms.MenuStrip();
      this.muFile = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.muFileConfigLoad = new System.Windows.Forms.ToolStripMenuItem();
      this.muFileConfigSave = new System.Windows.Forms.ToolStripMenuItem();
      this.muFileConfigSaveAs = new System.Windows.Forms.ToolStripMenuItem();
      this.muView = new System.Windows.Forms.ToolStripMenuItem();
      this.muViewToolBars = new System.Windows.Forms.ToolStripMenuItem();
      this.muViewToolBarsArchiver = new System.Windows.Forms.ToolStripMenuItem();
      this.muHelp = new System.Windows.Forms.ToolStripMenuItem();
      this.muHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.archiverTools = new System.Windows.Forms.ToolStrip();
      this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
      this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
      this.actions = new Psl.Actions.ActionList(this.components);
      this.acArchiverAbout = new Psl.Actions.Action(this.components);
      this.acArchiverLoad = new Psl.Actions.Action(this.components);
      this.acArchiverSave = new Psl.Actions.Action(this.components);
      this.acArchiverSaveAs = new Psl.Actions.Action(this.components);
      this.acArchiverToolsVisible = new Psl.Actions.Action(this.components);
      this.applicationEvents = new Psl.Applications.ApplicationEvents(this.components);
      this.menu.SuspendLayout();
      this.archiverTools.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
      this.SuspendLayout();
      // 
      // openDialog
      // 
      this.openDialog.CheckFileExists = false;
      this.openDialog.DefaultExt = "xpl";
      this.openDialog.Filter = "Fichiers d\'archives (*.archiver)|*.archiver|Tous les fichiers|*.*";
      this.openDialog.Title = "Ouvrir un fichier d\'archive";
      this.openDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openDialog_FileOk);
      // 
      // saveDialog
      // 
      this.saveDialog.DefaultExt = "xpl";
      this.saveDialog.Filter = "Fichiers d\'archives (*.archiver)|*.archiver|Tous les fichiers|*.*";
      this.saveDialog.Title = "Enregistrer un fichier d\'archive";
      // 
      // menu
      // 
      this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muFile,
            this.muView,
            this.muHelp});
      this.menu.Location = new System.Drawing.Point(0, 0);
      this.menu.Name = "menu";
      this.menu.Size = new System.Drawing.Size(181, 24);
      this.menu.TabIndex = 1;
      this.menu.Text = "menuStrip1";
      // 
      // muFile
      // 
      this.muFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.muFileConfigLoad,
            this.muFileConfigSave,
            this.muFileConfigSaveAs});
      this.muFile.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
      this.muFile.MergeIndex = 1;
      this.muFile.Name = "muFile";
      this.muFile.Size = new System.Drawing.Size(54, 20);
      this.muFile.Text = "&Fichier";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
      this.toolStripSeparator1.MergeIndex = 9000;
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(210, 6);
      // 
      // muFileConfigLoad
      // 
      this.muFileConfigLoad.Image = ((System.Drawing.Image)(resources.GetObject("muFileConfigLoad.Image")));
      this.muFileConfigLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.muFileConfigLoad.MergeAction = System.Windows.Forms.MergeAction.Insert;
      this.muFileConfigLoad.MergeIndex = 9000;
      this.muFileConfigLoad.Name = "muFileConfigLoad";
      this.muFileConfigLoad.ShortcutKeyDisplayString = "";
      this.muFileConfigLoad.Size = new System.Drawing.Size(213, 22);
      this.muFileConfigLoad.Text = "Restaurer une archive";
      this.muFileConfigLoad.ToolTipText = "Restaurer l\'état de l\'application à partir d\'un fichier d\'archive";
      // 
      // muFileConfigSave
      // 
      this.muFileConfigSave.Image = ((System.Drawing.Image)(resources.GetObject("muFileConfigSave.Image")));
      this.muFileConfigSave.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.muFileConfigSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
      this.muFileConfigSave.MergeIndex = 9000;
      this.muFileConfigSave.Name = "muFileConfigSave";
      this.muFileConfigSave.ShortcutKeyDisplayString = "";
      this.muFileConfigSave.Size = new System.Drawing.Size(213, 22);
      this.muFileConfigSave.Text = "Enregistrer l\'archive";
      this.muFileConfigSave.ToolTipText = "Enregistrer l\'état de l\'application dans un fichier d\'archive";
      // 
      // muFileConfigSaveAs
      // 
      this.muFileConfigSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("muFileConfigSaveAs.Image")));
      this.muFileConfigSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.muFileConfigSaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
      this.muFileConfigSaveAs.MergeIndex = 9000;
      this.muFileConfigSaveAs.Name = "muFileConfigSaveAs";
      this.muFileConfigSaveAs.ShortcutKeyDisplayString = "";
      this.muFileConfigSaveAs.Size = new System.Drawing.Size(213, 22);
      this.muFileConfigSaveAs.Text = "Enregistrer l\'archive sous...";
      this.muFileConfigSaveAs.ToolTipText = "Enregistrer l\'état de l\'application dans une autre archive";
      // 
      // muView
      // 
      this.muView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muViewToolBars});
      this.muView.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
      this.muView.MergeIndex = 10;
      this.muView.Name = "muView";
      this.muView.Size = new System.Drawing.Size(70, 20);
      this.muView.Text = "&Affichage";
      // 
      // muViewToolBars
      // 
      this.muViewToolBars.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muViewToolBarsArchiver});
      this.muViewToolBars.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
      this.muViewToolBars.MergeIndex = 1000;
      this.muViewToolBars.Name = "muViewToolBars";
      this.muViewToolBars.Size = new System.Drawing.Size(148, 22);
      this.muViewToolBars.Text = "&Barres d\'outils";
      // 
      // muViewToolBarsArchiver
      // 
      this.muViewToolBarsArchiver.CheckOnClick = true;
      this.muViewToolBarsArchiver.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.muViewToolBarsArchiver.MergeAction = System.Windows.Forms.MergeAction.Insert;
      this.muViewToolBarsArchiver.MergeIndex = 0;
      this.muViewToolBarsArchiver.Name = "muViewToolBarsArchiver";
      this.muViewToolBarsArchiver.ShortcutKeyDisplayString = "";
      this.muViewToolBarsArchiver.Size = new System.Drawing.Size(125, 22);
      this.muViewToolBarsArchiver.Text = "Archiveur";
      this.muViewToolBarsArchiver.ToolTipText = "Montrer/cacher la barre d\'outils de l\'archiveur";
      // 
      // muHelp
      // 
      this.muHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.muHelpAbout});
      this.muHelp.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
      this.muHelp.MergeIndex = 9999;
      this.muHelp.Name = "muHelp";
      this.muHelp.Size = new System.Drawing.Size(24, 20);
      this.muHelp.Text = "&?";
      // 
      // muHelpAbout
      // 
      this.muHelpAbout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
      this.muHelpAbout.Image = ((System.Drawing.Image)(resources.GetObject("muHelpAbout.Image")));
      this.muHelpAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.muHelpAbout.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
      this.muHelpAbout.MergeIndex = 1;
      this.muHelpAbout.Name = "muHelpAbout";
      this.muHelpAbout.ShortcutKeyDisplayString = "";
      this.muHelpAbout.Size = new System.Drawing.Size(131, 22);
      this.muHelpAbout.Text = "&A propos...";
      this.muHelpAbout.ToolTipText = "Informations de version concernant l\'archiveur";
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuItem1.Image")));
      this.toolStripMenuItem1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.ShortcutKeyDisplayString = "";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(240, 22);
      this.toolStripMenuItem1.Text = "A propos du plugin d\'archivage";
      this.toolStripMenuItem1.ToolTipText = "Informations de version concernant l\'archiveur";
      // 
      // archiverTools
      // 
      this.archiverTools.Dock = System.Windows.Forms.DockStyle.None;
      this.archiverTools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripButton3});
      this.archiverTools.Location = new System.Drawing.Point(25, 35);
      this.archiverTools.Name = "archiverTools";
      this.archiverTools.Size = new System.Drawing.Size(112, 25);
      this.archiverTools.TabIndex = 2;
      this.archiverTools.Text = "toolStrip1";
      // 
      // toolStripButton1
      // 
      this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
      this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton1.Name = "toolStripButton1";
      this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
      this.toolStripButton1.Text = "Restaurer une archive";
      this.toolStripButton1.ToolTipText = "Restaurer l\'état de l\'application à partir d\'un fichier d\'archive";
      // 
      // toolStripButton2
      // 
      this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton2.Image")));
      this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton2.Name = "toolStripButton2";
      this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
      this.toolStripButton2.Text = "Enregistrer l\'archive";
      this.toolStripButton2.ToolTipText = "Enregistrer l\'état de l\'application dans un fichier d\'archive";
      // 
      // toolStripButton3
      // 
      this.toolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.toolStripButton3.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton3.Image")));
      this.toolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.toolStripButton3.Name = "toolStripButton3";
      this.toolStripButton3.Size = new System.Drawing.Size(23, 22);
      this.toolStripButton3.Text = "Enregistrer l\'archive sous...";
      this.toolStripButton3.ToolTipText = "Enregistrer l\'état de l\'application dans une autre archive";
      // 
      // actions
      // 
      this.actions.Actions.Add(this.acArchiverAbout);
      this.actions.Actions.Add(this.acArchiverLoad);
      this.actions.Actions.Add(this.acArchiverSave);
      this.actions.Actions.Add(this.acArchiverSaveAs);
      this.actions.Actions.Add(this.acArchiverToolsVisible);
      // 
      // acArchiverAbout
      // 
      this.acArchiverAbout.Image = ((System.Drawing.Image)(resources.GetObject("acArchiverAbout.Image")));
      this.acArchiverAbout.Targets.Add(this.toolStripMenuItem1);
      this.acArchiverAbout.Text = "A propos du plugin d\'archivage";
      this.acArchiverAbout.ToolTipText = "Informations de version concernant l\'archiveur";
      this.acArchiverAbout.Execute += new System.EventHandler(this.acArchiverAbout_Execute);
      // 
      // acArchiverLoad
      // 
      this.acArchiverLoad.Image = ((System.Drawing.Image)(resources.GetObject("acArchiverLoad.Image")));
      this.acArchiverLoad.Targets.Add(this.muFileConfigLoad);
      this.acArchiverLoad.Targets.Add(this.toolStripButton1);
      this.acArchiverLoad.Text = "Restaurer une archive";
      this.acArchiverLoad.ToolTipText = "Restaurer l\'état de l\'application à partir d\'un fichier d\'archive";
      this.acArchiverLoad.Execute += new System.EventHandler(this.acArchiverLoad_Execute);
      // 
      // acArchiverSave
      // 
      this.acArchiverSave.Image = ((System.Drawing.Image)(resources.GetObject("acArchiverSave.Image")));
      this.acArchiverSave.Targets.Add(this.muFileConfigSave);
      this.acArchiverSave.Targets.Add(this.toolStripButton2);
      this.acArchiverSave.Text = "Enregistrer l\'archive";
      this.acArchiverSave.ToolTipText = "Enregistrer l\'état de l\'application dans un fichier d\'archive";
      this.acArchiverSave.Execute += new System.EventHandler(this.acArchiverSave_Execute);
      // 
      // acArchiverSaveAs
      // 
      this.acArchiverSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("acArchiverSaveAs.Image")));
      this.acArchiverSaveAs.Targets.Add(this.muFileConfigSaveAs);
      this.acArchiverSaveAs.Targets.Add(this.toolStripButton3);
      this.acArchiverSaveAs.Text = "Enregistrer l\'archive sous...";
      this.acArchiverSaveAs.ToolTipText = "Enregistrer l\'état de l\'application dans une autre archive";
      this.acArchiverSaveAs.Execute += new System.EventHandler(this.acArchiverSaveAs_Execute);
      // 
      // acArchiverToolsVisible
      // 
      this.acArchiverToolsVisible.CheckOnClick = true;
      this.acArchiverToolsVisible.Targets.Add(this.muViewToolBarsArchiver);
      this.acArchiverToolsVisible.Text = "Archiveur";
      this.acArchiverToolsVisible.ToolTipText = "Montrer/cacher la barre d\'outils de l\'archiveur";
      // 
      // applicationEvents
      // 
      this.applicationEvents.ApplicationOpen += new System.EventHandler(this.applicationEvents_ApplicationOpen);
      this.applicationEvents.ApplicationOpened += new System.EventHandler(this.applicationEvents_ApplicationOpened);
      this.applicationEvents.ApplicationClosing += new System.Windows.Forms.FormClosingEventHandler(this.applicationEvents_ApplicationClosing);
      this.applicationEvents.ApplicationIdle += new System.EventHandler(this.applicationEvents_ApplicationIdle);
      this.applicationEvents.Archive += new Psl.Applications.ArchiverEventHandler(this.applicationEvents_Archive);
      // 
      // ArchiverCluster
      // 
      this.Controls.Add(this.archiverTools);
      this.Controls.Add(this.menu);
      this.Name = "ArchiverCluster";
      this.Size = new System.Drawing.Size(181, 72);
      this.menu.ResumeLayout(false);
      this.menu.PerformLayout();
      this.archiverTools.ResumeLayout(false);
      this.archiverTools.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    private Psl.Actions.ActionList actions;
    private Psl.Actions.Action acArchiverAbout;
    private Psl.Actions.Action acArchiverLoad;
    private Psl.Actions.Action acArchiverSave;
    private Psl.Actions.Action acArchiverSaveAs;
    private Psl.Actions.Action acArchiverToolsVisible;
    private System.Windows.Forms.SaveFileDialog saveDialog;
    private Psl.Applications.ApplicationEvents applicationEvents;
    private System.Windows.Forms.OpenFileDialog openDialog;
    private System.Windows.Forms.MenuStrip menu;
    private System.Windows.Forms.ToolStripMenuItem muFile;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem muFileConfigLoad;
    private System.Windows.Forms.ToolStripMenuItem muFileConfigSave;
    private System.Windows.Forms.ToolStripMenuItem muFileConfigSaveAs;
    private System.Windows.Forms.ToolStripMenuItem muView;
    private System.Windows.Forms.ToolStripMenuItem muViewToolBars;
    private System.Windows.Forms.ToolStripMenuItem muHelp;
    private System.Windows.Forms.ToolStripButton toolStripButton1;
    private System.Windows.Forms.ToolStripButton toolStripButton2;
    private System.Windows.Forms.ToolStripButton toolStripButton3;
    private System.Windows.Forms.ToolStripMenuItem muViewToolBarsArchiver;
    private System.Windows.Forms.ToolStrip archiverTools;
    private System.Windows.Forms.ToolStripMenuItem muHelpAbout;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
  }
}
