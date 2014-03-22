/*
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 31 01 2008 : version initiale pour aihm 2007-2008
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Psl.Applications.Manager {

  /// <summary>
  /// Concepteur manuel pour les �l�ments de menus
  /// </summary>
  partial class PluginManager {

    /// <summary>
    /// Cr�ation et immersion des �l�ments de menu
    /// </summary>
    private void GenerateMenu() {
      if ( !Registry.Has( MainKeys.KeyMainMenu ) ) return;

      ToolStripMenuItem itemReport = new ToolStripMenuItem( "Etat du gestionnaire des plugins dynamiques", global::Psl.Properties.Resources.IconProperties, itemReport_OnClick );
      itemReport.MergeAction = MergeAction.Insert;
      itemReport.MergeIndex = 9000;
      itemReport.ImageTransparentColor = System.Drawing.Color.Magenta;

      ToolStripSeparator itemReportSep = new ToolStripSeparator();
      itemReportSep.MergeAction = MergeAction.Insert;
      itemReportSep.MergeIndex = 9000;

      ToolStripMenuItem itemAbout = new ToolStripMenuItem( "A propos du gestionnaire des plugins dynamiques", global::Psl.Properties.Resources.IconAbout, itemAbout_OnClick );
      itemAbout.MergeAction = MergeAction.Insert;
      itemAbout.MergeIndex = 9000;
      itemAbout.ImageTransparentColor = System.Drawing.Color.Magenta;

      ToolStripMenuItem itemHelpAbout = new ToolStripMenuItem( "A propos..." );
      itemHelpAbout.MergeAction = MergeAction.MatchOnly;
      itemHelpAbout.MergeIndex = 1;
      itemHelpAbout.DropDownItems.Add( itemAbout );

      ToolStripMenuItem menuHelp = new ToolStripMenuItem( "&?" );
      menuHelp.MergeAction = MergeAction.MatchOnly;
      menuHelp.MergeIndex = 9999;
      menuHelp.DropDownItems.Add( itemHelpAbout );
      menuHelp.DropDownItems.Add( itemReportSep );
      menuHelp.DropDownItems.Add( itemReport    );

      MenuStrip mainMenu = new MenuStrip();
      mainMenu.Items.Add( menuHelp );

      Registry.MergeInMainMenu( mainMenu );
    }

    /// <summary>
    /// Gestionnaire pour l'�v�nement Click de la commande d'affichage du rapport.
    /// </summary>
    /// <param name="source">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void itemReport_OnClick( object source, EventArgs e ) {
      ShowReport( true );
    }

    /// <summary>
    /// Gestionnaire pour l'�v�nemetn Click de la commande d'affichage de la bo�te "� propos".
    /// </summary>
    /// <param name="source">source de l'�v�nement</param>
    /// <param name="e">descripteur de l'�v�nement</param>
    private void itemAbout_OnClick( object source, EventArgs e ) {
      Information.ShowDialog( "Gestionnaire des plugins dynamiques", "standard", version );
    }  
  }
}
