/*                                                                                               // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * Gestionnaire des plugins dynamiques
 * 
 * 31 01 2008 : version initiale pour aihm 2007-2008
 * 01 06 2010 : adjonction du hook VerboseOff pour �viter l'affichage modal de la fen�tre en mode verbose
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
  /// Pour �tre d�tect�e comme un germe d'installation de plugin, une classe doit �tre tagg�e
  /// au moyen de cet attribut.
  /// <br/>
  /// Une classe taggu�e par cet attribut doit comporter une m�thode de classe :
  /// <code>public static void Install()</code>
  /// Si une telle m�thode est introuvable, un diagnostic d'erreur est �mis.
  /// <br/>
  /// Au sein d'une librairie dll, plusieurs classes peuvent jouer le r�le de germe d'installation. 
  /// </remarks>
  [AttributeUsage(AttributeTargets.Class)]
  public class PslPluginInstaller : Attribute {}                                                   // <wao PslPluginInstaller.end>

  /// <summary>
  /// Fonctionnalit�s du gestionnaire des plugins dynamiques.
  /// </summary>
  /// <remarks>
  /// Cette classe expose les fonctionnalit�s li�es au chargement dynamique des plugins
  /// et � leur installation sous la forme de m�thodes de classes.
  /// <br/>
  /// L'impl�mentation est report�e dans l'espace Psl.Applications.Manager.
  /// </remarks>
  public class PluginManager {

    /// <summary>
    /// Filtre par d�faut pour la recherche des dll � charger.
    /// </summary>
    private static string DefaultFileFilter = "*plugin.dll";

    /// <summary>
    /// Chargement et installation des plugins.
    /// </summary>
    /// <param name="verbose">si true, affiche le rapport en fin de chargement</param>
    /// <param name="fileFilter">filtre pour les librairies � charger</param>
    /// <returns>true si aucun diagnostic n'a �t� d�tect�</returns>
    public static bool LoadPlugins( bool verbose, string fileFilter ) {                            // <wao full.begin.&body>
      return Psl.Applications.Manager.PluginManager.Manager.LoadPlugins( verbose, fileFilter );
    }                                                                                              // <wao full.end>

    /// <summary>
    /// Chargement et installation des plugins (filtre par d�faut).
    /// </summary>
    /// <param name="verbose">si true, affiche le rapport en fin de chargement</param>
    /// <returns>true si aucun diagnostic n'a �t� d�tect�</returns>
    public static bool LoadPlugins( bool verbose ) {                                               // <wao full.begin.&body>
      return LoadPlugins( verbose, DefaultFileFilter );
    }                                                                                              // <wao full.end>

    /// <summary>
    /// Chargement et installation des plugins (mode bavard et filtre par d�faut).
    /// </summary>
    /// <returns>true si aucun diagnostic n'a �t� d�tect�</returns>
    public static bool LoadPlugins() {                                                             // <wao full.begin.&body>
      return LoadPlugins( true, DefaultFileFilter );
    }                                                                                              // <wao full.end>
                                                                                                   
    /// <summary>
    /// Affiche le rapport sur l'�tat courant du gestionnaire de plugins.
    /// </summary>
    public static void ShowReport() {                                                              // <wao full.begin.&body>
      Psl.Applications.Manager.PluginManager.Manager.ShowReport( true );
    }                                                                                              // <wao full.end>
                                                                                                   // <wao never.begin>
    /// <summary>
    /// Indique si le mode bavard de la m�thode LoadPlugins doit �tre inhib�
    /// </summary>
    /// <remarks>
    /// Hook permettant de forcer la cl�ture automatique de la fen�tre s'il n'y a pas de diagnostics
    /// </remarks>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static bool VerboseOff {
      get { return Psl.Applications.Manager.PluginManager.VerboseOff; }
      set { Psl.Applications.Manager.PluginManager.VerboseOff = value; }
    }
                                                                                                   // <wao never.end>
  }
}
