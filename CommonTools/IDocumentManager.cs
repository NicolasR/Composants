using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stl.Tme.Components.Tools
{
    /// <summary>
    /// Manager des documents
    /// </summary>
    public interface IDocumentManager 
    {
        /// <summary>
        /// Obtient le sérialiseur associé à l'extension donnée en paramètre
        /// </summary>
        /// <param name="extension">
        /// Extension qui doit être gérée par le sérialiseur
        /// </param>
        /// <returns>
        /// Un sérialiseur capable de gérer l'extension demandée.
        /// </returns>
        ISerializer GetSerializer(string extension);

        /// <summary>
        /// Crée un nouveau document
        /// </summary>
        /// <returns>Les données du document en lecture seules</returns>
        void CreateDocument(object sender, EventArgs e);

        /// <summary>
        /// Ouvre un document existant 
        /// </summary>
        void OpenDocument(object sender, EventArgs e);

        /// <summary>
        /// Demande à l'utilisateur s'il souhaite sauvegarder le fichier
        /// </summary>
        /// <param name="documentName"></param>
        void AskForSave(string documentName, IDocumentController controller);

        /// <summary>
        /// Sauvegarde le document
        /// </summary>
        void SaveDocument(string documentName, IDocumentController controller);

        /// <summary>
        /// Sauvegarde le document dans un nom de fichier spécifié
        /// </summary>
        void SaveDocumentAs(IDocumentController controller);

        /// <summary>
        /// Sauvegarde tous les documents ouverts
        /// </summary>
        void SaveAllDocuments();

        //Ferme la vue courante
        void CloseView();

        //Ferme l'onglet associé à la vue dont l'id est passé en paramètre
        void CloseTabView(int viewId);

        /// <summary>
        /// Ferme le document sélectionné
        /// </summary>
        void CloseDocument();

        /// <summary>
        /// Ferme tous les documents ouverts
        /// </summary>
        void CloseAllDocuments();

        /// <summary>
        /// Coupe un document vers le presse papier
        /// </summary>
        void CutDocument();

        /// <summary>
        /// Copie un document vers le presse papier
        /// </summary>
        void CopyDocument();

        /// <summary>
        /// Colle un document depuis le presse papier
        /// </summary>
        /// <returns></returns>
        void PasteDocument();


        void Erase(); //???

        /// <summary>
        /// Annule la dernière modification
        /// </summary>
        void Undo();

        /// <summary>
        /// Fait la dernière opération annulée
        /// </summary>
        void Redo();

        /// <summary>
        /// Enregistre une nouvelle vue dans la liste des vues disponibles
        /// </summary>
        /// <param name="view"></param>
        void RegisterViewPlugin(Type view, String name);

        /// <summary>
        /// Enregistre un nouveau sérialiseur dans la liste des sérialiseurs disponibles
        /// </summary>
        /// <param name="serializer"></param>
        void RegisterSerializerPlugin(ISerializer serializer);
    }
}
