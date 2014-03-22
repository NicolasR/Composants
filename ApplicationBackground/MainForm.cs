using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Psl.Applications;
using Stl.Tme.Components.Tools;

namespace Stl.Tme.Components.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            RegistryExtended.MainMenu = menu;         
            RegistryExtended.MainTools = tools;       
            RegistryExtended.MainPages = tabdock;    
            RegistryExtended.MainStatus = status;
            ArchiverPlugin.Install(true);
            PluginManager.LoadPlugins(false);

            ApplicationState.OnOpen(this, EventArgs.Empty);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ApplicationState.OnClosing(this, e);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ApplicationState.OnClose(this, e);
        }

        private void applicationEvents_Archive(IArchiver sender)
        {
            sender.PushSection("main.form");         
            try
            {
                sender.ArchiveProperty("Top", this, 50);                
                sender.ArchiveProperty("Left", this, 50);
                sender.ArchiveProperty("Width", this, 600);
                sender.ArchiveProperty("Height", this, 400);
            }
            finally
            {
                sender.PopSection();
            }
        }

        private void actionQuit_Execute(object sender, EventArgs e)
        {
            Dispose();
        }

         
    }
}
