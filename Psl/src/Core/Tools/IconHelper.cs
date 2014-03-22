/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 12 12 2008 : version initiale
 * 30 04 2009 : amélioration de l'heuristique relative à la conversion des icônes
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Psl.Windows;

namespace Psl.Tools {

  /// <summary>
  /// Classe de service proposant quelques fonctionnalités liées aux icônes fichiers ou web.
  /// </summary>
  public class IconHelper {                                                    // <wao headers.&header>

    //
    // Icônes relatives au système de fichiers                                 // <wao headers.&comgroup>
    //

    /// <summary>
    /// Obtient l'icône associée à un fichier ou à un répertoire.
    /// </summary>
    /// <param name="path">nom absolu du fichier ou du réperoire</param>
    /// <param name="imageLarge">true pour obenir la grande image, false sinon</param>
    /// <param name="isFolder">true s'il s'agit d'un répertoire, false sinon</param>
    /// <param name="linkOverlay">true pour superposer l'indication de raccourci, false sinon</param>
    /// <param name="folderOpened">dans le cas d'un répertoire, true pour obtenir l'image du répertoire ouvert</param>
    /// <returns>l'icône <see cref="Icon"/> obtenue, ou null si le fichier ou le répertoire est introuvable</returns>
    public static Icon GetItemIcon( string path, bool imageLarge, bool isFolder, bool linkOverlay, bool folderOpened ) {
      IntPtr handle = Shell32.GetItemIconHandle( path, imageLarge, linkOverlay, isFolder, folderOpened );
      return handle == IntPtr.Zero ? null : Icon.FromHandle( handle );
    }

    /// <summary>
    /// Obtient l'icône associée à un fichier.
    /// </summary>
    /// <param name="path">nom absolu du fichier</param>
    /// <param name="imageLarge">true pour obtenir la grande image, false sinon</param>
    /// <returns>l'icône <see cref="Icon"/> obtenue, ou null si le fichier est introuvable</returns>
    public static Icon GetFileIcon( string path, bool imageLarge ) {           // <wao headers.&body>
      return GetItemIcon( path, imageLarge, false, false, false );
    }

    /// <summary>
    /// Obtient l'icône associée à un répertoire.
    /// </summary>
    /// <param name="path">nom absolu du répertoire</param>
    /// <param name="imageLarge">true pour obtenir la grande image, false sinon</param>
    /// <param name="asFolderOpened">true pour obtenir l'image du répertoire ouvert, false sinon</param>
    /// <returns>l'icône <see cref="Icon"/> obtenue, ou null si le répertoire est introuvable</returns>
    public static Icon GetFolderIcon( string path, bool imageLarge, bool asFolderOpened ) { // <wao headers.&body>
      return GetItemIcon( path, imageLarge, true, false, asFolderOpened );
    }

    //
    // Icônes relatives aux pages et aux sites web                             // <wao headers.&comgroup>
    //

    /// <summary>
    /// Télécharge de manière bloquante une ressource déterminée par une url
    /// </summary>
    /// <param name="url">url de la ressource à télécharger</param>
    /// <returns>les données téléchargées sous la forme d'un tableau de bytes</returns>
    public static byte[] DownloadData( string url ) {                          // <wao headers.&body>
      using ( WebClient client = new WebClient() ) {
        return client.DownloadData( url );
      }
    }

    /// <summary>
    /// Convertit les données brutes d'une image en un objet <see cref="Image"/>.
    /// </summary>
    /// <param name="data">données brutes de l'image</param>
    /// <returns>l'objet image obtenu à partir des données brutes</returns>
    public static Image DataToImage( byte[] data ) {                           // <wao headers.&body>
      using ( MemoryStream stream = new MemoryStream( data ) ) {
        if ( stream.Length == 0 ) return null;
        return Image.FromStream( stream, true, true );
      }
    }

    /// <summary>
    /// Convertit les données brutes d'une image en un objet <see cref="Icon"/>
    /// </summary>
    /// <param name="data">données brutes de l'image</param>
    /// <returns>l'objet icône obtenu à partir des données brutes</returns>
    public static Icon DataToIcon( byte[] data ) {                             // <wao headers.&body>
      using ( MemoryStream stream = new MemoryStream( data ) ) {

        // valider le flux stream
        if ( stream.Length == 0 ) return null;

        // résultat par défaut
        Icon result = null;

        // commencer par supposer qu'il bien d'une image au format des icônes
        try {
          result = new Icon( stream );
          return result;
        }
        catch { } // absorber l'exception pour tenter une autre conversion

        // tenter la conversion en passant par le format bitmap
        Bitmap image = new Bitmap( stream, false );
        if ( image.RawFormat.Guid == ImageFormat.Icon.Guid ) {
          stream.Position = 0;
          result = new Icon( stream );
        }
        else {
          image.MakeTransparent();
          IntPtr hicon = image.GetHicon();
          result = Icon.FromHandle( hicon );
          image.Dispose();
        }
        return result;
      }
    }

    /// <summary>
    /// Télécharge une image déterminée par son url.
    /// </summary>
    /// <param name="url">url de l'image à télécharger</param>
    /// <param name="tryOnly">si false, laisse sortir les exceptions, si true retourne null en cas d'exception</param>
    /// <returns>l'image téléchargée</returns>
    public static Image DownloadImage( string url, bool tryOnly ) {            // <wao headers.&body>
      try {
        return DataToImage( DownloadData( url ) );
      }
      catch {
        if ( tryOnly )
          return null;
        else
          throw;
      }
    }

    /// <summary>
    /// Télécharge une image ou une icône et la retourne au format <see cref="Icon"/>.
    /// </summary>
    /// <param name="url">url de l'image ou de l'icône à télécharger</param>
    /// <param name="tryOnly">si false, laisse sortir les exceptions, si true retourne null en cas d'exception</param>
    /// <returns>l'objet icône obtenu</returns>
    public static Icon DownloadIcon( string url, bool tryOnly ) {              // <wao headers.&body>
      try {
        return DataToIcon( DownloadData( url ) );
      }
      catch {
        if ( tryOnly )
          return null;
        else
          throw;
      }
    }
  }                                                                            // <wao headers.&ender>
}
