/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 21 01 2008 : version aihm 2007-2008 pour net 2.0
 * 29 11 2008 : normalisation des identificateurs
 * 17 01 2011 : la méthode OnClosing bascule e.Cancel à false avant déclenchement de l'événement
 */                                                                            // <wao never.end>

using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

namespace Psl.Applications {

  /// <summary>
  /// Classe des exceptions produites par la gestion de l'état de l'application.
  /// </summary>
  public class EApplicationState : Exception {
    
    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="msg">Message associé à l'objet exception</param>
    public EApplicationState( string msg ) : base( msg ) {}

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="msg">Message associé à l'objet exception</param>
    /// <param name="inner">Object exception ayant cause l'exception</param>
    public EApplicationState( string msg, Exception inner ) : base( msg, inner ) { }
  }

  /// <summary>
  /// Classe des exceptions produites par la diffusion de l'événement Idle de l'application.
  /// </summary>
  public class EApplicationStateIdle : EApplicationState {

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="inner">objet exception intercepté</param>
    public EApplicationStateIdle( Exception inner ) 
      : base( 
        "Exception interceptée pendant la diffusion de l'événement ApplicationState.ApplicationIdle"
      + "\r\nHeure: " + DateTime.Now.ToLongTimeString(),
      inner
      ) { }
  }

  /// <summary>
  /// Reflet de l'état global de l'application dans ses grandes phases.
  /// </summary>
  /// <remarks>
  /// Cette classe est composée uniquement de membres de classes et ne doit
  /// jamais être instanciée. 
  /// <br/>
  /// Elle est destinée à tenir à jour un status qui correspond à l'état de l'application 
  /// relativement aux phases d'initialisation, de fonctionnement et de terminaison. 
  /// <br/>
  /// Le fonctionnement correct de cette classe suppose un "branchement" qui
  /// s'effectue en appelant, de manière appropriée, les méthodes 
  /// OnOpen, OnClosing OnClose.
  /// <br/>
  /// Les différentes étapes concernant l'état de l'application sont résumées
  /// dans le schéma ci-dessous. 
  /// <br/>
  /// Les étapes élémentaires sont mémorisées telles quelles dans le champ 
  /// status et sont retournées sous la forme d'une constante via la propriété Status. 
  /// 
  /// Les étapes synthétiques (starting, running et finishing) sont reconstituées
  /// à partir des étapes élémentaires.
  /// </remarks>
  public class ApplicationState {                                              // <wao code.&header>

    //     starting               ||        running                 |   finishing       // <wao table.begin>
    // Create |      Open         ||  Started  |       Closing      |    Close
    //        |                   ||           |                    | 
    //        OnOpen              ||           OnClosing            OnClose     
    //        * ApplicationOpen   ||           * ApplicationClosing * ApplicationClose
    //        * ApplicationOpened ||                                * ApplicationClosed
    // ___________________________||___________________________________________________
    //     constructeur de la     ||           méthode Run de la classe 
    //      classe principale     ||       System.Windows.Forms.Application              // <wao table.end>

    private ApplicationState() { }                                                       // <wao never>

    #region Enumération des états de l'application
                                                                               // <wao cnv.begin>
    /// <summary>
    /// Enumération des états de l'application
    /// </summary>
    public enum States {                                                       // <wao strip.begin>
                                                                               
      /// <summary>
      /// Valeur énumérée indiquant que l'application est en cours d'initialisation
      /// </summary>
      Create,

      /// <summary>
      /// Valeur énumérée indiquant que l'application est dans sa deuxième phase d'initialisation
      /// </summary>
      Open,

      /// <summary>
      /// Valeur énumérée indiquant que l'application est en fonctionnement normal
      /// </summary>
      Started,

      /// <summary>
      /// Valeur énumérée indiquant que l'application procède à l'enquête de fermeture
      /// </summary>
      Closing,

      /// <summary>
      /// Valeur énumérée indiquant que l'application est en phase de terminaison
      /// </summary>
      Close
    }                                                                         // <wao cnv.end strip.end >
	
    #endregion

    #region Champs de classe

    /// <summary>
    /// Mémorisation de l'état courant de l'application.
    /// </summary>
    private static States status = States.Create ;

    /// <summary>
    /// Mémorise le nombre d'appels à <see cref="OnClosing"/>
    /// </summary>
    /// <remarks>
    /// Cette mémorisation est destinée à la mise en oeuvre de hooks d'assistance 
    /// à la correction des projets d'étudiants
    /// </remarks>
    private static int closingCount = 0;

    #endregion

    #region Consultation du status                                             // <wao code.&comgroup>
  
    /// <summary>
    /// Accès à l'état courant de l'application.
    /// </summary>
    public static States Status    {                                           // <wao code.&body:ro>
      get { return status ; } 
    }

    /// <summary>
    /// Obtient le nombre d'appels qui ont été effectués à <see cref="OnClosing"/>
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static int ClosingCount {
      get { return closingCount; }
    }

    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsCreate    {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Create ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsOpen      {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Open   ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsStarted   {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Started ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsClosing   {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Closing ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsClose     {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Close ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsStarting  {                                           // <wao xxxcode.&body:ro>
      get { return IsCreate || IsOpen ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsRunning   {                                           // <wao xxxcode.&body:ro>
      get { return IsStarted || IsClosing ; } 
    }
    
    /// <summary>
    /// Raccourci d'accès à l'état de l'application.
    /// </summary>
    public static bool IsFinishing {                                           // <wao xxxcode.&body:ro>
      get { return IsClose ; } 
    }
	
    #endregion

    #region Evénements exposés                                            // <wao code.&comgroup>

    /// <summary>
    /// Evénement déclenché lorsque l'application s'initialise (1ère vague).
    /// </summary>
    public static event EventHandler ApplicationOpen ;                         // <wao cnv>

    /// <summary>
    /// Evénement déclenché lorsque l'application s'initialise (2ème vague).
    /// </summary>
    public static event EventHandler ApplicationOpened ;                       // <wao cnv>

    /// <summary>
    /// Evénement déclenché pour enquête de fermeture.
    /// Un abonné peut empêcher la fermeture de l'application soit en basculant à false
    /// la propriété Cancel des arguments de l'événement, soit en déclenchant une exception ECancelled.
    /// </summary>
    public static event FormClosingEventHandler ApplicationClosing ;           // <wao cnv>

    /// <summary>
    /// Evénément déclenché lorsque l'application se ferme effectivement (1ère vague)
    /// Cet événement n'est déclenché que si l'enquête de fermeture s'est conclu positivement. 
    /// </summary>
    public static event FormClosedEventHandler ApplicationClose ;              // <wao cnv>

    /// <summary>
    /// Evénément déclenché lorsque l'application se ferme effectivement (2ème vague)
    /// Cet événement n'est déclenché que si l'enquête de fermeture s'est conclu positivement. 
    /// </summary>
    public static event FormClosedEventHandler ApplicationClosed;              // <wao cnv>

    /// <summary>
    /// Evénement déclenché lorsque l'application est idle.
    /// </summary>
    /// <remarks>
    /// Cet événement relaie l'événement Idle de la classe Application standard.
    /// Son déclenchement comporte une sécurité (appel à DoEvents) pour éviter les
    /// risques des bouclages sur l'événement idle si les mises à jour induites par
    /// cet événement re-provoquent incessamment un événement idle
    /// </remarks>
    public static event EventHandler ApplicationIdle ;

    /// <summary>
    /// Evénement déclenché lorsqu'une exception a été interceptée pendant l'application 
    /// de la méthode ApplicationState.OnIdle.
    /// </summary>
    /// <remarks>
    /// Si aucune méthode n'est abonnée à cet événement, les exceptions interceptées par
    /// la méthode ApplicationState.OnIdle sont retransmises à l'événement ThreadException
    /// de la classe Application. 
    /// </remarks>
    public static event ThreadExceptionEventHandler IdleException;             // <wao cnv>

    #endregion

    #region Branchement du protocole                                           // <wao code.&comgroup>
                                                                               // <wao branchement.begin>
    /// <summary>
    /// Permet d'indiquer le moment où l'application est initialisée.
    /// </summary>
    /// <remarks>
    /// Cette méthode est une méthode de "branchement" du protocole.
    /// <br/>
    /// Insérer un appel à cette méthode à la fin du constructeur de la fenêtre principale.
    /// </remarks>
    /// <param name="sender">Objet émetteur de l'appel</param>
    /// <param name="e">Arguments de l'événement</param>
    public static void OnOpen( object sender, EventArgs e ) {                  // <wao code.&body>
      if (status != States.Create) 
        throw new EApplicationState( "OnOpen : status incorrect, " + status ) ;
      status = States.Open ;
      try { 
        if (ApplicationOpen   != null) ApplicationOpen  ( sender, e ) ; // 1ère vague
        if (ApplicationOpened != null) ApplicationOpened( sender, e ) ; // 2ème vague
      } 
      finally { 
        status = States.Started ;
        Application.Idle += new EventHandler( OnIdle );
      }
    }
	
    /// <summary>
    /// Permet d'indiquer le moment où l'application tente de se terminer.
    /// </summary>
    /// <remarks>
    /// Cette méthode est une méthode de "branchement" du protocole.
    /// <br/>
    /// Insérer un appel à cette méthode dans le gestionnaire de l'événement 
    /// <see cref="Form.FormClosing"/> de la fenêtre principale.  
    /// </remarks>
    /// <param name="sender">Objet émetteur de l'appel</param>
    /// <param name="e">Arguments de l'événement</param>
    public static void OnClosing( object sender, FormClosingEventArgs e ) {    // <wao code.&body>
      if (status != States.Started) 
        throw new EApplicationState( "OnClosing : status incorrect, " + status ) ;
      e.Cancel = false;
      status = States.Closing ;
      closingCount++;                                                          // <wao never>
      try { if (ApplicationClosing != null) ApplicationClosing( sender, e ) ; }
      finally { status = States.Started ; } 
    }
    	
    /// <summary>
    /// Permet d'indiquer que l'application doit se terminer.
    /// </summary>
    /// <remarks>
    /// Cette méthode est une méthode de "branchement" du protocole.
    /// <br/>
    /// Insérer un appel à cette méthode dans le gestionnaire de l'événement 
    /// <see cref="Form.FormClosed"/> de la fenêtre principale.  
    /// </remarks>
    /// <param name="sender">Objet émetteur de l'appel</param>
    /// <param name="e">Arguments de l'événement</param>
    public static void OnClose( object sender, FormClosedEventArgs e ) {       // <wao code.&body>
      if (status != States.Started) 
        throw new EApplicationState( "OnClose : status incorrect, " + status ) ;
      status = States.Close ;
      Application.Idle -= new EventHandler( OnIdle );
      if ( ApplicationClose  != null ) ApplicationClose ( sender, e ); // 1ère vague 
      if ( ApplicationClosed != null ) ApplicationClosed( sender, e ); // 2ème vague
    }
                                                                               // <wao branchement.end>
    #endregion

    #region Branchement du protocole depuis la classe Application

    /// <summary>
    /// Déclenchement de l'événement ApplicationIdle
    /// </summary>
    /// <remarks>
    /// C'est cette méthode que la classe ApplicationState abonne dans <see cref="OnOpen"/> 
    /// auprès de l'événement <see cref="Application.Idle"/>.
    /// <br/>
    /// Cette méthode inclut une sécurité sous la forme d'un appel à <see cref="Application.DoEvents"/>
    /// qui permet d'éviter une relance incessante de l'événement <see cref="Application.Idle"/>
    /// si des mises à jour incessantes sont produites par l'effet de la diffusion
    /// de l'événement.
    /// <br/>
    /// En outre, cette méthode intercepte les exceptions déclenchées pendant la diffusion
    /// de l'événement <see cref="ApplicationIdle"/> et pendant le traitement <see cref="Application.DoEvents"/>. 
    /// S'il y a au moins une méthode abonnée à l'événement <see cref="IdleException"/>, 
    /// l'exception est retransmise aux méthodes abonnées, sinon l'exception est retransmise 
    /// à l'événement <see cref="Application.ThreadException"/>.
    /// <br/>
    /// Cette interception des exceptions est motivée par le fait que les exceptions déclenchées
    /// pendant <see cref="Application.Idle"/> ne sont pas retransmises à l'événement <see cref="Application.ThreadException"/> 
    /// et provoquent l'arrêt de l'application. La retransmission des exceptions à l'événement <see cref="IdleException"/>
    /// permet éventuellement de filtrer les exceptions qui se reproduisent à chaque déclenchement
    /// de OnIdle. 
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments associés à l'événement</param>
    protected static void OnIdle( object sender, EventArgs e) {
      if (ApplicationIdle == null) return ;
      try {

        // diffuser l'événement ApplicationIdle
        ApplicationIdle( sender, e );

        // traiter les événements en attente
        Application.DoEvents();

      } catch ( Exception ex ) {
        if ( IdleException == null )
          Application.OnThreadException( ex );
        else
          IdleException( sender, new ThreadExceptionEventArgs( ex ) );
      }
    }

    #endregion
  }                                                                            // <wao code.&ender>
}
