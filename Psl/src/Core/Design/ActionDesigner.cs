/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 01 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.Design;
using Psl.Actions;
using Action = Psl.Actions.Action;

namespace Psl.Design {

  // <exclude
  /// <summary>
  /// Designer associé au composant <see cref="Action"/>
  /// </summary>
  public class ActionDesigner : ComponentDesignerCache<Action, ActionDesigner> {

    /// <summary>
    /// Descripteur du verbe de création d'une nouvelle action.
    /// </summary>
    private DesignerVerb verbNewAction = null;

    /// <summary>
    /// Descripteur du verbe de création d'un corps associé à l'action.
    /// </summary>
    private DesignerVerb verbNewBody = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public ActionDesigner() {
      verbNewBody   = cache.AddVerb( "Créer un corps d'action pour cette action"   , DoVerbNewBody   );
      verbNewAction = cache.AddVerb( "Créer une nouvelle action dans la même liste", DoVerbNewAction );
    }

    /// <summary>
    /// Mise à jour de l'état des verbes.
    /// </summary>
    protected override void DoUpdateVerbs() {
      base.DoUpdateVerbs();
      bool owned = GetOwnerDesigner() != null;
      verbNewAction.Enabled = owned;
    }

    //
    // Verbes
    //

    /// <summary>
    /// Obtient le <see cref="ActionListDesigner"/> de la liste d'action propriétaire de l'action associée au designer.
    /// </summary>
    /// <returns>la designer de la liste d'action propriétaire de l'action, ou null</returns>
    private ActionListDesigner GetOwnerDesigner() {
      if ( cache.Component == null ) return null;

      ActionList owner = cache.Component.Owner;
      if ( owner == null ) return null;

      if ( cache.DesignerHost == null ) return null;
      return cache.DesignerHost.GetDesigner( owner ) as ActionListDesigner;
    }

    /// <summary>
    /// Implémente le verbe de création d'un corps d'action.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbNewBody( object sender, EventArgs e ) {
      DoDefaultAction();
    }

    /// <summary>
    /// Implémente le verbe de création d'une nouvelle action dans la même liste d'actions.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbNewAction( object sender, EventArgs e ) {
      ActionListDesigner designer = GetOwnerDesigner();
      if ( designer == null ) return;
      designer.DoVerbNewAction( this, EventArgs.Empty );
    }

  }
}
