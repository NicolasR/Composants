using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;

namespace Stl.Tme.Components.Kernel
{
    [Serializable]
    class PropertyData : IPropertyData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
