/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 05 2007 : am�lioration du libell� de certains diagnostics
 */                                                                            // <wao never.end>
using System;
using System.Collections;

namespace Psl.Applications {

  /// <summary>
  /// Classe des exceptions produites par le registre d'application.
  /// </summary>
  public class ERegistry : Exception {
  
    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="op">libell� de l'op�ration d�clenchant l'exception</param>
    /// <param name="msg">diagnostic sp�cifique de l'exception</param>
    /// <param name="inner">object exception ayant caus� l'exception</param>
    public ERegistry( string op, string msg, Exception inner ) :
      base( ""
      + "Exception au niveau du registre interne de l'application\r\n"
      + "op�ration : " + '"' + op + '"' + "\r\n"
      + "diagnostic : " + '"' + msg + '"',
      inner) { }

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="op">libell� de l'op�ration d�clenchant l'exception</param>
    /// <param name="msg">diagnostic sp�cifique de l'exception</param>
    public ERegistry( string op, string msg ) : this( op, msg, (Exception) null ) {}

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="key">cl� de registre impliqu�e par l'exception</param>
    /// <param name="op">libell� de l'op�ration d�clenchant l'exception</param>
    /// <param name="msg">diagnostic sp�cifique de l'exception</param>
    /// <param name="inner">objet exception cause de l'exception</param>
    public ERegistry( string key, string op, string msg, Exception inner ) :
      base( ""
      + "Exception au niveau du registre interne de l'application\r\n"
      + "op�ration : " + '"' + op + '"' + "\r\n"
      + "cl� : " + '"' + key + '"' + "\r\n"
      + "diagnostic : " + '"' + msg + '"',
      inner ) {}

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="key">cl� de registre impliqu�e par l'exception</param>
    /// <param name="op">libell� de l'op�ration d�clenchant l'exception</param>
    /// <param name="msg">diagnostic sp�cifique de l'exception</param>
    public ERegistry( string key, string op, string msg ) : this( key, op, msg, null ) {}
  }

  /// <summary>
  /// Base du registre interne d'application.
  /// </summary>
  /// <remarks>
  /// Cette classe ne comporte que des membres de classe et ne doit donc jamais
  /// �tre instanci�e. 
  /// <br/>
  /// Le registre interne d'application n'est rien d'autre qu'une table
  /// d'associations cl�-valeur accompagn�e de quelques raccourcis usuels.
  /// Les cl�s sont [en principe] des cha�nes de caract�res, les valeurs des
  /// instances de Object. 
  /// <br/>
  /// Le registre d'application est destin� faire en sorte que les diff�rents 
  /// plugins d'une application soient hautement ind�pendants du point de vue
  /// syntaxique et surtout du point de vue des impl�mentations. 
  /// <br/>
  /// Tout objet (composant visuel, instance de classe, serveur d'interface, etc.)
  /// qui est fourni par un plugin de l'application et utilis� par un autre
  /// doit �tre enregistr� dans le registre par le plugin fournisseur. Les plugins
  /// utilisant un tel objet doivent obtenir sa r�f�rence en consultant le registre.
  /// </remarks>
  public class BaseRegistry {																				// <wao sp�cif.&header>
  
    /// <summary>
    /// Liste des associations cl�-valeur
    /// </summary>
    protected static SortedList storage = new SortedList() ;

    static BaseRegistry() {
      storage.Clear();
    }

    /// <summary>
    /// Ajouter une entr�e dans le registre.
    /// </summary>
    /// <param name="key">cl� de la nouvelle entr�e</param>
    /// <param name="value">r�f�rence sur l'objet � associer � cette cl�</param>
    /// <returns>L'objet value transmis en argument</returns>
    /// <exception cref="ERegistry">
    /// si la cl� est null, ou si la valeur est null, ou si l'entr�e existe d�j� dans le registre
    /// </exception>
    public static object Add( string key, object value ) {				//	<wao sp�cif.&body>
      if ((key == null) || (key == (""))) throw new ERegistry( key, "Add", "tentative d'enregister un objet avec une cl� vide" ) ;
      if (value == null)                  throw new ERegistry( key, "Add", "tentative d'enregistrement d'un objet null" ) ;
      if (storage.Contains(key))          throw new ERegistry( key, "Add", "tentative d'enregistrement d'un objet avec une cl� d�j� utilis�e" ) ;
      storage.Add( key, value ) ; 
      return value ;
    }
	
    /// <summary>
    /// Supprimer une entr�e du registre.
    /// </summary>
    /// <param name="key">cl� de l'entr�e � supprimer</param>
    public static void Remove( string key ) {													// <wao sp�cif.&body>
      storage.Remove( key ) ; 
    }
	
    /// <summary>
    /// Supprimer toutes les entr�es r�f�ren�ant un objet en r�le de valeur
    /// </summary>
    /// <param name="value">r�f�rence sur l'objet � effacer du registre</param>
    public static void Remove( object value ) {	  									// <wao sp�cif.&body>
      int ix = 0 ;
      while (ix < storage.Count) 
        if (storage.GetByIndex( ix ) == value)
          storage.RemoveAt( ix ) ;
        else
          ix++ ;
    }
	
    /// <summary>
    /// Indique si une cl� a une occurrence dans le registre.
    /// </summary>
    /// <param name="key">cl� � rechercher dans le registre</param>
    /// <returns>true si la cl� a une occurrence dans le registre, false sinon</returns>
    public static bool Has( string key ) {											// <wao sp�cif.&body>
      return storage.Contains( key ) ; 
    }
	
    /// <summary>
    /// Obtenir la r�f�rence sur l'objet associ� � une cl�.
    /// </summary>
    /// <param name="key">cl� � rechercher dans le registre</param>
    /// <returns>la r�f�rence sur l'objet</returns>
    /// <exception cref="ERegistry">si la cl� est introuvable</exception>
    public static object Get( string key ) {												// <wao sp�cif.&body>
      object result = GetIf( key ) ;
      if (result == null) throw new ERegistry( key, "Get", "objet non enregistr� dans le registre d'application ou null" ) ;
      return result ;
    }	

    /// <summary>
    /// Obtenir la r�f�rence sur l'objet associ� � une cl�.
    /// </summary>
    /// <param name="key">cl� � rechercher dan le registre</param>
    /// <returns>la r�f�rence sur l'objet, null si introuvable</returns>
    public static object GetIf( string key ) {										// <wao sp�cif.&body>
      try { return storage[ key ] ; }
      catch (Exception e) { throw new ERegistry( key, "Get/GetIf", "exception lors de l'acc�s", e ) ; }
    }	
  }																																	// <wao sp�cif.&ender>

} // namespace
