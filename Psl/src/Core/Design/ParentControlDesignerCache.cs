/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 05 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Psl.Design {

  /// <summary>
  /// Classe proposant un cache et des services associés à un <see cref="ControlDesigner"/>.
  /// </summary>
  /// <typeparam name="TControl">type du contrôle géré par le designer</typeparam>
  /// <typeparam name="TDesigner">type du designer client</typeparam>
  public class ParentControlDesignerCache<TControl, TDesigner> : ParentControlDesigner where TControl : Control where TDesigner : ParentControlDesigner {

    #region Cache de service
    ///////////////////////////////////////////////////////////////////////////////////////////////
    //                                                                                           //
    //                                    Cache de service                                       //
    //                                                                                           //
    ///////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Implémentation du cache pour les contrôles
    /// </summary>
    public class Cache : ControlDesignerCache<TControl, TDesigner>.Cache {

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="designer">référence sur le designer client du cache</param>
      /// <param name="onUpdateVerbs">délégué pour la mise à jour de l'état des verbes</param>
      public Cache( TDesigner designer, MethodInvoker onUpdateVerbs ) : base( designer, onUpdateVerbs ) { }
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
    public ParentControlDesignerCache() {
      cache = new Cache( this as TDesigner, DoUpdateVerbs );
    }

    /// <summary>
    /// Mise à jour de l'état des verbes
    /// </summary>
    protected virtual void DoUpdateVerbs() {
    }

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
