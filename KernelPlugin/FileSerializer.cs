using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using Stl.Tme.Components.Controls;

namespace Stl.Tme.Components.Kernel
{
    /// <summary>
    /// Gère les opérations sur les fichiers
    /// </summary>
    class FileSerializer
    {
        /// <summary>
        /// Sauvegarde le modèle dans le fichier spécifié avec le sérialiseur donné
        /// </summary>
        /// <param name="model">modèle à sauvegarder</param>
        /// <param name="filename">nom du fichier de destination</param>
        /// <param name="serializer">sérialiseur utilisé</param>
        public static void Save(IDocumentData model, string filename, ISerializer serializer)
        {            
            /*IControlData[] controls = new IControlData[model.ControlsDictionary.Count()];

            for(int i=0; i<model.ControlsDictionary.Count(); i++)
            {
                string key = model.ControlsDictionary.Keys[i];
                IControlData control = model.ControlsDictionary[key];
                controls[i] = control;
            }
            string serialization = serializer.Serialize(controls);*/
            string[][] test = new string[model.ControlsDictionary.Count][];
            ControlDataWrapper[] controls = new ControlDataWrapper[model.ControlsDictionary.Count];
            for(int indexControl=0; indexControl<model.ControlsDictionary.Count; indexControl++)
            {
                string key = model.ControlsDictionary.Keys[indexControl];
                SortedList<string, string> properties = model.ControlsDictionary[key].Properties;
                PropertyDataWrapper[] listproperties = new PropertyDataWrapper[properties.Count];
                test[indexControl] = new string[properties.Count];
                for(int indexProperty=0; indexProperty<properties.Count(); indexProperty++)
                {
                    string property = properties.Keys[indexProperty];
                    string value = properties[property];
                    listproperties[indexProperty] = new PropertyDataWrapper(property, value);
                }
                string controlName = model.ControlsDictionary[key].Name;
                string controlType = model.ControlsDictionary[key].Type;
                controls[indexControl] = new ControlDataWrapper(controlName, controlType, listproperties);
            }

            string serialization = serializer.Serialize(controls);
            ControlDataWrapper[] final = serializer.UnSerialize(serialization);

            SaveFile(filename, serialization);
        }

        /// <summary>
        /// Charge le fichier donné grâce au sérialiseur passé en paramètre
        /// </summary>
        /// <param name="filename">le fichier à ouvrir</param>
        /// <param name="serializer">le sérialiseur utilisé</param>
        /// <returns>les données du document</returns>
        public static IDocumentData Load(string filename, ISerializer serializer)
        {
            if (filename == null)
                throw new ArgumentNullException();

            string loaded = LoadFile(filename);
            //char[] separators = { '\n' };
            //string[] serializedControls = loaded.Split(separators);

            //IControlData[] controls = new IControlData[serializedControls.Count()];
            string extension = Path.GetExtension(filename);

            ControlDataWrapper[] controlwrappers = (ControlDataWrapper[]) serializer.UnSerialize(loaded);
            IDocumentData model = new DocumentModel();

            foreach (ControlDataWrapper wrapper in controlwrappers)
            {
                ControlData control = new ControlData(wrapper.name, wrapper.type);
                PropertyDataWrapper[] datawrappers = wrapper.Properties;
                foreach (PropertyDataWrapper datawrapper in datawrappers)
                    control.Properties.Add(datawrapper.Property, datawrapper.Value);
                model.ControlsDictionary.Add(control.Name, control);
            }
            
            return model;
        }

        /// <summary>
        /// Ecrit le contenu dans le fichier
        /// </summary>
        /// <param name="filename">le nom du fichier de destination</param>
        /// <param name="content">le contenu à écrire</param>
        private static void SaveFile(string filename, string content)
        {
            StreamWriter writer = File.CreateText(filename);
            writer.WriteLine(content);
            writer.Close();           
        }

        /// <summary>
        /// Charge le contenu du fichier
        /// </summary>
        /// <param name="filename">nom du fichier à charger</param>
        /// <returns>les données du fichier</returns>
        private static string LoadFile(string filename)
        {
            StreamReader reader = File.OpenText(filename);            
            string result = "";
            
            while(!reader.EndOfStream)
                result += reader.ReadLine();

            reader.Close();
            return result;
        }
    }
}
