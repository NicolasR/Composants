/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 20 05 2010 : adjonction de l'archivage du type double
 * 15 11 2010 : am�lioration des diagnostics li�s � l'archivage des propri�t�s
 * 26 11 2010 : correction d'une bogue dans PopSection() : la section d�pil�e n'�tait pas restaur�e
 * 16 01 2011 : exception dans DoWriteProperty si la valeur de propri�t� � archiver est null
 */                                                                            // <wao never.end>
using System;                                                                  // <wao never>
using System.Collections ;                                                     // <wao never>
using System.Reflection ;                                                      // <wao never>
using System.ComponentModel;                                                   // <wao never>
using System.Windows.Forms;                                                    // <wao never>
using Psl.Tools;                                                               // <wao never>

namespace Psl.Applications {

  /// <summary>
  /// Composant d'archivage style "ini file"
  /// </summary>
  public class Archiver : IArchiver, IArchiverManager {

    #region Champs

    SortedList sections  = new SortedList() ; // dictionnaire tri� des sections
    SortedList section   = null             ; // dictionnaire tri� associ� � la section courante
    Stack      stack     = new Stack()      ; // pile des sections
    bool       isReading = true             ; // true si en mode lecture, false sinon

    #endregion

    /// <summary>
    /// Constructeur
    /// </summary>
    public Archiver() {
    }
	
    #region Service : s�rialisation / d�s�rialisation des archives

    /// <summary>
    /// D�termine si la ligne en cours est un d�limiteur de section 
    /// </summary>
    /// <param name="line"></param>
    /// <returns>true si la ligne en cours est un d�limiteur de section</returns>
    private bool IsSectionName( string line ) {
      if (line.Length < 2) return false ;
      return line[ 0 ] == '[' && line[ line.Length - 1 ] == ']' ;
    }

    /// <summary>
    /// Retourne le dictionnaire de la section nomm�e dans line, cr�e la section si n�cessaire
    /// </summary>
    /// <param name="sections">Dictionnaire des sections</param>
    /// <param name="line">Ligne contenant un d�limiteur de section</param>
    /// <returns>Le dictionnaire associ� � la section</returns>
    protected SortedList DoCreateSection( SortedList sections, string line ) {
      string name = line.Substring( 1, line.Length - 2 ) ;
      int index = sections.IndexOfKey( name ) ;
      if (index == -1) {
        SortedList result = new SortedList() ;
        sections.Add( name, result ) ;
        return result ;
      } else return (SortedList) sections.GetByIndex( index ) ;
    }

    /// <summary>
    /// Ajoute la ligne line � la section en forme cl�=valeur
    /// </summary>
    /// <param name="section">Dictionnaire de la section</param>
    /// <param name="line">Ligne de l'archive en forme "cl�=valeur"</param>
    protected void DoAddLineToSection( SortedList section, string line ) {
      if (section == null) return ;
      int pos = line.IndexOf( '=' ) ;
      if (pos == -1) {
        string trimmed = line.Trim() ;
        if (trimmed != "") section.Add( trimmed, null ) ;
      } else {
        string key = line.Substring( 0, pos ) ;
        string val = line.Substring( pos + 1 ) ;
        int index = section.IndexOfKey( key ) ;
        if (index != -1) section.RemoveAt( index ) ;
        section.Add( key, val ) ;
      }
    }

    /// <summary>
    /// Chargement d'une archive
    /// </summary>
    /// <param name="stream">Streamer de cha�ne ouvert sur le fichier d'archivage</param>
    protected void DoLoad(StringStreamer stream) {
      sections = new SortedList() ;
      section  = null ;
      string line = stream.ReadLine() ;
      while (line != null) {
        if (IsSectionName( line )) 
          section = DoCreateSection( sections, line ) ;
        else
          DoAddLineToSection( section, line ) ;
        line = stream.ReadLine() ;
      }
    }

    /// <summary>
    /// Enregistrement d'une section
    /// </summary>
    /// <param name="stream">Streamer de cha�nes ouvert sur le fichier d'archivage</param>
    /// <param name="sectionName">Nom de la section � enregistrer</param>
    /// <param name="values">Dictionnaire cl�/valeurs associ� � la section</param>
    protected void DoStoreSection( StringStreamer stream, string sectionName, SortedList values ) {
      if (values.Count == 0) return ;
      stream.WriteLine( "" ) ;
      stream.WriteLine( "[" + sectionName + "]" ) ;
      foreach (DictionaryEntry entry in values) {
        if (entry.Value == null)
          stream.WriteLine( (string) entry.Key ) ;
        else
          stream.WriteLine( entry.Key + "=" + entry.Value ) ;
      }
    }

    /// <summary>
    /// Enregistrement d'une archive
    /// </summary>
    /// <param name="stream">Streamer de cha�nes ouvert sur le fichier d'archivage</param>
    protected void DoStore( StringStreamer stream ) {
      foreach (DictionaryEntry entry in sections)
        DoStoreSection( stream, (string) entry.Key, (SortedList) entry.Value ) ;
    }

    #endregion

    #region Service : lecture et �criture des entr�es

    /// <summary>
    /// Contr�le qu'une section est en cours
    /// </summary>
    protected void DoCheckSection() {
      if (section == null) throw new EArchiver( "Pas de section en cours" ) ;
    }

    /// <summary>
    /// Contr�le la validit� d'une cl� d'archivage
    /// </summary>
    /// <param name="key">Cl� d'archivage � contr�ler</param>
    protected void DoCheckKey( string key ) {
      if (key    == null) throw new EArchiver( "Argument <key> null"    ) ;
      if (key    == ""  ) throw new EArchiver( "Argument <key> vide"    ) ;
    }

    /// <summary>
    /// Obtenir la valeur associ�e � une cl� dans la section courante
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    /// <returns>La valeur associ�e � cl�, null si introuvable</returns>
    protected string DoReadEntry( string key ) {
      DoCheckSection() ;
      DoCheckKey    ( key ) ;
      string result = null ;
      int index = section.IndexOfKey( key ) ;
      if (index != -1) result = (string) section.GetByIndex( index ) ;
      return result ;
    }

    /// <summary>
    /// Supprime l'entr�e associ�e � la cl� dans la section courante, sans effet si introuvable
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    protected void DoRemoveEntry( string key ) {
      DoCheckSection() ;
      DoCheckKey    ( key ) ;
      int index = section.IndexOfKey( key ) ;
      if (index != -1) section.RemoveAt( index ) ;
    }

    /// <summary>
    /// Construit un object exception EArchiver et formate le messages
    /// </summary>
    /// <param name="op">Op�ration au cours de laquelle l'exception est d�clench�e</param>
    /// <param name="key">Cl� d'archivage</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <param name="inner">Objet exception identifiant la cause de la difficult�</param>
    /// <returns>Un objet exception de type EArchiver pr�t � �tre lanc�</returns>
    private EArchiver NewEArchiver( string op, string key, object dft, Exception inner ) {
      string msg = ""
        + "Exception durant une op�ration d'archivage"                                      + "\r"
        + "Op�ration d'archivage : " + op                                                   + "\r"
        + "Cl� d'archivage : "       + (key == null ? "<null>" : key)                       + "\r"
        + "Valeur par defaut "       + (dft == null ? "<null>" : dft.ToString() )           + "\r"
        ;

      return new EArchiver( msg, inner ) ;
    }

    /// <summary>
    /// Construit un objectexception EArchiver et formate le messages
    /// </summary>
    /// <param name="op">Op�ration au cours de laquelle l'exception est d�clench�e</param>
    /// <param name="key">Cl� d'archivage</param>
    /// <param name="target">Instance cible portant la propri�t�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <param name="inner">Objet exception identifiant la cause de la difficult�</param>
    /// <returns>Un objet exception de type EArchiver pr�t � �tre lanc�</returns>
    private EArchiver NewEArchiver( string op, string key, object target, string pname, object dft, Exception inner ) {

      // �laboration du nom de l'objet (seulement si l'objet est du type Control)
      string targetName ;
      Control targetAsControl = target as Control;
      if (targetAsControl == null)
        targetName = "<non d�fini>";
      else 
        targetName = string.IsNullOrEmpty( targetAsControl.Name) ? "<cha�ne vide>" : targetAsControl.Name ;

      // �laboration du message
      string msg = ""
        + "Exception durant une op�ration d'archivage"                                          + "\r"
        + "Op�ration d'archivage: " + op                                                        + "\r"
        + "Cl� d'archivage : "      + ( key == null ? "<null>" : key )                          + "\r"
        + "Type de l'objet : "      + ( target == null ? "<null>" : target.GetType().FullName ) + "\r"
        + "Nom de l'objet : "       + targetName                                                + "\r"
        + "Propri�t� � archiver : " + (pname  == null ? "<null>" : pname)                       + "\r"
        + "Valeur par defaut "      + (dft    == null ? "<null>" : dft.ToString() )             + "\r"
        ;

      return new EArchiver( msg, inner ) ;
    }

    /// <summary>
    /// Obtient les information de membre d'une propri�t�
    /// </summary>
    /// <param name="target">Instance cible portant la propri�t�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="isReading">Si true, pour une lecture d'archive, sinon pour une �criture d'archive</param>
    /// <returns></returns>
    private PropertyInfo DoGetPropertyInfo( object target, string pname, bool isReading ) {
      if (target == null) throw new EArchiver( "Argument <source> null" ) ;
      if (pname  == null) throw new EArchiver( "Argument <pname> null"  ) ;
      if (pname  == ""  ) throw new EArchiver( "Argument <pname> vide"  ) ;

      Type         targetType = target.GetType() ;
      PropertyInfo propInfo   = targetType.GetProperty( pname, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public ) ;
      if ( propInfo == null ) throw new EArchiver( string.Format( "La propri�t� \"{0}\" est introuvable dans un objet de type \"{1}\"", pname, targetType.FullName ) );

      switch (isReading) {
        case true: if ( !propInfo.CanWrite ) throw new EArchiver( string.Format( "La propri�t� \"{0}\" n'est pas modifiable dans un objet de type \"{1}\"", pname, targetType.FullName ) ); break;
        case false: if ( !propInfo.CanRead ) throw new EArchiver( string.Format( "La propri�t� \"{0}\" n'est pas consultable dans un objet de type \"{1}\"", pname, targetType.FullName ) ); break;
      }

      return propInfo ;
    }

    /// <summary>
    /// Lit la valeur d'une propri�t� � partir de la section courante
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    /// <param name="target">Instance cible portant la propri�t�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    private void DoReadProperty( string key, object target, string pname, object dft ) {
      object       entry    = DoReadEntry      ( key ) ;
      object       value    = entry == null ? dft : entry ;
      PropertyInfo propInfo = DoGetPropertyInfo( target, pname, true ) ;
      Type         propType = propInfo.PropertyType ;

      if (value == null || value.GetType() == propType)
        propInfo.SetValue( target, value, null ) ;
      else
        propInfo.SetValue( target, TypeDescriptor.GetConverter(propType).ConvertFrom(value), null ) ;
    }

    /// <summary>
    /// Ecrit la valeur d'une propri�t� dans la section courante
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    /// <param name="target">Instance cible portant la propri�t�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    private void DoWriteProperty( string key, object target, string pname, object dft ) {
      DoRemoveEntry( key ) ;
      PropertyInfo propInfo  = DoGetPropertyInfo( target, pname, true ) ;
      Type         propType  = propInfo.PropertyType ;
      object       propValue = propInfo.GetValue( target, null ) ;

      if (propValue == null) throw new EArchiver( string.Format( "La propri�t� \"{0}\" de type \"{1}\" dans l'objet de type \"{2}\" a pour valeur null et ne peut �tre archiv�e", pname, propType.FullName, target.GetType().FullName ) ) ;

      if (dft.GetType() != propType || ! propValue.Equals( dft ) )
        section.Add( key, propValue.ToString() ) ;
    }

    #endregion

    #region Impl�mentation de l'interface IArchiverManager

    /// <summary>
    /// Nettoie toutes les donn�es d'archivage en m�moire
    /// </summary>
    public void Clear() {
      if ( section != null ) throw new EArchiver( "Une op�ration Clear n'est pas applicable s'il y a une section courante" );
      sections.Clear();
    }

    /// <summary>
    /// Contr�le le timbrage d'un fichier d'archivage
    /// </summary>
    /// <param name="fileName">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de r�f�rence</param>
    public void ArchiveCheck( string fileName, string stamp ) {
      try {
        StringStreamer streamer = new StringStreamer( fileName, stamp ) ;
        try { streamer.Open( true ) ; }
        finally { streamer.Close() ; }
      } 
      catch (Exception x) { throw new EArchiver( "Echec du contr�le du timbrage", x ) ; }
    }

    /// <summary>
    /// Charge une archive � partir d'un fichier d'archivage
    /// </summary>
    /// <param name="fileName">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de r�f�rence � contr�ler</param>
    public void ArchiveLoad ( string fileName, string stamp ) {
      StringStreamer streamer = new StringStreamer( fileName, stamp ) ;
      streamer.Open( true ) ;
      try { DoLoad( streamer ) ; }
      finally { streamer.Close() ; }
    }

    /// <summary>
    /// Enregistre une archive dans un fichier d'archivage
    /// </summary>
    /// <param name="fileName">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre de r�f�rence � apposer</param>
    public void ArchiveStore ( string fileName, string stamp ) {
      StringStreamer streamer = new StringStreamer( fileName, stamp ) ;
      streamer.Open( false ) ;
      try { DoStore( streamer ) ; }
      finally { streamer.Close() ; }
    }

    /// <summary>
    /// Diffuser un �v�nement de notification d'archivage
    /// </summary>
    /// <param name="reading">True si pour une lecture, sinon pour une �criture</param>
    public void ArchivePerform( bool reading ) {
      isReading = reading ;
      if (Archive != null) Archive( this ) ;
    }

    /// <summary>
    /// Proc�der � un archivage complet : diffusion d'un �v�nement d'archivage + load/store sur disque
    /// </summary>
    /// <param name="fileName">Nom complet du fichier d'archivage</param>
    /// <param name="stamp">Timbre � contr�ler ou � apposer</param>
    /// <param name="reading">True si archivage en lecture, sinon archivage en �criture</param>
    public void ArchiveFull( string fileName, string stamp, bool reading ) {
      if (reading) ArchiveLoad( fileName, stamp ) ;
      ArchivePerform( reading ) ;
      if (! reading) ArchiveStore( fileName, stamp ) ;
    }

    #endregion

    #region Impl�mentation de l'interface IArchiver

    /// <summary>
    /// Abonnement � l'�v�nement d'archivage
    /// </summary>
    public event ArchiverEventHandler Archive ;

    /// <summary>
    /// Empile une section
    /// </summary>
    /// <param name="sectionName">Nom de la section � empiler</param>
    public void PushSection( string sectionName ) {
      int index = sections.IndexOfKey( sectionName ) ;
      if (index == -1) {
        section = new SortedList() ;
        sections.Add( sectionName, section ) ;
      } else {
        section = (SortedList) sections.GetByIndex( index ) ;
        if (! isReading) section.Clear() ;
      }
      stack.Push( section ) ;
    }

    /// <summary>
    /// D�pile une section
    /// </summary>
    public void PopSection() {
      if (stack.Count == 0) throw new EArchiver( "La pile des sections est d�j� vide" ) ;
      stack.Pop();
      section = (SortedList) ( stack.Count == 0 ? null : stack.Peek() );
    }

    /// <summary>
    /// Sens de l'archivage lecture ou �criture
    /// </summary>
    public bool IsReading {
      get { return isReading ; }
      set { isReading = value ; }
    }
   
    /// <summary>
    /// Lire l'entr�e key de la section courante en tant que cha�ne
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par d�faut si introuvable</returns>
    public string ReadString( string key, string dft ) {
      try {
        string entry = DoReadEntry( key ) ;
        if (entry == null) return dft ;
        return entry ;
      }
      catch (Exception x) { throw NewEArchiver( "ReadString", key, dft, x ) ; } 
    }

    /// <summary>
    /// Ecrire l'entr�e key de la section courante en tant que cha�ne
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Valeur � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteString( string key, string data, string dft ) {
      try {
        DoRemoveEntry( key ) ;
        if (data != dft) section.Add( key, data ) ; 
      }
      catch (Exception x) { throw NewEArchiver( "WriteString", key, dft, x ) ; } 
    }

    /// <summary>
    /// Archiver l'entr�e key de la section courante en tant que string
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Variable � lire ou � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveString( string key, ref string data, string dft ) {
      if (isReading) data = ReadString( key, dft ) ; WriteString( key, data, dft ) ;
    }

    /// <summary>
    /// Indexeur pour la lecture/�criture d'une valeur de type string
    /// </summary>
    public string this[ string key, string dft ] {
      get { return ReadString( key, dft ) ; }
      set { WriteString( key, value, dft ) ; }
    }

    /// <summary>
    /// Lire l'entr�e key de la section courante en tant que tableau de cha�nes de caract�res
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    /// <returns>le tableau de cha�ne lu dans l'archive</returns>
    public string[] ReadStrings( string key ) {
      int      count  = ReadInt( key + "._Count", 0 ) ;
      string[] result = new string[ count ] ;

      for (int ix = 0 ; ix < count ; ix++ ) 
        result[ ix ] = ReadString( key + "." + ix, "" ) ;

      return result ;
    }

    /// <summary>
    /// Ecrire l'entr�e key de la section courante en tant que tableau de cha�nes de caract�res
    /// </summary>
    /// <param name="key">Cl� d'archivage</param>
    /// <param name="values">la tableau de cha�nes � enregistrer</param>
    public void WriteStrings( string key, string[] values ) {
      WriteInt( key + "._Count", values.Length, 0 ) ;
      for (int ix = 0 ; ix < values.Length ; ix++ ) 
        WriteString( key + "." + ix, values[ ix ] == null ? "" : values[ ix ], "" ) ;      
    }

    /// <summary>
    /// Lire l'entr�e key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par d�faut si introuvable</returns>
    public int ReadInt( string key, int dft ) {
      try {
        string entry = DoReadEntry( key ) ;
        return entry == null ? dft : System.Int32.Parse( entry ) ;
      }
      catch (Exception x) { throw NewEArchiver( "ReadInt", key, dft, x ) ; } 
    }

    /// <summary>
    /// Ecrire l'entr�e key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Valeur � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteInt( string key, int data, int dft ) {
      try {
        DoRemoveEntry( key ) ;
        if (data != dft) section.Add( key, data.ToString() ) ; 
      }
      catch (Exception x) { throw NewEArchiver( "WriteInt", key, dft, x ) ; } 
    }

    /// <summary>
    /// Archiver l'entr�e key de la section courante en tant que int
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Variable � lire ou � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveInt( string key, ref int data, int dft ) {
      if (isReading) data = ReadInt( key, dft ) ; WriteInt( key, data, dft ) ;
    }

    /// <summary>
    /// Indexeur pour la lecture/�criture d'une valeur de type int
    /// </summary>
    public int this[ string key, int dft ] {
      get { return ReadInt( key, dft ) ; }
      set { WriteInt( key, value, dft ) ; }
    }

    /// <summary>
    /// Lire l'entr�e key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par d�faut si introuvable</returns>
    public bool ReadBool( string key, bool dft ) {
      try {
        string entry = DoReadEntry( key ) ;
        return entry == null ? dft : System.Boolean.Parse( entry ) ; 
      }
      catch (Exception x) { throw NewEArchiver( "ReadBool", key, dft, x ) ; } 
    }

    /// <summary>
    /// Ecrire l'entr�e key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Valeur � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteBool( string key, bool data, bool dft ) {
      try {
        DoRemoveEntry( key ) ;
        if (data != dft) section.Add( key, data.ToString() ) ; 
      }
      catch (Exception x) { throw NewEArchiver( "WriteBool", key, dft, x ) ; } 
    }

    /// <summary>
    /// Archiver l'entr�e key de la section courante en tant que bool
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Variable � lire ou � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveBool( string key, ref bool data, bool dft ) {
      if (isReading) data = ReadBool( key, dft ) ; WriteBool( key, data, dft ) ;
    }

    /// <summary>
    /// Indexeur pour la lecture/�criture d'une valeur de type bool
    /// </summary>
    public bool this[ string key, bool dft ] { 
      get { return ReadBool( key, dft ) ; }
      set { WriteBool( key, value, dft ) ; }
    }

    /// <summary>
    /// Lire l'entr�e key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="dft">Valeur par d�faut</param>
    /// <returns>La valeur figurant dans l'archive, ou la valeur par d�faut si introuvable</returns>
    public double ReadDouble( string key, double dft ) {
      try {
        string entry = DoReadEntry( key ) ;
        return entry == null ? dft : System.Double.Parse( entry ) ;
      }
      catch (Exception x) { throw NewEArchiver( "ReadDouble", key, dft, x ) ; } 
    }

    /// <summary>
    /// Ecrire l'entr�e key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Valeur � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteDouble( string key, double data, double dft ) {
      try {
        DoRemoveEntry( key ) ;
        if (data != dft) section.Add( key, data.ToString() ) ; 
      }
      catch (Exception x) { throw NewEArchiver( "WriteDouble", key, dft, x ) ; } 
    }

    /// <summary>
    /// Archiver l'entr�e key de la section courante en tant que double
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="data">Variable � lire ou � enregistrer</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveDouble( string key, ref double data, double dft ) {
      if (isReading) data = ReadDouble( key, dft ) ; WriteDouble( key, data, dft ) ;
    }

    /// <summary>
    /// Indexeur pour la lecture/�criture d'une valeur de type double
    /// </summary>
    public double this[ string key, double dft ] {
      get { return ReadDouble( key, dft ); }
      set { WriteDouble( key, value, dft ); }
    }

    /// <summary>
    /// Lecture de la valeur d'une propri�t�.
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="target">Object concern�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ReadProperty( string key, object target, string pname, object dft ) {
      try                 { DoReadProperty( key, target, pname, dft ) ; } 
      catch (Exception x) { throw NewEArchiver( "ReadProperty", key, target, pname, dft, x ) ; }
    }

    /// <summary>
    /// Ecriture de la valeur d'une propri�t�.
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="target">Object concern�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteProperty( string key, object target, string pname, object dft ) {
      try                 { DoWriteProperty( key, target, pname, dft ) ; } 
      catch (Exception x) { throw NewEArchiver( "WriteProperty", key, target, pname, dft, x ) ; }
    }

    /// <summary>
    /// Archivage de la valeur d'une propri�t�.
    /// </summary>
    /// <param name="key">Cl� d'archivage de la valeur</param>
    /// <param name="target">Object concern�</param>
    /// <param name="pname">Nom de la propri�t�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveProperty( string key, object target, string pname, object dft ) {
      if (isReading) ReadProperty( key, target, pname, dft) ; else WriteProperty( key, target, pname, dft ) ;
    }

    /// <summary>
    /// Lecture de la valeur d'une propri�t�.
    /// </summary>
    /// <param name="key">Cl� d'archivage et nom de la propri�t�</param>
    /// <param name="target">Objet concern�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ReadProperty( string key, object target, object dft ) {
      ReadProperty( key, target, key, dft ) ;
    }

    /// <summary>
    /// Ecriture de la valeur d'une propri�t�. 
    /// </summary>
    /// <param name="key">Cl� d'archivage et nom de la propri�t�</param>
    /// <param name="target">Object concern�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void WriteProperty( string key, object target, object dft ) {
      WriteProperty( key, target, key, dft ) ;
    }

    /// <summary>
    /// Archivage de la valeur d'une propri�t�.
    /// </summary>
    /// <param name="key">Cl� d'archivage et nom de la propri�t�</param>
    /// <param name="target">Object concern�</param>
    /// <param name="dft">Valeur par d�faut</param>
    public void ArchiveProperty( string key, object target, object dft ) {
      ArchiveProperty( key, target, key, dft ) ;
    }
  }

  #endregion
}