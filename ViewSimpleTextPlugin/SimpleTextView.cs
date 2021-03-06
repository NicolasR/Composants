using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.Collections;
using System.Windows.Forms;
using System.Reflection;
using Psl.Controls;

namespace Stl.Tme.Components.ViewSimpleTextPlugin
{
    /// <summary>
    /// Vue avec rendu simpliste en chaine de caracteres
    /// </summary>
    public partial class SimpleTextView : UserControl, IDocumentView
    {
        #region attributs

        private SortedList<string, Control> listControls;

        private SortedList<string, Type> availableTypes;

        /// <summary>
        /// Nom de la vue
        /// </summary>
        private String simpleName;

        #endregion

        #region propriétés

        public int Id { get; private set; }

        /// <summary>
        /// Nom simple de la vue
        /// </summary>
        public String SimpleName
        {
            get
            {
                return simpleName;
            }
        }

        /// <summary>
        /// Contrôleur de la vue. 
        /// </summary>
        public IDocumentController Controller
        {
            get;
            private set;
        }

        #endregion        

        #region constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        public SimpleTextView(IDocumentController controller, int id)
        {            
            simpleName = "Vue texte simple";
            listControls = new SortedList<string, Control>();
            availableTypes = new SortedList<string, Type>();

            Id = id;
            Controller = controller;

            InitializeComponent();
            Initialize();

            Controller.Model.Change += new EventHandler<MessageEventArgs>(Model_Change);
            Controller.NeedToCloseView += new EventHandler<CommandEventArgs>(Controller_NeedToCloseView);
            RegistryExtended.MergeInMainTools(tools);            
        }

        #endregion  

        #region méthodes publiques

        public void InitializeFromModel(IDocumentData model)
        {
            foreach (string key in model.ControlsDictionary.Keys)
            {
                IControlData control = model.ControlsDictionary[key];
                string[] description = { control.Name, control.Type };
                AddControl(description);

                foreach (string propertyname in control.Properties.Keys)
                {
                    string[] arguments = { control.Name, propertyname };
                    ChangeProperty(arguments);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void Update(Tools.Message message)
        {
            switch (message.Type)
            {
                case ChangeType.AddControl:
                    AddControl(message.Arguments);
                    break;

                case ChangeType.RemoveControl:
                    RemoveControl(message.Arguments);
                    break;

                case ChangeType.SelectControl:
                    SelectControl(message.Arguments);
                    break;

                case ChangeType.ChangeProperty:
                    ChangeProperty(message.Arguments);
                    break;
            }
        }

        #endregion

        #region méthodes privées

        private void Model_Change(object sender, MessageEventArgs e)
        {
            Update(e.Message);
        }

        private void Initialize()
        {
            Type[] newControls = { typeof(ComboBox), typeof(RichTextBox), typeof(Button), typeof(Label) };
            foreach (Type controlType in newControls)
            {
                availableTypes.Add(controlType.AssemblyQualifiedName, controlType);
                boxTypeControls.Items.Add(controlType.Name);
            }
            InitializeFromModel(Controller.Model);
        }
        
        #endregion
       
        #region événements

        private void actionAddControl_Execute(object sender, EventArgs e)
        {
            int number = 1;
            string typename = boxTypeControls.SelectedText;
            Type type = availableTypes[typename];

            while (Controller.Model.ControlsDictionary.Keys.Contains(typename + number))
                number++;

            string name = typename + number;
            string[] infos = { name, type.Name };
            CreateMessage(ChangeType.AddControl, infos);
        }

        private void actionRemoveControl_Execute(object sender, EventArgs e)
        {
            String namecontrol = instances.SelectedItem.ToString();

            //TODO : à nettoyer, lancer la bonne exception
            if (namecontrol == null)
                throw new Exception();

            string[] infos = { namecontrol };
            CreateMessage(ChangeType.RemoveControl, infos);
        }

        private void instances_SelectedValueChanged(object sender, EventArgs e)
        {
            if (instances.SelectedItem == null)
                return;

            string selected = instances.SelectedItem.ToString();
            Control selecteditem = listControls[selected];
            string selectedstring = selecteditem == null ? "" : selecteditem.Name;

            string[] infos = { selected };
            CreateMessage(ChangeType.SelectControl, infos);
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

        private void events_ApplicationIdle(object sender, EventArgs e)
        {
            tools.Visible = activebar.IsActive;
            actionRemoveControl.Enabled = instances.SelectedItem != null;
        }

        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            object value;
            PropertyDescriptor descriptor = Serializer.DoGetPropertyDescriptor(e, out value);
            

            string control = instances.SelectedItem.ToString();
            if (control == null)
                throw new ArgumentNullException();

            string property = descriptor.Name;
            string serialization = Serializer.SerializeProperty(descriptor, value);
            string[] infos = { control, property, serialization };

            CreateMessage(ChangeType.ChangeProperty, infos);
        }

        void Controller_NeedToCloseView(object sender, CommandEventArgs e)
        {
            if (!e.Command.Arguments.Contains<string>(Id.ToString()))
                return;
            String[] arguments = { Id.ToString() };
            Command command = new Command(MCommandType.ViewReadyToClose, arguments);
            Controller.ReceiveCommand(command);
            this.Dispose();
        }

        #endregion


        #region méthodes privées
        private void ChangeProperty(string[] args)
        {
            if (args.Count() < 2)
                throw new ArgumentException();

            string controlname = args[0];
            string propertyname = args[1];

            IControlData model = Controller.Model.ControlsDictionary[controlname];
            Control control = listControls[controlname];
            PropertyDescriptor descriptor = SerialHelper.GetPropertyDescriptor(control, propertyname);       

            string propertyvalue = model.Properties[propertyname];
            object value = SerialHelper.StringToPropertyValue(descriptor, propertyvalue);
                        
            PropertyInfo infos = control.GetType().GetProperty(propertyname);
            infos.SetValue(control, value, null);            
        }

        private void SelectControl(string[] args)
        {
            string controlname = args[0];

            if (controlname == "")
            {
                instances.ClearSelected();
                return;
            }
            instances.SelectedItem = controlname;
            properties.SelectedObject = listControls[controlname];
        }

        private void RemoveControl(string[] args)
        {
            if (args[0] == null)
                throw new ArgumentException();

            string controlname = args[0];
            instances.ClearSelected();
            instances.Items.Remove(controlname);
        }

        private void AddControl(string[] args)
        {
            if (args[0] == null)
                throw new ArgumentException();

            if (args[1] == null)
                throw new ArgumentException();


            string controlname = args[0];
            string typename = args[1];

            AddControl(controlname, typename);         
        }

        private void AddControl(string controlname, string typename)
        {
            if (!availableTypes.ContainsKey(typename))
            {
                MessageBox.Show("Le document contient des types de données non gérés par cette vue");
                return;
            }

            Type type = availableTypes[typename];
            ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
            Control control = (Control)constructor.Invoke(Type.EmptyTypes);
            control.Name = controlname;

            listControls.Add(controlname, control);
            instances.Items.Add(controlname);
        }
        #endregion
  
    }
}
