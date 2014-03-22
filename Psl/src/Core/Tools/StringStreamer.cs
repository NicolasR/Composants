/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 12 2000 : version initiale j++
 * 14 12 2001 : adaptation au contexte 2001-2002
 * 26 11 2003 : migration en vjs
 * 23 04 2005 : portage en cs et extraction du streamer de chaînes
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 22 03 2011 : le contrôle du timbre est ignoré si le timbre à contrôler est null ou vide
 */                                                                            // <wao never.end>
using System;
using System.IO ;

namespace Psl.Tools {

  /// <summary>
  /// Exceptions du streamer
  /// </summary>
  public class EStreamer : Exception {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="msg">message associé à l'exception</param>
    public EStreamer( string msg ) : base( msg ) {}

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="msg">message associé à l'exception</param>
    /// <param name="inner">object exception ayant causé l'exception</param>
    public EStreamer( string msg, Exception inner ) : base( msg, inner ) { }
  }

  /// <summary>
  /// Streamer de chaînes de caractères.
  /// </summary>
  /// <remarks>
  /// Ce streamer écrit/lit séquentiellement des données brutes (en binaire) dans un 
  /// fichier. Lors de l'ouverture, un timbrage est automatiquement contrôlé
  /// en lecture et enregistré en écriture.
  /// <br/>
  /// Il est très fortement recommandé de fermer le streamer dès que l'archive
  /// a été lue ou écrite. Les noms de fichiers sont supposés valides. Toutefois,
  /// en lecture, le streamer est conçu pour fonctionner même si le fichier à
  /// lire n'existe pas, en vue de faciliter la création de nouvelles archives.
  /// <br/>
  /// Le streamer est conçu pour la réalisation de procédures d'archivage
  /// bi-directionnelles, le sens (lecture/écriture) étant spécifié lors de
  /// l'ouverture. 
  /// </remarks>
  public class StringStreamer {
	
    /// <summary>
    /// Nom du fichier pour l'archivage en cours
    /// </summary>
    string  fileName  = ""    ;
  
    /// <summary>
    /// Timbrage d'authentification pour l'archivage en cours
    /// </summary>
    string  fileStamp = ""    ;
  
    /// <summary>
    /// L'archiveur est à considérer comme effectivement ouvert
    /// </summary>
    bool isOpened  = false ;
  
    /// <summary>
    /// Sens lecture/écriture de l'archivage en cours
    /// </summary>
    bool isReading = true  ;
  
    /// <summary>
    /// Accès au streamer de lecture
    /// </summary>
    TextReader reader = null  ;
	
    /// <summary>
    /// Accès au streamer d'écriture
    /// </summary>
    TextWriter writer = null  ;
	
    /// <summary>
    /// Initialisation d'un archiveur.
    /// </summary>
    /// <param name="fname">nom complet valide du fichier d'archivage</param>
    /// <param name="stamp">chaîne de timbrage pour l'authentification</param>
    public StringStreamer( string fname, string stamp ) {
      fileName  = fname   ;
      fileStamp = stamp   ;
      isReading = true    ;
      isOpened  = false   ;
      reader    = null    ;
      writer    = null    ;
    }
	
    /// <summary>
    /// Ouvrir un fichier d'archive (interface d'utilisation).
    /// </summary>
    /// <param name="reading">true, ouvrir en lecture, false, ouvrir en écriture</param>
    /// <exception cref="EStreamer">si l'archive est inacessible ou si le timbre n'est pas valide</exception>
    public void Open ( bool reading ) { 
      DoOpen ( reading ) ; 
    }
  
    /// <summary>
    /// Fermer le fichier d'archive courant (interface d'utilisation).
    /// </summary>
    public void Close() { 
      DoClose() ; 
    }
	
    /// <summary>
    /// Effectuer l'ouverture d'une archive (service).
    /// </summary>
    /// <param name="reading">true, ouvrir en lecture, false, ouvrir en écriture</param>
    /// <exception cref="EStreamer">si l'archive est inacessible ou si le timbre n'est pas valide</exception>
    protected void DoOpen( bool reading ) {
      isReading = reading ;
      if (isReading) {
        if (File.Exists( fileName )) {
          reader = File.OpenText( fileName ) ;
          isOpened = true ;
        } 
      } else {
        writer = File.CreateText( fileName ) ;
        isOpened = true ;
      }
      DoCheckStamp() ;
    }

    /// <summary>
    /// Effectuer la fermeture d'une archive (service).
    /// </summary>
    protected void DoClose() {
      if (reader != null) reader.Close() ;
      if (writer != null) writer.Close() ;
      isOpened = false ;
      reader = null ;
      writer = null ;
    }

    /// <summary>
    /// Effectuer le contrôle de validité du timbre d'un fichier (service).	
    /// </summary>
    /// <exception cref="EStreamer">si l'archive est inacessible ou si le timbre n'est pas valide</exception>
    public void DoCheckStamp() {

      // ignorer si le streamer n'est pas ouvert ou si le timbre à contrôler n'est pas significatif
      if ( !isOpened || string.IsNullOrEmpty( fileStamp ) ) return;

      // effectuer le contrôle du timbre
      if ( isReading ) {
        try {
          string stamp = reader.ReadLine();
          if ( fileStamp != stamp ) {
            DoClose();
            throw new EStreamer( "Timbre non valide :\r\n\"" + stamp + "\" (trouvé)\r\n\"" + fileStamp + "\" (attendu)" );
          }
        }
        catch ( Exception e ) {
          throw new EStreamer( "Le timbre n'est pas identifiable (ce n'est pas un fichier d'archive ou l'archive est obsolète, illisible ou abîmée)", e );
        }
      }
      else writer.WriteLine( fileStamp );
    }

	  /// <summary>
	  /// Lecture d'une ligne sur le streamer
	  /// </summary>
	  /// <returns>la ligne lue</returns>
    public string ReadLine() {
      if ( !isOpened || reader == null) return null ;
      return reader.ReadLine() ;
    }

    /// <summary>
    /// Ecriture d'une ligne sur le streamer
    /// </summary>
    /// <param name="line">la ligne à écrire</param>
    public void WriteLine( string line ) {
      if ( !isOpened || writer == null) return ;
      writer.WriteLine( line ) ;
    }

	  /// <summary>
    /// Lecture d'un tableau de chaînes (service).
	  /// </summary>
    /// <remarks>
    /// Dans l'archive, le tableau des chaînes doit être précédé du nombre de
    /// chaînes à relire.
    /// </remarks>
    /// <returns>le tableau des chaînes lues</returns>
    public string[] GetStrings() {
      if (isOpened) {
        int      count = Int32.Parse( reader.ReadLine() ) ;
        string[] lines = new string[ count ] ;
        for (int k = 0 ; k < count ; k++) 
          lines[ k ] = reader.ReadLine() ;
        return lines ;
      } else return new string[0] ;
    }
	
    /// <summary>
    /// Ecriture d'un tableau de chaînes (service).
    /// </summary>
    /// <remarks>
    /// L'enregistrement du tableau des chaînes est précédé de 
    /// l'enregistrement du nombre de chaînes enregistrées. 
    /// </remarks>
    /// <param name="strings">strings un tableau de chaînes à enregistrer</param>
    public void PutStrings( object[] strings ) {
      if (isOpened) {
        writer.WriteLine( strings.Length ) ;
        for (int k = 0 ; k < strings.Length ; k++) 
          writer.WriteLine( (string) strings[ k ] ) ;
      }
    }			
  }
}

