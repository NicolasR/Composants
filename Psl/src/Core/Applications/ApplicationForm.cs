/*
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 11 2008 : version initiale
 */

using System;
using System.Windows.Forms;

namespace Psl.Applications {

  // <exclude
  /// <summary>
  /// Classe de base pour la composition d'une fenêtre principale d'application.
  /// </summary>
  /// <remarks>
  /// Cette classe se borne à effectuer la connexion aux trois méthodes qui pilotent
  /// le protocole d'application de la classe <see cref="ApplicationState"/>.
  /// <br/>
  /// Cette classe est destinée à servir de classe de base à un concepteur de fenêtre.
  /// A ce titre elle sera exécutée dans le contexte de l'ide en mode conception. 
  /// Les méthodes sont donc toutes destinées à être transparentes en mode conception.
  /// </remarks>
  public class ApplicationForm : Form {

    /// <summary>
    /// Constructeur
    /// </summary>
    public ApplicationForm() {
      if ( DesignMode ) return;
      Application.EnterThreadModal += OnEnterThreadModal;
    }

    /// <summary>
    /// Méthode abonnée à l'événement <see cref="Application.EnterThreadModal"/> pour réaffichage.
    /// </summary>
    /// <remarks>
    /// Cette méthode se borne à déclencher l'événement <see cref="Application.Idle"/> pour provoquer
    /// un réaffichage éventuel de l'ihm avant d'afficher une boîte de dialogue modal.
    /// </remarks>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnEnterThreadModal( object sender, EventArgs e ) {
      if ( !DesignMode ) Application.RaiseIdle( EventArgs.Empty );
    }

    /// <summary>
    /// Redéfinit la méthode Dispose(bool) permettant d'appeler la méthode <see cref="OnDisposing"/> si l'argument est true.
    /// </summary>
    /// <remarks>
    /// La méthode <see cref="OnDisposing"/> n'est pas invoquée en mode conception. 
    /// </remarks>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && !DesignMode ) {
        Application.EnterThreadModal -= OnEnterThreadModal;
        OnDisposing();
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// Connexion du protocole <see cref="ApplicationState"/> pour la fin de l'initialisation de l'application.
    /// </summary>
    /// <remarks>
    /// La connexion au protocole <see cref="ApplicationState"/> n'a pas lieu en mode conception.
    /// </remarks>
    /// <param name="e">Descripteur d'événement</param>
    protected override void OnLoad( EventArgs e ) {
      base.OnLoad( e );
      if ( !DesignMode ) ApplicationState.OnOpen( this, e );
    }

    /// <summary>
    /// Connexion au protocole <see cref="ApplicationState"/> pour l'enquête de fermeture de l'application.
    /// </summary>
    /// <remarks>
    /// La connexion au protocole <see cref="ApplicationState"/> n'a pas lieu en mode conception.
    /// </remarks>
    /// <param name="e">Descripteur d'événement</param>
    protected override void OnFormClosing( FormClosingEventArgs e ) {
      if ( !DesignMode ) ApplicationState.OnClosing( this, e );
      base.OnFormClosing( e );
    }

    /// <summary>
    /// Connexion au protocole <see cref="ApplicationState"/> pour la notification de la fermeture de l'application.
    /// </summary>
    /// <remarks>
    /// La connexion au protocole <see cref="ApplicationState"/> n'a pas lieu en mode conception.
    /// </remarks>
    /// <param name="e">Descripteur d'événement</param>
    protected override void OnFormClosed( FormClosedEventArgs e ) {
      if ( !DesignMode ) ApplicationState.OnClose( this, e );
      base.OnFormClosed( e );
    }

    /// <summary>
    /// Méthode de confort relayant simplement la méthode Dispose(bool) quand l'argument est true.
    /// </summary>
    /// <remarks>
    /// Cette méthode n'est pas déclenchée en mode conception.
    /// </remarks>
    protected virtual void OnDisposing() { }

    private void InitializeComponent() {
      this.SuspendLayout();
      // 
      // ApplicationForm
      // 
      this.ClientSize = new System.Drawing.Size( 292, 266 );
      this.Name = "ApplicationForm";
      this.ResumeLayout( false );

    }
  }
}
