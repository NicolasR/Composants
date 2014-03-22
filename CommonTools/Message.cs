using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class Message
    {
        /// <summary>
        /// Identification d’un émetteur du message
        /// </summary>
        public IDocumentView Sender { get; set; }
        
        /// <summary>
        /// Identification du changement 
        /// </summary>
        public ChangeType Type { get; set; }

        /// <summary>
        /// Identification du type d'action
        /// </summary>
        public ActionType Action { get; set; }

        /// <summary>
        /// Arguments du changement : un tableau ou une liste de chaînes de caractères fournissant
        /// sous forme sérialisée les données associées au changement. 
        /// </summary>
        public String[] Arguments { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="sender">
        /// Vue qui a créé le message
        /// </param>
        /// <param name="type">
        /// Type de changement demandé
        /// </param>
        /// <param name="arguments">
        /// Arguments du message
        /// </param>
        public Message(IDocumentView sender, ChangeType type, String[] arguments)  
        { 
            this.Sender = sender; 
            this.Type = type;
            this.Arguments = arguments; 
        } 
    }

    /// <summary>
    /// Type de modification
    /// </summary>
    public enum ChangeType
    {
       /// <summary>
       /// Ajouter un contrôle. La description du contrôle est donnée dans les paramètres 
       /// du message. En fait juste son nom et son type, puisqu'il suffit à instancier un contrôle par réflexion
       /// (avec ses propriétés par défaut).
       /// </summary>
       AddControl,

       /// <summary>
       /// Retirer un contrôle. Le nom du contrôle à retirer est donné dans les paramètres du message.
       /// </summary>
       RemoveControl,

       /// <summary>
       /// Sélectionner l’un des contrôles. Le nom du contrôle à sélectionner est donné dans les paramètres 
       /// du message. Si le nom du contrôle est vide, désélectionner le contrôle actuellement sélectionné.
       /// </summary>
       SelectControl,

       /// <summary>
       /// Modifier la valeur d’une propriété d’un contrôle. Le nom du contrôle concerné, le nom de la 
       /// propriété concernée ainsi que la nouvelle valeur de la propriété (sous forme sérialisée) sont 
       /// donnés dans les paramètres du message.
       /// </summary>
       ChangeProperty
    }

    /// <summary>
    /// Type d'action
    /// </summary>
    public enum ActionType
    {
        /// <summary>
        /// Action de type Do, que l'on peut donc défaire.
        /// </summary>
        Do,

        /// <summary>
        /// Action de type Undo, que l'on peut donc refaire.
        /// </summary>
        Undo
    }
}
