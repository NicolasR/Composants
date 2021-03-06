using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.Collections;
using Psl.Controls;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using Stl.Tme.Components.Controls;

namespace Stl.Tme.Components.Kernel
{
    class DocumentModel : IDocumentData
    {
        #region attributs

        /// <summary>
        /// Nom du contrôle courant
        /// </summary>
        [Description("Nom du contrôle sélectionné. S'il n'y en a pas, c'est une chaîne vide")]
        private String currentControlName;

        /// <summary>
        /// Dictionnaire des contrôles présents dans le document
        /// </summary>
        [Description("Liste des contrôles présents dans le document. Les clés sont les noms des documents")]
        private SortedList<String, IControlData> controlsDictionary;

        #endregion

        #region événements
        
        /// <summary>
        /// Support de diffusion des changements : les vues doivent s'y abonner
        /// </summary>
        [Description("Evénement diffusé lorsque le modèle change d'état")]
        public event EventHandler<MessageEventArgs> Change;
    
        #endregion

        #region propriétés

        /// <summary>
        /// Nom du contrôle sélectionné
        /// </summary>
        [Description("Nom du contrôle sélectionné ou la chaîne vide s'il n'y en a pas")]
        public String CurrentControlName 
        {
            get
            {
                return currentControlName;
            }         
        }

        /// <summary>
        /// Liste des contrôles déposés sur la surface de conception
        /// </summary>
        [Description("Liste des contrôles présents dans le document")]
        public SortedList<String, IControlData> ControlsDictionary
        {
            get
            {
                return controlsDictionary;
            }       
        }
        #endregion

        #region méthodes exposées

        /// <summary>
        /// Constructeur
        /// </summary>
        public DocumentModel()
        {
            controlsDictionary = new SortedList<String, IControlData>();             
        }        
        
        /// <summary>
        /// Réception des messages envoyés par le contrôleur de document
        /// </summary>
        /// <param name="message">
        /// Message décrivant la modification à réaliser
        /// </param>
        [Description("Mise à jour du modèle par réception de message. Les modifications sont données par le message")]
        public void Receive(Tools.Message message)
        {            
            switch (message.Type)
            {
                case ChangeType.AddControl:
                    AddControl(message.Arguments, message.Action);
                    break;

                case ChangeType.RemoveControl:
                    RemoveControl(message.Arguments, message.Action);
                    break;

                case ChangeType.SelectControl:
                    SelectControl(message.Arguments, message.Action);
                    break;              

                case ChangeType.ChangeProperty:
                    ChangeProperty(message.Arguments, message.Action);
                    break;
            }
            OnChange(new MessageEventArgs(message));
        }
        #endregion
        
        #region méthodes protégées

        /// <summary>
        /// Diffusion de l'événement Change
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement. Ici, il s'agit de renvoyer le message aux abonnés pour qu'ils puissent se mettre à jour
        /// </param>
        [Description("Diffusion de l'événements Change")]
        protected void OnChange(MessageEventArgs e)
        {        
            if (Change != null)
                Change(this, e);            
        }   

        #endregion

        #region méthodes privées    
        
        /// <summary>
        /// Ajout d'un contrôle au dictionnaire
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir deux éléments dont le premier représente le
        /// nom du contrôle à ajouter et le second le type du contrôle à ajouter.
        /// </param>
        private void AddControl(string[] args, ActionType action)
        {
            if (args.Length != 4)
                throw new ArgumentException();
          
            string controlName = args[0];
            string controlType = args[1];
            int controlPosX = Convert.ToInt32(args[2]);
            int controlPosY = Convert.ToInt32(args[3]);

            if(action == ActionType.Undo)
            {
                string[] infos = { controlName };
                RemoveControl(infos, ActionType.Do);
                return;
            }
            
            IControlData data = new ControlData(controlName, controlType);
            Type type = Type.GetType(controlType);
            PropertyDescriptor descriptorLocation = Serializer.DoGetPropertyDescriptorType(type, "Location");
            
            System.Drawing.Point controlLocation = new System.Drawing.Point(controlPosX, controlPosY);
            string controlLocationValue = Serializer.SerializeProperty(descriptorLocation, controlLocation);
            data.Properties.Add("Location", controlLocationValue);

            controlsDictionary.Add(controlName, data);
        }

        /// <summary>
        /// Suppression d'un contrôle du dictionnaire
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir un seul élément représentant le nom 
        /// du contrôle à supprimer
        /// </param>
        private void RemoveControl(string[] args, ActionType action)
        {
            if (args.Length != 1)
                throw new ArgumentException();

            string controlName = args[0];

            if (action == ActionType.Undo)
            {
                //et la, c'est le drame : on a pas les infos pour reconstruire un message, il nous manque le type
                
                //string[] infos = { controlName, ??? };
                //AddControl(infos, ActionType.Do);

                return;
            }

            
            if(!controlsDictionary.ContainsKey(controlName))
                throw new ArgumentException();

            controlsDictionary.Remove(controlName);
        }

        /// <summary>
        /// Sélection d'un contrôle du dictionnaire
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir un seul élément représentant le nom 
        /// du contrôle à sélectionner. Si ce nom n'est pas contenu dans le dictionnaire, on déselectionne le dernier contrôle
        /// sélectionné.
        /// </param>
        private void SelectControl(string[] args, ActionType action)
        {            
            if(args.Length != 1)
                throw new ArgumentException();            
            
            string name = args[0];
            
            //Idem, on a perdu trop d'info, si on veut undo une sélection, pas de souci, mais si on veut
            //undo une déselection ? on a perdu le dernier contrôle sélectionné...
            if (action == ActionType.Undo)
            {
                currentControlName = controlsDictionary.ContainsKey(name) ? "" : name;
                return;
            }

            currentControlName = controlsDictionary.ContainsKey(name) ? name : "";                
        }

        /// <summary>
        /// Modification de la valeur d'une propriété pour un contrôle du dictionnaire
        /// </summary>
        /// <param name="args">
        /// Tableau de chaine représentant les arguments. Le tableau doit contenir trois éléments : le nom du contrôle, le nom
        /// de la propriété à modifier, la nouvelle valeur de cette propriété sous forme sérialisée.
        /// </param>
        private void ChangeProperty(string[] args, ActionType action)
        {
            if (args.Length != 3)
                throw new ArgumentException();

            String controlName = args[0];
            String propertyName = args[1];
            String serializedValue = args[2];           

            if(!controlsDictionary.ContainsKey(controlName))
                throw new ArgumentException();
            
            IControlData control = controlsDictionary[controlName];
            if (!control.Properties.ContainsKey(propertyName))
            {
                control.Properties.Add(propertyName, serializedValue);
                return;
            }            
            control.Properties[propertyName] = serializedValue;          
        }   

        #endregion

    }
}
