using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Identification d’un émetteur du message
        /// </summary>
        public IDocumentView Sender { get; set; }
        
        /// <summary>
        /// Identification de la commande 
        /// </summary>
        public MCommandType Type { get; set; }

        /// <summary>
        /// Arguments du changement : un tableau ou une liste de chaînes de caractères fournissant
        /// sous forme sérialisée les données associées au changement. 
        /// </summary>
        public String[] Arguments { get; set; }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// 
        /// <param name="type">
        /// Type de changement demandé
        /// </param>
        /// <param name="arguments">
        /// Arguments du message
        /// </param>
        public Command(MCommandType type, String[] arguments)  
        { 
            Type = type;
            Arguments = arguments; 
        } 
    }

    /// <summary>
    /// Type de modification
    /// </summary>
    public enum MCommandType
    {

        /// <summary>
        /// Sauvegarde le fichier
        /// </summary>
        PerformSave, 

       /// <summary>
       /// Sauvegarde le fichier en permettant à l'utilisateur de choisir une extension
       /// </summary>
       PerformSaveAs,

       /// <summary>
       /// Ferme la vue
       /// </summary>
       PerformCloseView,

       /// <summary>
       /// Ferme toutes les vues actives d'un document
       /// </summary>
       PerformCloseDocument,

       /// <summary>
       /// Ferme toutes les vues
       /// </summary>
       PerformCloseAll,

       /// <summary>
       /// Copie un controle vers le presse-papier
       /// </summary>
       PerformCopy,

       /// <summary>
       /// Coupe un controle vers le presse-papier
       /// </summary>
       PerformCut,

       /// <summary>
       /// Colle un controle dans la vue depuis le presse-papier
       /// </summary>
       PerformPast,

       /// <summary>
       /// Ne peut effectue la sauvegarde
       /// </summary>
       CantPerformSave,

       /// <summary>
       /// DocumentManager confirme que la vue peut être fermée
       /// </summary>
       ConfirmCloseView,

       /// <summary>
       /// La vue confirme qu'elle est prête a être fermée
       /// </summary>
       ViewReadyToClose,

       /// <summary>
       /// DocumentManager annule la fermeture de la vue
       /// </summary>
       AbortCloseView
    }

}
