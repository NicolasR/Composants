/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 01 06 2010 : perfectionnement de la s�quence d'initialisation pour les cr�ations dynamiques
 * 01 06 2010 : adjonction de la m�thode RaiseArchive
 * 02 12 2010 : distinction de OnApplicationOpen et OnApplicationOpenHandler
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Psl.Applications {

	/// <summary>
	/// Composant d'assistance relayant les �v�nements g�n�raux d'une application.
	/// </summary>
  [
  Description( "Composant d'assistance relayant les �v�nements g�n�raux d'une application" )
  ]
	public class ApplicationEvents : System.ComponentModel.Component {

    private IArchiver archiver = null ;

    #region Initialisation / terminaison

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <remarks>
    /// Constructeur de r�f�rence.
    /// </remarks>
    /// <param name="container">Conteneur du composant</param>
		public ApplicationEvents(System.ComponentModel.IContainer container) {
			if (container != null) container.Add(this);

      // pas de raccordement aux �v�nements en mode conception
      if (DesignMode) return ;
      ApplicationState.ApplicationOpen      += OnApplicationOpenHandler ;
      ApplicationState.ApplicationOpened    += OnApplicationOpened      ;
      ApplicationState.ApplicationClosing   += OnApplicationClosing     ;
      ApplicationState.ApplicationClose     += OnApplicationClose       ;
      ApplicationState.ApplicationClosed    += OnApplicationClosed      ;
      ApplicationState.ApplicationIdle      += OnApplicationIdle        ;

      // compl�ment de raccordement pour les composants cr��s dynamiquement
      if ( !ApplicationState.IsCreate ) TryFinishSubscriptions();
    }

    /// <summary>
    /// Constructeur
    /// </summary>
    public ApplicationEvents() : this( null ) { }

    /// <summary>
    /// Compl�ter l'initialisation du composant
    /// </summary>
    /// <remarks>
    /// Dans la majeure partie des cas, l'archiveur est d�j� install� et enregistr� dans le
    /// registre d'application lorsque le constructeur d'un composant ApplicationEvents est
    /// invoqu�, de sorte que l'abonnement � l'archiveur pourrait �tre fait dans le constructeur.
    /// <br/>
    /// Il reste cependant le cas particulier de la fen�tre principale : si les plugins (dont l'archiveur)
    /// sont install�s dans le constructeur de la fen�tre fincipale, un composant ApplicationEvents
    /// d�pos� sur la fen�tre principale sera initialis� avant l'installation du plugin d'archivage.
    /// D'o� la n�cessit� de pr�voir un compl�ment d'installation dans l'�v�nement ApplicationOpen.
    /// <br/>
    /// Mais, de mani�re � faire en sorte que la fen�tre principale soit toujours la premi�re � 
    /// s'archiver, en particulier pour fixer les dimensions de la fen�tre principale, tous les
    /// composant ApplicationEvents s'abonnent � l'archiveur dans l'�v�nement ApplicationOpen de
    /// sorte qu'ils s'abonnent � l'archiveur dans l'ordre o� ils re�oivent l'�v�nement ApplicationOpen,
    /// qui est en principe l'ordre de leur cr�ation.
    /// <br/>
    /// Il reste toutefois le cas des composants qui sont cr��s dynamiquement, � entendre ici
    /// comme cr��s pendant ou apr�s le d�clenchement de la m�thode ApplicationState.OnOpen. 
    /// Pour ceux-l�, l'�v�nement ApplicationOpen ne se d�clenchera pas (il est en cours ou il 
    /// est d�j� pass�), de sorte qu'il faut compl�ter l'enregistrement dans le constructeur 
    /// (d'o� le test sur ApplicationState.IsCreate). 
    /// </remarks>
    private void TryFinishSubscriptions() {
      if ( archiver != null || !Registry.Has( MainKeys.KeyMainArchiver ) ) return;
      archiver = Registry.MainArchiver;
      archiver.Archive += new ArchiverEventHandler( OnArchive );
    }

    /// <summary>
    /// Terminaison du composant. 
    /// </summary>
    /// <remarks>
    /// En mode conception, il n'y a pas de raccordement aux �v�nements. 
    /// </remarks>
    private void DoUnSubscribe () {
      if (DesignMode) return ;

      // D�connexion ApplicationState
      ApplicationState.ApplicationOpen       -= OnApplicationOpenHandler ;
      ApplicationState.ApplicationOpened     -= OnApplicationOpened      ;
      ApplicationState.ApplicationClosing    -= OnApplicationClosing     ;
      ApplicationState.ApplicationClose      -= OnApplicationClose       ;
      ApplicationState.ApplicationClosed     -= OnApplicationClosed      ;
      ApplicationState.ApplicationIdle       -= OnApplicationIdle        ;

      // D�connexion Archiveur
      if (archiver != null) archiver.Archive -= new ArchiverEventHandler(OnArchive) ;
    }

    /// <summary> 
    /// Nettoyage des ressources utilis�es.
    /// </summary>
    /// <param name="disposing">true si les ressources non manag�es doivent �tre lib�r�es</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing ) {
        DoUnSubscribe();
      }
      base.Dispose( disposing );
    }

    private void OnApplicationOpenHandler( object sender, EventArgs e ) {
      TryFinishSubscriptions();
      OnApplicationOpen( sender, e );
    }

    #endregion

    #region Relais des abonnements

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationOpen"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="ApplicationState.ApplicationOpen"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationOpen( object sender, EventArgs e) {
      if (ApplicationOpen != null) ApplicationOpen( sender, e ) ;
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationOpened"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="ApplicationState.ApplicationOpened"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationOpened( object sender, EventArgs e) {
      if (ApplicationOpened != null) ApplicationOpened( sender, e ) ;
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationClosing"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="ApplicationState.ApplicationClosing"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationClosing( object sender, FormClosingEventArgs e) {
      if (ApplicationClosing != null) ApplicationClosing( sender, e ) ;
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationClose"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="ApplicationState.ApplicationClose"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationClose( object sender, FormClosedEventArgs e) {
      if (ApplicationClose != null) ApplicationClose( sender, e ) ;
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationClosed"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="ApplicationState.ApplicationClosed"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationClosed( object sender, FormClosedEventArgs e ) {
      if (ApplicationClosed != null) ApplicationClosed( sender, e ) ;
    }

    /// <summary>
    /// D�clenchement centralis� de l'�v�nement local <see cref="ApplicationIdle"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="Application.Idle"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'�v�nement</param>
    /// <param name="e">Arguments de l'�v�nements</param>
    protected virtual void OnApplicationIdle( object sender, EventArgs e ) {
      if (ApplicationIdle != null) ApplicationIdle( sender, e ) ;
    }

    /// <summary>
    /// D�clenche les abonn�s de l'�v�nement local <see cref="Archive"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'�v�nement <see cref="IArchiver.Archive"/> de l'archiveur principal.
    /// </remarks>
    /// <param name="sender">Archiveur �metteur</param>
    protected virtual void OnArchive( IArchiver sender ) {
      if (Archive != null) Archive( sender ) ;
    }

    #endregion

    #region Fonctionnalit�s expos�es

    /// <summary>
    /// D�clenche l'�v�nement <see cref="Archive"/> du composant.
    /// </summary>
    /// <remarks>
    /// Cette fonctionnalit� avanc�e est destin�e aux composants cr��s dynamiquement, 
    /// pendant ou apr�s la diffusion de l'�v�nement archive au niveau de l'application. 
    /// </remarks>
    /// <param name="forReading">true pour un archivage en lecture, false sinon</param>
    public void RaiseArchive( bool forReading ) {
      if ( archiver == null ) return;

      bool wasReading = archiver.IsReading;
      archiver.IsReading = forReading;
      try {
        OnArchive( archiver );
      } finally { archiver.IsReading = wasReading; }
    }

    #endregion

    #region Propri�t�s �v�nements

    /// <summary>
    /// Ev�nement ApplicationOpen
    /// </summary>
    [Description("Ev�nement d�clench� lors de l'initialisation de l'application (1�re vague)")]
    public event EventHandler ApplicationOpen ;

    /// <summary>
    /// Ev�nement ApplicationStart
    /// </summary>
    [Description("Ev�nement d�clench� lors de l'initialisation de l'application (2�me vague)")]
    public event EventHandler ApplicationOpened ;

    /// <summary>
    /// Ev�nement ApplicationClosing
    /// </summary>
    [Description("Ev�nement d�clench� lors de l'enqu�te de fermeture de l'application")]
    public event FormClosingEventHandler ApplicationClosing ;


    /// <summary>
    /// Ev�nement ApplicationClose
    /// </summary>
    [Description("Ev�nement d�clench� lors de la fermeture de l'application (1�re vague)")]
    public event FormClosedEventHandler ApplicationClose ;

    /// <summary>
    /// Ev�nement ApplicationClosed
    /// </summary>
    [Description("Ev�nement d�clench� lors de la fermeture de l'application (2�me vague)")]
    public event FormClosedEventHandler ApplicationClosed;

    /// <summary>
    /// Ev�nement ApplicationIdle
    /// </summary>
    [Description("Ev�nement d�clench� quand l'application devient idle")]
    public event EventHandler ApplicationIdle ;

    /// <summary>
    /// Ev�nement Archive
    /// </summary>
    [Description("Ev�nement d�clench� pour archivage")]
    public event ArchiverEventHandler Archive ;

    #endregion

	}
}
