using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Interface de la partie "écriture seule du controlleur"
    /// </summary>
    public interface ICommandReceiver
    {
        /// <summary>
        /// Notification de commande
        /// </summary>
        /// <param name="message">
        /// Descripteur de la commande envoyée
        /// </param>
        void ReceiveCommand(Command message);        
    }

    public interface ICommandSender
    {
        /// <summary>
        /// Envoi d'une commande
        /// </summary>
        /// <param name="message">
        /// Description de la commande
        /// </param>
        void SendCommand(Command message);
    }
}
