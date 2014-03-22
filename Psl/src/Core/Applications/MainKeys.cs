/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System.Windows.Forms;
using Psl.Controls;

namespace Psl.Applications {

  /// <summary>
  /// Constantes li�es au conteneur principal de l'application.
  /// </summary>
  /// <remarks>
  /// Cette classe regroupe les cl�s d'enregistrement des
  /// objets associ�s aux raccourcis impl�ment�s dans la classe <see cref="Registry"/>
  /// </remarks>
  public class MainKeys {

    /// <summary>
    /// Cl� de registre pour le service d'affichage <see cref="IStatusService"/>
    /// </summary>
    public const string KeyMainStatus = "Psl.Applications.main.status";

    /// <summary>
    /// Cl� de registre pour la barre de menu principale <see cref="MenuStrip"/>
    /// </summary>
    public const string KeyMainMenu = "Psl.Applications.main.menu";

    /// <summary>
    /// Cl� de registre pour le conteneur principal de barres d'outils <see cref="ToolStripPanelEnh"/>
    /// </summary>
    public const string KeyMainTools = "Psl.Applications.main.tools";

    /// <summary>
    /// Cl� de registre pour la zone conteneur principale (type <see cref="Control"/> ou d�riv�s)
    /// </summary>
    public const string KeyMainContent = "Psl.Applications.main.content";

    /// <summary>
    /// Cl� de registre pour un classeur � onglets <see cref="Psl.Controls.TabDocker"/>
    /// </summary>
    public const string KeyMainPages = "Psl.Applications.main.pages";

    /// <summary>
    /// Cl� de registre pour l'interface d'utilisation de l'archiveur <see cref="IArchiver"/>.
    /// </summary>
    public const string KeyMainArchiver = "Psl.Applications.main.archiver";

    /// <summary>
    /// Cl� de registre pour l'interface de gestion de l'archiveur <see cref="IArchiverManager"/>.
    /// </summary>
    public const string KeyMainArchiverManager = "Psl.Applications.main.archiver.manager";
  }
} // namespace
