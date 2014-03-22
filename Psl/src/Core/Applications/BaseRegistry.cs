/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 05 2007 : amélioration du libellé de certains diagnostics
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
    /// <param name="op">libellé de l'opération déclenchant l'exception</param>
    /// <param name="msg">diagnostic spécifique de l'exception</param>
    /// <param name="inner">object exception ayant causé l'exception</param>
    public ERegistry( string op, string msg, Exception inner ) :
      base( ""
      + "Exception au niveau du registre interne de l'application\r\n"
      + "opération : " + '"' + op + '"' + "\r\n"
      + "diagnostic : " + '"' + msg + '"',
      inner) { }

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="op">libellé de l'opération déclenchant l'exception</param>
    /// <param name="msg">diagnostic spécifique de l'exception</param>
    public ERegistry( string op, string msg ) : this( op, msg, (Exception) null ) {}

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="key">clé de registre impliquée par l'exception</param>
    /// <param name="op">libellé de l'opération déclenchant l'exception</param>
    /// <param name="msg">diagnostic spécifique de l'exception</param>
    /// <param name="inner">objet exception cause de l'exception</param>
    public ERegistry( string key, string op, string msg, Exception inner ) :
      base( ""
      + "Exception au niveau du registre interne de l'application\r\n"
      + "opération : " + '"' + op + '"' + "\r\n"
      + "clé : " + '"' + key + '"' + "\r\n"
      + "diagnostic : " + '"' + msg + '"',
      inner ) {}

    /// <summary>
    /// Initialisation d'un objet exception.
    /// </summary>
    /// <param name="key">clé de registre impliquée par l'exception</param>
    /// <param name="op">libellé de l'opération déclenchant l'exception</param>
    /// <param name="msg">diagnostic spécifique de l'exception</param>
    public ERegistry( string key, string op, string msg ) : this( key, op, msg, null ) {}
  }

  /// <summary>
  /// Base du registre interne d'application.
  /// </summary>
  /// <remarks>
  /// Cette classe ne comporte que des membres de classe et ne doit donc jamais
  /// être instanciée. 
  /// <br/>
  /// Le registre interne d'application n'est rien d'autre qu'une table
  /// d'associations clé-valeur accompagnée de quelques raccourcis usuels.
  /// Les clés sont [en principe] des chaînes de caractères, les valeurs des
  /// instances de Object. 
  /// <br/>
  /// Le registre d'application est destiné faire en sorte que les différents 
  /// plugins d'une application soient hautement indépendants du point de vue
  /// syntaxique et surtout du point de vue des implémentations. 
  /// <br/>
  /// Tout objet (composant visuel, instance de classe, serveur d'interface, etc.)
  /// qui est fourni par un plugin de l'application et utilisé par un autre
  /// doit être enregistré dans le registre par le plugin fournisseur. Les plugins
  /// utilisant un tel objet doivent obtenir sa référence en consultant le registre.
  /// </remarks>
  public class BaseRegistry {																				// <wao spécif.&header>
  
    /// <summary>
    /// Liste des associations clé-valeur
    /// </summary>
    protected static SortedList storage = new SortedList() ;

    static BaseRegistry() {
      storage.Clear();
    }

    /// <summary>
    /// Ajouter une entrée dans le registre.
    /// </summary>
    /// <param name="key">clé de la nouvelle entrée</param>
    /// <param name="value">référence sur l'objet à associer à cette clé</param>
    /// <returns>L'objet value transmis en argument</returns>
    /// <exception cref="ERegistry">
    /// si la clé est null, ou si la valeur est null, ou si l'entrée existe déjà dans le registre
    /// </exception>
    public static object Add( string key, object value ) {				//	<wao spécif.&body>
      if ((key == null) || (key == (""))) throw new ERegistry( key, "Add", "tentative d'enregister un objet avec une clé vide" ) ;
      if (value == null)                  throw new ERegistry( key, "Add", "tentative d'enregistrement d'un objet null" ) ;
      if (storage.Contains(key))          throw new ERegistry( key, "Add", "tentative d'enregistrement d'un objet avec une clé déjà utilisée" ) ;
      storage.Add( key, value ) ; 
      return value ;
    }
	
    /// <summary>
    /// Supprimer une entrée du registre.
    /// </summary>
    /// <param name="key">clé de l'entrée à supprimer</param>
    public static void Remove( string key ) {													// <wao spécif.&body>
      storage.Remove( key ) ; 
    }
	
    /// <summary>
    /// Supprimer toutes les entrées référençant un objet en rôle de valeur
    /// </summary>
    /// <param name="value">référence sur l'objet à effacer du registre</param>
    public static void Remove( object value ) {	  									// <wao spécif.&body>
      int ix = 0 ;
      while (ix < storage.Count) 
        if (storage.GetByIndex( ix ) == value)
          storage.RemoveAt( ix ) ;
        else
          ix++ ;
    }
	
    /// <summary>
    /// Indique si une clé a une occurrence dans le registre.
    /// </summary>
    /// <param name="key">clé à rechercher dans le registre</param>
    /// <returns>true si la clé a une occurrence dans le registre, false sinon</returns>
    public static bool Has( string key ) {											// <wao spécif.&body>
      return storage.Contains( key ) ; 
    }
	
    /// <summary>
    /// Obtenir la référence sur l'objet associé à une clé.
    /// </summary>
    /// <param name="key">clé à rechercher dans le registre</param>
    /// <returns>la référence sur l'objet</returns>
    /// <exception cref="ERegistry">si la clé est introuvable</exception>
    public static object Get( string key ) {												// <wao spécif.&body>
      object result = GetIf( key ) ;
      if (result == null) throw new ERegistry( key, "Get", "objet non enregistré dans le registre d'application ou null" ) ;
      return result ;
    }	

    /// <summary>
    /// Obtenir la référence sur l'objet associé à une clé.
    /// </summary>
    /// <param name="key">clé à rechercher dan le registre</param>
    /// <returns>la référence sur l'objet, null si introuvable</returns>
    public static object GetIf( string key ) {										// <wao spécif.&body>
      try { return storage[ key ] ; }
      catch (Exception e) { throw new ERegistry( key, "Get/GetIf", "exception lors de l'accès", e ) ; }
    }	
  }																																	// <wao spécif.&ender>

} // namespace
