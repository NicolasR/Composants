/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System.Windows.Forms;
using Psl.Controls;

namespace Psl.Applications {

  /// <summary>
  /// Constantes liées au conteneur principal de l'application.
  /// </summary>
  /// <remarks>
  /// Cette classe regroupe les clés d'enregistrement des
  /// objets associés aux raccourcis implémentés dans la classe <see cref="Registry"/>
  /// </remarks>
  public class MainKeys {

    /// <summary>
    /// Clé de registre pour le service d'affichage <see cref="IStatusService"/>
    /// </summary>
    public const string KeyMainStatus = "Psl.Applications.main.status";

    /// <summary>
    /// Clé de registre pour la barre de menu principale <see cref="MenuStrip"/>
    /// </summary>
    public const string KeyMainMenu = "Psl.Applications.main.menu";

    /// <summary>
    /// Clé de registre pour le conteneur principal de barres d'outils <see cref="ToolStripPanelEnh"/>
    /// </summary>
    public const string KeyMainTools = "Psl.Applications.main.tools";

    /// <summary>
    /// Clé de registre pour la zone conteneur principale (type <see cref="Control"/> ou dérivés)
    /// </summary>
    public const string KeyMainContent = "Psl.Applications.main.content";

    /// <summary>
    /// Clé de registre pour un classeur à onglets <see cref="Psl.Controls.TabDocker"/>
    /// </summary>
    public const string KeyMainPages = "Psl.Applications.main.pages";

    /// <summary>
    /// Clé de registre pour l'interface d'utilisation de l'archiveur <see cref="IArchiver"/>.
    /// </summary>
    public const string KeyMainArchiver = "Psl.Applications.main.archiver";

    /// <summary>
    /// Clé de registre pour l'interface de gestion de l'archiveur <see cref="IArchiverManager"/>.
    /// </summary>
    public const string KeyMainArchiverManager = "Psl.Applications.main.archiver.manager";
  }
} // namespace
