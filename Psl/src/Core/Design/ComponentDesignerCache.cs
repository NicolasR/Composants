/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 06 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Psl.Design {

  /// <summary>
  /// Classe relai pour le branchement d'un cache de service associé à un <see cref="ComponentDesigner"/>.
  /// </summary>
  /// <typeparam name="TComponent">type du composant géré par le designer</typeparam>
  /// <typeparam name="TDesigner">type du designer client</typeparam>
  public class ComponentDesignerCache<TComponent, TDesigner> : ComponentDesigner 
    where TComponent : Component 
    where TDesigner : ComponentDesigner {

    #region Cache de service
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                           //
    //                                    Cache de service                                       //
    //                                                                                           //
    ///////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Classe proposant un cache et des services associés à un <see cref="ComponentDesigner"/>.
    /// </summary>
    /// <remarks>
    /// Le branchement de ce cache dans un designer suppose les insertions suivantes : <br/>
    /// - dans le constructeur : appel du constructeur du cache et adjonction des verbes 
    /// via <see cref="AddVerb(DesignerVerb)"/>, <see cref="AddVerb(string,EventHandler)"/> ou <see cref="AddVerbs"/><br/>
    /// - redéfinition de la propriété Verbs qui retourne simplement la collection <see cref="Verbs"/> du cache<br/>
    /// - redéfinition de la méthode Initialize qui appelle simplement la méthode <see cref="Initialize"/> du cache<br/>
    /// - redéfinition de la méthode Dispose qui appelle simplement la méthode <see cref="Dispose"/> du cache<br/>
    /// </remarks>
    public class Cache {

      /// <summary>
      /// Référence sur le designer pour lequel les objets de ComponentDesignerCache fonctionnent comme un cache d'accès.
      /// </summary>
      protected readonly TDesigner designer = null;

      /// <summary>
      /// Délégué pour la mise à jour des verbes
      /// </summary>
      private MethodInvoker onUpdateVerbs = null;

      /// <summary>
      /// Référence interne sur la collection des verbes (créée par nécessité)
      /// </summary>
      private DesignerVerbCollection verbs = null; // création par nécessité

      /// <summary>
      /// Référence interne sur l'hôte de conception (affectée lors du premier accès)
      /// </summary>
      private IDesignerHost designerHost = null; // affecté lors du premier accès

      /// <summary>
      /// Référence interne sur le service des références (affectée lors du premier accès)
      /// </summary>
      private IReferenceService referenceService = null; // affecté lors du premier accès

      /// <summary>
      /// Référence interne sur le service de sélection (affectée lors du premier accès)
      /// </summary>
      private ISelectionService selectionService = null; // affecté lors du premier accès

      /// <summary>
      /// Référence interne sur le service de changement des composants (affectée lors du premier accès)
      /// </summary>
      private IComponentChangeService componentChangeService = null; // affecté lors du premier accès

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="designer">référence sur le designer pour lequel le présent objet fonctionne comme un cache</param>
      /// <param name="onUpdateVerbs">délégué pour la mise à jour de l'état des verbes</param>
      public Cache( TDesigner designer, MethodInvoker onUpdateVerbs ) {
        this.designer = designer;
        this.onUpdateVerbs = onUpdateVerbs;
      }

      /// <summary>
      /// Initialisation d'une nouvelle instance du type de composant géré par le designer
      /// </summary>
      /// <param name="component">instance à initialiser</param>
      public virtual void Initialize( IComponent component ) {
        if ( ComponentChangeService != null ) ComponentChangeService.ComponentChanged += OnComponentChanged;
      }

      /// <summary>
      /// Libération des ressources gérées par le cache
      /// </summary>
      /// <param name="disposing"></param>
      public virtual void Dispose( bool disposing ) {
        if ( !disposing ) return;
        if ( ComponentChangeService != null ) ComponentChangeService.ComponentChanged -= OnComponentChanged;
      }

      private void OnComponentChanged( object sender, ComponentChangedEventArgs e ) {
        UpdateVerbs();
      }

      /// <summary>
      /// Mise à jour de l'état des verbes.
      /// </summary>
      /// <remarks>
      /// Cette méthode doit être redéfinie par les sous-classes si elles exposent des verbes
      /// nécessitant des mises à jour.
      /// </remarks>
      protected virtual void UpdateVerbs() {
        if ( onUpdateVerbs != null ) onUpdateVerbs();
      }

      /// <summary>
      /// Oblige les références mémorisées en cache à être ré-évaluées.
      /// </summary>
      public virtual void Invalidate() {
        designerHost = null;
        referenceService = null;
        selectionService = null;
        componentChangeService = null;
      }

      /// <summary>
      /// Obtient la référence bien typée sur le composant géré par le designer.
      /// </summary>
      public TComponent Component {
        get { return designer.Component as TComponent; }
      }

      /// <summary>
      /// Indique si au moins un verbe a été enregistré dans la liste des verbes.
      /// </summary>
      public bool HasVerbs {
        get { return verbs != null && verbs.Count > 0; }
      }

      /// <summary>
      /// Obtient la collection des verbes, ne retourne jamais null.
      /// </summary>
      public DesignerVerbCollection Verbs {
        get {
          if ( verbs == null ) verbs = new DesignerVerbCollection();
          return verbs;
        }
      }

      /// <summary>
      /// Ajouter plusieurs verbes à la collection des verbes.
      /// </summary>
      /// <param name="value">tableau des descripteurs de verbes à ajouter</param>
      public void AddVerbs( params DesignerVerb[] value ) {
        Verbs.AddRange( value );
      }

      /// <summary>
      /// Ajouter un verbe à la collection des verbes.
      /// </summary>
      /// <param name="value">descripeur de verbe à ajouter</param>
      /// <returns>la référence sur le descripteur de verbe ajouté</returns>
      public DesignerVerb AddVerb( DesignerVerb value ) {
        Verbs.Add( value );
        return value;
      }

      /// <summary>
      /// Créer un descripteur de verbe et l'ajouter à la collection des verbes.
      /// </summary>
      /// <param name="text">libellé associé au verbe</param>
      /// <param name="handler">délégué implémentant le verbe</param>
      /// <returns>le descripteur de verbe créé</returns>
      public DesignerVerb AddVerb( string text, EventHandler handler ) {
        return AddVerb( new DesignerVerb( text, handler ) );
      }

      /// <summary>
      /// Obtient la référence sur l'hôte de conception.
      /// </summary>
      /// <remarks>
      /// La référence obtenue est mémorisée lors du premier accès.
      /// </remarks>
      public IDesignerHost DesignerHost {
        get {
          if ( designerHost == null ) designerHost = (IDesignerHost) designer.Component.Site.GetService( typeof( IDesignerHost ) );
          return designerHost;
        }
      }

      /// <summary>
      /// Obtient la référence sur le service des références.
      /// </summary>
      /// <remarks>
      /// La référence obtenue est mémorisée lors du premier accès.
      /// </remarks>
      public IReferenceService ReferenceService {
        get {
          if ( referenceService == null ) referenceService = (IReferenceService) designer.Component.Site.GetService( typeof( IReferenceService ) );
          return referenceService;
        }
      }

      /// <summary>
      /// Obtient la référence sur le service des sélections.
      /// </summary>
      /// <remarks>
      /// La référence obtenue est mémorisée lors du premier accès.
      /// </remarks>
      public ISelectionService SelectionService {
        get {
          if ( selectionService == null ) selectionService = (ISelectionService) designer.Component.Site.GetService( typeof( ISelectionService ) );
          return selectionService;
        }
      }

      /// <summary>
      /// Obtient la référence sur le service de changement des composants.
      /// </summary>
      /// <remarks>
      /// La référence obtenue est mémorisée lors du premier accès.
      /// </remarks>
      public IComponentChangeService ComponentChangeService {
        get {
          if ( componentChangeService == null ) componentChangeService = (IComponentChangeService) designer.Component.Site.GetService( typeof( IComponentChangeService ) );
          return componentChangeService;
        }
      }

      /// <summary>
      /// Obtient la valeur d'une propriété d'un type donné d'un composant via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="source">composant source de la propriété</param>
      /// <param name="property">nom de la propriété</param>
      /// <param name="type">type attendu de la propriété</param>
      /// <returns>la valeur de la propriété, ou null si la propriété n'est pas accessible ou n'a pas le type voulu</returns>
      public object GetPropertyValue( Component source, string property, Type type ) {
        PropertyDescriptor descriptor = TypeDescriptor.GetProperties( source )[ property ];
        if ( descriptor == null || (type != null && descriptor.PropertyType != type) ) return null;
        return descriptor.GetValue( source );
      }

      /// <summary>
      /// Obtient la valeur d'une propriété d'un composant via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="source">composant source de la propriété</param>
      /// <param name="property">nom de la propriété</param>
      /// <returns>la valeur de la propriété, ou null si la propriété n'est pas accessible</returns>
      public object GetPropertyValue( Component source, string property ) {
        return GetPropertyValue( source, property, null );
      }

      /// <summary>
      /// Assigne une valeur à une propriété d'un type donné d'un composant via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="target">composant cible</param>
      /// <param name="property">nom de la propriété</param>
      /// <param name="value">valeur à affecter</param>
      /// <param name="type">type requis pour la propriété</param>
      public void SetPropertyValue( Component target, string property, object value, Type type ) {
        PropertyDescriptor descriptor = TypeDescriptor.GetProperties( target )[ property ];
        if ( descriptor == null || (type != null && descriptor.PropertyType != type) ) return;
        descriptor.SetValue( target, value );
      }

      /// <summary>
      /// Assigne une valeur à une propriété d'un composant via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="target">composant cible</param>
      /// <param name="property">nom de la propriété</param>
      /// <param name="value">valeur à affecter</param>
      public void SetPropertyValue( Component target, string property, object value ) {
        SetPropertyValue( target, property, value, null );
      }

      /// <summary>
      /// Affecte à un composant cible la valeur d'une propriété d'un type donné d'un composant source via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="source">composant source de la valeur</param>
      /// <param name="sourceProperty">nom de la propriété au sein du composant source</param>
      /// <param name="target">composant cible de l'affectation</param>
      /// <param name="targetProperty">nom de la propriété à affecter au sein du composant cible</param>
      /// <param name="type">type requis pour la propriété source et la propriété cible</param>
      /// <param name="AllowNull">si true, autorise l'affectation de la valeur null</param>
      public void AssignPropertyValue( Component source, string sourceProperty, Component target, string targetProperty, Type type, bool AllowNull ) {
        object value = GetPropertyValue( source, sourceProperty, type );
        if ( !AllowNull && value == null ) return;
        SetPropertyValue( target, targetProperty, value, type );
      }

      /// <summary>
      /// Affecte à un composant cible la valeur d'une propriété d'un composant source via <see cref="TypeDescriptor"/>.
      /// </summary>
      /// <param name="source">composant source de la valeur</param>
      /// <param name="sourceProperty">nom de la propriété au sein du composant source</param>
      /// <param name="target">composant cible de l'affectation</param>
      /// <param name="targetProperty">nom de la propriété à affecter au sein du composant cible</param>
      public void AssignPropertyValue( Component source, string sourceProperty, Component target, string targetProperty ) {
        AssignPropertyValue( source, sourceProperty, target, targetProperty, null, true );
      }

      /// <summary>
      /// Crée une transaction pour la modification groupée d'un composant.
      /// </summary>
      /// <param name="transaction">transaction créée, ou null si la création de la transaction n'a pas abouti</param>
      /// <param name="description">libellé de la transaction</param>
      /// <returns>true si la création de la transaction a abouti, false sinon</returns>
      public bool CreateTransaction( out DesignerTransaction transaction, string description ) {
        transaction = null;
        if ( DesignerHost == null ) return false;
        try {
          transaction = DesignerHost.CreateTransaction( description );
        }
        catch ( CheckoutException exception ) {
          if ( exception != CheckoutException.Canceled ) throw;
          return false;
        }
        return transaction != null;
      }

      /// <summary>
      /// Réalise le dialogue de modification d'une collection
      /// </summary>
      /// <param name="property">nom de la propriété à modifier</param>
      public void EditCollection( string property ) {
        EditorServiceContext service = new EditorServiceContext( this, property );
        service.EditValue();
      }

    }

    #endregion

    #region Contexte de services pour l'édition des collections
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                           //
    //                   Contexte de services pour l'édition des collections                     //
    //                                                                                           //
    ///////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Contexte de service pour l'édition de la valeur d'une propriété liée à un dialogue.
    /// </summary>
    /// <remarks>
    /// La classe homonyme du framework est internal. 
    /// La version proposée ici est adaptée à l'utilisation des caches. 
    /// </remarks>
    public class EditorServiceContext : IServiceProvider, IWindowsFormsEditorService, ITypeDescriptorContext {

      private Cache cache;
      private PropertyDescriptor descriptor;
      private UITypeEditor editor;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="cache">référence sur le cache associé au designer du composant</param>
      /// <param name="property">nom de la propriété à éditer</param>
      public EditorServiceContext( Cache cache, string property ) {
        this.cache = cache;
        descriptor = TypeDescriptor.GetProperties( cache.Component )[ property ];
        editor = descriptor.GetEditor( typeof( UITypeEditor ) ) as UITypeEditor;
      }

      /// <summary>
      /// Effectue l'édition de la valeur
      /// </summary>
      /// <returns>la valeur obtenue après édition</returns>
      public object EditValue() {
        object value = descriptor.GetValue( cache.Component );
        return editor.EditValue( this, this, value );
      }

      // membres de IServiceProvider

      /// <summary>
      /// Accès aux services
      /// </summary>
      /// <param name="serviceType">type du service requis</param>
      /// <returns>rééfrence sur le serveur, ou null si le service est introuvable</returns>
      public object GetService( Type serviceType ) {
        if ( serviceType == typeof( IWindowsFormsEditorService ) ) 
          return this;
        else
          return cache.Component.Site.GetService( serviceType );
      }

      // membres de IWindowsFormsEditorService

      /// <summary>
      /// Assure l'affichage du dialogue
      /// </summary>
      /// <param name="dialog">fenêtre du dialogue</param>
      /// <returns>le résultat du dialogue</returns>
      public DialogResult ShowDialog( Form dialog ) {
        dialog.ShowDialog();
        return dialog.DialogResult;
      }

      /// <summary>
      /// Affichage en drop down du dialogue pour les éditeurs en style drop down
      /// </summary>
      /// <param name="control">dialogue à afficher</param>
      public void DropDownControl( Control control ) {
        throw new NotImplementedException( GetType().FullName + ".DropDownControl(Control)" );
      }

      /// <summary>
      /// Ferme l'affichage drop down
      /// </summary>
      public void CloseDropDown() {
        throw new NotImplementedException( GetType().FullName + ".CloseDropDown()" );
      }

      // membres de ITypeDescriptorContext

      /// <summary>
      /// Autoriser un changement du composant
      /// </summary>
      /// <returns>true si la modification est autorisée, false sinon</returns>
      public bool OnComponentChanging() {
        try {
          cache.ComponentChangeService.OnComponentChanging( cache.Component, descriptor );
        }
        catch ( CheckoutException exception ) {
          if ( exception != CheckoutException.Canceled ) throw;
          return false;
        }
        return true;
      }

      /// <summary>
      /// Notifier une modification du composant
      /// </summary>
      public void OnComponentChanged() {
        cache.ComponentChangeService.OnComponentChanged( cache.Component, descriptor, null, null );
      }

      /// <summary>
      /// Obtenir le conteneur du composant
      /// </summary>
      public IContainer Container {
        get { return cache.Component.Site.Container; }
      }

      /// <summary>
      /// Obtenir le composant hébergeant la propriété à modifier
      /// </summary>
      public object Instance {
        get { return cache.Component; }
      }

      /// <summary>
      /// Obtenir le descripteur de propriété de la propriété à modifier
      /// </summary>
      public PropertyDescriptor PropertyDescriptor {
        get { return descriptor; }
      }
    }

    #endregion

    #region Relai d'accès au cache
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                           //
    //                           Relai d'accès au cache de service                               //
    //                                                                                           //
    ///////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Cache de service
    /// </summary>
    protected readonly Cache cache = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public ComponentDesignerCache() {
      cache = new Cache( this as TDesigner, DoUpdateVerbs );
    }

    /// <summary>
    /// Mise à jour de l'état des verbes
    /// </summary>
    protected virtual void DoUpdateVerbs() { }

    /// <summary>
    /// Libération des ressources
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose( bool disposing ) {
      cache.Dispose( disposing );
      base.Dispose( disposing );
    }

    /// <summary>
    /// Initialisation d'une nouvelle instance du type de composant géré par le designer
    /// </summary>
    /// <param name="component">instance à initialiser</param>
    public override void Initialize( IComponent component ) {
      base.Initialize( component );
      cache.Initialize( component );
    }

    /// <summary>
    /// Obtient la collection des verbes du designer.
    /// </summary>
    /// <remarks>
    /// Redéfinition de la propriété exposée par <see cref="ComponentDesigner"/>
    /// </remarks>
    public override DesignerVerbCollection Verbs {
      get { return cache.Verbs; }
    }
  }
    #endregion
}
