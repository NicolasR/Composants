using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    public interface IPropertyData
    {
        /// <summary>
        /// Nom de la propriété sérialisée
        /// </summary>
        String Name { get; set; }

        /// <summary>
        /// Valeur sérialisée de la propriété
        /// </summary>
        String Value { get; set; }        
    }
}
