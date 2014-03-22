/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 10 02 2008 : version initiale pour aihm 2007-2008 en net 2.0  
 * 08 04 2008 : correction d'une bogue dans le calcul des expansions
 * 26 09 2008 : am�lioration de la gestion de layout � l'initialisation
 * 04 05 2009 : suppression des calculs de positionnement en mode design
 * 19 05 2009 : am�lioration du placement des composants 
 * 18 07 2010 : am�lioration du placement et adjonction de l'archivage
 * 14 09 2010 : am�lioration d'ensemble de la gestion du placement
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
   * La stabilit� de la gestion du placement est �troitement d�pendante du calcul de positionnement
   * dans DoRearrageRow :
   * 
   * - un mauvais calcul de Location peut conduire � un effet d'inversion de l'ordre des contr�les
   * - un mauvais calcul de Width    peut conduire � des overflows inh�rents aux ToolStrips
   * 
   * En cas d'anomalie dans la restauration des contr�les, en particulier sur archivage, contr�ler
   * le bon positionnement, en particulier Location
   */

  /// <summary>
  /// Conteneur de rafting avec gestion am�lior�e du placement.
  /// </summary>
  /// <remarks>
  /// Cette extension du composant <see cref="ToolStripPanel"/> permet un placement
  /// am�lior� des conteneurs <see cref="ToolStrip"/> qui y sont plac�s.
  /// <br/>
  /// (1) comme dans le composant <see cref="ToolStripPanel"/>, un conteneur <see cref="ToolStrip"/>
  /// dont la propri�t� <see cref="ToolStrip.Stretch"/> est true occupe une rang�e enti�re;
  /// <br/>
  /// (2) dans ces conteneurs, les �l�ments <see cref="ToolStripItem"/> dont la propri�t� 
  /// <see cref="ToolStripItem.AutoSize"/> est false sont consid�r�s comme �tirables;
  /// <br/>
  /// (3) au sein d'une rang�e (comportant un ou plusieurs <see cref="ToolStrip"/>), 
  /// les �l�ments �tirables sont �tir�s, au pro rata de leur taille, pour que la rang�e
  /// soit compl�tement occup�e; autrement dit, d�s qu'au moins un conteneur <see cref="ToolStrip"/> 
  /// comporte au moins un �l�ment �tirable, la rang�e sera compl�tement occup�e.
  /// <br/><br/>
  /// Le placement par d�faut (hors archivage) applique les r�gles suivantes :
  /// <br/>
  /// (1) une barre de menus est toujours plac�e dans la premi�re rang�e; 
  /// <br/>
  /// (2) les autres conteneurs <see cref="ToolStrip"/> sont plac�s dans leur ordre
  /// d'insertion; les conteneurs non "stretch" ont tendance � �tre plac�s dans la
  /// m�me rang�e jusqu'� ce que la rang�e soit remplie.
  /// </remarks>
  [
  ToolboxBitmap( typeof( ToolStripPanelEnh ), "ToolStripPanelEnh.bmp" ), 
  ToolboxItemFilter( "System.Windows.Forms" ),
  Description( "Conteneur de rafting avec gestion am�lior�e du placement" )
  ]
  public class ToolStripPanelEnh : ToolStripPanel {

    private int layoutSuspendCount = 0;

    /// <summary>
    /// Permet de savoir si un ToolStripItem est �tirable dans son conteneur.
    /// </summary>
    /// <remarks>
    /// A partir du 18 juillet 2010, un item (bouton, combo, etc.) est �tirable
    /// si sa propri�t� <see cref="Control.AutoSize"/> est false.
    /// </remarks>
    /// <param name="item">item consid�r�</param>
    /// <returns>true si l'item est �tirable dans son conteneur</returns>
    private bool IsSpring( ToolStripItem item ) {
      return !item.AutoSize;
    }

    /// <summary>
    /// R�cup�rer le tableau des contr�les associ� � une rang�e
    /// </summary>
    /// <remarks>
    /// Lorsque les rang�es sont en cours de glissement via la souris, l'acc�s � la propri�t�
    /// Controls d'une rang�e provoque une exception. Je n'ai pas trouv� de moyen pour contourner
    /// cette difficult� en �vitant l'exception.
    /// </remarks>
    /// <param name="row">rang�e contenant les contr�les ou null</param>
    /// <returns>tableau des contr�les ou null</returns>
    private Control[] DoGetControlsOfRow( ToolStripPanelRow row ) {
      try {
        return row.Controls;
      }
      catch { return new Control[ 0 ]; }
    }

    // todo (ToolStripPanelEnh) tenir compte des contraintes MinimumSize et MaximumSize 
    /// <summary>
    /// Dimensionnement et placement des conteneurs <see cref="ToolStrip"/> d'une rang�e donn�e.
    /// </summary>
    /// <remarks>
    /// Seuls les �l�ments <see cref="ToolStripItem"/> �tirables (pour lesquels <see cref="IsSpring"/> retourne true) 
    /// des conteneurs <see cref="ToolStrip"/> sont �tir�s par cette m�thode.
    /// <br/>
    /// Si la rang�e comporte un ou plusieurs conteneur comportant un ou plusieurs �l�ments �tirables,
    /// l'�tirement s'effectue au pro rata de la taille actuels des �l�ments �tirables de telle mani�re
    /// que la rang�e soit compl�tement occup�e.
    /// </remarks>
    /// <param name="row">rang�e dont les �l�ments sont � redimensionner</param>
    private void DoRearrangeRow( ToolStripPanelRow row ) {

      // r�cup�rer les contr�les de la tang�e
      Control[] controls = DoGetControlsOfRow( row );
      if ( controls == null || controls.Length == 0 ) return;

      // initialisations des infos 
      int fixWidth = 7 + row.Margin.Horizontal + row.Padding.Horizontal;
      int varWidth = 0;
      int varCount = 0;

      // r�cup�rer les infos de dimensions horizontales
      for ( int ix = 0 ; ix < controls.Length ; ix++ ) {
        ToolStrip strip = (ToolStrip) controls[ ix ];
        if ( !strip.Visible ) continue;

        // dimensions horizontales hors �l�ments contenus
        fixWidth += 2 + strip.GripRectangle.Width + strip.GripMargin.Horizontal + strip.Padding.Horizontal + strip.Margin.Horizontal;

        // d�terminer les largeurs fixes et variables
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

      // facteur d'�tirement
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

        // position horizontale du contr�le
        Point location = new Point( leftAnchor, row.Bounds.Top );
        if ( strip.Location != location ) 
          strip.Location = location;

        // largeur du contr�le s'il est �tirable
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

        // forcer le conteneur � �tre mis � jour en fonction de la nouvelle dimension
        if (stripChanged) base.OnLayout(new LayoutEventArgs(strip, "PreferredSize"));

        // prochaine position possible
        if ( strip.Visible ) leftAnchor += strip.Width;
      }
    }

    /// <summary>
    /// Dimensionnement et placement des conteneurs ToolStrip de la rang�e � la quelle appartient le contr�le
    /// </summary>
    /// <param name="control">contr�le d�terminant la rang�e � compacter</param>
    private void DoRearrangeRowOf( Control control ) {
      ToolStripPanelRow row = PointToRow( control.Location );
      DoRearrangeRow( row );
    }

    /// <summary>
    /// Dimensionnement et placement des conteneurs ToolStrip de toutes les rang�es.
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
    /// Seuls les conteneurs <see cref="ToolStrip"/> comportant des �l�ments <see cref="ToolStripItem"/>
    /// �tirables (pour lesquels <see cref="IsSpring"/> retourne true) peuvent �tre compress�s.
    /// <br/>
    /// La compression pr�serve la proportionnalit� de chaque �l�ment �tirable en vue de leur r�-expansion.
    /// <br/>
    /// La compression permet de laisser le gestionnaire de layout effectuer normalement le contr�le du placement
    /// lorsque des conteneurs <see cref="ToolStrip"/> sont ins�r�s ou d�plac�s.
    /// </remarks>
    /// <param name="strip">conteneur � comprimer</param>
    private void DoShrinkToolStrip( ToolStrip strip ) {
      int varWidth = 0;
      int varCount = 0;

      // calculer les donn�es de la compression
      foreach ( ToolStripItem item in strip.Items )
        if ( IsSpring( item ) ) {
          varWidth += item.Width;
          varCount++;
        }

      // rien � faire si aucun �l�ment �tirable
      if ( varCount == 0 ) return;

      // facteur de compression pr�servant la proportionnalit�
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
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> d'une rang�e.
    /// </summary>
    /// <param name="row">rang�e � comprimer</param>
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
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> des rang�es.
    /// </summary>
    /// <param name="rows">tableau des rang�es � comprimer</param>
    private void DoShrinkRows( ToolStripPanelRow[] rows ) {
      for ( int ix = 0 ; ix < rows.Length ; ix++ )
        DoShrinkRow( rows[ ix ] );
    }

    /// <summary>
    /// Comprime en largeur les conteneurs <see cref="ToolStrip"/> de toutes les rang�es.
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
    // M�thodes red�finies
    //

    ///// <summary>
    ///// Adjonction d'un ToolStrip dans le panneau.
    ///// </summary>
    ///// <param name="e">descripteur de l'�v�nement</param>
    //protected override void OnControlAdded( ControlEventArgs e ) {
    //  Psl.Tracker.Tracker.Box.doTrack( "OnControlAdded: strip=" + e.Control.Name + ", location=" + e.Control.Location );
    //  base.OnControlAdded( e );
    //}

    /// <summary>
    /// Mise � jour du dimensionnement et du placement.
    /// </summary>
    /// <remarks>
    /// N'intervient que sur les cas de redimensionnements du panneau ou des contr�les contenus
    /// pour ajuster l'�tirement des conteneurs dans leur rang�e.
    /// </remarks>
    /// <param name="e">descripteur de l'�v�nement</param>
    protected override void OnLayout(LayoutEventArgs e) {
      if (!DesignMode) {
        //Psl.Tracker.Tracker.Box.doTrack("OnLayout.enter: control=" + (e.AffectedControl == null ? "null" : e.AffectedControl.Name) + ", " + "property=" + e.AffectedProperty + (e.AffectedControl == null ? "" : ", prefsize=" + e.AffectedControl.PreferredSize) );
        if (layoutSuspendCount == 0) {
          if (e.AffectedControl == this) {
            if (e.AffectedProperty == "Bounds") DoRearrangeRows();
          }
          else {
            bool doIt = false;

            // optimisation sur "PreferredSize" pour �viter les calculs de positionnement parasites
            if (e.AffectedProperty == "PreferredSize") {
              //doIt = true;
              string tag = e.AffectedControl.Tag as string;
              string pref = e.AffectedControl.PreferredSize.ToString();
              doIt = tag != pref;
              e.AffectedControl.Tag = pref;
            }

            // r�arranger �ventuellement la rang�e du contr�le
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
    // Fonctionnalit�s expos�es
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
    /// <param name="performLayout">appliquer les requ�tes de redisposition en attente</param>
    public new void ResumeLayout( bool performLayout ) {
      base.ResumeLayout( performLayout );
      layoutSuspendCount--;
    }

    /// <summary>
    /// Immerge un <see cref="ToolStrip"/> dans le conteneur
    /// </summary>
    /// <param name="strip">composant � immerger</param>
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
    /// R�fection compl�te de la disposition
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

        // r�ins�rer les contr�les
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
    /// Archivage de l'�tat du conteneur de rafting.
    /// </summary>
    /// <param name="sender">r�f�rence sur l'archiveur</param>
    /// <param name="key">cl�s sous laquelle les donn�es d'archivage seront enregistr�es</param>
    public void Archive( IArchiver sender, string key ) {
      if ( sender.IsReading )
        CellsToLayout( StringToCells( sender.ReadString( key, string.Empty ) ) );
      else
        sender.WriteString( key, CellsToString( LayoutToCells() ), string.Empty );
    }

  }
}
