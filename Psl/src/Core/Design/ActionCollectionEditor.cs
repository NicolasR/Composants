/*                                                                           // <wao never.Begin>
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
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Psl.Actions;
using Action = Psl.Actions.Action;

namespace Psl.Design {

  // <exclude
  /// <summary>
  /// Editeur pour la collection des actions d'une liste d'actions.
  /// </summary>
  public class ActionCollectionEditor : CollectionEditor {

    /// <summary>
    /// Constructeur
    /// </summary>
    public ActionCollectionEditor() : base( typeof( ActionList.ActionCollection ) ) { }

    /// <summary>
    /// Obtient la collection des types des items qui peuvent être créés dans la collection.
    /// </summary>
    /// <returns>la collection des types</returns>
    protected override Type[] CreateNewItemTypes() {
      return new Type[] { typeof( Action ) } ;
    }

    /// <summary>
    /// Effectue l'édition d'un élément de la collection.
    /// </summary>
    /// <param name="context">contexte d'édition associé au type</param>
    /// <param name="provider">fournisseur de services</param>
    /// <param name="value">élément concerné par l'édition</param>
    /// <returns>la nouvelle valeur</returns>
    public override object EditValue( ITypeDescriptorContext context, IServiceProvider provider, object value ) {
      object result = null;

      ((ActionList.ActionCollection) value).BeginEdit() ;
      try {
        result = base.EditValue( context, provider, value );
      }
      finally {
        ((ActionList.ActionCollection) value).EndEdit(); 
      }

      return result ;
    }
  }
}
