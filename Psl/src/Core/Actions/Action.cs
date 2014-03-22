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
 * todo (Action) : basculement CheckState pour les composants qui n'exposent pas d'�v�nement CheckStateChanged
 * todo (Action) : le composant CheckBox expose une propri�t� nomm�e AutoCheck et non CheckOnClick
 * todo (Action) : migrer le traitement li� au mode conception dans le concepteur
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 08 03 2008 : adjonction de la notification des changements pour la liste des cibles
 * 08 03 2008 : am�lioration de l'impl�mentation de la liste des cibles par redirection de ICollection(Component)
 * 08 03 2008 : la propri�t� Targets est maintenant de type ICollection(Component)
 * 08 03 2008 : suppression de la gestion (erron�e) de la propri�t� Checked dans DoActionExecute
 * 09 03 2008 : nettoyage de divers aspects du code
 * 19 04 2008 : mise � niveau de la mise � jour de la pt� ToolTipText pour les composants Net 2.0 ou ult�rieur
 * 09 11 2008 : refonte du protocole des actions dans Action avec prise en charge am�lior�e des exceptions
 * 25 11 2008 : test de owner != null dans DoActionExecute en cas de fermeture de la fen�tre h�te
 * 22 01 2009 : utilisation de ExceptionContainer comme pr�fixe EActionList pour contourner le pbm des exceptions non s�rialisables
 * 22 05 2009 : adjonction de DoGetClickEventName pour prendre en charge le cas des ToolStripSplitButton
 * 12 02 2010 : r�sorption de la bogue des ruptures d'attachement des actions due � une gestion incorrecte de l'�tat "building"
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

  #region Cache acc�l�rateur de la description des types des cibles
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //             Cache acc�l�rateur de la description r�flexive des types des cibles
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  public partial class Action {

    // <exclude/>
    /// <summary>
    /// Cache acc�l�rateur de la description r�flexive des types des cibles
    /// </summary>
    /// <remarks>
    /// Le cache acc�l�rateur est � la fois adaptatif et automatique, et se construit de lui-m�me
    /// � la vol�e au fur et � mesure des demandes qui lui sont adress�es. Il ne recherche qu'une
    /// seule fois les informations r�flexives de propri�t�s et de m�thodes dans la description
    /// r�flexive compl�te d'un type de cible donn�.
    /// <br/>
    /// Le cache expose les m�thodes permettant d'acc�der aux propri�t�s ou aux �v�nements
    /// des dif�rentes cibles.
    /// </remarks>
    protected struct TargetTypeCache {


      // <exclude
      /// <summary>
      /// Descripteur de l'extrait des informations r�flexives li�es � un type de cible donn�
      /// </summary>
      /// <remarks>
      /// Cette structure est embo�t�e dans la struture qui g�re le cache. Les propri�t�s et m�thodes
      /// de cette structure ne sont pas destin�es � �tre appel�es "de l'ext�rieur", mais seulement
      /// depuis les m�thodes du cache lui-m�me.
      /// </remarks>
      public struct TypeData {

        /// <summary>
        /// Type de la cible concern�
        /// </summary>
        private Type type;

        /// <summary>
        /// Cache acc�l�rateur pour les prori�t�s
        /// </summary>
        private Dictionary<string, PropertyInfo> properties;

        /// <summary>
        /// Cache acc�l�rateur pour les �v�nements
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
        /// R�cup�rer les informations r�flexives sur une propri�t�
        /// </summary>
        /// <remarks>
        /// Cette m�thode permet de construire � la vol�e le cache acc�l�rateur (dictionnaire "properties").
        /// D�s que les informations sur une propri�t� sont recherch�es pour ce type, la m�thode m�morise
        /// dans le cache "properties" l'association entre le nom de la propri�t� et les informations
        /// reflexives li�es � cette propri�t� (ou null si la propri�t� ne figure pas dans ce type de cible). 
        /// </remarks>
        /// <param name="propertyName">nom de la propri�t�</param>
        /// <param name="propertyInfos">informations de reflexion sur la propri�t� ou null si la propri�t� est introuvable</param>
        /// <returns>true si les informations sur la propri�t� </returns>
        private bool TryGetPropertyInfos( string propertyName, out PropertyInfo propertyInfos ) {
          if ( !properties.TryGetValue( propertyName, out propertyInfos ) ) {
            propertyInfos = type.GetProperty( propertyName );
            properties.Add( propertyName, propertyInfos );
          }
          return propertyInfos != null;
        }

        /// <summary>
        /// R�cup�rer les informations r�flexives sur un �v�nement
        /// </summary>
        /// <remarks>
        /// Cette m�thode permet de construire � la vol�e le cache acc�l�rateur (dictionnaire "events").
        /// D�s que les informations sur un �v�nement sont recherch�es pour ce type, la m�thode m�morise
        /// dans le cache "events" l'association entre le nom de l'�v�nement et les informations
        /// reflexives li�es � cet �v�nement (ou null si l'�v�nement ne figure pas dans ce type de cible). 
        /// </remarks>
        /// <param name="eventName">nom de l'�v�nement</param>
        /// <param name="eventInfos">informations de reflexion sur l'�v�nement ou null si l'�v�nement est introuvable</param>
        /// <returns></returns>
        private bool TryGetEventInfos( string eventName, out EventInfo eventInfos ) {
          if ( !events.TryGetValue( eventName, out eventInfos ) ) {
            eventInfos = type.GetEvent( eventName );
            events.Add( eventName, eventInfos );
          }
          return eventInfos != null;
        }

        /// <summary>
        /// Affecter une valeur � une propri�t� d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si la propri�t� est introuvable (pas d'exception).
        /// Contruction automatique � la vol�e du cache des propri�t�s 
        /// </remarks>
        /// <param name="propertyName">nom de la propri�t�</param>
        /// <param name="target">r�f�rence sur la cible</param>
        /// <param name="value">valeur � affecter</param>
        public void SetValue( string propertyName, object target, object value ) {
          PropertyInfo propertyInfos;
          if ( !TryGetPropertyInfos( propertyName, out propertyInfos ) ) return;
          propertyInfos.SetValue( target, value, null );
        }

        /// <summary>
        /// Obtenir la valeur d'une propri�t� d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si la propri�t� est introuvable (pas d'exception).
        /// Contruction automatique � la vol�e du cache des propri�t�s 
        /// </remarks>
        /// <param name="propertyName">nom de la propri�t�</param>
        /// <param name="target">r�f�rence sur la cible</param>
        /// <returns>la valeur de la propri�t� ou null si introuvable</returns>
        public object GetValue( string propertyName, object target ) {
          PropertyInfo propertyInfos;
          if ( !TryGetPropertyInfos( propertyName, out propertyInfos ) ) return null;
          return propertyInfos.GetValue( target, null );
        }

        /// <summary>
        /// Abonne un d�l�gu� � un �v�nement d'une cible
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
        /// Contruction automatique � la vol�e du cache des �v�nements 
        /// </remarks>
        /// <param name="eventName">nom de l'�v�nement</param>
        /// <param name="target">r�f�rence sur la cible</param>
        /// <param name="eventHandler">d�l�gu� � abonner</param>
        public void AddHandler( string eventName, object target, Delegate eventHandler ) {
          EventInfo eventInfos;
          if ( !TryGetEventInfos( eventName, out eventInfos ) ) return;
          eventInfos.AddEventHandler( target, eventHandler );
        }

        /// <summary>
        /// D�sabonne un d�l�gu� pour un �v�nement d'une cible donn�e
        /// </summary>
        /// <remarks>
        /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
        /// Contruction automatique � la vol�e du cache des �v�nements
        /// </remarks>
        /// <param name="eventName">nom de l'�v�nement</param>
        /// <param name="target">r�f�rence sur la cible</param>
        /// <param name="eventHandler">d�l�gu� � d�sabonner</param>
        public void RemHandler( string eventName, object target, Delegate eventHandler ) {
          EventInfo eventInfos;
          if ( !TryGetEventInfos( eventName, out eventInfos ) ) return;
          eventInfos.RemoveEventHandler( target, eventHandler );
        }
      }

      /// <summary>
      /// Dictionnaire des caches propres � chaque type de cible
      /// </summary>
      private Dictionary<Type, TypeData> typeDataCache;

      /// <summary>
      /// Initialiseur
      /// </summary>
      /// <param name="dummy">pour forcer la diff�rence avec l'initialiseur par d�faut sans para�tres</param>
      public TargetTypeCache( int dummy ) {
        typeDataCache = new Dictionary<Type, TypeData>();
      }

      /// <summary>
      /// Acc�s au cache associ� � un type de cible donn�
      /// </summary>
      /// <remarks>
      /// Cette m�thode ne retourne jamais null.
      /// Elle construit � la vol�e le dictionnaire des caches d'information sur les types de cibles.
      /// </remarks>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <returns>le cache associ� � la cible</returns>
      public TypeData this[ object target ] {
        get {
          TypeData typeData;
          if ( typeDataCache.TryGetValue( target.GetType(), out typeData ) ) return typeData;

          // ajouter �ventuellement les informations de type
          typeData = new TypeData( target.GetType() );
          typeDataCache.Add( target.GetType(), typeData );
          return typeData;
        }
      }

      /// <summary>
      /// Affecter une valeur � une propri�t� d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si la propri�t� est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// </remarks>
      /// <param name="propertyName">nom de la propri�t�</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <param name="value">valeur � affecter</param>
      public void SetValue( string propertyName, object target, object value ) {
        this[ target ].SetValue( propertyName, target, value );
      }

      /// <summary>
      /// Obtenir la valeur d'une propri�t� d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si la propri�t� est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// </remarks>
      /// <param name="propertyName">nom de la propri�t�</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <returns>la valeur de la propri�t� ou null si introuvable</returns>
      public object GetValue( string propertyName, object target ) {
        return this[ target ].GetValue( propertyName, target );
      }

      /// <summary>
      /// Abonne un d�l�gu� � un �v�nement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// </remarks>
      /// <param name="eventName">nom de l'�v�nement</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <param name="eventHandler">d�l�gu� � abonner</param>
      public void AddHandler( string eventName, object target, Delegate eventHandler ) {
        this[ target ].AddHandler( eventName, target, eventHandler );
      }

      /// <summary>
      /// D�sabonne un d�l�gu� pour un �v�nement d'une cible donn�e
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// </remarks>
      /// <param name="eventName">nom de l'�v�nement</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <param name="eventHandler">d�l�gu� � d�sabonner</param>
      public void RemHandler( string eventName, object target, Delegate eventHandler ) {
        this[ target ].RemHandler( eventName, target, eventHandler );
      }

      /// <summary>
      /// Abonne un d�l�gu� de type ToolBarButtonClickEventHandler � un �v�nement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// <br/>
      /// Pour raisons de r�gularit�, la cible "source" est de type ToolBarButton, mais l'�v�nement
      /// est en fait associ� � barre d'outils ToolBar qui contient de ce bouton. La m�thode agit
      /// d'elle-m�me sur la barre d'outils contenant le bouton.
      /// </remarks>
      /// <param name="eventName">nom de l'�v�nement</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <param name="eventHandler">d�l�gu� � abonner</param>
      public void AddHandler( string eventName, object target, ToolBarButtonClickEventHandler eventHandler ) {
        ToolBarButton button = target as ToolBarButton;
        if ( button == null ) return;
        ToolBar parent = button.Parent;
        if ( parent == null ) return;
        this[ parent ].AddHandler( eventName, parent, eventHandler );
      }

      /// <summary>
      /// D�sabonne un d�l�gu� de type ToolBarButtonClickEventHandler � un �v�nement d'une cible
      /// </summary>
      /// <remarks>
      /// Programmation en style bypass si l'�v�nement est introuvable (pas d'exception).
      /// Contruction automatique � la vol�e des caches n�cessaires.
      /// <br/>
      /// Pour raisons de r�gularit�, la cible "source" est de type ToolBarButton, mais l'�v�nement
      /// est en fait associ� � barre d'outils ToolBar qui contient de ce bouton. La m�thode agit
      /// d'elle-m�me sur la barre d'outils contenant le bouton.
      /// </remarks>
      /// <param name="eventName">nom de l'�v�nement</param>
      /// <param name="target">r�f�rence sur la cible</param>
      /// <param name="eventHandler">d�l�gu� � d�sabonner</param>
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
    /// Classe interne associ�e � la liste des cibles
    /// </summary>
    /// <remarks>
    /// Permet d'exposer la collection des cibles comme ICollection(Component)
    /// en redirigeant les m�thodes d'ajonction et de suppression pour effectuer la gestion
    /// des cibles. 
    /// <br/>
    /// Permet en outre de retarder la gestion des cibles pendant la phase de construction de
    /// mani�re � �viter de risquer la mise � jour de cibles n'ayant pas encore �t� initialis�es.
    /// </remarks>
    protected class TargetList : List<Component>, ICollection<Component> {

      /// <summary>
      /// R�f�rence sur l'action Acton propri�taire de la liste
      /// </summary>
      private Action owner;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="owner">action propri�taire de la liste</param>
      internal TargetList( Action owner ) {
        this.owner = owner;
      }

      /// <summary>
      /// Vide la liste des cibles
      /// </summary>
      /// <remarks>
      /// R�impl�mentation du membre correspondant de ICollection.
      /// <br/>
      /// D�tourne le traitement via l'action propri�taire pour garantir la gestion des cibles.
      /// </remarks>
      void ICollection<Component>.Clear() {
        if ( owner.IsBuilding )
          base.Clear();
        else
          owner.DoDetachTargets();
      }

      /// <summary>
      /// Ajoute une cible � la liste
      /// </summary>
      /// <remarks>
      /// R�impl�mentation du membre correspondant de ICollection.
      /// <br/>
      /// D�tourne le traitement via l'action propri�taire pour grantir la gestion des cibles.
      /// <br/>
      /// C'est cette m�thode qui est utilis�e pour la d�s�rialisation du concepteur.
      /// </remarks>
      /// <param name="target">cible � ajouter</param>
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
      /// R�impl�mentation du membre correspondant de ICollection.
      /// <br/>
      /// D�tourne le traitement via l'action propri�taire pour grantir la gestion des cibles.
      /// </remarks>
      /// <param name="target">cible � supprimer</param>
      /// <returns>true si la cible a effectivement �t� supprim�e</returns>
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
  //                             Impl�mentation des actions
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Impl�mentation des actions.
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
    /// Enum�ration pour l'�tat global de l'action relativement � son cycle de vie
    /// </summary>
    protected enum ActionState {

      /// <summary>
      /// L'action est en cours de construction
      /// </summary>
      Building,

      /// <summary>
      /// L'action est op�rationnelle
      /// </summary>
      Standard,

      /// <summary>
      /// L'action est en train de lib�rer ses ressources
      /// </summary>
      Disposing,

      /// <summary>
      /// L'action a d�j� lib�r� toutes ses ressources
      /// </summary>
      Disposed
    }

    /// <summary>
    /// Enum�ration pour l'�tat local de l'action pendant les mises � jour
    /// </summary>
    internal enum WorkingState {

      /// <summary>
      /// L'action est � l'�coute des �v�nements de ses clients
      /// </summary>
      Listening,

      /// <summary>
      /// L'action est en train de piloter ses clients
      /// </summary>
      Driving
    }

    // cache acc�l�rateur unique pour toutes les actions donnant acc�s aux propri�t�s r�flexives des cibles 
    private   static TargetTypeCache         types                         = new TargetTypeCache(0);

    // valeurs par d�faut
    private   static readonly Color          defaultImageTransparentColor  = Color.Magenta ;

    // gestion g�n�rale des l'action
    private   ActionList                     owner                         = null;
    private   TargetList                     targets                       = null;
    private   PropertyDescriptorCollection   updatableProperties           = null;
    private   IComponentChangeService        changeService                 = null;

    // gestion interne de l'�tat de l'action
    internal  WorkingState                   workingState                  = WorkingState.Listening;
    private   ActionState                    state                         = ActionState.Building;

    // champs associ�s aux propri�t�s publiques
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

    // d�l�gu�s uniques pour abonnement aux �v�nements des cibles          
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
    /// <param name="container">conteneur dans lequel ins�rer l'action</param>
    public Action( System.ComponentModel.IContainer container ) : this() {
      //Psl.Tracker.Tracker.Track( "Action.cctor(container)" );
      container.Add( this );
    }

    /// <summary>
    /// Lib�ration des ressources, c'est-�-dire d�-liaison de tous les liens
    /// </summary>
    /// <param name="disposing">true si les donn�es non manag�es doivent �tre lib�r�es</param>
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
    /// Red�finition de la propri�t� Site pour acc�der au service IComponentChangeService
    /// </summary>
    public override ISite Site {
      get { return base.Site; }
      set {
        base.Site = value;
        changeService = GetService( typeof( IComponentChangeService ) ) as IComponentChangeService;
      }
    }

    /// <summary>
    /// Obtient ou d�termine la liste d'actions propri�taire d'une action
    /// </summary>
    /// <remarks>
    /// Sachant que ce sont les listes d'actions qui sont tenues au courant de la phase 
    /// de construction, l'�tat par d�faut lors de l'initialisation d'une action est Building
    /// jusqu'� ce que la phase de construction soit termin�e. Cependant, lors de la cr�ation
    /// d'une action en mode conception, l'�v�nement de fin de phase de construction n'arrivera
    /// �vicemment jamais (appel de DoBuildComplete), de sorte qu'il faut lib�rer
    /// au plus t�t l'action de son �tat Building. Le premier moment o� cela est possible,
    /// c'est lorsque l'action est ins�r�e dans la liste de sa liste d'action propri�taire, 
    /// ce qui se traduit par l'invocation du setter de la propri�t� Owner. C'est l� qu'il faut
    /// ajuster l'�tat de l'action si la liste de d'actions propri�taire est sortie de la phase
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
    /// Retourne true si l'�tat interne de l'action est "Building"
    /// </summary>
    /// <remarks>
    /// L'�tat "Building" s'�tend depuis la cr�ation de l'action jusqu'� l'appel � DoBuildComplete
    /// qui correspond � l'invocation EndInit sur la liste d'actions. 
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
    /// Notifie aupr�s du designer associ� � l'action que le composant va �tre modifi�.
    /// </summary>
    /// <remarks>
    /// Permet de faire savoir au designer associ� � l'action que le composant Action a �t�
    /// modifi� en dehors des actions explicites de l'utilisateur sur l'ihm de conception.
    /// <br/>
    /// Les notifications de changement doivent �tre inhib�es durant la phase de construction 
    /// de l'action de mani�re � �viter qu'un concepteur soit marqu� modifi� d�s son chargement.
    /// </remarks>
    /// <param name="target">composant cible concern� par la modification</param>
    private void DoNotifyChanging( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanging (((, action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      if ( changeService == null || IsBuilding ) return;
      changeService.OnComponentChanging( this  , null );
      changeService.OnComponentChanging( target, null );
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanging ))), action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    /// <summary>
    /// Notifie aupr�s du designer associ� � l'action que le composant a �t� modifi�.
    /// </summary>
    /// <remarks>
    /// Voir les remarques associ�es � la m�thode <see cref="DoNotifyChanging"/>
    /// <br/>
    /// Les notifications de changement doivent �tre inhib�es durant la phase de construction 
    /// de l'action de mani�re � �viter qu'un concepteur soit marqu� modifi� d�s son chargement.
    /// </remarks>
    /// <param name="target">composant cible concern� par la modification</param>
    private void DoNotifyChanged( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanged (((, action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      if ( changeService == null || IsBuilding ) return;
      changeService.OnComponentChanged( target, null, null, null );
      changeService.OnComponentChanged( this  , null, null, null );
      //Psl.Tracker.Tracker.Track( "Action.DoNotifyChanged ))), action=" + this.ToString() + ", state=" + state + ", changeService=" + (changeService == null ? "null" : changeService.ToString()) + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    //
    // Contr�le de coh�rence
    //

                                                                               // <wao DoCheckOwner.begin>
    /// <summary>
    /// Contr�le que l'action est actuellement li�e � une liste d'actions
    /// </summary>
    protected void DoCheckOwner() {
      if (owner == null) 
        throw new EActionList( "L'action n'est pas li�e � une liste d'actions" ) ;
    }
                                                                               // <wao DoCheckOwner.end>

    #region Gestion des collections
    //
    // Gestion de la collection des cibles
    //

    /// <summary>
    /// Mise � jour des cibles lors de la fin de la construction d'une action
    /// </summary>
    /// <remarks>
    /// Pendant la phase de construction, les cibles sont simplement enregistr�es dans la liste des
    /// cibles sans �tre mises � jour. Lorsque l'action termine sa construction, cette m�thode
    /// effectue le backtracking sur les cibles enregistr�es de mani�re � les attacher � l'action.
    /// </remarks>
    internal void DoBuildComplete() {

      // attacher toutes les cibles en attente
      foreach (Component target in targets) {
        DoAttachTarget( target );
      }

      // d�verrouiller la notification des changements et l'attachement des cibles dans la liste des cibles
      state = ActionState.Standard;
    }

    /// <summary>
    /// Attachement d'une cible � une action
    /// </summary>
    /// <remarks>
    /// M�thode de service appel�e par DoBuildComplete et TargetList.Add
    /// pour attacher une cible � l'action. Cette m�thode redirige le traitement sur la m�thode
    /// <see cref="ActionProvider.SetAction"/> qui est l'unique m�thode pilotant la gestion des cibles.
    /// C'est cette m�thode qui effectuera les appels appropri�s � <see cref="DoAddTarget"/> et 
    /// <see cref="DoRemoveTarget"/>.
    /// </remarks>
    /// <param name="target">cible � attacher</param>
    internal void DoAttachTarget( Component target ) {
      //Psl.Tracker.Tracker.Track( "Action.DoAttachTarget +, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
      ActionProvider.TryGetActionProvider( owner.Site ).SetAction( target, this );
      //Psl.Tracker.Tracker.Track( "Action.DoAttachTarget -, count=" + targets.Count + ", action=" + this.ToString() + ", target=" + target.GetHashCode() + "-" + target.GetType().Name );
    }

    /// <summary>
    /// D�tachement de toutes les cibles d'une action
    /// </summary>
    /// <remarks>
    /// M�thode de service appel�e par TargetList.Clear pour d�tacher toutes les cibles
    /// de l'action. Cette m�thode redirige le traitement sur la m�thode <see cref="ActionProvider.SetAction"/> 
    /// qui est l'unique m�thode pilotant la gestion des cibles. C'est cette m�thode qui effectuera les appels 
    /// appropri�s � <see cref="DoRemoveTarget"/>.
    /// </remarks>
    internal void DoDetachTargets() {
      while ( targets.Count > 0 )
        ActionProvider.TryGetActionProvider( owner.Site ).SetAction( targets[ targets.Count - 1 ], null );
    }

    /// <summary>
    /// Adjonction d'une cible aux cibles de l'action
    /// </summary>
    /// <remarks>
    /// M�thode appel�e depuis <see cref="ActionProvider.SetAction"/> pour effectuer la mise � jour
    /// de la cible conform�ment aux donn�es de l'action.
    /// <br/>
    /// Cette m�thode suppose que la cible a d�j� �t� d�li�e de toute action.
    /// </remarks>
    /// <param name="target">cible � ajouter</param>
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
    /// M�thode appel�e depuis <see cref="ActionProvider.SetAction"/> pour d�lier une cible de l'action
    /// </summary>
    /// <remarks>
    /// Cette m�thode n'effectue aucun traitement si la cible ne figurait pas dans la liste des cibles
    /// de l'action.
    /// </remarks>
    /// <param name="target">cible � d�lier</param>
    /// <returns>true si la cible a effectivement �t� d�li�e</returns>
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
    /// M�thode appel�e lorsqu'une cible a �t� d�tach�e de l'action (pour extensions)
    /// </summary>
    /// <param name="target">cible d�tach�e</param>
    protected virtual void OnTargetRemoved( Component target ) { }

    /// <summary>
    /// M�thode appel�e lorsqu'une cible a �t� attach�e � l'action (pour extensions)
    /// </summary>
    /// <param name="target">cible attach�e</param>
    protected virtual void OnTargetAdded( Component target ) { }

    #endregion

    #region Mise � jour des contr�les cibles
    //
    // Mise � jour des contr�les cible
    //

    /// <summary>
    /// D�termine si une cible est effectivement valid�e.
    /// </summary>
    /// <remarks>
    /// Cette m�thode de service tient compte de la propri�t� Enabled de la liste d'actions
    /// propri�taire de l'action, laquelle est prioritaire sur la propri�t� Enabled de l'action.
    /// </remarks>
    /// <returns>true si la cible est effectivement valid�e</returns>
    protected virtual bool DoGetTargetEnabled() {
        return enabled && (owner == null ? true : owner.Enabled);
    }

    /// <summary>
    /// D�termine si une cible est effectivement visible ou non.
    /// </summary>
    /// <remarks>
    /// Cette m�thode de service tient compte de la propri�t� Visible de la liste d'actions
    /// propri�taire de l'action, laquelle est prioritaire sur la propri�t� Visible de l'action.
    /// En outre, en mode conception, les cibles sont maintenues toujours visibles pour ne pas 
    /// emp�cher l'utilisation des services de l'ide en mode conception. 
    /// </remarks>
    /// <returns>true si la cible est effectivement visible</returns>
    protected virtual bool DoGetTargetVisible() {
        return DesignMode ? true : (visible && (owner == null ? true : owner.Visible));
    }

    /// <summary>
    /// Mise � jour d'une propri�t� de l'action aupr�s d'une cible
    /// </summary>
    /// <remarks>
    /// Cette m�thode traite le cas particulier des conseils (propri�t� ToolTipText) 
    /// selon la nature de la cible. Pour les propri�t�s autres que ToolTipText, la mise � jour
    /// s'effectue par r�flexivit� (via le cache types.SetValue) en style bypass.
    /// <br/>
    /// Dans le cas de la propri�t� ToolTipText, deux cas peuvent se pr�senter, selon que 
    /// la cible fait ou non partie des composants qui peuvent �tre �tendus par un ToolTip provider 
    /// (test via CanExtend) :
    /// <br/>
    /// 1) si oui, la mise � jour de la propri�t� s'effectue via le ToolTip provider local de
    /// la liste d'actions (la propri�t� n'est pas affich�e en mode conception au niveau de la cible) ;
    /// <br/>
    /// 2) si non, il est pr�sum� qu'il s'agit d'un composant au moins Net 2.0 qui comporte une
    /// propri�t� normale "ToolTipText", auquel cas la mise � jour s'effectue normalement 
    /// par r�flexivit� via types.SetValue ; si le composant ne comporte pas une telle propri�t�,
    /// la mise � jour est simplement ignor�e (style bypass)
    /// </remarks>
    /// <param name="target">cible � mettre � jour</param>
    /// <param name="propertyName">nom de la propri�t� � mettre � jour</param>
    /// <param name="value">nouvelle valeur de la propri�t�</param>
    private void DoRefreshProperty( Component target, string propertyName, object value ) {
      if ( target == null ) return;

      workingState = WorkingState.Driving;
      try {

        // d�terminer si la cible requiert ou non l'extension d'un provider ToolTip
        bool needsProvider = (owner != null)
          && (propertyName == "ToolTipText")
          && owner.ToolTipProvider.CanExtend( target );

        // mise � jour selon le cas
        if ( needsProvider )
          owner.ToolTipProvider.SetToolTip( target as Control, (string) value );
        else
          types.SetValue( propertyName, target, value );

      } finally { workingState = WorkingState.Listening; }
    }

    /// <summary>
    /// Mise � jour d'une propri�t� de l'action aupr�s de toutes les cibles de l'action
    /// </summary>
    /// <param name="propertyName">nom de la propri�t� � mettre � jour</param>
    /// <param name="value">nouvelle valeur de la propri�t�</param>
    protected void DoRefreshPropertyInAll( string propertyName, object value ) {
      foreach ( Component target in targets )
        DoRefreshProperty( target, propertyName, value );
    }

    /// <summary>
    /// Mise � jour de toutes les propri�t�s prioritaires issues la liste d'action propri�taire
    /// aupr�s de toutes les cibles de l'action.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est d�clench�e depuis la liste d'action propri�taire d�s lors que l'une
    /// des propri�t�s prioritaires li�es � la liste d'action est modifi�e. 
    /// </remarks>
    internal void DoRefreshOverridesInAll() {
      DoRefreshPropertyInAll( "Enabled", DoGetTargetEnabled() );
      DoRefreshPropertyInAll( "Visible", DoGetTargetVisible() );
    }

    /// <summary>
    /// Obtient le nom de l'�v�nement consid�r� comme click pour un type de cible
    /// </summary>
    /// <param name="defaultName">nom par d�faut de l'�v�nement clic</param>
    /// <param name="target">cible vis�e</param>
    /// <remarks>
    /// Ce calcul est li� au fait que certains composants exposent un �v�nement dont la
    /// s�mantique est un clic convenant � une action mais qui ne sont pas expos�s
    /// sous le nom usuel "Click". Qui plus est, certains de ces composants exposent
    /// aussi un �v�nement nomm� "Click" mais qui ne convient pas � la s�mantique des actions.
    /// D'o� ce filtrage qui doit se faire au niveau du type des cibles. 
    /// <br/>
    /// Ce filtrage doit cependant rediriger sur un �v�nement de signature standard "object, EventArgs".
    /// Pour les �v�nements qui ne sont pas de signature standard, il faut op�rer en jouant
    /// sur le type de d�l�gu� dans la m�thode AddHandler du cache des types.
    /// </remarks>
    /// <returns>le nom de l'�v�nement clic pour la cible</returns>
    protected virtual string DoGetClickEventName( string defaultName, object target ) {
      string result = defaultName;

      // expose aussi un �v�nement Click, mais concerne le bouton dans son ensemble (bouton ou fl�che)
      if ( target is ToolStripSplitButton )
        result = "ButtonClick";

      // retourne le nom redirig�
      return result;
    }

    /// <summary>
    /// Abonne, aupr�s d'une cible, les handlers requis pour que l'action puisse g�rer la cible
    /// </summary>
    /// <param name="target">composant cible concern�</param>
    protected virtual void DoAddHandlers( Component target ) {
      string clickEventName = DoGetClickEventName( "Click", target );

      types.AddHandler( clickEventName     , target, dgTargetStandardClick      );
      types.AddHandler( "CheckStateChanged", target, dgTargetCheckStateChanged  );
      types.AddHandler( "ButtonClick"      , target, dgTargetToolBarButtonClick );
    }

    /// <summary>
    /// D�sabonne, aupr�s d'une cible, les handlers requis pour que l'action puisse g�rer la cible
    /// </summary>
    /// <param name="target">composant cible concern�</param>
    protected virtual void DoRemoveHandlers( Component target ) {
      string clickEventName = DoGetClickEventName( "Click", target );

      types.RemHandler( clickEventName     , target, dgTargetStandardClick      );
      types.RemHandler( "CheckStateChanged", target, dgTargetCheckStateChanged  );
      types.RemHandler( "ButtonClick"      , target, dgTargetToolBarButtonClick );
    }

    /// <summary>
    /// Mise � jour de toutes les propri�t�s "updatables" de l'action aupr�s d'une cible donn�e.
    /// </summary>
    /// <param name="target">cible concern�e</param>
    private void DoRefreshProperties( Component target ) {
      foreach (PropertyDescriptor property in updatableProperties) {
        DoRefreshProperty(target, property.Name, property.GetValue(this));
      }
    }

    #endregion

    #region Gestion des �v�nements des cibles
    //
    // Gestion des �v�nements des cibles
    //

    /// <summary>
    /// M�thode de service pour le traitement associ� � un �v�nement Click d'une cible
    /// </summary>
    /// <param name="sender">source de l'�v�nement Click</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void DoHandleTargetClick( object sender, EventArgs e ) {
      if (workingState != WorkingState.Listening) return;
      DoActionExecute();
    }

    /// <summary>
    /// Gestionnaire appel� par une cible pour un �v�nement Click dans le cas g�n�ral des Control
    /// </summary>
    /// <param name="sender">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void OnTargetStandardClick(object sender, EventArgs e) {
      DoHandleTargetClick(sender, e);
    }

    /// <summary>
    /// Gestionaire appel� par une cible pour un �v�nement ButtonClick dans le cas particulier des Toolbar
    /// </summary>
    /// <param name="sender">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void OnTargetToolBarButtonClick( object sender, ToolBarButtonClickEventArgs e ) {
      if (! targets.Contains( e.Button )) return;
      DoHandleTargetClick( e.Button, e );
    }

    /// <summary>
    /// Gestionnaire appel� par une cible pour un �v�nement CheckStateChanged
    /// </summary>
    /// <param name="sender">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
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
    /// Ajoute un objet Exception � la liste des exceptions du rapport de l'action.
    /// </summary>
    /// <param name="report">exception synth�tique � relancer</param>
    /// <param name="exception">objet exception � ajouter</param>
    /// <param name="step">identification de l'�tape o� l'exception s'est produite</param>
    private void DoWrapException( ref EActionList report, Exception exception, string step) {

      // compl�ter les information relatives � l'action
      exception.Data.Add( 
        "Remarque", 
        "Exception intercept�e pendant une action de type " + GetType().FullName );
      exception.Data.Add( 
        "Etape", 
        step );

      // cr�er l'exception enveloppe si n�cessaire
      if ( report == null ) 
        report = new EActionList( "Exception(s) d�clench�e(s) pendant une action" );

      /*                                                                       // <wao never>
      // ajouter l'exception � la liste des exceptions intercept�es
      report.Data.Add( 
        "Embedded", 
        exception );
      */                                                                       // <wao never>
      report.ContainedExceptions.Add( exception );                             // <wao never>
    }
                                                                               // <wao DoWrapException.end>
                                                                               // <wao DoActionExecute.begin>
    /// <summary>
    /// M�thode unique de d�clenchement du protocole d'action.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 1. Tout d�clenchement du corps d'une action (via un bouton, un �l�ment de menu,
    /// l'appel � Perform, etc.) passe par cette m�thode : DoActionExecute est donc le point d'entr�e
    /// du protocole des actions. 
    /// </para>
    /// <para>
    /// 2. Le corps de l'action (�v�nement Execute) sera d�clench� depuis la m�thode
    /// OnExecute qui sera appel�e depuis la m�thode DoActionExecute de ActionList.
    /// </para>
    /// <para>
    /// o Le protocole d�clenche, dans l'ordre :<br/>
    ///   - ActionGlobalBefore (diffus� � toutes les listes d'action de l'application)<br/>
    ///   - ActionExecuting    (au niveau de la liste � laquelle appartient l'action)<br/>
    ///   - Execute            (corps de l'action � ex�cuter)<br/>
    ///   - ActionExecuted     (au niveau de la liste � laquelle appartient l'action)<br/>
    ///   - ActionGlobalAfter  (diffus� � toutes les listes d'action de l'application)<br/>
    /// <br/>
    /// o Les diff�rentes �tapes sont �tanches les unes par rapport aux du point de vue
    /// de la gestion des exceptions
    /// <br/>
    /// o L'exception ECancelled est intercept�e comme exception silencieuse au niveau
    ///   du corps de l'action seulement
    ///   <br/>
    /// o Si au moins une exception a �t� trait�e, une unique exception EActionList est 
    ///   d�clench�e en fin de protocole, contenant dans sa collection Data la liste de tous
    ///   les objets exception intercept�s (avec leur stanck trace intacte) 
    /// </para>
    /// <para>
    /// (note du 19 04 2008) Le recours � la suveillance de l''�v�nement CheckStateChanged des
    /// cibles a pour cons�quence que les composants qui ne comportent pas un tel �v�nement 
    /// (par exemple Button) ne peuvent pas basculer la propri�t� CheckState par un simple clic.
    /// <br/>
    /// Il faudra am�liorer cet aspect, probablement en ins�rant un test appropri� dans le handler
    /// des clics des cibles (basculer CheckState si la cible n'expose pas d'�v�nement CheckStateChanged).
    /// </para>
    /// </remarks>
    internal protected virtual void DoActionExecute() {
      EActionList report = null;
      ECancelled cancelled = null;

      // v�rifier que l'action est lli�e � une liste d'action
      DoCheckOwner();

      // si l'action n'est pas actuellement valid�e, ne rien faire
      if ( !Enabled ) return;

      // diffusion de l'�v�nement ActionGlobalBefore
      try { ActionList.FireActionGlobalBeforeEvent( this, EventArgs.Empty ); }
      catch ( Exception x ) { DoWrapException( ref report, x, "�v�nement ActionGlobalBefore" ); }

      // diffusion de l'�v�nement ActionExecuting
      try { owner.OnActionExecuting( this, EventArgs.Empty ); } 
      catch ( Exception x ) { DoWrapException( ref report, x, "�v�nement ActionExecuting" ); }

      // diffusion de l'�v�nement Execute
      try { OnExecute(); }
      catch ( ECancelled x ) { cancelled = x; }
      catch ( Exception x ) { DoWrapException( ref report, x, "�v�nement Execute" ); }

      // Attention !                                                                      // <wao never.begin> 
      // L'ex�cution de certaines actions (en particulier la fermeture de la fen�tre h�bergeant l'action)
      // peut provoquer la restitution des ressources et d�lier l'action de sa liste d'action.
      // D'o� le test renouvel� de owner pour �viter les exceptions non souhaitables      // <wao never.end>
      // diffusion de l'�v�nement ActionExecuted (sauf si restitution de ressources)
      if ( owner != null ) {
        try { owner.OnActionExecuted( this, EventArgs.Empty ); }
        catch ( Exception x ) { DoWrapException( ref report, x, "�v�nement ActionExecuted" ); }
      }

      // diffusion de l'�v�nement ActionGlobalAfter
      try { ActionList.FireActionGlobalAfterEvent( this, EventArgs.Empty ); }
      catch ( Exception x ) { DoWrapException( ref report, x, "�v�nement ActionGlobalAfter" ); }

      // bilan des exceptions
      if ( report != null ) throw report;
      if ( cancelled != null ) throw cancelled;
    }
                                                                               // <wao DoActionExecute.end>
    
    #endregion

    #region D�clenchement centralis� des �v�nements
                                                                               // <wao OnExecute.begin>
    /// <summary>
    /// D�clenchement centralis� de l'�v�nement Execute de l'action
    /// </summary>
    protected virtual void OnExecute() {
      if (Execute != null) Execute( this, EventArgs.Empty );
    }
                                                                               // <wao OnExecute.end>

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement Update de l'action
    /// </summary>
    protected virtual void OnUpdate() {
      if (Update != null) Update( this, EventArgs.Empty );
    }

    #endregion

    #region Fonctionnalit�s

                                                                               // <wao Perform.begin>
    /// <summary>                                                               
    /// D�clenchement du protocole d'action par programme.
    /// </summary>
    /// <remarks>
    /// Cette m�thode publique permet de d�clencher une action par programme en
    /// appliquant le protocole d'actions exactement comme si l'action 
    /// avait �t� d�clench�e par l'ihm (�l�ment de menu, bouton, etc.)
    /// </remarks>
    public void Perform() {
      DoActionExecute();
    }                                                                          // <wao Perform.end>

    /// <summary>
    /// D�clenche la mise � jour de l'action.
    /// </summary>
    public void DoUpdate() {
      OnUpdate();
    }

    #endregion

    #region Propri�t�s publiques
    //
    // Propri�t�s publiques
    //

    /// <summary>
    /// Acc�s � la collection des cibles, invisible en mode conception
    /// </summary>
    /// <remarks>
    /// Cette propri�t� est expos�e pour permettre la s�rialisation de la liste des cibles
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
    /// Obtient ou d�termine le texte utilis� comme libell� dans les contr�les associ�s � l'action
    /// </summary>
    [ 
      DefaultValue( "" ), 
      UpdatableProperty(), 
      Localizable( true ),
      Description( "Obtient ou d�termine le texte utilis� comme libell� dans les contr�les associ�s � l'action" )
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
    /// Obtient ou d�termine si un clic sur un contr�le associ� � l'action bascule la propri�t� Checked
    /// </summary>
    [
      DefaultValue( false ),
      UpdatableProperty,
      Description( "Obtient ou d�termine si un clic sur un contr�le associ� � l'action bascule la propri�t� Checked" )
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
    /// Obtient ou d�termine l'�tat Checked de l'action et de ses cibles
    /// </summary>
    /// <remarks>
    /// Il s'agit d'une propri�t� d�riv�e de la propri�t� CheckState. Elle n'est donc pas s�rialis�e, 
    /// et ne provoque pas directement de mise � jour aupr�s des cibles (voir la propri�t� CkeckState).
    /// </remarks>
    [
      DefaultValue( false ),
      DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
      Description( "Obtient ou d�termine si les contr�les associ�s � l'actions sont coch�s ou non" )
    ]
    public bool Checked {
      get { return (checkState != CheckState.Unchecked); }
      set {
        if (value == Checked) return;
        CheckState = value ? CheckState.Checked : CheckState.Unchecked;
      }
    }

    /// <summary>
    /// Obtient ou d�termine l'�tat CheckState d'une action et de ses cibles
    /// </summary>
    [
      DefaultValue( CheckState.Unchecked ), 
      UpdatableProperty(),
      Description( "Obtient ou d�termine l'�tat de la propri�t� CheckedState des contr�les associ�s � l'action" )
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
    /// Obtient ou d�termine l'�tat de validation de l'action et de ses cibles.
    /// </summary>
    /// <remarks>
    /// Cette propri�t� refl�te l'�tat de validation local (pour l'action).
    /// Sachant que la propri�t� Enabled de la liste d'action propri�taire est prioritaire,
    /// les cibles ne seront arm�es que si les deux propri�t�s Enabled sont true.
    /// </remarks>
    [
      DefaultValue( true ), 
      UpdatableProperty,
      Description( "Obtient ou d�termine si les contr�les associ�s � l'action sont arm�s ou inhib�s" )
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
    /// Obtient ou d�termine l'�tat de visibilit� de l'action et de ses cibles
    /// </summary>
    /// <remarks>
    /// Cette propri�t� refl�te l'�tat de visibilit� local (pour l'action).
    /// Sachant que la propri�t� Visible de la liste d'action propri�taire est prioritaire,
    /// les cibles ne seront visibles que si les deux propri�t�s Visible sont true.
    /// Toutefois, en mode conception, les cibles sont maintenues toujours visibles pour
    /// ne pas perturber les possibilit�s de l'ide.
    /// </remarks>
    [
      DefaultValue( true ), 
      UpdatableProperty,
      Description( "Obtient ou d�termine si les contr�les associ�s � l'action sont visibles" )
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
    /// Obtient ou d�termine l'image associ�e � l'action et � ses cibles
    /// </summary>
    [
      DefaultValue( null ), 
      UpdatableProperty,
      Description( "Obtient ou d�termine l'image utilis�e par les contr�les associ�s � l'action" )
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
    /// D�termine si la propri�t� ImageTransparentColor doit �tre s�rialis�e 
    /// </summary>
    /// <returns>true si la propri�t� ImageTransparentColor doit �tre s�rialis�e</returns>
    protected bool ShouldSerializeImageTransparentColor() {
      //Psl.Tracker.Tracker.Track( "Action.ShouldSerializeImageTransparentColor" );
      return ImageTransparentColor != defaultImageTransparentColor;
    }

    /// <summary>
    /// Obtient ou d�termine la couleur transparente pour l'image de l'action
    /// </summary>
    [
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou d�termine la couleur transparente pour l'image de l'action" ) 
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
    /// Obtient ou d�termine la touche de raccourci qui d�clenche l'action
    /// </summary>
    [
      DefaultValue( Keys.None ), 
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou d�termine la touche de raccourci clavier qui d�clenche l'action" )
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
    /// Obtient ou d�termine la cha�ne � afficher en tant que raccourci clavier
    /// </summary>
    [
      DefaultValue( "" ),
      UpdatableProperty,
      Localizable( true ),
      Description( "Obtient ou d�termine la cha�ne � afficher en tant que raccourci clavier" )
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
    /// Obtient ou d�termine si les raccourcis clavier doivent �tre affich�s dans les menus
    /// </summary>
    [
      DefaultValue( true ),
      UpdatableProperty,
      Localizable( true ),
      Description( "Obtient ou d�termine si les raccourcis clavier doivent �tre affich�s dans les menus" )
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
    /// Obtient ou d�termine le texte utilis� dans les bulles d'aide des contr�les associ�s � l'action 
    /// </summary>
    [
      DefaultValue( "" ), 
      UpdatableProperty, 
      Localizable( true ),
      Description( "Obtient ou d�termine le texte utilis� dans les bulles d'aide des contr�les associ�s � l'action" ) 
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
    /// Evenement d�clench� lorsque l'action est appliqu�e
    /// </summary>
    [
      Description( "Evenement d�clench� lorsque l'action est appliqu�e" ) 
    ]
    public event EventHandler Execute;

    /// <summary>
    /// Ev�nement d�clench� (sur Idle) pour permettre la mise � jour des propri�t�s de l'action
    /// </summary>
    [
      Description( "Ev�nement d�clench� (sur Idle) pour permettre la mise � jour des propri�t�s de l'action" )
    ]
    public event EventHandler Update;

    #endregion

  }
}
