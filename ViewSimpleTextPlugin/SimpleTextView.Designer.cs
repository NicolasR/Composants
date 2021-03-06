using System;
using Stl.Tme.Components.Tools;
namespace Stl.Tme.Components.ViewSimpleTextPlugin
{
    partial class SimpleTextView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SimpleTextView));
            this.tools = new System.Windows.Forms.ToolStrip();
            this.buttonAdd = new System.Windows.Forms.ToolStripButton();
            this.buttonRemove = new System.Windows.Forms.ToolStripButton();
            this.actions = new Psl.Actions.ActionList(this.components);
            this.actionAddControl = new Psl.Actions.Action(this.components);
            this.actionRemoveControl = new Psl.Actions.Action(this.components);
            this.boxTypeControls = new System.Windows.Forms.ComboBox();
            this.instances = new System.Windows.Forms.ListBox();
            this.properties = new System.Windows.Forms.PropertyGrid();
            this.splitter = new System.Windows.Forms.Splitter();
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.activebar = new Psl.Controls.ActiveBar();
            this.tools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
            this.SuspendLayout();
            // 
            // tools
            // 
            this.tools.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAdd,
            this.buttonRemove});
            this.tools.Location = new System.Drawing.Point(0, 0);
            this.tools.Name = "tools";
            this.tools.Size = new System.Drawing.Size(493, 25);
            this.tools.TabIndex = 0;
            this.tools.Text = "toolStrip1";
            // 
            // buttonAdd
            // 
            this.buttonAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAdd.Image = ((System.Drawing.Image)(resources.GetObject("buttonAdd.Image")));
            this.buttonAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(23, 22);
            this.buttonAdd.Text = "Ajouter un contrôle";
            // 
            // buttonRemove
            // 
            this.buttonRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemove.Image = ((System.Drawing.Image)(resources.GetObject("buttonRemove.Image")));
            this.buttonRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(23, 22);
            this.buttonRemove.Text = "Supprimer un contrôle";
            // 
            // actions
            // 
            this.actions.Actions.Add(this.actionAddControl);
            this.actions.Actions.Add(this.actionRemoveControl);
            // 
            // actionAddControl
            // 
            this.actionAddControl.Image = ((System.Drawing.Image)(resources.GetObject("actionAddControl.Image")));
            this.actionAddControl.Targets.Add(this.buttonAdd);
            this.actionAddControl.Text = "Ajouter un contrôle";
            this.actionAddControl.Execute += new System.EventHandler(this.actionAddControl_Execute);
            // 
            // actionRemoveControl
            // 
            this.actionRemoveControl.Image = ((System.Drawing.Image)(resources.GetObject("actionRemoveControl.Image")));
            this.actionRemoveControl.Targets.Add(this.buttonRemove);
            this.actionRemoveControl.Text = "Supprimer un contrôle";
            this.actionRemoveControl.Execute += new System.EventHandler(this.actionRemoveControl_Execute);
            // 
            // boxTypeControls
            // 
            this.boxTypeControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.boxTypeControls.FormattingEnabled = true;
            this.boxTypeControls.Location = new System.Drawing.Point(0, 25);
            this.boxTypeControls.Name = "boxTypeControls";
            this.boxTypeControls.Size = new System.Drawing.Size(493, 21);
            this.boxTypeControls.TabIndex = 2;
            // 
            // instances
            // 
            this.instances.Dock = System.Windows.Forms.DockStyle.Left;
            this.instances.FormattingEnabled = true;
            this.instances.Location = new System.Drawing.Point(0, 46);
            this.instances.Name = "instances";
            this.instances.Size = new System.Drawing.Size(120, 430);
            this.instances.TabIndex = 3;
            this.instances.SelectedValueChanged += new System.EventHandler(this.instances_SelectedValueChanged);
            // 
            // properties
            // 
            this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.properties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.properties.Location = new System.Drawing.Point(120, 46);
            this.properties.Name = "properties";
            this.properties.Size = new System.Drawing.Size(373, 430);
            this.properties.TabIndex = 4;
            this.properties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.properties_PropertyValueChanged);
            // 
            // splitter
            // 
            this.splitter.Location = new System.Drawing.Point(120, 46);
            this.splitter.Name = "splitter";
            this.splitter.Size = new System.Drawing.Size(3, 430);
            this.splitter.TabIndex = 5;
            this.splitter.TabStop = false;
            // 
            // events
            // 
            this.events.ApplicationIdle += new System.EventHandler(this.events_ApplicationIdle);
            // 
            // activebar
            // 
            this.activebar.BackColor = System.Drawing.Color.Silver;
            this.activebar.Dock = System.Windows.Forms.DockStyle.Top;
            this.activebar.Location = new System.Drawing.Point(123, 46);
            this.activebar.Name = "activebar";
            this.activebar.Size = new System.Drawing.Size(370, 3);
            this.activebar.TabIndex = 6;
            // 
            // SimpleTextView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.activebar);
            this.Controls.Add(this.splitter);
            this.Controls.Add(this.properties);
            this.Controls.Add(this.instances);
            this.Controls.Add(this.boxTypeControls);
            this.Controls.Add(this.tools);
            this.Name = "SimpleTextView";
            this.Size = new System.Drawing.Size(493, 476);
            this.tools.ResumeLayout(false);
            this.tools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tools;
        private System.Windows.Forms.ToolStripButton buttonAdd;
        private System.Windows.Forms.ToolStripButton buttonRemove;
        private Psl.Actions.ActionList actions;
        private Psl.Actions.Action actionAddControl;
        private Psl.Actions.Action actionRemoveControl;
        private System.Windows.Forms.ComboBox boxTypeControls;
        private System.Windows.Forms.ListBox instances;
        private System.Windows.Forms.PropertyGrid properties;
        private System.Windows.Forms.Splitter splitter;
        private Psl.Applications.ApplicationEvents events;
        private Psl.Controls.ActiveBar activebar;



    }
}
