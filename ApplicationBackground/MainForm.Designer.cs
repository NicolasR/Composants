namespace Stl.Tme.Components.Demo
{
    partial class MainForm
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

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menu = new System.Windows.Forms.MenuStrip();
            this.menuItemFile = new System.Windows.Forms.ToolStripMenuItem();
            this.itemQuit = new System.Windows.Forms.ToolStripMenuItem();
            this.status = new Psl.Controls.StatusReporter();
            this.actions = new Psl.Actions.ActionList(this.components);
            this.actionQuit = new Psl.Actions.Action(this.components);
            this.tabdock = new Psl.Controls.TabDocker();
            this.tools = new Psl.Controls.ToolStripPanelEnh();
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.menu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
            this.SuspendLayout();
            // 
            // menu
            // 
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemFile});
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(584, 24);
            this.menu.TabIndex = 0;
            this.menu.Text = "menuStrip1";
            // 
            // menuItemFile
            // 
            this.menuItemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemQuit});
            this.menuItemFile.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.menuItemFile.MergeIndex = 1;
            this.menuItemFile.Name = "menuItemFile";
            this.menuItemFile.Size = new System.Drawing.Size(50, 20);
            this.menuItemFile.Text = "&Fichier";
            // 
            // itemQuit
            // 
            this.itemQuit.Image = ((System.Drawing.Image)(resources.GetObject("itemQuit.Image")));
            this.itemQuit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.itemQuit.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.itemQuit.MergeIndex = 9999;
            this.itemQuit.Name = "itemQuit";
            this.itemQuit.ShortcutKeyDisplayString = "";
            this.itemQuit.Size = new System.Drawing.Size(119, 22);
            this.itemQuit.Text = "Quitter";
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(0, 337);
            this.status.MinimumSize = new System.Drawing.Size(0, 25);
            this.status.Name = "status";
            this.status.ShowItemToolTips = true;
            this.status.Size = new System.Drawing.Size(584, 25);
            // 
            // 
            // 
            this.status.StatusInfos.Size = new System.Drawing.Size(429, 20);
            this.status.StatusInfos.Spring = true;
            this.status.StatusInfos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // 
            // 
            this.status.StatusLeft.AutoSize = false;
            this.status.StatusLeft.Size = new System.Drawing.Size(70, 20);
            // 
            // 
            // 
            this.status.StatusMiddle.AutoSize = false;
            this.status.StatusMiddle.Size = new System.Drawing.Size(70, 20);
            // 
            // 
            // 
            this.status.StatusProgress.AutoSize = false;
            this.status.StatusProgress.Margin = new System.Windows.Forms.Padding(4, 6, 1, 6);
            this.status.StatusProgress.Size = new System.Drawing.Size(80, 12);
            this.status.StatusProgress.Visible = false;
            // 
            // 
            // 
            this.status.StatusRight.Size = new System.Drawing.Size(4, 4);
            this.status.StatusRight.Visible = false;
            this.status.TabIndex = 1;
            this.status.Text = "statusReporter1";
            this.status.ZDisplayed.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status.StatusLeft,
            this.status.StatusMiddle,
            this.status.StatusRight,
            this.status.StatusInfos,
            this.status.StatusProgress});
            // 
            // actions
            // 
            this.actions.Actions.Add(this.actionQuit);
            // 
            // actionQuit
            // 
            this.actionQuit.Image = ((System.Drawing.Image)(resources.GetObject("actionQuit.Image")));
            this.actionQuit.Targets.Add(this.itemQuit);
            this.actionQuit.Text = "Quitter";
            this.actionQuit.Execute += new System.EventHandler(this.actionQuit_Execute);
            // 
            // tabdock
            // 
            this.tabdock.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabdock.AutoRightSelect = false;
            this.tabdock.ItemSize = new System.Drawing.Size(50, 21);
            this.tabdock.Location = new System.Drawing.Point(0, 75);
            this.tabdock.Name = "tabdock";
            this.tabdock.Padding = new System.Drawing.Point(4, 4);
            this.tabdock.ShowToolTips = true;
            this.tabdock.Size = new System.Drawing.Size(584, 262);
            this.tabdock.TabIndex = 2;
            // 
            // tools
            // 
            this.tools.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tools.Location = new System.Drawing.Point(0, 24);
            this.tools.Name = "tools";
            this.tools.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.tools.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tools.Size = new System.Drawing.Size(584, 48);
            // 
            // events
            // 
            this.events.Archive += new Psl.Applications.ArchiverEventHandler(this.applicationEvents_Archive);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 362);
            this.Controls.Add(this.tabdock);
            this.Controls.Add(this.tools);
            this.Controls.Add(this.status);
            this.Controls.Add(this.menu);
            this.MainMenuStrip = this.menu;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "MainForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem menuItemFile;
        private System.Windows.Forms.ToolStripMenuItem itemQuit;
        
        private Psl.Controls.StatusReporter status;
        private Psl.Actions.ActionList actions;
        private Psl.Controls.TabDocker tabdock;
        private Psl.Controls.ToolStripPanelEnh tools;
        private Psl.Actions.Action actionQuit;
        private Psl.Applications.ApplicationEvents events;
    }
}

