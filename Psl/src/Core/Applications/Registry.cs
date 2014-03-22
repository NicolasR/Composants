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
  /// Registre interne d'application.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Cette classe ne comporte que des membres de classe et ne doit donc jamais �tre instanci�e. 
  /// </para>
  /// <para>
  /// Extension de la classe <see cref="BaseRegistry"/> fournissant quelques raccourcis d'acc�s usuels
  /// bien typ�s : ce sont les raccourcis qui correspondent aux cl�s d'enregistrement de
  /// la classe <see cref="MainKeys"/>.
  /// </para>
  /// </remarks>
  public partial class Registry : BaseRegistry {																			//	<wao sp�cif.&header>

    /// <summary>
    /// Raccourci d'acc�s au service d'affichage <see cref="IStatusService"/>.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond � une entr�e tr�s probablement pr�sente dans 
    /// une application et retourne une r�f�rence bien qualifi�e pour all�ger
    /// les �critures.
    /// <br/>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un serveur de l'interface <see cref="IStatusService"/>
    /// sous la cl� <see cref="MainKeys.KeyMainStatus"/>
    /// </remarks>
    public static IStatusService MainStatus {                                 // <wao sp�cif.&body:rw>
      get { return (IStatusService) Get( MainKeys.KeyMainStatus ) ; }
      set { Add( MainKeys.KeyMainStatus, value ); }
    }
	

    /// <summary>
    /// Raccourci d'acc�s � la barre de menu principale de l'aplication.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond � une entr�e tr�s probablement pr�sente dans 
    /// une application et retourne une r�f�rence bien qualifi�e pour all�ger
    /// les �critures. 
    /// <br/>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// une barre de menus <see cref="MenuStrip"/> 
    /// sous la cl� <see cref="MainKeys.KeyMainMenu"/>
    /// </remarks>
    public static MenuStrip MainMenu {                                          // <wao sp�cif.&body:rw>
      get { return (MenuStrip) Get( MainKeys.KeyMainMenu ) ; }
      set { Add( MainKeys.KeyMainMenu, value ); }
    }

    /// <summary>
    /// Raccourci d'acc�s au conteneur principal de barres d'outils.
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond � une entr�e tr�s probablement pr�sente dans 
    /// une application et retourne une r�f�rence bien qualifi�e pour all�ger
    /// les �critures.
    /// <br/>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un conteneur de barres d'outils <see cref="ToolStripPanel"/> 
    /// sous la cl� <see cref="MainKeys.KeyMainTools"/>
    /// </remarks>
    public static ToolStripPanel MainTools {                                   // <wao sp�cif.&body:rw>
      get { return (ToolStripPanel) Get( MainKeys.KeyMainTools ); }
      set { Add( MainKeys.KeyMainTools, value ); }
    }
	
    /// <summary>
    /// Raccourci d'acc�s au conteneur principal de contr�les visuels
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond � une entr�e tr�s probablement pr�sente dans 
    /// une application et retourne une r�f�rence bien qualifi�e pour all�ger
    /// les �critures.
    /// <br/>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un conteneur <see cref="Control"/> (ou d�riv�)
    /// sous la cl� <see cref="MainKeys.KeyMainContent"/>
    /// </remarks>
    public static Control MainContent {                                        //	<wao sp�cif.&body:rw>
      get { return (Control) Get( MainKeys.KeyMainContent ) ; }
      set { Add( MainKeys.KeyMainContent, value ); }
    }
	
    /// <summary>
    /// Raccourci d'acc�s au conteneur principal de contr�les visuels
    /// </summary>
    /// <remarks>
    /// Un raccourci correspond � une entr�e tr�s probablement pr�sente dans 
    /// une application et retourne une r�f�rence bien qualifi�e pour all�ger
    /// les �critures.
    /// <br/>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un classeur � onglets <see cref="TabDocker"/> 
    /// sous la cl� <see cref="MainKeys.KeyMainPages"/>
    /// </remarks>
    public static TabDocker MainPages {                                        //	<wao sp�cif.&body:rw>
      get { return (TabDocker) Get( MainKeys.KeyMainPages ) ; }
      set { Add( MainKeys.KeyMainPages, value ); }
    }
	
    /// <summary>
    /// Raccourci d'acc�s � l'interface d'utilisation l'archiveur de l'application.
    /// </summary>
    /// <remarks>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un archiveur <see cref="IArchiver"/> sous la cl� <see cref="MainKeys.KeyMainArchiver"/>
    /// </remarks>
    public static IArchiver MainArchiver {                                     // <wao sp�cif.&body:rw>
      get { return (IArchiver) Get( MainKeys.KeyMainArchiver ) ; }
      set { Add( MainKeys.KeyMainArchiver, value ); }
    }

    /// <summary>
    /// Raccourci d'acc�s � l'interface de gestion de archiveur de l'application.
    /// </summary>
    /// <remarks>
    /// Ce raccourci n'est op�rationnel que si l'application a enregistr� 
    /// un archiveur <see cref="IArchiverManager"/> sous la cl� <see cref="MainKeys.KeyMainArchiverManager"/>
    /// </remarks>
    public static IArchiverManager MainArchiverManager {                      // <wao sp�cif.&body:rw>
      get { return (IArchiverManager) Get( MainKeys.KeyMainArchiverManager ) ; }
      set { Add( MainKeys.KeyMainArchiverManager, value ); }
    }

                                                                               // <wao show>
    /// <summary>
    /// Immersion de menus <see cref="MenuStrip"/> dans un menu principal.
    /// </summary>
    /// <param name="menu">menu � immerger dans le menu principal</param>
    public static void MergeInMainMenu( MenuStrip menu ) {                     // <wao sp�cif.&body>
      if (menu == null) throw new ERegistry( "MergeInMainMenu", "l'argument 'menu' est null" );

      MenuStrip host = (MenuStrip) GetIf( MainKeys.KeyMainMenu );
      if (host == null) throw new ERegistry( MainKeys.KeyMainMenu, "MergeInMainMenu", "le menu principal est introuvable dans le registre d'application" );

      MergeMenu( host, menu );
    }

    /// <summary>
    /// Immersion de barres d'outils <see cref="ToolStrip"/> dans un <see cref="ToolStripContainer"/>.
    /// </summary>
    /// <param name="tools">menu � immerger dans le menu principal</param>
    public static void MergeInMainTools( ToolStrip tools ) {                   // <wao sp�cif.&body>
      if (tools == null) throw new ERegistry( "MergeInMainTools", "l'argument 'tools' est null" );

      ToolStripPanel host = (ToolStripPanel) GetIf( MainKeys.KeyMainTools );
      if (host == null) throw new ERegistry( MainKeys.KeyMainTools, "MergeInMainTools", "le conteneur de barres d'outils est introuvable dans le registre d'application" );

      MergeTools( host, tools );
    }
  }                                                                            // <wao sp�cif.&ender>
} // namespace
