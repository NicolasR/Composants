/*                                                                             // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Crédits :
 * Les listes d'action, inspirées du composant TActionList de Delphi, sont issues d'une 
 * refonte et d'une adaptation de deux composants freeware open source :
 * - les listes d'action pour net 1.1 de Serge Weinstock
 * - les listes d'action pour net 2.0 de Marco de Sanctis
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 26 11 2008 : amélioration de la classe ActionProvider et suppression du ActionProviderDesigner
 * 22 05 2009 : adjonction de la prise en charge des boutons ToolStripSplitButton
 */
using System;                                                                  // <wao never.end>
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Psl.Actions {

  #region Classe des exceptions du provider
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                         Classe des exceptions du provider
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe d'exception liée aux exception de la classe ActionProvider
  /// </summary>
  public class EActionProvider : Exception {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associé à l'objet exception</param>
    public EActionProvider( string message ) : base( message ) { }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="message">message associé à l'objet exception</param>
    /// <param name="inner">object exception cause de l'exception</param>
    public EActionProvider( string message, Exception inner ) : base( message, inner ) { }
  }

  #endregion

  #region Dictionnaire d'associations
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                                Dictionnaire d'association cibles/actions
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Dictionnaire d'association du provider de la propriété Action entre cibles et actions
  /// </summary>
  /// <remarks>
  /// <para>
  /// Ce dictionnaire est simplement une extension générique de List destinée à permettre
  /// un double accès par recherche selon la clé (cible) et par indexation simple (ce que
  /// ne permet pas directement la classe Dictionary), cela afin d'optimiser le nombre
  /// des recherches, surtout dans la gestion de la dissociation des liens.
  /// </para>
  /// Ce wrapper n'implémente que ce qui est strictement nécessaire à l'implémentation
  /// du provider des actions.
  /// <para>
  /// </para>
  /// </remarks>
  /// <typeparam name="TKey">type des clés, en principe Component</typeparam>
  /// <typeparam name="TValue">type des valeurs, en principe Action</typeparam>
  internal class Pairs<TKey, TValue> {

    // liste interne d'implémentation
    private List<KeyValuePair<TKey, TValue>> items = new List<KeyValuePair<TKey, TValue>>();

    /// <summary>
    /// Retourne le nombre d'éléments dans la liste
    /// </summary>
    public int Count {
      get { return items.Count; }
    }

    /// <summary>
    /// Recherche d'une entrée du dictionnaire à partir de la clé
    /// </summary>
    /// <remarks>
    /// La clé de recherche étant (en principe) une référence, il n'est pas possible de trier 
    /// ces clés pour accéléer les recherches. C'est donc une recherche en O(n).
    /// </remarks>
    /// <param name="key">clé à rechercher (comparaison selon la méthode Equals)</param>
    /// <returns>l'index de l'entrée trouvée, ou -1</returns>
    public int IndexOfKey( TKey key ) {
      for (int ix = 0 ; ix < items.Count ; ix++)
        if (items[ ix ].Key.Equals( key )) return ix;
      return -1;
    }

    /// <summary>
    /// Tente d'obtenir une valeur à partir d'une clé.
    /// </summary>
    /// <param name="key">clé de recherche</param>
    /// <param name="value">la valeur si trouvée, null sinon</param>
    /// <returns>true si la clé à été trouvée, false sinon</returns>
    public bool TryGetValue( TKey key, out TValue value ) {
      value = default( TValue );
      int index = IndexOfKey( key );
      if (index == -1) return false;
      value = this[ index ].Value;
      return true;
    }

    /// <summary>
    /// Adjonction d'une entrée d'association dans le dictionnaire
    /// </summary>
    /// <remarks>
    /// A des fins d'optimisation, cette méthode n'effectue aucune vérification 
    /// quant au fait qu'une entrée ayant la même clé figure déjà dans le dictionnaire. 
    /// </remarks>
    /// <param name="key">clé de la nouvelle entrée</param>
    /// <param name="value">valeur de la nouvelle entrée</param>
    public void Add( TKey key, TValue value ) {
      items.Add( new KeyValuePair<TKey, TValue>( key, value ) );
    }

    /// <summary>
    /// Suppression d'une entrée du dictionnaire à une position donnée.
    /// </summary>
    /// <param name="index"></param>
    public void RemoveAt( int index ) {
      items.RemoveAt( index );
    }

    /// <summary>
    /// Indexeur retournant une entrée du dictionnaire selon son index
    /// </summary>
    /// <param name="index">index de l'entrée</param>
    /// <returns>l'entrée se trouvant à cette position</returns>
    public KeyValuePair<TKey, TValue> this[ int index ] {
      get { return items[ index ]; }
    }

    /// <summary>
    /// Modification de la valeur associée à une clé.
    /// </summary>
    /// <param name="index">indes de l'entrée du dictionnaire à modifier</param>
    /// <param name="value">nouvelle valeur</param>
    public void SetValue( int index, TValue value ) {
      items[ index ] = new KeyValuePair<TKey, TValue>( this[ index ].Key, value );
    }
  }

  #endregion

  #region Fournisseur de la propriété Action
  // ////////////////////////////////////////////////////////////////////////////////////
  //
  //                         Fournisseur de la propriété Action
  //
  // ////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Fournisseur d'une propriété Action
  /// </summary>
  /// <remarks>
  /// <para>
  /// Ce fournisseur est destiné à être unique pour un site conteneur donné de listes d'actions.
  /// Cela permet d'utiliser plusieurs listes d'actions dans un même site conteneur tout en faisant
  /// en sorte que chaque contrôle comporte une unique propriété "Action".
  /// </para>
  /// <para>
  /// Ce fournisseur n'est pas un composant palette, et n'apparaît jamais comme un composant 
  /// en mode conception. Il tient à jour un décompte des listes d'actions "clientes" et se détruit 
  /// automatiquement lorsqu'il n'y a plus de listes d'actions dans le site conteneur. Il n'est
  /// instancié que s'il y a au moins une liste d'action sur un formulaire. 
  /// </para>
  /// <para>
  /// Dans son ensemble, la gestion de l'articulation entre provider, listes d'actions, actions
  /// et cibles est assez délicate, en particulier en mode conception, à cause des aléas relatifs
  /// à l'ordre dans lequel les différents composants sont initialisés. 
  /// </para>
  /// <para>
  /// En mode conception, le fournisseur est instancié par nécessité via TryGetActionProvider. 
  /// Le fournisseur est alors enregistré comme un service d'extendeur sans être inclus dans un 
  /// conteneur, ce qui permet d'éviter la présence d'un composant non visuel.
  /// </para>
  /// <para>
  /// En mode exécution, le fournisseur est instancié par nécessité et inclus dans la liste des
  /// composants du site racine lié à un formulaire. 
  /// </para>
  /// <para>
  /// L'ensemble du fournisseur est programmé avec soin quant à la gestion des ressources de manière
  /// à maintenir à jour l'ensemble des liens entre les différents éléments. 
  /// </para>
  /// </remarks>
  [
    ProvideProperty( "Action", typeof( Component ) ),
    ToolboxItem( false )
  ]
  public class ActionProvider : Component, IExtenderProvider {

    //
    // Champs
    //

    private int clientCount  = 0 ;
    private Pairs<Component, Action> targets = new Pairs<Component, Action>();

    private EventHandler dgClientDisposed ;
    private EventHandler dgTargetDisposed ;
    private EventHandler dgActionDisposed ;

    private ISite site = null ;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <remarks>
    /// Crée une fois pour toutes les délégués de surveillance
    /// </remarks>
    /// <param name="site">site d'hébergement du provider</param>
    protected internal ActionProvider( ISite site ) {
      //Psl.Tracker.Tracker.Track( "ActionProvider.cctor (empty)" );
      dgClientDisposed = new EventHandler( OnClientDisposed );
      dgTargetDisposed = new EventHandler( OnTargetDisposed );
      dgActionDisposed = new EventHandler( OnActionDisposed );
      this.site = site;
    }

    /// <summary>
    /// Suppression du provider
    /// </summary>
    private void DoRemoveProvider() {
      //Psl.Tracker.Tracker.Track( "ActionProvider.Removing" );

      // déliaison des actions gérées par le provider
      for ( int ix = targets.Count - 1 ; ix >= 0 ; ix-- )
        DoSetAction( ix, null, null );

      // dé-enregistrer le provider de son site (mode conception seulement)
      if ( site == null ) return;
      IExtenderProviderService service = (IExtenderProviderService) site.GetService( typeof( IExtenderProviderService ) );
      if ( service != null ) service.RemoveExtenderProvider( this );

      //Psl.Tracker.Tracker.Track( "ActionProvider.Removed" );
    }

    /// <summary>
    /// Restitutions des ressources
    /// </summary>
    /// <param name="disposing">true si les ressources non managées doivent être libérées</param>
    protected override void Dispose( bool disposing ) {
      //Psl.Tracker.Tracker.Track( "ActionProvider.Disposing" );
      if ( disposing ) DoRemoveProvider();
      base.Dispose( disposing );
      //Psl.Tracker.Tracker.Track( "ActionProvider.Disposed" );
    }

    /// <summary>
    /// Enregistrement d'une nouvelle liste d'actions cliente
    /// </summary>
    /// <param name="client">référence sur la liste cliente</param>
    protected void DoAddClient( Component client ) {
      clientCount ++ ;
      client.Disposed += dgClientDisposed ;
    }

    /// <summary>
    /// Gestionnaire abonné à l'événement Disposed des clients (listes d'actions) du fournisseur
    /// </summary>
    /// <remarks>
    /// Le fournisseur le connaît pas directement les listes d'actions si ce n'est sous l'angle
    /// de leur nombre. Cette surveillance de la finalisation des listes d'actions est uniquement
    /// motiviée par un décompte de références permettant de déterminer quand le fournisseur doit
    /// se détruire. 
    /// </remarks>
    /// <param name="sender">objet en cours de finalisation</param>
    /// <param name="e">informations complémentaires</param>
    private void OnClientDisposed( object sender, EventArgs e ) {
      ((Component) sender).Disposed -= dgClientDisposed;
      clientCount--;
      //Psl.Tracker.Tracker.Track( "OnClientDisposed : clients restants : " + clientCount );
      if (clientCount > 0) return;
      Dispose();
    }

    /// <summary>
    /// Gestionnaire abonné à l'événement Disposed des cibles
    /// </summary>
    /// <remarks>
    /// Lorsqu'une cible se détruit, le fournisseur supprime l'association liée à cette
    /// cible et réfléchit cette suppression à l'action concernée via une affectation 
    /// SetAction( cible, null ) pour que l'action supprime cette cible de la liste de ses
    /// cibles. Cela régularise et simplifie le protocole au niveau des actions qui n'ont 
    /// pas besoin de surveiller la destruction des cibles. 
    /// </remarks>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur d'événements</param>
    private void OnTargetDisposed( object sender, EventArgs e ) {
      DoSetAction( sender as Component, null );
    }

    /// <summary>
    /// Gestionnaire abonné à l'événement Disposed des actions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette surveillance de la finalisation des actions est destinée avant tout à éviter
    /// d'avoir à gérer trop finement le problème du chaîne arrière entre les actions et le
    /// fournisseur. Dans une version complètement stabilisée, cette surveillance pourra
    /// sans doute être suprimée.
    /// </para>
    /// <para>
    /// Lorsqu'une action se détruit, le fournisseur supprime toutes les assocations liées 
    /// à cette action. A chaque fois, il réféléchit cette suppression par une affectation
    /// SetAction( cible, nul ). 
    /// </para>
    /// </remarks>
    /// <param name="sender">référence sur l'action en cours de finalisation</param>
    /// <param name="e">informations complémentaires</param>
    private void OnActionDisposed( object sender, EventArgs e ) {
      Action action = sender as Action;

      for (int ix = targets.Count - 1 ; ix >= 0 ; ix--)
        if (targets[ ix ].Value == action)
          DoSetAction( ix, null, null );
    }

    //
    // Accès au fournisseur
    //

    /// <summary>
    /// Récupère la racine des sites pour le concepteur en cours
    /// </summary>
    /// <param name="site">site d'un composant dont on veur rechercher le site racine</param>
    /// <returns>le site racine si trouvé, le site transmis en argument sinon</returns>
    internal static ISite DoTryGetRootSite( ISite site ) {
      if (site == null) return null ;

      IDesignerHost host = site.GetService( typeof( IDesignerHost ) ) as IDesignerHost;
      if (host == null) return site ;

      IComponent root = host.RootComponent ;
      if (root == null) return site ;
      if (root.Site == null ) return site ;
      return root.Site ;
    }

    /// <summary>
    /// Recherche du fournisseur pour le site racine du site d'un composant donné
    /// </summary>
    /// <remarks>
    /// <para>
    /// En mode conception, le fournisseur est récupéré via le service IExtenderListService
    /// qui fournit (via GetExtenderProviders) la liste des extendeurs d'un site donné. Voir
    /// le designer associé (ActionProviderDesigner) pour l'enregistrement du fournisseur).
    /// </para>
    /// <para>
    /// En mode exécution, le fournisseur a été inclus dans la liste des composants du site racine,
    /// de sorte qu'il suffit de rechercher dans cette liste. 
    /// </para>
    /// </remarks>
    /// <param name="site">site du composant pour lequel on recherche le fournisseur</param>
    /// <returns>le fournisseur si trouvé, null sinon</returns>
    private static ActionProvider DoTryGetActionProvider( ISite site ) {

      if (site.DesignMode) {
        IExtenderListService service = site.GetService( typeof( IExtenderListService ) ) as IExtenderListService;
        if (service == null) throw new EActionProvider( "(ActionProvider) : Le service 'IExtenderListService' est introuvable" );
        IExtenderProvider[] providers = service.GetExtenderProviders();

        foreach (object provider in providers)
          if (provider is ActionProvider) return provider as ActionProvider;

        return null;
      }
      else {
        foreach (Component component in site.Container.Components)
          if (component is ActionProvider) return (ActionProvider) component;
        return null;
      }  
    }

    /// <summary>
    /// Recherche et instanciation éventuelle d'un fournisseur pour un site donné
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette méthode permet en fait à une liste d'action de signaler qu'elle existe et qu'un
    /// fournisseur doit maintenant être disponible si ce n'est déjà le cas. 
    /// </para>
    /// <para>
    /// En mode conception, instancie si nécessaire un fournisseur et l'enregistre au niveau du site
    /// racine concerné. En principe, le fournisseur n'est jamais instancié ni enregistré par cette
    /// voie, car le designer associé (ActionProviderDesigner) aura toujours été déjà démarré
    /// par vsn (à cause de l'attribut DesignerAttribute de la classe ActionList). 
    /// </para>
    /// <para>
    /// En mode exécution, le fournisseur est instancié et inclus dans la liste des composants
    /// du site racine pour pouvoir être ultérieurement récupéré. 
    /// </para>
    /// </remarks>
    /// <param name="actions">référence sur la liste d'actions qui recherche le fournisseur</param>
    /// <param name="site">site associé à la liste d'action</param>
    /// <returns>le fournisseur si trouvé, nul sinon</returns>
    public static ActionProvider TryGetActionProvider( ActionList actions, ISite site ) {
      if (actions == null) throw new EActionProvider( "(TryGetActionProvider) : la liste d'actions est null" );
      if (site    == null) throw new EActionProvider( "(TryGetActionProvider) : le ISite conteneur est null" );

      site = DoTryGetRootSite( site );
      ActionProvider provider = DoTryGetActionProvider( site ) ;

      if (provider == null) {
        if (site.DesignMode) {
          //Psl.Tracker.Tracker.Track( "TryGetActionProvider : adjonction du provider en mode design" );
          IExtenderProviderService service = (IExtenderProviderService) site.GetService( typeof( IExtenderProviderService ) );
          if (service == null) throw new EActionProvider( "(TryGetActionProvider) : le service des providers d'extensions est introuvable" );

          provider = new ActionProvider( site );
          service.AddExtenderProvider( provider );
        }
        else {
          provider = new ActionProvider( site );
          site.Container.Add( provider, "actionProvider" );
        }
      }

      provider.DoAddClient( actions ) ;
      return provider ;
    }

    /// <summary>
    /// Recherche d'un fournisseur pour un site donné.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette méthode sert en fait lors de la dé-sérialisation des actions, alors que les
    /// liens arrières (de l'action sur la liste d'actions) ne sont pas encore garantis.
    /// </para>
    /// </remarks>
    /// <param name="site">site pour le lequel on recherche un fournisseur</param>
    /// <returns>le fournisseur si trouvé, nul sinon</returns>
    public static ActionProvider TryGetActionProvider( ISite site ) {
      site = DoTryGetRootSite( site );
      return DoTryGetActionProvider( site );
    }

    //
    // Gestion du dictionnaire d'association cible/action
    //

    // <exclude
    /// <summary>
    /// Enumération pour les opérations de modification du dictionnaire d'associations
    /// </summary>
    protected enum TOp {

      /// <summary>
      /// Modifier l'action associé à une cible
      /// </summary>
      change,
 
      /// <summary>
      /// Supprimer une assocation cible/action
      /// </summary>
      remove, 
 
      /// <summary>
      /// Ajouter une nouvelle association cible/action
      /// </summary>
      add       
    }

    /// <summary>
    /// Méthode centralisée d'application des changements au dictionnaire des associations
    /// </summary>
    /// <remarks>
    /// Unique méthode de service apportant des modifications au dictionnaire des associations. 
    /// Assure la gestion des entrées du dictionnaires, les abonnements de surveillance des 
    /// finalisations, et la réflexion en écho aux actions
    /// </remarks>
    /// <param name="index">index de l'entrée à modifier ou -1 si nouvelle entrée</param>
    /// <param name="newTarget">référence sur la cible, ou null pour une suppression</param>
    /// <param name="newAction">référence sur l'action, ou null pour une suppression</param>
    private void DoSetAction( int index, Component newTarget, Action newAction ) {
      if (index == -1 && newAction == null) return;

      // déterminer l'opération à effectuer
      TOp op = index == -1 ? TOp.add : newAction == null ? TOp.remove : TOp.change ;

      // état courant de l'entrée (si modification ou suppression)
      Action oldAction = null;
      Component oldTarget = null;

      if (op != TOp.add) {
        oldTarget = targets[ index ].Key   ;
        oldAction = targets[ index ].Value;
      }

      switch (op) {

        // suppression d'une association
        case TOp.remove :
          oldTarget.Disposed -= dgTargetDisposed;
          oldAction.Disposed -= dgActionDisposed;
          oldAction.DoRemoveTarget( oldTarget );
          targets.RemoveAt( index ) ;
          break ;

        // modification d'une association (la cible demeure, l'action change)
        case TOp.change :
          oldAction.Disposed -= dgActionDisposed;
          oldAction.DoRemoveTarget( oldTarget );
          newAction.Disposed += dgActionDisposed;
          newAction.DoAddTarget( oldTarget ) ;
          targets.SetValue( index, newAction ) ;
          break;

        // adjonction d'une association
        case TOp.add :
          newTarget.Disposed += dgTargetDisposed;
          newAction.Disposed += dgActionDisposed;
          newAction.DoAddTarget( newTarget );
          targets.Add( newTarget, newAction ) ;
          break ;
      }
      //Psl.Tracker.Tracker.Track( "DoSetAction : index=" + index + ", op=" + op + ", source=" + (newTarget == null ? "null" : newTarget.GetHashCode() + "-" + newTarget.ToString() + " [" + newTarget.GetType().Name + "]") + ", action=" + (newAction == null ? "null" : newAction.ToString()) );
    }

    /// <summary>
    /// Apporter une modification au dictionnaire des associations
    /// </summary>
    /// <param name="newTarget">nouvelle cible ou null pour une adjonction</param>
    /// <param name="newAction">nouvelle action ou null pour une suppression</param>
    private void DoSetAction( Component newTarget, Action newAction ) {
      DoSetAction( targets.IndexOfKey( newTarget), newTarget, newAction ) ;
    }

    //
    // Implémentation de IExtenderProvider
    //

    /// <summary>
    /// Méthode getter pour la propriété Action implémentée comme provider 
    /// </summary>
    /// <remarks>
    /// La valeur de cette propriété est mémorisée par le provider sous la forme d'un dictionnaire
    /// d'associations cible/action.
    /// C'est cette méthode qui expose les attributs qui sont associés à la propriété Action de
    /// chaque cible.
    /// </remarks>
    /// <param name="target">cible requérant l'accès à "sa" propriété Action</param>
    /// <returns>la référence sur l'action associée à cette cible</returns>
    public Action GetAction( Component target ) {
      Action action;
      return targets.TryGetValue( target, out action ) ? action : null ;
    }

    /// <summary>
    /// Méthode setter pour la propriété Action implémentée comme provider par la liste d'action
    /// </summary>
    /// <param name="target">référence sur la cible dont "sa" propriété Action doit être affectée</param>
    /// <param name="action">null ou référence sur l'action à affecter à cette propriété</param>
    public void SetAction( Component target, Action action ) {
      if (target == null) throw new ArgumentNullException( "source" );
      DoSetAction( target, action ) ;
    }

    /// <summary>
    /// Détermine si un composant peut se voir ajouter une propriété Action
    /// </summary>
    /// <param name="target">référence sur le composant</param>
    /// <returns>true si le composant peut se voir ajouter une propriété Action false sinon</returns>
    public bool CanExtend( object target ) {
      Type targetType = target.GetType();

      foreach (Type t in GetSupportedTypes())
        if (t.IsAssignableFrom( targetType ))
          return true;

      return false;
    }

    /// <summary>
    /// Méthode interne déterminant les types des composants pouvant se voir ajouter une propriété Action
    /// </summary>
    /// <returns>tableau de Type des types pouvant supporter une propriété Action</returns>
    protected virtual Type[] GetSupportedTypes() {
      return new Type[] {
        typeof(ButtonBase), 
        typeof(ToolStripButton), 
        typeof(ToolStripSplitButton),
        typeof(ToolStripDropDownButton),
        typeof(ToolStripMenuItem), 
        typeof(ToolBarButton), 
        typeof(MenuItem)};
    }
  }

  #endregion
}
