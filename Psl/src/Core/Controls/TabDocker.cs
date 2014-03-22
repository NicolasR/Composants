/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 06 12 2008 : version initiale
 * 05 04 2009 : adjonction de la méthode ForceShowAndSelect
 * 08 05 2010 : adjonction de la propriété AutoRightSelect et de la sélection d'onglet sur clic droit
 * 17 05 2010 : amélioration de la méthode DoPageMove pour éviter la scintillation des onglets
 * 17 05 2010 : adjonction protocole drag-drop pour réorganiser les onglets
 * 15 10 2010 : correction de l'initialisation incorrecte de la propriété AutoRightSelect
 * 15 10 2010 : adjonction du déclenchement de Selected et SelectedIndexChanged lors de l'adjonction d'un premier onglet
 * 26 11 2010 : amélioration de l'activation du client même si le client n'est pas focusable
 * 30 11 2010 : amélioration de l'activation du client, y compris dans le cas cas d'un client non focusable créé comme premier onglet (cf. OnControlAdded)
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Psl.Windows;

namespace Psl.Controls {

  /// <summary>
  /// Contrôle conteneur de pages muni d'une gestion semi-automatique des pages.
  /// </summary>
  [
    Designer( typeof( Psl.Design.TabDockerDesigner ) ), // Designer   ( "Psl.Design.TabDockerDesigner, " + Psl.AssemblyRef.PslCoreDesign ),
    Description( "Conteneur de pages muni d'une gestion semi-automatique des pages" )
  ]
  public partial class TabDocker : TabControl {

    //
    // Champs
    //

    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    //
    // Gestion générale
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public TabDocker() {
      InitializeComponent();
      ItemSize = new Size( 50, 21 );
      Padding = new Point( 4, 4 );
      ShowToolTips = true;
      AutoFocusClient = true;
      AutoRightSelect = true;
    }

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
    }

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    //
    // Service
    //

    /// <summary>
    /// Obtient l'index de l'onglet incluant une corrdonnée client donnée.
    /// </summary>
    /// <param name="x">coordonnée client en X</param>
    /// <param name="y">coordonnée client en Y</param>
    /// <returns>l'index de l'onglet ou -1 si introuvable</returns>
    private int DoGetTabIndexAt( int x, int y ) {
      for ( int ix = 0 ; ix < TabCount ; ix++ )
        if ( GetTabRect( ix ).Contains( x, y ) ) return ix;
      return -1;
    }

    /// <summary>
    /// Déclenche les événements <see cref="TabControl.Selected"/> et <see cref="TabControl.SelectedIndexChanged"/>.
    /// </summary>
    /// <remarks>
    /// Cette méthode est destinée à déclencher les deux événements <see cref="TabControl.Selected"/> 
    /// et <see cref="TabControl.SelectedIndexChanged"/> lors de la création du premier onglet : 
    /// aucun de ces deux événements n'est déclenché par le composant <see cref="TabControl"/>.
    /// </remarks>
    protected void DoFireSelectEvents() {
      OnSelected( new TabControlEventArgs( SelectedTab, SelectedIndex, TabControlAction.Selected ) );
      OnSelectedIndexChanged( EventArgs.Empty );
    }

    /// <summary>
    /// Insère la page à la position spécifiée par index
    /// </summary>
    /// <remarks>
    /// Cette méthode est requise parce que le composant <see cref="TabControl"/> n'assume pas correctement
    /// cette fonctionnalité : les différentes méthodes Insert de TabPageCollection ne fonctionnent pas
    /// correctement lorsque le contrôleur de pages n'est pas visible. 
    /// <br/>
    /// Cette méthode aura en outre pour effet de déclencher les deux événements <see cref="TabControl.Selected"/> 
    /// et <see cref="TabControl.SelectedIndexChanged"/> lors de l'adjonction d'un onglet quand la collection des onglets
    /// est vide : aucun de ces deux événements n'est déclenché par le composant <see cref="TabControl"/>.
    /// </remarks>
    /// <param name="index">position d'insertion</param>
    /// <param name="page">page à insérer</param>
    /// <param name="select">si true, sélectionne la page insérée</param>
    /// <returns>la référence sur la page insérée</returns>
    protected TabDockerPage DoPageInsert( int index, TabDockerPage page, bool select ) {

      // validation de la page
      if ( page == null ) throw new ArgumentNullException( "page", "La référence sur la page à ajouter ou à insérer ne peut être null" );

      // Insertion en fin : simplement ajouter la page
      if ( TabCount == 0 || index < 0 || index >= TabCount ) {
        TabPages.Add( page );
        if ( select ) SelectedDockerTab = page;
        //if ( TabCount == 1 ) DoFireSelectEvents(); // déclencher Selected et SelectedIndexChanged // cf. OnControlAdded (30 11 2010)
        return page;
      }

      // mémoriser l'onglet sélectionné
      TabPage selected = SelectedTab;

      // nouvelles pages
      TabPage[] pages = new TabPage[ TabPages.Count + 1 ];

      // récupérer les pages figurant avant la position d'insertion
      for ( int ix = 0 ; ix < index ; ix++ )
        pages[ ix ] = TabPages[ ix ];

      // placer la nouvelle page
      pages[ index ] = page;

      // récupérer les pages figurant après la position d'insertion
      for ( int ix = index ; ix < TabPages.Count ; ix++ )
        pages[ ix + 1 ] = TabPages[ ix ];

      // remplacer les pages
      TabPages.Clear();
      TabPages.AddRange( pages );

      // restaurer ou modifier la sélection
      SelectedTab = select ? page : selected;

      // retourner la page elle-même
      return page;
    }

    /// <summary>
    /// Déplace une page à l'intérieur de la collection des pages
    /// </summary>
    /// <remarks>
    /// Il se produit des effets de scintillation des onglets quand on retire puis réinsère les
    /// pages, parce que le composant TabControl resélectionne l'onglet 0 dès qu'une page est 
    /// ajoutée ou retirée (via Add ou Remove).
    /// <br/>
    /// Pour éviter ces effets désagréables, il est possible d'agir directement sur la collection
    /// des pages en réordonnant cette table puis en la réinstallant. 
    /// </remarks>
    /// <param name="index">index de la page à déplacer</param>
    /// <param name="target">index de l'insertion</param>
    protected void DoPageMove( int index, int target ) {
      TabPage[] oldPages = new TabPage[ TabCount ];
      TabPage[] newPages = new TabPage[ TabCount ];

      // récupérer la table actuelle des pages
      for ( int ix = 0 ; ix < oldPages.Length ; ix++ )
        oldPages[ ix ] = TabPages[ ix ];

      // composer le tableau du nouvel ordre des pages
      if ( index < target ) {
        Array.Copy( oldPages, 0         , newPages, 0         , index - 0 );
        Array.Copy( oldPages, index  + 1, newPages, index     , target - index );
        Array.Copy( oldPages, target + 1, newPages, target + 1, oldPages.Length - target - 1 );
      }
      else {
        Array.Copy( oldPages, 0         , newPages, 0         , target - 0 );
        Array.Copy( oldPages, target    , newPages, target + 1, index - target );
        Array.Copy( oldPages, index + 1 , newPages, index  + 1, oldPages.Length - index - 1 );
      }

      // déplacer la source en position cible
      newPages[ target ] = oldPages[ index ];

      // installer la table des nouvelles pages
      for ( int ix = 0 ; ix < newPages.Length ; ix++ )
        TabPages[ ix ] = newPages[ ix ];

      // resélectionner la cible
      SelectedIndex = target;
    }
    
    /// <summary>
    /// Adjonction d'un contrôle à docker
    /// </summary>
    /// <param name="page">page à ajouter</param>
    /// <param name="tabIndex">position d'insertion de l'onglet</param>
    /// <param name="client">contrôle à docker</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image associée à l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <returns>la référence sur la page créée</returns>
    private TabDockerPage DoAddClient( TabDockerPage page, int tabIndex, Control client, string text, object imageOrIndexOrKey, bool select ) {
     
      // préparer la nouvelle page
      page.Text = text;
      page.Client = client;
      client.Dock = DockStyle.Fill;

      // insérer la nouvelle page
      DoPageInsert( tabIndex, page, select );

      // gestion de l'image
      if ( imageOrIndexOrKey != null ) {
        if ( imageOrIndexOrKey is Image ) {
          if ( ImageList != null ) {
            ImageList.Images.Add( imageOrIndexOrKey as Image );
            page.ImageIndex = ImageList.Images.Count - 1;
          }
        }
        else if ( imageOrIndexOrKey is Icon ) {
          if ( ImageList != null ) {
            ImageList.Images.Add( imageOrIndexOrKey as Icon );
            page.ImageIndex = ImageList.Images.Count - 1;
          }
        }
        else if ( imageOrIndexOrKey is string ) {
          string imageKey = imageOrIndexOrKey as string;
          if ( !string.IsNullOrEmpty( imageKey ) ) page.ImageKey = imageKey;
        }
        else {
          int imageIndex = -1;
          try {
            imageIndex = (int) imageOrIndexOrKey;
          }
          catch ( Exception x ) {
            throw new ArgumentException( "L'argument doit être de type Image, Icon, string ou int", "imageOrIndexOrKey", x );
          }
          if ( imageIndex != -1 ) page.ImageIndex = imageIndex;
        }
      }

      // donner le focus au client si nécessaire
      if ( select && AutoFocusClient ) page.ActivatePage();

      // retourner la page créée
      return page;
    }

    /// <summary>
    /// Adjonction d'un contrôle à docker
    /// </summary>
    /// <param name="tabIndex">position d'insertion de l'onglet</param>
    /// <param name="client">contrôle à docker</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image associée à l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <returns>la référence sur la page créée</returns>
    private TabDockerPage DoAddClient( int tabIndex, Control client, string text, object imageOrIndexOrKey, bool select ) {
      return DoAddClient( new TabDockerPage(), tabIndex, client, text, imageOrIndexOrKey, select );
    }

    //
    // Redéfinition de méthodes héritées pour la gestion des onglets
    // Voir le calque drag-drop pour la redéfinition des méthodes hérités souris/drag-drop
    //

    /// <summary>
    /// Surveille les pages ajoutées pour refuser les contrôles qui ne sont pas <see cref="TabDockerPage"/>.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnControlAdded( ControlEventArgs e ) {
      base.OnControlAdded( e );

      // filtrer les pages qui ne seraient pas des TabDockerPage
      if ( !( e.Control is TabDockerPage ) ) {
        Controls.Remove( e.Control );
        throw new InvalidCastException( "Un contrôle TabDocker ne peut héberger que des pages de type TabDockerPage" );
      }

      // prendre en charge le cas de la création du premier onglet (30 11 2010)
      // TabControl ne déclenche pas les événements Selected et SelectedIndexChanged
      if ( TabCount == 1 ) DoFireSelectEvents();
    }

    /// <summary>
    /// Surveille la suppression des pages pour ajuster la page sélectionnée après une suppression.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnControlRemoved( ControlEventArgs e ) {
      int index = SelectedIndex;
      base.OnControlRemoved( e );
      index = index >= Controls.Count ? Controls.Count - 1 : index;
      if ( index != -1 ) SelectedTab = Controls[ index ] as TabPage;
    }

    ///// <summary>
    ///// Surveille le changement de page sélectionnée pour sélectionner automatiquement le contrôle <see cref="TabDockerPage.Client"/>
    ///// </summary>
    ///// <param name="e">descripteur de l'événement</param>
    //protected override void OnSelected( TabControlEventArgs e ) {  // débrayée le 26 11 2010, remplacée par OnSelectedIndexChanged
    //  TabDockerPage page = e.TabPage as TabDockerPage;
    //  if ( AutoFocusClient && page != null ) page.ActivatePage();
    //  base.OnSelected( e );
    //}

    /// <summary>
    /// Surveille le changement de page sélectionnée pour sélectionner automatiquement le contrôle <see cref="TabDockerPage.Client"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnSelectedIndexChanged( EventArgs e ) {
      TabDockerPage page = SelectedDockerTab;
      if ( AutoFocusClient && page != null ) page.ActivatePage();
      base.OnSelectedIndexChanged( e );
    }

    /// <summary>
    /// Surveille l'acquisition du focus pour transmettre le focus à la page sélectionnée.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnGotFocus( EventArgs e ) {
      base.OnGotFocus( e );
      TabDockerPage page = SelectedDockerTab;
      if ( AutoFocusClient && page != null ) page.ActivatePage();
    }

    //
    // Propriétés
    //

    /// <summary>
    /// Collection des pages.
    /// </summary>
    /// <remarks>
    /// La redéclaration de cette propriété est seulement destinée à l'associer à son éditeur en mode conception.
    /// </remarks>
    [
      Browsable( true ),
      MergableProperty(false),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Editor( typeof( Psl.Design.TabDockerCollectionEditor ), typeof( UITypeEditor ) ), // Editor( "Psl.Design.TabDockerCollectionEditor, " + AssemblyRef.PslCoreDesign, typeof( UITypeEditor ) ), 
      Category( "Behavior" ),
      Description( "Liste des onglets" )
    ]
    public new TabPageCollection TabPages {
      get {
        return base.TabPages;
      }
    }

    /// <summary>
    /// Obtient ou détermine la page <see cref="TabDockerPage"/> sélectionnée.
    /// </summary>
    /// <remarks>
    /// La déclaration de cette propriété, qui opère sur les mêmes valeurs que <see cref="TabControl.SelectedTab"/> 
    /// permet simplement de bénéficier d'un typage <see cref="TabDockerPage"/>
    /// </remarks>
    [
      Browsable(false), 
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
    ]
    public TabDockerPage SelectedDockerTab {
      get { return base.SelectedTab as TabDockerPage; }
      set { base.SelectedTab = value; }
    }

    /// <summary>
    /// Obtient le client de la page <see cref="TabDockerPage"/> sélectionnée.
    /// </summary>
    [
      Browsable(false), 
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
    ]
    public Control SelectedDockerClient {
      get { return SelectedDockerTab == null ? null : SelectedDockerTab.Client; }
    }

    /// <summary>
    /// Obtient ou détermine si le <see cref="TabDockerPage.Client"/> obtient automatiquement le focus lors d'un changement de page.
    /// </summary>
    [
      Browsable(true),
      DefaultValue( true ),
      Category( "Focus" ),
      Description( "Transfère automatiquement le focus au contrôle client lors d'un changement d'onglet" )
    ]
    public bool AutoFocusClient { get ; set ; }

    //
    // Fonctionnalités liées aux pages seulement
    //

    /// <summary>
    /// Obtient l'index d'un onglet à partir de coordonnées client
    /// </summary>
    /// <param name="location">coordonnées client</param>
    /// <returns>l'index d'un onglet ou -1 si aucun onglet ne correspond à l'emplacement</returns>
    public int TabIndexFromLocation( Point location ) {
      return DoGetTabIndexAt( location.X, location.Y );
    }

    /// <summary>
    /// Obtient l'index d'un onglet à partir de coordonnées client
    /// </summary>
    /// <param name="X">coordonnée client en X</param>
    /// <param name="Y">coordonnée client en Y</param>
    /// <returns>l'index d'un onglet ou -1 si aucun onglet ne correspond à l'emplacement</returns>
    public int TabIndexFromLocation( int X, int Y ) {
      return DoGetTabIndexAt( X, Y );
    }

    /// <summary>
    /// Déplace une page à l'intérieur de la collection des pages
    /// </summary>
    /// <param name="index">index de l'onglet à déplacer</param>
    /// <param name="target">index cible du déplacement</param>
    public void PageMove( int index, int target ) {
      DoPageMove( index, target );
    }

    /// <summary>
    /// Insère une page à la position index.
    /// </summary>
    /// <remarks>
    /// Permet de contourner le fonctionnement incorrect de <see cref="TabControl.TabPageCollection.Insert(int,TabPage)"/>.
    /// </remarks>
    /// <param name="index">index d'insertion de la page</param>
    /// <param name="page">page à insérer</param>
    /// <param name="select">si true, sélectionne la page insérée</param>
    /// <returns>la page insérée (valeur de l'argument page)</returns>
    public TabDockerPage PageInsert( int index, TabDockerPage page, bool select ) {
      return DoPageInsert( index, page, select );
    }

    /// <summary>
    /// Insère une page après la page actuellement sélectionnée.
    /// </summary>
    /// <remarks>
    /// Permet de contourner le fonctionnement incorrect de <see cref="TabControl.TabPageCollection.Insert(int,TabPage)"/>.
    /// </remarks>
    /// <param name="page">page à insérer</param>
    /// <param name="select">si true, sélectionne la page insérée</param>
    /// <returns>la page insérée (valeur de l'argument page)</returns>
    public TabDockerPage PageInsert( TabDockerPage page, bool select ) {
      return DoPageInsert( SelectedIndex + 1, page, select );
    }

    /// <summary>
    /// Ajoute une page après la dernière page.
    /// </summary>
    /// <param name="page">page à ajouter</param>
    /// <param name="select">si true, sélectionne la page ajoutée</param>
    /// <returns>la page insérée (valeur de l'argument page)</returns>
    public TabDockerPage PageAdd( TabDockerPage page, bool select ) {
      return DoPageInsert( -1, page, select );
    }

    /// <summary>
    /// Force un contrôle et tous ses conteneurs à être visibles, puis sélectionne le contrôle.
    /// </summary>
    /// <remarks>
    /// Cette méthode prend en charge tous les classeurs à onglets qui sont les conteneurs
    /// directs ou indirects du contrôle de manière à forcer la sélection de la page concernée
    /// au sein de chaque classeur à onglets.
    /// </remarks>
    /// <param name="control">contrôle à rendre effectivement visible et sélectionné</param>
    public static void ForceShowAndSelect( Control control ) {

      // contrôle de l'argument : ignorer si null
      if ( control == null ) return;

      // remontée récursive frontale jusqu'au conteneur top-level
      Control parent = control.Parent;
      if ( parent != null ) ForceShowAndSelect( parent );

      // descente de la récursion : traiter le cas des pages des classeurs à onglets
      TabPage controlAsTabPage = control as TabPage;
      TabControl parentAsTabControl = parent as TabControl;

      // le contrôle n'est pas une page, ou n'est pas hébergé par un classeur à onglets
      if ( controlAsTabPage == null || parentAsTabControl == null ) {
        control.Show();
        control.Select();
        return;
      }

      // le contrôle est une page hébergée par un classeur à onglets : sélectionner la page
      parentAsTabControl.SelectedTab = controlAsTabPage;
      control.Select();
    }

    //
    // Fonctionnalités liées aux clients des pages
    //

    /// <summary>
    /// Création et insertion d'une page <see cref="TabDockerPage"/> pour héberger un contrôle client.
    /// </summary>
    /// <remarks>
    /// La gestion de l'image n'est prise en charge que si le contrôle <see cref="TabDocker"/>
    /// est associé à une liste d'images via la propriété <see cref="ImageList"/>.
    /// <br/>
    /// Si l'image est déterminée par un index ou par une clé, l'image doit déjà figurer dans la liste d'images <see cref="ImageList"/>.
    /// Si l'image est directement fournie (type <see cref="Image"/>), elle sera automatiquement ajoutée à la liste d'images <see cref="ImageList"/>.
    /// </remarks>
    /// <param name="index">index d'insertion de la page à créer, ou -1 pour adjonction en fin</param>
    /// <param name="client">contrôle client à héberger</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image à associer à l'onglet</param>
    /// <returns>la page créée pour héberger le contrôle client</returns>
    public TabDockerPage ClientInsert( int index, Control client, string text, object imageOrIndexOrKey, bool select ) {
      return DoAddClient( index, client, text, imageOrIndexOrKey, select );
    }

    /// <summary>
    /// Création et insertion d'une page <see cref="TabDockerPage"/> pour héberger un contrôle client.
    /// </summary>
    /// <remarks>
    /// La gestion de l'image n'est prise en charge que si le contrôle <see cref="TabDocker"/>
    /// est associé à une liste d'images via la propriété <see cref="ImageList"/>.
    /// <br/>
    /// Si l'image est déterminée par un index ou par une clé, l'image doit déjà figurer dans la liste d'images <see cref="ImageList"/>.
    /// Si l'image est directement fournie (type <see cref="Image"/>), elle sera automatiquement ajoutée à la liste d'images <see cref="ImageList"/>.
    /// </remarks>
    /// <param name="client">contrôle client à héberger</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image à associer à l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <returns>la page créée pour héberger le contrôle client</returns>
    public TabDockerPage ClientInsert( Control client, string text, object imageOrIndexOrKey, bool select ) {
      return DoAddClient( SelectedIndex + 1, client, text, imageOrIndexOrKey, select );
    }

    /// <summary>
    /// Création et insertion d'une page <see cref="TabDockerPage"/> pour héberger un contrôle client.
    /// </summary>
    /// <remarks>
    /// La gestion de l'image n'est prise en charge que si le contrôle <see cref="TabDocker"/>
    /// est associé à une liste d'images via la propriété <see cref="ImageList"/>.
    /// <br/>
    /// Si l'image est déterminée par un index ou par une clé, l'image doit déjà figurer dans la liste d'images <see cref="ImageList"/>.
    /// Si l'image est directement fournie (type <see cref="Image"/>), elle sera automatiquement ajoutée à la liste d'images <see cref="ImageList"/>.
    /// </remarks>
    /// <param name="client">contrôle client à héberger</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image à associer à l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <returns>la page créée pour héberger le contrôle client</returns>
    public TabDockerPage ClientAdd( Control client, string text, object imageOrIndexOrKey, bool select ) {
      return DoAddClient( -1, client, text, imageOrIndexOrKey, select );
    }

    /// <summary>
    /// Insertion d'une page <see cref="TabDockerPage"/> pour héberger un contrôle client.
    /// </summary>
    /// <remarks>
    /// La gestion de l'image n'est prise en charge que si le contrôle <see cref="TabDocker"/>
    /// est associé à une liste d'images via la propriété <see cref="ImageList"/>.
    /// <br/>
    /// Si l'image est déterminée par un index ou par une clé, l'image doit déjà figurer dans la liste d'images <see cref="ImageList"/>.
    /// Si l'image est directement fournie (type <see cref="Image"/>), elle sera automatiquement ajoutée à la liste d'images <see cref="ImageList"/>.
    /// <br/>
    /// L'argument <paramref name="page"/> permet de fournir une page personnalisée pour héberger le client. 
    /// Cette page doit être une sous-classe de la classe <see cref="TabDockerPage"/>.
    /// </remarks>
    /// <param name="page">page destinée à héberger le client</param>
    /// <param name="client">contrôle client à héberger</param>
    /// <param name="text">libellé de l'onglet</param>
    /// <param name="imageOrIndexOrKey">image ou index d'image ou clé d'image à associer à l'onglet</param>
    /// <param name="select">si true, sélectionne la page créée</param>
    /// <returns>la page créée pour héberger le contrôle client</returns>
    public TabDockerPage ClientAdd( TabDockerPage page, Control client, string text, object imageOrIndexOrKey, bool select ) {
      if ( page == null ) throw new ArgumentNullException( "ClientAdd  : la référence sur la page d'hébergement est null", "page" );
      return DoAddClient( page, -1, client, text, imageOrIndexOrKey, select );
    }
  }
}
