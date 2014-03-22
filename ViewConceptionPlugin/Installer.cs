using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using Psl.Applications;

namespace Stl.Tme.Components.ViewConceptionPlugin
{
    [PslPluginInstaller]
    static class ViewConceptionPlugin
    {
        static void Install()
        {
            ApplicationState.ApplicationOpen += new EventHandler(ApplicationState_ApplicationOpen);
        }

        static void ApplicationState_ApplicationOpen(object sender, EventArgs e)
        {
            RegistryExtended.DocumentManager.RegisterViewPlugin(typeof(DesignerCluster), "Vue conception");
        }
    }
}
