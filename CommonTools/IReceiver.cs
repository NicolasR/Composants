using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Interface de la partie "écriture seule du modèle"
    /// </summary>
    public interface IReceiver
    {
        /// <summary>
        /// Notification de changement
        /// </summary>
        /// <param name="message">
        /// Descripteur du changement apporté
        /// </param>
        void Receive(Message message);
    }
}
