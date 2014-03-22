/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 11 2008 : version initiale de DesignFrame
 * 10 10 2009 : séparation de DesignCapture et DesignFrame pour dégager la gestion de la sélection
 * 11 10 2009 : version pour l'automne 2009
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Psl.Windows;

namespace Psl.Controls {

  /// <summary>
  /// Gestion de la capture souris pour une surface de conception simplifiée
  /// </summary>
  /// <remarks>
  /// 
  /// </remarks>
  [ToolboxItem(false)]
  public class DesignCapture : UserControl, IMessageFilter {

    //
    // Champs
    //

    // message posté de rappel local pour restauration de la capture
    private const int DFM_RESTORECAPTURE = Win.WM_APP + 0x2000;

    // message posté de rappel local pour contourner le problème des ComboBox
    private const int DFM_CONTEXTMENUCLOSED = Win.WM_APP + 0x2001;

    // permet de savoir si le composant est utilisé ou non en mode conception
    private bool isDesignMode = true;

    //
    // Gestion générale
    //

    /// <summary>
    /// Redéfinition pour détecter le mode design
    /// </summary>
    /// <param name="e">descripteur d'événement</param>
    protected override void OnLoad( EventArgs e ) {
      base.OnLoad( e );
      IsDesignMode = (Site != null && Site.DesignMode) || TopLevelControl == null || (TopLevelControl.Site != null && TopLevelControl.Site.DesignMode);
      if ( IsDesignMode ) return;
      Application.AddMessageFilter( this );
    }

    /// <summary> 
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing /* && (components != null)*/ ) {
        if ( !IsDesignMode ) Application.RemoveMessageFilter( this );
        //components.Dispose();
      }
      base.Dispose( disposing );
    }

    /// <summary>
    /// Obtient le contrôle enfant à une position donnée
    /// </summary>
    /// <remarks>
    /// Redéclaration avec redéfinition pour ancrage de la gestion de la sélection
    /// </remarks>
    /// <param name="location">position en coordonnées client</param>
    /// <returns>la référence sur le contrôle trouvé ou null</returns>
    public new virtual Control GetChildAtPoint( Point location ) {
      return base.GetChildAtPoint( location );
    }

    //
    // Service
    //

    /// <summary>
    /// Retrouver un contrôle enfant à partir de son handle
    /// </summary>
    /// <param name="handle">handle du contrôle</param>
    /// <returns>la référence sur le contrôle ou null si introuvable</returns>
    private Control DoFindChildWithHandle( IntPtr handle ) {
      foreach ( Control child in Controls )
        if ( child.Handle == handle ) return child;
      return null;
    }

    /// <summary>
    /// Détermine si une coordonnée client est dans la surface de conception
    /// </summary>
    /// <param name="location">coordonnées client à examiner</param>
    /// <returns>true si les coordonnées client sont dans la surface de conception, false sinon</returns>
    private bool IsInside( Point location ) {
      return location.X >= 0 && location.Y >= 0 && location.X < Width && location.Y < Height;
    }

    /// <summary>
    /// Mise à jour du contrôle se trouvant à l'aplomb de la souris
    /// </summary>
    /// <param name="location">coordonnées client de la souris</param>
    private void DoUpdateHoverControl( Point location ) {
      Control hover = GetChildAtPoint( location );
      if ( HoverControl == hover ) return;
      HoverControl = hover;
      OnHoverControlChanged( EventArgs.Empty );
    }

    /// <summary>
    /// Libérer la capture souris
    /// </summary>
    public void DoReleaseMouseCapture() {
      if ( IsDesignMode ) return;

      if ( Win.GetCapture() == Handle ) 
        Win.ReleaseCapture();
    }

    /// <summary>
    /// Mise à jour de la capture souris
    /// </summary>
    /// <param name="location">coordonnées client de la souris</param>
    /// <param name="force">si true, forcer la capture même si la souris n'est pas dans la surface de conception</param>
    private void DoUpdateMouseCapture( Point location, bool force ) {
      if ( IsDesignMode ) return;

      if ( force || IsInside( location ) ) {
        if ( Win.GetCapture() != Handle )
          Win.SetCapture( Handle );
      }
      else
        DoReleaseMouseCapture();

      DoUpdateHoverControl( location );
    }

    /// <summary>
    /// Mise à jour de la capture souris en fonction de sa position courante
    /// </summary>
    /// <param name="force">si true, forcer la capture même si la souris n'est pas dans la surface de conception</param>
    public void DoUpdateMouseCapture( bool force ) {
      DoUpdateMouseCapture( PointToClient( MousePosition ), force );
    }

    /// <summary>
    /// Mise à jour de la capture souris 
    /// </summary>
    /// <param name="location">coordonnées client relativement auxquels effectuer la mise à jour</param>
    private void DoUpdateMouseCapture( Point location ) {
      DoUpdateMouseCapture( location, false );
    }

    //
    // Déclenchement centralisé des événements
    //

    /// <summary>
    /// Déclencher l'événement <see cref="HoverControlChanged"/>
    /// </summary>
    /// <param name="e">descrpteur de l'événement</param>
    protected virtual void OnHoverControlChanged( EventArgs e ) {
      if ( HoverControlChanged != null ) HoverControlChanged( this, e );
    }

    //
    // Hook de filtrage des événements souris au niveau de la pompe à messages
    //

    /// <summary>
    /// Filtre les messages Windows relatifs à la surface de conception et à ses composants enfants
    /// </summary>
    /// <param name="m">message windows à filtrer</param>
    /// <returns>true si le message doit être arrêté, false pour le laisser être diffusé</returns>
    bool IMessageFilter.PreFilterMessage( ref Message m ) {
      if ( !Visible ) return false;

      try {

        switch ( m.Msg ) {
          case Win.WM_MOUSEMOVE:
          case Win.WM_LBUTTONDOWN:
          case Win.WM_LBUTTONUP:
          case Win.WM_LBUTTONDBLCLK:
          case Win.WM_RBUTTONDOWN:
          case Win.WM_RBUTTONUP:
          case Win.WM_RBUTTONDBLCLK:

            if ( m.HWnd == Handle ) {
              Point pos = Win.Util.LParamToPoint( (int) m.LParam );

              if ( m.Msg == Win.WM_MOUSEMOVE )
                OnMouseMove( new MouseEventArgs( Control.MouseButtons, 0, pos.X, pos.Y, 0 ) );
              else if ( m.Msg == Win.WM_LBUTTONDOWN )
                OnMouseDown( new MouseEventArgs( MouseButtons.Left, 0, pos.X, pos.Y, 0 ) );
              else if ( m.Msg == Win.WM_LBUTTONUP ) {
                OnMouseUp( new MouseEventArgs( MouseButtons.Left, 0, pos.X, pos.Y, 0 ) );
                OnMouseClick( new MouseEventArgs( MouseButtons.Left, 0, pos.X, pos.Y, 0 ) );
              }
              else if ( m.Msg == Win.WM_LBUTTONDBLCLK )
                OnMouseDoubleClick( new MouseEventArgs( MouseButtons.Left, 0, pos.X, pos.Y, 0 ) );
              else if ( m.Msg == Win.WM_RBUTTONDOWN )
                OnMouseDown( new MouseEventArgs( MouseButtons.Right, 0, pos.X, pos.Y, 0 ) );
              else if ( m.Msg == Win.WM_RBUTTONUP )
                OnMouseUp( new MouseEventArgs( MouseButtons.Right, 0, pos.X, pos.Y, 0 ) );
              else if ( m.Msg == Win.WM_RBUTTONDBLCLK )
                OnMouseDoubleClick( new MouseEventArgs( MouseButtons.Right, 0, pos.X, pos.Y, 0 ) );
              return true;
            }

            Control child = DoFindChildWithHandle( m.HWnd );
            return child != null;
        }

        return false;
      }
      catch ( Exception ex ) {
        Application.OnThreadException( ex );
        return true;
      }
    }

    //
    // Surveillance de la souris
    //

    /// <summary>
    /// Redéfinition pour gérer le message de rappel permettant de restaurer la capture souris
    /// </summary>
    /// <param name="m"></param>
    protected override void WndProc( ref Message m ) {
      switch ( m.Msg ) {
        case DFM_RESTORECAPTURE:
          DoUpdateMouseCapture( true );
          break;
        case DFM_CONTEXTMENUCLOSED:
          //if ( ActiveControl != null && ActiveControl is ComboBox ) 
          //  Win.PostMessage( ActiveControl.Handle, Win.WM_KILLFOCUS, 0, 0 );
          DoUpdateMouseCapture( true );
          break;
      }
      base.WndProc( ref m );
    }

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris entre
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseEnter( EventArgs e ) {
      DoUpdateMouseCapture( true );
      base.OnMouseEnter( e );
    }

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris sort
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseLeave( EventArgs e ) {
      DoReleaseMouseCapture();
      base.OnMouseLeave( e );
    }

    /// <summary>
    /// Surveillance des événements souris : impact souris
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseDown( MouseEventArgs e ) {
      DoUpdateMouseCapture( e.Location );
      base.OnMouseDown( e );
    }

    /// <summary>
    /// Surveillance des événements souris : lorsqu'un bouton est relâché
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseUp( MouseEventArgs e ) {
      LastClickLocation = e.Location;

      base.OnMouseUp( e );
      if ( IsDesignMode ) return;

      if ( e.Button == MouseButtons.Right && ContextMenuStrip != null ) {
        DoReleaseMouseCapture();
        ContextMenuStrip.Closed += ContextMenuStrip_Closed;
        ContextMenuStrip.Show( this, e.Location );
      }
    }

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris est déplacée
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseMove( MouseEventArgs e ) {
      DoUpdateMouseCapture( e.Location );
      base.OnMouseMove( e );
    }

    /// <summary>
    /// Evénement abonné à l'événement Closed du menu contextuel (voir <see cref="OnMouseUp"/>)
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void ContextMenuStrip_Closed( object sender, ToolStripDropDownClosedEventArgs e ) {
      ContextMenuStrip.Closed -= ContextMenuStrip_Closed;
      Win.PostMessage( Handle, DFM_CONTEXTMENUCLOSED, 0, 0 );
    }

    /// <summary>
    /// Interception de l'événement DragDrop pour restaurer la capture souris
    /// </summary>
    /// <param name="drgevent">descripteur de l'événement</param>
    protected override void OnDragDrop( DragEventArgs drgevent ) {
      base.OnDragDrop( drgevent );
      Win.SendMessage( Handle, DFM_RESTORECAPTURE, 0, 0 );
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Indique si la surface de conception est ou non en mode conception sous l'ide de vsn
    /// </summary>
    [Browsable( false )]
    public bool IsDesignMode {
      get { return isDesignMode; }
      private set { isDesignMode = value; }
    }

    /// <summary>
    /// Obtient le contrôle actuellement présent sous la souris,
    /// null s'il n'y a aucun contrôle
    /// </summary>
    [Browsable( false )]
    public Control HoverControl { get; private set; }

    /// <summary>
    /// Obtient les coordonnées client du dernier clic souris sur la surface de conception
    /// </summary>
    [Browsable( false )]
    public Point LastClickLocation { get; private set; }

    /// <summary>
    /// Déclenché quand le <see cref="HoverControl"/> change
    /// </summary>
    [Description( "Déclenché quand le HoverControl change" )]
    public event EventHandler HoverControlChanged;
  }

}
