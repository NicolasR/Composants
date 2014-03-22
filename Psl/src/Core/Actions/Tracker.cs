using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Tracker {

  /// <summary>
  /// Fen�tre d'affichage de traces pour d�bogage en mode conception et en mode application
  /// </summary>
  public partial class Tracker : Form, ITrackerService {

    private static ITrackerService box = null;

    /// <summary>
    /// Obtient la fen�tre d'affichage (cr�ation par n�cessit�)
    /// </summary>
    public static ITrackerService Box {
      get {
        if ( box != null ) return box;

        box = Registry.GetIf( TrackerServiceKeys.KeyTrackerService ) as ITrackerService;
        if ( box != null ) return box;

        if ( box == null ) box = new Tracker();
        return box;
      }
    }

    /// <summary>
    /// Enregistre un objet comme service courant de tracking
    /// </summary>
    /// <param name="service">objet � enregistrer</param>
    public static void RegisterTracker( ITrackerService service ) {

      if ( box != null ) box.StopService();
      if ( !Registry.Has( TrackerServiceKeys.KeyTrackerService ) ) Registry.Remove( box );
      box = null;

      if ( service == null ) return;
      Registry.Add( TrackerServiceKeys.KeyTrackerService, service );
      box = service;
    }

    /// <summary>
    /// Ajouter une trace � la fen�tre des traces
    /// </summary>
    /// <param name="message">trace � afficher</param>
    public static void Track( string message ) {      
      Box.Track( message );
    }

    /// <summary>
    /// Constructeur standard
    /// </summary>
    public Tracker() {
      InitializeComponent();
      ((ITrackerService) this).Track( "DesignMode : " + DesignMode );
      Show();
    }

    void ITrackerService.Track( string message ) {
      tracks.TopIndex = tracks.Items.Add( "" + DateTime.Now + " : " + message );
      tracks.Invalidate();
      tracks.Update();
    }

    void ITrackerService.StopService() {
      Registry.Remove( this );
      Close();
    }

    private void nettoyerToolStripMenuItem_Click( object sender, EventArgs e ) {
      tracks.Items.Clear();
    }

    private void Tracker_FormClosed( object sender, FormClosedEventArgs e ) {
      box = null;
    }
  }

  /// <summary>
  /// Interface du service de tracking
  /// </summary>
  public interface ITrackerService {

    /// <summary>
    /// Tracking
    /// </summary>
    /// <param name="message">message de tracking</param>
    void Track( string message );

    /// <summary>
    /// Arr�ter le service
    /// </summary>
    void StopService();
  }

  /// <summary>
  /// Cl�s d'enregistrement du service de tracking
  /// </summary>
  public class TrackerServiceKeys {

    /// <summary>
    /// Cl� d'enregistrement du service de tracking
    /// </summary>
    public const string KeyTrackerService = "Psl.Tracker.TracerService.service";
  } 

}