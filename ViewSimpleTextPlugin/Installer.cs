using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using Psl.Applications;

namespace Stl.Tme.Components.ViewSimpleTextPlugin
{
    [PslPluginInstaller]
    static class ViewSimpleTextPlugin
    {
        static void Install()
        {
            ApplicationState.ApplicationOpen += new EventHandler(ApplicationState_ApplicationOpen);
        }

        static void ApplicationState_ApplicationOpen(object sender, EventArgs e)
        {
            RegistryExtended.DocumentManager.RegisterViewPlugin(typeof(SimpleTextView), "Vue texte simple");           
        }
    }
}
