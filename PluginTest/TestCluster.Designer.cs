using Stl.Tme.Components.Controls;
namespace Stl.Tme.Components.Test
{
    partial class TestCluster
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
            this.events = new Psl.Applications.ApplicationEvents(this.components);
            this.actions = new Psl.Actions.ActionList(this.components);
            this.actionSwitchValidate = new Psl.Actions.Action(this.components);
            this.checkValidate = new System.Windows.Forms.CheckBox();
            this.actionSwitchCancel = new Psl.Actions.Action(this.components);
            this.checkCancel = new System.Windows.Forms.CheckBox();
            this.actionTextEnter = new Psl.Actions.Action(this.components);
            this.buttonTextEnter = new System.Windows.Forms.Button();
            this.actionSwitchListStyle = new Psl.Actions.Action(this.components);
            this.checkBoxSwitchListStyle = new System.Windows.Forms.CheckBox();
            this.actionSwitchAutoLimit = new Psl.Actions.Action(this.components);
            this.checkBoxSwitchAutoLimit = new System.Windows.Forms.CheckBox();
            this.listTrace = new System.Windows.Forms.ListBox();
            this.labelComboValidateConception = new System.Windows.Forms.Label();
            this.labelComboIndex = new System.Windows.Forms.Label();
            this.comboIndex = new Stl.Tme.Components.Controls.ComboIndex();
            this.comboValidateConceptionMode = new Stl.Tme.Components.Controls.ComboValidate();
            ((System.ComponentModel.ISupportInitialize)(this.actions)).BeginInit();
            this.SuspendLayout();
            // 
            // events
            // 
            this.events.ApplicationIdle += new System.EventHandler(this.events_ApplicationIdle);
            // 
            // actions
            // 
            this.actions.Actions.Add(this.actionSwitchValidate);
            this.actions.Actions.Add(this.actionSwitchCancel);
            this.actions.Actions.Add(this.actionTextEnter);
            this.actions.Actions.Add(this.actionSwitchListStyle);
            this.actions.Actions.Add(this.actionSwitchAutoLimit);
            // 
            // actionSwitchValidate
            // 
            this.actionSwitchValidate.CheckOnClick = true;
            this.actionSwitchValidate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.actionSwitchValidate.Targets.Add(this.checkValidate);
            this.actionSwitchValidate.Text = "Validation";
            this.actionSwitchValidate.ToolTipText = "Activation de la validation ou non du texte dernièrement saisi dans une des combo" +
                "box";
            this.actionSwitchValidate.Execute += new System.EventHandler(this.actionSwitchValidate_Execute);
            // 
            // checkValidate
            // 
            this.checkValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkValidate.AutoSize = true;
            this.checkValidate.Checked = true;
            this.checkValidate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkValidate.Location = new System.Drawing.Point(6, 364);
            this.checkValidate.Name = "checkValidate";
            this.checkValidate.Size = new System.Drawing.Size(72, 17);
            this.checkValidate.TabIndex = 1;
            this.checkValidate.Text = "Validation";
            this.checkValidate.UseVisualStyleBackColor = true;
            // 
            // actionSwitchCancel
            // 
            this.actionSwitchCancel.CheckOnClick = true;
            this.actionSwitchCancel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.actionSwitchCancel.Targets.Add(this.checkCancel);
            this.actionSwitchCancel.Text = "Annulation";
            this.actionSwitchCancel.ToolTipText = "Activation de l\'annulation ou non du texte dernièrement saisi dans une des combob" +
                "ox";
            this.actionSwitchCancel.Execute += new System.EventHandler(this.actionSwitchCancel_Execute);
            // 
            // checkCancel
            // 
            this.checkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkCancel.AutoSize = true;
            this.checkCancel.Checked = true;
            this.checkCancel.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkCancel.Location = new System.Drawing.Point(6, 390);
            this.checkCancel.Name = "checkCancel";
            this.checkCancel.Size = new System.Drawing.Size(76, 17);
            this.checkCancel.TabIndex = 2;
            this.checkCancel.Text = "Annulation";
            this.checkCancel.UseVisualStyleBackColor = true;
            // 
            // actionTextEnter
            // 
            this.actionTextEnter.Targets.Add(this.buttonTextEnter);
            this.actionTextEnter.Text = "textEnter";
            this.actionTextEnter.ToolTipText = "Active la validation du texte par code";
            this.actionTextEnter.Execute += new System.EventHandler(this.actionTextEnter_Execute);
            // 
            // buttonTextEnter
            // 
            this.buttonTextEnter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonTextEnter.Location = new System.Drawing.Point(376, 413);
            this.buttonTextEnter.Name = "buttonTextEnter";
            this.buttonTextEnter.Size = new System.Drawing.Size(113, 23);
            this.buttonTextEnter.TabIndex = 7;
            this.buttonTextEnter.Text = "textEnter";
            this.buttonTextEnter.UseVisualStyleBackColor = true;
            // 
            // actionSwitchListStyle
            // 
            this.actionSwitchListStyle.CheckOnClick = true;
            this.actionSwitchListStyle.Targets.Add(this.checkBoxSwitchListStyle);
            this.actionSwitchListStyle.Text = "LRU Management";
            this.actionSwitchListStyle.ToolTipText = "Change le style de gestion de la liste (LRU ou Trié)";
            this.actionSwitchListStyle.Execute += new System.EventHandler(this.actionSwitchListStyle_Execute);
            // 
            // checkBoxSwitchListStyle
            // 
            this.checkBoxSwitchListStyle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSwitchListStyle.AutoSize = true;
            this.checkBoxSwitchListStyle.Location = new System.Drawing.Point(376, 390);
            this.checkBoxSwitchListStyle.Name = "checkBoxSwitchListStyle";
            this.checkBoxSwitchListStyle.Size = new System.Drawing.Size(113, 17);
            this.checkBoxSwitchListStyle.TabIndex = 8;
            this.checkBoxSwitchListStyle.Text = "LRU Management";
            this.checkBoxSwitchListStyle.UseVisualStyleBackColor = true;
            // 
            // actionSwitchAutoLimit
            // 
            this.actionSwitchAutoLimit.CheckOnClick = true;
            this.actionSwitchAutoLimit.Targets.Add(this.checkBoxSwitchAutoLimit);
            this.actionSwitchAutoLimit.Text = "AutoLimit";
            this.actionSwitchAutoLimit.ToolTipText = "Active ou désactive la limite d\'entrée dans la combo index";
            // 
            // checkBoxSwitchAutoLimit
            // 
            this.checkBoxSwitchAutoLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxSwitchAutoLimit.AutoSize = true;
            this.checkBoxSwitchAutoLimit.Location = new System.Drawing.Point(376, 364);
            this.checkBoxSwitchAutoLimit.Name = "checkBoxSwitchAutoLimit";
            this.checkBoxSwitchAutoLimit.Size = new System.Drawing.Size(69, 17);
            this.checkBoxSwitchAutoLimit.TabIndex = 9;
            this.checkBoxSwitchAutoLimit.Text = "AutoLimit";
            this.checkBoxSwitchAutoLimit.UseVisualStyleBackColor = true;
            // 
            // listTrace
            // 
            this.listTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listTrace.FormattingEnabled = true;
            this.listTrace.Location = new System.Drawing.Point(3, 94);
            this.listTrace.Name = "listTrace";
            this.listTrace.Size = new System.Drawing.Size(486, 264);
            this.listTrace.TabIndex = 0;
            // 
            // labelComboValidateConception
            // 
            this.labelComboValidateConception.AutoSize = true;
            this.labelComboValidateConception.Location = new System.Drawing.Point(3, 50);
            this.labelComboValidateConception.Name = "labelComboValidateConception";
            this.labelComboValidateConception.Size = new System.Drawing.Size(44, 13);
            this.labelComboValidateConception.TabIndex = 5;
            this.labelComboValidateConception.Text = "validate";
            // 
            // labelComboIndex
            // 
            this.labelComboIndex.AutoSize = true;
            this.labelComboIndex.Location = new System.Drawing.Point(3, 70);
            this.labelComboIndex.Name = "labelComboIndex";
            this.labelComboIndex.Size = new System.Drawing.Size(32, 13);
            this.labelComboIndex.TabIndex = 6;
            this.labelComboIndex.Text = "index";
            // 
            // comboIndex
            // 
            this.comboIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboIndex.FormattingEnabled = true;
            this.comboIndex.ListLimit = 0;
            this.comboIndex.Location = new System.Drawing.Point(53, 67);
            this.comboIndex.Name = "comboIndex";
            this.comboIndex.Size = new System.Drawing.Size(439, 21);
            this.comboIndex.Sorted = true;
            this.comboIndex.TabIndex = 4;
            this.comboIndex.TextCancel = true;
            this.comboIndex.TextValidate = true;
            this.comboIndex.TextValidating += new System.EventHandler<Stl.Tme.Components.Controls.TextValidatingEventArgs>(this.combo_TextValidating);
            this.comboIndex.TextCancelling += new System.EventHandler<System.ComponentModel.CancelEventArgs>(this.combo_TextCancelling);
            this.comboIndex.TextValidated += new System.EventHandler<System.EventArgs>(this.combo_TextValidated);
            this.comboIndex.TextCancelled += new System.EventHandler<System.EventArgs>(this.combo_TextCancelled);
            // 
            // comboValidateConceptionMode
            // 
            this.comboValidateConceptionMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboValidateConceptionMode.FormattingEnabled = true;
            this.comboValidateConceptionMode.Location = new System.Drawing.Point(53, 47);
            this.comboValidateConceptionMode.Name = "comboValidateConceptionMode";
            this.comboValidateConceptionMode.Size = new System.Drawing.Size(439, 21);
            this.comboValidateConceptionMode.TabIndex = 3;
            this.comboValidateConceptionMode.TextCancel = true;
            this.comboValidateConceptionMode.TextValidate = true;
            this.comboValidateConceptionMode.TextValidating += new System.EventHandler<Stl.Tme.Components.Controls.TextValidatingEventArgs>(this.combo_TextValidating);
            this.comboValidateConceptionMode.TextCancelling += new System.EventHandler<System.ComponentModel.CancelEventArgs>(this.combo_TextCancelling);
            this.comboValidateConceptionMode.TextValidated += new System.EventHandler<System.EventArgs>(this.combo_TextValidated);
            this.comboValidateConceptionMode.TextCancelled += new System.EventHandler<System.EventArgs>(this.combo_TextCancelled);
            // 
            // TestCluster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxSwitchAutoLimit);
            this.Controls.Add(this.checkBoxSwitchListStyle);
            this.Controls.Add(this.buttonTextEnter);
            this.Controls.Add(this.labelComboIndex);
            this.Controls.Add(this.labelComboValidateConception);
            this.Controls.Add(this.comboIndex);
            this.Controls.Add(this.comboValidateConceptionMode);
            this.Controls.Add(this.checkCancel);
            this.Controls.Add(this.checkValidate);
            this.Controls.Add(this.listTrace);
            this.Name = "TestCluster";
            this.Size = new System.Drawing.Size(492, 439);
            ((System.ComponentModel.ISupportInitialize)(this.actions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Psl.Applications.ApplicationEvents events;
        private ComboValidate combo;
        private Psl.Actions.ActionList actions;
        private Psl.Actions.Action actionSwitchValidate;
        private Psl.Actions.Action actionSwitchCancel;
        private System.Windows.Forms.ListBox listTrace;
        private System.Windows.Forms.CheckBox checkValidate;
        private System.Windows.Forms.CheckBox checkCancel;
        private ComboValidate comboValidateConceptionMode;
        private ComboIndex comboIndex;
        private System.Windows.Forms.Label labelComboValidateConception;
        private System.Windows.Forms.Label labelComboIndex;
        private Psl.Actions.Action actionTextEnter;
        private System.Windows.Forms.Button buttonTextEnter;
        private Psl.Actions.Action actionSwitchListStyle;
        private System.Windows.Forms.CheckBox checkBoxSwitchListStyle;
        private Psl.Actions.Action actionSwitchAutoLimit;
        private System.Windows.Forms.CheckBox checkBoxSwitchAutoLimit; 
    }
}
