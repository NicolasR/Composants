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

namespace Stl.Tme.Components.SimpleTextViewPlugin
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
        /// <summary>
        /// Obtient le modèle représenté par la vue (lecture seule)
        /// </summary>
        public IDocumentData Model
        {
            get;
            private set;
        }

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

        public IDocumentController Controller
        {
            get;
            private set;
        }

        #endregion

        #region événements
        /// <summary>
        /// Evénement diffusé lorsqu'une vue a été modifiée. Elle requiert alors une modification du modèle,
        /// modification que seul le contrôleur est à même de réaliser.
        /// </summary>
        public event EventHandler<MessageEventArgs> NewMessage;

        /// <summary>
        /// Evénement diffusé à la fermeture de la vue
        /// </summary>
        public event EventHandler<EventArgs> Close;

        #endregion

        #region méthodes publiques

        /// <summary>
        /// Constructeur
        /// </summary>
        public SimpleTextView(IDocumentData model, IDocumentController controller)
        {
            listControls = new SortedList<string, Control>();
            availableTypes = new SortedList<string, Type>();
            simpleName = "Vue texte simple";
            Model = model;
            Controller = controller;
            InitializeComponent();
            Initialize();
            model.Change += new EventHandler(OnChange);
        }

        #endregion

        #region méthodes protégées

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected void OnClose(EventArgs e)
        {
            if (Close != null)
                Close(this, e);
        }

        #endregion

        #region méthodes privées

        private void Initialize()
        {
            Type[] newControls = { typeof(ComboBox), typeof(WebBrowser), typeof(Button), typeof(Label) };
            foreach (Type controlType in newControls)
            {
                availableTypes.Add(controlType.FullName, controlType);
                boxTypeControls.Items.Add(controlType.FullName);
            }

        }

        private void OnChange(object sender, EventArgs e)
        {
            DoUpdateState();
        }

        private void DoUpdateState()
        {
            instances.Items.Clear();
            for (int i = 0; i < Model.ControlsDictionary.Count; i++)
            {                
                instances.Items.Add(Model.ControlsDictionary.Keys[i]);
            }
            instances.SelectedItem = Model.CurrentControlName;
            properties.SelectedObject = instances.SelectedItem;
        }

        #region Gestion des actions utilisateur

        private void actionAddControl_Execute(object sender, EventArgs e)
        {
            AddControl(false);
        }

        private void actionRemoveControl_Execute(object sender, EventArgs e)
        {
            RemoveControl(false);
        }

        private void instances_SelectedValueChanged(object sender, EventArgs e)
        {
            string selected = instances.SelectedItem as string;
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
            Stl.Tme.Components.Tools.Message msg = new Stl.Tme.Components.Tools.Message(this, change, infos);
            MessageEventArgs args = new MessageEventArgs(msg);
            Controller.Receive(msg);
        }

        #endregion

        private void properties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            PropertyDescriptor descriptor = e.ChangedItem.PropertyDescriptor;            
            object value = e.ChangedItem.Value;

            string control = instances.SelectedItem as string;
            if (control == null)
                throw new ArgumentNullException();

            string property = e.ChangedItem.PropertyDescriptor.Name;
            string serialization = Serializer.SerializeProperty(descriptor, value);
            string[] infos = { control, property, serialization };
            
            CreateMessage(ChangeType.ChangeProperty, infos);
        }        
        
        #endregion

        public void Receive(Tools.Message message)
        {
            switch (message.Type)
            {
                case ChangeType.SetControls:
                    //SetControls(message.Arguments);
                    break;

                case ChangeType.AddControl:
                    AddControl(true);
                    break;

                case ChangeType.RemoveControl:
                    RemoveControl(true);
                    break;

                case ChangeType.SelectControl:
                    throw new NotImplementedException();
                    break;

                case ChangeType.ChangeProperty:
                    //ChangeProperty(message.Arguments);
                    break;
            }
            throw new NotImplementedException();
        }

        private void AddControl(bool notifyViews)
        {
            int number = 1;
            string fullname = boxTypeControls.SelectedItem.ToString();
            Type type = availableTypes[fullname];

            ConstructorInfo CInfo = type.GetConstructor(Type.EmptyTypes);
            Control control = (Control)CInfo.Invoke(Type.EmptyTypes);

            while (Model.ControlsDictionary.Keys.Contains(control.Name + number))
                number++;

            string name = control.Name + number;
            
            string[] infos = { name, control.GetType().FullName };
            if (notifyViews)
                CreateMessage(ChangeType.AddControl, infos);
        }

        private void RemoveControl(bool notifyViews)
        {
            string selected = instances.SelectedItem as string;

            //TODO : à nettoyer, lancer la bonne exception
            if (selected == null)
                throw new Exception();


            string[] infos = { selected };
            if (notifyViews)
                CreateMessage(ChangeType.RemoveControl, infos);
        }

        private void ChangeProperty(bool notifyViews)
        {

        }
    }
}
