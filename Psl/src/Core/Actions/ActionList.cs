/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * Cr�dits :
 * Les listes d'action, inspir�es du composant TActionList de Delphi, sont issues d'une 
 * refonte et d'une adaptation de deux composants freeware open source :
 * - les listes d'action pour net 1.1 de Serge Weinstock
 * - les listes d'action pour net 2.0 de Marco de Sanctis
 * 
 * todo (ActionList) : reporter les traitements li�s au mode conception dans le concepteur
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 11 2008 : refonte du protocole des actions dans Action avec prise en charge am�lior�e des exceptions
 * 12 02 2010 : r�sorption de la bogue des ruptures d'attachement des actions due � une gestion incorrecte de l'�tat "building"
 * 10 11 2010 : adjonction des m�thodes GetAction et SetAction
 */
using System;                                                                  // <wao never.end>
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Actions {

  /// <summary>
  /// Attribut destin� � indiquer, dans une action, quelles sont les propri�t�s 
  /// qui doivent �tre distribu�es sur les cibles de l'action
  /// </summary>
  public class UpdatablePropertyAttribute : Attribute {}

  /// <summary>
  /// Type de d�l�gu� pour les �v�nements de gestion du protocole des actions
  /// </summary>
  public delegate void ActionEventHandler( Action action, EventArgs args );

  /// <summary>
  /// Classe d'exception sp�cifique aux actions et aux listes d'actions
  /// </summary>
  [Serializable]
  public class EActionList : ExceptionContainer {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associ� � l'objet exception</param>
    public EActionList( string message ) : base( message ) { }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associ� � l'objet exception</param>
    /// <param name="inner">object exception ayant caus� la relance</param>
    public EActionList( string message, Exception inner ) : base( message, inner ) { }
  }

  #region Liste d'actions
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                    Impl�mentation du composant ActionList
  //          Voir aussi la partie de ActionList �non�ant ActionCollection
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe d'impl�mentation des listes d'actions
  /// </summary>
  [
    DefaultProperty   ( "Actions" ),
    ToolboxItemFilter ( "System.Windows.Forms" ),
    ToolboxBitmap     ( typeof( ActionList ), "ActionList.bmp" ),
    Designer          ( typeof( Psl.Design.ActionListDesigner ) ), // Designer          ( "Psl.Design.ActionListDesigner, " + Psl.AssemblyRef.PslCoreDesign ),
    Description       ( "Collection d'actions associ�es � l'ihm" )
  ]
  public partial class ActionList : Component, ISupportInitialize {

    //
    // Etat
    //

    /// <summary>
    /// Indicateurs de l'�tat de la liste d'actions
    /// </summary>
    protected enum State {

      /// <summary>
      /// La liste d'actions est en cours de construction
      /// </summary>
      building,

      /// <summary>
      /// La liste d'actions est op�rationnelle.
      /// </summary>
      standard,

      /// <summary>
      /// La liste d'action est en cours de lib�ration des ressources.
      /// </summary>
      disposing,

      /// <summary>
      /// La liste d'actions a lib�r� toutes ses ressources.
      /// </summary>
      disposed
    }

    //
    // Champs
    //

    // provider des tool tips
    private ToolTip toolTipProvider = new ToolTip();

    // �tat de la validation des actions au niveau de la liste d'actions
    private bool enabled = true;

    // �tat de la visibilit� des actions au niveau de la liste d'actions.
    private bool visible = true ;

    // contr�le conteneur englobant pour la surveillance des raccourcis clavier
    private ContainerControl containerControl;

    // collection des actions
    private ActionCollection actions;

    // �tat de la liste d'actions
    State state = State.standard;

    // h�te de conception en mode design
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
    /// <param name="container">conteneur auquel la liste d'action est ajout�e</param>
    public ActionList( System.ComponentModel.IContainer container ) : this() {
      //Psl.Tracker.Tracker.Track( "ActionList.cctor.3 (container)" );
      container.Add( this );
    }

    /// <summary>
    /// Nettoyage des instances
    /// </summary>
    /// <param name="disposing">true si les ressources non manag�es doivent �tre lib�r�es</param>
    protected override void Dispose( bool disposing ) {
      if (disposing && (state != State.disposing) && (state != State.disposed) ) {
        //Psl.Tracker.Tracker.Track( "ActionList.Disposing" ); 
        state = State.disposing;
        try {

          // d�sabonnements propres � la liste d'action
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
    // Gestion de l'�tat de la liste d'actions
    //

    /// <summary>
    /// Indique si la liste d'actions est en cours de construction
    /// </summary>
    [Browsable(false)]
    public bool IsBuilding {
      get { return state == State.building; }
    }

    /// <summary>
    /// Indique si la liste d'actions a �t� "dispos�e"
    /// </summary>
    [Browsable( false )]
    public bool IsDisposed {
      get { return state == State.disposed; }
    }

    /// <summary>
    /// Red�finition de la propri�t� Site
    /// </summary>
    public override ISite Site {
      get {
        return base.Site;
      }
      set {
        //Psl.Tracker.Tracker.Track( "ActionList.SetSite" );
        base.Site = value;
        if (value == null) return;

        // cr�ation par n�cessit� du provider de la propri�t� "Action"
        ActionProvider.TryGetActionProvider( this, Site );

        // r�cup�rer l'h�te de design
        designerHost = value.GetService( typeof( IDesignerHost ) ) as IDesignerHost;

        // en mode design abonnement � l'�v�nement de fin de chargement
        if (DesignMode) {
          if (designerHost == null) throw new EActionList( "(ActionList) Une liste d'action doit comporter un h�te de conception" );
          designerHost.LoadComplete += new EventHandler( OnLoadComplete );
        }
        
        // r�cup�rer le contr�le conteneur pour l'interception des raccourcis clavier
        if (! DesignMode) {
          if (designerHost == null) return ;
          IComponent root = designerHost.RootComponent;
          if (!(root is ContainerControl)) return;
          this.ContainerControl = (ContainerControl) root;
        }
      }
    }

    //
    // Impl�mentation de ISupportInitialize
    //

    /// <summary>
    /// Notifie le d�but de l'initialisation d'une liste d'actions
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
      //Psl.Tracker.Tracker.Track( "ActionList.EndInit : d�but, state=" + state + ", DesignMode=" + DesignMode );
      if ( !DesignMode ) BuildComplete();
      state = State.standard;
      //Psl.Tracker.Tracker.Track( "ActionList.EndInit : fin, state=" + state );
    }

    /// <summary>
    /// Handler de l'�v�nement LoadComplete du concepteur
    /// </summary>
    /// <remarks>
    /// Permet de terminer l'initialisation des actions.
    /// Cette gestion est � migrer vers le concepteur des listes d'actions. 
    /// </remarks>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnLoadComplete( object sender, EventArgs e ) {
      //Psl.Tracker.Tracker.Track( "ActionList.OnLoadComplete : d�but, designerHost.Loading=" + designerHost.Loading );
      BuildComplete() ;
      //Psl.Tracker.Tracker.Track( "ActionList.OnLoadComplete : fin" );
    }

    /// <summary>
    /// Termine l'initialisation des actions, en pariculier le lien des actions � leurs cibles.
    /// </summary>
    public virtual void BuildComplete() {
      state = State.standard ;

      foreach (Action action in actions)
        action.DoBuildComplete();

      DoRefreshActions();
    }

    /// <summary>
    /// Rafra�chit l'�tat Enabled et Checked de toutes les actions
    /// </summary>
    private void DoRefreshActions() {
      foreach ( Action action in actions )
        action.DoRefreshOverridesInAll();
    }

    //
    // Surveillance Idle
    //

    /// <summary>
    /// Mise � jour des actions sur Idle
    /// </summary>
    /// <param name="sender">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void Application_Idle( object sender, EventArgs e ) {
      OnUpdate( EventArgs.Empty );
    }

    /// <summary>
    /// D�clenchement centralis� de la mise � jour sur Idle
    /// </summary>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected virtual void OnUpdate( EventArgs e ) {

      // mise � jour de la liste d'actions elle-m�me
      if (Update != null) Update( this, e );

      // mise � jour des actions de la liste
      foreach (Action action in actions) {
        action.DoUpdate();
      }
    }

    //
    // Gestion de l'interception des raccourcis clavier
    //

    /// <summary>
    /// M�morise de conteneur top-level et g�re l'abonnement KeyDown
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
    /// Handler pour l'�v�nement KeyDown permettant de g�rer les raccourcis clavier.
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
    /// Ev�nement relai pour la diffusion de l'�v�nement <see cref="ActionGlobalBefore"/>
    /// </summary>
    private static event ActionEventHandler ActionGlobalBeforeEvent = null;

    /// <summary>
    /// Ev�nement relai pour la diffusion de l'�v�nement <see cref="ActionGlobalAfter"/>
    /// </summary>
    private static event ActionEventHandler ActionGlobalAfterEvent = null;

    /// <summary>
    /// D�clenche la diffusion de l'�v�nement de classe ActionGlobalBefore
    /// </summary>
    /// <param name="action">action qui va �tre appliqu�e</param>
    /// <param name="args">arguments compl�mentaires de l'�v�nement</param>
    internal static void FireActionGlobalBeforeEvent( Action action, EventArgs args ) {
      if (ActionGlobalBeforeEvent != null) ActionGlobalBeforeEvent( action, args );
    }

    /// <summary>
    /// D�clenche la diffusion de l'�v�nement de classe ActionGlobalAfter
    /// </summary>
    /// <param name="action">action qui vient d'�tre appliqu�e</param>
    /// <param name="args">arguments compl�mentaires de l'�v�nement</param>
    internal static void FireActionGlobalAfterEvent( Action action, EventArgs args ) {
      if (ActionGlobalAfterEvent != null) ActionGlobalAfterEvent( action, args );
    }

    /// <summary>
    /// D�clenche la diffusion de l'�v�nement ActionGlobalBefore au niveau de l'instance
    /// </summary>
    /// <param name="action">action qui va �tre appliqu�e</param>
    /// <param name="args">arguments compl�mentaires de l'action</param>
    protected virtual void OnActionGlobalBefore( Action action, EventArgs args ) {
      if (ActionGlobalBefore != null) ActionGlobalBefore( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'�v�nement ActionGlobalAfter au niveau de l'instance
    /// </summary>
    /// <param name="action">action qui vient d'�tre appliqu�e</param>
    /// <param name="args">arguments compl�mentaires de l'action</param>
    protected virtual void OnActionGlobalAfter( Action action, EventArgs args ) {
      if (ActionGlobalAfter != null) ActionGlobalAfter( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'�v�nement ActionExecuting
    /// </summary>
    /// <param name="action">action qui va �tre ex�cut�e</param>
    /// <param name="args">arguments compl�mentaires de l'action</param>
    internal protected virtual void OnActionExecuting( Action action, EventArgs args ) {
      if (ActionExecuting != null) ActionExecuting( action, args );
    }

    /// <summary>
    /// Provoque la diffusion de l'�v�nement ActionExecuted
    /// </summary>
    /// <param name="action">action qui vient d'�tre ex�cut�e</param>
    /// <param name="args">arguments compl�mentaires de l'action</param>
    internal protected virtual void OnActionExecuted( Action action, EventArgs args ) {
      if (ActionExecuted != null) ActionExecuted( action, args );
    }

    //
    // Fonctionnalit�s expos�es
    //

    /// <summary>
    /// Tente d'obtenir l'action � laquelle le composant <paramref name="target"/> est attach�. 
    /// </summary>
    /// <param name="target">composant li� � l'action recherch�e</param>
    /// <returns>la r�f�rence sur l'action � laquelle le composant <paramref name="target"/> est attach�, ou null si l'action est introuvable</returns>
    public Action GetAction( Component target ) {
      if ( Site == null ) return null;

      ActionProvider provider = ActionProvider.TryGetActionProvider( Site );
      if ( provider == null ) return null;

      return provider.GetAction( target );
    }

    /// <summary>
    /// Attache une action � un composant.
    /// </summary>
    /// <param name="target">composant auquel attacher l'action</param>
    /// <param name="value">action � attacher au composant</param>
    public void SetAction( Component target, Action value ) {
      ActionProvider provider = ActionProvider.TryGetActionProvider( Site );
      if ( provider == null ) return;

      provider.SetAction( target, value );
    }

    #region Propri�t�s expos�es en mode conception
    //
    // Propri�t�s expos�es en mode conception
    //

    /// <summary>
    /// Obtient ou d�termine si tous les contr�les associ�s � toutes les actions de cette liste sont valid�s ou inhib�s.
    /// </summary>
    [ 
      DefaultValue( true ),
      Description( "Obtient ou d�termine si tous les contr�les associ�s � toutes les actions de cette liste sont valid�s ou inhib�s" )
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
    /// Obtient ou d�termine si tous les contr�les associ�s � toutes les actions de cette liste sont visibles ou non.
    /// </summary>
    [
      DefaultValue( true ),
      Description( "Obtient ou d�termine si tous les contr�les associ�s � toutes les actions de cette liste sont visibles ou non" )
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
    /// Provider des conseils pour la propri�t� ToolTipText.
    /// </summary>
    public ToolTip ToolTipProvider {
      get { return toolTipProvider; }
    }

    /// <summary>
    /// Obtient ou d�termine le contr�le conteneur � surveiller pour intercepter les raccourcis de d�clenchement des actions.
    /// </summary>
    [
      Browsable( false ),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
      Description( "Obtient ou d�termine le contr�le conteneur � surveiller pour intercepter les raccourcis de d�clenchement des actions" )
    ]
    public ContainerControl ContainerControl {
      get { return containerControl; }
      set {
        if (containerControl == value) return;
        DoSetContainerControl( value ) ;
      }
    }

    /// <summary>
    /// Ev�nement d�clench� avant l'ex�cution du corps de toute action de l'application
    /// </summary>
    [Description( "Ev�nement d�clench� avant l'ex�cution du corps de toute action de l'application" )]
    public event ActionEventHandler ActionGlobalBefore;

    /// <summary>
    /// Ev�nement d�clench� apr�s l'ex�cution du corps de toute action de l'application
    /// </summary>
    [Description( "Ev�nement d�clench� apr�s l'ex�cution du corps de toute action de l'application" )]
    public event ActionEventHandler ActionGlobalAfter;

    /// <summary>
    /// Ev�nement d�clench� juste avant que le corps d'une action de cette liste d'action soit ex�cut�
    /// </summary>
    [Description( "Ev�nement d�clench� juste avant que le corps d'une action de cette liste d'action soit ex�cut�" )]
    public event ActionEventHandler ActionExecuting;

    /// <summary>
    /// Ev�nement d�clench� juste apr�s que le corps d'une action de cette liste d'action a �t� ex�cut�
    /// </summary>
    [Description( "Ev�nement d�clench� juste apr�s que le corps d'une action de cette liste d'action a �t� ex�cut�" )]
    public event ActionEventHandler ActionExecuted;

    /// <summary>
    /// Ev�nement d�clench� sur Idle pour la mise � jour de la liste d'actions
    /// </summary>
    [Description( "Ev�nement d�clench� (sur Idle) pour permettre la mise � jour de la liste d'actions" )]
    public event EventHandler Update;

    #endregion

  }
  #endregion
}
