/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 21 01 2008 : version aihm 2007-2008 pour net 2.0
 * 29 11 2008 : normalisation des identificateurs
 * 17 01 2011 : la m�thode OnClosing bascule e.Cancel � false avant d�clenchement de l'�v�nement
 */                                                                            // <wao never.end>

using System;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

namespace Psl.Applications {

  /// <summary>
  /// Classe des exceptions produites par la gestion de l'�tat de l'application.
  /// </summary>
  public class EApplicationState : Exception {
    
    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="msg">Message associ� � l'objet exception</param>
    public EApplicationState( string msg ) : base( msg ) {}

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="msg">Message associ� � l'objet exception</param>
    /// <param name="inner">Object exception ayant cause l'exception</param>
    public EApplicationState( string msg, Exception inner ) : base( msg, inner ) { }
  }

  /// <summary>
  /// Classe des exceptions produites par la diffusion de l'�v�nement Idle de l'application.
  /// </summary>
  public class EApplicationStateIdle : EApplicationState {

    /// <summary>
    /// Constructeur.
    /// </summary>
    /// <param name="inner">objet exception intercept�</param>
    public EApplicationStateIdle( Exception inner ) 
      : base( 
        "Exception intercept�e pendant la diffusion de l'�v�nement ApplicationState.ApplicationIdle"
      + "\r\nHeure: " + DateTime.Now.ToLongTimeString(),
      inner
      ) { }
  }

  /// <summary>
  /// Reflet de l'�tat global de l'application dans ses grandes phases.
  /// </summary>
  /// <remarks>
  /// Cette classe est compos�e uniquement de membres de classes et ne doit
  /// jamais �tre instanci�e. 
  /// <br/>
  /// Elle est destin�e � tenir � jour un status qui correspond � l'�tat de l'application 
  /// relativement aux phases d'initialisation, de fonctionnement et de terminaison. 
  /// <br/>
  /// Le fonctionnement correct de cette classe suppose un "branchement" qui
  /// s'effectue en appelant, de mani�re appropri�e, les m�thodes 
  /// OnOpen, OnClosing OnClose.
  /// <br/>
  /// Les diff�rentes �tapes concernant l'�tat de l'application sont r�sum�es
  /// dans le sch�ma ci-dessous. 
  /// <br/>
  /// Les �tapes �l�mentaires sont m�moris�es telles quelles dans le champ 
  /// status et sont retourn�es sous la forme d'une constante via la propri�t� Status. 
  /// 
  /// Les �tapes synth�tiques (starting, running et finishing) sont reconstitu�es
  /// � partir des �tapes �l�mentaires.
  /// </remarks>
  public class ApplicationState {                                              // <wao code.&header>

    //     starting               ||        running                 |   finishing       // <wao table.begin>
    // Create |      Open         ||  Started  |       Closing      |    Close
    //        |                   ||           |                    | 
    //        OnOpen              ||           OnClosing            OnClose     
    //        * ApplicationOpen   ||           * ApplicationClosing * ApplicationClose
    //        * ApplicationOpened ||                                * ApplicationClosed
    // ___________________________||___________________________________________________
    //     constructeur de la     ||           m�thode Run de la classe 
    //      classe principale     ||       System.Windows.Forms.Application              // <wao table.end>

    private ApplicationState() { }                                                       // <wao never>

    #region Enum�ration des �tats de l'application
                                                                               // <wao cnv.begin>
    /// <summary>
    /// Enum�ration des �tats de l'application
    /// </summary>
    public enum States {                                                       // <wao strip.begin>
                                                                               
      /// <summary>
      /// Valeur �num�r�e indiquant que l'application est en cours d'initialisation
      /// </summary>
      Create,

      /// <summary>
      /// Valeur �num�r�e indiquant que l'application est dans sa deuxi�me phase d'initialisation
      /// </summary>
      Open,

      /// <summary>
      /// Valeur �num�r�e indiquant que l'application est en fonctionnement normal
      /// </summary>
      Started,

      /// <summary>
      /// Valeur �num�r�e indiquant que l'application proc�de � l'enqu�te de fermeture
      /// </summary>
      Closing,

      /// <summary>
      /// Valeur �num�r�e indiquant que l'application est en phase de terminaison
      /// </summary>
      Close
    }                                                                         // <wao cnv.end strip.end >
	
    #endregion

    #region Champs de classe

    /// <summary>
    /// M�morisation de l'�tat courant de l'application.
    /// </summary>
    private static States status = States.Create ;

    /// <summary>
    /// M�morise le nombre d'appels � <see cref="OnClosing"/>
    /// </summary>
    /// <remarks>
    /// Cette m�morisation est destin�e � la mise en oeuvre de hooks d'assistance 
    /// � la correction des projets d'�tudiants
    /// </remarks>
    private static int closingCount = 0;

    #endregion

    #region Consultation du status                                             // <wao code.&comgroup>
  
    /// <summary>
    /// Acc�s � l'�tat courant de l'application.
    /// </summary>
    public static States Status    {                                           // <wao code.&body:ro>
      get { return status ; } 
    }

    /// <summary>
    /// Obtient le nombre d'appels qui ont �t� effectu�s � <see cref="OnClosing"/>
    /// </summary>
    [EditorBrowsable( EditorBrowsableState.Never )]
    public static int ClosingCount {
      get { return closingCount; }
    }

    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsCreate    {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Create ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsOpen      {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Open   ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsStarted   {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Started ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsClosing   {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Closing ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsClose     {                                           // <wao xxxcode.&body:ro>
      get { return status == States.Close ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsStarting  {                                           // <wao xxxcode.&body:ro>
      get { return IsCreate || IsOpen ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsRunning   {                                           // <wao xxxcode.&body:ro>
      get { return IsStarted || IsClosing ; } 
    }
    
    /// <summary>
    /// Raccourci d'acc�s � l'�tat de l'application.
    /// </summary>
    public static bool IsFinishing {                                           // <wao xxxcode.&body:ro>
      get { return IsClose ; } 
    }
	
    #endregion

    #region Ev�nements expos�s                                            // <wao code.&comgroup>

    /// <summary>
    /// Ev�nement d�clench� lorsque l'application s'initialise (1�re vague).
    /// </summary>
    public static event EventHandler ApplicationOpen ;                         // <wao cnv>

    /// <summary>
    /// Ev�nement d�clench� lorsque l'application s'initialise (2�me vague).
    /// </summary>
    public static event EventHandler ApplicationOpened ;                       // <wao cnv>

    /// <summary>
    /// Ev�nement d�clench� pour enqu�te de fermeture.
    /// Un abonn� peut emp�cher la fermeture de l'application soit en basculant � false
    /// la propri�t� Cancel des arguments de l'�v�nement, soit en d�clenchant une exception ECancelled.
    /// </summary>
    public static event FormClosingEventHandler ApplicationClosing ;           // <wao cnv>

    /// <summary>
    /// Ev�n�ment d�clench� lorsque l'application se ferme effectivement (1�re vague)
    /// Cet �v�nement n'est d�clench� que si l'enqu�te de fermeture s'est conclu positivement. 
    /// </summary>
    public static event FormClosedEventHandler ApplicationClose ;              // <wao cnv>

    /// <summary>
    /// Ev�n�ment d�clench� lorsque l'application se ferme effectivement (2�me vague)
    /// Cet �v�nement n'est d�clench� que si l'enqu�te de fermeture s'est conclu positivement. 
    /// </summary>
    public static event FormClosedEventHandler ApplicationClosed;              // <wao cnv>

    /// <summary>
    /// Ev�nement d�clench� lorsque l'application est idle.
    /// </summary>
    /// <remarks>
    /// Cet �v�nement relaie l'�v�nement Idle de la classe Application standard.
    /// Son d�clenchement comporte une s�curit� (appel � DoEvents) pour �viter les
    /// risques des bouclages sur l'�v�nement idle si les mises � jour induites par
    /// cet �v�nement re-provoquent incessamment un �v�nement idle
    /// </remarks>
    public static event EventHandler ApplicationIdle ;

    /// <summary>
    /// Ev�nement d�clench� lorsqu'une exception a �t� intercept�e pendant l'application 
    /// de la m�thode ApplicationState.OnIdle.
    /// </summary>
    /// <remarks>
    /// Si aucune m�thode n'est abonn�e � cet �v�nement, les exceptions intercept�es par
    /// la m�thode ApplicationState.OnIdle sont retransmises � l'�v�nement ThreadException
    /// de la classe Application. 
    /// </remarks>
    public static event ThreadExceptionEventHandler IdleException;             // <wao cnv>

    #endregion

    #region Branchement du protocole                                           // <wao code.&comgroup>
                                                                               // <wao branchement.begin>
    /// <summary>
    /// Permet d'indiquer le moment o� l'application est initialis�e.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est une m�thode de "branchement" du protocole.
    /// <br/>
    /// Ins�rer un appel � cette m�thode � la fin du constructeur de la fen�tre principale.
    /// </remarks>
    /// <param name="sender">Objet �metteur de l'appel</param>
    /// <param name="e">Arguments de l'�v�nement</param>
    public static void OnOpen( object sender, EventArgs e ) {                  // <wao code.&body>
      if (status != States.Create) 
        throw new EApplicationState( "OnOpen : status incorrect, " + status ) ;
      status = States.Open ;
      try { 
        if (ApplicationOpen   != null) ApplicationOpen  ( sender, e ) ; // 1�re vague
        if (ApplicationOpened != null) ApplicationOpened( sender, e ) ; // 2�me vague
      } 
      finally { 
        status = States.Started ;
        Application.Idle += new EventHandler( OnIdle );
      }
    }
	
    /// <summary>
    /// Permet d'indiquer le moment o� l'application tente de se terminer.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est une m�thode de "branchement" du protocole.
    /// <br/>
    /// Ins�rer un appel � cette m�thode dans le gestionnaire de l'�v�nement 
    /// <see cref="Form.FormClosing"/> de la fen�tre principale.  
    /// </remarks>
    /// <param name="sender">Objet �metteur de l'appel</param>
    /// <param name="e">Arguments de l'�v�nement</param>
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
    /// Cette m�thode est une m�thode de "branchement" du protocole.
    /// <br/>
    /// Ins�rer un appel � cette m�thode dans le gestionnaire de l'�v�nement 
    /// <see cref="Form.FormClosed"/> de la fen�tre principale.  
    /// </remarks>
    /// <param name="sender">Objet �metteur de l'appel</param>
    /// <param name="e">Arguments de l'�v�nement</param>
    public static void OnClose( object sender, FormClosedEventArgs e ) {       // <wao code.&body>
      if (status != States.Started) 
        throw new EApplicationState( "OnClose : status incorrect, " + status ) ;
      status = States.Close ;
      Application.Idle -= new EventHandler( OnIdle );
      if ( ApplicationClose  != null ) ApplicationClose ( sender, e ); // 1�re vague 
      if ( ApplicationClosed != null ) ApplicationClosed( sender, e ); // 2�me vague
    }
                                                                               // <wao branchement.end>
    #endregion

    #region Branchement du protocole depuis la classe Application

    /// <summary>
    /// D�clenchement de l'�v�nement ApplicationIdle
    /// </summary>
    /// <remarks>
    /// C'est cette m�thode que la classe ApplicationState abonne dans <see cref="OnOpen"/> 
    /// aupr�s de l'�v�nement <see cref="Application.Idle"/>.
    /// <br/>
    /// Cette m�thode inclut une s�curit� sous la forme d'un appel � <see cref="Application.DoEvents"/>
    /// qui permet d'�viter une relance incessante de l'�v�nement <see cref="Application.Idle"/>
    /// si des mises � jour incessantes sont produites par l'effet de la diffusion
    /// de l'�v�nement.
    /// <br/>
    /// En outre, cette m�thode intercepte les exceptions d�clench�es pendant la diffusion
    /// de l'�v�nement <see cref="ApplicationIdle"/> et pendant le traitement <see cref="Application.DoEvents"/>. 
    /// S'il y a au moins une m�thode abonn�e � l'�v�nement <see cref="IdleException"/>, 
    /// l'exception est retransmise aux m�thodes abonn�es, sinon l'exception est retransmise 
    /// � l'�v�nement <see cref="Application.ThreadException"/>.
    /// <br/>
    /// Cette interception des exceptions est motiv�e par le fait que les exceptions d�clench�es
    /// pendant <see cref="Application.Idle"/> ne sont pas retransmises � l'�v�nement <see cref="Application.ThreadException"/> 
    /// et provoquent l'arr�t de l'application. La retransmission des exceptions � l'�v�nement <see cref="IdleException"/>
    /// permet �ventuellement de filtrer les exceptions qui se reproduisent � chaque d�clenchement
    /// de OnIdle. 
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments associ�s � l'�v�nement</param>
    protected static void OnIdle( object sender, EventArgs e) {
      if (ApplicationIdle == null) return ;
      try {

        // diffuser l'�v�nement ApplicationIdle
        ApplicationIdle( sender, e );

        // traiter les �v�nements en attente
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
