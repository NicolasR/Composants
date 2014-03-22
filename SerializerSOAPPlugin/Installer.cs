using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using Psl.Applications;

namespace Stl.Tme.Components.SerializerSOAPPlugin
{
    [PslPluginInstaller]
    static class SerializerSOAPPlugin
    {
        static void Install()
        {
            ApplicationState.ApplicationOpen += new EventHandler(ApplicationState_ApplicationOpen);
        }

        static void ApplicationState_ApplicationOpen(object sender, EventArgs e)
        {
            RegistryExtended.DocumentManager.RegisterSerializerPlugin(new SoapSerializer());
        }
    }
}
