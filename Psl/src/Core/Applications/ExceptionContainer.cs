/*
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 22 01 2009 : version initiale
 */
using System;
using System.Collections.Generic;

namespace Psl.Applications {

  /// <summary>
  /// Classe d'exception pour des objets exceptions regroupant plusieurs objets exception
  /// </summary>
  /// <remarks>
  /// Cette classe et simplement destinée à exposer une collection <see cref="ContainedExceptions"/>
  /// permettant de mémoriser une collection d'objets exceptions, même si ces objets exception
  /// proviennent de classes d'exceptions non sérialisables. En effet, le propriété <see cref="Exception.Data"/>,
  /// qui accepte tous objets, n'accepte cependant que des objets exception sérialisables.
  /// <br/>
  /// Voir la boîte <see cref="ExceptionBox"/> pour l'affichage des objets exception contenus. 
  /// </remarks>
  public class ExceptionContainer : Exception {

    // collection des exceptions
    private List<Exception> exceptions = new List<Exception>();

    /// <summary>
    /// Constructeur par défaut sans paramètres
    /// </summary>
    public ExceptionContainer() : base() { }

    /// <summary>
    /// Constructeur acceptant un message.
    /// </summary>
    /// <param name="message">message associé à l'exception</param>
    public ExceptionContainer( string message ) : base( message ) { }

    /// <summary>
    /// Constructeur acceptant un message et un objet exception inner
    /// </summary>
    /// <param name="message">message associé à l'exception</param>
    /// <param name="inner">objet exception inner</param>
    public ExceptionContainer( string message, Exception inner ) : base( message, inner ) { }

    /// <summary>
    /// Obtient la collection des exceptions contenues dans l'objet exception.
    /// </summary>
    public List<Exception> ContainedExceptions {
      get { return exceptions; }
    }

    /// <summary>
    /// Obtient le premier message enveloppé dans le conteneur d'exceptions.
    /// </summary>
    public string FirstMessage {
      get {
        Exception first = null;
        if ( exceptions.Count != 0 ) first = exceptions[ 0 ];
        if ( first == null ) first = InnerException;

        if ( first == null ) return Message;
        if (first is ExceptionContainer) return (first as ExceptionContainer).FirstMessage;
        return first.Message;
      }
    }
  }
}
