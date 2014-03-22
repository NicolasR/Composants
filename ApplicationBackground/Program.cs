using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Psl.Applications;

namespace Stl.Tme.Components.Demo
{
    static class Program
    {
        // Gestionnaire de classe associé à l'événement ThreadException 
        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            if (!(e.Exception is ECancelled)) 
                ExceptionBox.Show(e.Exception);
        }

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += OnThreadException;
            Application.Run(new MainForm());
        }
    }
}
