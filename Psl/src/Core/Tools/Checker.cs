/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System;                                                                  // <wao cnv>
namespace Psl.Tools {                                                         // <wao cnv>
                                                                               // <wao cnv>
  /// <summary>
  /// Bo�te � outils minimale pour les plans de test en application console.
  /// </summary>
  /// <remarks>
  /// Cette bo�te � outils est destin�e � faciliter la r�daction de plans
  /// de test automatiques muets, targetAsControl'est-�-dire n'�ditant de diagnostics
  /// que pour notifier une diff�rence entre une valeur attendue et une
  /// valeur effectivement obtenue. 
  /// 18 02 2006 : adjonction des m�thode pour le contr�le des exceptions
  /// </remarks>
  public class Checker {                                                         // <wao cnv>

    /// <summary> 
    /// Intitul� de la phase de test en cours
    /// </summary>
    /// <remarks>
    /// Ce libell� est m�moris� lors de l'appel � la m�thode Begin,
    /// il est rappel� lors de la l'appel � la m�thode End. 
    /// </remarks>
    protected static string title = "???" ;                                      // <wao cnv>
                                                                                 // <wao cnv>
    /// <summary>
    /// D�termination de l'intitul� au d�but d'une phase de test.
    /// </summary>
    /// <remarks>
    /// Cet intitul� est destin� � faciliter l'identification des difficult�s.
    /// </remarks>
    /// <param name="ATitle">intitul� de la phase de test en cours</param>
    public static void Begin( string ATitle  ) {                                // <wao code.&body>
      title = ATitle ;
      Console.WriteLine( "[d�but : " + title + "]" ) ;
    }

    /// <summary>
    /// Notification de la fin d'une phase de test.
    /// </summary>
    /// <remarks>
    /// Entre l'application de cette m�thode et la prochaine application 
    /// de la m�thode <see cref="Begin"/>, l'intitul� de la phase courante
    /// est ind�termin�, et est affich� "???"
    /// </remarks>
    public static void End() {                                                  // <wao code.&body>
      Console.WriteLine( "[fin   : " + title + "]" ) ;
      Console.WriteLine() ;
      title = "???" ;
    }
                                                                                // <wao cnv>
    /// <summary>
    /// Edition d'un messages simple sans d�coration
    /// </summary>
    /// <remarks>
    /// Le texte transmis en argument est simplement �dit� tel quel. 
    /// </remarks>
    /// <param name="AMessage">texte du messages � �diter</param>
    public static void Message( string AMessage ) {                             // <wao code.&body>
      Console.WriteLine( AMessage ) ;
    }
                                                                                // <wao cnv>
    /// <summary>
    /// M�thode pour le contr�le d'un cas de test �l�mentaire (type string).
    /// </summary>
    /// <remarks>
    /// Le principe de cette m�thode consiste � comparer la valeur de l'argument
    /// AAttendu et la valeur de l'argument AObtenu. Si les deux valeurs sont 
    /// identiques, la m�thode n'effectue aucune action. Si les deux valeurs sont
    /// diff�rentes, la m�thode �dite l'argument AMessage (identification du cas de
    /// test), la valeur attendue et la valeur effectivement obtenue.
    /// </remarks>
    /// <param name="AMessage">cha�ne servant � identifier le cas de test</param>
    /// <param name="AAttendu">cha�ne de caract�res attendue</param>
    /// <param name="AObtenu">cha�ne de caract�res effectivement obtenue</param>
    public static void Check(string AMessage, string  AAttendu, string  AObtenu) { // <wao code.&body check.begin> 
  
      // Rien � faire si AAttendu et AObtenu co�ncident
      if ( AAttendu == AObtenu ) return ;
    
      // Identification par d�faut du cas de test
      string message = AMessage == "" ? 
        "** Echec de comparaison" :
        "** Cas : " + AMessage ;
      
      // Edition du diagnostic
      Console.WriteLine() ;
      Console.WriteLine( message ) ;
      Console.WriteLine( "** attendu : <" + AAttendu + ">" ) ;
      Console.WriteLine( "** obtenu  : <" + AObtenu  + ">" ) ;
      Console.WriteLine() ;
    }                                                                            
                                                                               // <wao check.end>
    /// <summary>
    /// M�thode pour le contr�le d'un cas de test �l�mentaire (type boolean).
    /// </summary>
    /// <remarks>
    /// M�me sp�cification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">cha�ne servant � identifier le cas de test</param>
    /// <param name="AAttendu">bool�en attendu</param>
    /// <param name="AObtenu">bool�en effectivement obtenu</param>
    public static void Check(string AMessage, bool    AAttendu, bool    AObtenu) {  // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// M�thode pour le contr�le d'un cas de test �l�mentaire (type int).
    /// </summary>
    /// <remarks>
    /// M�me sp�cification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">cha�ne servant � identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type int attendue</param>
    /// <param name="AObtenu">valeur de type int effectivement obtenue</param>
    public static void Check(string AMessage, int     AAttendu, int     AObtenu) {  // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// M�thode pour le contr�le d'un cas de test �l�mentaire (type double).
    /// </summary>
    /// <remarks>
    /// M�me sp�cification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">cha�ne servant � identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type double attendue</param>
    /// <param name="AObtenu">valeur de type double effectivement obtenue</param>
    public static void Check(string AMessage, double  AAttendu, double  AObtenu) { // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// M�thode pour le contr�le d'un cas de test �l�mentaire (type object).
    /// </summary>
    /// <remarks>
    /// M�me sp�cification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">cha�ne servant � identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type object attendue</param>
    /// <param name="AObtenu">valeur de type object effectivement obtenue</param>
    public static void Check(string AMessage, Object  AAttendu, Object  AObtenu) { // <wao code.&body>
      Check( AMessage, AAttendu.ToString(), AObtenu.ToString() ) ;
    }
                                                                               // <wao cnv>
    // Aspects li�s au contr�le des exceptions                                 // <wao cnv>
                                                                               // <wao cnv>
    /// <summary>
    /// M�morisation du dernier objet exception notifi� par la m�thode Thrown.
    /// </summary>
    /// <remarks>
    /// Ce champ est normalement � null sauf, �ventuellement, entre un appel � Thrown
    /// et l'appel correspondant � Expected. 
    /// </remarks>
    protected static Exception thrown ;                                          // <wao cnv cnvException>

    /// <summary>
    /// Notifie qu'un objet exception a �t� r�cup�r�.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est destin�e � �tre appel�e depuis un bloc catch(Exception x) 
    /// au sein d'un cas de test. Un appel � Thrown doit obligatoire �tre associ� � 
    /// un appel � la m�thode Expected. 
    /// </remarks>
    /// <param name="e">object exception r�cup�r�</param>
    public static void Thrown( Exception e ) {                               // <wao code.&body codeException.&body>
      thrown = e ;
    }

    /// <summary>
    /// Contr�le qu'un objet exception d'un certain type a bien �t� r�cup�r�.
    /// </summary>
    /// <remarks>
    /// Cette m�thode ne fait rien dans deux cas : 
    /// (1) si la m�thode Thrown a pr�c�demment �t� appel�e en transmettant
    /// comme argument un objet exception dont le type est transmis en tant qu'argument type 
    /// de la m�thode Expected (l'exception du type attendu a bien �t� d�clench�e) ;
    /// (2) si la m�thode Thrown n'a pas �t� appel�e et si l'argument type de la m�thode Expected
    /// est null (aucune exception attendue ni aucune exception d�clench�e). 
    /// Dans tous les autres cas, la m�thode Thrown �dite un diagnostic d'erreur appropri�.
    /// Dans l'exemple qui suit, l'op�ration plac�e sous contr�le n'est consid�r�e comme
    /// correcte que si elle d�clenche une exception du type ArgumentOutOfRangeException. 
    /// Un diagnostic d'erreur sera �dit� si aucune exception n'est d�clench�e ou si une exception
    /// d'un autre type est d�clench�e. 
    /// </remarks>
    /// <example>
    /// try { &lt;op�ration plac�e sous contr�le&gt; } catch(Exception x) { Checker.Thrown( x ) ; }
    /// Checker.Expected( "cas id", typeof( ArgumentOutOfRangeException ) ) ;
    /// </example>
    /// <param name="cas">identification du cas de test</param>
    /// <param name="type">type associ� � l'exception qu'on s'attend � r�cup�rer</param>
    public static void Expected( string cas, Type type ) {                     // <wao code.&body codeException.&body>
      try {
        if (thrown == null)
          if (type == null)
            return ;
          else
            Check( cas, "exception : " + type.Name, "exception non d�clench�e" ) ;
        else
          if (type == null)
            Check( cas, "aucune exception", "exception : " + thrown.GetType().Name + " (" + thrown.Message + ")" ) ;
          else
            if (thrown.GetType() == type)
              return ;
           else
             Check( cas, "exception : " + type.Name, "exception : " + thrown.GetType().Name + " (" + thrown.Message + ")" ) ;        
      } finally { thrown = null ; }
    }


  }                                                                            // <wao cnv>
} // namespace                                                                 // <wao cnv>
