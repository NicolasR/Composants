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
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 11 2008 : refonte du protocole des actions dans Action avec prise en charge améliorée des exceptions
 */                                                                            // <wao never.end>

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;

namespace Psl.Actions {

  public partial class ActionList {

    /// <summary>
    /// Collection des actions enregistrées dans un composant ActionList
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette extension de la classe Collection est destinée à compléter les opérations d'adjonction
    /// et de suppression des items (ici : des Action) de manière à tenir à jour le lien arrière Owner
    /// qui relie une action à sa liste d'action propriétaire. Au moment du design, cette collection 
    /// est associée à un éditeur de collection approprié nommé ActionCollectionEditor. 
    /// </para>
    /// <para>
    /// Cette collection peut être sollicitée de plusieurs façons : 
    /// (1) via le fichier designer du formulaire lorsque la liste d'action est initialisée (Add) ;
    /// (2) via l'éditeur de collection en mode design pour créer ou supprimer des actions (Clear, Add);
    /// (3) via la méthode Dispose d'une action lorsqu'une action est détruite (Remove);
    /// (4) par programme si le programme déplace des actions d'une liste à une autre (peu usuel) ;
    /// </para>
    /// <para>
    /// La difficulté de l'implémentation de cette collection réside dans le fait que l'éditeur de
    /// collection, après modification d'une collection, supprime tous les items (appel de ClearItems),
    /// puis réinsère, dans l'ordre, les items de la collection restants (appel de InsertItem). 
    /// Il s'ensuit qu'il est nécessaire de distinguer la phase de reconstruction d'une collection
    /// (liée à la fin de l'édition d'une collection) et les opérations qui sont réellement une
    /// adjonction ou une suppression d'actions. Cette distinction est opérée via le booléen "editing",
    /// tenu à jour via le parenthésage d'appels à BeginEdit et EndEdit déclenchés depuis la méthode
    /// Editing de l'éditeur de collection. 
    /// </para>
    /// <para>
    /// L'implémentation de la collection est équilibrée pour que toutes les opérations hors
    /// gestion explicite par programme conservent la cohérence des actions quant à leur appartenance
    /// à une liste d'actions. En ce qui concerne les opérations explicites par programme, peu usuelles,
    /// il revient au programmeur de s'assurer qu'une action n'est utilisée que lorsqu'elle est liée à une
    /// liste d'actions. 
    /// </para>
    /// </remarks>    
    [
      Editor( typeof( Psl.Design.ActionCollectionEditor ), typeof( UITypeEditor ) ), // Editor( "Psl.Design.ActionCollectionEditor, " + AssemblyRef.PslCoreDesign, typeof( UITypeEditor ) ),
    ] 
    public class ActionCollection : Collection<Action> {

      /// <summary>
      /// Lien arrière sur l'instance de Action propriétaire de la collection.
      /// </summary>
      private ActionList owner;

      /// <summary>
      /// Indique si la collection est en cours d'édition via l'éditeur de collection.
      /// </summary>
      private bool editing = false;

      /// <summary>
      /// Constructeur de la collection
      /// </summary>
      /// <param name="parent">référence sur la liste d'actions propriétaire de la collection</param>
      protected internal ActionCollection( ActionList parent ) {
        //Psl.Tracker.Tracker.Track( "ActionCollection.cctor.8" );
        this.owner = parent;
      }

      /// <summary>
      /// Accès à la liste d'actions propriétaire de la collection
      /// </summary>
      protected ActionList Owner {
        get { return owner; }
      }

      /// <summary>
      /// Ouvre une phase d'édition de la collection.
      /// </summary>
      /// <remarks>
      /// Méthode appelée depuis la méthode EditValue de ActionCollectionEditor.
      /// </remarks>
      public void BeginEdit() { // protected internal void BeginEdit() {
        editing = true;
      }

      /// <summary>
      /// Ferme une phase d'édition de la collection ouverte par <see cref="BeginEdit"/>
      /// </summary>
      /// <remarks>
      /// Méthode appelée depuis la méthode EditValue de ActionCollectionEditor.
      /// </remarks>
      public void EndEdit() { // protected internal void EndEdit() {
        editing = false;
      }

      /// <summary>
      /// Méthode déclenchée par la collection sur suppression des items de la collection. 
      /// </summary>
      /// <remarks>
      /// Les items de la collection sont seulement retirés de la collection, mais ne sont pas détruits.
      /// </remarks>
      protected override void ClearItems() {
        if ( !editing )
          foreach ( Action action in this )
            action.Owner = null;
        base.ClearItems();
      }

      /// <summary>
      /// Méthode déclenchée par la collection sur adjonction d'un source dans la collection. 
      /// </summary>
      /// <remarks>
      /// Hors phase d'édition, cette méthode gère l'éventuelle extraction préalable d'un source (une Action) 
      /// de sa liste d'action d'origine avant de l'insérer dans la présente collection. 
      /// En phase d'édition, l'appel à InsertItem n'est jamais lié à un changement de liste, mais seulement
      /// à une reconstruction de la liste comportant cependant éventuellement de nouveaux éléments.
      /// </remarks>
      /// <param name="index">index d'insertion de l'item</param>
      /// <param name="item">action à insérer</param>
      protected override void InsertItem( int index, Action item ) {
        if ( !editing )
          if ( item.Owner != null ) item.Owner.Actions.Remove( item );

        item.Owner = owner;
        base.InsertItem( index, item );
      }

      /// <summary>
      /// Méthode déclenchée par la collection sur suppression d'un source de la collection. 
      /// </summary>
      /// <remarks>
      /// L'source n'est pas détruit, mais seulement retiré de la collection.
      /// </remarks>
      /// <param name="index">index de l'source à supprimer</param>
      protected override void RemoveItem( int index ) {
        this[ index ].Owner = null;
        base.RemoveItem( index );
      }

      /// <summary>
      /// Méthode déclenchée par la collection pour remplacer un source de la collection. 
      /// </summary>
      /// <remarks>
      /// Cette opération n'est pas prise en charge.
      /// </remarks>
      /// <param name="index">index de l'source à supprimer</param>
      /// <param name="item">item associé à la modification</param>
      protected override void SetItem( int index, Action item ) {
        throw new EActionList( "(ActionCollection.SetItem) : opération non supportée" );
      }
    }
  }
}
