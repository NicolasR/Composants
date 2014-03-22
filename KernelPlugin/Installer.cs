using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psl.Applications;
using Stl.Tme.Components.Tools;

namespace Stl.Tme.Components.Kernel
{
    // Germe d'installation du plugin de test des composants
    [PslPluginInstaller]
    static class KernelPlugin
    {
        // Méthode d'installation du plugin
        static void Install()
        {
            RegistryExtended.MainPages.ClientAdd(DocumentManager.Instance, "Plugin Noyau", null, true);
            RegistryExtended.DocumentManager = DocumentManager.Instance;
        }
    }
}
