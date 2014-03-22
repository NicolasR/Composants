namespace Stl.Tme.Components.Kernel
{
    partial class DocumentManager
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentManager));
            this.toolspanel = new Psl.Controls.ToolStripPanelEnh();
            this.tools = new System.Windows.Forms.ToolStrip();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveAs = new System.Windows.Forms.ToolStripButton();
            this.buttonSaveAll = new System.Windows.Forms.ToolStripButton();
            this.buttonClose = new System.Windows.Forms.ToolStripButton();
            this.buttonCloseAll = new System.Windows.Forms.ToolStripButton();
            this.buttonCopy = new System.Windows.Forms.ToolStripButton();
            this.buttonCut = new System.Windows.Forms.ToolStripButton();
            this.buttonPaste = new System.Windows.Forms.ToolStripButton();
            this.buttonUndo = new System.Windows.Forms.ToolStripButton();
            this.buttonRedo = new System.Windows.Forms.ToolStripButton();
            this.actions = new Psl.Actions.ActionList(this.components);
            this.actionSaveAs = new Psl.Actions.Action(this.components);
            this.itemSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.actionSave = new Psl.Actions.Action(this.components);
            this.itemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.actionCopy = new Psl.Actions.Action(this.components);
            this.itemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.actionCut = new Psl.Actions.Action(this.components);
            this.itemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.actionPaste = new Psl.Actions.Action(this.components);
            this.itemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.actionUndo = new Psl.Actions.Action(this.components);
            this.itemUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.actionSaveAll = new Psl.Actions.Action(this.components);
            this.sauvegarderToutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionClose = new Psl.Actions.Action(this.components);
            this.fermerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionCloseAll = new Psl.Actions.Action(this.components);
            this.fermerToutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionRedo = new Psl.Actions.Action(this.components);
            this.actionCloseDocument = new Psl.Actions.Action(this.components);
            this.fermerDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.itemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.filechooser = new System.Windows.Forms.OpenFileDialog();
            this.filesaver = new System.Windows.Forms.SaveFileDialog();
            this.viewList = new System.Windows.Forms.TabControl();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.itemNew = new System.Windows.Forms.ToolStripMenuItem();
            this.menuEdition = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.bar = new Psl.Controls.ActiveBar();
            this.toolspanel.SuspendLayout();
            this.tools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolspanel
            // 
            this.toolspanel.Controls.Add(this.tools);
            this.toolspanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolspanel.Location = new System.Drawing.Point(0, 24);
            this.toolspanel.Name = "toolspanel";
            this.toolspanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolspanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolspanel.Size = new System.Drawing.Size(626, 25);
            // 
            // tools
            // 
            this.tools.Dock = System.Windows.Forms.DockStyle.None;
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSave,
            this.buttonSaveAs,
            this.buttonSaveAll,
            this.buttonClose,
            this.buttonCloseAll,
            this.buttonCopy,
            this.buttonCut,
            this.buttonPaste,
            this.buttonUndo,
            this.buttonRedo});
            this.tools.Location = new System.Drawing.Point(3, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(240, 25);
            this.tools.TabIndex = 0;
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(23, 22);
            this.buttonSave.Text = "&Sauvegarder";
            // 
            // buttonSaveAs
            // 
            this.buttonSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveAs.Image")));
            this.buttonSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveAs.Name = "buttonSaveAs";
            this.buttonSaveAs.Size = new System.Drawing.Size(23, 22);
            this.buttonSaveAs.Text = "&Sauvegarder sous";
            // 
            // buttonSaveAll
            // 
            this.buttonSaveAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSaveAll.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveAll.Image")));
            this.buttonSaveAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveAll.Name = "buttonSaveAll";
            this.buttonSaveAll.Size = new System.Drawing.Size(23, 22);
            this.buttonSaveAll.Text = "&Sauvegarder tout";
            // 
            // buttonClose
            // 
            this.buttonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClose.Image = ((System.Drawing.Image)(resources.GetObject("buttonClose.Image")));
            this.buttonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(23, 22);
            this.buttonClose.Text = "&Fermer vue courante";
            // 
            // buttonCloseAll
            // 
            this.buttonCloseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCloseAll.Image = ((System.Drawing.Image)(resources.GetObject("buttonCloseAll.Image")));
            this.buttonCloseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCloseAll.Name = "buttonCloseAll";
            this.buttonCloseAll.Size = new System.Drawing.Size(23, 22);
            this.buttonCloseAll.Text = "&Fermer tout";
            // 
            // buttonCopy
            // 
            this.buttonCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCopy.Image = ((System.Drawing.Image)(resources.GetObject("buttonCopy.Image")));
            this.buttonCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCopy.Name = "buttonCopy";
            this.buttonCopy.Size = new System.Drawing.Size(23, 22);
            this.buttonCopy.Text = "&Copier";
            // 
            // buttonCut
            // 
            this.buttonCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCut.Image = ((System.Drawing.Image)(resources.GetObject("buttonCut.Image")));
            this.buttonCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCut.Name = "buttonCut";
            this.buttonCut.Size = new System.Drawing.Size(23, 22);
            this.buttonCut.Text = "&Couper";
            // 
            // buttonPaste
            // 
            this.buttonPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonPaste.Image = ((System.Drawing.Image)(resources.GetObject("buttonPaste.Image")));
            this.buttonPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPaste.Name = "buttonPaste";
            this.buttonPaste.Size = new System.Drawing.Size(23, 22);
            this.buttonPaste.Text = "&Coller";
            // 
            // buttonUndo
            // 
            this.buttonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUndo.Image = ((System.Drawing.Image)(resources.GetObject("buttonUndo.Image")));
            this.buttonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(23, 22);
            this.buttonUndo.Text = "&Défaire";
            // 
            // buttonRedo
            // 
            this.buttonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRedo.Image = ((System.Drawing.Image)(resources.GetObject("buttonRedo.Image")));
            this.buttonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRedo.Name = "buttonRedo";
            this.buttonRedo.Size = new System.Drawing.Size(23, 22);
            this.buttonRedo.Text = "&Refaire";
            // 
            // actions
            // 
            this.actions.Actions.Add(this.actionSaveAs);
            this.actions.Actions.Add(this.actionSave);
            this.actions.Actions.Add(this.actionCopy);
            this.actions.Actions.Add(this.actionCut);
            this.actions.Actions.Add(this.actionPaste);
            this.actions.Actions.Add(this.actionUndo);
            this.actions.Actions.Add(this.actionSaveAll);
            this.actions.Actions.Add(this.actionClose);
            this.actions.Actions.Add(this.actionCloseAll);
            this.actions.Actions.Add(this.actionRedo);
            this.actions.Actions.Add(this.actionCloseDocument);
            // 
            // actionSaveAs
            // 
            this.actionSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("actionSaveAs.Image")));
            this.actionSaveAs.Targets.Add(this.itemSaveAs);
            this.actionSaveAs.Targets.Add(this.buttonSaveAs);
            this.actionSaveAs.Text = "&Sauvegarder sous";
            this.actionSaveAs.Execute += new System.EventHandler(this.actionSaveAs_Execute);
            // 
            // itemSaveAs
            // 
            this.itemSaveAs.Image = ((System.Drawing.Image)(resources.GetObject("itemSaveAs.Image")));
            this.itemSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemSaveAs.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemSaveAs.MergeIndex = 20;
            this.itemSaveAs.Name = "itemSaveAs";
            this.itemSaveAs.ShortcutKeyDisplayString = "";
            this.itemSaveAs.Size = new System.Drawing.Size(186, 22);
            this.itemSaveAs.Text = "&Sauvegarder sous";
            // 
            // actionSave
            // 
            this.actionSave.Image = ((System.Drawing.Image)(resources.GetObject("actionSave.Image")));
            this.actionSave.Targets.Add(this.itemSave);
            this.actionSave.Targets.Add(this.buttonSave);
            this.actionSave.Text = "&Sauvegarder";
            this.actionSave.Execute += new System.EventHandler(this.actionSave_Execute);
            // 
            // itemSave
            // 
            this.itemSave.Image = ((System.Drawing.Image)(resources.GetObject("itemSave.Image")));
            this.itemSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemSave.MergeIndex = 25;
            this.itemSave.Name = "itemSave";
            this.itemSave.ShortcutKeyDisplayString = "";
            this.itemSave.Size = new System.Drawing.Size(186, 22);
            this.itemSave.Text = "&Sauvegarder";
            // 
            // actionCopy
            // 
            this.actionCopy.Image = ((System.Drawing.Image)(resources.GetObject("actionCopy.Image")));
            this.actionCopy.Targets.Add(this.itemCopy);
            this.actionCopy.Targets.Add(this.buttonCopy);
            this.actionCopy.Text = "&Copier";
            this.actionCopy.Execute += new System.EventHandler(this.actionCopy_Execute);
            // 
            // itemCopy
            // 
            this.itemCopy.Image = ((System.Drawing.Image)(resources.GetObject("itemCopy.Image")));
            this.itemCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemCopy.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemCopy.MergeIndex = 50;
            this.itemCopy.Name = "itemCopy";
            this.itemCopy.ShortcutKeyDisplayString = "";
            this.itemCopy.Size = new System.Drawing.Size(152, 22);
            this.itemCopy.Text = "&Copier";
            // 
            // actionCut
            // 
            this.actionCut.Image = ((System.Drawing.Image)(resources.GetObject("actionCut.Image")));
            this.actionCut.Targets.Add(this.itemCut);
            this.actionCut.Targets.Add(this.buttonCut);
            this.actionCut.Text = "&Couper";
            this.actionCut.Execute += new System.EventHandler(this.actionCut_Execute);
            // 
            // itemCut
            // 
            this.itemCut.Image = ((System.Drawing.Image)(resources.GetObject("itemCut.Image")));
            this.itemCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemCut.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemCut.MergeIndex = 55;
            this.itemCut.Name = "itemCut";
            this.itemCut.ShortcutKeyDisplayString = "";
            this.itemCut.Size = new System.Drawing.Size(152, 22);
            this.itemCut.Text = "&Couper";
            // 
            // actionPaste
            // 
            this.actionPaste.Image = ((System.Drawing.Image)(resources.GetObject("actionPaste.Image")));
            this.actionPaste.Targets.Add(this.itemPaste);
            this.actionPaste.Targets.Add(this.buttonPaste);
            this.actionPaste.Text = "&Coller";
            this.actionPaste.Execute += new System.EventHandler(this.actionPaste_Execute);
            // 
            // itemPaste
            // 
            this.itemPaste.Image = ((System.Drawing.Image)(resources.GetObject("itemPaste.Image")));
            this.itemPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemPaste.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemPaste.MergeIndex = 60;
            this.itemPaste.Name = "itemPaste";
            this.itemPaste.ShortcutKeyDisplayString = "";
            this.itemPaste.Size = new System.Drawing.Size(152, 22);
            this.itemPaste.Text = "&Coller";
            // 
            // actionUndo
            // 
            this.actionUndo.Image = ((System.Drawing.Image)(resources.GetObject("actionUndo.Image")));
            this.actionUndo.Targets.Add(this.itemUndo);
            this.actionUndo.Targets.Add(this.buttonUndo);
            this.actionUndo.Text = "&Défaire";
            this.actionUndo.Execute += new System.EventHandler(this.actionUndo_Execute);
            // 
            // itemUndo
            // 
            this.itemUndo.Image = ((System.Drawing.Image)(resources.GetObject("itemUndo.Image")));
            this.itemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemUndo.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemUndo.MergeIndex = 65;
            this.itemUndo.Name = "itemUndo";
            this.itemUndo.ShortcutKeyDisplayString = "";
            this.itemUndo.Size = new System.Drawing.Size(152, 22);
            this.itemUndo.Text = "&Défaire";
            // 
            // actionSaveAll
            // 
            this.actionSaveAll.Image = ((System.Drawing.Image)(resources.GetObject("actionSaveAll.Image")));
            this.actionSaveAll.Targets.Add(this.sauvegarderToutToolStripMenuItem);
            this.actionSaveAll.Targets.Add(this.buttonSaveAll);
            this.actionSaveAll.Text = "&Sauvegarder tout";
            this.actionSaveAll.Execute += new System.EventHandler(this.actionSaveAll_Execute);
            // 
            // sauvegarderToutToolStripMenuItem
            // 
            this.sauvegarderToutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("sauvegarderToutToolStripMenuItem.Image")));
            this.sauvegarderToutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sauvegarderToutToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.sauvegarderToutToolStripMenuItem.MergeIndex = 30;
            this.sauvegarderToutToolStripMenuItem.Name = "sauvegarderToutToolStripMenuItem";
            this.sauvegarderToutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.sauvegarderToutToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.sauvegarderToutToolStripMenuItem.Text = "&Sauvegarder tout";
            // 
            // actionClose
            // 
            this.actionClose.Image = ((System.Drawing.Image)(resources.GetObject("actionClose.Image")));
            this.actionClose.Targets.Add(this.fermerToolStripMenuItem);
            this.actionClose.Targets.Add(this.buttonClose);
            this.actionClose.Text = "&Fermer vue courante";
            this.actionClose.Execute += new System.EventHandler(this.actionClose_Execute);
            // 
            // fermerToolStripMenuItem
            // 
            this.fermerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fermerToolStripMenuItem.Image")));
            this.fermerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fermerToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.fermerToolStripMenuItem.MergeIndex = 35;
            this.fermerToolStripMenuItem.Name = "fermerToolStripMenuItem";
            this.fermerToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.fermerToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.fermerToolStripMenuItem.Text = "&Fermer vue courante";
            // 
            // actionCloseAll
            // 
            this.actionCloseAll.Image = ((System.Drawing.Image)(resources.GetObject("actionCloseAll.Image")));
            this.actionCloseAll.Targets.Add(this.fermerToutToolStripMenuItem);
            this.actionCloseAll.Targets.Add(this.buttonCloseAll);
            this.actionCloseAll.Text = "&Fermer tout";
            this.actionCloseAll.Execute += new System.EventHandler(this.actionCloseAll_Execute);
            // 
            // fermerToutToolStripMenuItem
            // 
            this.fermerToutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("fermerToutToolStripMenuItem.Image")));
            this.fermerToutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fermerToutToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.fermerToutToolStripMenuItem.MergeIndex = 40;
            this.fermerToutToolStripMenuItem.Name = "fermerToutToolStripMenuItem";
            this.fermerToutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.fermerToutToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.fermerToutToolStripMenuItem.Text = "&Fermer tout";
            // 
            // actionRedo
            // 
            this.actionRedo.Image = ((System.Drawing.Image)(resources.GetObject("actionRedo.Image")));
            this.actionRedo.Targets.Add(this.buttonRedo);
            this.actionRedo.Text = "&Refaire";
            this.actionRedo.Execute += new System.EventHandler(this.actionRedo_Execute);
            // 
            // actionCloseDocument
            // 
            this.actionCloseDocument.Targets.Add(this.fermerDocumentToolStripMenuItem);
            this.actionCloseDocument.Text = "&Fermer document";
            this.actionCloseDocument.ToolTipText = "Fermer toutes les vues du document";
            // 
            // fermerDocumentToolStripMenuItem
            // 
            this.fermerDocumentToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.fermerDocumentToolStripMenuItem.Name = "fermerDocumentToolStripMenuItem";
            this.fermerDocumentToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.fermerDocumentToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.fermerDocumentToolStripMenuItem.Text = "&Fermer document";
            this.fermerDocumentToolStripMenuItem.ToolTipText = "Fermer toutes les vues du document";
            // 
            // itemOpen
            // 
            this.itemOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemOpen.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemOpen.MergeIndex = 15;
            this.itemOpen.Name = "itemOpen";
            this.itemOpen.ShortcutKeyDisplayString = "";
            this.itemOpen.Size = new System.Drawing.Size(186, 22);
            this.itemOpen.Text = "&Ouvrir";
            // 
            // filechooser
            // 
            this.filechooser.FileName = "openFileDialog1";
            // 
            // viewList
            // 
            this.viewList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewList.Location = new System.Drawing.Point(0, 49);
            this.viewList.Name = "viewList";
            this.viewList.SelectedIndex = 0;
            this.viewList.Size = new System.Drawing.Size(626, 468);
            this.viewList.TabIndex = 1;
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemNew,
            this.itemOpen,
            this.itemSaveAs,
            this.itemSave,
            this.sauvegarderToutToolStripMenuItem,
            this.fermerToolStripMenuItem,
            this.fermerDocumentToolStripMenuItem,
            this.fermerToutToolStripMenuItem});
            this.menuFile.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.menuFile.MergeIndex = 1;
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(50, 20);
            this.menuFile.Text = "&Fichier";
            // 
            // itemNew
            // 
            this.itemNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemNew.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemNew.MergeIndex = 10;
            this.itemNew.Name = "itemNew";
            this.itemNew.ShortcutKeyDisplayString = "";
            this.itemNew.Size = new System.Drawing.Size(186, 22);
            this.itemNew.Text = "&Nouveau";
            // 
            // menuEdition
            // 
            this.menuEdition.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCut,
            this.itemCopy,
            this.itemPaste,
            this.itemUndo});
            this.menuEdition.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.menuEdition.MergeIndex = 5;
            this.menuEdition.Name = "menuEdition";
            this.menuEdition.Size = new System.Drawing.Size(51, 20);
            this.menuEdition.Text = "&Edition";
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuEdition});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(626, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            // 
            // events
            // 
            this.events.ApplicationIdle += new System.EventHandler(this.events_ApplicationIdle);
            // 
            // bar
            // 
            this.bar.BackColor = System.Drawing.Color.Silver;
            this.bar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bar.Location = new System.Drawing.Point(0, 516);
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(626, 1);
            this.bar.TabIndex = 3;
            // 
            // DocumentManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bar);
            this.Controls.Add(this.viewList);
            this.Controls.Add(this.toolspanel);
            this.Controls.Add(this.menu);
            this.Name = "DocumentManager";
            this.Size = new System.Drawing.Size(626, 517);
            this.toolspanel.ResumeLayout(false);
            this.toolspanel.PerformLayout();
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Psl.Controls.ToolStripPanelEnh toolspanel;
        private Psl.Actions.ActionList actions;
        private Psl.Actions.Action actionSaveAs;
        private Psl.Actions.Action actionSave;
        private Psl.Actions.Action actionCopy;
        private Psl.Actions.Action actionCut;
        private Psl.Actions.Action actionPaste;
        private Psl.Actions.Action actionUndo;
        private Psl.Actions.Action actionSaveAll;
        private Psl.Actions.Action actionClose;
        private Psl.Actions.Action actionCloseAll;
        private System.Windows.Forms.OpenFileDialog filechooser;
        private System.Windows.Forms.SaveFileDialog filesaver;
        private Psl.Actions.Action actionRedo;
        private System.Windows.Forms.TabControl viewList;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem itemNew;
        private System.Windows.Forms.ToolStripMenuItem itemOpen;
        private System.Windows.Forms.ToolStripMenuItem itemSaveAs;
        private System.Windows.Forms.ToolStripMenuItem itemSave;
        private System.Windows.Forms.ToolStripMenuItem sauvegarderToutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fermerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fermerToutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menuEdition;
        private System.Windows.Forms.ToolStripMenuItem itemCopy;
        private System.Windows.Forms.ToolStripMenuItem itemCut;
        private System.Windows.Forms.ToolStripMenuItem itemPaste;
        private System.Windows.Forms.ToolStripMenuItem itemUndo;
        private System.Windows.Forms.MenuStrip menu;
        private Psl.Applications.ApplicationEvents events;
        private System.Windows.Forms.ToolStrip tools;
        private System.Windows.Forms.ToolStripButton buttonSave;
        private System.Windows.Forms.ToolStripButton buttonSaveAs;
        private System.Windows.Forms.ToolStripButton buttonSaveAll;
        private System.Windows.Forms.ToolStripButton buttonClose;
        private System.Windows.Forms.ToolStripButton buttonCloseAll;
        private System.Windows.Forms.ToolStripButton buttonCopy;
        private System.Windows.Forms.ToolStripButton buttonCut;
        private System.Windows.Forms.ToolStripButton buttonPaste;
        private System.Windows.Forms.ToolStripButton buttonUndo;
        private System.Windows.Forms.ToolStripButton buttonRedo;
        private Psl.Controls.ActiveBar bar;
        private Psl.Actions.Action actionCloseDocument;
        private System.Windows.Forms.ToolStripMenuItem fermerDocumentToolStripMenuItem;
    }
}
