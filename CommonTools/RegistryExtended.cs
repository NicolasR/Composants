using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psl.Applications;
using Stl.Tme.Components.Tools;
using System.Windows.Forms;

namespace Stl.Tme.Components.Tools
{
    public partial class RegistryExtended : Registry
    {
        /// <summary>
        /// Raccourci d'accès au Controleur de document
        /// </summary>
        public static IDocumentManager DocumentManager
        {
            get { return (IDocumentManager)Get(DocumentKeys.KeyDocumentManager); }
            set { Add(DocumentKeys.KeyDocumentManager, value); }
        }
    }
}
