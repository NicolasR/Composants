using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using Psl.Controls;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Soap;

namespace Stl.Tme.Components.Controls
{
    /// <summary>
    /// Classe qui gère la sérialisation
    /// </summary>
    public class DesignSerializer
    {
        //public static DataFormats.Format Format { get {} } utilité???

        #region "Services"

        /// <summary>
        /// Renvoie une chaine sérialisée d'une liste de controles
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        [Description("Renvoie une chaine sérialisée d'une liste de controles")]
        public static string ControlsToString( IEnumerable controls ) 
        {

            ControlWrapper controlwrapped = DoWrapList(controls);
            return ToStringAsSoap(controlwrapped);
        }

        /// <summary>
        /// Renvoie une chaine sérialisée à partir d'un controle
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        [Description("Renvoie une chaine sérialisée à partir d'un controle")]
        public static string ControlToString( Control control )
        {
            ControlWrapper controlwrapped = DoWrapSolo(control);
            return ToStringAsSoap(controlwrapped);
        }

        /// <summary>
        /// Renvoie un tableau de controles à partir d'une chaine sérialisée
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("Renvoie un tableau de controles à partir d'une chaine sérialisée")]
        public static Control[] StringToControls( string value )
        {
            ControlWrapper controlwrapped = (ControlWrapper)FromStringAsSoap(value);
            Control controllist = new Control();
            DoUnwrapList(controlwrapped, controllist);
            Control[] tabcontrol = new Control[controllist.Controls.Count];
            for (int i=0;i<tabcontrol.Length;i++)
            {
                tabcontrol[i] = controllist.Controls[i];
            }
            return tabcontrol;
        }

        /// <summary>
        /// Renvoie un controle à partir d'une chaine sérialisée
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("Renvoie un controle à partir d'une chaine sérialisée")]
        public static Control StringToControl( string value )
        {
            ControlWrapper controlwrapped = (ControlWrapper)FromStringAsSoap(value);
            return DoUnwrapSolo(controlwrapped);
        }

        /// <summary>
        /// Met le controle dans le presse papier
        /// </summary>
        /// <param name="control"></param>
        [Description("Met le controle dans le presse papier")]
        public static void ControlToClipboard( Control control )
        {
            IDataObject dataObject = new DataObject() ;
            String serialized = ControlToString(control);
            dataObject.SetData(typeof(ControlWrapper).FullName, true, serialized );
            dataObject.SetData(DataFormats.Text, true, serialized );
            Clipboard.SetDataObject(dataObject, false);
        }

        /// <summary>
        /// Renvoie le controle présent dans le presse papier
        /// </summary>
        [Description("Renvoie le controle présent dans le presse papier")]
        public static Control ClipboardToControl()
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (!iData.GetDataPresent(DataFormats.Text))
                return null;

            String serialized = (String)iData.GetData(DataFormats.Text);
            return StringToControl(serialized);
        }

        /// <summary>
        /// Met l'élément dans le presse-papier avec le format associé
        /// </summary>
        /// <param name="objet">l'élément que l'on souhaite placer dans le presse-papier</param>
        /// <param name="format">le format associé</param>
        [Description("Met l'élément dans le presse papier, avec le format donné")]
        public static void ToClipboard(Object objet, String format)
        {
            IDataObject dataObject = new DataObject();
            dataObject.SetData(format, false, objet);
            Clipboard.SetDataObject(dataObject, false);
        }

        /// <summary>
        /// Récupère l'élément depuis le presse-papier si le format associé est bien celui que l'on attendait
        /// </summary>
        /// <param name="format">le format recherché</param>
        /// <returns>l'élément correspondant dans le presse-papier</returns>
        [Description("Récupère l'élément depuis le presse-papier si le format associé est bien celui donné en paramètre")]
        public static Object FromClipboard(String format)
        {
            IDataObject iData = Clipboard.GetDataObject();
            if (!iData.GetDataPresent(format, false))
                return null;

            return iData.GetData(DataFormats.Text);
        }

        /// <summary>
        /// Sauvegarde la liste de controles dans un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="controls"></param>
        [Description("Sauvegarde la liste de controles dans un fichier")]
        public static void ControlsToFile( string filename, IEnumerable controls ) 
        {
            TextWriter file = new StreamWriter(filename);
            String serialized = ControlsToString(controls);
            file.WriteLine(serialized);
            file.Close();
        }

        /// <summary>
        /// Renvoie un tableau de controles sauvegardé dans un fichier
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [Description("Renvoie un tableau de controles sauvegardé dans un fichier")]
        public static Control[] FileToControls( string filename ) 
        {
            TextReader file = new StreamReader(filename);
            String serialized = file.ReadToEnd();
            return StringToControls(serialized);
        }

        /// <summary>
        /// Sérialise un objet
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("Sérialise un objet")]
        private static string ToStringAsSoap(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                IFormatter formatter = new SoapFormatter();
                formatter.Serialize(stream, value);
                return Encoding.ASCII.GetString(stream.GetBuffer());
            }
        }

        /// <summary>
        /// Désérialise une chaine
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Description("Désérialise une chaine")]
        private static object FromStringAsSoap(string value)
        {
            using (MemoryStream stream = new MemoryStream(Encoding.ASCII.GetBytes(value)))
            {
                IFormatter formatter = new SoapFormatter();
                return formatter.Deserialize(stream);
            }
        }

        /// <summary>
        /// Enveloppe un controle
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        [Description("Enveloppe un controle")]
        private static ControlWrapper DoWrapSolo(Control control)
        {
            return ControlWrapper.Wrap(control);
        }

        /// <summary>
        /// Enveloppe une liste de controles
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        [Description("Enveloppe une liste de controles")]
        private static ControlWrapper DoWrapList(IEnumerable controls)
        {
            return ControlWrapper.Wrap(controls);
        }

        /// <summary>
        /// Extrait un controle d'une enveloppe
        /// </summary>
        /// <param name="wrapper"></param>
        /// <returns></returns>
        [Description("Extrait un controle d'une enveloppe")]
        private static Control DoUnwrapSolo(ControlWrapper wrapper)
        {
            return wrapper.Control;
        }

        /// <summary>
        /// Extrait une liste de controles d'une enveloppe 
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="control"></param>
        [Description("Extrait une liste de controles d'une enveloppe ")]
        private static void DoUnwrapList(ControlWrapper wrapper, Control control)
        {
            control.Controls.AddRange(wrapper.Controls);
        }
        #endregion
    }
}
