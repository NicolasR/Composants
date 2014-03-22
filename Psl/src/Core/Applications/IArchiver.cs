/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 20 05 2010 : adjonction de l'archivage du type double
 */                                                                            // <wao never.end>
using System;                                                                  // <wao never>
using Psl.Tools;                                                               // <wao never>

namespace Psl.Applications {                                                   

  /////////////////////////////////////////////////////////////////////////////
  //                                                                         //
  //            EArchiver : classe d'exceptions pour l'archivage             //
  //                                                                         //
  /////////////////////////////////////////////////////////////////////////////
                                                                               // <wao EArchiver.begin>
  /// <summary>
  /// Classe d'exception pour l'archiveur
  /// </summary>
  [Serializable]                                                               // <wao never>
  public class EArchiver : Exception {                                         // <wao strip.begin>
  
    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="msg">Message associé à l'exception</param>
    public EArchiver( string msg ) : base( msg ) {}

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="msg">Message associé à l'exception</param>
    /// <param name="inner">Exception originale</param>
    public EArchiver( string msg, Exception inner ) : base( msg, inner ) {}
  }                                                                            // <wao EArchiver.end strip.end>

  /////////////////////////////////////////////////////////////////////////////
  //                                                                         //
  //  ArchiverEventHandler : type de délégué pour les événements d'archivage //
  //                                                                         //
  /////////////////////////////////////////////////////////////////////////////
                                                                               // <wao ArchiverEventHandler.begin>
  /// <summary>
  /// Type de gestionnaire d'événement pour l'archivage
  /// </summary>
  public delegate void ArchiverEventHandler( IArchiver sender ) ;              // <wao ArchiverEventHandler.end>

  /////////////////////////////////////////////////////////////////////////////
  //                                                                         //
  //            IArchiver : interface d'utilisation de l'archiveur           //
  //                                                                         //
  /////////////////////////////////////////////////////////////////////////////
                                                                               // <wao IArchiver.begin>
  /// <summary>
  /// Interface d'utilisation d'un archiver
  /// </summary>
  public interface IArchiver {                                                 // <wao strip.begin>

    //
    // Evénements                                                              // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Abonnement à l'événement d'archivage
    /// </summary>
    event ArchiverEventHandler Archive ;

    //
    // Gestion générale                                                        // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Sens de l'archivage lecture ou écriture
    /// </summary>
    bool IsReading { get ; set ; }

    /// <summary>
    /// Empile une section
    /// </summary>
    /// <param name="sname">Nom de la section à empiler</param>
    void PushSection( string sname ) ;

    /// <summary>
    /// Dépile une section
    /// </summary>
    void PopSection() ;

    //
    // Indexeurs                                                               // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Indexeur pour la lecture/écriture d'une valeur de type string
    /// </summary>
    string this[ string key, string dft ] { get ; set ; }

    /// <summary>
    /// Indexeur pour la lecture/écriture d'une valeur de type int
    /// </summary>
    int this[ string key, int dft ] { get ; set ; }

    /// <summary>
    /// Indexeur pour la lecture/écriture d'une valeur de type bool
    /// </summary>
    bool this[ string key, bool dft ] { get ; set ; }

    /// <summary>
    /// Indexeur pour la lecture/écriture d'une valeur de type double
    /// </summary>
    double this[ string key, double dft ] { get ; set ; }

    //
    // Type string                                                             // <wao IArchiver.&comitem>
    //
    
    /// <summary>
    /// Lire l'entrée key de la section courante en tant que chaîne
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="dft">Valeur par défaut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par défaut si introuvable</returns>
    string ReadString( string key, string dft ) ;

    /// <summary>
    /// Ecrire l'entrée key de la section courante en tant que chaîne
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Valeur à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteString( string key, string data, string dft ) ;

    /// <summary>
    /// Archiver l'entrée key de la section courante en tant que string
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Variable à lire ou à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveString( string key, ref string data, string dft ) ;

    /// <summary>
    /// Lire l'entrée key de la section courante en tant que tableau de chaînes
    /// </summary>
    /// <param name="key">Clé d'archivage</param>
    /// <returns>le tableau de chaîne lu dans l'archive</returns>
    string[] ReadStrings( string key ) ;

    /// <summary>
    /// Ecrire l'entrée key de la section courante en tant que tableau de chaînes
    /// </summary>
    /// <param name="key">Clé d'archivage</param>
    /// <param name="values">le tableau de chaînes à enregistrer</param>
    void WriteStrings( string key, string[] values ) ;

    //
    // Type int                                                                // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Lire l'entrée key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="dft">Valeur par défaut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par défaut si introuvable</returns>
    int ReadInt( string key, int dft ) ;

    /// <summary>
    /// Ecrire l'entrée key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Valeur à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteInt( string key, int data, int dft ) ;

    /// <summary>
    /// Archiver l'entrée key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Variable à lire ou à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveInt( string key, ref int data, int dft ) ;

    //
    // Type bool                                                               // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Lire l'entrée key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="dft">Valeur par défaut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par défaut si introuvable</returns>
    bool ReadBool( string key, bool dft ) ;

    /// <summary>
    /// Ecrire l'entrée key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Valeur à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteBool( string key, bool data, bool dft ) ;

    /// <summary>
    /// Archiver l'entrée key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Variable à lire ou à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveBool( string key, ref bool data, bool dft ) ;

    //
    // type double                                                             // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Lire l'entrée key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="dft">Valeur par défaut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par défaut si introuvable</returns>
    double ReadDouble( string key, double dft ) ;

    /// <summary>
    /// Ecrire l'entrée key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Valeur à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteDouble( string key, double data, double dft ) ;

    /// <summary>
    /// Archiver l'entrée key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="data">Variable à lire ou à enregistrer</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveDouble( string key, ref double data, double dft ) ;

    //
    // Archivage des propriétés                                                // <wao IArchiver.&comitem>
    //

    /// <summary>
    /// Lecture de la valeur d'une propriété.
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="target">Object concerné</param>
    /// <param name="pname">Nom de la propriété</param>
    /// <param name="dft">Valeur par défaut</param>
    void ReadProperty( string key, object target, string pname, object dft ) ;

    /// <summary>
    /// Ecriture de la valeur d'une propriété.
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="target">Object concerné</param>
    /// <param name="pname">Nom de la propriété</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteProperty( string key, object target, string pname, object dft ) ;

    /// <summary>
    /// Archivage de la valeur d'une propriété.
    /// </summary>
    /// <param name="key">Clé d'archivage de la valeur</param>
    /// <param name="target">Object concerné</param>
    /// <param name="pname">Nom de la propriété</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveProperty( string key, object target, string pname, object dft ) ;

    /// <summary>
    /// Lecture de la valeur d'une propriété.
    /// </summary>
    /// <param name="key">Clé d'archivage et nom de la propriété</param>
    /// <param name="target">Objet concerné</param>
    /// <param name="dft">Valeur par défaut</param>
    void ReadProperty( string key, object target, object dft ) ;

    /// <summary>
    /// Ecriture de la valeur d'une propriété. 
    /// </summary>
    /// <param name="key">Clé d'archivage et nom de la propriété</param>
    /// <param name="target">Object concerné</param>
    /// <param name="dft">Valeur par défaut</param>
    void WriteProperty( string key, object target, object dft ) ;

    /// <summary>
    /// Archivage de la valeur d'une propriété.
    /// </summary>
    /// <param name="key">Clé d'archivage et nom de la propriété</param>
    /// <param name="target">Object concerné</param>
    /// <param name="dft">Valeur par défaut</param>
    void ArchiveProperty( string key, object target, object dft ) ;
  }                                                                            // <wao IArchiver.end strip.end>

  /////////////////////////////////////////////////////////////////////////////
  //                                                                         //
  //         IArchiverManager : interface de ilotage de l'archiveur          //
  //                                                                         //
  /////////////////////////////////////////////////////////////////////////////
                                                                               // <wao IArchiverManager.begin>
  /// <summary>
  /// Interface de pilotage d'un archiveur.
  /// </summary>
  public interface IArchiverManager {                                          // <wao strip.begin>

    /// <summary>
    /// Contrôle le timbrage d'un fichier d'archivage.
    /// </summary>
    /// <param name="fname">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de référence</param>
    void ArchiveCheck( string fname, string stamp ) ;

    /// <summary>
    /// Charge une archive à partir d'un fichier d'archivage.
    /// </summary>
    /// <param name="fname">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de référence à contrôler</param>
    void ArchiveLoad ( string fname, string stamp ) ;

    /// <summary>
    /// Enregistre une archive dans un fichier d'archivage.
    /// </summary>
    /// <param name="fname">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de référence à apposer</param>
    void ArchiveStore ( string fname, string stamp ) ;

    /// <summary>
    /// Diffuse un événement de notification d'archivage.
    /// </summary>
    /// <param name="reading">True si pour une lecture, sinon pour une écriture</param>
    void ArchivePerform( bool reading ) ;

    /// <summary>
    /// Cycle d'archivage complet (événement Archive + load ou store)
    /// </summary>
    /// <param name="fname">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre à contrôler ou à apposer</param>
    /// <param name="reading">True si archivage en lecture, sinon archivage en écriture</param>
    void ArchiveFull( string fname, string stamp, bool reading ) ;
                                                                               // <wao never.begin>
    /// <summary>
    /// Nettoie toutes les données d'archivage en mémoire
    /// </summary>
    void Clear();                                                              // <wao never.end>
  }                                                                            // <wao IArchiverManager.end strip.end>
}                                                                              
