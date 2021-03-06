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
        public SimpleTextView(IDocumentController controller)
        {
            listControls = new SortedList<string, Control>();
            availableTypes = new SortedList<string, Type>();
            simpleName = "Vue texte simple";            
            Controller = controller;
            InitializeComponent();
            Initialize();         
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

        #region Gestion des actions utilisateur

        private void actionAddControl_Execute(object sender, EventArgs e)
        {
            string fullname = boxTypeControls.SelectedText;
            Type type = availableTypes[fullname];
            type.GetConstructor(Type.EmptyTypes);

            /*

            while (Model.ControlsDictionary.Keys.Contains(control.Name + number))
                number++;

            string name = control.Name + number;
            
            string[] infos = { name, control.GetType().FullName };
            CreateMessage(ChangeType.AddControl, infos);
             * */
        }

        private void actionRemoveControl_Execute(object sender, EventArgs e)
        {           
            string selected = instances.SelectedItem as string;

            //TODO : à nettoyer, lancer la bonne exception
            if(selected == null)
                throw new Exception();

            
            string[] infos = { selected };
            CreateMessage(ChangeType.RemoveControl, infos);
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
                /*case ChangeType.SetControls:
                    SetControls(message.Arguments);
                    break;

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
                    break;*/
            }
            throw new NotImplementedException();
        }
    }
}
