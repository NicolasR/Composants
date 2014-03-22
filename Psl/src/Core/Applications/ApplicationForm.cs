/*
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 15 11 2008 : version initiale
 */

using System;
using System.Windows.Forms;

namespace Psl.Applications {

  // <exclude
  /// <summary>
  /// Classe de base pour la composition d'une fen�tre principale d'application.
  /// </summary>
  /// <remarks>
  /// Cette classe se borne � effectuer la connexion aux trois m�thodes qui pilotent
  /// le protocole d'application de la classe <see cref="ApplicationState"/>.
  /// <br/>
  /// Cette classe est destin�e � servir de classe de base � un concepteur de fen�tre.
  /// A ce titre elle sera ex�cut�e dans le contexte de l'ide en mode conception. 
  /// Les m�thodes sont donc toutes destin�es � �tre transparentes en mode conception.
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
    /// M�thode abonn�e � l'�v�nement <see cref="Application.EnterThreadModal"/> pour r�affichage.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se borne � d�clencher l'�v�nement <see cref="Application.Idle"/> pour provoquer
    /// un r�affichage �ventuel de l'ihm avant d'afficher une bo�te de dialogue modal.
    /// </remarks>
    /// <param name="sender">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected virtual void OnEnterThreadModal( object sender, EventArgs e ) {
      if ( !DesignMode ) Application.RaiseIdle( EventArgs.Empty );
    }

    /// <summary>
    /// Red�finit la m�thode Dispose(bool) permettant d'appeler la m�thode <see cref="OnDisposing"/> si l'argument est true.
    /// </summary>
    /// <remarks>
    /// La m�thode <see cref="OnDisposing"/> n'est pas invoqu�e en mode conception. 
    /// </remarks>
    /// <param name="disposing">true si les ressources manag�es doivent �tre supprim�es�; sinon, false.</param>
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
    /// <param name="e">Descripteur d'�v�nement</param>
    protected override void OnLoad( EventArgs e ) {
      base.OnLoad( e );
      if ( !DesignMode ) ApplicationState.OnOpen( this, e );
    }

    /// <summary>
    /// Connexion au protocole <see cref="ApplicationState"/> pour l'enqu�te de fermeture de l'application.
    /// </summary>
    /// <remarks>
    /// La connexion au protocole <see cref="ApplicationState"/> n'a pas lieu en mode conception.
    /// </remarks>
    /// <param name="e">Descripteur d'�v�nement</param>
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
    /// <param name="e">Descripteur d'�v�nement</param>
    protected override void OnFormClosed( FormClosedEventArgs e ) {
      if ( !DesignMode ) ApplicationState.OnClose( this, e );
      base.OnFormClosed( e );
    }

    /// <summary>
    /// M�thode de confort relayant simplement la m�thode Dispose(bool) quand l'argument est true.
    /// </summary>
    /// <remarks>
    /// Cette m�thode n'est pas d�clench�e en mode conception.
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
