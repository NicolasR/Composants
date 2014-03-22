/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 29 10 2009 : version inititiale
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Psl.WindowsForms {

  /// <summary>
  /// Wrapper minimal de <see cref="HScrollBar"/> pour hébergement comme <see cref="ToolStripItem"/>.
  /// </summary>
  [
  DesignerCategory("code"),
  ToolStripItemDesignerAvailability( ToolStripItemDesignerAvailability.All )
  ]
  public class ToolStripHScrollBar : ToolStripControlHost {

    /// <summary>
    /// Initialisation des objets.
    /// </summary>
    public ToolStripHScrollBar()
      : base( CreateControlInstance() ) {
    }

    /// <summary>
    /// Initialisation des objets.
    /// </summary>
    public ToolStripHScrollBar( string name )
      : this() {
      base.Name = name;
    }

    private static Control CreateControlInstance() {
      return new HScrollBar();
    }

    /// <summary>
    /// Retrieves the size of a rectangular area into which a control can be fitted.
    /// </summary>
    /// <param name="constrainingSize">The custom-sized area for a control.</param>
    /// <returns>An ordered pair of type <see cref="T:System.Drawing.Size"></see> representing the width and height of a rectangle.</returns>
    public override Size GetPreferredSize( Size constrainingSize ) {
      return this.HScrollBar.GetPreferredSize( constrainingSize );
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ScrollBar.ValueChanged"/>.
    /// </summary>
    /// <param name="sender">émetteur de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    protected virtual void OnValueChanged( object sender, EventArgs e ) {
      if ( ValueChanged != null ) ValueChanged( sender, e );
    }

    /// <summary>
    /// Subscribes events from the hosted control.
    /// </summary>
    /// <param name="control">The control from which to subscribe events.</param>
    protected override void OnSubscribeControlEvents( Control control ) {
      base.OnSubscribeControlEvents( control );
      HScrollBar bar = control as HScrollBar;
      bar.ValueChanged += OnValueChanged;
    }

    /// <summary>
    /// Unsubscribes events from the hosted control.
    /// </summary>
    /// <param name="control">The control from which to unsubscribe events.</param>
    protected override void OnUnsubscribeControlEvents( Control control ) {
      base.OnUnsubscribeControlEvents( control );
      HScrollBar bar = control as HScrollBar;
      bar.ValueChanged -= OnValueChanged;
    }

    /// <summary>
    /// Référence sur le <see cref="HScrollBar"/> enveloppé.
    /// </summary>
    [
    RefreshProperties( RefreshProperties.All ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Description( "Référence sur le composant HScrollBar enveloppé" ),
    ]
    public HScrollBar HScrollBar {
      get { return Control as HScrollBar; }
    }

    /// <summary>
    /// Déclenché lorsque l'événement ValueChanged du composant HScrollBar enveloppé est déclenché. 
    /// </summary>
    [
    Description ( "Déclenché lorsque l'événement ValueChanged du composant HScrollBar enveloppé est déclenché" ),
    ]
    public event EventHandler ValueChanged;
  }
}
