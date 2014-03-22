/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 01 06 2010 : perfectionnement de la séquence d'initialisation pour les créations dynamiques
 * 01 06 2010 : adjonction de la méthode RaiseArchive
 * 02 12 2010 : distinction de OnApplicationOpen et OnApplicationOpenHandler
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Psl.Applications {

	/// <summary>
	/// Composant d'assistance relayant les événements généraux d'une application.
	/// </summary>
  [
  Description( "Composant d'assistance relayant les événements généraux d'une application" )
  ]
	public class ApplicationEvents : System.ComponentModel.Component {

    private IArchiver archiver = null ;

    #region Initialisation / terminaison

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <remarks>
    /// Constructeur de référence.
    /// </remarks>
    /// <param name="container">Conteneur du composant</param>
		public ApplicationEvents(System.ComponentModel.IContainer container) {
			if (container != null) container.Add(this);

      // pas de raccordement aux événements en mode conception
      if (DesignMode) return ;
      ApplicationState.ApplicationOpen      += OnApplicationOpenHandler ;
      ApplicationState.ApplicationOpened    += OnApplicationOpened      ;
      ApplicationState.ApplicationClosing   += OnApplicationClosing     ;
      ApplicationState.ApplicationClose     += OnApplicationClose       ;
      ApplicationState.ApplicationClosed    += OnApplicationClosed      ;
      ApplicationState.ApplicationIdle      += OnApplicationIdle        ;

      // complément de raccordement pour les composants créés dynamiquement
      if ( !ApplicationState.IsCreate ) TryFinishSubscriptions();
    }

    /// <summary>
    /// Constructeur
    /// </summary>
    public ApplicationEvents() : this( null ) { }

    /// <summary>
    /// Compléter l'initialisation du composant
    /// </summary>
    /// <remarks>
    /// Dans la majeure partie des cas, l'archiveur est déjà installé et enregistré dans le
    /// registre d'application lorsque le constructeur d'un composant ApplicationEvents est
    /// invoqué, de sorte que l'abonnement à l'archiveur pourrait être fait dans le constructeur.
    /// <br/>
    /// Il reste cependant le cas particulier de la fenêtre principale : si les plugins (dont l'archiveur)
    /// sont installés dans le constructeur de la fenêtre fincipale, un composant ApplicationEvents
    /// déposé sur la fenêtre principale sera initialisé avant l'installation du plugin d'archivage.
    /// D'où la nécessité de prévoir un complément d'installation dans l'événement ApplicationOpen.
    /// <br/>
    /// Mais, de manière à faire en sorte que la fenêtre principale soit toujours la première à 
    /// s'archiver, en particulier pour fixer les dimensions de la fenêtre principale, tous les
    /// composant ApplicationEvents s'abonnent à l'archiveur dans l'événement ApplicationOpen de
    /// sorte qu'ils s'abonnent à l'archiveur dans l'ordre où ils reçoivent l'événement ApplicationOpen,
    /// qui est en principe l'ordre de leur création.
    /// <br/>
    /// Il reste toutefois le cas des composants qui sont créés dynamiquement, à entendre ici
    /// comme créés pendant ou après le déclenchement de la méthode ApplicationState.OnOpen. 
    /// Pour ceux-là, l'événement ApplicationOpen ne se déclenchera pas (il est en cours ou il 
    /// est déjà passé), de sorte qu'il faut compléter l'enregistrement dans le constructeur 
    /// (d'où le test sur ApplicationState.IsCreate). 
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
    /// En mode conception, il n'y a pas de raccordement aux événements. 
    /// </remarks>
    private void DoUnSubscribe () {
      if (DesignMode) return ;

      // Déconnexion ApplicationState
      ApplicationState.ApplicationOpen       -= OnApplicationOpenHandler ;
      ApplicationState.ApplicationOpened     -= OnApplicationOpened      ;
      ApplicationState.ApplicationClosing    -= OnApplicationClosing     ;
      ApplicationState.ApplicationClose      -= OnApplicationClose       ;
      ApplicationState.ApplicationClosed     -= OnApplicationClosed      ;
      ApplicationState.ApplicationIdle       -= OnApplicationIdle        ;

      // Déconnexion Archiveur
      if (archiver != null) archiver.Archive -= new ArchiverEventHandler(OnArchive) ;
    }

    /// <summary> 
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources non managées doivent être libérées</param>
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
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationOpen"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="ApplicationState.ApplicationOpen"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationOpen( object sender, EventArgs e) {
      if (ApplicationOpen != null) ApplicationOpen( sender, e ) ;
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationOpened"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="ApplicationState.ApplicationOpened"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationOpened( object sender, EventArgs e) {
      if (ApplicationOpened != null) ApplicationOpened( sender, e ) ;
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationClosing"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="ApplicationState.ApplicationClosing"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationClosing( object sender, FormClosingEventArgs e) {
      if (ApplicationClosing != null) ApplicationClosing( sender, e ) ;
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationClose"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="ApplicationState.ApplicationClose"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationClose( object sender, FormClosedEventArgs e) {
      if (ApplicationClose != null) ApplicationClose( sender, e ) ;
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationClosed"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="ApplicationState.ApplicationClosed"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationClosed( object sender, FormClosedEventArgs e ) {
      if (ApplicationClosed != null) ApplicationClosed( sender, e ) ;
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement local <see cref="ApplicationIdle"/>.
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="Application.Idle"/>.
    /// </remarks>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événements</param>
    protected virtual void OnApplicationIdle( object sender, EventArgs e ) {
      if (ApplicationIdle != null) ApplicationIdle( sender, e ) ;
    }

    /// <summary>
    /// Déclenche les abonnés de l'événement local <see cref="Archive"/>. 
    /// </summary>
    /// <remarks>
    /// Handler de l'événement <see cref="IArchiver.Archive"/> de l'archiveur principal.
    /// </remarks>
    /// <param name="sender">Archiveur émetteur</param>
    protected virtual void OnArchive( IArchiver sender ) {
      if (Archive != null) Archive( sender ) ;
    }

    #endregion

    #region Fonctionnalités exposées

    /// <summary>
    /// Déclenche l'événement <see cref="Archive"/> du composant.
    /// </summary>
    /// <remarks>
    /// Cette fonctionnalité avancée est destinée aux composants créés dynamiquement, 
    /// pendant ou après la diffusion de l'événement archive au niveau de l'application. 
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

    #region Propriétés événements

    /// <summary>
    /// Evénement ApplicationOpen
    /// </summary>
    [Description("Evénement déclenché lors de l'initialisation de l'application (1ère vague)")]
    public event EventHandler ApplicationOpen ;

    /// <summary>
    /// Evénement ApplicationStart
    /// </summary>
    [Description("Evénement déclenché lors de l'initialisation de l'application (2ème vague)")]
    public event EventHandler ApplicationOpened ;

    /// <summary>
    /// Evénement ApplicationClosing
    /// </summary>
    [Description("Evénement déclenché lors de l'enquête de fermeture de l'application")]
    public event FormClosingEventHandler ApplicationClosing ;


    /// <summary>
    /// Evénement ApplicationClose
    /// </summary>
    [Description("Evénement déclenché lors de la fermeture de l'application (1ère vague)")]
    public event FormClosedEventHandler ApplicationClose ;

    /// <summary>
    /// Evénement ApplicationClosed
    /// </summary>
    [Description("Evénement déclenché lors de la fermeture de l'application (2ème vague)")]
    public event FormClosedEventHandler ApplicationClosed;

    /// <summary>
    /// Evénement ApplicationIdle
    /// </summary>
    [Description("Evénement déclenché quand l'application devient idle")]
    public event EventHandler ApplicationIdle ;

    /// <summary>
    /// Evénement Archive
    /// </summary>
    [Description("Evénement déclenché pour archivage")]
    public event ArchiverEventHandler Archive ;

    #endregion

	}
}
