using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Interface implémentée par tout sérialiseur
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        /// Nom du sérialiseur
        /// </summary>
        string SimpleName { get; }

        /// <summary>
        /// Extension associée aux fichier sérialisés avec ce sérialiseur
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Sérialise les données
        /// </summary>
        /// <param name="value">le modèle a sérialiser</param>
        /// <returns>la chaîne sérialisée</returns>
        string Serialize(ControlDataWrapper[] controls);

        /// <summary>
        /// Désérialise la chaîne en paramètre
        /// </summary>
        /// <param name="value">la chaîne sérialisée</param>
        /// <returns>le modèle désérialisé</returns>
        ControlDataWrapper[] UnSerialize(string value);
    }

    [Serializable]
    public class PropertyDataWrapper
    {
        public string Property { get; set; }
        public string Value { get; set; }

        public PropertyDataWrapper(string property, string value)
        {
            this.Property = property;
            this.Value = value;
        }
    }

    [Serializable]
    public class ControlDataWrapper
    {
        public string name { get; set; }
        public string type { get; set; }
        public PropertyDataWrapper[] Properties { get; set; }

        public ControlDataWrapper(string name, string type, PropertyDataWrapper[] properties)
        {
            this.name = name;
            this.type = type;
            this.Properties = properties;
        }
    }
}
