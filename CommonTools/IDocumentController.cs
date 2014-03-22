using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Contrôleur associé à un document
    /// </summary>
    public interface IDocumentController : IReceiver, ICommandReceiver
    {
        /// <summary>
        /// Modèle associé au contrôleur
        /// </summary>
        IDocumentData Model { get; }

        /// <summary>
        /// Nom du document avec son extension
        /// </summary>
        string DocumentName { get; set; }

        /// <summary>
        /// Indique si le document a changé.
        /// </summary>
        bool IsDocumentChanged { get; }

        /// <summary>
        /// Ajout d'une vue au document
        /// </summary>
        /// <param name="view">
        /// Vue à ajouter
        /// </param>
        void AddView(IDocumentView view);

        /// <summary>
        /// Notification Fermeture d'une vue du document
        /// </summary>  
        void RemoveView();

        /// <summary>
        /// Evénément diffusé quand plus aucune vue n'est ouverte sur le document
        /// </summary>
        event EventHandler<EventArgs> NoMoreViews;

        /// <summary>
        /// Evènement diffusé lorsque la vue doit être fermée
        /// </summary>
        event EventHandler<CommandEventArgs> NeedToCloseView;
    }

    /// <summary>
    /// Données relatives à l'événement NewMessage
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        /// <summary>
        /// Message à transmettre
        /// </summary>
        public Command Command { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="command">
        /// Message permettant de construire la requête d'ajout
        /// </param>
        public CommandEventArgs(Command command)
        {
            this.Command = command;
        }
    }
}
