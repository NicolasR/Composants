

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psl.Applications;

namespace Stl.Tme.Components.Designer
{
    // Germe d'installation du plugin de test des composants
    [PslPluginInstaller]
    static class DesignerPlugin
    {
        // Méthode d'installation du plugin
        static void Install()
        {
            //Registry.MainPages.ClientInsert(1, new DesignerCluster(), "Plugin de Design", null, true);
        }        
    }
}
