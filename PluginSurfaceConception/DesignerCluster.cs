using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Psl.Applications;
using System.Reflection;
using Stl.Tme.Components.Designer;
using Psl.Controls;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using Stl.Tme.Components.Controls;

namespace Stl.Tme.Components.Designer {


    public partial class DesignerCluster : UserControl
    {
        #region "Attributs"
        /// <summary>
        /// Référence sur la bar de statut
        /// </summary>
        private IStatusService status;

        /// <summary>
        /// Indique si la surface de conception a changée
        /// </summary>
        private Boolean isChanged;

        /// <summary>
        /// Fichier qui a été sauvegardée au moins une fois
        /// </summary>
        private String saveFile;

        /// <summary>
        /// Corbeille où sont stockés les composants supprimés
        /// </summary>
        private DesignThrash thrash;

        #endregion

        #region "Evènements"

        /// <summary>
        /// Gestion evènement lorsque le clique souris est relaché
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_MouseUp(object sender, MouseEventArgs e)
        {
            if (designToolbox.SelectedControl == null)
                return;
            Type componentType = designToolbox.SelectedControl.Type;
            if (!componentType.IsSubclassOf(typeof(Control))) throw new ArgumentException();
            ConstructorInfo CInfo = componentType.GetConstructor(Type.EmptyTypes);
            Control component = (Control)CInfo.Invoke(Type.EmptyTypes);
            component.Location = e.Location;
            int number = 1;
            String name = componentType.Name;
            while (design.Controls.ContainsKey(name + number))
                number++;

            component.Name = name + number;
            addControl(component);
            designToolbox.UnSelectAll();
            status.TextInfos = "Ajout du composant " + component.Name;
        }

        /// <summary>
        /// Gestion evènement Application au repos (Application idle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void events_ApplicationIdle(object sender, EventArgs e)
        {

            bool active = bar.IsActive;
            if (!active) tools.Visible = false;
            actions.Visible = active;
            //actions.Enabled = false;
            editionToolStripMenuItem.Visible = active;
            if (!active)
                return;

            acRestore.Enabled = !thrash.IsEmpty();
            acRemoveAll.Enabled = design.Controls.Count > 0;
            acRemoveComp.Enabled = design.SelectedControl != null;
            acCopyControl.Enabled = design.SelectedControl != null;
            acCutControl.Enabled = design.SelectedControl != null;
            acPastControl.Enabled = Clipboard.ContainsData(typeof(ControlWrapper).FullName);
            tools.Visible = true;

            if (designToolbox.SelectedControl == null)
                this.Cursor = null;
            else
                this.Cursor = designToolbox.SelectedControl.Cursor;
        }

        /// <summary>
        /// Gestion evènement Sauvegarde de Fichier OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            SaveToFile(saveFileDialog.FileName);
        }

        /// <summary>
        /// Gestion evènement Ouverture de Fichier OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            Control[] controllist = DesignSerializer.FileToControls(this.openFileDialog.FileName);
            foreach (Control control in controllist)
            {
                design.Controls.Add(control);
            }
            status.TextInfos = "Ouverture " + openFileDialog.FileName + " OK";
        }
        /// <summary>
        /// Gestion evènement Controle ajouté
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_ControlAdded(object sender, ControlEventArgs e)
        {
            this.isChanged = true;
        }

        /// <summary>
        /// Gestion evènement Controle supprimé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.isChanged = true;
        }

        /// <summary>
        /// Gestion de l'évènement Fin de déplacement controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_SelectedControlEndDrag(object sender, EventArgs e)
        {
            this.isChanged = true;
        }

        /// <summary>
        /// Gestion evènement Changement de controle sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_SelectedControlChanged(object sender, EventArgs e)
        {
            if (design.SelectedControl == null)
                return;
            selection.Text = design.SelectedControl.Name;
            properties.SelectedObject = design.SelectedControl;
        }

        /// <summary>
        /// Gestion evènement Valeur de propriété changée
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.isChanged = true;
        }

        /// <summary>
        /// Gestion de l'évènement SelectedValueChanged de la combobox de sélection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selection_SelectedValueChanged(object sender, EventArgs e)
        {
            design.SelectedControl = design.Controls[selection.Text];
            properties.SelectedObject = design.SelectedControl;
        }

        /// <summary>
        /// Gestion de l'évènement ApplicationClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void events_ApplicationClosing(object sender, FormClosingEventArgs e)
        {
            checkForSave();
        }
        #endregion

        #region "Actions"

        /// <summary>
        /// Ouverture d'un fichier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acOpenFile_Execute(object sender, EventArgs e)
        {
            checkForSave();
            this.openFileDialog.FileName = "";
            this.openFileDialog.ShowDialog();
        }
        
        /// <summary>
        /// Sauvegarde d'un fichier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acSaveFile_Execute(object sender, EventArgs e)
        {
            if (this.saveFile == null)
                ShowSaveToFile();
            else
                SaveToFile(this.saveFile);
        }

        /// <summary>
        /// Supprimer tous les controles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acRemoveAll_Execute(object sender, EventArgs e)
        {
            thrash.PushControls(design.Controls);
            removeAllControl();
            status.TextInfos = "Suppression de tous les composants";
        }

        /// <summary>
        /// Supprimer un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acRemoveComp_Execute(object sender, EventArgs e)
        {
            String name = design.SelectedControl.Name;
            thrash.PushControl(design.SelectedControl);
            removeControl(design.SelectedControl);
            status.TextInfos = "Suppression du composant " + name;
        }

        /// <summary>
        /// Copier un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acCopyControl_Execute(object sender, EventArgs e)
        {
            copyControl();
        }

        /// <summary>
        /// Couper un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acCutControl_Execute(object sender, EventArgs e)
        {
            copyControl();
            String name = design.SelectedControl.Name;
            design.Controls.Remove(design.SelectedControl);
            selection.Items.Remove(name);
            selection.Text = "";
            status.TextInfos = name + " coupé vers le presse papier";
        }

        /// <summary>
        /// Coller un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acPastControl_Execute(object sender, EventArgs e)
        {
            pastControl();
            status.TextInfos = "Nouvel élement collé depuis le presse papier";
        }

        /// <summary>
        /// Afficher fenêtre de sauvegarde de fichier
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acSaveFileAs_Execute(object sender, EventArgs e)
        {
            ShowSaveToFile();
        }

        /// <summary>
        /// Restauration du dernier élément supprimé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acRestore_Execute(object sender, EventArgs e)
        {
            if (!thrash.IsFirstOnlyOneControle())
            {
                Control[] controleslist = thrash.PopControls();
                foreach (Control controle in controleslist)
                {
                    controle.Name = findUniqueName(controle);
                    addControl(controle);
                }
                status.TextInfos = "Restauration de " + controleslist.Length + " composant(s)";
            }
            else
            {
                Control controle = thrash.PopControl();
                controle.Name = findUniqueName(controle);
                addControl(controle);
                status.TextInfos = "Restauration d'un composant";
            }

        }
        #endregion

        #region "Constructeur"

        /// <summary>
        /// Construit l'interface de conception
        /// </summary>
        public DesignerCluster()
        {
            InitializeComponent();
            thrash = new DesignThrash();

            Registry.MergeInMainMenu( menu );
            Registry.MergeInMainTools( tools );
                
            //Ajout controles
            designToolbox.RegisterControl(new ComboValidate().GetType());
            designToolbox.RegisterControl(new ComboBox().GetType());
            designToolbox.RegisterControl(new ComboIndex().GetType());
            designToolbox.RegisterControl(new DesignToolBox().GetType());
            designToolbox.RegisterControl(new CheckBox().GetType());
            designToolbox.RegisterControl(new ListBox().GetType());
            designToolbox.RegisterControl(new RadioButton().GetType());
            designToolbox.RegisterControl(new Button().GetType());
            designToolbox.RegisterControl(new ProgressBar().GetType());
            designToolbox.RegisterControl(new RichComboBox().GetType());
            designToolbox.RegisterControl(new RichTextBox().GetType());
            status = Registry.MainStatus;
            isChanged = false;
        }
        #endregion

        #region "Services"
        /// <summary>
        /// Ajout d'un controle
        /// </summary>
        /// <param name="component">Controle à ajouter: Control</param>
        [Description("Ajoute un controle")]
        public void addControl(Control component)
        {
            if (component == null)
                return;
            design.Controls.Add(component);
            selection.Items.Add(component.Name);
            this.Cursor = null;
        }

        /// <summary>
        /// Suppression d'un controle
        /// </summary>
        /// <param name="component">Controle à supprimer: Control</param>
        [Description("Supprime un controle")]
        public void removeControl(Control component)
        {
            if (component == null)
                return;
            design.Controls.Remove(component);
            selection.Items.Remove(component.Name);
            selection.Text = "";
        }

        /// <summary>
        /// Supprimer tous les controles
        /// </summary>
        [Description("Supprime tous les controles")]
        public void removeAllControl()
        {
            design.Controls.Clear();
            selection.Items.Clear();
            selection.Text = "";
        }

        /// <summary>
        /// Copier un controle dans le presse papier
        /// </summary>
        [Description("Copie un controle vers le presse papier")]
        private void copyControl()
        {
            if (design.SelectedControl == null)
                return;

            DesignSerializer.ControlToClipboard(design.SelectedControl);
            status.TextInfos = design.SelectedControl.Name+" copié vers le presse papier";
        }

        /// <summary>
        /// Coller un controle
        /// </summary>
        [Description("Colle un controle depuis le presse papier")]
        private void pastControl()
        {
            Control control = DesignSerializer.ClipboardToControl();
            control.Location = design.LastClickLocation;
            control.Name = findUniqueName(control);
            addControl(control);
        }

        /// <summary>
        /// Sauvegarder dans un fichier
        /// </summary>
        /// <param name="filename">Nom du fichier: String</param>
        [Description("Sauvegarde un controle vers un fichier")]
        private void SaveToFile(String filename)
        {
            DesignSerializer.ControlsToFile(filename, design.Controls);
            this.isChanged = false;
            this.saveFile = saveFileDialog.FileName;
            status.TextInfos = "Enregistrement effectué dans " + filename;
        }

        /// <summary>
        /// Afficher la boite de dialogue de sauvegarde
        /// </summary>
        [Description("Affiche la boite de dialogue de sauvegarde")]
        private void ShowSaveToFile()
        {
            if (this.saveFile != null)
                saveFileDialog.FileName = this.saveFile;
            saveFileDialog.ShowDialog();
        }

        /// <summary>
        /// Vérifie s'il faut demander à l'utilisateur de sauvegarder (Interface changée)
        /// </summary>
        [Description("Vérifie si l'utilisateur doit sauvegarder")]
        private void checkForSave()
        {
            DialogResult result = DialogResult.No;
            if (this.isChanged)
                result = MessageBox.Show("L'interface a été modifiée. Sauvegarder le fichier ?", "Question", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Cancel)
                return;
            if (result == DialogResult.Yes)
                ShowSaveToFile();
        }

        /// <summary>
        /// Renvoie un nom unique qui ne se trouve pas dans la surface de conception
        /// </summary>
        /// <param name="control">Control pour lequel on cherche un nom unique: Control</param>
        /// <returns>nom unique: String</returns>
        [Description("Renvoie un nom unique qui ne se trouve pas dans la surface de conception")]
        private String findUniqueName(Control control)
        {
            int number = 1;
            String name = control.GetType().Name;
            while (design.Controls.ContainsKey(name + number))
                number++;
            return name + number;
        }
        #endregion

    }

}
