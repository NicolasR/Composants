using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Psl.Controls;

namespace Stl.Tme.Components.Controls
{
    /// <summary>
    /// Classe qui construit le DesignToolBox (selection de controle)
    /// </summary>
    public partial class DesignToolBox : UserControl
    {
        #region "Evènements"

        /// <summary>
        /// Evènement qui indique que le controle sélectionné a changé
        /// </summary>
        [Browsable(true)]
        [Description("Evènement indiquant que le controle sélectionné a changé")]
        public event EventHandler SelectedControlChanged;

        /// <summary>
        /// Gestion evènement SelectedIndexChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void listViewComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedControlChanged(e);
        }

        /// <summary>
        /// Gestion evènement OnSelectedControlChanged
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectedControlChanged(EventArgs e)
        {
            if (SelectedControlChanged != null) SelectedControlChanged(this, e);
        }

        /// <summary>
        /// Gestion evènement tout déselectionner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bUnselectAll_Click(object sender, EventArgs e)
        {
            UnSelectAll();
        }

        /// <summary>
        /// Gestion evènement application au repos (ApplicationIdle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void events_ApplicationIdle(object sender, EventArgs e)
        {
            /*if (SelectedControl != null)
                this.Cursor = SelectedControl.Cursor;
            else
                this.Cursor = null;*/

            acCancelSellected.Visible = listViewComp.SelectedItems.Count > 0;
        }
#endregion

        #region "Actions"

        /// <summary>
        /// Action voir détails
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acViewDetails_Execute(object sender, EventArgs e)
        {
            changeView(View.Details);
            checkViewActions(true, false, false);
        }

        /// <summary>
        /// Action Voir icones larges
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acViewLarge_Execute(object sender, EventArgs e)
        {
            changeView(View.LargeIcon);
            checkViewActions(false, false, true);
        }

        /// <summary>
        /// Action voir petites icones
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acViewSmall_Execute(object sender, EventArgs e)
        {
            changeView(View.SmallIcon);
            checkViewActions(false, true, false);
        }

        /// <summary>
        /// Action annuler sélections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acCancelSellected_Execute(object sender, EventArgs e)
        {
            UnSelectAll();
        }
        #endregion

        #region "Constructeur"

        /// <summary>
        /// Constructeur DesignToolBox
        /// </summary>
        public DesignToolBox()
        {
            InitializeComponent();
            listViewComp.SelectedIndexChanged += new EventHandler(listViewComp_SelectedIndexChanged);
            acViewDetails.Checked = true;
        }
        #endregion

        #region "Services"

        /// <summary>
        /// Enregistrement d'un controle
        /// </summary>
        /// <param name="type"></param>
        [Description("Enregistre un controle")]
        public void RegisterControl(Type type)
        {
            if (type == null) throw new ArgumentNullException();
            if (!type.IsSubclassOf(typeof(Control))) throw new ArgumentException();

            Image small, large;
            System.Windows.Forms.Cursor cursor;
            if (!CursorHelper.GetComponentImages(type, out small, out large, out cursor))
                return;

            ControlData newControl = new ControlData(type, cursor);
            ListViewItem newItem = new ListViewItem();
            newItem.Tag = newControl;
            newItem.Text = type.Name;
            newItem.ImageKey = type.ToString();
            listViewComp.Items.Add(newItem);
            SmallImageList.Images.Add(type.ToString(), small);
            LargeImageList.Images.Add(type.ToString(), large);
        }

        /// <summary>
        /// Tout déselectionner
        /// </summary>
        [Description("Tout déselectionner")]
        public void UnSelectAll() 
        { 
            listViewComp.SelectedItems.Clear(); 
        }

        /// <summary>
        /// Changer le type de vue
        /// </summary>
        /// <param name="newview">Nouvelle vue à appliquer: View</param>
        [Description("Changer type de vue")]
        private void changeView(View newview)
        {
            listViewComp.View = newview;
        }

        /// <summary>
        /// Appliquer les nouvelles propriétés de vues
        /// </summary>
        /// <param name="viewDetails">Activation vue détails: boolean</param>
        /// <param name="viewSmall">Activation vue petites icones: boolean</param>
        /// <param name="viewLarge">Activation vue grandes icones: boolean</param>
        [Description("Appliquer nouvelle propriétés de vues")]
        private void checkViewActions(bool viewDetails, bool viewSmall, bool viewLarge)
        {
            acViewDetails.Checked = viewDetails;
            acViewLarge.Checked = viewLarge;
            acViewSmall.Checked = viewSmall;
        }
        #endregion

        #region "Propriétés"

        /// <summary>
        /// Classe qui contient les données du controle
        /// </summary>
        [Browsable(true)]
        [Description("Données du controle")]
        public class ControlData
        {
            /// <summary>
            /// Données du controle
            /// </summary>
            /// <param name="type">type du controle: Type</param>
            /// <param name="cursor">Curseur du controle: Cursor</param>
            [Browsable(true)]
            [Description("Données du controle")]
            public ControlData(Type type, Cursor cursor) { this.Type = type; this.Cursor = cursor; }

            /// <summary>
            /// Accesseur Type du controle
            /// </summary>
            [Description("Accesseur type du controle")]
            public Type Type { get; private set; }

            /// <summary>
            /// Accesseur Curseur du controle
            /// </summary>
            [Description("Accesseur Curseur du controle")]
            public Cursor Cursor { get; private set; }
        }   

        /// <summary>
        /// Données du controle sélectionné
        /// </summary>
        [Browsable(true)]
        [Description("Données du controle sélectionné")]
        public ControlData SelectedControl
        {
            get
            {
                if (listViewComp.SelectedIndices.Count == 0)
                    return null;
                return (ControlData)listViewComp.SelectedItems[0].Tag;
            }
        }
        #endregion
        
    }

    
}
