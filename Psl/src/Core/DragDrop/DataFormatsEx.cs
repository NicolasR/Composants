/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 09 05 2010 : version initiale
 */                                                                            // <wao never.end>
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Psl.DragDrop {

  /// <summary>
  /// Extension de <see cref="DataFormats"/> exposant des noms de formats usuels
  /// </summary>
  public class DataFormatsEx : DataFormats {

    // Pour contourner le fait que le constructeur de DataFormats est private
    private DataFormatsEx() : this( 0 ) { }
    private DataFormatsEx( int x ) : this() { }

    /// <summary>
    /// Format de données pour le drag-drop de chaînes unicode 
    /// </summary>
    /// <remarks>
    /// Le format de données est une chaîne du type System.String
    /// </remarks>
    public static readonly string String = "UnicodeText";

    /// <summary>
    /// Format de données Psl pour le drag-drop d'un tableau de chaînes
    /// </summary>
    /// <remarks>
    /// Le format de données est un tableau de chaînes unicode System.String
    /// </remarks>
    public static readonly string StringArray = "PslStringArray";

    /// <summary>
    /// Format de données Windows Explorer pour le drag-drop de noms de fichiers
    /// </summary>
    /// <remarks>
    /// Le format de données est un nom de fichier en unicode
    /// </remarks>
    public static readonly string FileName = "FileNameW";

    /// <summary>
    /// Format de données Windows Explorer pour le drag-drop de noms de fichiers
    /// </summary>
    /// <remarks>
    /// Le format de données est une tableau de noms de fichiers unicode
    /// </remarks>
    public static readonly string FileNameArray = "FileDrop";

    /// <summary>
    /// Format de données Internet Explorer pour le drag-drop des urls depuis la boîte combo
    /// </summary>
    /// <remarks>
    /// Le format de données est un flux <see cref="System.IO.MemoryStream"/> contenant l'url en unicode
    /// </remarks>
    public static readonly string UrlW = "UniformResourceLocatorW";

    /// <summary>
    /// Format de données Psl pour le drag-drop d'une url
    /// </summary>
    /// <remarks>
    /// Le format des données est une chaîne unicode System.String
    /// </remarks>
    public static readonly string Url = "PslUrl";

    /// <summary>
    /// Format de données Psl pour un tableau d'url
    /// </summary>
    /// <remarks>
    /// Le format des données est un tableau de chaînes unicode
    /// </remarks>
    public static readonly string UrlArray = "PslUrlArray";

  }
}
