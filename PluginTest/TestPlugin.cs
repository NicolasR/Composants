using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Psl.Applications;

namespace Stl.Tme.Components.Test
{
    // Germe d'installation du plugin de test des composants
    [PslPluginInstaller]
    static class TestPlugin
    {
        // Méthode d'installation du plugin
        static void Install()
        {
            Registry.MainPages.ClientInsert(0, new TestCluster(), "Plugin de test", null, true);
        }
    }
}
