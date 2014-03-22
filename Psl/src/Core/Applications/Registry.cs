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
  /// Registre interne d'application.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Cette classe ne comporte que des membres de classe et ne doit donc jamais être instanciée. 
  /// </para>
  /// <para>
  /// Extension de la classe <see cref="BaseRegistry"/> fournissant quelques raccourcis d'accès usuels
  /// bien typés : ce sont les raccourcis qui correspondent aux clés d'enregistrement de
  /// la classe <see cref="MainKeys"/>.
  /// </para>
  /// </remarks>
  public partial class Registry : BaseRegistry {																			//	<wao spécif.&header>

    /// <summary>
    /// Raccourci d'accès au service d'affichage <see cref="IStatusService"/>.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond à une entrée très probablement présente dans 
    /// une application et retourne une référence bien qualifiée pour alléger
    /// les écritures.
    /// <br/>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un serveur de l'interface <see cref="IStatusService"/>
    /// sous la clé <see cref="MainKeys.KeyMainStatus"/>
    /// </remarks>
    public static IStatusService MainStatus {                                 // <wao spécif.&body:rw>
      get { return (IStatusService) Get( MainKeys.KeyMainStatus ) ; }
      set { Add( MainKeys.KeyMainStatus, value ); }
    }
	

    /// <summary>
    /// Raccourci d'accès à la barre de menu principale de l'aplication.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond à une entrée très probablement présente dans 
    /// une application et retourne une référence bien qualifiée pour alléger
    /// les écritures. 
    /// <br/>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// une barre de menus <see cref="MenuStrip"/> 
    /// sous la clé <see cref="MainKeys.KeyMainMenu"/>
    /// </remarks>
    public static MenuStrip MainMenu {                                          // <wao spécif.&body:rw>
      get { return (MenuStrip) Get( MainKeys.KeyMainMenu ) ; }
      set { Add( MainKeys.KeyMainMenu, value ); }
    }

    /// <summary>
    /// Raccourci d'accès au conteneur principal de barres d'outils.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond à une entrée très probablement présente dans 
    /// une application et retourne une référence bien qualifiée pour alléger
    /// les écritures.
    /// <br/>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un conteneur de barres d'outils <see cref="ToolStripPanel"/> 
    /// sous la clé <see cref="MainKeys.KeyMainTools"/>
    /// </remarks>
    public static ToolStripPanel MainTools {                                   // <wao spécif.&body:rw>
      get { return (ToolStripPanel) Get( MainKeys.KeyMainTools ); }
      set { Add( MainKeys.KeyMainTools, value ); }
    }
	
    /// <summary>
    /// Raccourci d'accès au conteneur principal de contrôles visuels
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond à une entrée très probablement présente dans 
    /// une application et retourne une référence bien qualifiée pour alléger
    /// les écritures.
    /// <br/>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un conteneur <see cref="Control"/> (ou dérivé)
    /// sous la clé <see cref="MainKeys.KeyMainContent"/>
    /// </remarks>
    public static Control MainContent {                                        //	<wao spécif.&body:rw>
      get { return (Control) Get( MainKeys.KeyMainContent ) ; }
      set { Add( MainKeys.KeyMainContent, value ); }
    }
	
    /// <summary>
    /// Raccourci d'accès au conteneur principal de contrôles visuels
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond à une entrée très probablement présente dans 
    /// une application et retourne une référence bien qualifiée pour alléger
    /// les écritures.
    /// <br/>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un classeur à onglets <see cref="TabDocker"/> 
    /// sous la clé <see cref="MainKeys.KeyMainPages"/>
    /// </remarks>
    public static TabDocker MainPages {                                        //	<wao spécif.&body:rw>
      get { return (TabDocker) Get( MainKeys.KeyMainPages ) ; }
      set { Add( MainKeys.KeyMainPages, value ); }
    }
	
    /// <summary>
    /// Raccourci d'accès à l'interface d'utilisation l'archiveur de l'application.
    /// </summary>
    /// <remarks>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un archiveur <see cref="IArchiver"/> sous la clé <see cref="MainKeys.KeyMainArchiver"/>
    /// </remarks>
    public static IArchiver MainArchiver {                                     // <wao spécif.&body:rw>
      get { return (IArchiver) Get( MainKeys.KeyMainArchiver ) ; }
      set { Add( MainKeys.KeyMainArchiver, value ); }
    }

    /// <summary>
    /// Raccourci d'accès à l'interface de gestion de archiveur de l'application.
    /// </summary>
    /// <remarks>
    /// Ce raccourci n'est opérationnel que si l'application a enregistré 
    /// un archiveur <see cref="IArchiverManager"/> sous la clé <see cref="MainKeys.KeyMainArchiverManager"/>
    /// </remarks>
    public static IArchiverManager MainArchiverManager {                      // <wao spécif.&body:rw>
      get { return (IArchiverManager) Get( MainKeys.KeyMainArchiverManager ) ; }
      set { Add( MainKeys.KeyMainArchiverManager, value ); }
    }

                                                                               // <wao show>
    /// <summary>
    /// Immersion de menus <see cref="MenuStrip"/> dans un menu principal.
    /// </summary>
    /// <param name="menu">menu à immerger dans le menu principal</param>
    public static void MergeInMainMenu( MenuStrip menu ) {                     // <wao spécif.&body>
      if (menu == null) throw new ERegistry( "MergeInMainMenu", "l'argument 'menu' est null" );

      MenuStrip host = (MenuStrip) GetIf( MainKeys.KeyMainMenu );
      if (host == null) throw new ERegistry( MainKeys.KeyMainMenu, "MergeInMainMenu", "le menu principal est introuvable dans le registre d'application" );

      MergeMenu( host, menu );
    }

    /// <summary>
    /// Immersion de barres d'outils <see cref="ToolStrip"/> dans un <see cref="ToolStripContainer"/>.
    /// </summary>
    /// <param name="tools">menu à immerger dans le menu principal</param>
    public static void MergeInMainTools( ToolStrip tools ) {                   // <wao spécif.&body>
      if (tools == null) throw new ERegistry( "MergeInMainTools", "l'argument 'tools' est null" );

      ToolStripPanel host = (ToolStripPanel) GetIf( MainKeys.KeyMainTools );
      if (host == null) throw new ERegistry( MainKeys.KeyMainTools, "MergeInMainTools", "le conteneur de barres d'outils est introuvable dans le registre d'application" );

      MergeTools( host, tools );
    }
  }                                                                            // <wao spécif.&ender>
} // namespace
