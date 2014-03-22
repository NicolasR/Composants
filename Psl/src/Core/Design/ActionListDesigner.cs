/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design ;
using Psl.Actions;

namespace Psl.Design {

  // <exclude
  /// <summary>
  /// Designer associé à une liste d'actions.
  /// </summary>
  public class ActionListDesigner : ComponentDesignerCache<ActionList, ActionListDesigner> {

    /// <summary>
    /// Descripteur du verbe de création d'une nouvelle action
    /// </summary>
    private DesignerVerb verbNewAction = null ;

    /// <summary>
    /// Descripteur du verbe d'édition de la collection des actions
    /// </summary>
    private DesignerVerb verbEditActions = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public ActionListDesigner() {
      verbNewAction   = cache.AddVerb( "Créer une nouvelle action"      , DoVerbNewAction );
      verbEditActions = cache.AddVerb( "Gérer la collection des actions", DoVerbEditActions );
    }

    //
    // Redéfinition de membres hérités
    //

    /// <summary>
    /// Implémente l'action par défaut pour ce designer : créer une nouvelle action.
    /// </summary>
    public override void DoDefaultAction() {
      DoVerbNewAction( this, EventArgs.Empty );
    }

    //
    // Verbes
    //

    /// <summary>
    /// Implémente le verbe de création d'une nouvelle action.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbNewAction( object sender, EventArgs e ) {
      cache.Invalidate();      

      // Créer une nouvelle action via l'hôte de conception
      if ( cache.DesignerHost == null ) return;
      Psl.Actions.Action action = cache.DesignerHost.CreateComponent( typeof( Psl.Actions.Action ) ) as Psl.Actions.Action;

      // déterminer le texte par défaut de l'action 
      if ( action == null ) return;
      if ( cache.ReferenceService == null ) return;
      action.Text = cache.ReferenceService.GetName( action );

      // ajouter l'action à la liste d'actions
      (Component as ActionList).Actions.Add( action );

      // sélectionner l'action créée
      if ( cache.SelectionService == null ) return;
      cache.SelectionService.SetSelectedComponents( new object[] { action } );
    }

    /// <summary>
    /// Implémente le verbe d'édition de la collection des actions.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbEditActions( object sender, EventArgs e ) {
      cache.EditCollection( "Actions" );
    }
  }
}
