using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Psl.Converters;

namespace Stl.Tme.Components.Controls
{ 
    /// <summary>
    /// ComboBox prenant en compte la gestion automatique de la liste déroulante 
    /// </summary>
    public partial class ComboIndex : ComboValidate
    {
        private int listLimit=0;
        private ListStyles listStyle = ListStyles.None;

        /// <summary>
        /// donne la valeur par défaut de la propriété ListLimit
        /// </summary>
        [Description("Nombre d'éléments par défaut conservés")]
        public const int DefaultLimit = 40;

        /// <summary>
        /// obtient ou détermine le style de gestion de la liste
        /// </summary>
        [Browsable(true)]
        [TypeConverter(typeof(EnumFlagsConverter))]     
        [DefaultValue(ListStyles.None)]
        [Description("Obtient ou détermine le style de gestion de la liste")]
        public ListStyles ListStyle 
        {
            get
            {
                return listStyle;
            }
            set
            {
                listStyle = value;         
                OnListStyleChanged(EventArgs.Empty);
            } 
        }       

        /// <summary>
        /// obtient ou détermine le nombre d’entrées maximal dans le cas d’une gestion en historique LRU. Le setter de la propriété assimile toute valeur négative à la valeur 0.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(DefaultLimit)]
        public int ListLimit
        {
            get
            {
                return listLimit;
            }

            set
            {
                if (value < 0)
                    listLimit = 0;
                else
                    listLimit = value;
            }
        }

        /// <summary>
        /// événement déclenché lorsque la valeur de la propriété ListStyle a changé.
        /// </summary>
        [Browsable(true)]
        [Description("Evénement déclenché lorsque la valeur de la propriété ListStyle a changé")]
        public event EventHandler ListStyleChanged;

        /// <summary>
        /// Constructeur 
        /// </summary>
        public ComboIndex()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Diffusion du changement de style 
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événements associé au changement de style
        /// </param>
        protected void OnListStyleChanged(EventArgs e)
        {
            if(ListStyleChanged != null)
                ListStyleChanged(this, e);
        }

        /// <summary>
        /// Diffusion de la confirmation de la validation
        /// </summary>
        /// <param name="e">
        /// Descripteur d'événement associé à la confirmation de validation
        /// </param>
        protected override void OnTextValidated(EventArgs e)
        {
            base.OnTextValidated(e);

            if (listStyle != ListStyles.AutoInsert)
                return;

            if (DropDownStyle == ComboBoxStyle.DropDownList)
                return;

            if (Sorted)
                SortedIndexManagement();
            else
                HistoricLRUManagement();               
        }

        #region Méthodes de service

        /// <summary>
        /// Gestion des ajouts en mode index trié
        /// </summary>
        private void SortedIndexManagement()
        {
            if (Items.IndexOf(Text) != -1)
                return;

            Items.Add(Text);            
        }

        /// <summary>
        /// Gestion des ajouts en mode Last Recently Used
        /// </summary>
        private void HistoricLRUManagement()
        {
            if (ListStyle == ListStyles.AutoLimit || ListStyle == ListStyles.All)
            {
                if (Items.Count == DefaultLimit)    
                    Items.RemoveAt(Items.Count);
            }

            if (Items.IndexOf(Text) == -1)
                Items.Insert(0, Text);
            else
                moveAtFirst(Text);
        }

        /// <summary>
        /// Déplacement d'un item en tête de liste
        /// </summary>
        /// <param name="item">
        /// Item à déplacer
        /// </param>
        private void moveAtFirst(Object item)
        {
            int index = Items.IndexOf(item);
            Object first = Items[0];
            Items.RemoveAt(index);
            Items.RemoveAt(0);
            Items.Insert(0, item);
            Items.Insert(index, first);
        }

        #endregion
    }

    /// <summary>
    /// Enumération des styles pour la gestion de la liste
    /// </summary> 
    [Flags]
    public enum ListStyles
    {
        /// <summary>
        /// Aucune option armée
        /// </summary> 
        None = 0x0,

        /// <summary>
        /// Ajouter automatiquement les chaînes validées
        /// </summary> 
        AutoInsert = 0x1,

        /// <summary>        
        /// Limiter le nombre d'entrées dans le cas d'une gestion en historique LRU
        /// </summary>
        AutoLimit = 0x2,

        /// <summary>
        /// Toutes options armées
        /// </summary> 
        All = 0x3,
    }
}
