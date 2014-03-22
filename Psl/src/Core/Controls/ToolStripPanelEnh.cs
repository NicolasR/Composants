/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 10 02 2008 : version initiale pour aihm 2007-2008 en net 2.0  
 * 08 04 2008 : correction d'une bogue dans le calcul des expansions
 * 26 09 2008 : amélioration de la gestion de layout à l'initialisation
 * 04 05 2009 : suppression des calculs de positionnement en mode design
 * 19 05 2009 : amélioration du placement des composants 
 * 18 07 2010 : amélioration du placement et adjonction de l'archivage
 * 14 09 2010 : amélioration d'ensemble de la gestion du placement
 */                                                                            // <wao never.end>
using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using Psl.Applications;

namespace Psl.Controls {

  /*
   * 14 09 2010
   * La stabilité de la gestion du placement est étroitement dépendante du calcul de positionnement
   * dans DoRearrageRow :
   * 
   * - un mauvais calcul de Location peut conduire à un effet d'inversion de l'ordre des contrôles
   * - un mauvais calcul de Width    peut conduire à des overflows inhérents aux ToolStrips
   * 
   * En cas d'anomalie dans la restauration des contrôles, en particulier sur archivage, contrôler
   * le bon positionnement, en particulier Location
   */

  /// <summary>
  /// Conteneur de rafting avec gestion améliorée du placement.
  /// </summary>
  /// <remarks>
  /// Cette extension du composant <see cref="ToolStripPanel"/> permet un placement
  /// amélioré des conteneurs <see cref="ToolStrip"/> qui y sont placés.
  /// <br/>
  /// (1) comme dans le composant <see cref="ToolStripPanel"/>, un conteneur <see cref="ToolStrip"/>
  /// dont la propriété <see cref="ToolStrip.Stretch"/> est true occupe une rangée entière;
  /// <br/>
  /// (2) dans ces conteneurs, les éléments <see cref="ToolStripItem"/> dont la propriété 
  /// <see cref="ToolStripItem.AutoSize"/> est false sont considérés comme étirables;
  /// <br/>
  /// (3) au sein d'une rangée (comportant un ou plusieurs <see cref="ToolStrip"/>), 
  /// les éléments étirables sont étirés, au pro rata de leur taille, pour que la rangée
  /// soit complètement occupée; autrement dit, dès qu'au moins un conteneur <see cref="ToolStrip"/> 
  /// comporte au moins un élément étirable, la rangée sera complètement occupée.
  /// <br/><br/>
  /// Le placement par défaut (hors archivage) applique les règles suivantes :
  /// <br/>
  /// (1) une barre de menus est toujours placée dans la première rangée; 
  /// <br/>
  /// (2) les autres conteneurs <see cref="ToolStrip"/> sont placés dans leur ordre
  /// d'insertion; les conteneurs non "stretch" ont tendance à être placés dans la
  /// même rangée jusqu'à ce que la rangée soit remplie.
  /// </remarks>
  [
  ToolboxBitmap( typeof( ToolStripPanelEnh ), "ToolStripPanelEnh.bmp" ), 
  ToolboxItemFilter( "System.Windows.Forms" ),
  Description( "Conteneur de rafting avec gestion améliorée du placement" )
  ]
  public class ToolStripPanelEnh : ToolStripPanel {

    private int layoutSuspendCount = 0;

    /// <summary>
    /// Permet de savoir si un ToolStripItem est étirable dans son conteneur.
    /// </summary>
    /// <remarks>
    /// A partir du 18 juillet 2010, un item (bouton, combo, etc.) est étirable
    /// si sa propriété <see cref="Control.AutoSize"/> est false.
    /// </remarks>
    /// <param name="item">item considéré</param>
    /// <returns>true si l'item est étirable dans son conteneur</returns>
    private bool IsSpring( ToolStripItem item ) {
      return !item.AutoSize;
    }

    /// <summary>
    /// Récupérer le tableau des contrôles associé à une rangée
    /// </summary>
    /// <remarks>
    /// Lorsque les rangées sont en cours de glissement via la souris, l'accès à la propriété
    /// Controls d'une rangée provoque une exception. Je n'ai pas trouvé de moyen pour contourner
    /// cette difficulté en évitant l'exception.
    /// </remarks>
    /// <param name="row">rangée contenant les contrôles ou null</param>
    /// <returns>tableau des contrôles ou null</returns>
    private Control[] DoGetControlsOfRow( ToolStripPanelRow row ) {
      try {
        return row.Controls;
      }
      catch { return new Control[ 0 ]; }
    }

    // todo (ToolStripPanelEnh) tenir compte des contraintes MinimumSize et MaximumSize 
    /// <summary>
    /// Dimensionnement et placement des conteneurs <see cref="ToolStrip"/> d'une rangée donnée.
    /// </summary>
    /// <remarks>
    /// Seuls les éléments <see cref="ToolStripItem"/> étirables (pour lesquels <see cref="IsSpring"/> retourne true) 
    /// des conteneurs <see cref="ToolStrip"/> sont étirés par cette méthode.
    /// <br/>
    /// Si la rangée comporte un ou plusieurs conteneur comportant un ou plusieurs éléments étirables,
    /// l'étirement s'effectue au pro rata de la taille actuels des éléments étirables de telle manière
    /// que la rangée soit complètement occupée.
    /// </remarks>
    /// <param name="row">rangée dont les éléments sont à redimensionner</param>
    private void DoRearrangeRow( ToolStripPanelRow row ) {

      // récupérer les contrôles de la tangée
      Control[] controls = DoGetControlsOfRow( row );
      if ( controls == null || controls.Length == 0 ) return;

      // initialisations des infos 
      int fixWidth = 7 + row.Margin.Horizontal + row.Padding.Horizontal;
      int varWidth = 0;
      int varCount = 0;

      // récupérer les infos de dimensions horizontales
      for ( int ix = 0 ; ix < controls.Length ; ix++ ) {
        ToolStrip strip = (ToolStrip) controls[ ix ];
        if ( !strip.Visible ) continue;

        // dimensions horizontales hors éléments contenus
        fixWidth += 2 + strip.GripRectangle.Width + strip.GripMargin.Horizontal + strip.Padding.Horizontal + strip.Margin.Horizontal;

        // déterminer les largeurs fixes et variables
        for ( int iy = 0 ; iy < strip.Items.Count ; iy++ ) {
          ToolStripItem item = strip.Items[ iy ];
          if ( IsSpring( item ) ) {
            varCount++;
            varWidth += item.Width;
            fixWidth += item.Margin.Horizontal;
          }
          else
            fixWidth += item.Width + item.Margin.Horizontal;
        }
      }

      // facteur d'étirement
      float factor = 0;
      if ( varCount != 0 ) {
        int newWidth = row.Bounds.Width;
        int remains = newWidth - fixWidth;
        if ( varWidth == 0 ) varWidth = varCount;
        factor = Math.Abs( (float) remains / (float) varWidth );
      }

      // ancre de placement horizontal
      int leftAnchor = row.Bounds.Left;

      // tenter d'expanser le conteneur strip
      for ( int ix = 0 ; ix < controls.Length ; ix++ ) {
        ToolStrip strip = (ToolStrip) controls[ ix ];
        bool stripChanged = false;

        // position horizontale du contrôle
        Point location = new Point( leftAnchor, row.Bounds.Top );
        if ( strip.Location != location ) 
          strip.Location = location;

        // largeur du contrôle s'il est étirable
        if ( factor != 0 ) {
          for ( int iy = 0 ; iy < strip.Items.Count ; iy++ ) {
            ToolStripItem item = strip.Items[ iy ];
            if ( !IsSpring( item ) ) continue;
            int width = Convert.ToInt32(item.Width * factor);
            if ( width < 1 ) width = 1;
            if (item.Width != width) {
              item.Width = width;
              stripChanged = true;
            }
          }
        }

        // forcer le conteneur à être mis à jour en fonction de la nouvelle dimension
        if (stripChanged) base.OnLayout(new LayoutEventArgs(strip, "PreferredSize"));

        // prochaine position possible
        if ( strip.Visible ) leftAnchor += strip.Width;
      }
    }

    /// <summary>
    /// Dimensionnement et placement des conteneurs ToolStrip de la rangée à la quelle appartient le contrôle
    /// </summary>
    /// <param name="control">contrôle déterminant la rangée à compacter</param>
    private void DoRearrangeRowOf( Control control ) {
      ToolStripPanelRow row = PointToRow( control.Location );
      DoRearrangeRow( row );
    }

    /// <summary>
    /// Dimensionnement et placement des conteneurs ToolStrip de toutes les rangées.
    /// </summary>
    private void DoRearrangeRows() {
      ToolStripPanelRow[] rows = Rows;
      for ( int ix = 0 ; ix < rows.Length ; ix++ )
        DoRearrangeRow( rows[ ix ] );
    }

    // todo (ToolStripPanelEnh) tenir compte des contraintes MinimumSize et MaximumSize 
    /// <summary>
    /// Comprime en largeur un conteneur ToolStrip
    /// </summary>
    /// <remarks>
    /// Seuls les conteneurs <see cref="ToolStrip"/> comportant des éléments <see cref="ToolStripItem"/>
    /// étirables (pour lesquels <see cref="IsSpring"/> retourne true) peuvent être compressés.
    /// <br/>
    /// La compression préserve la proportionnalité de chaque élément étirable en vue de leur ré-expansion.
    /// <br/>
    /// La compression permet de laisser le gestionnaire de layout effectuer normalement le contrôle du placement
    /// lorsque des conteneurs <see cref="ToolStrip"/> sont insérés ou déplacés.
    /// </remarks>
    /// <param name="strip">conteneur à comprimer</param>
    private void DoShrinkToolStrip( ToolStrip strip ) {
      int varWidth = 0;
      int varCount = 0;

      // calculer les données de la compression
      foreach ( ToolStripItem item in strip.Items )
        if ( IsSpring( item ) ) {
          varWidth += item.Width;
          varCount++;
        }

      // rien à faire si aucun élément étirable
      if ( varCount == 0 ) return;

      // facteur de compression préservant la proportionnalité
      int varMax = varCount * 25;
      if ( varWidth <= varMax ) return;
      float factor = Math.Abs( (float) varWidth / (float) varMax );

      // appliquer la compression
      foreach ( ToolStripItem item in strip.Items ) {
        if ( !IsSpring( item ) ) continue;
        int width = Convert.ToInt32( item.Width / factor );
        if ( item.Width != width ) item.Width = width;
      }
    }

    /// <summary>
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> d'une rangée.
    /// </summary>
    /// <param name="row">rangée à comprimer</param>
    private void DoShrinkRow( ToolStripPanelRow row ) {
      int leftAnchor = 0;

      Control[] strips = row.Controls;
      for ( int ix = 0 ; ix < strips.Length ; ix++ ) {
        ToolStrip strip = (ToolStrip) strips[ ix ];

        Point location = new Point( leftAnchor, row.Bounds.Top );
        if ( strip.Location != location ) strip.Location = location;

        DoShrinkToolStrip( strip );
        //OnLayout(new LayoutEventArgs(strip, "Bounds"));
        //OnLayout(new LayoutEventArgs(strip, "PreferredSize"));

        if ( strip.Visible ) leftAnchor += strip.Width;
      }
    }

    /// <summary>
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> des rangées.
    /// </summary>
    /// <param name="rows">tableau des rangées à comprimer</param>
    private void DoShrinkRows( ToolStripPanelRow[] rows ) {
      for ( int ix = 0 ; ix < rows.Length ; ix++ )
        DoShrinkRow( rows[ ix ] );
    }

    /// <summary>
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> de toutes les rangées.
    /// </summary>
    private void DoShrinkRows() {
      DoShrinkRows( Rows );
    }


    private Point DoGetAddLocation( int rowAnchor, ToolStrip strip ) {
      Point result = new Point();
      ToolStripPanelRow[] rows = Rows;
      if ( rows.Length == 0 ) return result;

      if ( strip is MenuStrip ) return result;

      if ( rowAnchor < 0 || rowAnchor > rows.Length ) rowAnchor = rows.Length;

      for ( int ix = rowAnchor ; ix < rows.Length ; ix++ ) {
        ToolStripPanelRow row = rows[ ix ];
        Control[] strips = row.Controls;
        if ( strips.Length == 0 ) {
          result.Y = row.Bounds.Top;
          return result;
        }

        if ( strips[ 0 ] is MenuStrip ) continue;

        ToolStrip last = (ToolStrip) strips[ strips.Length - 1 ];
        if ( row.Bounds.Right - last.Bounds.Right < strip.Width ) continue;

        result.X = last.Bounds.Right + 1;
        result.Y = row.Bounds.Top;
        return result;
      }

      result.Y = rows[ rows.Length - 1 ].Bounds.Bottom;
      return result;
    }

    //
    // Méthodes redéfinies
    //

    ///// <summary>
    ///// Adjonction d'un ToolStrip dans le panneau.
    ///// </summary>
    ///// <param name="e">descripteur de l'événement</param>
    //protected override void OnControlAdded( ControlEventArgs e ) {
    //  Psl.Tracker.Tracker.Box.doTrack( "OnControlAdded: strip=" + e.Control.Name + ", location=" + e.Control.Location );
    //  base.OnControlAdded( e );
    //}

    /// <summary>
    /// Mise à jour du dimensionnement et du placement.
    /// </summary>
    /// <remarks>
    /// N'intervient que sur les cas de redimensionnements du panneau ou des contrôles contenus
    /// pour ajuster l'étirement des conteneurs dans leur rangée.
    /// </remarks>
    /// <param name="e">descripteur de l'événement</param>
    protected override void OnLayout(LayoutEventArgs e) {
      if (!DesignMode) {
        //Psl.Tracker.Tracker.Box.doTrack("OnLayout.enter: control=" + (e.AffectedControl == null ? "null" : e.AffectedControl.Name) + ", " + "property=" + e.AffectedProperty + (e.AffectedControl == null ? "" : ", prefsize=" + e.AffectedControl.PreferredSize) );
        if (layoutSuspendCount == 0) {
          if (e.AffectedControl == this) {
            if (e.AffectedProperty == "Bounds") DoRearrangeRows();
          }
          else {
            bool doIt = false;

            // optimisation sur "PreferredSize" pour éviter les calculs de positionnement parasites
            if (e.AffectedProperty == "PreferredSize") {
              //doIt = true;
              string tag = e.AffectedControl.Tag as string;
              string pref = e.AffectedControl.PreferredSize.ToString();
              doIt = tag != pref;
              e.AffectedControl.Tag = pref;
            }

            // réarranger éventuellement la rangée du contrôle
            if (doIt || e.AffectedProperty == "Bounds" || e.AffectedProperty == "Visible")
              DoRearrangeRowOf(e.AffectedControl);
          }
        }
      }
      //Psl.Tracker.Tracker.Box.doTrack("OnLayout.base");
      base.OnLayout(e);
      //Psl.Tracker.Tracker.Box.doTrack("OnLayout.exit");
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Suspend le processus de redisposition
    /// </summary>
    public new void SuspendLayout() {
      base.SuspendLayout();
      layoutSuspendCount++;
    }

    /// <summary>
    /// Reprend le processus de redisposition
    /// </summary>
    public new void ResumeLayout() {
      base.ResumeLayout();
      layoutSuspendCount--;
    }

    /// <summary>
    /// Reprendre le processus de redisposition
    /// </summary>
    /// <param name="performLayout">appliquer les requêtes de redisposition en attente</param>
    public new void ResumeLayout( bool performLayout ) {
      base.ResumeLayout( performLayout );
      layoutSuspendCount--;
    }

    /// <summary>
    /// Immerge un <see cref="ToolStrip"/> dans le conteneur
    /// </summary>
    /// <param name="strip">composant à immerger</param>
    public void Merge( ToolStrip strip ) {
      SuspendLayout();
      try {
        DoShrinkRows();
        Point location = DoGetAddLocation( 0, strip );
        DoAddStrip( strip, location );
      } finally {
        ResumeLayout(true);
        //ResumeLayout(false);
        //PerformLayout();
      }
    }

    /// <summary>
    /// Réfection complète de la disposition
    /// </summary>
    public void RebuildLayout() {
      SuspendLayout();
      try {
        DoRearrangeRows();
      } finally {
        ResumeLayout(true);
        //ResumeLayout(false);
        //PerformLayout();
      }
    }

    //
    // Archivage
    //

    private int GetHashCodeOf( ToolStrip strip ) {
      string key = strip.GetType().Name + ':' + strip.Name;
      foreach ( ToolStripItem item in strip.Items )
        key += item.GetType().Name + ':' + item.Name;
      return key.GetHashCode();
    }

    private class Cell {
      public int StripKey = 0 ;
      public int RowIndex = -1;
      public ToolStrip Strip = null;

      public override string ToString() {
        string hexStripKey = string.Format("{0:X}", StripKey);
        return Strip.Name + ":" + hexStripKey + ":" + RowIndex;
      }

      public static Cell FromString( string value ) {
        Cell result = new Cell();

        if (string.IsNullOrEmpty( value) ) return result ;

        string[] fields = value.Split( ':' );
        if ( fields.Length != 3 ) return result;

        int stripKey;
        if (!int.TryParse(fields[1], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out stripKey)) return result;

        int rowIndex;
        if ( !int.TryParse( fields[ 2 ], out rowIndex ) ) return result;

        result.StripKey = stripKey;
        result.RowIndex = rowIndex;

        return result;
      }
    }

    private List<Cell> LayoutToCells() {
      List<Cell> result = new List<Cell>();

      ToolStripPanelRow[] rows = Rows;
      int rowIndex = -1;
      foreach ( ToolStripPanelRow row in rows ) {
        rowIndex++;
        Control[] controls = DoGetControlsOfRow( row );
        if ( controls == null ) continue;
        foreach ( ToolStrip strip in controls )
          result.Add( new Cell() { StripKey = GetHashCodeOf( strip ), RowIndex = rowIndex, Strip = strip } );
      }

      return result;
    }

    private string CellsToString( List<Cell> cells ) {
      string result = string.Empty;
      for ( int ix = 0 ; ix < cells.Count ; ix++ ) {
        if ( ix > 0 ) result += ",";
        result += cells[ ix ].ToString();
      }
      return result;
    }

    private List<Cell> StringToCells( string value ) {
      List<Cell> result = new List<Cell>();

      string[] items = value.Split( ',' );
      for ( int ix = 0 ; ix < items.Length ; ix++ )
        result.Add( Cell.FromString( items[ ix ] ) );

      return result;
    }

    private int IndexOfStripKey( List<Cell> cells, int stripKey ) {
      for ( int ix = 0 ; ix < cells.Count ; ix++ )
        if ( cells[ ix ].StripKey == stripKey ) return ix;
      return -1;
    }

    private int IndexOfStrip( List<Cell> cells, ToolStrip strip ) {
      for ( int ix = 0 ; ix < cells.Count ; ix++ )
        if ( cells[ ix ].Strip == strip ) return ix;
      return -1;
    }

    private void TryLinkStripToCell( List<Cell> cells, ToolStrip strip ) {
      int stripKey = GetHashCodeOf( strip ) ;
      int index = IndexOfStripKey( cells, stripKey );
      if ( index == -1 || cells[ index ].Strip != null )
        cells.Add( new Cell() { StripKey = stripKey, RowIndex = -1, Strip = strip } );
      else
        cells[ index ].Strip = strip;
    }


    private void DoAddStrip( ToolStrip strip, Point location ) {
      //Psl.Tracker.Tracker.Box.doTrack( "DoAddStrip: strip=" + strip.Name + ", location=" + location + ", Bounds=" + Bounds );
      Join( strip, location );
      OnLayout( new LayoutEventArgs( strip, "Parent" ) );
    }

    private void CellsToLayout( List<Cell> cells ) {

      foreach ( ToolStripPanelRow row in Rows ) {
        Control[] controls = DoGetControlsOfRow( row );
        if ( controls == null ) continue;
        foreach ( ToolStrip strip in controls )
          TryLinkStripToCell( cells, strip );
      }

      SuspendLayout();
      //Psl.Tracker.Tracker.Box.doTrack( "CellsToLayout: layout suspended" );
      try {

        Controls.Clear();

        // réinsérer les contrôles
        for (int ix = 0 ; ix < cells.Count ; ix++) {
          //for (int ix = cells.Count - 1; ix >= 0; ix--) {
          Cell cell = cells[ix];
          ToolStrip strip = cells[ix].Strip;
          if (strip == null) continue;

          DoShrinkToolStrip(strip);

          Point location = DoGetAddLocation(cell.RowIndex, strip);
          DoAddStrip(strip, location);
        }

      } finally {
        //Psl.Tracker.Tracker.Box.doTrack( "CellsToLayout: layout before resume" );
        ResumeLayout(true);
        //ResumeLayout(false);
        //PerformLayout();
        //Psl.Tracker.Tracker.Box.doTrack( "CellsToLayout: layout after resume" );
      }
    }

    /// <summary>
    /// Archivage de l'état du conteneur de rafting.
    /// </summary>
    /// <param name="sender">référence sur l'archiveur</param>
    /// <param name="key">clés sous laquelle les données d'archivage seront enregistrées</param>
    public void Archive( IArchiver sender, string key ) {
      if ( sender.IsReading )
        CellsToLayout( StringToCells( sender.ReadString( key, string.Empty ) ) );
      else
        sender.WriteString( key, CellsToString( LayoutToCells() ), string.Empty );
    }

  }
}
