using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Stl.Tme.Components.Tools;
using Psl.Applications;
using System.Reflection;
using System.IO;

namespace Stl.Tme.Components.Kernel
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DocumentManager : UserControl, IDocumentManager
    {
        //REMARQUE : HYPOTHESE FAITE SUR L'IDENTIFICATION DU MODELE PAR RAPPORT AU DOCUMENT => LE NOM DU FICHIER SANS 
        //L'EXTENSION REPRESENTE DE MANIERE UNIQUE UN DOCUMENT 
        //On peut donc désormais récupérer le contrôleur correspondant à un document quand on l'ouvre grâce à la table
        //d'association controllers. Possible conflit avec la propriété "DocumentName" de IDocumentController

        //PROBLEME : on est partis sur une mauvaise idée : à savoir "ouvrir" un même document par différentes vues...
        //défaut : on le gère alors qu'on devrait pas
        //avantage : on le gère alors qu'on devrait pas 
        // =D

        

        #region attributs

        /// <summary>
        /// id généré pour la prochaine vue à ajouter
        /// </summary>
        private int id;

        /// <summary>
        /// List des vues créées
        /// key : id de la vue
        /// value : indice de la vue dans le tableau à onglets
        /// </summary>
        private SortedList<int, int> managedViews;

        /// <summary>
        /// Unique instance 
        /// </summary>
        private static DocumentManager instance;

        /// <summary>
        /// Liste des vues disponibles
        /// </summary>
        private SortedList<string, Type> availableViews;

        /// <summary>
        /// Liste des sérialiseurs disponibles
        /// </summary>
        private SortedList<string, ISerializer> availableSerializers;

        /// <summary>
        /// Contrôleurs créés.
        /// </summary>
        private SortedList<string, IDocumentController> controllers;

        #endregion

        #region constructeur et singleton

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        private DocumentManager()
        {
            InitializeComponent();

            RegistryExtended.MergeInMainMenu(menu);
            RegistryExtended.MergeInMainTools(tools);
            menu.Dispose();

            availableViews = new SortedList<string, Type>();
            availableSerializers = new SortedList<string, ISerializer>();
            managedViews = new SortedList<int, int>();
            controllers = new SortedList<string, IDocumentController>();
        }

        /// <summary>
        /// Unique instance de la classe (singleton)
        /// </summary>
        public static DocumentManager Instance
        {
            get
            {
                if (instance == null) 
                    instance = new DocumentManager();
                return instance;
            }
        }
        #endregion

        #region gestion du document

        /// <summary>
        /// Création d'un document vierge
        /// </summary>
        /// <param name="sender">
        /// Objet à l'origine de l'appel
        /// </param>
        /// <param name="e">
        /// Descripteur correspondant aux paramètres de l'opération
        /// </param>
        public void CreateDocument(object sender, EventArgs e)
        {              
            DocumentModel model = new DocumentModel();
            DocumentController controller = new DocumentController(model);                  
            ToolStripMenuItem viewSelected = sender as ToolStripMenuItem;
            Control view = CreateView(viewSelected, controller);
            AddView(view, "Nouveau");          
        }
        

        /// <summary>
        /// Ouverture d'un document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OpenDocument(object sender, EventArgs e)
        {
            filechooser.Filter = Buildfilter() + "|All files (*.*)|*.*";
            filechooser.FileName = "";
            DialogResult result = filechooser.ShowDialog();
            if (result != DialogResult.OK)
                return;

            string filename = Path.GetFileNameWithoutExtension(filechooser.FileName);            
                            
            ToolStripMenuItem viewSelected = sender as ToolStripMenuItem;
            IDocumentController controller = FindDocumentController(filename);
            if (controller == null)
            {
                controller = new DocumentController(LoadModel(filechooser.FileName));
                controllers.Add(filename, controller);
            }
            controller.DocumentName = filechooser.FileName;
            Control view = CreateView(viewSelected, controller);
            AddView(view, filename);
        }

        public void AskForSave(string documentName, IDocumentController controller)
        {
            string message = null;
            bool isNewFile = documentName.Length == 0;
            if (isNewFile)
                message = "Le fichier "+documentName+" a été modifié. Voulez-vous le sauvegarder?";
            else
                message = "Le fichier n'a pas été sauvegardé. Voulez-vous le sauvegarder?";

            DialogResult result = MessageBox.Show(message, "Enregistrement", MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Yes:
                    if (isNewFile)
                        SaveDocumentAs(controller);
                    else
                        SaveDocument(documentName, controller);
                    break;
                case DialogResult.No:
                    break;

                case DialogResult.Cancel:
                    return;
            }
            Command command = new Command(MCommandType.ConfirmCloseView, null);
            string file = Path.GetFileNameWithoutExtension(documentName);
            controller.ReceiveCommand(command);
        }

        /// <summary>
        /// Sauvegarde le document
        /// </summary>
        public void SaveDocument(string documentName, IDocumentController controller)
        {
            string fileName = ViewInfos.getCurrentViewController().DocumentName;
            if (fileName.Length == 0)
            {
                SaveDocumentAs(controller);
                return;
            }
            Command command = new Command(MCommandType.PerformSave, null);
            ViewInfos.getCurrentViewController().ReceiveCommand(command);
        }

        /// <summary>
        /// Sauvegarde le document dans un nom de fichier spécifié
        /// </summary>
        public void SaveDocumentAs(IDocumentController controller)
        {
            filesaver.Filter = Buildfilter();         

            DialogResult result = filesaver.ShowDialog();
            if (result != DialogResult.OK)
                return;
            
            string extension = this.GetExtension(Path.GetExtension(filesaver.FileName));
            if (!availableSerializers.ContainsKey(extension))
                throw new ArgumentException();
            
            String[] arguments = { filesaver.FileName, extension };
            Command command = new Command(MCommandType.PerformSaveAs, arguments);
            controller.ReceiveCommand(command);

            viewList.SelectedTab.Text = Path.GetFileNameWithoutExtension(filesaver.FileName);
        }

        /// <summary>
        /// Sauvegarde tous les documents ouverts
        /// </summary>
        public void SaveAllDocuments()
        {
            //Parcourir tous les documents ouverts et les sauvegarder.
            //Evenements pour connaitre la liste des documents ouverts? Liste de documents?
        }

        /// <summary>
        /// Ferme la vue courante
        /// </summary>
        public void CloseView()
        {
            string viewId = ViewInfos.getCurrentView().Id.ToString();
            String[] arguments = { viewId };
            Command command = new Command(MCommandType.PerformCloseView, arguments);
            ViewInfos.getCurrentViewController().ReceiveCommand(command);
        }

        /// <summary>
        /// Ferme l'onglet de la vue dont l'id est passé en paramètre
        /// </summary>
        /// <param name="viewId">id de la vue</param>
        public void CloseTabView(int viewId)
        {
            viewList.TabPages.RemoveAt(managedViews[viewId]);
        }

        /// <summary>
        /// Ferme toutes les vues du document sélectionné
        /// </summary>
        public void CloseDocument()
        {
            //Demander le choix d'un sérialiseur dans une messagebox contenant une combobox
            string documentname = viewList.SelectedTab.Text;

            //remplacer le availableSerializers["xml"].SimpleName par le choix de l'utilisateur dans la combobox
            //mentionnée ci avant
            string[] arguments = { documentname, availableSerializers["xml"].SimpleName };
            
            Command command = new Command(MCommandType.PerformCloseDocument, arguments);
            ViewInfos.getCurrentViewController().ReceiveCommand(command);
        }

        /// <summary>
        /// Ferme tous les documents ouverts
        /// </summary>
        public void CloseAllDocuments()
        {
            //Parcourir tous les documents ouverts et les fermer.
            //Evenements pour connaitre la liste des documents ouverts? Liste de documents?
        }

        /// <summary>
        /// Coupe un document vers le presse papier
        /// </summary>
        public void CutDocument()
        {
            //Presse papier: quel serialisation? Par défaut ou choix utilisateur?
        }

        /// <summary>
        /// Copie un document vers le presse papier
        /// </summary>
        public void CopyDocument()
        {

        }

        /// <summary>
        /// Colle un document depuis le presse papier
        /// </summary>
        /// <returns></returns>
        public void PasteDocument()
        {

        }

        public void Erase()//???
        {

        }
        #endregion

        #region undo/redo

        /// <summary>
        /// Annule la dernière modification
        /// </summary>
        public void Undo()
        {

        }

        /// <summary>
        /// Fait la dernière opération annulée
        /// </summary>
        public void Redo()
        {

        }
        #endregion

        #region actions 

        private void events_ApplicationIdle(object sender, EventArgs e)
        {
            tools.Visible = bar.IsActive;
            menuFile.Visible = bar.IsActive;
            menuEdition.Visible = bar.IsActive;
            itemNew.Visible = bar.IsActive;
            itemOpen.Visible = bar.IsActive;
            actions.Visible = bar.IsActive;
            IDocumentView view = ViewInfos.getCurrentView();
            bool isViewOpened = view != null;
            
            IDocumentData model = null;
            IDocumentController controller = null;
            if (isViewOpened)
            {
                model = view.Controller.Model;
                controller = view.Controller;
            }

            //open, saveas toujours dispo
            actionSave.Enabled = isViewOpened && controller.IsDocumentChanged;
            actionSaveAs.Enabled = isViewOpened;
            actionSaveAll.Enabled = isViewOpened && controller.IsDocumentChanged;
            actionClose.Enabled = isViewOpened;
            actionCloseDocument.Enabled = isViewOpened;
            actionCloseAll.Enabled = isViewOpened;

            actionCopy.Enabled = isViewOpened && model.CurrentControlName != "";
            actionCut.Enabled = isViewOpened && model.CurrentControlName != "";
            //actionPaste.Enabled = //si le clipboard est plein
            
            actionUndo.Enabled = isViewOpened && controller.IsDocumentChanged;
            actionRedo.Enabled = isViewOpened && controller.IsDocumentChanged;
            

        }     

        private void actionSaveAs_Execute(object sender, EventArgs e)
        {
            IDocumentController controller = ViewInfos.getCurrentViewController();
            SaveDocumentAs(controller);
        }

        private void actionSave_Execute(object sender, EventArgs e)
        {
            string documentName = ViewInfos.getCurrentViewController().DocumentName;
            IDocumentController controller = ViewInfos.getCurrentViewController();
            SaveDocument(documentName, controller);
        }

        private void actionSaveAll_Execute(object sender, EventArgs e)
        {
            SaveAllDocuments();
        }

        private void actionClose_Execute(object sender, EventArgs e)
        {
            CloseView();
        }

        private void actionCloseAll_Execute(object sender, EventArgs e)
        {
            CloseAllDocuments();
        }

        private void actionCut_Execute(object sender, EventArgs e)
        {
            CutDocument();
        }

        private void actionPaste_Execute(object sender, EventArgs e)
        {
            PasteDocument();
        }

        private void actionCopy_Execute(object sender, EventArgs e)
        {
            CopyDocument();
        }

        private void actionUndo_Execute(object sender, EventArgs e)
        {
            Undo();
        }

        private void actionRedo_Execute(object sender, EventArgs e)
        {
            Redo();
        }

        private void filechooser_FileOk(object sender, CancelEventArgs e)
        {
            String extension = Path.GetExtension(filechooser.FileName);
            if (availableSerializers[extension] == null)
            {
                MessageBox.Show("Extension inconnue");
                return;
            }
            //String[] arguments = { filechooser.FileName, extension };
            //Command command = new Command(MCommandType.PerformSaveAs, arguments);
            //ViewInfos.getCurrentViewController().ReceiveCommand(command);

            DocumentModel model = new DocumentModel();
            DocumentController controller = new DocumentController(model, filechooser.FileName + "." + extension);
        }

        private void filesaver_FileOk(object sender, CancelEventArgs e)
        {
            string tempExtension = Path.GetExtension(filesaver.FileName);
            String extension = tempExtension.Substring(1, tempExtension.Length - 1);
            MessageBox.Show(extension);
            if (availableSerializers[extension] == null)
            {
                MessageBox.Show("Extension inconnue");
                return;
            }
            String[] arguments = { filechooser.FileName, extension };
            Command command = new Command(MCommandType.PerformSaveAs, arguments);
            ViewInfos.getCurrentViewController().ReceiveCommand(command);

            /*DocumentModel model = new DocumentModel();
            DocumentController controller = new DocumentController(model, filechooser.FileName + "." + extension);
            controller.NoMoreViews += new EventHandler<EventArgs>(newController_NoMoreViews);*/
        }

        #endregion        

        #region enregistrement des plugins chargés dynamiquement

        /// <summary>
        /// Enregistre un plugin pour une vue donnée.
        /// </summary>
        /// <param name="viewType">
        /// Type des vues gérées par le plugin.
        /// </param>
        /// <param name="viewName">
        /// Nom associé à ce type de vues.
        /// </param>
        public void RegisterViewPlugin(Type viewType, String viewName)
        {
            bool isUserControl = false;
            bool isIDocumentView = false;
            foreach (Type inter in viewType.GetInterfaces())
            {
                isIDocumentView = isIDocumentView || inter == typeof(IDocumentView);
                if (isIDocumentView)
                    break;
            }

            isUserControl = viewType.IsSubclassOf(typeof(UserControl));
            if (!isUserControl || !isIDocumentView)
                return;

            
            itemNew.DropDownItems.Add(viewName, null, new EventHandler(CreateDocument));
            itemOpen.DropDownItems.Add(viewName, null, new EventHandler(OpenDocument));

            availableViews.Add(viewName, viewType);
        }

        /// <summary>
        /// Enregistre un plugin de sérialisation.
        /// </summary>
        /// <param name="serializer">
        /// Sérialiseur à ajouter.
        /// </param>
        public void RegisterSerializerPlugin(ISerializer serializer)
        {
            availableSerializers.Add(serializer.Extension, serializer);
        }
        #endregion
            
       
        
        /// <summary>
        /// Envoi d'un message correspondant à la commande demandée et transmission à la vue courante pour son exécution.
        /// </summary>
        /// <param name="message">
        /// Description de la commande à réaliser.
        /// </param>
        public void SendCommand(Command message)
        {
            switch (message.Type)
            {
                case MCommandType.CantPerformSave:
                    Command command = new Command(MCommandType.PerformSaveAs, message.Arguments);
                    ViewInfos.getCurrentViewController().ReceiveCommand(command);
                    break;               
            }
        }

        /// <summary>
        /// Obtient le sérialiseur identifié par son extension
        /// </summary>
        /// <param name="extension">
        /// Extension du sérialiseur
        /// </param>
        /// <returns>
        /// Sérialiseur correspondant à l'extension s'il existe.
        /// </returns>
        public ISerializer GetSerializer(string extension)
        {
            if (!availableSerializers.ContainsKey(extension))
                throw new ArgumentException();

            return availableSerializers[extension];
        }

        #region méthodes utilitaires

        /// <summary>
        /// Crée une vue
        /// </summary>
        /// <param name="viewSelected">
        /// Item sur lequel a cliqué l'utilisateur et correspondant à la vue désirée.
        /// </param>
        /// <param name="controller">
        /// Contrôleur à laquelle ajouter la vue.
        /// </param>
        /// <returns></returns>
        private Control CreateView(ToolStripMenuItem viewSelected, IDocumentController controller)
        {
            if (viewSelected == null)
                throw new InvalidCastException();

            if(!availableViews.ContainsKey(viewSelected.Text))
                throw new ArgumentException("Type de vue non géré par l'application");

            int id = GenerateID();
            Type viewType = availableViews[viewSelected.Text];
            Type[] paramsTypes = { typeof(DocumentController), typeof(int) };
            Object[] param = { controller, id };
            ConstructorInfo CInfo = viewType.GetConstructor(paramsTypes);

            Control view = (Control)CInfo.Invoke(param);
            IDocumentView iview = view as IDocumentView;

            controller.AddView(iview);
            view.Dock = DockStyle.Fill;            
            return view;
        }


        /// <summary>
        /// Ajoute une vue au tableau à onglets.
        /// </summary>
        /// <param name="view">
        /// Vue à ajoutée au tableau.
        /// </param>
        /// <param name="documentName">
        /// Nom du document
        /// </param>
        /// 
        private void AddView(Control view, string documentName)
        {
            viewList.TabPages.Add(documentName);
            int currentIndexTab = viewList.TabCount-1;
            int viewId = ((IDocumentView)view).Id;
            managedViews.Add(viewId, currentIndexTab);
            viewList.TabPages[currentIndexTab].Controls.Add(view);
            viewList.SelectedTab = viewList.TabPages[currentIndexTab];
        }

        /// <summary>
        /// Génère un id unique pour les vues
        /// </summary>
        /// <returns>
        /// ID unique
        /// </returns>
        private int GenerateID()
        {
            return id++;            
        }

        /// <summary>
        /// Construit un filtre contenant les extensions correspondant aux sérialiseurs connus.
        /// </summary>
        /// <returns>
        /// Les extensions gérées par l'application.
        /// </returns>
        private string Buildfilter()
        {
            string filter = "";
            foreach (ISerializer serializer in availableSerializers.Values)
            {
                filter += serializer.SimpleName;
                filter += "(*." + serializer.Extension + ")";
                filter += "|*." + serializer.Extension + "|";
            }          
            return filter.TrimEnd('|');
        }

        /// <summary>
        /// Obtient l'extension d'un fichier à partir de son nom
        /// </summary>
        /// <param name="name">
        /// Nom du fichier
        /// </param>
        /// <returns>
        /// Extension du fichier
        /// </returns>
        private string GetExtension(string name)
        {
            string choosedextension = Path.GetExtension(name);
            string extension = choosedextension.Substring(1, choosedextension.Length - 1);
            return extension;
        }


        /// <summary>
        /// Récupère le contrôleur correspondant au document ouvert (au sens fichier sur disque)
        /// // possible conflit avec la propriété documentname du contrôleur. on peut remplacer cet
        /// "algo" par une recherche dans tous les contrôleurs connus du documentname.
        /// </summary>
        /// <returns>
        /// Le contrôleur associé au document s'il existe, et un nouveau contrôleur sinon
        /// </returns>
        private IDocumentController FindDocumentController(string documentname)
        {            
            return controllers.ContainsKey(documentname) ? controllers[documentname] : null;
        }      

        /// <summary>
        /// Crée un modèle depuis un fichier.
        /// </summary>
        /// <param name="filename">
        /// Nom du fichier depuis lequel on charge les données du modèle (avec son extension)
        /// </param>    
        /// <returns>
        /// Modèle contenu dans le fichier.
        /// </returns>
        private IDocumentData LoadModel(string filename)
        {            
            String extension = GetExtension(filename);
            IDocumentData res =  FileSerializer.Load(filename, GetSerializer(extension));
            MessageBox.Show(res.ControlsDictionary.Count().ToString());

            return res;
        }

        #endregion

    }
}
