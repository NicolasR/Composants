/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 09 05 2010 : ébauche initiale
 * 17 05 2010 : version initiale finalisée
 * 15 10 2010 : affecter true à AutoDragTabs force AllowDrop à true
 * 13 02 2011 : introduction de la propriété AutoDragStart et implémentation de l'interface d'extension IDragDropExtender
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Psl.DragDrop;
using Psl.Windows;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                Calque du protocole auto drag tab                            //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <remarks>
  /// Le composant TabDocker est muni d'une prise en charge optionnelle du protocole drag-and-drop 
  /// concernant les onglets. Pour mettre en oeuvre cette prise en charge, basculer à true 
  /// la propriété <see cref="TabDocker.AutoDragTabs"/>, ce qui aura automatiquement pour effet de
  /// basculer à trie les propriétés <see cref="AutoDragStart"/> et <see cref="Control.AllowDrop"/>.
  /// <br/><br/>
  /// Le protocole intégré permet de changer l'ordre des onglets d'un même TabDocker par drag-drop
  /// avec le bouton droit ou le bouton gauche de la souris. Aucun code supplémentaire n'est requis.
  /// <br/><br/>
  /// Ce protocole intégré permet en outre de gérer le protocole standard drag-drop dans le cas où
  /// l'utilisateur glisse un onglet en-dehors de la zone des onglets du TabDocker auquel appartient 
  /// l'onglet glissé. Une telle prise en charge (drag-drop "externe") requiert le code de prise en
  /// charge du protocole drag-drop pour ces opérations "externes". 
  /// <br/><br/>
  /// Remarque : le protocole automatique de drag-drop "interne" est préemptif en ce sens qu'il "absorbe"
  /// tous les événements du protocole drag-drop tant que l'onglet est glissé au-dessus de la zone des
  /// onglets du même composant TabDocker. Les événements du protocole drag-drop exposés par le composant
  /// TabDocker ne sont déclenchés que lorsque le glissement de l'onglet est "externe".
  /// <br/><br/>
  /// Lorsque le démarrage du glissement d'un onglet est détecté, le composant TabDocker déclenche
  /// l'événement <see cref="DragStart"/>. Le gestionnaire associé à cet événement peut annuler le
  /// glissement en basculant à true la propriété <see cref="DragStartEventArgs.Cancel"/> du descripteur
  /// de l'événement. Il peut aussi, et surtout, ajouter des formats de données permettant la prise en
  /// charge des glissements "externes" (dépôt sur d'autres composants et/ou sur d'autres applications). 
  /// Cette gestion s'effectuera via le protocole drag-drop standard, c'est-à-dire via les événements
  /// standard <see cref="Control.DragEnter"/>, <see cref="Control.DragOver"/>, <see cref="Control.DragDrop"/>
  /// et <see cref="Control.DragLeave"/>
  /// </remarks>
  public partial class TabDocker : IDragDropExtender {

    //
    // Champs
    //

    /// <summary>
    /// Assistant pour la détection du démarrage d'un drag-drop
    /// </summary>
    private DragWatcher dragWatcher = new DragWatcher();

    /// <summary>
    /// Nom de format pour le drag-drop automatique d'onglets.
    /// </summary>
    /// <remarks>
    /// Ce champ est initialisé par nécessité via le getter de la propriété <see cref="AutoDragTabFormat"/>.
    /// Ne pas utiliser directement ce champ. 
    /// </remarks>
    private string dragFormat__ = string.Empty;

    /// <summary>
    /// Mémorisation de la propriété <see cref="AutoDragStart"/>.
    /// </summary>
    private bool autoDragStart = false;

    /// <summary>
    /// Mémorisation de la propriété <see cref="AutoDragTabs"/>.
    /// </summary>
    private bool autoDragTabs = false;

    //
    // Drag and drop automatique des onglets
    // Protocole léger direct limité au composant lui-même
    //

    /// <summary>
    /// Obtient le nom de format pour le glissement d'onglets appartenant au composant
    /// </summary>
    /// <remarks>
    /// Le nom de format est muni d'un GUID pour que l'instance du TabDocker puisse être
    /// identifiée même dans le cas de deux TabDocker instanciés dans des applications diféfrentes.
    /// </remarks>
    [Browsable( false )]
    public string AutoDragTabFormat {
      get {
        if ( dragFormat__ == string.Empty )
          dragFormat__ = GetType().FullName + ".TabIndex.{" + Guid.NewGuid() + "}";
        return dragFormat__;
      }
    }

    /// <summary>
    /// Démarre une opération glisser-déplacer 
    /// </summary>
    /// <param name="e">descripteur de l'événement souris</param>
    /// <remarks>l'opération effectuée par la cible</remarks>
    /// <returns>l'effet réalisé par la cible du drop</returns>
    protected virtual DragDropEffects DoDragDrop( MouseEventArgs e ) {
      DragDropEffects result = DragDropEffects.None;

      try {
        if ( DesignMode ) return result;

        // construire l'objet transitionnel du drag-drop
        IDataObject dataObject = new DataObject();

        // si prise en charge du drag-drop des onglets, déterminer l'index de l'onglet à glisser
        if ( autoDragTabs ) {
          int draggedIndex = TabIndexFromLocation( dragWatcher.Location );
          if ( draggedIndex < 0 || draggedIndex >= TabCount ) throw new ArgumentOutOfRangeException( "dragTabIndex", draggedIndex, "Impossible de démarrer le drag-drop automatique d'onglet" );
          dataObject.SetData( AutoDragTabFormat, draggedIndex );
        }

        // noter que le dragging commence
        dragWatcher.DragStart();

        // diffuser l'événement DragStart
        DragStartEventArgs args = new DragStartEventArgs( DragDropEffects.Scroll, dataObject, dragWatcher.Button, dragWatcher.Location, false );
        OnDragStart( args );
        if ( args.Cancel || args.AllowedEffects == DragDropEffects.None ) return result;

        // effectuer le protocole drag-drop
        result = DoDragDrop( dataObject, args.AllowedEffects );

        // terminer le protocole drag-drop côté source
        if ( result != DragDropEffects.None ) OnDragTerminate( new DragEventArgs( dataObject, 0, 0, 0, args.AllowedEffects, result ) );
 
      } finally { dragWatcher.DragTerminate(); }
      return result;
    }

    /// <summary>
    /// Détermine l'effet d'un survol de la souris au-dessus du composant
    /// </summary>
    /// <remarks>
    /// L'effet possible est directement affecté dans la propriété <see cref="DragEventArgs.Effect"/>
    /// du descripteur d'événement transmis
    /// </remarks>
    /// <param name="e">descripteur d'événement associé à l'événement requérant la détermination de l'effet</param>
    /// <param name="external">retourne true si le drag-drop est externe, false si le drag-drop est interne</param>
    /// <param name="draggedIndex">retourne l'index de l'onglet en cours de glissement, ou -1</param>
    /// <param name="targetIndex">retourne l'index de l'onglet survolé par la souris, ou -1</param>
    /// <returns>true si une opération préemptive de dépôt est possible</returns>
    private bool SetDragDropEffect( DragEventArgs e, out bool external, out int draggedIndex, out int targetIndex ) {
      
      // résultats par défaut
      draggedIndex = -1;
      targetIndex = -1;
      e.Effect = DragDropEffects.None;

      // drag-drop externe : pas de prise en charge préemptive, laisser faire le protocole standard
      external = !e.Data.GetDataPresent( AutoDragTabFormat );
      if ( external ) return false;

      // drag-drop interne préemptif de glissement des onglets si un onglet cible peut être trouvé
      // récupérer l'index de l'onglet source
      targetIndex = TabIndexFromLocation( PointToClient( new Point( e.X, e.Y ) ) );
      if ( targetIndex == -1 ) {
        external = true;
        return false;
      }

      // récupérer l'index de l'onglet cible, pas de dépôt si source index = cible index
      draggedIndex = (int) e.Data.GetData( AutoDragTabFormat );
      if ( draggedIndex == targetIndex ) return false;

      // bilan du protocole préemptif
      e.Effect = DragDropEffects.Scroll;
      return true;
    }

    /// <summary>
    /// Détermine l'effet d'un survol de la souris au-dessus du composant
    /// </summary>
    /// <remarks>
    /// L'effet possible est directement affecté dans la propriété <see cref="DragEventArgs.Effect"/>
    /// du descripteur d'événement transmis
    /// </remarks>
    /// <param name="e">descripteur d'événement associé à l'événement requérant la détermination de l'effet</param>
    /// <param name="external">retourne true si le drag-drop est externe, false si le drag-drop est interne</param>
    /// <returns>true si une opération préemptive de dépôt est possible</returns>
    private bool SetDragDropEffect( DragEventArgs e, out bool external ) {
      int draggedIndex;
      int targetIndex;
      return SetDragDropEffect( e, out external, out draggedIndex, out targetIndex );
    }

    //
    // Redéfinition de méthodes héritées : événements souris
    //

    /// <summary>
    /// Surveille l'événemement MouseDown pour la détection du démarrage d'un drag-drop d'onglets.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseDown( MouseEventArgs e ) {

      // démarrage éventuel d'un drag drop automatique
      if ( AutoDragStart ) {
        int tabIndex = DoGetTabIndexAt( e.X, e.Y );
        if ( tabIndex != -1 ) dragWatcher.WhenMouseDown( e );
      }

      // corps de méthode hérité
      base.OnMouseDown( e );
    }

    /// <summary>
    /// Surveille l'événement MouseMove pour la détection du démarrage d'un drag-drop.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseMove( MouseEventArgs e ) {
      if ( dragWatcher.WhenMouseMove( e ) ) DoDragDrop( e );
      base.OnMouseMove( e );
    }

    /// <summary>
    /// Surveille l'événement MouseUp pour la sélection sur clic droit et la détection du démarrage d'un drag and drop.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseUp( MouseEventArgs e ) {

      // sélection automatique d'onglet sur clic droit
      if ( AutoRightSelect && e.Button == MouseButtons.Right ) {
        int tabIndex = DoGetTabIndexAt( e.X, e.Y );
        if ( tabIndex != -1 ) SelectedIndex = tabIndex;
      }

      // surveillance du démarrage d'un drag-drop d'onglets
      dragWatcher.WhenMouseUp( e );

      // corps de méthode hérité
      base.OnMouseUp( e );
    }

    /// <summary>
    /// Surveille l'événement MouseLeave pour la détection du démarrage d'un drag and drop.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseLeave( EventArgs e ) {
      dragWatcher.WhenMouseLeave( e );
      base.OnMouseLeave( e );
    }

    //
    // Redéfinition de méthodes héritées : protocole drag and drop
    //

    // todo : améliorer le filtrage sur NCHITTEST pour que la zone libre des tabs réponde
    //protected override void WndProc( ref Message msg ) {
    //  switch ( msg.Msg ) {
    //    case Win.WM_NCHITTEST:
    //      base.WndProc( ref msg );
    //      msg.Result = (IntPtr) 1;
    //      Psl.Tracker.Tracker.Track( "WM_NCHHITTEST : " + msg.Result.ToString( "X" ) );
    //      return;
    //  }
    //  base.WndProc( ref msg );
    //}

    /// <summary>
    /// Surveillance du déclenchement de l'événement <see cref="Control.GiveFeedback"/>.
    /// </summary>
    /// <remarks>
    /// Détermine un curseur pour l'opération <see cref="DragDropEffects.Scroll"/> qui n'est associée à aucun curseur.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnGiveFeedback( GiveFeedbackEventArgs e ) {
      if ( e.Effect == DragDropEffects.Scroll && e.UseDefaultCursors ) {
        e.UseDefaultCursors = false;
        Cursor.Current = Cursors.Hand;
      }
      base.OnGiveFeedback( e );
    }

    /// <summary>
    /// Surveillance du déclenchement de l'événement <see cref="Control.DragEnter"/>.
    /// </summary>
    /// <remarks>
    /// Le protocole interne est préemptif : si l'opération de dépôt est possible,
    /// l'événement <see cref="Control.DragEnter"/> ne sera pas déclenché.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDragEnter( DragEventArgs e ) {
      bool external;
      SetDragDropEffect( e, out external );
      if ( external ) base.OnDragEnter( e );
    }

    /// <summary>
    /// Surveillance du déclenchement de l'événement <see cref="Control.DragOver"/>.
    /// </summary>
    /// <remarks>
    /// Le protocole interne est préemptif : si l'opération de dépôt est possible,
    /// l'événement <see cref="Control.DragOver"/> ne sera pas déclenché.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDragOver( DragEventArgs e ) {
      bool external;
      SetDragDropEffect( e, out external );
      if ( external ) base.OnDragOver( e );
    }

    /// <summary>
    /// Surveillance du déclenchement de l'événement <see cref="Control.DragDrop"/>.
    /// </summary>
    /// <remarks>
    /// Le protocole interne est préemptif : si l'opération de dépôt est possible,
    /// l'événement <see cref="Control.DragDrop"/> ne sera pas déclenché.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDragDrop( DragEventArgs e ) {
      int draggedIndex;
      int targetIndex;
      bool external;

      if ( SetDragDropEffect( e, out external, out draggedIndex, out targetIndex ) ) PageMove( draggedIndex, targetIndex );
      e.Effect = DragDropEffects.None; // pas d'effet de retour pour éviter toute destruction éventuelle dans DragTerminate

      if ( external ) base.OnDragDrop( e );
    }

    /// <summary>
    /// Surveillance du déclenchement de l'événement <see cref="Control.DragLeave"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnDragLeave( EventArgs e ) {
      base.OnDragLeave( e );
    }

    //
    // Déclenchement centralisé des événements
    //

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="DragStart"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnDragStart( DragStartEventArgs e ) {
      if ( DragStart != null ) DragStart( this, e );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="DragTerminate"/>.
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnDragTerminate( DragEventArgs e ) {
      if ( DragTerminate != null ) DragTerminate( this, e );
    }

    //
    // Propriétés et événements
    //

    /// <summary>
    /// Obtient ou détermine si un onglet est sélectionné lorsqu'il est la cible d'un clic souris droit.
    /// </summary>
    [
      Category( "Behavior" ),
      DefaultValue( true ),
      Description( "Obtient ou détermine si un onglet est sélectionné lorsqu'il est la cible d'un clic souris droit" )
    ]
    public bool AutoRightSelect { get; set; }

    /// <summary>
    /// Obtient ou détermine si le contôle prend en charge la détection automatique des opérations drag-drop.
    /// </summary>
    /// <remarks>
    /// Si la valeur affectée à la propriété est true, le setter force la propriété <see cref="Control.AllowDrop"/> à true.
    /// </remarks>
    [
      Category( "Behavior" ),
      DefaultValue( false ),
      Description( "Obtient ou détermine si le contôle prend en charge la détection automatique des opérations drag-drop" )
    ]
    public bool AutoDragStart {
      get { return autoDragStart; }
      set {
        autoDragStart = value;
        if ( value ) AllowDrop = true;
      }
    }

    /// <summary>
    /// Obtient ou détermine si le contôle prend en charge le drag drop automatique des onglets.
    /// </summary>
    /// <remarks>
    /// Si la valeur affectée à la propriété est true, le setter force la propriété <see cref="AutoDragStart"/> à true,
    /// ce qui aura pour effet en cascade de forcer la propriété <see cref="Control.AllowDrop"/> à true.
    /// </remarks>
    [
      Category( "Behavior" ),
      DefaultValue( false ),
      Description( "Obtient ou détermine si le contôle prend en charge le drag drop automatique des onglets" )
    ]
    public bool AutoDragTabs {
      get {
        return autoDragTabs;
      }
      set {
        autoDragTabs = value;
        if ( value ) AutoDragStart = true;
      }
    }

    /// <summary>
    /// Déclenché côté source lorsqu'une opération drag-drop commence.
    /// </summary>
    [
      Description( "Déclenché côté source lorsqu'une opération drag-drop commence" )
    ]
    public event DragStartEventHandler DragStart;

    /// <summary>
    /// Déclenché côté source à l'issue d'une opération drag-drop si un drop a été effectué côté cible.
    /// </summary>
    [
      Description( "Déclenché côté source à l'issue d'une opération drag-drop si un drop a été effectué côté cible" )
    ]
    public event DragEventHandler DragTerminate;
  }

}
