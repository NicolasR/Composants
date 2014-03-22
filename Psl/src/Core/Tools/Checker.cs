/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System;                                                                  // <wao cnv>
namespace Psl.Tools {                                                         // <wao cnv>
                                                                               // <wao cnv>
  /// <summary>
  /// Boîte à outils minimale pour les plans de test en application console.
  /// </summary>
  /// <remarks>
  /// Cette boîte à outils est destinée à faciliter la rédaction de plans
  /// de test automatiques muets, targetAsControl'est-à-dire n'éditant de diagnostics
  /// que pour notifier une différence entre une valeur attendue et une
  /// valeur effectivement obtenue. 
  /// 18 02 2006 : adjonction des méthode pour le contrôle des exceptions
  /// </remarks>
  public class Checker {                                                         // <wao cnv>

    /// <summary> 
    /// Intitulé de la phase de test en cours
    /// </summary>
    /// <remarks>
    /// Ce libellé est mémorisé lors de l'appel à la méthode Begin,
    /// il est rappelé lors de la l'appel à la méthode End. 
    /// </remarks>
    protected static string title = "???" ;                                      // <wao cnv>
                                                                                 // <wao cnv>
    /// <summary>
    /// Détermination de l'intitulé au début d'une phase de test.
    /// </summary>
    /// <remarks>
    /// Cet intitulé est destiné à faciliter l'identification des difficultés.
    /// </remarks>
    /// <param name="ATitle">intitulé de la phase de test en cours</param>
    public static void Begin( string ATitle  ) {                                // <wao code.&body>
      title = ATitle ;
      Console.WriteLine( "[début : " + title + "]" ) ;
    }

    /// <summary>
    /// Notification de la fin d'une phase de test.
    /// </summary>
    /// <remarks>
    /// Entre l'application de cette méthode et la prochaine application 
    /// de la méthode <see cref="Begin"/>, l'intitulé de la phase courante
    /// est indéterminé, et est affiché "???"
    /// </remarks>
    public static void End() {                                                  // <wao code.&body>
      Console.WriteLine( "[fin   : " + title + "]" ) ;
      Console.WriteLine() ;
      title = "???" ;
    }
                                                                                // <wao cnv>
    /// <summary>
    /// Edition d'un messages simple sans décoration
    /// </summary>
    /// <remarks>
    /// Le texte transmis en argument est simplement édité tel quel. 
    /// </remarks>
    /// <param name="AMessage">texte du messages à éditer</param>
    public static void Message( string AMessage ) {                             // <wao code.&body>
      Console.WriteLine( AMessage ) ;
    }
                                                                                // <wao cnv>
    /// <summary>
    /// Méthode pour le contrôle d'un cas de test élémentaire (type string).
    /// </summary>
    /// <remarks>
    /// Le principe de cette méthode consiste à comparer la valeur de l'argument
    /// AAttendu et la valeur de l'argument AObtenu. Si les deux valeurs sont 
    /// identiques, la méthode n'effectue aucune action. Si les deux valeurs sont
    /// différentes, la méthode édite l'argument AMessage (identification du cas de
    /// test), la valeur attendue et la valeur effectivement obtenue.
    /// </remarks>
    /// <param name="AMessage">chaîne servant à identifier le cas de test</param>
    /// <param name="AAttendu">chaîne de caractères attendue</param>
    /// <param name="AObtenu">chaîne de caractères effectivement obtenue</param>
    public static void Check(string AMessage, string  AAttendu, string  AObtenu) { // <wao code.&body check.begin> 
  
      // Rien à faire si AAttendu et AObtenu coïncident
      if ( AAttendu == AObtenu ) return ;
    
      // Identification par défaut du cas de test
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
    /// Méthode pour le contrôle d'un cas de test élémentaire (type boolean).
    /// </summary>
    /// <remarks>
    /// Même spécification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">chaîne servant à identifier le cas de test</param>
    /// <param name="AAttendu">booléen attendu</param>
    /// <param name="AObtenu">booléen effectivement obtenu</param>
    public static void Check(string AMessage, bool    AAttendu, bool    AObtenu) {  // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// Méthode pour le contrôle d'un cas de test élémentaire (type int).
    /// </summary>
    /// <remarks>
    /// Même spécification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">chaîne servant à identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type int attendue</param>
    /// <param name="AObtenu">valeur de type int effectivement obtenue</param>
    public static void Check(string AMessage, int     AAttendu, int     AObtenu) {  // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// Méthode pour le contrôle d'un cas de test élémentaire (type double).
    /// </summary>
    /// <remarks>
    /// Même spécification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">chaîne servant à identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type double attendue</param>
    /// <param name="AObtenu">valeur de type double effectivement obtenue</param>
    public static void Check(string AMessage, double  AAttendu, double  AObtenu) { // <wao code.&body>
      Check( AMessage, "" + AAttendu, "" + AObtenu ) ;
    }

    /// <summary>
    /// Méthode pour le contrôle d'un cas de test élémentaire (type object).
    /// </summary>
    /// <remarks>
    /// Même spécification de principe que <see cref="Check(string,string,string)"/>.
    /// </remarks>
    /// <param name="AMessage">chaîne servant à identifier le cas de test</param>
    /// <param name="AAttendu">valeur de type object attendue</param>
    /// <param name="AObtenu">valeur de type object effectivement obtenue</param>
    public static void Check(string AMessage, Object  AAttendu, Object  AObtenu) { // <wao code.&body>
      Check( AMessage, AAttendu.ToString(), AObtenu.ToString() ) ;
    }
                                                                               // <wao cnv>
    // Aspects liés au contrôle des exceptions                                 // <wao cnv>
                                                                               // <wao cnv>
    /// <summary>
    /// Mémorisation du dernier objet exception notifié par la méthode Thrown.
    /// </summary>
    /// <remarks>
    /// Ce champ est normalement à null sauf, éventuellement, entre un appel à Thrown
    /// et l'appel correspondant à Expected. 
    /// </remarks>
    protected static Exception thrown ;                                          // <wao cnv cnvException>

    /// <summary>
    /// Notifie qu'un objet exception a été récupéré.
    /// </summary>
    /// <remarks>
    /// Cette méthode est destinée à être appelée depuis un bloc catch(Exception x) 
    /// au sein d'un cas de test. Un appel à Thrown doit obligatoire être associé à 
    /// un appel à la méthode Expected. 
    /// </remarks>
    /// <param name="e">object exception récupéré</param>
    public static void Thrown( Exception e ) {                               // <wao code.&body codeException.&body>
      thrown = e ;
    }

    /// <summary>
    /// Contrôle qu'un objet exception d'un certain type a bien été récupéré.
    /// </summary>
    /// <remarks>
    /// Cette méthode ne fait rien dans deux cas : 
    /// (1) si la méthode Thrown a précédemment été appelée en transmettant
    /// comme argument un objet exception dont le type est transmis en tant qu'argument type 
    /// de la méthode Expected (l'exception du type attendu a bien été déclenchée) ;
    /// (2) si la méthode Thrown n'a pas été appelée et si l'argument type de la méthode Expected
    /// est null (aucune exception attendue ni aucune exception déclenchée). 
    /// Dans tous les autres cas, la méthode Thrown édite un diagnostic d'erreur approprié.
    /// Dans l'exemple qui suit, l'opération placée sous contrôle n'est considérée comme
    /// correcte que si elle déclenche une exception du type ArgumentOutOfRangeException. 
    /// Un diagnostic d'erreur sera édité si aucune exception n'est déclenchée ou si une exception
    /// d'un autre type est déclenchée. 
    /// </remarks>
    /// <example>
    /// try { &lt;opération placée sous contrôle&gt; } catch(Exception x) { Checker.Thrown( x ) ; }
    /// Checker.Expected( "cas id", typeof( ArgumentOutOfRangeException ) ) ;
    /// </example>
    /// <param name="cas">identification du cas de test</param>
    /// <param name="type">type associé à l'exception qu'on s'attend à récupérer</param>
    public static void Expected( string cas, Type type ) {                     // <wao code.&body codeException.&body>
      try {
        if (thrown == null)
          if (type == null)
            return ;
          else
            Check( cas, "exception : " + type.Name, "exception non déclenchée" ) ;
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
