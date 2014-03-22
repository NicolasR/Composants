using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

namespace Stl.Tme.Components.SerializerSOAPPlugin
{
    /// <summary>
    /// Sérialiseur SOAP : GAFFE => TOUT GERER AVEC LES WRAPPERS
    /// </summary>
    public class SoapSerializer : ISerializer
    {
        /// <summary>
        /// Nom du sérialiseur
        /// </summary>
        private string simpleName;

        /// <summary>
        /// Extension associée aux fichiers sérialisés avec ce sérialiseur
        /// </summary>
        private string extension;

        /// <summary>
        /// Nom du sérialiseur
        /// </summary>
        public string SimpleName 
        {
            get { return simpleName; } 
        }

        /// <summary>
        /// Extension associée aux fichiers sérialisés avec ce sérialiseur
        /// </summary>
        public string Extension 
        {
            get { return extension; }
        }

        /// <summary>
        /// Constructeur qui initialise le nom et l'extension du sérialiseur
        /// </summary>
        public SoapSerializer()
        {
            simpleName = "SoapSerializer";
            extension = "xml";
        }
        /// <summary>
        /// Sérialise les données
        /// </summary>
        /// <param name="value">le dictionnaire de controles</param>
        /// <returns>la chaîne sérialisée</returns>
        public string Serialize(ControlDataWrapper[] controls)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new SoapFormatter();
                formatter.Serialize(stream, controls);
                return Encoding.ASCII.GetString(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Désérialise la chaîne en paramètre
        /// </summary>
        /// <param name="value">le dictionnaire de controles</param>
        /// <returns>le dictionnaire de controles désérialisé</returns>
        public ControlDataWrapper[] UnSerialize(string value)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value)))
            {
                IFormatter formatter = new SoapFormatter();
                return (ControlDataWrapper[])formatter.Deserialize(stream);              
            }
        }



        
    }
}
