using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;


namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Interface des contrôles présents sur la surface de conception
    /// </summary>
    public interface IControlData
    {
        /// <summary>
        /// Nom du contrôle
        /// </summary>     
        string Name { get; }

        /// <summary>
        /// Nom du type du contrôle
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Liste des propriétés du contrôle. Les clés correspondent au nom des propriétés. Les valeurs
        /// correspondent aux valeurs sérialisées des propriétés. Type string -> string. 
        /// Note : la taille de cette liste est fixe (on ajoute pas et on ne supprime pas de propriétés d'un contrôle)
        /// </summary>
        SortedList<string, string> Properties { get; }


    }
}
