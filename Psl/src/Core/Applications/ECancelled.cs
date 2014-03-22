/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System;                                                                  // <wao never>

namespace Psl.Applications {

  /// <summary>
  /// Classe d'exception silencieuse associée aux commandes annulées par l'utilisateur
  /// </summary>
  public class ECancelled : Exception {

    /// <summary>
    /// Constructeur associant la classe d'exception à un messages standard
    /// </summary>
    public ECancelled() : base( "Commande annulée par l'utilisateur" ) { }

    /// <summary>
    /// Constructeur autorisant un message personnalisé et une excepton interne
    /// </summary>
    /// <param name="message">message asocié à l'exception</param>
    /// <param name="inner">object exception ayant causé l'exception</param>
    public ECancelled( string message, Exception inner ) : base( message, inner ) { }
  }

} // namespace
