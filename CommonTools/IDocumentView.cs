using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Interface représentant une vue quelconque d'une surface de conception
    /// </summary>
    public interface IDocumentView 
    {
        /// <summary>
        /// ID de la vue, 
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Controleur de la vue
        /// </summary>
        IDocumentController Controller { get; }

        /// <summary>
        /// Nom de la vue
        /// </summary>
        String SimpleName { get; }

        /// <summary>
        /// Mise à jour lorsque le modèle change, cette méthode doit être abonnée à l'événement
        /// </summary>
        /// <param name="message">
        /// Message décrivant les mises à jour à réaliser
        /// </param>
        void Update(Message message);

        /// <summary>
        /// Initialisation de la vue à partir d'un modèle
        /// </summary>
        /// <param name="model">
        /// modèle dont on tire les données à représenter
        /// </param>
        void InitializeFromModel(IDocumentData model);
    }

    /// <summary>
    /// Données relatives à l'événement NewMessage
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Message à transmettre
        /// </summary>
        public Message Message { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="message">
        /// Message permettant de construire la requête d'ajout
        /// </param>
        public MessageEventArgs(Message message)
        {
            this.Message = message;
        }
    }  
}
