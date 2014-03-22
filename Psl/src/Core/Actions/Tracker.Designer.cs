namespace Psl.Tracker {
  partial class Tracker {

    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      this.components = new System.ComponentModel.Container();
      this.tracks = new System.Windows.Forms.ListBox();
      this.trackMenu = new System.Windows.Forms.ContextMenuStrip( this.components );
      this.nettoyerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.trackMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // tracks
      // 
      this.tracks.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tracks.FormattingEnabled = true;
      this.tracks.Location = new System.Drawing.Point( 0, 0 );
      this.tracks.Name = "tracks";
      this.tracks.Size = new System.Drawing.Size( 409, 458 );
      this.tracks.TabIndex = 0;
      // 
      // trackMenu
      // 
      this.trackMenu.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.nettoyerToolStripMenuItem} );
      this.trackMenu.Name = "trackMenu";
      this.trackMenu.Size = new System.Drawing.Size( 121, 26 );
      // 
      // nettoyerToolStripMenuItem
      // 
      this.nettoyerToolStripMenuItem.Name = "nettoyerToolStripMenuItem";
      this.nettoyerToolStripMenuItem.Size = new System.Drawing.Size( 152, 22 );
      this.nettoyerToolStripMenuItem.Text = "&Nettoyer";
      this.nettoyerToolStripMenuItem.Click += new System.EventHandler( this.nettoyerToolStripMenuItem_Click );
      // 
      // Tracker
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 409, 458 );
      this.Controls.Add( this.tracks );
      this.Name = "Tracker";
      this.Text = "Tracker";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.Tracker_FormClosed );
      this.trackMenu.ResumeLayout( false );
      this.ResumeLayout( false );

    }

    #endregion

    private System.Windows.Forms.ListBox tracks;
    private System.Windows.Forms.ContextMenuStrip trackMenu;
    private System.Windows.Forms.ToolStripMenuItem nettoyerToolStripMenuItem;
  }
}