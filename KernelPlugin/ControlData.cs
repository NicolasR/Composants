using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;

namespace Stl.Tme.Components.Kernel
{
    [Serializable]
    class ControlData : IControlData
    {        
        //TODO: VOIR POINT 23
        private IPropertyData[] properties;        

        public string Name { get; set; }

        public SortedList<string, string> Properties { get; private set; }        

        public string Type { get; set; }       

        public ControlData(string name, string type)
        {
            this.Name = name;
            this.Type = type;
            this.Properties = new SortedList<string, string>();
        }
    }
}
