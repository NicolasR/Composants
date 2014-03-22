/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Crédits :
 * Les listes d'action, inspirées du composant TActionList de Delphi, sont issues d'une 
 * refonte et d'une adaptation de deux composants freeware open source :
 * - les listes d'action pour net 1.1 de Serge Weinstock
 * - les listes d'action pour net 2.0 de Marco de Sanctis
 * 
 * todo (ActionList) : reporter les traitements liés au mode conception dans le concepteur
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 11 2008 : refonte du protocole des actions dans Action avec prise en charge améliorée des exceptions
 * 12 02 2010 : résorption de la bogue des ruptures d'attachement des actions due à une gestion incorrecte de l'état "building"
 * 10 11 2010 : adjonction des méthodes GetAction et SetAction
 */
using System;                                                                  // <wao never.end>
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Actions {

  /// <summary>
  /// Attribut destiné à indiquer, dans une action, quelles sont les propriétés 
  /// qui doivent être distribuées sur les cibles de l'action
  /// </summary>
  public class UpdatablePropertyAttribute : Attribute {}

  /// <summary>
  /// Type de délégué pour les événements de gestion du protocole des actions
  /// </summary>
  public delegate void ActionEventHandler( Action action, EventArgs args );

  /// <summary>
  /// Classe d'exception spécifique aux actions et aux listes d'actions
  /// </summary>
  [Serializable]
  public class EActionList : ExceptionContainer {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associé à l'objet exception</param>
    public EActionList( string message ) : base( message ) { }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associé à l'objet exception</param>
    /// <param name="inner">object exception ayant causé la relance</param>
    public EActionList( string message, Exception inner ) : base( message, inner ) { }
  }

  #region Liste d'actions
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                    Implémentation du composant ActionList
  //          Voir aussi la partie de ActionList énonçant ActionCollection
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe d'implémentation des listes d'actions
  /// </summary>
  [
    DefaultProperty   ( "Actions" ),
    ToolboxItemFilter ( "System.Windows.Forms" ),
    ToolboxBitmap     ( typeof( ActionList ), "ActionList.bmp" ),
    Designer          ( typeof( Psl.Design.ActionListDesigner ) ), // Designer          ( "Psl.Design.ActionListDesigner, " + Psl.AssemblyRef.PslCoreDesign ),
    Description       ( "Collection d'actions associées à l'ihm" )
  ]
  public partial class ActionList : Component, ISupportInitialize {

    //
    // Etat
    //

    /// <summary>
    /// Indicateurs de l'état de la liste d'actions
    /// </summary>
    protected enum State {

      /// <summary>
      /// La liste d'actions est en cours de construction
      /// </summary>
      building,

      /// <summary>
      /// La liste d'actions est opérationnelle.
      /// </summary>
      standard,

      /// <summary>
      /// La liste d'action est en cours de libération des ressources.
      /// </summary>
      disposing,

      /// <summary>
      /// La liste d'actions a libéré toutes ses ressources.
      /// </summary>
      disposed
    }

    //
    // Champs
    //

    // provider des tool tips
    private ToolTip toolTipProvider = new ToolTip();

    // état de la validation des actions au niveau de la liste d'actions
    private bool enabled = true;

    // état de la visibilité des actions au niveau de la liste d'actions.
    private bool visible = true ;

    // contrôle conteneur englobant pour la surveillance des raccourcis clavier
    private ContainerControl containerControl;

    // collection des actions
    private ActionCollection actions;

    // état de la liste d'actions
    State state = State.standard;

    // hôte de conception en mode design
    IDesignerHost designerHost = null ;

    /// <summary>
    /// Constructeur
    /// </summary>
    public ActionList() {
      //Psl.Tracker.Tracker.Track( "ActionList.cctor.3 (empty)" );

      // initialisation dans tous les cas
      actions = new ActionCollection( this );

      // initialisations hors mode conception
      if (DesignMode) return;
      Application.Idle += new EventHandler( Application_Idle );
      ActionGlobalBeforeEvent += new ActionEventHandler( OnActionGlobalBefore );
      ActionGlobalAfterEvent += new ActionEventHandler( OnActionGlobalAfter );
    }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="container">conteneur auquel la liste d'action est ajoutée</param>
    public ActionList( System.ComponentModel.IContainer container ) : this() {
      //Psl.Tracker.Tracker.Track( "ActionList.cctor.3 (container)" );
      container.Add( this );
    }

    /// <summary>
    /// Nettoyage des instances
    /// </summary>
    /// <param name="disposing">true si les ressources non managées doivent être libérées</param>
    protected override void Dispose( bool disposing ) {
      if (disposing && (state != State.disposing) && (state != State.disposed) ) {
        //Psl.Tracker.Tracker.Track( "ActionList.Disposing" ); 
        state = State.disposing;
        try {

          // désabonnements propres à la liste d'action
          if (!DesignMode) {
            Application.Idle -= new EventHandler( Application_Idle );
            ActionGlobalBeforeEvent -= new ActionEventHandler( OnActionGlobalBefore );
            ActionGlobalAfterEvent -= new ActionEventHandler( OnActionGlobalAfter );
            DoSetContainerControl( null );
          }

          // destruction de toutes les actions de la liste
          while (actions.Count > 0) {
            Action action = actions[ actions.Count - 1 ];
            actions.RemoveAt( actions.Count - 1 );
            action.Dispose();
          }
          //Psl.Tracker.Tracker.Track( "ActionList.Disposed" );
        }
        finally { state = State.disposed; }
      }
      base.Dispose( disposing );
    }

    //
    // Gestion de l'état de la liste d'actions
    //

    /// <summary>
    /// Indique si la liste d'actions est en cours de construction
    /// </summary>
    [Browsable(false)]
    public bool IsBuilding {
      get { return state == State.building; }
    }

    /// <summary>
    /// Indique si la liste d'actions a été "disposée"
    /// </summary>
    [Browsable( false )]
    public bool IsDisposed {
      get { return state == State.disposed; }
    }

    /// <summary>
    /// Redéfinition de la propriété Site
    /// </summary>
    public override ISite Site {
      get {
        return base.Site;
      }
      set {
        //Psl.Tracker.Tracker.Track( "ActionList.SetSite" );
        base.Site = value;
        if (value == null) return;

        // création par nécessité du provider de la propriété "Action"
        ActionProvider.TryGetActionProvider( this, Site );

        // récupérer l'hôte de design
        designerHost = value.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

        // en mode design abonnement à l'événement de fin de chargement
        if (DesignMode) {
          if (designerHost == null) throw new EActionList( "(ActionList) Une liste d'action doit comporter un hôte de conception" );
          designerHost.LoadComplete += new EventHandler( OnLoadComplete );
        }
        
        // récupérer le contrôle conteneur pour l'interception des raccourcis clavier
        if (! DesignMode) {
          if (designerHost == null) return ;
          IComponent root = designerHost.RootComponent;
          if (!(root is ContainerControl)) return;
          this.ContainerControl = (ContainerControl) root;
        }
      }
    }

    //
    // Implémentation de ISupportInitialize
    //

    /// <summary>
    /// Notifie le début de l'initialisation d'une liste d'actions
    /// </summary>
    public void BeginInit() {
      state = State.building;
      //Psl.Tracker.Tracker.Track( "ActionList.BeginInit" );
    }

    /// <summary>
    /// Notifie la fin de l'initialisation d'une liste d'actions
    /// </summary>
    /// <remarks>
    /// Permet de terminer l'initialisation des actions.
    /// </remarks>
    public void EndInit() {
      //Psl.Tracker.Tracker.Track( "ActionList.EndInit : début, state=" + state + ", DesignMode=" + DesignMode );
      if ( !DesignMode ) BuildComplete();
      state = State.standard;
      //Psl.Tracker.Tracker.Track( "ActionList.EndInit : fin, state=" + state );
    }

    /// <summary>
    /// Handler de l'événement LoadComplete du concepteur
    /// </summary>
    /// <remarks>
    /// Permet de terminer l'initialisation des actions.
    /// Cette gestion est à migrer vers le concepteur des listes d'actions. 
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoadComplete( object sender, EventArgs e ) {
      //Psl.Tracker.Tracker.Track( "ActionList.OnLoadComplete : début, designerHost.Loading=" + designerHost.Loading );
      BuildComplete() ;
      //Psl.Tracker.Tracker.Track( "ActionList.OnLoadComplete : fin" );
    }

    /// <summary>
    /// Termine l'initialisation des actions, en pariculier le lien des actions à leurs cibles.
    /// </summary>
    public virtual void BuildComplete() {
      state = State.standard ;

      foreach (Action action in actions)
        action.DoBuildComplete();

      DoRefreshActions();
    }

    /// <summary>
    /// Rafraîchit l'état Enabled et Checked de toutes les actions
    /// </summary>
    private void DoRefreshActions() {
      foreach ( Action action in actions )
        action.DoRefreshOverridesInAll();
    }

    //
    // Surveillance Idle
    //

    /// <summary>
    /// Mise à jour des actions sur Idle
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void Application_Idle( object sender, EventArgs e ) {
      OnUpdate( EventArgs.Empty );
    }

    /// <summary>
    /// Déclenchement centralisé de la mise à jour sur Idle
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnUpdate( EventArgs e ) {

      // mise à jour de la liste d'actions elle-même
      if (Update != null) Update( this, e );

      // mise à jour des actions de la liste
      foreach (Action action in actions) {
        action.DoUpdate();
      }
    }

    //
    // Gestion de l'interception des raccourcis clavier
    //

    /// <summary>
    /// Mémorise de conteneur top-level et gère l'abonnement KeyDown
    /// </summary>
    /// <param name="value"></param>
    private void DoSetContainerControl( ContainerControl value ) {
      if (containerControl == value) return ;

      ContainerControl previousContainer = containerControl ;
      containerControl = value ;
      if (DesignMode) return;

      Form previousForm = previousContainer as Form ;
      if (previousForm != null) previousForm.KeyDown -= new KeyEventHandler( Form_KeyDown );

      Form newForm = containerControl as Form;
      if (newForm == null) return ;

      newForm.KeyPreview = true;
      newForm.KeyDown += new KeyEventHandler( Form_KeyDown ) ;
    }

    /// <summary>
    /// Handler pour l'événement KeyDown permettant de gérer les raccourcis clavier.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form_KeyDown( object sender, KeyEventArgs e ) {
      if ( DesignMode ) return;
      foreach (Action action in actions)
        if (action.Visible && action.Enabled && action.ShortcutKeys == (Keys) e.KeyData)
          action.Perform();
    }

    //
    // Protocole des actions
    //

    /// <summary>
    /// Evénement relai pour la diffusion de l'événement <see cref="ActionGlobalBefore"/>
    /// </summary>
    private static event ActionEventHandler ActionGlobalBeforeEvent = null;

    /// <summary>
    /// Evénement relai pour la diffusion de l'événement <see cref="ActionGlobalAfter"/>
    /// </summary>
    private static event ActionEventHandler ActionGlobalAfterEvent = null;

    /// <summary>
    /// Déclenche la diffusion de l'événement de classe ActionGlobalBefore
    /// </summary>
    /// <param name="action">action qui va être appliquée</param>
    /// <param name="args">arguments complémentaires de l'événement</param>
    internal static void FireActionGlobalBeforeEvent( Action action, EventArgs args ) {
      if (ActionGlobalBeforeEvent != null) ActionGlobalBeforeEvent( action, args );
    }

    /// <summary>
    /// Déclenche la diffusion de l'événement de classe ActionGlobalAfter
    /// </summary>
    /// <param name="action">action qui vient d'être appliquée</param>
    /// <param name="args">arguments complémentaires de l'événement</param>
    internal static void FireActionGlobalAfterEvent( Action action, EventArgs args ) {
      if (ActionGlobalAfterEvent != null) ActionGlobalAfterEvent( action, args );
    }

    /// <summary>
    /// Déclenche la diffusion de l'événement ActionGlobalBefore au niveau de l'instance
    /// </summary>
    /// <param name="action">action qui va être appliquée</param>
    /// <param name="args">arguments complémentaires de l'action</param>
    protected virtual void OnActionGlobalBefore( Action action, EventArgs args ) {
      if (ActionGlobalBefore != null) ActionGlobalBefore( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'événement ActionGlobalAfter au niveau de l'instance
    /// </summary>
    /// <param name="action">action qui vient d'être appliquée</param>
    /// <param name="args">arguments complémentaires de l'action</param>
    protected virtual void OnActionGlobalAfter( Action action, EventArgs args ) {
      if (ActionGlobalAfter != null) ActionGlobalAfter( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'événement ActionExecuting
    /// </summary>
    /// <param name="action">action qui va être exécutée</param>
    /// <param name="args">arguments complémentaires de l'action</param>
    internal protected virtual void OnActionExecuting( Action action, EventArgs args ) {
      if (ActionExecuting != null) ActionExecuting( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'événement ActionExecuted
    /// </summary>
    /// <param name="action">action qui vient d'être exécutée</param>
    /// <param name="args">arguments complémentaires de l'action</param>
    internal protected virtual void OnActionExecuted( Action action, EventArgs args ) {
      if (ActionExecuted != null) ActionExecuted( action, args );
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Tente d'obtenir l'action à laquelle le composant <paramref name="target"/> est attaché. 
    /// </summary>
    /// <param name="target">composant lié à l'action recherchée</param>
    /// <returns>la référence sur l'action à laquelle le composant <paramref name="target"/> est attaché, ou null si l'action est introuvable</returns>
    public Action GetAction( Component target ) {
      if ( Site == null ) return null;

      ActionProvider provider = ActionProvider.TryGetActionProvider( Site );
      if ( provider == null ) return null;

      return provider.GetAction( target );
    }

    /// <summary>
    /// Attache une action à un composant.
    /// </summary>
    /// <param name="target">composant auquel attacher l'action</param>
    /// <param name="value">action à attacher au composant</param>
    public void SetAction( Component target, Action value ) {
      ActionProvider provider = ActionProvider.TryGetActionProvider( Site );
      if ( provider == null ) return;

      provider.SetAction( target, value );
    }

    #region Propriétés exposées en mode conception
    //
    // Propriétés exposées en mode conception
    //

    /// <summary>
    /// Obtient ou détermine si tous les contrôles associés à toutes les actions de cette liste sont validés ou inhibés.
    /// </summary>
    [ 
      DefaultValue( true ),
      Description( "Obtient ou détermine si tous les contrôles associés à toutes les actions de cette liste sont validés ou inhibés" )
    ]
    public bool Enabled {
      get { return enabled; }
      set {
        if (enabled == value) return;
        enabled = value;
        DoRefreshActions();
      }
    }

    /// <summary>
    /// Obtient ou détermine si tous les contrôles associés à toutes les actions de cette liste sont visibles ou non.
    /// </summary>
    [
      DefaultValue( true ),
      Description( "Obtient ou détermine si tous les contrôles associés à toutes les actions de cette liste sont visibles ou non" )
    ]
    public bool Visible {
      get { return visible ; }
      set {
        if (visible == value) return;
        visible = value;
        DoRefreshActions();
      }
    }

    /// <summary>
    /// Liste des actions.
    /// </summary>
    [
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
      Description( "Liste des actions" ) 
    ]
    public ActionCollection Actions {
      get { return actions; }
    }

    /// <summary>
    /// Provider des conseils pour la propriété ToolTipText.
    /// </summary>
    public ToolTip ToolTipProvider {
      get { return toolTipProvider; }
    }

    /// <summary>
    /// Obtient ou détermine le contrôle conteneur à surveiller pour intercepter les raccourcis de déclenchement des actions.
    /// </summary>
    [
      Browsable( false ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
      Description( "Obtient ou détermine le contrôle conteneur à surveiller pour intercepter les raccourcis de déclenchement des actions" )
    ]
    public ContainerControl ContainerControl {
      get { return containerControl; }
      set {
        if (containerControl == value) return;
        DoSetContainerControl( value ) ;
      }
    }

    /// <summary>
    /// Evénement déclenché avant l'exécution du corps de toute action de l'application
    /// </summary>
    [Description( "Evénement déclenché avant l'exécution du corps de toute action de l'application" )]
    public event ActionEventHandler ActionGlobalBefore;

    /// <summary>
    /// Evénement déclenché après l'exécution du corps de toute action de l'application
    /// </summary>
    [Description( "Evénement déclenché après l'exécution du corps de toute action de l'application" )]
    public event ActionEventHandler ActionGlobalAfter;

    /// <summary>
    /// Evénement déclenché juste avant que le corps d'une action de cette liste d'action soit exécuté
    /// </summary>
    [Description( "Evénement déclenché juste avant que le corps d'une action de cette liste d'action soit exécuté" )]
    public event ActionEventHandler ActionExecuting;

    /// <summary>
    /// Evénement déclenché juste après que le corps d'une action de cette liste d'action a été exécuté
    /// </summary>
    [Description( "Evénement déclenché juste après que le corps d'une action de cette liste d'action a été exécuté" )]
    public event ActionEventHandler ActionExecuted;

    /// <summary>
    /// Evénement déclenché sur Idle pour la mise à jour de la liste d'actions
    /// </summary>
    [Description( "Evénement déclenché (sur Idle) pour permettre la mise à jour de la liste d'actions" )]
    public event EventHandler Update;

    #endregion

  }
  #endregion
}
