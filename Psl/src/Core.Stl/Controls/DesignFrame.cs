/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 11 2008 : version initiale de DesignFrame
 * 10 10 2009 : séparation de DesignCapture et DesignFrame pour dégager la gestion de la sélection
 * 11 10 2009 : version pour l'automne 2009
 * 20 10 2010 : le contrôle sélectionné est aussi le contrôle actif du conteneur UserControl (sauf les combos)
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Psl.Windows;

namespace Psl.Controls {

  /// <summary>
  /// Esquisse d'une surface de conception simplifiée
  /// </summary>
  /// <remarks>
  /// Ce compsoant n'utilise pas de fenêtre transparente placée devant la surface de conception.
  /// Le cadre de sélection se trouve peint en-dessous des composants : dans les contrôles WF,
  /// les composants enfants sont peints avant le composant conteneur.
  /// </remarks>
  [ToolboxItem(true)]
  public partial class DesignFrame : DesignCapture {

    //
    // Indicateur de l'état courant relativement aux glissements
    //

    private enum DragState {
      None,      // au repos
      Watching,  // surveillance pour la détection d'une amorce de glissement
      Dragging,  // glissement ou redimensionnement en cours
    }

    //
    // Descripteur pour les régions du cadre de sélection courant
    //

    /// <summary>
    /// Structure regroupant des données associées aux régions de cadre de sélection
    /// </summary>
    private struct AreaData {

      // constructeur pour un rectangle
      public AreaData( SelectorArea area, Cursor cursor, int x, int y, int w, int h )
        : this() {
        Area = area;
        Cursor = cursor;
        Bounds = new Rectangle( x, y, w, h );
      }

      // constructeur pour une poignée
      public AreaData( SelectorArea direction, Cursor cursor, int handleSize, int x, int y )
        : this( direction, cursor, x, y, handleSize, handleSize ) {
      }

      // aire associée à la région
      public SelectorArea Area { get; set; }

      // rectangle associé à la région
      public Rectangle Bounds { get; set; }

      // curseur associé à la région
      public Cursor Cursor { get; set; }
    }

    //
    // Paramétrage du composant
    //

    // taille des poignées
    private const int HandleSize = 7;

    // nombre de points autour de l'impact souris pour la détectgion drag and drop
    private const int DragDetectArea = 1;

    // nom du format enregistré pour le drag and drop
    private const string DragFormatID = "Psl.Controls.DesignFrame.DragFormat";

    //
    // Options
    //

    // true si dessin automatique du cadre de sélection
    private bool autoSelect = true;

    // true si prise en charge automatique du glissage des contrôles
    private bool autoDrag = true;

    //
    // Sélection courante
    //

    // contrôle actuellement sélectionné ou null
    private Control selectedControl = null;

    // tableau de description des régions du cadre de sélection ajusté aux coordonnées du contrôle sélectionné
    private AreaData[] selectedAreas = new AreaData[ 11 ];

    // indexeur pour faciliter l'accès aux régions
    private AreaData this[ SelectorArea area ] {
      get { return selectedAreas[ (int) area ]; }
      set { selectedAreas[ (int) area ] = value; }
    }

    // indexeur pour faciliter l'accès aux régions
    private AreaData this[ int area ] {
      get { return selectedAreas[ area ]; }
      set { selectedAreas[ area ] = value; }
    }

    //
    // Gestion des glissements et des redimensionnements
    //

    // état courant relativement aux glissements et regimensionnements
    private DragState dragState = DragState.None;

    // coordonnées de l'impact souris qui commence la surveillance de la détection drag
    private Point dragHitLocation = new Point();

    // région associée à l'impact souris qui commence la surveillance de la détection drag
    private SelectorArea dragHitArea = SelectorArea.None;

    // différence entre la localisation du composant et l'impact souris initial
    private Size dragHitDelta = new Size();

    // dernière position souris prise en compte pendant un glissement/redimensionnement
    private Point dragLastLocation = new Point();

    //
    // Propriétés
    //

    // curseur déterminé par l'utilisateur du composant
    private Cursor userCursor = Cursors.Default;

    // curseur pour les opérations glisser/redimensionner
    private Cursor dragCursor = Cursors.Default;

    //
    // Gestion générale
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public DesignFrame() {
      InitializeComponent();
      DataFormats.GetFormat( DragFormat );
    }

    /// <summary> 
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      this.SuspendLayout();
      this.BackColor = System.Drawing.Color.Azure;
      this.CausesValidation = false;
      this.Name = "DesignFrame";
      this.Size = new System.Drawing.Size( 292, 271 );
      this.ResumeLayout( false );
    }

    /// <summary>
    /// Obtient le contrôle enfant à une position donnée en tenant compte des poignées de sélection
    /// </summary>
    /// <param name="location">position en coordonnées client</param>
    /// <returns>la référence sur le contrôle trouvé ou null</returns>
    public override Control GetChildAtPoint( Point location ) {
      if ( AreaContains( location ) ) return selectedControl;
      return base.GetChildAtPoint( location );
    }

    //
    // Adjonction/suppression des contrôles
    //

    /// <summary>
    /// Surveillance de la suppression des contrôles enfants pour tenir à jour le <see cref="SelectedControl"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnControlRemoved( ControlEventArgs e ) {
      if ( e.Control == SelectedControl ) SelectedControl = null;
      base.OnControlRemoved( e );
    }

    //
    // Gestion de la souris
    //

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris entre
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseEnter( EventArgs e ) {
      dragState = DragState.None;
      base.OnMouseEnter( e );
    }

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris sort
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseLeave( EventArgs e ) {
      dragState = DragState.None;
      base.OnMouseLeave( e );
    }

    /// <summary>
    /// Surveillance des événements souris : impact souris
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseDown( MouseEventArgs e ) {
      DragState oldState = dragState;
      try {
        if ( IsDesignMode ) return;

        SelectedControl = GetChildAtPoint( e.Location );

        if ( !autoDrag || e.Button != MouseButtons.Left ) return;
        dragState = DragState.Watching;
        dragHitLocation = e.Location;
      }
      finally {
        if ( oldState != dragState ) Invalidate();
        base.OnMouseDown( e );
      }
    }

    /// <summary>
    /// Surveillance des événements souris : lorsqu'un bouton est relâché
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseUp( MouseEventArgs e ) {
      DragState oldState = dragState;
      try {
        if ( IsDesignMode ) return;

        if ( e.Button != MouseButtons.Left ) return;
        dragState = DragState.None;
      }
      finally {
        if ( oldState != dragState ) Invalidate();
        base.OnMouseUp( e );
        if ( oldState == DragState.Dragging ) OnSelectedControlEndDrag();
      }
    }

    /// <summary>
    /// Surveillance des événements souris : lorsque la souris est déplacée
    /// </summary>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnMouseMove( MouseEventArgs e ) {
      DragState oldState = dragState;
      try {
        switch ( dragState ) {

          // surveillance de détection d'une amorce de glissement
          case DragState.Watching:
            Rectangle dragDetect = new Rectangle( Point.Subtract( dragHitLocation, new Size( DragDetectArea, DragDetectArea ) ), new Size( 2 * DragDetectArea, 2 * DragDetectArea ) );
            if ( dragDetect.Contains( e.Location ) ) return;

            dragState = DragState.None;
            Control control = GetChildAtPoint( dragHitLocation );
            if ( control == null ) return;
            SelectedControl = control;
            dragHitDelta = new Size( control.Location.X - dragHitLocation.X, control.Location.Y - dragHitLocation.Y );
            dragHitArea = AreaEnumOf( dragHitLocation );
            dragLastLocation = dragHitLocation;
            dragState = DragState.Dragging;
            OnDragDetect( new DragDetectEventArgs( e.Location, dragHitLocation, dragHitArea, dragHitDelta, control ) );
            break;

          // glissement ou redimensionnement en cours
          case DragState.Dragging:
            PerformDrag( e.Location );
            break;
        }
      }
      finally {
        DoUpdateCursor( e.Location, e.Button );
        if ( oldState != dragState ) Invalidate();
        base.OnMouseMove( e );
      }
    }

    //
    // Gestion du cadre de sélection
    //

    /// <summary>
    /// Ajuster les régions du cadre de sélection en fonction du contrôle sélectionné
    /// </summary>
    private void RefreshAreas() {
      if ( selectedControl == null ) return;

      int handleSize = HandleSize;
      int handleHalf = HandleSize / 2;

      int x = selectedControl.Location.X;
      int y = selectedControl.Location.Y;
      int w = selectedControl.Size.Width;
      int w2 = selectedControl.Size.Width / 2;
      int h = selectedControl.Size.Height;
      int h2 = selectedControl.Size.Height / 2;

      this[ SelectorArea.None ] = new AreaData( SelectorArea.None, Cursors.Default, 0, 0, 0, 0);

      this[ SelectorArea.HandleN ] = new AreaData( SelectorArea.HandleN, Cursors.SizeNS, handleSize, x + w2 - handleHalf, y - handleSize );
      this[ SelectorArea.HandleS ] = new AreaData( SelectorArea.HandleS, Cursors.SizeNS, handleSize, x + w2 - handleHalf, y + h );
      this[ SelectorArea.HandleW ] = new AreaData( SelectorArea.HandleW, Cursors.SizeWE, handleSize, x - handleSize, y + h2 - handleHalf );
      this[ SelectorArea.HandleE ] = new AreaData( SelectorArea.HandleE, Cursors.SizeWE, handleSize, x + w, y + h2 - handleHalf );

      this[ SelectorArea.HandleNW ] = new AreaData( SelectorArea.HandleNW, Cursors.SizeNWSE, handleSize, x - handleSize, y - handleSize );
      this[ SelectorArea.HandleNE ] = new AreaData( SelectorArea.HandleNE, Cursors.SizeNESW, handleSize, x + w, y - handleSize );
      this[ SelectorArea.HandleSE ] = new AreaData( SelectorArea.HandleNE, Cursors.SizeNWSE, handleSize, x + w, y + h );
      this[ SelectorArea.HandleSW ] = new AreaData( SelectorArea.HandleSW, Cursors.SizeNESW, handleSize, x - handleSize, y + h );

      this[ SelectorArea.Border ] = new AreaData( SelectorArea.Border, Cursors.SizeAll, x - handleHalf, y - handleHalf, w + handleSize, h + handleSize );
      this[ SelectorArea.Control ] = new AreaData( SelectorArea.Control, Cursors.SizeAll, x, y, w, h );
    }

    /// <summary>
    /// Obtient l'index (parallèle à l'énumération SelectorArea) de la région contenant la position 
    /// </summary>
    /// <param name="location">la localisation souris en coordonnées client</param>
    /// <returns>l'index trouvé ou -1</returns>
    private int AreaIndexOf( Point location ) {
      if ( selectedControl == null ) return -1;
      RefreshAreas();

      if ( !autoSelect )
        return this[ SelectorArea.Control ].Bounds.Contains( location ) ? (int) SelectorArea.Control : -1;

      for ( int ix = 1 ; ix <= 10 ; ix++ )
        if ( this[ ix ].Bounds.Contains( location ) ) return ix;
      return -1;
    }

    /// <summary>
    /// Obtient la valeur d'énumération SelectorArea de la région contenant la localisation
    /// </summary>
    /// <param name="location">la localisation souris en coordonnées client</param>
    /// <returns>la valeur d'énumération trouvée ou SelectorArea.None si non trouvée</returns>
    private SelectorArea AreaEnumOf( Point location ) {
      int index = AreaIndexOf( location );
      return index == -1 ? SelectorArea.None : (SelectorArea) index;
    }

    /// <summary>
    /// Indique si la localisation souris est contenue dans l'une des régions du cadre de sélection
    /// </summary>
    /// <param name="location">la localisation souris en coordonnées client</param>
    /// <returns>true si le cadre de sélection contient la localisation</returns>
    private bool AreaContains( Point location ) {
      return AreaIndexOf( location ) != -1;
    }

    /// <summary>
    /// Réalisation des glissements et des redimensionnements
    /// </summary>
    /// <param name="location">localisation actuelle de la souris en coordonnées client</param>
    private void PerformDrag( Point location ) {
      if ( location == dragLastLocation ) return;

      // éclatement des coordonnées du contrôle sélectionné
      int x = selectedControl.Location.X;
      int y = selectedControl.Location.Y;
      int w = selectedControl.Size.Width;
      int h = selectedControl.Size.Height;

      // déplacement par rapport à la dernière position souris prise en compte
      int dx = location.X - dragLastLocation.X;
      int dy = location.Y - dragLastLocation.Y;

      try {

        switch ( dragHitArea ) {

          // glissement sans redimensionnement
          case SelectorArea.Border:
          case SelectorArea.Control:
            x += dx;
            y += dy;
            return;
        }

        // redimensionnement, projection Nord
        switch ( dragHitArea ) {
          case SelectorArea.HandleN:
          case SelectorArea.HandleNE:
          case SelectorArea.HandleNW:
            y += dy;
            h -= dy;
            break;
        }

        // redimensionnement, projection Sud
        switch ( dragHitArea ) {
          case SelectorArea.HandleS:
          case SelectorArea.HandleSE:
          case SelectorArea.HandleSW:
            h += dy;
            break;
        }

        // redimensionnement, projection Ouest
        switch ( dragHitArea ) {
          case SelectorArea.HandleW:
          case SelectorArea.HandleNW:
          case SelectorArea.HandleSW:
            x += dx;
            w -= dx;
            break;
        }

        // redimensionnement, projection Est
        switch ( dragHitArea ) {
          case SelectorArea.HandleE:
          case SelectorArea.HandleNE:
          case SelectorArea.HandleSE:
            w += dx;
            break;
        }

      }
      finally {

        // mémorisation de la dernière position souris prise en compte
        dragLastLocation = location;

        // mise à jour du contrôle sléectionné
        selectedControl.SetBounds( x, y, w, h );
        Invalidate();
      }
    }

    //
    // Gestion du curseur
    //

    /// <summary>
    /// Mise à jour du curseur affiché
    /// </summary>
    private void DoUpdateCursor() {
      Cursor cursor = dragCursor == Cursors.Default ? userCursor : dragCursor;
      if ( base.Cursor == cursor ) return;
      base.Cursor = cursor;
    }

    /// <summary>
    /// Mise à jour du curseur en fonction de l'état courant
    /// </summary>
    /// <param name="location"></param>
    /// <param name="Button"></param>
    private void DoUpdateCursor( Point location, MouseButtons Button ) {
      int index = AreaIndexOf( location );
      dragCursor = index == -1 ? Cursors.Default : this[ index ].Cursor;
      DoUpdateCursor();
    }

    //
    // Peinture du cadre de sélection
    //

    /// <summary>
    /// Peinture du cadre de sélection
    /// </summary>
    /// <param name="graphics">contexte graphique de peinture</param>
    private void DrawSelector( Graphics graphics ) {
      DoUpdateCursor();
      if ( !autoSelect || selectedControl == null ) return;

      RefreshAreas();
      ControlPaint.DrawBorder( graphics, this[ SelectorArea.Border ].Bounds, Color.Gray, ButtonBorderStyle.Dotted );
      for ( int ix = 1 ; ix <= 8 ; ix++ )
        ControlPaint.DrawGrabHandle( graphics, this[ ix ].Bounds, dragState != DragState.Dragging, dragState != DragState.Watching );
    }

    /// <summary>
    /// Redéfinition pour l'affichage du cadre de sélection
    /// </summary>
    /// <param name="e">descripteur d'événements</param>
    protected override void OnPaint( PaintEventArgs e ) {
      base.OnPaint( e );
      DrawSelector( e.Graphics );
    }


    //
    // Déclenchement centralisé des événements
    //

    /// <summary>
    /// Déclencher l'événement <see cref="SelectedControlChanged"/>
    /// </summary>
    /// <param name="e">descrpteur de l'événement</param>
    protected virtual void OnSelectedControlChanged( EventArgs e ) {
      if ( SelectedControlChanged != null ) SelectedControlChanged( this, e );
    }

    /// <summary>
    /// Déclencher l'événement <see cref="DragDetect"/>
    /// </summary>
    /// <param name="e">descrpteur de l'événement</param>
    protected virtual void OnDragDetect( DragDetectEventArgs e ) {
      if ( DragDetect != null ) DragDetect( this, e );
    }

    /// <summary>
    /// Déclencher l'événement <see cref="SelectedControlEndDrag"/>
    /// </summary>
    protected virtual void OnSelectedControlEndDrag() {
      if ( SelectedControlEndDrag != null ) SelectedControlEndDrag( this, EventArgs.Empty );
    }

    //
    // Propriétés exposées
    //

    /// <summary>
    /// Obtient le nom du format personnalisé qui est enregistré pour le drag and drop
    /// </summary>
    [Browsable( false )]
    [Description( "Nom du format personnalisé enregistré pour le drag and drop" )]
    public string DragFormat {
      get { return DragFormatID; }
    }

    /// <summary>
    /// Indique si le contrôle courant est en cours de déplacement/redimensionnement
    /// </summary>
    [Browsable(false)]
    public bool IsDragging {
      get {
        return dragState == DragState.Dragging;
      }
    }

    /// <summary>
    /// Obtient ou détermine le contrôle actuellement sélectionné
    /// </summary>
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public Control SelectedControl {
      get {
        return selectedControl;
      }
      set {
        if ( selectedControl == value ) return;
        if ( value != null && !Controls.Contains( value ) ) throw new EDesignFrame( null, "Le contrôle nommé \"{0}\" de type \"{1}\" ne figure pas dans la liste des contrôles hébergés", value.Name, value.GetType().Name );

        selectedControl = value;
        ActiveControl = value;

        Invalidate();
        OnSelectedControlChanged( EventArgs.Empty );
      }
    }

    /// <summary>
    /// Obtient ou détermine si les bordures de sélection doivent être dessinées.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( true ),
    Description( "Obtient ou détermine si les bordures de sélection doivent être dessinées" )
    ]
    public bool AutoSelect {
      get { return autoSelect; }
      set {
        if ( autoSelect == value ) return;
        autoSelect = value;
        if ( selectedControl != null ) Invalidate();
      }
    }

    /// <summary>
    /// Obtient ou détermine si le déplacement et le redimensionnement des composants sont pris en charge automatiquement.
    /// </summary>
    [
    Browsable( true ),
    DefaultValue( true ),
    Description( "Obtient ou détermine si le déplacement et le redimensionnement des composants sont pris en charge automatiquement" )
    ]
    public bool AutoDrag {
      get { return autoDrag; }
      set {
        if ( autoDrag == value ) return;
        autoDrag = value;
        dragState = DragState.None;
      }
    }

    private bool ShouldSerializeCursor() {
      return userCursor != Cursors.Default;
    }

    /// <summary>
    /// Obtient ou détermine la forme du curseur souris
    /// </summary>
    [
    Browsable(true),
    Description( "Obtient ou détermine la forme du curseur souris"),
    ]
    public new Cursor Cursor {
      get { return userCursor; }
      set {
        if ( userCursor == value ) return;
        userCursor = value;
        DoUpdateCursor();
      }
    }

    //
    // Evénements exposés
    //

    /// <summary>
    /// Déclenché quand le <see cref="SelectedControl"/> change
    /// </summary>
    [Description( "Déclenché quand le SelectedControl change" )]
    public event EventHandler SelectedControlChanged;

    /// <summary>
    /// Déclenché quand une amorce de drag and drop a été détectée
    /// </summary>
    [Description( "Déclenché quand une amorce de drag and drop a été détectée" )]
    public event EventHandler<DragDetectEventArgs> DragDetect;

    /// <summary>
    /// Déclenché lorsque le déplacement ou le redimensionnement du contrôle courant est terminé
    /// </summary>
    [Description( "Déclenché lorsque le déplacement ou le redimensionnement du contrôle courant est terminé" )]
    public event EventHandler SelectedControlEndDrag ;
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                       Classe des exceptions                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe des exceptions du contrôle EDesignFrame
  /// </summary>
  public class EDesignFrame : Exception {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="inner">référence sur un objet exception original</param>
    /// <param name="text">message de l'exception à formatter</param>
    /// <param name="args">arguments du formattage</param>
    public EDesignFrame( Exception inner, string text, params object[] args ) : base( string.Format( text, args ), inner ) { }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                          Descripteur d'événement pour DragDetect                            //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Descripteur pour l'événement DragDetect
  /// </summary>
  public class DragDetectEventArgs : EventArgs {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="location">localisation actuelle de la souris en coordonnées client</param>
    /// <param name="startLocation">localisation en coordonnées client de l'impact initial</param>
    /// <param name="startArea">région du cadre de sélection concerné par l'impact initial</param>
    /// <param name="startDelta">différence entre la localisation du composant et l'impact souris initial</param>
    /// <param name="control">contrôle concerné par l'impact initial</param>
    public DragDetectEventArgs( Point location, Point startLocation, SelectorArea startArea, Size startDelta, Control control ) {
      Location = location;
      StartLocation = startLocation;
      StartArea = startArea;
      StartDelta = startDelta;
      Control = control;
    }

    /// <summary>
    /// Obtient la localisation en coordonnées client de l'impact initial
    /// </summary>
    public Point StartLocation { get; private set; }

    /// <summary>
    /// Obtient la région du cadre de sélection contenant l'impact initial
    /// </summary>
    public SelectorArea StartArea { get; private set; }

    /// <summary>
    /// Différence entre la localisation initiale du contrôle et la localisation de l'impact initial
    /// </summary>
    public Size StartDelta { get; private set; }

    /// <summary>
    /// Obtient la localisation actuelle de la souris en coordonnées client
    /// </summary>
    public Point Location { get; private set; }

    /// <summary>
    /// Obtient le contrôle concerné par l'impact initial
    /// </summary>
    public Control Control { get; private set; }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                          SelectorArea                                       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

                                                                               // <wao SelectorArea.begin>
  /// <summary>
  /// Indication des différentes régions d'un cadre de sélection
  /// </summary>
  public enum SelectorArea {

    /// <summary>
    /// Hors du cadre de sélection
    /// </summary>
    None,

    /// <summary>
    /// Poignée de redimensionnement Nord (bord supérieur)
    /// </summary>
    HandleN,

    /// <summary>
    /// Poignée de redimensionnement Sud (bord inférieur)
    /// </summary>
    HandleS,
    
    /// <summary>
    /// Poignée de redimensionnement Ouest (bord gauche)
    /// </summary>
    HandleW,

    /// <summary>
    /// Poignée de redimensionnement Est (bord droit)
    /// </summary>
    HandleE,

    /// <summary>
    /// Poignée de redimensionnement Nord-Est (bord supérieur et bord droit)
    /// </summary>
    HandleNE,

    /// <summary>
    /// Poignée de redimensionnement Sud-Ouest (bord inférieur et bord gauche)
    /// </summary>
    HandleSW,

    /// <summary>
    /// Poignée de redimensionnement Sud-Est (bord inférieur et bord droit)
    /// </summary>
    HandleSE,

    /// <summary>
    /// Poignée de redimensionnement Nord-Ouest (bord supérieur et bord gauche)
    /// </summary>
    HandleNW,

    /// <summary>
    /// Entre la bordure du cadre de sélection et le contrôle lui-même
    /// </summary>
    Border,

    /// <summary>
    /// Le contrôle lui-même
    /// </summary>
    Control,
  }                                                                            // <wao SelectorArea.end>

}
