/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 05 10 2009 : adjonction de l'option d'installation silencieuse
 */                                                                            // <wao never.end>
using System;                                                                  // <wao never.begin>
using Psl.Applications ;                                                      // <wao never.end>

namespace Psl.Applications {                                                  // <wao Archiver_plugin.begin>
                                                                               
  /// <summary>
  /// Germe statique d'installation du plugin d'archivage.
  /// </summary>
  [PslPluginInstaller]
  public static class ArchiverPlugin {

    /// <summary>
    /// Installation du plugin d'archivage
    /// </summary>
    public static void Install() {

      // Créer et enregistrer le composant d'archivage
      Archiver archiver            = new Archiver() ;
      Registry.MainArchiver        = archiver;
      Registry.MainArchiverManager = archiver;

      // Créer le cluster ihm d'archivage
      new ArchiverCluster() ;
    }
                                                                               // <wao never.begin>
    // Cette seconde signature de Install est laissée en copie parce que la première signature
    // sert plusieurs fois d'exemple de germe de plugin dans la documentation.

    /// <summary>
    /// Installation du plugin d'archivage avec option d'installation silencieuse
    /// </summary>
    /// <remarks>
    /// Cette signature de Install n'est pas documentée dans le poly.
    /// Elle permet d'utiliser le plugin d'archivage avant d'avoir étudié les plugins
    /// </remarks>
    /// <param name="allowSilent">si true, autorise l'installation du plugin même si les conteneurs généraux ne sont pas enregistrés</param>
    public static void Install( bool allowSilent ) {

      // Créer et enregistrer le composant d'archivage
      Archiver archiver            = new Archiver() ;
      Registry.MainArchiver        = archiver;
      Registry.MainArchiverManager = archiver;

      // Créer le cluster ihm d'archivage
      new ArchiverCluster( allowSilent ) ;
    }                                                                          // <wao never.end>
  }                                                                               
}                                                                              // <wao Archiver_plugin.end>
