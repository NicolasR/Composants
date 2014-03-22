using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Aide à la récupération des éléments associés à la vue courante
    /// </summary>
    public class ViewInfos
    {
        /// <summary>
        /// Récupère la vue courante
        /// </summary>
        /// <returns>l'interface de la vue courante</returns>
        public static IDocumentView getCurrentView()
        {
            //Récupérer controle actif, parcourir parents jusqu'a ce que ce soit une vue
            Form currentForm = Form.ActiveForm;
            if (currentForm == null)
                return null;
            Control currentControl = Form.ActiveForm.ActiveControl;
            if (!(currentControl is IDocumentManager))
                return null;

            Control last = null;
            while (!(currentControl is IDocumentView))
            {
                last = currentControl;
                if (currentControl.GetContainerControl() == null)
                    return null;
                currentControl = currentControl.GetContainerControl().ActiveControl;
                if (currentControl == null || last == currentControl)
                    return null;
            }

            return (IDocumentView)currentControl;
        }

        /// <summary>
        /// Récupère les données de la vue courante
        /// </summary>
        /// <returns>l'interface des données de la vue courante</returns>
        public static IDocumentData getCurrentViewData()
        {
            return ViewInfos.getCurrentViewController().Model;
        }

        /// <summary>
        /// Récupère le controleur de la vue courante
        /// </summary>
        /// <returns>l'interface du controleur de la vue courante</returns>
        public static IDocumentController getCurrentViewController()
        {
            IDocumentView currentView = ViewInfos.getCurrentView();
            return currentView.Controller;
        }
    }
}
