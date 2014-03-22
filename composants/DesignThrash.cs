using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Stl.Tme.Components.Controls
{
    /// <summary>
    /// Construit la corbeille
    /// </summary>
    public partial class DesignThrash : Component
    {

        #region "Attributs"

        /// <summary>
        /// Contient les elements mis dans la corbeille
        /// </summary>
        private List<String> listElement;

        /// <summary>
        /// Indique si un élement un Controle (true) ou une liste de controles (false)
        /// </summary>
        private List<Boolean> isOnlyControl;
        #endregion

        #region "Constructeur"
        /// <summary>
        /// Constructeur qui initialise la corbeille
        /// </summary>
        public DesignThrash()
        {
            InitializeComponent();
            listElement = new List<String>();
            isOnlyControl = new List<Boolean>();

        }
        
        /// <summary>
        /// Constructeur qui initialise la corbeille
        /// </summary>
        /// <param name="container"></param>
        public DesignThrash(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
        #endregion

        #region "Services"

        /// <summary>
        /// Ajoute un élement dans la corbeille
        /// </summary>
        /// <param name="element">Element à ajouter: String</param>
        /// <param name="isOneControle">Indique s'il s'agit d'un seul controle: Boolean</param>
        [Description("Ajoute un élement dans la corbeille")]
        private void PushElement(String element, Boolean isOneControle)
        {
            listElement.Insert(0, element);
            isOnlyControl.Insert(0, isOneControle);
        }

        /// <summary>
        /// Recupère le dernier élement ajouté dans la corbeille
        /// </summary>
        /// <returns>Dernier élement ajouté: String</returns>
        [Description("Recupère le dernier élement ajouté dans la corbeille")]
        private String PopElement()
        {
            String element = listElement.ElementAt(0);
            isOnlyControl.RemoveAt(0);
            listElement.RemoveAt(0);
            return element;
        }

        #endregion

        #region "Propriétés"

        /// <summary>
        /// Indique si la corbeille est vide
        /// </summary>
        /// <returns></returns>
        [Description("Indique si la corbeille est vide")]
        public Boolean IsEmpty()
        {
            return listElement.Count == 0;
        }

        /// <summary>
        /// Indique si le premier élement est un seul controle
        /// </summary>
        /// <returns></returns>
        /// [Browsable
        [Description("Indique si le premier élement est un seul controle")]
        public Boolean IsFirstOnlyOneControle()
        {
            if (IsEmpty())
                throw new Exception("Liste vide");
            return isOnlyControl.ElementAt(0);
        }

        /// <summary>
        /// Ajoute le controle dans la corbeille
        /// </summary>
        /// <param name="controle">controle à ajouter: Control</param>
        [Description("Ajoute le controle dans la corbeille")]
        public void PushControl(Control controle)
        {
            String serialized = DesignSerializer.ControlToString(controle);
            PushElement(serialized, true);
        }

        /// <summary>
        /// Ajoute une liste de controles dans la corbeille
        /// </summary>
        /// <param name="controles">Liste de controles: IEnumerable</param>
        [Description("Ajoute une liste de controles dans la corbeille")]
        public void PushControls(IEnumerable controles)
        {
            String serialized = DesignSerializer.ControlsToString(controles);
            PushElement(serialized, false);
        }

        /// <summary>
        /// Récupère le dernier controle supprimé
        /// </summary>
        /// <returns>Dernier controle supprimé: Control</returns>
        [Description("Récupère le dernier controle supprimé")]
        public Control PopControl()
        {
            if (IsEmpty())
                throw new Exception("Liste vide");

            if (isOnlyControl.ElementAt(0) == false)
                return null;

            String serialized = PopElement();
            Control controle = DesignSerializer.StringToControl(serialized);
            return controle;
        }

        /// <summary>
        /// Récupère la derniere liste de  controles supprimée sous forme de tableau
        /// </summary>
        /// <returns>Tableau de controle supprimé: Control</returns>
        [Description("Récupère la derniere liste de  controles supprimée sous forme de tableau")]
        public Control[] PopControls()
        {
            if (IsEmpty())
                throw new Exception("Liste vide");

            if (isOnlyControl.ElementAt(0) == true)
                return null;

            Control[] controles = DesignSerializer.StringToControls(PopElement());
            return controles;
        }
        #endregion
    }
}
