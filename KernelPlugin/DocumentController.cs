using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stl.Tme.Components.Tools;
using System.IO;

namespace Stl.Tme.Components.Kernel
{
    class DocumentController : IDocumentController
    {
        #region attributs
        private IDocumentData model;

        private int totalViews;

        private bool isDocumentChanged; 
        #endregion

        #region propriétés
        public bool IsDocumentChanged
        {
            get
            {
                return isDocumentChanged;
            }
        }

        public IDocumentData Model
        {
            get { return model; }
        }

        public string DocumentName { get; set; }
        
        #endregion

        #region événements

        public event EventHandler<EventArgs> NoMoreViews;

        public event EventHandler<CommandEventArgs> NeedToCloseView;
        #endregion

        #region constructeurs

        /// <summary>
        /// Constructeur lors de la création d'un nouveau document
        /// </summary>
        /// <param name="model">
        /// Modèle associé
        /// </param>
        public DocumentController(IDocumentData model) : this(model, "") { }

        /// <summary>
        /// Constructeur lors de l'ouverture d'un document existant
        /// </summary>
        /// <param name="model">
        /// Modèle associé
        /// </param>
        /// <param name="fileName">
        /// Nom du document géré par ce contrôleur.
        /// </param>
        public DocumentController(IDocumentData model, string fileName) 
        {
            this.model = model;
            totalViews = 0;
            isDocumentChanged = false;            
            DocumentName = fileName;
        }
        #endregion

        #region gestion des vues

        /// <summary>
        /// Ajout d'une vue gérée par ce contrôleur
        /// </summary>
        /// <param name="view"></param>
        public void AddView(IDocumentView view)
        {
            totalViews++;
        }

        public void RemoveView()
        {
            totalViews--;
            if (totalViews == 0)
                OnNoMoreViews(EventArgs.Empty);
        }
       
        #endregion

        #region Diffusion des événements
        protected void OnNoMoreViews(EventArgs e)
        {
            if (NoMoreViews != null)
                NoMoreViews(this, e);
        }

        protected void OnNeedToCloseView(CommandEventArgs e)
        {
            if (NeedToCloseView != null)
                NeedToCloseView(this, e);
        }
        #endregion

        #region réception de messages
        public void Receive(Message message)
        {            
            isDocumentChanged = (message.Type != ChangeType.SelectControl) || IsDocumentChanged;           
            Model.Receive(message);            
        }

        /// <summary>
        /// Réception d'une commande
        /// </summary>
        /// <param name="message">
        /// Description de la commande à réaliser
        /// </param>
        public void ReceiveCommand(Command message)
        {
            switch (message.Type)
            {                             
                case MCommandType.PerformSave:
                    String temp = Path.GetExtension(DocumentName);
                    string extension = Path.GetExtension(DocumentName).TrimStart('.');
                    ISerializer documentSerializer = RegistryExtended.DocumentManager.GetSerializer(extension);
                    FileSerializer.Save(model, DocumentName, documentSerializer);
                    break;
                
                case MCommandType.PerformSaveAs:
                    ISerializer serializer = RegistryExtended.DocumentManager.GetSerializer(message.Arguments[1]);
                    FileSerializer.Save(model, message.Arguments[0], serializer);
                    DocumentName = message.Arguments[0];
                    break;

                case MCommandType.PerformCloseView:
                    if (MustAskForSave())
                        RegistryExtended.DocumentManager.AskForSave(DocumentName,(IDocumentController)this);
                    else
                        OnNeedToCloseView(new CommandEventArgs(message));
                    break;

                case MCommandType.PerformCloseDocument:
                    throw new NotImplementedException();
                    break;

                case MCommandType.PerformCloseAll:
                    throw new NotImplementedException();
                    break;

                case MCommandType.ConfirmCloseView:
                    OnNeedToCloseView(new CommandEventArgs(message));
                    break;

                case MCommandType.ViewReadyToClose:
                    RemoveView();
                    int viewId = Convert.ToInt32(message.Arguments[0]);
                    if (viewId == null)
                        throw new ArgumentException();
                    RegistryExtended.DocumentManager.CloseTabView(viewId);
                    //throw new NotImplementedException();
                    break;
            }
        }
        #endregion

        private bool MustAskForSave()
        {
            return (totalViews-1 == 0) && (DocumentName.Length == 0 || isDocumentChanged);
        }
    }
}
