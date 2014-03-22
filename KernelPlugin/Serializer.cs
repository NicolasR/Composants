using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psl.Controls;
using System.ComponentModel;
using System.Windows.Forms;


namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Classe de services pour la sérialisation
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Serialisation d'une propriété en chaine de caractère
        /// </summary>
        /// <param name="descriptor">
        /// Descripteur de la propriété
        /// </param>
        /// <param name="value">
        /// Valeur de la propriété
        /// </param>
        /// <returns>
        /// Valeur sérialisée de la propriété
        /// </returns>

        public static String SerializeProperty(PropertyDescriptor descriptor, object value)
        {
            return SerialHelper.PropertyValueToString(descriptor, value);
        }

        /// <summary>
        /// Serialisation d'une propriété en chaine de caractère dans le cadre de la coopération MVC par message
        /// </summary>
        /// <param name="descriptor">
        /// Descripteur de la propriété
        /// </param>
        /// <param name="value">
        /// Valeur sérialisée de la propriété
        /// </param>
        /// <returns>
        /// Valeur désérialisée de la propriété
        /// </returns>
        public static object DeserializeProperty(PropertyDescriptor descriptor, string value)
        {
            return SerialHelper.StringToPropertyValue(descriptor, value);
        }

        /// <summary>
        /// Récupère le descripteur de plus haut niveau associé à la propriété qui a été modifié.
        /// La valeur passée en référence corespond à la nouvelle valeur associée au descripteur.
        /// </summary>
        /// <param name="e">La propriété qui a été modifiée</param>
        /// <param name="value">La valeur associée</param>
        /// <returns>Le descripteur de plus haut niveau</returns>
        public static PropertyDescriptor DoGetPropertyDescriptor(PropertyValueChangedEventArgs e, out object value)
        {
            value = null;
            // récupérer l'élément de la grille qui a changé
            GridItem item = e.ChangedItem;

            // ignorer les éléments de grille qui ne sont pas des propriétés
            if (item.GridItemType != GridItemType.Property) return null;

            // remonter jusqu'à la propriété originale du composant
            while (item.Parent.GridItemType == GridItemType.Property)
                item = item.Parent;
            value = item.Value;
            // retourner le descripteur de la propriété modifiée au niveau du composant
            return item.PropertyDescriptor;
        }

        /// <summary>
        /// Récupère le descripteur de propriété associée au nom de la propriété
        /// </summary>
        /// <param name="type">Type du controle</param>
        /// <param name="propertyName">Nom de la propriété</param>
        /// <returns>le descripteur de propriété associé</returns>
        public static PropertyDescriptor DoGetPropertyDescriptorType(Type type, string propertyName)
        {
            PropertyDescriptorCollection controlDefaultProperties = TypeDescriptor.GetProperties(type, new Attribute[] { BrowsableAttribute.Yes });
            return controlDefaultProperties.Find(propertyName, false);
        }

        /// <summary>
        /// Récupère la valeur de la propriété associée au descripteur de propriété
        /// </summary>
        /// <param name="type">Type du controle</param>
        /// <param name="control">Les données associées au controle</param>
        /// <param name="propertyName">Le nom de la propriété</param>
        /// <returns>La valeur associée à la propriété</returns>
        public static object DoGetPropertyValue(Type type, IControlData control, string propertyName)
        {
            string controlLocationValue = control.Properties[propertyName];

            PropertyDescriptor descriptor = Serializer.DoGetPropertyDescriptorType(type, propertyName);
            return Serializer.DeserializeProperty(descriptor, controlLocationValue); 
        }


    }
}

