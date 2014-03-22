using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDocumentData : IReceiver
    {
        /// <summary>
        /// Support de diffusion des changements : les vues doivent s'y abonner
        /// </summary>
        event EventHandler<MessageEventArgs> Change;

        /// <summary>
        /// Nom du contrôle sélectionné
        /// </summary>
        String CurrentControlName { get; }

        /// <summary>
        /// Liste des contrôles déposés sur la surface de conception. Types String -> IControlData
        /// </summary>
        SortedList<String, IControlData> ControlsDictionary { get; }     
    }
}
