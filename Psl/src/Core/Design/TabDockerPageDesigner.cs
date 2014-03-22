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
  /// Designer pour les pages <see cref="TabDockerPage"/> du contrôle <see cref="TabDocker"/>.
  /// </summary>
  public class TabDockerPageDesigner : PanelDesignerCache<TabDockerPage, TabDockerPageDesigner> {

    /// <summary>
    /// Descripteur du verbe d'adjonction d'une page
    /// </summary>
    private DesignerVerb verbAddPage = null;

    /// <summary>
    /// Descripteur du verbe de suppression d'une page
    /// </summary>
    private DesignerVerb verbRemovePage = null;

    /// <summary>
    /// Descripteur du verbe d'édition de la collection des pages
    /// </summary>
    private DesignerVerb verbEditPages = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public TabDockerPageDesigner() {
      verbAddPage    = cache.AddVerb( new DesignerVerb( "Ajouter un onglet docker"     , DoVerbAddPage    ) );
      verbRemovePage = cache.AddVerb( new DesignerVerb( "Supprimer l'onglet docker"    , DoVerbRemovePage ) );
      verbEditPages  = cache.AddVerb( new DesignerVerb( "Gérer la collection des pages", DoVerbEditPages  ) );
    }

    /// <summary>
    /// Mise à jour de l'état des verbes
    /// </summary>
    protected override void DoUpdateVerbs() {
      TabDocker docker = ParentComponent as TabDocker;
      bool accessible = docker != null;
      verbAddPage.Enabled = accessible;
      verbRemovePage.Enabled = accessible && docker.Controls.Count > 0;
    }

    //
    // Redéfinition de méthodes
    //

    /// <summary>
    /// Détermine si la page associée au designer peut être placée comme contrôle enfant du contrôle géré par parentDesigner.
    /// </summary>
    /// <param name="parentDesigner">designer du contrôle parent</param>
    /// <returns>true si la page peut être ajoutées aux enfants du contrôle parent</returns>
    public override bool CanBeParentedTo( IDesigner parentDesigner ) {
      return parentDesigner != null && parentDesigner is TabDockerDesigner;
    }

    //
    // Verbes
    //

    /// <summary>
    /// Réalisation du verbe d'adjonction d'une page
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void DoVerbAddPage( object sender, EventArgs e ) {
      TabDockerDesigner tabDesigner = cache.DesignerOfParent as TabDockerDesigner;
      tabDesigner.DoVerbAddPage( sender, e );
    }

    /// <summary>
    /// Réalisation du verbe de suppression d'une page
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void DoVerbRemovePage( object sender, EventArgs e ) {
      TabDockerDesigner tabDesigner = cache.DesignerOfParent as TabDockerDesigner;
      tabDesigner.DoVerbRemovePage( sender, e );
    }

    /// <summary>
    /// Implémente le verbe d'édition de la collection des pages.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbEditPages( object sender, EventArgs e ) {
      TabDockerDesigner tabDesigner = cache.DesignerOfParent as TabDockerDesigner;
      tabDesigner.DoVerbEditPages( sender, e );
    }

  }
}
