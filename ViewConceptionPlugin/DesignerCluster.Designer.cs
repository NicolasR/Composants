namespace Stl.Tme.Components.ViewConceptionPlugin {

    /// <summary>
    /// Classe qui construit l'interface de Conception
    /// </summary>
    partial class DesignerCluster {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerCluster));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.editionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supprimerControleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.supprimerTousLesControlesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.couperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tools = new System.Windows.Forms.ToolStrip();
            this.tSClearComp = new System.Windows.Forms.ToolStripButton();
            this.tSClearAllComp = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tSCut = new System.Windows.Forms.ToolStripButton();
            this.tSCopy = new System.Windows.Forms.ToolStripButton();
            this.tSPast = new System.Windows.Forms.ToolStripButton();
            this.tSSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bar = new Psl.Controls.ActiveBar();
            this.panelLeft = new System.Windows.Forms.Panel();
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.splitterProperties = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.selection = new System.Windows.Forms.ComboBox();
            this.designmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.actions = new Psl.Actions.ActionList(this.components);
            this.acRemoveAll = new Psl.Actions.Action(this.components);
            this.supprimerToutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acRemoveComp = new Psl.Actions.Action(this.components);
            this.supprimerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.acCopyControl = new Psl.Actions.Action(this.components);
            this.copierToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.acCutControl = new Psl.Actions.Action(this.components);
            this.couperToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.acPastControl = new Psl.Actions.Action(this.components);
            this.collerToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.restaurerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.design = new Psl.Controls.DesignFrame();
            this.contextFrame = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.splitterToolbox = new System.Windows.Forms.Splitter();
            this.designToolbox = new Stl.Tme.Components.Controls.DesignToolBox();
            this.menu.SuspendLayout();
            this.tools.SuspendLayout();
            this.panelLeft.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
            this.contextFrame.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editionToolStripMenuItem});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(985, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            this.menu.Visible = false;
            // 
            // editionToolStripMenuItem
            // 
            this.editionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.supprimerControleToolStripMenuItem,
            this.supprimerTousLesControlesToolStripMenuItem,
            this.toolStripSeparator,
            this.couperToolStripMenuItem,
            this.copierToolStripMenuItem,
            this.collerToolStripMenuItem});
            this.editionToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.editionToolStripMenuItem.MergeIndex = 5;
            this.editionToolStripMenuItem.Name = "editionToolStripMenuItem";
            this.editionToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.editionToolStripMenuItem.Text = "&Edition";
            this.editionToolStripMenuItem.Visible = false;
            // 
            // supprimerControleToolStripMenuItem
            // 
            this.supprimerControleToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("supprimerControleToolStripMenuItem.Image")));
            this.supprimerControleToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.supprimerControleToolStripMenuItem.Name = "supprimerControleToolStripMenuItem";
            this.supprimerControleToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.supprimerControleToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.supprimerControleToolStripMenuItem.Text = "Supprimer controle";
            this.supprimerControleToolStripMenuItem.ToolTipText = "Supprimer le composant sélectionné de la frame ";
            // 
            // supprimerTousLesControlesToolStripMenuItem
            // 
            this.supprimerTousLesControlesToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("supprimerTousLesControlesToolStripMenuItem.Image")));
            this.supprimerTousLesControlesToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.supprimerTousLesControlesToolStripMenuItem.Name = "supprimerTousLesControlesToolStripMenuItem";
            this.supprimerTousLesControlesToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.supprimerTousLesControlesToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.supprimerTousLesControlesToolStripMenuItem.Text = "Supprimer tous les controles";
            this.supprimerTousLesControlesToolStripMenuItem.ToolTipText = "Retirer tous les composants de la frame";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(217, 6);
            // 
            // couperToolStripMenuItem
            // 
            this.couperToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("couperToolStripMenuItem.Image")));
            this.couperToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.couperToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.couperToolStripMenuItem.MergeIndex = 10;
            this.couperToolStripMenuItem.Name = "couperToolStripMenuItem";
            this.couperToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.couperToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.couperToolStripMenuItem.Text = "Couper";
            this.couperToolStripMenuItem.ToolTipText = "Coupe le control sélectionné vers le presse papier";
            // 
            // copierToolStripMenuItem
            // 
            this.copierToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copierToolStripMenuItem.Image")));
            this.copierToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copierToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.copierToolStripMenuItem.MergeIndex = 15;
            this.copierToolStripMenuItem.Name = "copierToolStripMenuItem";
            this.copierToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.copierToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.copierToolStripMenuItem.Text = "Copier";
            this.copierToolStripMenuItem.ToolTipText = "Copie le controle dans le presse papier";
            // 
            // collerToolStripMenuItem
            // 
            this.collerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("collerToolStripMenuItem.Image")));
            this.collerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collerToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.collerToolStripMenuItem.MergeIndex = 20;
            this.collerToolStripMenuItem.Name = "collerToolStripMenuItem";
            this.collerToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.collerToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.collerToolStripMenuItem.Text = "Coller";
            this.collerToolStripMenuItem.ToolTipText = "Colle le control du presse papier vers la frame";
            // 
            // tools
            // 
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tSClearComp,
            this.tSClearAllComp,
            this.toolStripSeparator1,
            this.tSCut,
            this.tSCopy,
            this.tSPast,
            this.tSSeparator});
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(985, 25);
            this.tools.TabIndex = 1;
            this.tools.Text = "toolStrip1";
            // 
            // tSClearComp
            // 
            this.tSClearComp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSClearComp.Image = ((System.Drawing.Image)(resources.GetObject("tSClearComp.Image")));
            this.tSClearComp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSClearComp.Name = "tSClearComp";
            this.tSClearComp.Size = new System.Drawing.Size(23, 22);
            this.tSClearComp.Text = "Supprimer controle";
            this.tSClearComp.ToolTipText = "Supprimer le composant sélectionné de la frame ";
            // 
            // tSClearAllComp
            // 
            this.tSClearAllComp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSClearAllComp.Image = ((System.Drawing.Image)(resources.GetObject("tSClearAllComp.Image")));
            this.tSClearAllComp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSClearAllComp.Name = "tSClearAllComp";
            this.tSClearAllComp.Size = new System.Drawing.Size(23, 22);
            this.tSClearAllComp.Text = "Supprimer tous les controles";
            this.tSClearAllComp.ToolTipText = "Retirer tous les composants de la frame";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tSCut
            // 
            this.tSCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSCut.Image = ((System.Drawing.Image)(resources.GetObject("tSCut.Image")));
            this.tSCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSCut.Name = "tSCut";
            this.tSCut.Size = new System.Drawing.Size(23, 22);
            this.tSCut.Text = "Couper";
            this.tSCut.ToolTipText = "Coupe le control sélectionné vers le presse papier";
            // 
            // tSCopy
            // 
            this.tSCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSCopy.Image = ((System.Drawing.Image)(resources.GetObject("tSCopy.Image")));
            this.tSCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSCopy.Name = "tSCopy";
            this.tSCopy.Size = new System.Drawing.Size(23, 22);
            this.tSCopy.Text = "Copier";
            this.tSCopy.ToolTipText = "Copie le controle dans le presse papier";
            // 
            // tSPast
            // 
            this.tSPast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tSPast.Image = ((System.Drawing.Image)(resources.GetObject("tSPast.Image")));
            this.tSPast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tSPast.Name = "tSPast";
            this.tSPast.Size = new System.Drawing.Size(23, 22);
            this.tSPast.Text = "Coller";
            this.tSPast.ToolTipText = "Colle le control du presse papier vers la frame";
            // 
            // tSSeparator
            // 
            this.tSSeparator.Name = "tSSeparator";
            this.tSSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bar
            // 
            this.bar.BackColor = System.Drawing.Color.Silver;
            this.bar.Dock = System.Windows.Forms.DockStyle.Top;
            this.bar.Location = new System.Drawing.Point(0, 25);
            this.bar.Name = "bar";
            this.bar.Size = new System.Drawing.Size(985, 1);
            this.bar.TabIndex = 2;
            // 
            // panelLeft
            // 
            this.panelLeft.Controls.Add(this.designToolbox);
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Location = new System.Drawing.Point(0, 26);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(200, 571);
            this.panelLeft.TabIndex = 3;
            // 
            // properties
            // 
            this.properties.Dock = System.Windows.Forms.DockStyle.Right;
            this.properties.Location = new System.Drawing.Point(804, 26);
            this.properties.Name = "properties";
            this.properties.Size = new System.Drawing.Size(181, 571);
            this.properties.TabIndex = 5;
            this.properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.properties_PropertyValueChanged);
            // 
            // splitterProperties
            // 
            this.splitterProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitterProperties.Location = new System.Drawing.Point(800, 26);
            this.splitterProperties.Name = "splitterProperties";
            this.splitterProperties.Size = new System.Drawing.Size(4, 571);
            this.splitterProperties.TabIndex = 6;
            this.splitterProperties.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.selection);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(203, 26);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(597, 27);
            this.panel1.TabIndex = 7;
            // 
            // selection
            // 
            this.selection.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selection.FormattingEnabled = true;
            this.selection.Location = new System.Drawing.Point(0, 0);
            this.selection.Name = "selection";
            this.selection.Size = new System.Drawing.Size(594, 21);
            this.selection.Sorted = true;
            this.selection.TabIndex = 0;
            this.selection.SelectedValueChanged += new System.EventHandler(this.selection_SelectedValueChanged);
            // 
            // designmenu
            // 
            this.designmenu.Name = "designmenu";
            this.designmenu.Size = new System.Drawing.Size(61, 4);
            // 
            // actions
            // 
            this.actions.Actions.Add(this.acRemoveAll);
            this.actions.Actions.Add(this.acRemoveComp);
            this.actions.Actions.Add(this.acCopyControl);
            this.actions.Actions.Add(this.acCutControl);
            this.actions.Actions.Add(this.acPastControl);
            // 
            // acRemoveAll
            // 
            this.acRemoveAll.Image = ((System.Drawing.Image)(resources.GetObject("acRemoveAll.Image")));
            this.acRemoveAll.Targets.Add(this.tSClearAllComp);
            this.acRemoveAll.Targets.Add(this.supprimerTousLesControlesToolStripMenuItem);
            this.acRemoveAll.Targets.Add(this.supprimerToutToolStripMenuItem);
            this.acRemoveAll.Text = "Supprimer tous les controles";
            this.acRemoveAll.ToolTipText = "Retirer tous les composants de la frame";
            this.acRemoveAll.Execute += new System.EventHandler(this.acRemoveAll_Execute);
            // 
            // supprimerToutToolStripMenuItem
            // 
            this.supprimerToutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("supprimerToutToolStripMenuItem.Image")));
            this.supprimerToutToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.supprimerToutToolStripMenuItem.Name = "supprimerToutToolStripMenuItem";
            this.supprimerToutToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.supprimerToutToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.supprimerToutToolStripMenuItem.Text = "Supprimer tous les controles";
            this.supprimerToutToolStripMenuItem.ToolTipText = "Retirer tous les composants de la frame";
            // 
            // acRemoveComp
            // 
            this.acRemoveComp.Image = ((System.Drawing.Image)(resources.GetObject("acRemoveComp.Image")));
            this.acRemoveComp.Targets.Add(this.tSClearComp);
            this.acRemoveComp.Targets.Add(this.supprimerControleToolStripMenuItem);
            this.acRemoveComp.Targets.Add(this.supprimerToolStripMenuItem);
            this.acRemoveComp.Text = "Supprimer controle";
            this.acRemoveComp.ToolTipText = "Supprimer le composant sélectionné de la frame ";
            this.acRemoveComp.Execute += new System.EventHandler(this.acRemoveComp_Execute);
            // 
            // supprimerToolStripMenuItem
            // 
            this.supprimerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("supprimerToolStripMenuItem.Image")));
            this.supprimerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.supprimerToolStripMenuItem.Name = "supprimerToolStripMenuItem";
            this.supprimerToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.supprimerToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.supprimerToolStripMenuItem.Text = "Supprimer controle";
            this.supprimerToolStripMenuItem.ToolTipText = "Supprimer le composant sélectionné de la frame ";
            // 
            // acCopyControl
            // 
            this.acCopyControl.Image = ((System.Drawing.Image)(resources.GetObject("acCopyControl.Image")));
            this.acCopyControl.Targets.Add(this.tSCopy);
            this.acCopyControl.Targets.Add(this.copierToolStripMenuItem);
            this.acCopyControl.Targets.Add(this.copierToolStripMenuItem2);
            this.acCopyControl.Text = "Copier";
            this.acCopyControl.ToolTipText = "Copie le controle dans le presse papier";
            this.acCopyControl.Execute += new System.EventHandler(this.acCopyControl_Execute);
            // 
            // copierToolStripMenuItem2
            // 
            this.copierToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("copierToolStripMenuItem2.Image")));
            this.copierToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copierToolStripMenuItem2.Name = "copierToolStripMenuItem2";
            this.copierToolStripMenuItem2.ShortcutKeyDisplayString = "";
            this.copierToolStripMenuItem2.Size = new System.Drawing.Size(220, 22);
            this.copierToolStripMenuItem2.Text = "Copier";
            this.copierToolStripMenuItem2.ToolTipText = "Copie le controle dans le presse papier";
            // 
            // acCutControl
            // 
            this.acCutControl.Image = ((System.Drawing.Image)(resources.GetObject("acCutControl.Image")));
            this.acCutControl.Targets.Add(this.tSCut);
            this.acCutControl.Targets.Add(this.couperToolStripMenuItem);
            this.acCutControl.Targets.Add(this.couperToolStripMenuItem2);
            this.acCutControl.Text = "Couper";
            this.acCutControl.ToolTipText = "Coupe le control sélectionné vers le presse papier";
            this.acCutControl.Execute += new System.EventHandler(this.acCutControl_Execute);
            // 
            // couperToolStripMenuItem2
            // 
            this.couperToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("couperToolStripMenuItem2.Image")));
            this.couperToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.couperToolStripMenuItem2.Name = "couperToolStripMenuItem2";
            this.couperToolStripMenuItem2.ShortcutKeyDisplayString = "";
            this.couperToolStripMenuItem2.Size = new System.Drawing.Size(220, 22);
            this.couperToolStripMenuItem2.Text = "Couper";
            this.couperToolStripMenuItem2.ToolTipText = "Coupe le control sélectionné vers le presse papier";
            // 
            // acPastControl
            // 
            this.acPastControl.Image = ((System.Drawing.Image)(resources.GetObject("acPastControl.Image")));
            this.acPastControl.Targets.Add(this.tSPast);
            this.acPastControl.Targets.Add(this.collerToolStripMenuItem);
            this.acPastControl.Targets.Add(this.collerToolStripMenuItem2);
            this.acPastControl.Text = "Coller";
            this.acPastControl.ToolTipText = "Colle le control du presse papier vers la frame";
            this.acPastControl.Execute += new System.EventHandler(this.acPastControl_Execute);
            // 
            // collerToolStripMenuItem2
            // 
            this.collerToolStripMenuItem2.Image = ((System.Drawing.Image)(resources.GetObject("collerToolStripMenuItem2.Image")));
            this.collerToolStripMenuItem2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.collerToolStripMenuItem2.Name = "collerToolStripMenuItem2";
            this.collerToolStripMenuItem2.ShortcutKeyDisplayString = "";
            this.collerToolStripMenuItem2.Size = new System.Drawing.Size(220, 22);
            this.collerToolStripMenuItem2.Text = "Coller";
            this.collerToolStripMenuItem2.ToolTipText = "Colle le control du presse papier vers la frame";
            // 
            // restaurerToolStripMenuItem
            // 
            this.restaurerToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("restaurerToolStripMenuItem.Image")));
            this.restaurerToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restaurerToolStripMenuItem.Name = "restaurerToolStripMenuItem";
            this.restaurerToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.restaurerToolStripMenuItem.Size = new System.Drawing.Size(220, 22);
            this.restaurerToolStripMenuItem.Text = "Restaurer composant(s)";
            this.restaurerToolStripMenuItem.ToolTipText = "Restaurer la dernière suppression";
            // 
            // events
            // 
            this.events.ApplicationIdle += new System.EventHandler(this.events_ApplicationIdle);
            // 
            // design
            // 
            this.design.BackColor = System.Drawing.Color.Azure;
            this.design.CausesValidation = false;
            this.design.ContextMenuStrip = this.contextFrame;
            this.design.Dock = System.Windows.Forms.DockStyle.Fill;
            this.design.Location = new System.Drawing.Point(203, 53);
            this.design.Name = "design";
            this.design.Size = new System.Drawing.Size(597, 544);
            this.design.TabIndex = 8;
            this.design.SelectedControlChanged += new System.EventHandler(this.design_SelectedControlChanged);
            this.design.MouseUp += new System.Windows.Forms.MouseEventHandler(this.design_MouseUp);
            // 
            // contextFrame
            // 
            this.contextFrame.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.supprimerToolStripMenuItem,
            this.supprimerToutToolStripMenuItem,
            this.couperToolStripMenuItem2,
            this.copierToolStripMenuItem2,
            this.collerToolStripMenuItem2,
            this.toolStripSeparator2,
            this.restaurerToolStripMenuItem});
            this.contextFrame.Name = "contextFrame";
            this.contextFrame.Size = new System.Drawing.Size(221, 142);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(217, 6);
            // 
            // splitterToolbox
            // 
            this.splitterToolbox.Location = new System.Drawing.Point(200, 26);
            this.splitterToolbox.Name = "splitterToolbox";
            this.splitterToolbox.Size = new System.Drawing.Size(3, 571);
            this.splitterToolbox.TabIndex = 4;
            this.splitterToolbox.TabStop = false;
            // 
            // designToolbox
            // 
            this.designToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.designToolbox.Location = new System.Drawing.Point(0, 0);
            this.designToolbox.Name = "designToolbox";
            this.designToolbox.Size = new System.Drawing.Size(200, 571);
            this.designToolbox.TabIndex = 0;
            // 
            // DesignerCluster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.design);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitterProperties);
            this.Controls.Add(this.properties);
            this.Controls.Add(this.splitterToolbox);
            this.Controls.Add(this.panelLeft);
            this.Controls.Add(this.bar);
            this.Controls.Add(this.tools);
            this.Controls.Add(this.menu);
            this.Name = "DesignerCluster";
            this.Size = new System.Drawing.Size(985, 597);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            this.panelLeft.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
            this.contextFrame.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStrip tools;
        private Psl.Controls.ActiveBar bar;
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.PropertyGrid properties;
        private System.Windows.Forms.Splitter splitterProperties;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox selection;
        private System.Windows.Forms.ContextMenuStrip designmenu;
        private Psl.Actions.ActionList actions;
        private Psl.Applications.ApplicationEvents events;
        private Psl.Actions.Action acRemoveAll;
        private Psl.Actions.Action acRemoveComp;
        private Psl.Actions.Action acCopyControl;
        private Psl.Actions.Action acCutControl;
        private Psl.Actions.Action acPastControl;
        private Psl.Controls.DesignFrame design;
        private System.Windows.Forms.ToolStripButton tSClearComp;
        private System.Windows.Forms.ToolStripButton tSClearAllComp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tSCut;
        private System.Windows.Forms.ToolStripButton tSCopy;
        private System.Windows.Forms.ToolStripButton tSPast;
        private System.Windows.Forms.ToolStripSeparator tSSeparator;
        private System.Windows.Forms.ToolStripMenuItem editionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem couperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copierToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem collerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supprimerControleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supprimerTousLesControlesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem supprimerToutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem supprimerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copierToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem couperToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem collerToolStripMenuItem2;
        private System.Windows.Forms.ContextMenuStrip contextFrame;
        private Stl.Tme.Components.Controls.DesignToolBox designToolbox;
        private System.Windows.Forms.Splitter splitterToolbox;
        private System.Windows.Forms.ToolStripMenuItem restaurerToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
