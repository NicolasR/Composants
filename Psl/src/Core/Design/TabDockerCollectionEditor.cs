/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel.Design;
using Psl.Controls;

namespace Psl.Design {

  /// <summary>
  /// Editeur associé à la collection des <see cref="TabDockerPage"/> d'un <see cref="TabDocker"/>.
  /// </summary>
  public class TabDockerCollectionEditor : CollectionEditor {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="type">type de la collection à modifier au moyen de cet éditeur</param>
    public TabDockerCollectionEditor( Type type ) : base( type ) { }

    /// <summary>
    /// Crée un nouveau formulaire pour afficher et modifier la collection en cours
    /// </summary>
    /// <returns>formulaire à proposer comme interface utilisateur pour la modification de la collection</returns>
    protected override CollectionEditor.CollectionForm CreateCollectionForm() {
      CollectionForm baseForm = base.CreateCollectionForm();
      baseForm.Text = "Editeur de collections TabPageDocker";
      return baseForm;
    }

    /// <summary>
    /// Obtient le type des données contenues dans cette collection.
    /// </summary>
    /// <returns>le type des données contenues dans cette collection</returns>
    protected override Type CreateCollectionItemType() {
      return typeof( TabDockerPage );
    }

    /// <summary>
    /// Obtient les types de données que cet éditeur de collections peut contenir.
    /// </summary>
    /// <returns>tableau des types de données que cette collection peut contenir</returns>
    protected override Type[] CreateNewItemTypes() {
      return new Type[] { typeof( TabDockerPage ) };
    }
  }
}
