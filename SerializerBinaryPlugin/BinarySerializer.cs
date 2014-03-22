using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Stl.Tme.Components.SerializerBinaryPlugin
{
    /// <summary>
    /// Sérialiseur Binaire : TOUT GERER AVEC DES WRAPPERS, FAIS GAFFE
    /// </summary>
    class BinarySerializer : ISerializer
    {
        /// <summary>
        /// Nom du sérialiseur
        /// </summary>
        private string name;

        /// <summary>
        /// Extension associée au sérialiseur Binaire
        /// </summary>
        private string extension;

        /// <summary>
        /// Renvoie le nom du sérialiseur
        /// </summary>
        public string SimpleName
        {
            get { return name; }
        }

        /// <summary>
        /// Renvoie l'extension associée au sérialiseur Binaire
        /// </summary>
        public string Extension
        {
            get { return extension; }
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        public BinarySerializer()
        {
            name = "BinarySerializer";
            extension = "bin";
        }

        /// <summary>
        /// Sérialise les données du document
        /// </summary>
        /// <param name="controls">le tableau de données de controles à sérialiser</param>
        /// <returns>une chaîne sérialisée</returns>
        public string Serialize(ControlDataWrapper[] controls)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, controls);
                return Encoding.ASCII.GetString(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Désérialise les données du document
        /// </summary>
        /// <param name="value">la chaîne sérialisée</param>
        /// <returns>la tableau de données de controles à sérialiser</returns>
        public ControlDataWrapper[] UnSerialize(string value)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value)))
            {
                IFormatter formatter = new BinaryFormatter();
                return (ControlDataWrapper[])formatter.Deserialize(stream);
            }
        }
    }
}
