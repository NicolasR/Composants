namespace Stl.Tme.Components.Controls
{
    partial class DesignToolBox
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
            this.listViewComp = new System.Windows.Forms.ListView();
            this.Composant = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.annulerSélectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.typeDeVueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.détailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grandesIconesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.petitesIconesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LargeImageList = new System.Windows.Forms.ImageList(this.components);
            this.SmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.bUnselectAll = new System.Windows.Forms.Button();
            this.aLListview = new Psl.Actions.ActionList(this.components);
            this.acViewDetails = new Psl.Actions.Action(this.components);
            this.acViewLarge = new Psl.Actions.Action(this.components);
            this.acViewSmall = new Psl.Actions.Action(this.components);
            this.acCancelSellected = new Psl.Actions.Action(this.components);
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.contextMenuListView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.aLListview)).BeginInit();
            this.SuspendLayout();
            // 
            // listViewComp
            // 
            this.listViewComp.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Composant});
            this.listViewComp.ContextMenuStrip = this.contextMenuListView;
            this.listViewComp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewComp.LargeImageList = this.LargeImageList;
            this.listViewComp.Location = new System.Drawing.Point(0, 23);
            this.listViewComp.Name = "listViewComp";
            this.listViewComp.Size = new System.Drawing.Size(208, 172);
            this.listViewComp.SmallImageList = this.SmallImageList;
            this.listViewComp.TabIndex = 0;
            this.listViewComp.UseCompatibleStateImageBehavior = false;
            this.listViewComp.View = System.Windows.Forms.View.Details;
            // 
            // Composant
            // 
            this.Composant.Text = "Composant";
            this.Composant.Width = 100;
            // 
            // contextMenuListView
            // 
            this.contextMenuListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.annulerSélectionsToolStripMenuItem,
            this.typeDeVueToolStripMenuItem});
            this.contextMenuListView.Name = "contextMenuListView";
            this.contextMenuListView.Size = new System.Drawing.Size(173, 48);
            // 
            // annulerSélectionsToolStripMenuItem
            // 
            this.annulerSélectionsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.annulerSélectionsToolStripMenuItem.Name = "annulerSélectionsToolStripMenuItem";
            this.annulerSélectionsToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.annulerSélectionsToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.annulerSélectionsToolStripMenuItem.Text = "Annuler sélections";
            // 
            // typeDeVueToolStripMenuItem
            // 
            this.typeDeVueToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.détailsToolStripMenuItem,
            this.grandesIconesToolStripMenuItem,
            this.petitesIconesToolStripMenuItem});
            this.typeDeVueToolStripMenuItem.Name = "typeDeVueToolStripMenuItem";
            this.typeDeVueToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.typeDeVueToolStripMenuItem.Text = "Type de vue";
            // 
            // détailsToolStripMenuItem
            // 
            this.détailsToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.détailsToolStripMenuItem.Name = "détailsToolStripMenuItem";
            this.détailsToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.détailsToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.détailsToolStripMenuItem.Text = "Détails";
            // 
            // grandesIconesToolStripMenuItem
            // 
            this.grandesIconesToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.grandesIconesToolStripMenuItem.Name = "grandesIconesToolStripMenuItem";
            this.grandesIconesToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.grandesIconesToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.grandesIconesToolStripMenuItem.Text = "Grandes Icones";
            // 
            // petitesIconesToolStripMenuItem
            // 
            this.petitesIconesToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.petitesIconesToolStripMenuItem.Name = "petitesIconesToolStripMenuItem";
            this.petitesIconesToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.petitesIconesToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.petitesIconesToolStripMenuItem.Text = "Petites Icones";
            // 
            // LargeImageList
            // 
            this.LargeImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.LargeImageList.ImageSize = new System.Drawing.Size(32, 32);
            this.LargeImageList.TransparentColor = System.Drawing.Color.Magenta;
            // 
            // SmallImageList
            // 
            this.SmallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.SmallImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.SmallImageList.TransparentColor = System.Drawing.Color.Magenta;
            // 
            // bUnselectAll
            // 
            this.bUnselectAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.bUnselectAll.Location = new System.Drawing.Point(0, 0);
            this.bUnselectAll.Name = "bUnselectAll";
            this.bUnselectAll.Size = new System.Drawing.Size(208, 23);
            this.bUnselectAll.TabIndex = 1;
            this.bUnselectAll.Text = "Tout Déselectionner";
            this.bUnselectAll.UseVisualStyleBackColor = true;
            this.bUnselectAll.Click += new System.EventHandler(this.bUnselectAll_Click);
            // 
            // aLListview
            // 
            this.aLListview.Actions.Add(this.acViewDetails);
            this.aLListview.Actions.Add(this.acViewLarge);
            this.aLListview.Actions.Add(this.acViewSmall);
            this.aLListview.Actions.Add(this.acCancelSellected);
            // 
            // acViewDetails
            // 
            this.acViewDetails.Targets.Add(this.détailsToolStripMenuItem);
            this.acViewDetails.Text = "Détails";
            this.acViewDetails.Execute += new System.EventHandler(this.acViewDetails_Execute);
            // 
            // acViewLarge
            // 
            this.acViewLarge.Targets.Add(this.grandesIconesToolStripMenuItem);
            this.acViewLarge.Text = "Grandes Icones";
            this.acViewLarge.Execute += new System.EventHandler(this.acViewLarge_Execute);
            // 
            // acViewSmall
            // 
            this.acViewSmall.Targets.Add(this.petitesIconesToolStripMenuItem);
            this.acViewSmall.Text = "Petites Icones";
            this.acViewSmall.Execute += new System.EventHandler(this.acViewSmall_Execute);
            // 
            // acCancelSellected
            // 
            this.acCancelSellected.Targets.Add(this.annulerSélectionsToolStripMenuItem);
            this.acCancelSellected.Text = "Annuler sélections";
            this.acCancelSellected.Visible = false;
            this.acCancelSellected.Execute += new System.EventHandler(this.acCancelSellected_Execute);
            // 
            // events
            // 
            this.events.ApplicationIdle += new System.EventHandler(this.events_ApplicationIdle);
            // 
            // DesignToolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewComp);
            this.Controls.Add(this.bUnselectAll);
            this.Name = "DesignToolbox";
            this.Size = new System.Drawing.Size(208, 195);
            this.contextMenuListView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.aLListview)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewComp;
        private System.Windows.Forms.ColumnHeader Composant;
        private System.Windows.Forms.ImageList LargeImageList;
        private System.Windows.Forms.ImageList SmallImageList;
        private System.Windows.Forms.Button bUnselectAll;
        private Psl.Actions.ActionList aLListview;
        private Psl.Actions.Action acViewDetails;
        private Psl.Actions.Action acViewLarge;
        private Psl.Actions.Action acViewSmall;
        private System.Windows.Forms.ToolStripMenuItem détailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem grandesIconesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem petitesIconesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuListView;
        private System.Windows.Forms.ToolStripMenuItem typeDeVueToolStripMenuItem;
        private Psl.Actions.Action acCancelSellected;
        private System.Windows.Forms.ToolStripMenuItem annulerSélectionsToolStripMenuItem;
        private Psl.Applications.ApplicationEvents events;
    }
}
