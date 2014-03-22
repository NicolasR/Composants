using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Stl.Tme.Components.Controls;

namespace Stl.Tme.Components.Test
{
    public partial class TestCluster : UserControl
    {
        public TestCluster()
        {
            InitializeComponent();            
            combo = new ComboValidate();
            combo.Parent = this;
            combo.Dock = DockStyle.Top;            
            combo.TextValidate = true;
            combo.TextCancel = true;
            combo.Name = "comboValidateHorsModeConception";

            combo.TextValidating += new EventHandler<TextValidatingEventArgs>(combo_TextValidating);
            combo.TextValidated += new EventHandler<EventArgs>(combo_TextValidated);
            combo.TextCancelling += new EventHandler<CancelEventArgs>(combo_TextCancelling);
            combo.TextCancelled += new EventHandler<EventArgs>(combo_TextCancelled);
        }             

        #region Méthodes de service
        private void trace(String desc)
        {
            ComboValidate cv = ActiveControl as ComboValidate;

            if (cv != null)
                listTrace.Items.Add(ActiveControl.Name + " : " + desc + " - text : " + cv.Text);
        }

        /// <summary>
        /// Méthode de réaffichage centralisé.
        /// </summary>
        private void DoUpdateState()
        {
            Boolean isComboIndex = (ActiveControl is ComboIndex);
            checkBoxSwitchAutoLimit.Enabled = isComboIndex;
            checkBoxSwitchListStyle.Enabled = isComboIndex;
        }

        #endregion

        #region Code relatif aux événements

        private void combo_TextCancelled(object sender, EventArgs e)
        {
            trace("text cancelled");
        }

        private void combo_TextCancelling(object sender, CancelEventArgs e)
        {
            trace("text cancelling");
        }

        private void combo_TextValidated(object sender, EventArgs e)
        {
            trace("text validated");
        }

        private void combo_TextValidating(object sender, TextValidatingEventArgs e)
        {
            trace("text validating");
        }

        private void events_ApplicationIdle(object sender, EventArgs e)
        {
            DoUpdateState();
        }        

        private void actionSwitchValidate_Execute(object sender, EventArgs e)
        {
            combo.TextValidate = actionSwitchValidate.Checked;
            comboValidateConceptionMode.TextValidate = actionSwitchValidate.Checked;
            comboIndex.TextValidate = actionSwitchValidate.Checked;
        }

        private void actionSwitchCancel_Execute(object sender, EventArgs e)
        {
            combo.TextCancel = actionSwitchCancel.Checked;
            comboValidateConceptionMode.TextCancel = actionSwitchCancel.Checked;
            comboIndex.TextCancel = actionSwitchCancel.Checked;
        }      

        private void actionTextEnter_Execute(object sender, EventArgs e)
        {
            combo.TextEnter();
        }    

        private void actionSwitchListStyle_Execute(object sender, EventArgs e)
        {
            comboIndex.Sorted = !actionSwitchListStyle.Checked;
        }

        #endregion
    }
}
