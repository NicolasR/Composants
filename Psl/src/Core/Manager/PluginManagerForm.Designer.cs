namespace Psl.Applications.Manager {
  partial class PluginManagerForm {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( PluginManagerForm ) );
      this.images = new System.Windows.Forms.ImageList( this.components );
      this.statusLabel = new System.Windows.Forms.Label();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.statusProgress = new System.Windows.Forms.ProgressBar();
      this.btShowAll = new System.Windows.Forms.RadioButton();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.libraries = new System.Windows.Forms.ListView();
      this.listHeaderName = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.btOk = new System.Windows.Forms.Button();
      this.pages = new System.Windows.Forms.TabControl();
      this.pageManager = new System.Windows.Forms.TabPage();
      this.properties = new System.Windows.Forms.ListView();
      this.propertiesHeaderNames = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.propertiesHeaderValues = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.pageLibrary = new System.Windows.Forms.TabPage();
      this.library = new System.Windows.Forms.ListView();
      this.libraryHeaderName = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.libraryHeaderValues = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      ( (System.ComponentModel.ISupportInitialize) ( this.splitContainer ) ).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.Panel2.SuspendLayout();
      this.splitContainer.SuspendLayout();
      ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).BeginInit();
      this.pages.SuspendLayout();
      this.pageManager.SuspendLayout();
      this.pageLibrary.SuspendLayout();
      this.SuspendLayout();
      // 
      // images
      // 
      this.images.ImageStream = ( (System.Windows.Forms.ImageListStreamer) ( resources.GetObject( "images.ImageStream" ) ) );
      this.images.TransparentColor = System.Drawing.Color.Magenta;
      this.images.Images.SetKeyName( 0, "a-Motif Properties.bmp" );
      this.images.Images.SetKeyName( 1, "wao xpl action explore current.bmp" );
      this.images.Images.SetKeyName( 2, "Sys Show Hint.bmp" );
      this.images.Images.SetKeyName( 3, "a-aPadding.bmp" );
      this.images.Images.SetKeyName( 4, "a-aPadding.bmp" );
      this.images.Images.SetKeyName( 5, "a-Motif Dll gris.bmp" );
      this.images.Images.SetKeyName( 6, "a-Motif plugIn dll.bmp" );
      this.images.Images.SetKeyName( 7, "a-aPadding.bmp" );
      this.images.Images.SetKeyName( 8, "a-aPadding.bmp" );
      this.images.Images.SetKeyName( 9, "a-aPadding.bmp" );
      this.images.Images.SetKeyName( 10, "Sys Show Infos.bmp" );
      this.images.Images.SetKeyName( 11, "vsn.do.warning.bmp" );
      this.images.Images.SetKeyName( 12, "Sys Stop.bmp" );
      this.images.Images.SetKeyName( 13, "Sys Rond Orange.bmp" );
      this.images.Images.SetKeyName( 14, "Sys Rond Vert.bmp" );
      // 
      // statusLabel
      // 
      this.statusLabel.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.statusLabel.Location = new System.Drawing.Point( 12, 259 );
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size( 462, 23 );
      this.statusLabel.TabIndex = 16;
      this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point( 0, 0 );
      this.splitContainer.Margin = new System.Windows.Forms.Padding( 1 );
      this.splitContainer.Name = "splitContainer";
      this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add( this.statusProgress );
      this.splitContainer.Panel1.Controls.Add( this.btShowAll );
      this.splitContainer.Panel1.Controls.Add( this.label1 );
      this.splitContainer.Panel1.Controls.Add( this.pictureBox1 );
      this.splitContainer.Panel1.Controls.Add( this.libraries );
      // 
      // splitContainer.Panel2
      // 
      this.splitContainer.Panel2.Controls.Add( this.btOk );
      this.splitContainer.Panel2.Controls.Add( this.statusLabel );
      this.splitContainer.Panel2.Controls.Add( this.pages );
      this.splitContainer.Size = new System.Drawing.Size( 566, 568 );
      this.splitContainer.SplitterDistance = 279;
      this.splitContainer.TabIndex = 17;
      // 
      // statusProgress
      // 
      this.statusProgress.Location = new System.Drawing.Point( 114, 70 );
      this.statusProgress.Name = "statusProgress";
      this.statusProgress.Size = new System.Drawing.Size( 86, 10 );
      this.statusProgress.TabIndex = 21;
      // 
      // btShowAll
      // 
      this.btShowAll.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.btShowAll.AutoCheck = false;
      this.btShowAll.AutoSize = true;
      this.btShowAll.Location = new System.Drawing.Point( 353, 64 );
      this.btShowAll.Name = "btShowAll";
      this.btShowAll.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
      this.btShowAll.Size = new System.Drawing.Size( 201, 17 );
      this.btShowAll.TabIndex = 19;
      this.btShowAll.TabStop = true;
      this.btShowAll.Text = "afficher toutes les librairies examinées";
      this.btShowAll.UseVisualStyleBackColor = true;
      this.btShowAll.Click += new System.EventHandler( this.btShowAll_Click );
      // 
      // label1
      // 
      this.label1.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.label1.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
      this.label1.ForeColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 0 ) ) ) ), ( (int) ( ( (byte) ( 0 ) ) ) ), ( (int) ( ( (byte) ( 192 ) ) ) ) );
      this.label1.Location = new System.Drawing.Point( 114, 12 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 440, 46 );
      this.label1.TabIndex = 18;
      this.label1.Text = "Chargement et installation des plugins dynamiques";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // pictureBox1
      // 
      this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox1.Image = ( (System.Drawing.Image) ( resources.GetObject( "pictureBox1.Image" ) ) );
      this.pictureBox1.Location = new System.Drawing.Point( 12, 8 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 96, 72 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 17;
      this.pictureBox1.TabStop = false;
      // 
      // libraries
      // 
      this.libraries.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.libraries.BackColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 236 ) ) ) ), ( (int) ( ( (byte) ( 236 ) ) ) ), ( (int) ( ( (byte) ( 255 ) ) ) ) );
      this.libraries.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.listHeaderName} );
      this.libraries.FullRowSelect = true;
      this.libraries.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.libraries.HideSelection = false;
      this.libraries.LabelWrap = false;
      this.libraries.Location = new System.Drawing.Point( 12, 86 );
      this.libraries.MultiSelect = false;
      this.libraries.Name = "libraries";
      this.libraries.Size = new System.Drawing.Size( 542, 190 );
      this.libraries.SmallImageList = this.images;
      this.libraries.StateImageList = this.images;
      this.libraries.TabIndex = 16;
      this.libraries.UseCompatibleStateImageBehavior = false;
      this.libraries.View = System.Windows.Forms.View.Details;
      this.libraries.SelectedIndexChanged += new System.EventHandler( this.libraries_SelectedIndexChanged );
      // 
      // listHeaderName
      // 
      this.listHeaderName.Text = "Librairies";
      this.listHeaderName.Width = 417;
      // 
      // btOk
      // 
      this.btOk.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.btOk.Location = new System.Drawing.Point( 479, 259 );
      this.btOk.Name = "btOk";
      this.btOk.Size = new System.Drawing.Size( 75, 23 );
      this.btOk.TabIndex = 16;
      this.btOk.Text = "Fermer";
      this.btOk.UseVisualStyleBackColor = true;
      this.btOk.Click += new System.EventHandler( this.btOk_Click );
      // 
      // pages
      // 
      this.pages.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.pages.Controls.Add( this.pageManager );
      this.pages.Controls.Add( this.pageLibrary );
      this.pages.ImageList = this.images;
      this.pages.ItemSize = new System.Drawing.Size( 58, 0 );
      this.pages.Location = new System.Drawing.Point( 12, 3 );
      this.pages.Name = "pages";
      this.pages.SelectedIndex = 0;
      this.pages.ShowToolTips = true;
      this.pages.Size = new System.Drawing.Size( 542, 253 );
      this.pages.TabIndex = 15;
      // 
      // pageManager
      // 
      this.pageManager.Controls.Add( this.properties );
      this.pageManager.ImageIndex = 0;
      this.pageManager.Location = new System.Drawing.Point( 4, 23 );
      this.pageManager.Name = "pageManager";
      this.pageManager.Padding = new System.Windows.Forms.Padding( 3 );
      this.pageManager.Size = new System.Drawing.Size( 534, 226 );
      this.pageManager.TabIndex = 2;
      this.pageManager.Text = "Gestionnaire";
      this.pageManager.ToolTipText = "Etat courant du gestionnaire des plugins";
      this.pageManager.UseVisualStyleBackColor = true;
      // 
      // properties
      // 
      this.properties.BackColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 255 ) ) ) ), ( (int) ( ( (byte) ( 255 ) ) ) ), ( (int) ( ( (byte) ( 192 ) ) ) ) );
      this.properties.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.propertiesHeaderNames,
            this.propertiesHeaderValues} );
      this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
      this.properties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.properties.Location = new System.Drawing.Point( 3, 3 );
      this.properties.Name = "properties";
      this.properties.Size = new System.Drawing.Size( 528, 220 );
      this.properties.TabIndex = 1;
      this.properties.UseCompatibleStateImageBehavior = false;
      this.properties.View = System.Windows.Forms.View.Details;
      // 
      // propertiesHeaderNames
      // 
      this.propertiesHeaderNames.Width = 120;
      // 
      // propertiesHeaderValues
      // 
      this.propertiesHeaderValues.Width = 343;
      // 
      // pageLibrary
      // 
      this.pageLibrary.BackColor = System.Drawing.Color.Transparent;
      this.pageLibrary.Controls.Add( this.library );
      this.pageLibrary.ImageIndex = 1;
      this.pageLibrary.Location = new System.Drawing.Point( 4, 23 );
      this.pageLibrary.Name = "pageLibrary";
      this.pageLibrary.Padding = new System.Windows.Forms.Padding( 3 );
      this.pageLibrary.Size = new System.Drawing.Size( 534, 226 );
      this.pageLibrary.TabIndex = 0;
      this.pageLibrary.Text = "Librairie";
      this.pageLibrary.ToolTipText = "Informations relatives à la librairie sélectionnée";
      this.pageLibrary.UseVisualStyleBackColor = true;
      // 
      // library
      // 
      this.library.BackColor = System.Drawing.Color.FromArgb( ( (int) ( ( (byte) ( 236 ) ) ) ), ( (int) ( ( (byte) ( 236 ) ) ) ), ( (int) ( ( (byte) ( 255 ) ) ) ) );
      this.library.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.libraryHeaderName,
            this.libraryHeaderValues} );
      this.library.Dock = System.Windows.Forms.DockStyle.Fill;
      this.library.FullRowSelect = true;
      this.library.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
      this.library.HideSelection = false;
      this.library.Location = new System.Drawing.Point( 3, 3 );
      this.library.MultiSelect = false;
      this.library.Name = "library";
      this.library.Size = new System.Drawing.Size( 528, 220 );
      this.library.TabIndex = 0;
      this.library.UseCompatibleStateImageBehavior = false;
      this.library.View = System.Windows.Forms.View.Details;
      this.library.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler( this.library_MouseDoubleClick );
      // 
      // libraryHeaderName
      // 
      this.libraryHeaderName.Width = 90;
      // 
      // libraryHeaderValues
      // 
      this.libraryHeaderValues.Width = 25;
      // 
      // PluginManagerForm
      // 
      this.AcceptButton = this.btOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 566, 568 );
      this.Controls.Add( this.splitContainer );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "PluginManagerForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Gestionnaire des plugins dynamiques";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler( this.PluginManagerForm_FormClosed );
      this.ResizeEnd += new System.EventHandler( this.PluginManagerForm_ResizeEnd );
      this.splitContainer.Panel1.ResumeLayout( false );
      this.splitContainer.Panel1.PerformLayout();
      this.splitContainer.Panel2.ResumeLayout( false );
      ( (System.ComponentModel.ISupportInitialize) ( this.splitContainer ) ).EndInit();
      this.splitContainer.ResumeLayout( false );
      ( (System.ComponentModel.ISupportInitialize) ( this.pictureBox1 ) ).EndInit();
      this.pages.ResumeLayout( false );
      this.pageManager.ResumeLayout( false );
      this.pageLibrary.ResumeLayout( false );
      this.ResumeLayout( false );

    }

    #endregion

    private System.Windows.Forms.Label statusLabel;
    private System.Windows.Forms.ImageList images;
    private System.Windows.Forms.SplitContainer splitContainer;
    private System.Windows.Forms.Button btOk;
    private System.Windows.Forms.TabControl pages;
    private System.Windows.Forms.TabPage pageManager;
    private System.Windows.Forms.ListView properties;
    private System.Windows.Forms.ColumnHeader propertiesHeaderNames;
    private System.Windows.Forms.ColumnHeader propertiesHeaderValues;
    private System.Windows.Forms.TabPage pageLibrary;
    private System.Windows.Forms.ListView library;
    private System.Windows.Forms.ColumnHeader libraryHeaderName;
    private System.Windows.Forms.ColumnHeader libraryHeaderValues;
    private System.Windows.Forms.ProgressBar statusProgress;
    private System.Windows.Forms.RadioButton btShowAll;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.ListView libraries;
    private System.Windows.Forms.ColumnHeader listHeaderName;
  }
}