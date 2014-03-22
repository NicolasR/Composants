/*                                                                                               // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Gestionnaire des plugins dynamiques
 * 
 * 31 01 2008 : version initiale pour aihm 2007-2008
 * 01 06 2010 : adjonction du hook VerboseOff pour éviter l'affichage modal de la fenêtre en mode verbose
 */
using System;
using System.ComponentModel;
                                                                                                   // <wao never.end>
namespace Psl.Applications {
                                                                                                   // <wao PslPluginInstaller.begin>
  /// <summary>                                                                                    
  /// Classe d'attribut pour tagguer les classes d'installation de plugins.
  /// </summary>
  /// <remarks>
  /// Pour être détectée comme un germe d'installation de plugin, une classe doit être taggée
  /// au moyen de cet attribut.
  /// <br/>
  /// Une classe tagguée par cet attribut doit comporter une méthode de classe :
  /// <code>public static void Install()</code>
  /// Si une telle méthode est introuvable, un diagnostic d'erreur est émis.
  /// <br/>
  /// Au sein d'une librairie dll, plusieurs classes peuvent jouer le rôle de germe d'installation. 
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class PslPluginInstaller : Attribute {}                                                   // <wao PslPluginInstaller.end>

  /// <summary>
  /// Fonctionnalités du gestionnaire des plugins dynamiques.
  /// </summary>
  /// <remarks>
  /// Cette classe expose les fonctionnalités liées au chargement dynamique des plugins
  /// et à leur installation sous la forme de méthodes de classes.
  /// <br/>
  /// L'implémentation est reportée dans l'espace Psl.Applications.Manager.
  /// </remarks>
  public class PluginManager {

    /// <summary>
    /// Filtre par défaut pour la recherche des dll à charger.
    /// </summary>
    private static string DefaultFileFilter = "*plugin.dll";

    /// <summary>
    /// Chargement et installation des plugins.
    /// </summary>
    /// <param name="verbose">si true, affiche le rapport en fin de chargement</param>
    /// <param name="fileFilter">filtre pour les librairies à charger</param>
    /// <returns>true si aucun diagnostic n'a été détecté</returns>
    public static bool LoadPlugins( bool verbose, string fileFilter ) {                            // <wao full.begin.&body>
      return Psl.Applications.Manager.PluginManager.Manager.LoadPlugins( verbose, fileFilter );
    }                                                                                              // <wao full.end>

    /// <summary>
    /// Chargement et installation des plugins (filtre par défaut).
    /// </summary>
    /// <param name="verbose">si true, affiche le rapport en fin de chargement</param>
    /// <returns>true si aucun diagnostic n'a été détecté</returns>
    public static bool LoadPlugins( bool verbose ) {                                               // <wao full.begin.&body>
      return LoadPlugins( verbose, DefaultFileFilter );
    }                                                                                              // <wao full.end>

    /// <summary>
    /// Chargement et installation des plugins (mode bavard et filtre par défaut).
    /// </summary>
    /// <returns>true si aucun diagnostic n'a été détecté</returns>
    public static bool LoadPlugins() {                                                             // <wao full.begin.&body>
      return LoadPlugins( true, DefaultFileFilter );
    }                                                                                              // <wao full.end>
                                                                                                   
    /// <summary>
    /// Affiche le rapport sur l'état courant du gestionnaire de plugins.
    /// </summary>
    public static void ShowReport() {                                                              // <wao full.begin.&body>
      Psl.Applications.Manager.PluginManager.Manager.ShowReport( true );
    }                                                                                              // <wao full.end>
                                                                                                   // <wao never.begin>
    /// <summary>
    /// Indique si le mode bavard de la méthode LoadPlugins doit être inhibé
    /// </summary>
    /// <remarks>
    /// Hook permettant de forcer la clôture automatique de la fenêtre s'il n'y a pas de diagnostics
    /// </remarks>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static bool VerboseOff {
      get { return Psl.Applications.Manager.PluginManager.VerboseOff; }
      set { Psl.Applications.Manager.PluginManager.VerboseOff = value; }
    }
                                                                                                   // <wao never.end>
  }
}
