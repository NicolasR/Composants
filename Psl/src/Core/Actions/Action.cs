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
 * todo (Action) : basculement CheckState pour les composants qui n'exposent pas d'événement CheckStateChanged
 * todo (Action) : le composant CheckBox expose une propriété nommée AutoCheck et non CheckOnClick
 * todo (Action) : migrer le traitement lié au mode conception dans le concepteur
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 08 03 2008 : adjonction de la notification des changements pour la liste des cibles
 * 08 03 2008 : amélioration de l'implémentation de la liste des cibles par redirection de ICollection(Component)
 * 08 03 2008 : la propriété Targets est maintenant de type ICollection(Component)
 * 08 03 2008 : suppression de la gestion (erronée) de la propriété Checked dans DoActionExecute
 * 09 03 2008 : nettoyage de divers aspects du code
 * 19 04 2008 : mise à niveau de la mise à jour de la pté ToolTipText pour les composants Net 2.0 ou ultérieur
 * 09 11 2008 : refonte du protocole des actions dans Action avec prise en charge améliorée des exceptions
 * 25 11 2008 : test de owner != null dans DoActionExecute en cas de fermeture de la fenêtre hôte
 * 22 01 2009 : utilisation de ExceptionContainer comme préfixe EActionList pour contourner le pbm des exceptions non sérialisables
 * 22 05 2009 : adjonction de DoGetClickEventName pour prendre en charge le cas des ToolStripSplitButton
 * 12 02 2010 : résorption de la bogue des ruptures d'attachement des actions due à une gestion incorrecte de l'état "building"
 */                                                                            // <wao never.end>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Actions {

  #region Cache accélérateur de la description des types des cibles
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //             Cache accélérateur de la description réflexive des types des cibles
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  public partial class Action {

    // <exclude/>
    /// <summary>
    /// Cache accélérateur de la description réflexive des types des cibles
    /// </summary>
    /// <remarks>
    /// Le cache accélérateur est à la fois adaptatif et automatique, et se construit de lui-même
    /// à la volée au fur et à mesure des demandes qui lui sont adressées. Il ne recherche qu'une
    /// seule fois les informations réflexives de propriétés et de méthodes dans la description
    /// réflexive complète d'un type de cible donné.
    /// <br/>
    /// Le cache expose les méthodes permettant d'accéder aux propriétés ou aux événements
    /// des diférentes cibles.
    /// </remarks>
    protected struct TargetTypeCache {


      // <exclude
      /// <summary>
      /// Descripteur de l'extrait des informations réflexives liées à un type de cible donné
      /// </summary>
      /// <remarks>
      /// Cette structure est emboîtée dans la struture qui gère le cache. Les propriétés et méthodes
      /// de cette structure ne sont pas destinées à être appelées "de l'extérieur", mais seulement
      /// depuis les méthodes du cache lui-même.
      /// </remarks>
      public struct TypeData {

        /// <summary>
        /// Type de la cible concerné
        /// </summary>
        private Type type;

        /// <summary>
        /// Cache accélérateur pour les proriétés
        /// </summary>
        private Dictionary<string, PropertyInfo> properties;

        /// <summary>
        /// Cache accélérateur pour les événements
        /// </summary>
        private Dictionary<string, EventInfo> events;

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="type">type de la cible</param>
        public TypeData( Type type ) {
          this.type = type;
          properties = new Dictionary<string, PropertyInfo>();
          events = new Dictionary<string, EventInfo>();
        }

        /// <summary>
        /// Récupérer les informations réflexives sur une propriété
        /// </summary>
        /// <remarks>
        /// Cette méthode permet de construire à la volée le cache accélérateur (dictionnaire "properties").
        /// Dès que les informations sur une propriété sont recherchées pour ce type, la méthode mémorise
        /// dans le cache "properties" l'association entre le nom de la propriété et les informations
        /// reflexives liées à cette propriété (ou null si la propriété ne figure pas dans ce type de cible). 
        /// </remarks>
        /// <param name="propertyName">nom de la propriété</param>
        /// <param name="propertyInfos">informations de reflexion sur la propriété ou null si la propriété est introuvable</param>
        /// <returns>true si les informations sur la propriété </returns>
        private bool TryGetPropertyInfos( string propertyName, out PropertyInfo propertyInfos ) {
          if ( !properties.TryGetValue( propertyName, out propertyInfos ) ) {
            propertyInfos = type.GetProperty( propertyName );
            properties.Add( propertyName, propertyInfos );
          }
          return propertyInfos != null;
        }

        /// <summary>
        /// Récupérer les informations réflexives sur un événement
        /// </summary>
        /// <remarks>
        /// Cette méthode permet de construire à la volée le cache accélérateur (dictionnaire "events").
        /// Dès que les informations sur un événement sont recherchées pour ce type, la méthode mémorise
        /// dans le cache "events" l'association entre le nom de l'événement et les informations
        /// reflexives liées à cet événement (ou null si l'événement ne figure pas dans ce type de cible). 
        /// </remarks>
        /// <param name="eventName">nom de l'événement</param>
        /// <param name="eventInfos">informations de reflexion sur l'événement ou null si l'événement est introuvable</param>
        /// <returns></returns>
        private bool TryGetEventInfos( string eventName, out EventInfo eventInfos ) {
          if ( !events.TryGetValue( eventName, out eventInfos ) ) {
            eventInfos = type.GetEvent( eventName );
            events.Add( eventName, eventInfos );
          }
          return eventInfos != null;
        }

        /// <summary>
        /// Affecter une valeur à une propriété d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si la propriété est introuvable (pas d'exception).
        /// Contruction automatique à la volée du cache des propriétés 
        /// </remarks>
        /// <param name="propertyName">nom de la propriété</param>
        /// <param name="target">référence sur la cible</param>
        /// <param name="value">valeur à affecter</param>
        public void SetValue( string propertyName, object target, object value ) {
          PropertyInfo propertyInfos;
          if ( !TryGetPropertyInfos( propertyName, out propertyInfos ) ) return;
          propertyInfos.SetValue( target, value, null );
        }

        /// <summary>
        /// Obtenir la valeur d'une propriété d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si la propriété est introuvable (pas d'exception).
        /// Contruction automatique à la volée du cache des propriétés 
        /// </remarks>
        /// <param name="propertyName">nom de la propriété</param>
        /// <param name="target">référence sur la cible</param>
        /// <returns>la valeur de la propriété ou null si introuvable</returns>
        public object GetValue( string propertyName, object target ) {
          PropertyInfo propertyInfos;
          if ( !TryGetPropertyInfos( propertyName, out propertyInfos ) ) return null;
          return propertyInfos.GetValue( target, null );
        }

        /// <summary>
        /// Abonne un délégué à un événement d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
        /// Contruction automatique à la volée du cache des événements 
        /// </remarks>
        /// <param name="eventName">nom de l'événement</param>
        /// <param name="target">référence sur la cible</param>
        /// <param name="eventHandler">délégué à abonner</param>
        public void AddHandler( string eventName, object target, Delegate eventHandler ) {
          EventInfo eventInfos;
          if ( !TryGetEventInfos( eventName, out eventInfos ) ) return;
          eventInfos.AddEventHandler( target, eventHandler );
        }

        /// <summary>
        /// Désabonne un délégué pour un événement d'une cible donnée
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
        /// Contruction automatique à la volée du cache des événements
        /// </remarks>
        /// <param name="eventName">nom de l'événement</param>
        /// <param name="target">référence sur la cible</param>
        /// <param name="eventHandler">délégué à désabonner</param>
        public void RemHandler( string eventName, object target, Delegate eventHandler ) {
          EventInfo eventInfos;
          if ( !TryGetEventInfos( eventName, out eventInfos ) ) return;
          eventInfos.RemoveEventHandler( target, eventHandler );
        }
      }

      /// <summary>
      /// Dictionnaire des caches propres à chaque type de cible
      /// </summary>
      private Dictionary<Type, TypeData> typeDataCache;

      /// <summary>
      /// Initialiseur
      /// </summary>
      /// <param name="dummy">pour forcer la différence avec l'initialiseur par défaut sans paraètres</param>
      public TargetTypeCache( int dummy ) {
        typeDataCache = new Dictionary<Type, TypeData>();
      }

      /// <summary>
      /// Accès au cache associé à un type de cible donné
      /// </summary>
      /// <remarks>
      /// Cette méthode ne retourne jamais null.
      /// Elle construit à la volée le dictionnaire des caches d'information sur les types de cibles.
      /// </remarks>
      /// <param name="target">référence sur la cible</param>
      /// <returns>le cache associé à la cible</returns>
      public TypeData this[ object target ] {
        get {
          TypeData typeData;
          if ( typeDataCache.TryGetValue( target.GetType(), out typeData ) ) return typeData;

          // ajouter éventuellement les informations de type
          typeData = new TypeData( target.GetType() );
          typeDataCache.Add( target.GetType(), typeData );
          return typeData;
        }
      }

      /// <summary>
      /// Affecter une valeur à une propriété d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si la propriété est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// </remarks>
      /// <param name="propertyName">nom de la propriété</param>
      /// <param name="target">référence sur la cible</param>
      /// <param name="value">valeur à affecter</param>
      public void SetValue( string propertyName, object target, object value ) {
        this[ target ].SetValue( propertyName, target, value );
      }

      /// <summary>
      /// Obtenir la valeur d'une propriété d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si la propriété est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// </remarks>
      /// <param name="propertyName">nom de la propriété</param>
      /// <param name="target">référence sur la cible</param>
      /// <returns>la valeur de la propriété ou null si introuvable</returns>
      public object GetValue( string propertyName, object target ) {
        return this[ target ].GetValue( propertyName, target );
      }

      /// <summary>
      /// Abonne un délégué à un événement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// </remarks>
      /// <param name="eventName">nom de l'événement</param>
      /// <param name="target">référence sur la cible</param>
      /// <param name="eventHandler">délégué à abonner</param>
      public void AddHandler( string eventName, object target, Delegate eventHandler ) {
        this[ target ].AddHandler( eventName, target, eventHandler );
      }

      /// <summary>
      /// Désabonne un délégué pour un événement d'une cible donnée
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// </remarks>
      /// <param name="eventName">nom de l'événement</param>
      /// <param name="target">référence sur la cible</param>
      /// <param name="eventHandler">délégué à désabonner</param>
      public void RemHandler( string eventName, object target, Delegate eventHandler ) {
        this[ target ].RemHandler( eventName, target, eventHandler );
      }

      /// <summary>
      /// Abonne un délégué de type ToolBarButtonClickEventHandler à un événement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// <br/>
      /// Pour raisons de régularité, la cible "source" est de type ToolBarButton, mais l'événement
      /// est en fait associé à barre d'outils ToolBar qui contient de ce bouton. La méthode agit
      /// d'elle-même sur la barre d'outils contenant le bouton.
      /// </remarks>
      /// <param name="eventName">nom de l'événement</param>
      /// <param name="target">référence sur la cible</param>
      /// <param name="eventHandler">délégué à abonner</param>
      public void AddHandler( string eventName, object target, ToolBarButtonClickEventHandler eventHandler ) {
        ToolBarButton button = target as ToolBarButton;
        if ( button == null ) return;
        ToolBar parent = button.Parent;
        if ( parent == null ) return;
        this[ parent ].AddHandler( eventName, parent, eventHandler );
      }

      /// <summary>
      /// Désabonne un délégué de type ToolBarButtonClickEventHandler à un événement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'événement est introuvable (pas d'exception).
      /// Contruction automatique à la volée des caches nécessaires.
      /// <br/>
      /// Pour raisons de régularité, la cible "source" est de type ToolBarButton, mais l'événement
      /// est en fait associé à barre d'outils ToolBar qui contient de ce bouton. La méthode agit
      /// d'elle-même sur la barre d'outils contenant le bouton.
      /// </remarks>
      /// <param name="eventName">nom de l'événement</param>
      /// <param name="target">référence sur la cible</param>
      /// <param name="eventHandler">délégué à désabonner</param>
      public void RemHandler( string eventName, object target, ToolBarButtonClickEventHandler eventHandler ) {
        ToolBarButton button = target as ToolBarButton;
        if ( button == null ) return;
        ToolBar parent = button.Parent;
        if ( parent == null ) return;
        this[ parent ].RemHandler( eventName, parent, eventHandler );
      }
    }
  }

  #endregion

  #region Liste des cibles
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                               Liste des cibles
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  public partial class Action {

    // <exclude
    /// <summary>
    /// Classe interne associée à la liste des cibles
    /// </summary>
    /// <remarks>
    /// Permet d'exposer la collection des cibles comme ICollection(Component)
    /// en redirigeant les méthodes d'ajonction et de suppression pour effectuer la gestion
    /// des cibles. 
    /// <br/>
    /// Permet en outre de retarder la gestion des cibles pendant la phase de construction de
    /// manière à éviter de risquer la mise à jour de cibles n'ayant pas encore été initialisées.
    /// </remarks>
    protected class TargetList : List<Component>, ICollection<Component> {

      /// <summary>
      /// Référence sur l'action Acton propriétaire de la liste
      /// </summary>
      private Action owner;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="owner">action propriétaire de la liste</param>
      internal TargetList( Action owner ) {
        this.owner = owner;
      }

      /// <summary>
      /// Vide la liste des cibles
      /// </summary>
      /// <remarks>
      /// Réimplémentation du membre correspondant de ICollection.
      /// <br/>
      /// Détourne le traitement via l'action propriétaire pour garantir la gestion des cibles.
      /// </remarks>
      void ICollection<Component>.Clear() {
        if ( owner.IsBuilding )
          base.Clear();
        else
          owner.DoDetachTargets();
      }

      /// <summary>
      /// Ajoute une cible à la liste
      /// </summary>
      /// <remarks>
      /// Réimplémentation du membre correspondant de ICollection.
      /// <br/>
      /// Détourne le traitement via l'action propriétaire pour grantir la gestion des cibles.
      /// <br/>
      /// C'est cette méthode qui est utilisée pour la désérialisation du concepteur.
      /// </remarks>
      /// <param name="target">cible à ajouter</param>
      void ICollection<Component>.Add( Component target ) {
        if ( owner.IsBuilding )
          base.Add( target );
        else {
          owner.DoAttachTarget( target );
        }
      }

      /// <summary>
      /// Supprimer une cible de la liste.
      /// </summary>
      /// <remarks>
      /// Réimplémentation du membre correspondant de ICollection.
      /// <br/>
      /// Détourne le traitement via l'action propriétaire pour grantir la gestion des cibles.
      /// </remarks>
      /// <param name="target">cible à supprimer</param>
      /// <returns>true si la cible a effectivement été supprimée</returns>
      bool ICollection<Component>.Remove( Component target ) {
        if ( owner.IsBuilding )
          return base.Remove( target );
        else {
          return owner.DoRemoveTarget( target );
        }
      }
    }
  }
  #endregion

  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                             Implémentation des actions
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Implémentation des actions.
  /// </summary>
  [                      
    ToolboxItem      (false),
    ToolboxBitmap    (typeof(Action), "Action.bmp"),
		DefaultProperty  ("Text"),
    DefaultEvent     ("Execute"),
    Designer         ( typeof( Psl.Design.ActionDesigner ) ), // Designer         ( "Psl.Design.ActionDesigner, " + Psl.AssemblyRef.PslCoreDesign ), 
  ]
  public partial class Action : Component {

    // <exclude
    /// <summary>
    /// Enumération pour l'état global de l'action relativement à son cycle de vie
    /// </summary>
    protected enum ActionState {

      /// <summary>
      /// L'action est en cours de construction
      /// </summary>
      Building,

      /// <summary>
      /// L'action est opérationnelle
      /// </summary>
      Standard,

      /// <summary>
      /// L'action est en train de libérer ses ressources
      /// </summary>
      Disposing,

      /// <summary>
      /// L'action a déjà libéré toutes ses ressources
      /// </summary>
      Disposed
    }

    /// <summary>
    /// Enumération pour l'état local de l'action pendant les mises à jour
    /// </summary>
    internal enum WorkingState {

      /// <summary>
      /// L'action est à l'écoute des événements de ses clients
      /// </summary>
      Listening,

      /// <summary>
      /// L'action est en train de piloter ses clients
      /// </summary>
      Driving
    }

    // cache accélérateur unique pour toutes les actions donnant accès aux propriétés réflexives des cibles 
    private   static TargetTypeCache         types                         = new TargetTypeCache(0);

    // valeurs par défaut
    private   static readonly Color          defaultImageTransparentColor  = Color.Magenta ;

    // gestion générale des l'action
    private   ActionList                     owner                         = null;
    private   TargetList                     targets                       = null;
    private   PropertyDescriptorCollection   updatableProperties           = null;
    private   IComponentChangeService        changeService                 = null;

    // gestion interne de l'état de l'action
    internal  WorkingState                   workingState                  = WorkingState.Listening;
    private   ActionState                    state                         = ActionState.Building;

    // champs associés aux propriétés publiques
    private   string                         text                          = string.Empty ;
    private   CheckState                     checkState                    = CheckState.Unchecked ;
    private   bool                           enabled                       = true ;
    private   Image                          image                         = null ;
    private   Color                          imageTransparentColor         = defaultImageTransparentColor;
    private   bool                           checkOnClick                  = false ;
    private   Keys                           shortcutKeys                  = Keys.None ;
    private   string                         shortcutKeyDisplayString      = string.Empty ;
    private   bool                           showShortcutKeys              = true ;
    private   bool                           visible                       = true ;
    private   string                         toolTipText                   = string.Empty ;

    // délégués uniques pour abonnement aux événements des cibles          
    private   EventHandler                   dgTargetStandardClick         = null;
    private   EventHandler                   dgTargetCheckStateChanged     = null;
    private   ToolBarButtonClickEventHandler dgTargetToolBarButtonClick    = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public Action() {
      //Psl.Tracker.Tracker.Track( "Action.cctor(empty)" );
      targets = new TargetList( this );
      updatableProperties = TypeDescriptor.GetProperties( this, new Attribute[] { new UpdatablePropertyAttribute() } );

      dgTargetStandardClick      = OnTargetStandardClick;
      dgTargetCheckStateChanged  = OnTargetCheckStateChanged;
      dgTargetToolBarButtonClick = OnTargetToolBarButtonClick;
    }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="container">conteneur dans lequel insérer l'action</param>
    public Action( System.ComponentModel.IContainer container ) : this() {
      //Psl.Tracker.Tracker.Track( "Action.cctor(container)" );
      container.Add( this );
    }

    /// <summary>
    /// Libération des ressources, c'est-à-dire dé-liaison de tous les liens
    /// </summary>
    /// <param name="disposing">true si les données non managées doivent être libérées</param>
    protected override void Dispose( bool disposing ) {
      if (disposing && (state != ActionState.Disposing) && (state != ActionState.Disposed)) {
        //Psl.Tracker.Tracker.Track( "Action.Dispose" );
        state = ActionState.Disposing;
        try {
          while (targets.Count > 0)
            DoRemoveTarget( targets[ 0 ] );
          if (owner != null) owner.Actions.Remove( this );
        }
        finally {
          state = ActionState.Disposed ;
        }
      }
      base.Dispose( disposing ) ;
    }

    /// <summary>
    /// Redéfinition de la propriété Site pour accéder au service IComponentChangeService
    /// </summary>
    public override ISite Site {
      get { return base.Site; }
      set {
        base.Site = value;
        changeService = GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
      }
    }

    /// <summary>
    /// Obtient ou détermine la liste d'actions propriétaire d'une action
    /// </summary>
    /// <remarks>
    /// Sachant que ce sont les listes d'actions qui sont tenues au courant de la phase 
    /// de construction, l'état par défaut lors de l'initialisation d'une action est Building
    /// jusqu'à ce que la phase de construction soit terminée. Cependant, lors de la création
    /// d'une action en mode conception, l'événement de fin de phase de construction n'arrivera
    /// évicemment jamais (appel de DoBuildComplete), de sorte qu'il faut libérer
    /// au plus tôt l'action de son état Building. Le premier moment où cela est possible,
    /// c'est lorsque l'action est insérée dans la liste de sa liste d'action propriétaire, 
    /// ce qui se traduit par l'invocation du setter de la propriété Owner. C'est là qu'il faut
    /// ajuster l'état de l'action si la liste de d'actions propriétaire est sortie de la phase
    /// de construction.
    /// </remarks>
    [
     Browsable( false ),
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public ActionList Owner { // protected internal ActionList Owner {
      get { return owner; }
      set {
        owner = value;
        if ( owner == null || owner.IsBuilding || !IsBuilding ) return;
        DoBuildComplete();
      }
    }

    /// <summary>
    /// Retourne true si l'état interne de l'action est "Building"
    /// </summary>
    /// <remarks>
    /// L'état "Building" s'étend depuis la création de l'action jusqu'à l'appel à DoBuildComplete
    /// qui correspond à l'invocation EndInit sur la liste d'actions. 
    /// </remarks>
    protected internal bool IsBuilding {
      get {
        return state == ActionState.Building;
      }
    }

    //
    // Notification des changements
    //

    /// <summary>
    /// Notifie auprès du designer associé à l'action que le composant va être modifié.
    /// </summary>
    /// <remarks>
    /// Permet de faire savoir au designer associé à l'action que le composant Action a été
    /// modifié en dehors des actions explicites de l'utilisateur sur l'ihm de conception.
    /// <br/>
    /// Les notifications de changement doivent être inhibées durant la phase de construction 
    /// de l'action de manière à éviter qu'un concepteur soit marqué modifié dès son chargement.
    /// </remarks>
    /// <param name="target">composant cible concerné par la modification</param>
    private void DoNotifyChanging( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanging (((, action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      if ( changeService == null || IsBuilding ) return;
      changeService.OnComponentChanging( this  , null );
      changeService.OnComponentChanging( target, null );
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanging ))), action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    /// <summary>
    /// Notifie auprès du designer associé à l'action que le composant a été modifié.
    /// </summary>
    /// <remarks>
    /// Voir les remarques associées à la méthode <see cref="DoNotifyChanging"/>
    /// <br/>
    /// Les notifications de changement doivent être inhibées durant la phase de construction 
    /// de l'action de manière à éviter qu'un concepteur soit marqué modifié dès son chargement.
    /// </remarks>
    /// <param name="target">composant cible concerné par la modification</param>
    private void DoNotifyChanged( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanged (((, action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      if ( changeService == null || IsBuilding ) return;
      changeService.OnComponentChanged( target, null, null, null );
      changeService.OnComponentChanged( this  , null, null, null );
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanged ))), action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    //
    // Contrôle de cohérence
    //

                                                                               // <wao DoCheckOwner.begin>
    /// <summary>
    /// Contrôle que l'action est actuellement liée à une liste d'actions
    /// </summary>
    protected void DoCheckOwner() {
      if (owner == null) 
        throw new EActionList( "L'action n'est pas liée à une liste d'actions" ) ;
    }
                                                                               // <wao DoCheckOwner.end>

    #region Gestion des collections
    //
    // Gestion de la collection des cibles
    //

    /// <summary>
    /// Mise à jour des cibles lors de la fin de la construction d'une action
    /// </summary>
    /// <remarks>
    /// Pendant la phase de construction, les cibles sont simplement enregistrées dans la liste des
    /// cibles sans être mises à jour. Lorsque l'action termine sa construction, cette méthode
    /// effectue le backtracking sur les cibles enregistrées de manière à les attacher à l'action.
    /// </remarks>
    internal void DoBuildComplete() {

      // attacher toutes les cibles en attente
      foreach (Component target in targets) {
        DoAttachTarget( target );
      }

      // déverrouiller la notification des changements et l'attachement des cibles dans la liste des cibles
      state = ActionState.Standard;
    }

    /// <summary>
    /// Attachement d'une cible à une action
    /// </summary>
    /// <remarks>
    /// Méthode de service appelée par DoBuildComplete et TargetList.Add
    /// pour attacher une cible à l'action. Cette méthode redirige le traitement sur la méthode
    /// <see cref="ActionProvider.SetAction"/> qui est l'unique méthode pilotant la gestion des cibles.
    /// C'est cette méthode qui effectuera les appels appropriés à <see cref="DoAddTarget"/> et 
    /// <see cref="DoRemoveTarget"/>.
    /// </remarks>
    /// <param name="target">cible à attacher</param>
    internal void DoAttachTarget( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoAttachTarget +, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      ActionProvider.TryGetActionProvider( owner.Site ).SetAction( target, this );
      //Psl.Tracker.Tracker.Track( "Action.DoAttachTarget -, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    /// <summary>
    /// Détachement de toutes les cibles d'une action
    /// </summary>
    /// <remarks>
    /// Méthode de service appelée par TargetList.Clear pour détacher toutes les cibles
    /// de l'action. Cette méthode redirige le traitement sur la méthode <see cref="ActionProvider.SetAction"/> 
    /// qui est l'unique méthode pilotant la gestion des cibles. C'est cette méthode qui effectuera les appels 
    /// appropriés à <see cref="DoRemoveTarget"/>.
    /// </remarks>
    internal void DoDetachTargets() {
      while ( targets.Count > 0 )
        ActionProvider.TryGetActionProvider( owner.Site ).SetAction( targets[ targets.Count - 1 ], null );
    }

    /// <summary>
    /// Adjonction d'une cible aux cibles de l'action
    /// </summary>
    /// <remarks>
    /// Méthode appelée depuis <see cref="ActionProvider.SetAction"/> pour effectuer la mise à jour
    /// de la cible conformément aux données de l'action.
    /// <br/>
    /// Cette méthode suppose que la cible a déjà été déliée de toute action.
    /// </remarks>
    /// <param name="target">cible à ajouter</param>
    internal void DoAddTarget( Component target ) {
      DoNotifyChanging( target );
      try {
        //Psl.Tracker.Tracker.Track( "Action.DoAddTarget +, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
        if ( !targets.Contains( target ) ) targets.Add( target );
        DoRefreshProperties( target );
        DoAddHandlers( target );
        OnTargetAdded( target );
        //Psl.Tracker.Tracker.Track( "Action.DoAddTarget -, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      } finally { DoNotifyChanged( target ); }
    }

    /// <summary>
    /// Méthode appelée depuis <see cref="ActionProvider.SetAction"/> pour délier une cible de l'action
    /// </summary>
    /// <remarks>
    /// Cette méthode n'effectue aucun traitement si la cible ne figurait pas dans la liste des cibles
    /// de l'action.
    /// </remarks>
    /// <param name="target">cible à délier</param>
    /// <returns>true si la cible a effectivement été déliée</returns>
    internal bool DoRemoveTarget( Component target ) {
      DoNotifyChanging( target );
      try {
        if ( !targets.Remove( target ) ) return false;
        DoRemoveHandlers( target );
        OnTargetRemoved( target );
        //Psl.Tracker.Tracker.Track( "Action.DoRemoveTarget, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
        return true;
      } finally { DoNotifyChanged( target ); }
    }

    /// <summary>
    /// Méthode appelée lorsqu'une cible a été détachée de l'action (pour extensions)
    /// </summary>
    /// <param name="target">cible détachée</param>
    protected virtual void OnTargetRemoved( Component target ) { }

    /// <summary>
    /// Méthode appelée lorsqu'une cible a été attachée à l'action (pour extensions)
    /// </summary>
    /// <param name="target">cible attachée</param>
    protected virtual void OnTargetAdded( Component target ) { }

    #endregion

    #region Mise à jour des contrôles cibles
    //
    // Mise à jour des contrôles cible
    //

    /// <summary>
    /// Détermine si une cible est effectivement validée.
    /// </summary>
    /// <remarks>
    /// Cette méthode de service tient compte de la propriété Enabled de la liste d'actions
    /// propriétaire de l'action, laquelle est prioritaire sur la propriété Enabled de l'action.
    /// </remarks>
    /// <returns>true si la cible est effectivement validée</returns>
    protected virtual bool DoGetTargetEnabled() {
        return enabled && (owner == null ? true : owner.Enabled);
    }

    /// <summary>
    /// Détermine si une cible est effectivement visible ou non.
    /// </summary>
    /// <remarks>
    /// Cette méthode de service tient compte de la propriété Visible de la liste d'actions
    /// propriétaire de l'action, laquelle est prioritaire sur la propriété Visible de l'action.
    /// En outre, en mode conception, les cibles sont maintenues toujours visibles pour ne pas 
    /// empêcher l'utilisation des services de l'ide en mode conception. 
    /// </remarks>
    /// <returns>true si la cible est effectivement visible</returns>
    protected virtual bool DoGetTargetVisible() {
        return DesignMode ? true : (visible && (owner == null ? true : owner.Visible));
    }

    /// <summary>
    /// Mise à jour d'une propriété de l'action auprès d'une cible
    /// </summary>
    /// <remarks>
    /// Cette méthode traite le cas particulier des conseils (propriété ToolTipText) 
    /// selon la nature de la cible. Pour les propriétés autres que ToolTipText, la mise à jour
    /// s'effectue par réflexivité (via le cache types.SetValue) en style bypass.
    /// <br/>
    /// Dans le cas de la propriété ToolTipText, deux cas peuvent se présenter, selon que 
    /// la cible fait ou non partie des composants qui peuvent être étendus par un ToolTip provider 
    /// (test via CanExtend) :
    /// <br/>
    /// 1) si oui, la mise à jour de la propriété s'effectue via le ToolTip provider local de
    /// la liste d'actions (la propriété n'est pas affichée en mode conception au niveau de la cible) ;
    /// <br/>
    /// 2) si non, il est présumé qu'il s'agit d'un composant au moins Net 2.0 qui comporte une
    /// propriété normale "ToolTipText", auquel cas la mise à jour s'effectue normalement 
    /// par réflexivité via types.SetValue ; si le composant ne comporte pas une telle propriété,
    /// la mise à jour est simplement ignorée (style bypass)
    /// </remarks>
    /// <param name="target">cible à mettre à jour</param>
    /// <param name="propertyName">nom de la propriété à mettre à jour</param>
    /// <param name="value">nouvelle valeur de la propriété</param>
    private void DoRefreshProperty( Component target, string propertyName, object value ) {
      if ( target == null ) return;

      workingState = WorkingState.Driving;
      try {

        // déterminer si la cible requiert ou non l'extension d'un provider ToolTip
        bool needsProvider = (owner != null)
          && (propertyName == "ToolTipText")
          && owner.ToolTipProvider.CanExtend( target );

        // mise à jour selon le cas
        if ( needsProvider )
          owner.ToolTipProvider.SetToolTip( target as Control, (string) value );
        else
          types.SetValue( propertyName, target, value );

      } finally { workingState = WorkingState.Listening; }
    }

    /// <summary>
    /// Mise à jour d'une propriété de l'action auprès de toutes les cibles de l'action
    /// </summary>
    /// <param name="propertyName">nom de la propriété à mettre à jour</param>
    /// <param name="value">nouvelle valeur de la propriété</param>
    protected void DoRefreshPropertyInAll( string propertyName, object value ) {
      foreach ( Component target in targets )
        DoRefreshProperty( target, propertyName, value );
    }

    /// <summary>
    /// Mise à jour de toutes les propriétés prioritaires issues la liste d'action propriétaire
    /// auprès de toutes les cibles de l'action.
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclenchée depuis la liste d'action propriétaire dès lors que l'une
    /// des propriétés prioritaires liées à la liste d'action est modifiée. 
    /// </remarks>
    internal void DoRefreshOverridesInAll() {
      DoRefreshPropertyInAll( "Enabled", DoGetTargetEnabled() );
      DoRefreshPropertyInAll( "Visible", DoGetTargetVisible() );
    }

    /// <summary>
    /// Obtient le nom de l'événement considéré comme click pour un type de cible
    /// </summary>
    /// <param name="defaultName">nom par défaut de l'événement clic</param>
    /// <param name="target">cible visée</param>
    /// <remarks>
    /// Ce calcul est lié au fait que certains composants exposent un événement dont la
    /// sémantique est un clic convenant à une action mais qui ne sont pas exposés
    /// sous le nom usuel "Click". Qui plus est, certains de ces composants exposent
    /// aussi un événement nommé "Click" mais qui ne convient pas à la sémantique des actions.
    /// D'où ce filtrage qui doit se faire au niveau du type des cibles. 
    /// <br/>
    /// Ce filtrage doit cependant rediriger sur un événement de signature standard "object, EventArgs".
    /// Pour les événements qui ne sont pas de signature standard, il faut opérer en jouant
    /// sur le type de délégué dans la méthode AddHandler du cache des types.
    /// </remarks>
    /// <returns>le nom de l'événement clic pour la cible</returns>
    protected virtual string DoGetClickEventName( string defaultName, object target ) {
      string result = defaultName;

      // expose aussi un événement Click, mais concerne le bouton dans son ensemble (bouton ou flèche)
      if ( target is ToolStripSplitButton )
        result = "ButtonClick";

      // retourne le nom redirigé
      return result;
    }

    /// <summary>
    /// Abonne, auprès d'une cible, les handlers requis pour que l'action puisse gérer la cible
    /// </summary>
    /// <param name="target">composant cible concerné</param>
    protected virtual void DoAddHandlers( Component target ) {
      string clickEventName = DoGetClickEventName( "Click", target );

      types.AddHandler( clickEventName     , target, dgTargetStandardClick      );
      types.AddHandler( "CheckStateChanged", target, dgTargetCheckStateChanged  );
      types.AddHandler( "ButtonClick"      , target, dgTargetToolBarButtonClick );
    }

    /// <summary>
    /// Désabonne, auprès d'une cible, les handlers requis pour que l'action puisse gérer la cible
    /// </summary>
    /// <param name="target">composant cible concerné</param>
    protected virtual void DoRemoveHandlers( Component target ) {
      string clickEventName = DoGetClickEventName( "Click", target );

      types.RemHandler( clickEventName     , target, dgTargetStandardClick      );
      types.RemHandler( "CheckStateChanged", target, dgTargetCheckStateChanged  );
      types.RemHandler( "ButtonClick"      , target, dgTargetToolBarButtonClick );
    }

    /// <summary>
    /// Mise à jour de toutes les propriétés "updatables" de l'action auprès d'une cible donnée.
    /// </summary>
    /// <param name="target">cible concernée</param>
    private void DoRefreshProperties( Component target ) {
      foreach (PropertyDescriptor property in updatableProperties) {
        DoRefreshProperty(target, property.Name, property.GetValue(this));
      }
    }

    #endregion

    #region Gestion des événements des cibles
    //
    // Gestion des événements des cibles
    //

    /// <summary>
    /// Méthode de service pour le traitement associé à un événement Click d'une cible
    /// </summary>
    /// <param name="sender">source de l'événement Click</param>
    /// <param name="e">descripteur de l'événement</param>
    private void DoHandleTargetClick( object sender, EventArgs e ) {
      if (workingState != WorkingState.Listening) return;
      DoActionExecute();
    }

    /// <summary>
    /// Gestionnaire appelé par une cible pour un événement Click dans le cas général des Control
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnTargetStandardClick(object sender, EventArgs e) {
      DoHandleTargetClick(sender, e);
    }

    /// <summary>
    /// Gestionaire appelé par une cible pour un événement ButtonClick dans le cas particulier des Toolbar
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnTargetToolBarButtonClick( object sender, ToolBarButtonClickEventArgs e ) {
      if (! targets.Contains( e.Button )) return;
      DoHandleTargetClick( e.Button, e );
    }

    /// <summary>
    /// Gestionnaire appelé par une cible pour un événement CheckStateChanged
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnTargetCheckStateChanged( object sender, EventArgs e ) {
      if (workingState != WorkingState.Listening) return;
      if (owner == null) return;

      Component target = sender as Component;
      CheckState = (CheckState) types.GetValue( "CheckState", sender );
    }

    #endregion

    #region Protocole des actions

    //
    // Protocole des actions
    //
                                                                               // <wao DoWrapException.begin>
    /// <summary>
    /// Ajoute un objet Exception à la liste des exceptions du rapport de l'action.
    /// </summary>
    /// <param name="report">exception synthétique à relancer</param>
    /// <param name="exception">objet exception à ajouter</param>
    /// <param name="step">identification de l'étape où l'exception s'est produite</param>
    private void DoWrapException( ref EActionList report, Exception exception, string step) {

      // compléter les information relatives à l'action
      exception.Data.Add( 
        "Remarque", 
        "Exception interceptée pendant une action de type " + GetType().FullName );
      exception.Data.Add( 
        "Etape", 
        step );

      // créer l'exception enveloppe si nécessaire
      if ( report == null ) 
        report = new EActionList( "Exception(s) déclenchée(s) pendant une action" );

      /*                                                                       // <wao never>
      // ajouter l'exception à la liste des exceptions interceptées
      report.Data.Add( 
        "Embedded", 
        exception );
      */                                                                       // <wao never>
      report.ContainedExceptions.Add( exception );                             // <wao never>
    }
                                                                               // <wao DoWrapException.end>
                                                                               // <wao DoActionExecute.begin>
    /// <summary>
    /// Méthode unique de déclenchement du protocole d'action.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 1. Tout déclenchement du corps d'une action (via un bouton, un élément de menu,
    /// l'appel à Perform, etc.) passe par cette méthode : DoActionExecute est donc le point d'entrée
    /// du protocole des actions. 
    /// </para>
    /// <para>
    /// 2. Le corps de l'action (événement Execute) sera déclenché depuis la méthode
    /// OnExecute qui sera appelée depuis la méthode DoActionExecute de ActionList.
    /// </para>
    /// <para>
    /// o Le protocole déclenche, dans l'ordre :<br/>
    ///   - ActionGlobalBefore (diffusé à toutes les listes d'action de l'application)<br/>
    ///   - ActionExecuting    (au niveau de la liste à laquelle appartient l'action)<br/>
    ///   - Execute            (corps de l'action à exécuter)<br/>
    ///   - ActionExecuted     (au niveau de la liste à laquelle appartient l'action)<br/>
    ///   - ActionGlobalAfter  (diffusé à toutes les listes d'action de l'application)<br/>
    /// <br/>
    /// o Les différentes étapes sont étanches les unes par rapport aux du point de vue
    /// de la gestion des exceptions
    /// <br/>
    /// o L'exception ECancelled est interceptée comme exception silencieuse au niveau
    ///   du corps de l'action seulement
    ///   <br/>
    /// o Si au moins une exception a été traitée, une unique exception EActionList est 
    ///   déclenchée en fin de protocole, contenant dans sa collection Data la liste de tous
    ///   les objets exception interceptés (avec leur stanck trace intacte) 
    /// </para>
    /// <para>
    /// (note du 19 04 2008) Le recours à la suveillance de l''événement CheckStateChanged des
    /// cibles a pour conséquence que les composants qui ne comportent pas un tel événement 
    /// (par exemple Button) ne peuvent pas basculer la propriété CheckState par un simple clic.
    /// <br/>
    /// Il faudra améliorer cet aspect, probablement en insérant un test approprié dans le handler
    /// des clics des cibles (basculer CheckState si la cible n'expose pas d'événement CheckStateChanged).
    /// </para>
    /// </remarks>
    internal protected virtual void DoActionExecute() {
      EActionList report = null;
      ECancelled cancelled = null;

      // vérifier que l'action est lliée à une liste d'action
      DoCheckOwner();

      // si l'action n'est pas actuellement validée, ne rien faire
      if ( !Enabled ) return;

      // diffusion de l'événement ActionGlobalBefore
      try { ActionList.FireActionGlobalBeforeEvent( this, EventArgs.Empty ); }
      catch ( Exception x ) { DoWrapException( ref report, x, "événement ActionGlobalBefore" ); }

      // diffusion de l'événement ActionExecuting
      try { owner.OnActionExecuting( this, EventArgs.Empty ); } 
      catch ( Exception x ) { DoWrapException( ref report, x, "événement ActionExecuting" ); }

      // diffusion de l'événement Execute
      try { OnExecute(); }
      catch ( ECancelled x ) { cancelled = x; }
      catch ( Exception x ) { DoWrapException( ref report, x, "événement Execute" ); }

      // Attention !                                                                      // <wao never.begin> 
      // L'exécution de certaines actions (en particulier la fermeture de la fenêtre hébergeant l'action)
      // peut provoquer la restitution des ressources et délier l'action de sa liste d'action.
      // D'où le test renouvelé de owner pour éviter les exceptions non souhaitables      // <wao never.end>
      // diffusion de l'événement ActionExecuted (sauf si restitution de ressources)
      if ( owner != null ) {
        try { owner.OnActionExecuted( this, EventArgs.Empty ); }
        catch ( Exception x ) { DoWrapException( ref report, x, "événement ActionExecuted" ); }
      }

      // diffusion de l'événement ActionGlobalAfter
      try { ActionList.FireActionGlobalAfterEvent( this, EventArgs.Empty ); }
      catch ( Exception x ) { DoWrapException( ref report, x, "événement ActionGlobalAfter" ); }

      // bilan des exceptions
      if ( report != null ) throw report;
      if ( cancelled != null ) throw cancelled;
    }
                                                                               // <wao DoActionExecute.end>
    
    #endregion

    #region Déclenchement centralisé des événements
                                                                               // <wao OnExecute.begin>
    /// <summary>
    /// Déclenchement centralisé de l'événement Execute de l'action
    /// </summary>
    protected virtual void OnExecute() {
      if (Execute != null) Execute( this, EventArgs.Empty );
    }
                                                                               // <wao OnExecute.end>

    /// <summary>
    /// Déclenchement centralisé de l'événement Update de l'action
    /// </summary>
    protected virtual void OnUpdate() {
      if (Update != null) Update( this, EventArgs.Empty );
    }

    #endregion

    #region Fonctionnalités

                                                                               // <wao Perform.begin>
    /// <summary>                                                               
    /// Déclenchement du protocole d'action par programme.
    /// </summary>
    /// <remarks>
    /// Cette méthode publique permet de déclencher une action par programme en
    /// appliquant le protocole d'actions exactement comme si l'action 
    /// avait été déclenchée par l'ihm (élément de menu, bouton, etc.)
    /// </remarks>
    public void Perform() {
      DoActionExecute();
    }                                                                          // <wao Perform.end>

    /// <summary>
    /// Déclenche la mise à jour de l'action.
    /// </summary>
    public void DoUpdate() {
      OnUpdate();
    }

    #endregion

    #region Propriétés publiques
    //
    // Propriétés publiques
    //

    /// <summary>
    /// Accès à la collection des cibles, invisible en mode conception
    /// </summary>
    /// <remarks>
    /// Cette propriété est exposée pour permettre la sérialisation de la liste des cibles
    /// </remarks>
    [
      Browsable( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    ]
    public ICollection<Component> Targets {
      get {
        //Psl.Tracker.Tracker.Track( "Action.geTargets, count=" + targets.Count + ", action=" + this.ToString() );
        return targets;
      }
    }

    /// <summary>
    /// Obtient ou détermine le texte utilisé comme libellé dans les contrôles associés à l'action
    /// </summary>
    [ 
      DefaultValue( "" ), 
      UpdatableProperty(), 
      Localizable( true ),
      Description( "Obtient ou détermine le texte utilisé comme libellé dans les contrôles associés à l'action" )
    ]
    public string Text {
      get { return text; }
      set {
        if (text == value) return;
        text = value;
        DoRefreshPropertyInAll( "Text", text );
      }
    }

    /// <summary>
    /// Obtient ou détermine si un clic sur un contrôle associé à l'action bascule la propriété Checked
    /// </summary>
    [
      DefaultValue( false ),
      UpdatableProperty,
      Description( "Obtient ou détermine si un clic sur un contrôle associé à l'action bascule la propriété Checked" )
    ]
    public bool CheckOnClick {
      get { return checkOnClick; }
      set {
        if ( checkOnClick == value ) return;
        checkOnClick = value;
        DoRefreshPropertyInAll( "CheckOnClick", CheckOnClick );
      }
    }

    /// <summary>
    /// Obtient ou détermine l'état Checked de l'action et de ses cibles
    /// </summary>
    /// <remarks>
    /// Il s'agit d'une propriété dérivée de la propriété CheckState. Elle n'est donc pas sérialisée, 
    /// et ne provoque pas directement de mise à jour auprès des cibles (voir la propriété CkeckState).
    /// </remarks>
    [
      DefaultValue( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Description( "Obtient ou détermine si les contrôles associés à l'actions sont cochés ou non" )
    ]
    public bool Checked {
      get { return (checkState != CheckState.Unchecked); }
      set {
        if (value == Checked) return;
        CheckState = value ? CheckState.Checked : CheckState.Unchecked;
      }
    }

    /// <summary>
    /// Obtient ou détermine l'état CheckState d'une action et de ses cibles
    /// </summary>
    [
      DefaultValue( CheckState.Unchecked ), 
      UpdatableProperty(),
      Description( "Obtient ou détermine l'état de la propriété CheckedState des contrôles associés à l'action" )
    ]
    public CheckState CheckState {
      get { return checkState; }
      set {
        if (checkState == value) return;
        checkState = value;
        DoRefreshPropertyInAll( "CheckState", checkState );
      }
    }

    /// <summary>
    /// Obtient ou détermine l'état de validation de l'action et de ses cibles.
    /// </summary>
    /// <remarks>
    /// Cette propriété reflète l'état de validation local (pour l'action).
    /// Sachant que la propriété Enabled de la liste d'action propriétaire est prioritaire,
    /// les cibles ne seront armées que si les deux propriétés Enabled sont true.
    /// </remarks>
    [
      DefaultValue( true ), 
      UpdatableProperty,
      Description( "Obtient ou détermine si les contrôles associés à l'action sont armés ou inhibés" )
    ]
    public bool Enabled {
      get { return enabled ; }
      set {
        if (enabled == value) return;
        enabled = value;
        DoRefreshPropertyInAll( "Enabled", DoGetTargetEnabled() );
      }
    }

    /// <summary>
    /// Obtient ou détermine l'état de visibilité de l'action et de ses cibles
    /// </summary>
    /// <remarks>
    /// Cette propriété reflète l'état de visibilité local (pour l'action).
    /// Sachant que la propriété Visible de la liste d'action propriétaire est prioritaire,
    /// les cibles ne seront visibles que si les deux propriétés Visible sont true.
    /// Toutefois, en mode conception, les cibles sont maintenues toujours visibles pour
    /// ne pas perturber les possibilités de l'ide.
    /// </remarks>
    [
      DefaultValue( true ), 
      UpdatableProperty,
      Description( "Obtient ou détermine si les contrôles associés à l'action sont visibles" )
    ]
    public bool Visible {
      get { return visible; }
      set {
        if (visible == value) return;
        visible = value;
        DoRefreshPropertyInAll( "Visible", DoGetTargetVisible() ) ; 
      }
    }

    /// <summary>
    /// Obtient ou détermine l'image associée à l'action et à ses cibles
    /// </summary>
    [
      DefaultValue( null ), 
      UpdatableProperty,
      Description( "Obtient ou détermine l'image utilisée par les contrôles associés à l'action" )
    ]
    public Image Image {
      get { return image; }
      set {
        if (image == value) return;
        image = value;
        DoRefreshPropertyInAll( "Image", Image );
      }
    }

    /// <summary>
    /// Détermine si la propriété ImageTransparentColor doit être sérialisée 
    /// </summary>
    /// <returns>true si la propriété ImageTransparentColor doit être sérialisée</returns>
    protected bool ShouldSerializeImageTransparentColor() {
      //Psl.Tracker.Tracker.Track( "Action.ShouldSerializeImageTransparentColor" );
      return ImageTransparentColor != defaultImageTransparentColor;
    }

    /// <summary>
    /// Obtient ou détermine la couleur transparente pour l'image de l'action
    /// </summary>
    [
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou détermine la couleur transparente pour l'image de l'action" ) 
    ]
    public Color ImageTransparentColor {
      get { return imageTransparentColor; }
      set {
        if (imageTransparentColor == value) return;
        imageTransparentColor = value;
        DoRefreshPropertyInAll( "ImageTransparentColor", ImageTransparentColor );
      }
    }

    /// <summary>
    /// Obtient ou détermine la touche de raccourci qui déclenche l'action
    /// </summary>
    [
      DefaultValue( Keys.None ), 
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou détermine la touche de raccourci clavier qui déclenche l'action" )
    ]
    public Keys ShortcutKeys {
      get { return shortcutKeys; }
      set {
        if (shortcutKeys == value) return;
        shortcutKeys = value;
        DoRefreshPropertyInAll( "ShortcutKeys", value );
      }
    }

    /// <summary>
    /// Obtient ou détermine la chaîne à afficher en tant que raccourci clavier
    /// </summary>
    [
      DefaultValue( "" ),
      UpdatableProperty,
      Localizable( true ),
      Description( "Obtient ou détermine la chaîne à afficher en tant que raccourci clavier" )
    ]
    public string ShortcutKeyDisplayString {
      get { return shortcutKeyDisplayString; }
      set {
        if (shortcutKeyDisplayString == value) return;
        shortcutKeyDisplayString = value;
        DoRefreshPropertyInAll( "ShortcutKeyDisplayString", value );
      }
    }

    /// <summary>
    /// Obtient ou détermine si les raccourcis clavier doivent être affichés dans les menus
    /// </summary>
    [
      DefaultValue( true ),
      UpdatableProperty,
      Localizable( true ),
      Description( "Obtient ou détermine si les raccourcis clavier doivent être affichés dans les menus" )
    ]
    public bool ShowShortcutKeys {
      get { return showShortcutKeys; }
      set {
        if (showShortcutKeys == value) return;
        showShortcutKeys = value;
        DoRefreshPropertyInAll( "ShowShortcutKeys", value );
      }
    }

    /// <summary>
    /// Obtient ou détermine le texte utilisé dans les bulles d'aide des contrôles associés à l'action 
    /// </summary>
    [
      DefaultValue( "" ), 
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou détermine le texte utilisé dans les bulles d'aide des contrôles associés à l'action" ) 
    ]
    public string ToolTipText {
      get { return toolTipText; }
      set {
        if (toolTipText == value) return;
        toolTipText = value;
        DoRefreshPropertyInAll( "ToolTipText", ToolTipText );
      }
    }

    /// <summary>
    /// Evenement déclenché lorsque l'action est appliquée
    /// </summary>
    [
      Description( "Evenement déclenché lorsque l'action est appliquée" ) 
    ]
    public event EventHandler Execute;

    /// <summary>
    /// Evénement déclenché (sur Idle) pour permettre la mise à jour des propriétés de l'action
    /// </summary>
    [
      Description( "Evénement déclenché (sur Idle) pour permettre la mise à jour des propriétés de l'action" )
    ]
    public event EventHandler Update;

    #endregion

  }
}
