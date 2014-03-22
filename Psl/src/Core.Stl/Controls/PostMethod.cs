/*
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Utilitaires pour poster des méthodes de rappel
 * 
 * 27 10 2008 : vesion initiale
 */
using System;
using System.Threading;
using System.Windows.Forms;

namespace Psl.Controls {

  /// <summary>
  /// Classe utilitaire pour poster des méthodes de rappel.
  /// </summary>
  /// <remarks>
  /// Cette classe de service a deux objectifs :
  /// <br/>
  /// 1) elle permet de poster directement des méthodes rappel depuis n'importe 
  ///    quelle classe, sans avoir à atteindre des éléments de l'UI quand il
  ///    s'agit de poster une méthode pour le thread principal
  /// <br/>
  /// 2) elle permet en même temps d'éviter d'avoir à
  ///    produire des méthodes de rappel intermédiaires quand les méthodes à
  ///    rappeler comportent plus d'un paramètre : les méthodes Post proposées 
  ///    par les contextes de synchronization sont liées au type de délégué 
  ///    SendOrPostCallback qui n'admettent qu'un paramètre de type object.
  /// <br/>
  /// En outre, les méthodes de services exposées opèrent directement en
  /// utilisant le contexte de synchronisation approprié s'il existe. 
  /// </remarks>
  public class PostMethod {

    /// <summary>
    /// Classe interne mémorisant la méthode à rappeler, avec ses paramètres.
    /// </summary>
    private class CallbackData {

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="dlg">délégué à rappeler</param>
      /// <param name="args">arguments à transmettre au délégué</param>
      public CallbackData( Delegate dlg, object[] args ) {
        Dlg = dlg;
        Args = args;
      }

      /// <summary>
      /// Délégué associé à la méthode de rappel
      /// </summary>
      public Delegate Dlg { get; set;}

      /// <summary>
      /// Paramètres actuels à fournir à la méthode de rappel
      /// </summary>
      public object[] Args { get; set; }
    }

    /// <summary>
    /// Méthode de rappel effectivement postée.
    /// </summary>
    /// <remarks>
    /// Les méthodes Post proposées par les contextes de synchronization sont
    /// liées au type de délégué SendOrPostCallback qui n'admettent qu'un paramètre
    /// de type object. 
    /// </remarks>
    /// <param name="state">référence sur une instance de CallbackData</param>
    private static void CallBack( object state ) {      
      CallbackData data = (CallbackData) state;
      data.Dlg.Method.Invoke( data.Dlg.Target, data.Args );
    }

    /// <summary>
    /// Invoke le délégué de manière synchrone sur le thread créateur du contrôle.
    /// </summary>
    /// <remarks>
    /// Si la propriété InvokeRequired du contrôle retourne true, l'invocation s'effectue via
    /// la méthode Invoke du contrôle ; sinon le délégué est immédiatement invoqué. 
    /// </remarks>
    /// <param name="control">contrôle sur le thread duquel l'invocation doit être réalisée</param>
    /// <param name="dlg">délégué à invoquer</param>
    /// <param name="args">arguments à transmettre au délégué</param>
    public static object InvokeOrCall( Control control, Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );
      if ( control == null ) throw new ArgumentNullException( "control" );

      if ( control.InvokeRequired )
        return control.Invoke( dlg, args );
      else
        return dlg.Method.Invoke( dlg.Target, args );
    }

    /// <summary>
    /// Invoke le délégué de manière synchrone sur le thread principal.
    /// </summary>
    /// <remarks>
    /// L'invocation est contrôlée via la propriété InvokeRequired et la méthode Invoke
    /// de la première fenêtre enregistrée dans Application.OpenForms. Une exception est
    /// déclenchée si aucune fenêtre ne figure dans cette liste.
    /// <br/>
    /// Si la propriété InvokeRequired de cette fenêtre retourne true, l'invocation s'effectue via
    /// la méthode Invoke de la fenêtre ; sinon le délégué est immédiatement invoqué. 
    /// </remarks>
    /// <param name="dlg">délégué à invoquer</param>
    /// <param name="args">arguments à transmettre au délégué</param>
    public static object InvokeOrCall( Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );
      if ( Application.OpenForms.Count == 0 ) throw new InvalidOperationException( "Aucune fenêtre n'est ouverte dans l'application" );
      return InvokeOrCall( Application.OpenForms[ 0 ], dlg, args );
    }

    /// <summary>
    /// Effectue le rappel posté d'une méthode sur un thread quelconque du pool des threads de travail.
    /// </summary>
    /// <param name="dlg">délégué associé à la méthode à rappeler</param>
    /// <param name="args">paramètres actuels de la méthode à rappeler</param>
    public static void ToThreadPool( Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );
      ThreadPool.QueueUserWorkItem( new WaitCallback( CallBack ), new CallbackData( dlg, args ) );
    }

    /// <summary>
    /// Effectue le rappel posté d'une méthode sur le thread courant si c'est possible.
    /// </summary>
    /// <remarks>
    /// Après examen du code source du framework, il s'avère que cette méthode est équivalente à ToThreadPool.
    /// <br/>
    /// Lorsqu'il n'y a pas de contexte de synchronisation pour le thread courant, je ne dispose
    /// pas de moyen pour effectuer le rappel sur le thread courant. J'utilise alors une mise
    /// en attente pour un thread du pool de threads de travail.
    /// </remarks>
    /// <param name="dlg">délégué associé à la méthode à rappeler</param>
    /// <param name="args">paramètres actuels de la méthode à rappeler</param>
    [Obsolete( "Utiliser désormais la méthode ToThreadPool" )]
    public static void ToCurrentThread( Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );

      // tenter de poster pour le thread courant
      if ( SynchronizationContext.Current != null )
        SynchronizationContext.Current.Post( new SendOrPostCallback( CallBack ), new CallbackData( dlg, args ) );
      else
        ToThreadPool( dlg, args );
    }

    /// <summary>
    /// Invoke le délégué de manière asynchrone sur le thread créateur du contrôle.
    /// </summary>
    /// <remarks>
    /// Si la propriété InvokeRequired du contrôle retourne true, l'invocation s'effectue via
    /// la méthode Invoke du contrôle ; sinon le délégué est immédiatement invoqué. 
    /// </remarks>
    /// <param name="control">contrôle sur le thread duquel l'invocation doit être réalisée</param>
    /// <param name="dlg">délégué à invoquer</param>
    /// <param name="args">arguments à transmettre au délégué</param>
    public static void ToMainThread( Control control, Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );
      if ( control == null ) throw new ArgumentNullException( "control" );
      control.BeginInvoke( dlg, args );
    }

    /// <summary>
    /// Effectue le rappel posté d'une méthode sur le thread principal 
    /// </summary>
    /// <remarks>
    /// S'il n'y a pas de contexte de synchronisation Windows Forms, je n'ai pas trouvé d'autre
    /// moyen (pas très élégant) que celui d'utiliser la méthode Invoke de la première fenêtre listée dans
    /// Application.OpenForms (il n'y a pas moyen d'accéder à la fenêtre principale via des 
    /// méthodes ou propriétés de classe). 
    /// <br/>
    /// Déclenche une exception si aucun moyen d'atteindre le thread principal n'est trouvé. 
    /// </remarks>
    /// <param name="dlg">délégué associé à la méthode à rappeler</param>
    /// <param name="args">paramètres actuels de la méthode à rappeler</param>
    public static void ToMainThread( Delegate dlg, params object[] args ) {
      if ( dlg == null ) throw new ArgumentNullException( "dlg" );
      if ( Application.OpenForms.Count == 0 ) throw new InvalidOperationException( "Aucune fenêtre n'est ouverte dans l'application" );
      ToMainThread( Application.OpenForms[ 0 ], dlg, args );
    }
  }
}
