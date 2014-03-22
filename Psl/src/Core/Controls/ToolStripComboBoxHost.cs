/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * Crédit : Microsoft
 * 
 * 21 05 2009 : version initiale
 * 10 05 2010 : ré-exposition des propriétés et événements liés au protocole drag-and-drop
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using Psl;
using Psl.Windows;

//#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Controls {

  /// <summary>
  /// Wrapper ToolsStrip pour les boîtes combinées <see cref="System.Windows.Forms.ComboBox"/> et dérivées
  /// </summary>
  public partial class ToolStripComboBoxHost : ToolStripControlHost {

    // clés pour le relai des événements
    internal static readonly object EventDropDown = new object();
    internal static readonly object EventDropDownClosed = new object();
    internal static readonly object EventDropDownStyleChanged = new object();
    internal static readonly object EventSelectedIndexChanged = new object();
    internal static readonly object EventSelectionChangeCommitted = new object();
    internal static readonly object EventTextUpdate = new object();

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="control">référence sur le contrôle embarqué</param>
    public ToolStripComboBoxHost( ComboBox control )
      : base( control ) {
    }

    //
    // Service
    //

    /// <summary>
    /// Déclenche la diffusion d'un événement de clé <paramref name="key"/> à partir du catalogue des événements.
    /// </summary>
    /// <param name="key">clé de l'événement à déclencher</param>
    /// <param name="e">descripteur d'événement à transmettre</param>
    internal protected void RaiseEvent( object key, EventArgs e ) {
      EventHandler handler = (EventHandler) base.Events[ key ];
      if ( handler != null ) handler( this, e );
    }

    /// <summary>
    /// Accès au renderer du conteneur
    /// </summary>
    /// <remarks>
    /// Propriété déclarée internal dans ToolStripItem
    /// Sert dans l'adaptateur de la RichComboBox
    /// </remarks>
    public ToolStripRenderer Renderer {
      get { return Owner == null  ? null : Owner.Renderer; }
    }

    //
    // Mécanique de gestion du relai d'événements
    //

    private void HandleDropDown( object sender, System.EventArgs e ) {
      OnDropDown( e );
    }

    private void HandleDropDownClosed( object sender, System.EventArgs e ) {
      OnDropDownClosed( e );
    }

    private void HandleDropDownStyleChanged( object sender, System.EventArgs e ) {
      OnDropDownStyleChanged( e );
    }

    private void HandleSelectedIndexChanged( object sender, System.EventArgs e ) {
      OnSelectedIndexChanged( e );
    }

    private void HandleSelectionChangeCommitted( object sender, System.EventArgs e ) {
      OnSelectionChangeCommitted( e );
    }

    private void HandleTextUpdate( object sender, System.EventArgs e ) {
      OnTextUpdate( e );
    }

    /// <inheritdoc/>
    protected override void OnSubscribeControlEvents( Control control ) {
      ComboBox comboBox = control as ComboBox;
      if ( comboBox != null ) {
        comboBox.DropDown += new EventHandler( HandleDropDown );
        comboBox.DropDownClosed += new EventHandler( HandleDropDownClosed );
        comboBox.DropDownStyleChanged += new EventHandler( HandleDropDownStyleChanged );
        comboBox.SelectedIndexChanged += new EventHandler( HandleSelectedIndexChanged );
        comboBox.SelectionChangeCommitted += new EventHandler( HandleSelectionChangeCommitted );
        comboBox.TextUpdate += new EventHandler( HandleTextUpdate );
      }
      base.OnSubscribeControlEvents( control );
    }

    /// <inheritdoc/>
    protected override void OnUnsubscribeControlEvents( Control control ) {
      ComboBox comboBox = control as ComboBox;
      if ( comboBox != null ) {
        comboBox.DropDown -= new EventHandler( HandleDropDown );
        comboBox.DropDownClosed -= new EventHandler( HandleDropDownClosed );
        comboBox.DropDownStyleChanged -= new EventHandler( HandleDropDownStyleChanged );
        comboBox.SelectedIndexChanged -= new EventHandler( HandleSelectedIndexChanged );
        comboBox.SelectionChangeCommitted -= new EventHandler( HandleSelectionChangeCommitted );
        comboBox.TextUpdate -= new EventHandler( HandleTextUpdate );
      }
      base.OnUnsubscribeControlEvents( control );
    }

    //
    // Déclenchement centralisé des événements relayés
    //

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnDropDown" />
    protected virtual void OnDropDown( EventArgs e ) {
      RaiseEvent( EventDropDown, e );
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnDropDownClosed" />
    protected virtual void OnDropDownClosed( EventArgs e ) {
      RaiseEvent( EventDropDownClosed, e );
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnDropDownStyleChanged" />
    protected virtual void OnDropDownStyleChanged( EventArgs e ) {
      RaiseEvent( EventDropDownStyleChanged, e );
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnSelectedIndexChanged" />
    protected virtual void OnSelectedIndexChanged( EventArgs e ) {
      RaiseEvent( EventSelectedIndexChanged, e );
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnSelectionChangeCommitted" />
    protected virtual void OnSelectionChangeCommitted( EventArgs e ) {
      RaiseEvent( EventSelectionChangeCommitted, e );
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.OnTextUpdate" />
    protected virtual void OnTextUpdate( EventArgs e ) {
      RaiseEvent( EventTextUpdate, e );
    }

    //
    // Propriétés propres au wrapper
    //

    /// <summary>
    /// Obtient la référence sur la <see cref="ComboBox"/> embarquée. 
    /// </summary>
    [
     Browsable( false ), 
     DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public ComboBox ComboBox {
      get { return Control as ComboBox; }
    }

    //
    // Relai de méthodes
    //

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.BeginUpdate" />
    public void BeginUpdate() { ComboBox.BeginUpdate(); }


    /// <inheritdoc cref="System.Windows.Forms.ComboBox.EndUpdate"/>
    public void EndUpdate() { ComboBox.EndUpdate(); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.FindString(string)"/>
    public int FindString( string s ) { return ComboBox.FindString( s ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.FindString(string,int)"/>
    public int FindString( string s, int startIndex ) { return ComboBox.FindString( s, startIndex ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.FindStringExact(string)"/>
    public int FindStringExact( string s ) { return ComboBox.FindStringExact( s ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.FindStringExact(string,int)"/>
    public int FindStringExact( string s, int startIndex ) { return ComboBox.FindStringExact( s, startIndex ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.GetItemHeight"/>
    public int GetItemHeight( int index ) { return ComboBox.GetItemHeight( index ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.Select"/>
    public void Select( int start, int length ) { ComboBox.Select( start, length ); }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectAll"/>
    public void SelectAll() { ComboBox.SelectAll(); }

    /// <summary>
    /// Obtient une chaîne à partir de l'instance this.
    /// </summary>
    /// <returns>le nom du type du composant suivi du nombre d'éléments dans la liste déroulante</returns>
    public override string ToString() {
      return base.ToString() + ", Items.Count: " + Items.Count.ToString( CultureInfo.CurrentCulture );
    }

    //
    // Relai des propriétés
    //

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.AutoCompleteCustomSource" />
    [
    DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Localizable( true ),
    Editor( "System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
    Browsable( true ), EditorBrowsable( EditorBrowsableState.Always )
    ]
    public AutoCompleteStringCollection AutoCompleteCustomSource {
      get { return ComboBox.AutoCompleteCustomSource; }
      set { ComboBox.AutoCompleteCustomSource = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.AutoCompleteMode" />
    [
    DefaultValue( AutoCompleteMode.None ),
    Browsable( true ), EditorBrowsable( EditorBrowsableState.Always )
    ]
    public AutoCompleteMode AutoCompleteMode {
      get { return ComboBox.AutoCompleteMode; }
      set { ComboBox.AutoCompleteMode = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.AutoCompleteSource" />
    [
    DefaultValue( AutoCompleteSource.None ),
    Browsable( true ), EditorBrowsable( EditorBrowsableState.Always )
    ]
    public AutoCompleteSource AutoCompleteSource {
      get { return ComboBox.AutoCompleteSource; }
      set { ComboBox.AutoCompleteSource = value; }
    }

    /// <inheritdoc/>
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public override Image BackgroundImage {
      get { return base.BackgroundImage; }
      set { base.BackgroundImage = value; }
    }

    /// <inheritdoc/>
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )
    ]
    public override ImageLayout BackgroundImageLayout {
      get { return base.BackgroundImageLayout; }
      set { base.BackgroundImageLayout = value; }
    }

    /// <inheritdoc/>
    protected override Size DefaultSize {
      get { return new Size( 100, 22 ); }
    }

    /// <inheritdoc/>
    protected override Padding DefaultMargin {
      get {
        if ( IsOnDropDown ) {
          return new Padding( 2 );
        }
        else {
          return new Padding( 1, 0, 1, 0 );
        }
      }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDownHeight" />
    [
    Category( "Behavior" ),
    Browsable( true ), EditorBrowsable( EditorBrowsableState.Always ),
    DefaultValue( 106 )
    ]
    public int DropDownHeight {
      get { return ComboBox.DropDownHeight; }
      set { ComboBox.DropDownHeight = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DrawMode" />
    [
    Category( "Behavior" ),
    DefaultValue( RichComboBox.DefaultDrawMode ),
    RefreshPropertiesAttribute( RefreshProperties.Repaint ),
    Description( "Obtient ou détermine le mode de peinture du contrôle" ),
    ]
    public DrawMode DrawMode {
      get { return ComboBox.DrawMode; }
      set { ComboBox.DrawMode = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDownStyle" />
    [
    Category( "Appearance" ),
    DefaultValue( ComboBoxStyle.DropDown ),
    RefreshPropertiesAttribute( RefreshProperties.Repaint )
    ]
    public ComboBoxStyle DropDownStyle {
      get { return ComboBox.DropDownStyle; }
      set { ComboBox.DropDownStyle = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDownWidth" />
    [
    Category( "Behavior" )
    ]
    public int DropDownWidth {
      get { return ComboBox.DropDownWidth; }
      set { ComboBox.DropDownWidth = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DroppedDown" />
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public bool DroppedDown {
      get { return ComboBox.DroppedDown; }
      set { ComboBox.DroppedDown = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.FlatStyle" />
    [
    Category( "Appearance" ),
    DefaultValue( FlatStyle.Popup ),
    Localizable( true ),
    ]
    public FlatStyle FlatStyle {
      get { return ComboBox.FlatStyle; }
      set { ComboBox.FlatStyle = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.IntegralHeight" />
    [
    Category( "Behavior" ),
    DefaultValue( true ),
    Localizable( true ),
    ]
    public bool IntegralHeight {
      get { return ComboBox.IntegralHeight; }
      set { ComboBox.IntegralHeight = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.ItemHeight" />
    [
    Category( "Behavior" ),
    DefaultValue( 15 ),
    Description( "Obtient ou détermine la hauteur fixe (exprimée en pixels) d'un item" )
    ]
    public int ItemHeight {
      get { return ComboBox.ItemHeight; }
      set { ComboBox.ItemHeight = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.Items" />
    [
    Category( "Data" ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Content ),
    Localizable( true ),
    Editor( "System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof( UITypeEditor ) ),
    Description( "Obtient une référence sur la collection des éléments" ),
    ]
    public ComboBox.ObjectCollection Items {
      get { return ComboBox.Items; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.MaxDropDownItems" />
    [
    Category( "Behavior" ),
    DefaultValue( 8 ),
    Localizable( true ),
    ]
    public int MaxDropDownItems {
      get { return ComboBox.MaxDropDownItems; }
      set { ComboBox.MaxDropDownItems = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.MaxLength" />
    [
    Category( "Behavior" ),
    DefaultValue( 0 ),
    Localizable( true ),
    ]
    public int MaxLength {
      get { return ComboBox.MaxLength; }
      set { ComboBox.MaxLength = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectedIndex" />
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public int SelectedIndex {
      get { return ComboBox.SelectedIndex; }
      set { ComboBox.SelectedIndex = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectedItem" />
    [
    Browsable( false ),
    Bindable( true ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    Description( "Obtient ou détermine l'élément actuellement sélectionné" ),
    ]
    public object SelectedItem {
      get { return ComboBox.SelectedItem; }
      set { ComboBox.SelectedItem = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectedText" />
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public string SelectedText {
      get { return ComboBox.SelectedText; }
      set { ComboBox.SelectedText = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectionLength" />
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public int SelectionLength {
      get { return ComboBox.SelectionLength; }
      set { ComboBox.SelectionLength = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectionStart" />
    [
    Browsable( false ),
    DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden ),
    ]
    public int SelectionStart {
      get { return ComboBox.SelectionStart; }
      set { ComboBox.SelectionStart = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.Sorted" />
    [
    Category( "Behavior" ),
    DefaultValue( false ),
    ]
    public bool Sorted {
      get { return ComboBox.Sorted; }
      set { ComboBox.Sorted = value; }
    }

    //
    // Ré-exposition de propriété et d'événements
    // Déclarés [Browsable(false)] dans ToolStripItem
    //

    /// <inheritdoc cref="System.Windows.Forms.Control.AllowDrop" />
    [
    Browsable( true )
    ]
    public new bool AllowDrop {
      get { return ComboBox.AllowDrop; }
      set { ComboBox.AllowDrop = value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ToolStripItem.DragEnter" />
    [
    Browsable( true )
    ]
    public new event DragEventHandler DragEnter {
      add { base.DragEnter += value; }
      remove { base.DragEnter -= value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ToolStripItem.DragOver" />
    [
    Browsable( true )
    ]
    public new event DragEventHandler DragOver {
      add { base.DragOver += value; }
      remove { base.DragOver -= value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ToolStripItem.DragDrop" />
    [
    Browsable( true )
    ]
    public new event DragEventHandler DragDrop {
      add { base.DragDrop += value; }
      remove { base.DragDrop -= value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ToolStripItem.DragLeave" />
    [
    Browsable( true )
    ]
    public new event EventHandler DragLeave {
      add { base.DragLeave += value; }
      remove { base.DragLeave -= value; }
    }

    //
    // Relai des événements
    //

    /// <inheritdoc cref="System.Windows.Forms.ToolStripItem.DoubleClick" />
    [
    Browsable( false ),
    EditorBrowsable( EditorBrowsableState.Never )
    ]
    public new event EventHandler DoubleClick {
      add { base.DoubleClick += value; }
      remove { base.DoubleClick -= value; }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDown" />
    [
    Category( "Behavior" )
    ]
    public event EventHandler DropDown {
      add { Events.AddHandler( EventDropDown, value ); }
      remove { Events.RemoveHandler( EventDropDown, value ); }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDownClosed" />
    [
    Category( "Behavior" )
    ]
    public event EventHandler DropDownClosed {
      add { Events.AddHandler( EventDropDownClosed, value ); }
      remove { Events.RemoveHandler( EventDropDownClosed, value ); }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.DropDownStyleChanged" />
    [
    Category( "Behavior" )
    ]
    public event EventHandler DropDownStyleChanged {
      add { Events.AddHandler( EventDropDownStyleChanged, value ); }
      remove { Events.RemoveHandler( EventDropDownStyleChanged, value ); }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.SelectedIndexChanged" />
    [
    Category( "Behavior" )
    ]
    public event EventHandler SelectedIndexChanged {
      add { Events.AddHandler( EventSelectedIndexChanged, value ); }
      remove { Events.RemoveHandler( EventSelectedIndexChanged, value ); }
    }

    /// <inheritdoc cref="System.Windows.Forms.ComboBox.TextUpdate" />
    [
    Category( "Behavior" )
    ]
    public event EventHandler TextUpdate {
      add { Events.AddHandler( EventTextUpdate, value ); }
      remove { Events.RemoveHandler( EventTextUpdate, value ); }
    }

    /// <inheritdoc/>
    public override Size GetPreferredSize( Size constrainingSize ) {
      Size preferredSize = base.GetPreferredSize( constrainingSize );
      preferredSize.Width = Math.Max( preferredSize.Width, 75 );
      return preferredSize;
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                               Composant ToolStripComboBoxHost                               //
  //                                      Flat Combo adapter                                     //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class ToolStripComboBoxHost {

    /// <summary>
    /// Adaptateur pour la peinture des <see cref="ComboBox"/> en style plat.
    /// </summary>
    protected class FlatComboAdapter {

      private const int WhiteFillRectWidth = 5; // used for making the button look smaller than it is 

      /// <summary>
      /// Référence sur la <see cref="ComboBox"/> à peindre.
      /// </summary>
      protected ComboBox comboBox;

      private Rectangle clientRect;
      RightToLeft origRightToLeft; // The combo box's RTL value when we were created 

      private Rectangle outerBorder;
      private Rectangle innerBorder;
      private Rectangle innerInnerBorder;

      /// <summary>
      /// Rectangle associé au bouton drop-down
      /// </summary>
      protected Rectangle dropDownRect;

      /// <summary>
      /// Rectangle à remplir
      /// </summary>
      protected Rectangle whiteFillRect;

      /// <summary>
      ///  Constructeur
      /// </summary>
      /// <param name="comboBox">référence sur la combo à peindre</param>
      /// <param name="smallButton">si true, dessiner un petit bouton pour le drop-down</param>
      public FlatComboAdapter( ComboBox comboBox, bool smallButton ) {
        this.comboBox = comboBox;

        clientRect = comboBox.ClientRectangle;
        origRightToLeft = comboBox.RightToLeft;

        int dropDownButtonWidth = System.Windows.Forms.SystemInformation.HorizontalScrollBarArrowWidth;
        outerBorder = new Rectangle( clientRect.Location, new Size( clientRect.Width - 1, clientRect.Height - 1 ) );
        innerBorder = new Rectangle( outerBorder.X + 1, outerBorder.Y + 1, outerBorder.Width - dropDownButtonWidth - 2, outerBorder.Height - 2 );
        innerInnerBorder = new Rectangle( innerBorder.X + 1, innerBorder.Y + 1, innerBorder.Width - 2, innerBorder.Height - 2 );
        dropDownRect = new Rectangle( innerBorder.Right + 1, innerBorder.Y, dropDownButtonWidth, innerBorder.Height + 1 );

        // fill in several pixels of the dropdown rect with white so that it looks like the combo button is thinner.
        if ( smallButton ) {
          whiteFillRect = dropDownRect;
          whiteFillRect.Width = WhiteFillRectWidth;
          dropDownRect.X += WhiteFillRectWidth;
          dropDownRect.Width -= WhiteFillRectWidth;
        }

        if ( origRightToLeft == RightToLeft.Yes ) {
          innerBorder.X = clientRect.Width - innerBorder.Right;
          innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
          dropDownRect.X = clientRect.Width - dropDownRect.Right;
          whiteFillRect.X = clientRect.Width - whiteFillRect.Right + 1;  // since we're filling, we need to move over to the next px.
        }
      }

      /// <summary>
      /// Détermine si la peinture actuelle est valide (pas de changement d'orientation intervenu)
      /// </summary>
      /// <param name="combo">référence sur la <see cref="ComboBox"/> concernée</param>
      /// <returns>true si la peinture actuelle est valide</returns>
      public bool IsValid( ComboBox combo ) {
        return ( combo.ClientRectangle == clientRect && combo.RightToLeft == origRightToLeft );
      }

      /// <summary>
      /// Détermine si le rectangle a l'une au moins de ses dimensions égale à 0
      /// </summary>
      /// <param name="rectangle">rectangle concerné</param>
      /// <returns>true si l'une au moins des dimensions est égale à 0</returns>
      public bool IsZeroWidthOrHeight( Rectangle rectangle ) {
        if ( rectangle.Width != 0 ) {
          return ( rectangle.Height == 0 );
        }
        return true;
      }

      /// <summary>
      /// Retourne true si la souris est au-dessus de la combo
      /// </summary>
      protected bool MouseIsOver {
        get {
          return comboBox.ClientRectangle.Contains( comboBox.PointToClient( Control.MousePosition ) );
        }
      }

      /// <summary>
      /// Peint la combo en style plat
      /// </summary>
      /// <param name="g">référence sur le contexte graphique</param>
      public virtual void DrawFlatCombo( Graphics g ) {

        if ( comboBox.DropDownStyle == ComboBoxStyle.Simple ) {
          return;
        }

        Color outerBorderColor = GetOuterBorderColor();
        Color innerBorderColor = GetInnerBorderColor();
        bool rightToLeft = comboBox.RightToLeft == RightToLeft.Yes;

        // draw the drop down 
        DrawFlatComboDropDown( g );

        // when we are disabled there is one line of color that seems to eek through if backcolor is set
        // so lets erase it. 
        if ( !IsZeroWidthOrHeight( whiteFillRect ) ) {
          // fill in two more pixels with white so it looks smaller. 
          using ( Brush b = new SolidBrush( innerBorderColor ) ) {
            g.FillRectangle( b, whiteFillRect );
          }
        }

        // Draw the outer border
        if ( outerBorderColor.IsSystemColor ) {
          Pen outerBorderPen = SystemPens.FromSystemColor( outerBorderColor );
          g.DrawRectangle( outerBorderPen, outerBorder );
          if ( rightToLeft ) {
            g.DrawRectangle( outerBorderPen, new Rectangle( outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height ) );
          }
          else {
            g.DrawRectangle( outerBorderPen, new Rectangle( dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height ) );
          }
        }
        else {
          using ( Pen outerBorderPen = new Pen( outerBorderColor ) ) {
            g.DrawRectangle( outerBorderPen, outerBorder );
            if ( rightToLeft ) {
              g.DrawRectangle( outerBorderPen, new Rectangle( outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height ) );
            }
            else {
              g.DrawRectangle( outerBorderPen, new Rectangle( dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height ) );
            }
          }
        }

        // Draw the inner border
        if ( innerBorderColor.IsSystemColor ) {
          Pen innerBorderPen = SystemPens.FromSystemColor( innerBorderColor );
          g.DrawRectangle( innerBorderPen, innerBorder );
          g.DrawRectangle( innerBorderPen, innerInnerBorder );
        }
        else {
          using ( Pen innerBorderPen = new Pen( innerBorderColor ) ) {
            g.DrawRectangle( innerBorderPen, innerBorder );
            g.DrawRectangle( innerBorderPen, innerInnerBorder );
          }
        }

        // Draw a dark border around everything if we're in popup mode 
        if ( ( !comboBox.Enabled ) || ( comboBox.FlatStyle == FlatStyle.Popup ) ) {
          bool focused = comboBox.ContainsFocus || MouseIsOver;
          Color borderPenColor = GetPopupOuterBorderColor( focused );

          using ( Pen borderPen = new Pen( borderPenColor ) ) {

            Pen innerPen = ( comboBox.Enabled ) ? borderPen : SystemPens.Control;

            // around the dropdown 
            if ( rightToLeft ) {
              g.DrawRectangle( innerPen, new Rectangle( outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height ) );
            }
            else {
              g.DrawRectangle( innerPen, new Rectangle( dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height ) );
            }

            // around the whole combobox. 
            g.DrawRectangle( borderPen, outerBorder );

          }
        }
      }

      /// <summary>
      /// Peint le bouton drop-down en style plat
      /// </summary>
      /// <param name="g">référence sur le contexte graphique</param>
      public virtual void DrawFlatComboDropDown( Graphics g ) {

        g.FillRectangle( SystemBrushes.Control, dropDownRect );

        Brush brush = ( comboBox.Enabled ) ? SystemBrushes.ControlText : SystemBrushes.ControlDark;

        Point middle = new Point( dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2 );
        if ( origRightToLeft == RightToLeft.Yes ) {
          // if the width is odd - favor pushing it over one pixel left. 
          middle.X -= ( dropDownRect.Width % 2 );
        }
        else {
          // if the width is odd - favor pushing it over one pixel right. 
          middle.X += ( dropDownRect.Width % 2 );
        }

        g.FillPolygon( brush, new Point[] {
                     new Point(middle.X - 2, middle.Y - 1), 
                     new Point(middle.X + 3, middle.Y - 1),
                     new Point(middle.X, middle.Y + 2)
                 } );
      }

      /// <summary>
      /// Obtient la couleur de la bordure extérieure
      /// </summary>
      /// <returns>la couleur de la bordure extérieure</returns>
      protected virtual Color GetOuterBorderColor() {
        return ( comboBox.Enabled ) ? SystemColors.Window : SystemColors.ControlDark;
      }

      /// <summary>
      /// Obtient la couleur de la bordure extérieure notifiant le focus
      /// </summary>
      /// <param name="focused">true si la combo a le focus</param>
      /// <returns>la couleur de la bordure extérieure notifiant le focus</returns>
      protected virtual Color GetPopupOuterBorderColor( bool focused ) {
        if ( !comboBox.Enabled ) {
          return SystemColors.ControlDark;
        }
        return ( focused ) ? SystemColors.ControlDark : SystemColors.Window;
      }

      /// <summary>
      /// Obtient la couleur de la bordure intérieure
      /// </summary>
      /// <returns>la couleur de la bordure intérieure</returns>
      protected virtual Color GetInnerBorderColor() {
        return ( comboBox.Enabled ) ? comboBox.BackColor : SystemColors.Control;
      }

      // this eliminates flicker by removing the pieces we're going to paint ourselves from
      // the update region.  Note the UpdateRegionBox is the bounding box of the actual update region. 
      // this is just here so we can quickly eliminate rectangles that arent in the update region.
      /// <summary>
      /// Valide les régions de peinture concernées par la peinture personnalisées pour éviter le flickering
      /// </summary>
      /// <param name="updateRegionBox">rectangle à mettre à jour</param>
      public void ValidateOwnerDrawRegions( Rectangle updateRegionBox ) {
        Win.RECT validRect;

        Rectangle topOwnerDrawArea = new Rectangle( 0, 0, comboBox.Width, innerBorder.Top );
        Rectangle bottomOwnerDrawArea = new Rectangle( 0, innerBorder.Bottom, comboBox.Width, comboBox.Height - innerBorder.Bottom );
        Rectangle leftOwnerDrawArea = new Rectangle( 0, 0, innerBorder.Left, comboBox.Height );
        Rectangle rightOwnerDrawArea = new Rectangle( innerBorder.Right, 0, comboBox.Width - innerBorder.Right, comboBox.Height );

        if ( topOwnerDrawArea.IntersectsWith( updateRegionBox ) ) {
          validRect = new Win.RECT( topOwnerDrawArea );
          Win.ValidateRect( comboBox.Handle, ref validRect );
        }

        if ( bottomOwnerDrawArea.IntersectsWith( updateRegionBox ) ) {
          validRect = new Win.RECT( bottomOwnerDrawArea );
          Win.ValidateRect( comboBox.Handle, ref validRect );
        }

        if ( leftOwnerDrawArea.IntersectsWith( updateRegionBox ) ) {
          validRect = new Win.RECT( leftOwnerDrawArea );
          Win.ValidateRect( comboBox.Handle, ref validRect );
        }

        if ( rightOwnerDrawArea.IntersectsWith( updateRegionBox ) ) {
          validRect = new Win.RECT( rightOwnerDrawArea );
          Win.ValidateRect( comboBox.Handle, ref validRect );
        }
      }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                               Composant ToolStripComboBoxHost                               //
  //                                 Flat Combo ToolsStrip adapter                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  partial class ToolStripComboBoxHost {

    /// <summary>
    /// Adaptateur pour la peinture en style plat des <see cref="ComboBox"/> embarquées.
    /// </summary>
    protected class FlatComboAdapterStrip : FlatComboAdapter {

      /// <summary>
      /// Référence sur le <see cref="ToolStripItem"/> propriétaire.
      /// </summary>
      protected ToolStripItem owner;

      /// <summary>
      /// Constructeur
      /// </summary>
      /// <param name="comboBox">référence sur la <see cref="ComboBox"/> embarquée à peindre</param>
      /// <param name="owner">référence sur le <see cref="ToolStripItem"/> propriétaire</param>
      public FlatComboAdapterStrip( ComboBox comboBox, ToolStripItem owner )
        : base( comboBox, true ) {
        this.owner = owner;
      }

      private bool UseBaseAdapter() {
        return GetColorTable() == null;
      }

      private ProfessionalColorTable ColorTable {
        get {
          if ( owner == null ) return null;
          ToolStrip owner2 = owner.Owner;
          if ( owner2 == null ) return null;
          ToolStripProfessionalRenderer renderer = owner2.Renderer as ToolStripProfessionalRenderer;
          if ( renderer == null ) return null;
          return renderer.ColorTable;
        }
      }

      private ProfessionalColorTable GetColorTable() {
        return ColorTable;
      }

      /// <inheritdoc/>
      protected override Color GetOuterBorderColor() {
        if ( UseBaseAdapter() ) {
          return base.GetOuterBorderColor();
        }
        return ( comboBox.Enabled ) ? SystemColors.Window : ComboBoxBorder( GetColorTable() );
      }

      /// <inheritdoc/>
      protected override Color GetPopupOuterBorderColor( bool focused ) {
        if ( UseBaseAdapter() ) {
          return base.GetPopupOuterBorderColor( focused );
        }
        if ( !comboBox.Enabled ) {
          return SystemColors.ControlDark;
        }
        return ( focused ) ? ComboBoxBorder( GetColorTable() ) : SystemColors.Window;
      }

      /// <inheritdoc/>
      public override void DrawFlatComboDropDown( Graphics g ) {

        if ( UseBaseAdapter() ) {
          base.DrawFlatComboDropDown( g );
          return;
        }

        ProfessionalColorTable colorTable = GetColorTable();

        if ( colorTable == null ) {
          base.DrawFlatComboDropDown( g );
          return;
        }

        if ( !comboBox.Enabled || !ToolStripManager.VisualStylesEnabled ) {
          g.FillRectangle( SystemBrushes.Control, dropDownRect );
        }
        else {

          if ( !comboBox.DroppedDown ) {
            bool focused = comboBox.ContainsFocus || MouseIsOver;
            if ( focused ) {
              using ( Brush b = new LinearGradientBrush( dropDownRect, ComboBoxButtonSelectedGradientBegin( colorTable ), ComboBoxButtonSelectedGradientEnd( colorTable ), LinearGradientMode.Vertical ) ) {
                g.FillRectangle( b, dropDownRect );
              }
            }
            else if ( owner.IsOnOverflow ) {
              using ( Brush b = new SolidBrush( ComboBoxButtonOnOverflow( colorTable ) ) ) {
                g.FillRectangle( b, dropDownRect );
              }
            }
            else {
              using ( Brush b = new LinearGradientBrush( dropDownRect, ComboBoxButtonGradientBegin( colorTable ), ComboBoxButtonGradientEnd( colorTable ), LinearGradientMode.Vertical ) ) {
                g.FillRectangle( b, dropDownRect );
              }
            }
          }
          else {
            using ( Brush b = new LinearGradientBrush( dropDownRect, ComboBoxButtonPressedGradientBegin( colorTable ), ComboBoxButtonPressedGradientEnd( colorTable ), LinearGradientMode.Vertical ) ) {
              g.FillRectangle( b, dropDownRect );
            }
          }
        }

        Brush brush = ( comboBox.Enabled ) ? SystemBrushes.ControlText : SystemBrushes.GrayText;
        Point middle = new Point( dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2 );

        // if the width is odd - favor pushing it over one pixel right. 
        middle.X += ( dropDownRect.Width % 2 );
        g.FillPolygon( brush, new Point[] {
                            new Point(middle.X - 2, middle.Y - 1), new Point(middle.X + 3, middle.Y - 1), new Point(middle.X, middle.Y + 2) 
                        } );
      }

      internal Color ComboBoxBorder( ProfessionalColorTable table ) {
        return table.ButtonSelectedHighlightBorder;
      }

      internal Color ComboBoxButtonGradientBegin( ProfessionalColorTable table ) {
        return table.MenuItemPressedGradientBegin;
      }

      internal Color ComboBoxButtonGradientEnd( ProfessionalColorTable table ) {
        return table.MenuItemPressedGradientEnd;
      }

      internal Color ComboBoxButtonOnOverflow( ProfessionalColorTable table ) {
        return table.ToolStripDropDownBackground;
      }

      internal Color ComboBoxButtonPressedGradientBegin( ProfessionalColorTable table ) {
        return table.ButtonPressedGradientBegin;
      }

      internal Color ComboBoxButtonPressedGradientEnd( ProfessionalColorTable table ) {
        return table.ButtonPressedGradientEnd;
      }

      internal Color ComboBoxButtonSelectedGradientBegin( ProfessionalColorTable table ) {
        return table.MenuItemSelectedGradientBegin;
      }

      internal Color ComboBoxButtonSelectedGradientEnd( ProfessionalColorTable table ) {
        return table.MenuItemSelectedGradientEnd;
      }

    }
  }
}
