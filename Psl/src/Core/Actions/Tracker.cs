using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Psl.Applications;

namespace Psl.Tracker {

  /// <summary>
  /// Fenêtre d'affichage de traces pour débogage en mode conception et en mode application
  /// </summary>
  public partial class Tracker : Form, ITrackerService {

    private static ITrackerService box = null;

    /// <summary>
    /// Obtient la fenêtre d'affichage (création par nécessité)
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
    /// <param name="service">objet à enregistrer</param>
    public static void RegisterTracker( ITrackerService service ) {

      if ( box != null ) box.StopService();
      if ( !Registry.Has( TrackerServiceKeys.KeyTrackerService ) ) Registry.Remove( box );
      box = null;

      if ( service == null ) return;
      Registry.Add( TrackerServiceKeys.KeyTrackerService, service );
      box = service;
    }

    /// <summary>
    /// Ajouter une trace à la fenêtre des traces
    /// </summary>
    /// <param name="message">trace à afficher</param>
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
    /// Arrêter le service
    /// </summary>
    void StopService();
  }

  /// <summary>
  /// Clés d'enregistrement du service de tracking
  /// </summary>
  public class TrackerServiceKeys {

    /// <summary>
    /// Clé d'enregistrement du service de tracking
    /// </summary>
    public const string KeyTrackerService = "Psl.Tracker.TracerService.service";
  } 

}