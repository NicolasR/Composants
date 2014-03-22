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
using Psl.Controls;
using System.IO;
using Stl.Tme.Components.Controls;
using Stl.Tme.Components.Tools;

namespace Stl.Tme.Components.ViewConceptionPlugin
{

    /// <summary>
    /// Vue surface de conception
    /// </summary>
    public partial class DesignerCluster : UserControl, IDocumentView
    {
        #region attributs
    
        private SortedList<string, Type> availableTypes;
        private string simpleName;

        #endregion

        #region propriétés

        /// <summary>
        /// Identifiant de la vue
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Contrôleur de la vue.
        /// </summary>
        public IDocumentController Controller
        {
            get;
            private set;
        }

        /// <summary>
        /// Nom simple de ce type de vue. C'est le nom qui apparaitra sur les onglets "ouvrir" et "nouveau" dans les menus lors
        /// du chargement dynamique des plugins.
        /// </summary>
        public string SimpleName
        {
            get
            {
                return simpleName;
            }
        }

        #endregion

        #region événements

        /// <summary>
        /// Gestion evènement lorsque le clique souris est relaché
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void design_MouseUp(object sender, MouseEventArgs e)
        {
            properties.SelectedObject = properties.SelectedObject;
            if (designToolbox.SelectedControl == null)
                return;

            Type componentType = designToolbox.SelectedControl.Type;            
            string name = FindUniqueName(componentType);

            string posX = e.Location.X.ToString();
            string posY = e.Location.Y.ToString();
            string[] infos = { name, componentType.AssemblyQualifiedName, posX, posY};
            CreateMessage(ChangeType.AddControl, infos);
            designToolbox.UnSelectAll();
        }

        /// <summary>
        /// Gestion evènement Application au repos (Application idle)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void events_ApplicationIdle(object sender, EventArgs e)
        {

            bool active = bar.IsActive;
            tools.Visible = active;
            actions.Visible = active;
            //actions.Enabled = false;
            editionToolStripMenuItem.Visible = active;
            if (!active)
                return;

            acRemoveAll.Enabled = design.Controls.Count > 0;
            acRemoveComp.Enabled = design.SelectedControl != null;
            acCopyControl.Enabled = design.SelectedControl != null;
            acCutControl.Enabled = design.SelectedControl != null;
            acPastControl.Enabled = Clipboard.ContainsData(typeof(ControlWrapper).FullName);
            tools.Visible = true;

            if (designToolbox.SelectedControl == null)
                this.Cursor = DefaultCursor;
            else
                this.Cursor = designToolbox.SelectedControl.Cursor;
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

            string[] infos = { design.SelectedControl.Name };
            CreateMessage(ChangeType.SelectControl, infos)  ;
        }

        /// <summary>
        /// Gestion de l'évènement SelectedValueChanged de la combobox de sélection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selection_SelectedValueChanged(object sender, EventArgs e)
        {
            string[] infos = { design.SelectedControl.Name };
            CreateMessage(ChangeType.SelectControl, infos);           
        }
        
        /// <summary>
        /// Gestion de l'événement PropertyValueChanged de la grille de propriétés
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            design.Invalidate();
            object value;
            PropertyDescriptor descriptor = Serializer.DoGetPropertyDescriptor(e, out value);

            string control = design.SelectedControl.Name;
            if (control == null)
                throw new ArgumentNullException();

            string property = descriptor.Name;
            string serialization = Serializer.SerializeProperty(descriptor, value);
            string[] infos = { control, property, serialization };

            CreateMessage(ChangeType.ChangeProperty, infos);
        }

        void Model_Change(object sender, MessageEventArgs e)
        {
            Tools.Message msg = e.Message;
            Update(msg);
        }

        void Controller_NeedToCloseView(object sender, CommandEventArgs e)
        {
            if (!e.Command.Arguments.Contains<string>(Id.ToString()))
                return;
            Command command = new Command(MCommandType.ViewReadyToClose, e.Command.Arguments);
            Controller.ReceiveCommand(command);
            this.Dispose();
        }
        #endregion

        #region actions
      
        /// <summary>
        /// Supprimer tous les controles
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acRemoveAll_Execute(object sender, EventArgs e)
        {
            RemoveAllControl();
        }

        /// <summary>
        /// Supprimer un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acRemoveComp_Execute(object sender, EventArgs e)
        {
            string name = design.SelectedControl.Name;
            string[] infos = { name };

            CreateMessage(ChangeType.RemoveControl, infos);
        }

        /// <summary>
        /// Copier un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acCopyControl_Execute(object sender, EventArgs e)
        {
            CopyControl();
        }

        /// <summary>
        /// Couper un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acCutControl_Execute(object sender, EventArgs e)
        {
            CopyControl();
            String name = design.SelectedControl.Name;
            design.Controls.Remove(design.SelectedControl);
            selection.Items.Remove(name);
            selection.Text = "";
        }

        /// <summary>
        /// Coller un controle
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void acPastControl_Execute(object sender, EventArgs e)
        {
            PasteControl();
        }

        #endregion

        #region constructeur

        /// <summary>
        /// Construit l'interface de conception
        /// </summary>
        public DesignerCluster(IDocumentController controller, int id)
        {
            InitializeComponent();

            Registry.MergeInMainMenu( menu );
            Registry.MergeInMainTools( tools );

            simpleName = "Vue conception";
            availableTypes = new SortedList<string, Type>();
            Id = id;
            Controller = controller;

            Controller.Model.Change += new EventHandler<MessageEventArgs>(Model_Change);
            Controller.NeedToCloseView += new EventHandler<CommandEventArgs>(Controller_NeedToCloseView);
            Initialize();
        }
        #endregion
     
        #region méthodes publiques

       

        /// <summary>
        /// Supprimer tous les controles
        /// </summary>
        [Description("Supprime tous les controles")]
        public void RemoveAllControl()
        {
            foreach (Control control in design.Controls)
                CreateMessage(ChangeType.RemoveControl, new string[] { control.Name });
        }

        /// <summary>
        /// Mise à jour lorsque le modèle change, cette méthode doit être abonnée à l'événement
        /// </summary>
        /// <param name="message">
        /// Message décrivant les mises à jour à réaliser
        /// </param>
        [Description("Mise à jour de la vue à partir des informations contenues dans le message donné")]
        public void Update(Tools.Message message)
        {
            switch (message.Type)
            {
                case ChangeType.AddControl:
                    UpdateAddControl(message.Arguments);
                    break;

                case ChangeType.RemoveControl:
                    UpdateRemoveControl(message.Arguments);
                    break;

                case ChangeType.SelectControl:
                    UpdateSelectControl(message.Arguments);
                    break;

                case ChangeType.ChangeProperty:
                    UpdateChangeProperty(message.Arguments);
                    break;
            }
        }

        /// <summary>
        /// Initialise la vue avec les données d'un modèle
        /// </summary>
        /// <param name="model">
        /// Modèle dont on tire les informations à afficher
        /// </param>
        [Description("Initialisation de la vue à partir des informations contenues dans le modèle donné")]
        public void InitializeFromModel(IDocumentData model)
        {
            foreach (string key in model.ControlsDictionary.Keys)
            {
                IControlData control = model.ControlsDictionary[key];

                Type type = Type.GetType(control.Type);

                Point controlLocation = (Point)Serializer.DoGetPropertyValue(type, control, "Location");

                if (control == null)
                    throw new ArgumentNullException();

                string posX = controlLocation.X.ToString();
                string posY = controlLocation.Y.ToString();
                string[] description = { control.Name, control.Type, posX, posY };
                UpdateAddControl(description);

                foreach (string propertyname in control.Properties.Keys)
                {
                    string[] arguments = { control.Name, propertyname };
                    UpdateChangeProperty(arguments);
                }
            }
        }

        #endregion

        #region méthodes privées

        private void Initialize()
        {
            Type[] newControls = { typeof(ComboValidate), typeof(ComboBox), typeof(ComboIndex), typeof(DesignToolBox), typeof(CheckBox),
                                   typeof(ListBox), typeof(RadioButton), typeof(Button), typeof(ProgressBar), typeof(RichComboBox), typeof(RichTextBox)};
            foreach (Type controlType in newControls)
            {
                availableTypes.Add(controlType.AssemblyQualifiedName, controlType);
                designToolbox.RegisterControl(controlType);
            }

            InitializeFromModel(Controller.Model);
        }

        /// <summary>
        /// Création d'un message
        /// </summary>
        /// <param name="change">
        /// Type de message
        /// </param>
        /// <param name="infos">
        /// Arguments du message
        /// </param>
        private void CreateMessage(ChangeType change, string[] infos)
        {
            Tools.Message msg = new Tools.Message(this, change, infos);
            MessageEventArgs args = new MessageEventArgs(msg);
            Controller.Receive(msg);       
        }

        /// <summary>
        /// Sélection d'un contrôle 
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir un seul élément représentant le nom 
        /// du contrôle à sélectionner. Si ce nom est inconnu, on déselectionne le dernier contrôle sélectionné.
        /// </param>
        private void UpdateSelectControl(string[] p)
        {
            if(p.Length != 1)
                throw new ArgumentException();

            string controlname = p[0];

            if(!design.Controls.ContainsKey(controlname))
            {
                designToolbox.UnSelectAll();
                return;
            }

            design.SelectedControl = design.Controls[controlname];           
            properties.SelectedObject = design.SelectedControl;
        }

        /// <summary>
        /// Modification de la valeur d'une propriété pour un contrôle donné
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir trois éléments : le nom du contrôle, le nom
        /// de la propriété à modifier, la nouvelle valeur de cette propriété sous forme sérialisée.
        /// </param>
        private void UpdateChangeProperty(string[] args)
        {
            if (args.Count() < 2) 
                throw new ArgumentException();

            string controlname = args[0];
            string propertyname = args[1];

            IControlData model = Controller.Model.ControlsDictionary[controlname];
            Control control = design.Controls[controlname];

            PropertyDescriptor descriptor = SerialHelper.GetPropertyDescriptor(control, propertyname);

            string propertyvalue = model.Properties[propertyname];
            object value = SerialHelper.StringToPropertyValue(descriptor, propertyvalue);

            PropertyInfo infos = control.GetType().GetProperty(propertyname);
            infos.SetValue(control, value, null);
        }

        /// <summary>
        /// Suppression d'un contrôle 
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir un seul élément représentant le nom 
        /// du contrôle à supprimer
        /// </param>
        private void UpdateRemoveControl(string[] args)
        {
            if (args[0] == null)
                throw new ArgumentException();

            string controlname = args[0];
            if (!design.Controls.ContainsKey(controlname))
                return;

            UpdateRemoveControl(design.Controls[controlname]);
        }

        /// <summary>
        /// Suppression d'un controle
        /// </summary>
        /// <param name="component">
        /// Controle à supprimer: Control
        /// </param> 
        [Description("Supprime un controle")]
        private void UpdateRemoveControl(Control component)
        {
            if (component == null)
                return;

            design.Controls.Remove(component);
            selection.Items.Remove(component.Name);
            selection.Text = "";
        }

        /// <summary>
        /// Ajout d'un contrôle
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir deux éléments dont le premier représente le
        /// nom du contrôle à ajouter et le second le type du contrôle à ajouter.
        /// </param>
        private void UpdateAddControl(string[] args)
        {
            if (args[0] == null)
                throw new ArgumentException();

            if (args[1] == null)
                throw new ArgumentException();

            if (args[2] == null)
                throw new ArgumentException();

            if (args[3] == null)
                throw new ArgumentException();

            string controName = args[0];
            string typeName = args[1];
            int posX = Convert.ToInt32(args[2]);
            int posY = Convert.ToInt32(args[3]);

            UpdateAddControl(controName, typeName, new Point(posX, posY));            
        }

        private void UpdateAddControl(string controlname, string typename, Point location)
        {
            if (!availableTypes.ContainsKey(typename))
            {
                MessageBox.Show("Type de données non géré par la vue");
                return;
            }

            Type type = availableTypes[typename];
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            Control control = (Control)constructor.Invoke(Type.EmptyTypes);
            control.Name = controlname;
            control.Location = location;

            UpdateAddControl(control);
        }

        /// <summary>
        /// Ajout d'un controle
        /// </summary>
        /// <param name="component">
        /// Controle à ajouter: Control
        /// </param>        
        [Description("Ajoute un controle")]
        private void UpdateAddControl(Control component)
        {
            if (component == null)
                return;

            design.Controls.Add(component);
            selection.Items.Add(component.Name);
            this.Cursor = null;
            //virer le selected object
        }

        /// <summary>
        /// Copier un controle dans le presse papier
        /// </summary>
        [Description("Copie un controle vers le presse papier")]
        private void CopyControl()
        {
            if (design.SelectedControl == null)
                return;
            /*DesignSerializer.ControlToClipboard(design.SelectedControl);*/
        }

        /// <summary>
        /// Coller un controle
        /// </summary>
        [Description("Colle un controle depuis le presse papier")]
        private void PasteControl()
        {
            /*Control control = DesignSerializer.ClipboardToControl();
            control.Location = design.LastClickLocation;
            control.Name = findUniqueName(control);
            addControl(control);*/
        }        

        /// <summary>
        /// Trouve un nom unique pour créer une instance du type donné
        /// </summary>
        /// <param name="type">
        /// Type de l'instance pour laquelle on cherche un nom unique
        /// </param>
        /// <returns>
        /// Nom unique pour ce type de données.
        /// </returns>
        private string FindUniqueName(Type type)
        {
            int number = 1;
            string typeName = type.FullName;

            while (Controller.Model.ControlsDictionary.Keys.Contains(typeName + number))
                number++;

            return typeName + number;
        }

        #endregion


        
    }

}
