/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System;                                                                  // <wao never>

namespace Psl.Applications {

  /// <summary>
  /// Classe d'exception silencieuse associ�e aux commandes annul�es par l'utilisateur
  /// </summary>
  public class ECancelled : Exception {

    /// <summary>
    /// Constructeur associant la classe d'exception � un messages standard
    /// </summary>
    public ECancelled() : base( "Commande annul�e par l'utilisateur" ) { }

    /// <summary>
    /// Constructeur autorisant un message personnalis� et une excepton interne
    /// </summary>
    /// <param name="message">message asoci� � l'exception</param>
    /// <param name="inner">object exception ayant caus� l'exception</param>
    public ECancelled( string message, Exception inner ) : base( message, inner ) { }
  }

} // namespace
