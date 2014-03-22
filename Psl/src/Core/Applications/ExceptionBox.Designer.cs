namespace Psl.Applications {

  partial class ExceptionBox {

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup( "Propriétés de l\'objet exception", System.Windows.Forms.HorizontalAlignment.Left );
      System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup( "Données complémentaires", System.Windows.Forms.HorizontalAlignment.Left );
      System.Windows.Forms.ListViewGroup listViewGroup3 = new System.Windows.Forms.ListViewGroup( "Trace de la pile d\'exécution", System.Windows.Forms.HorizontalAlignment.Left );
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ExceptionBox ) );
      this.firstPanel = new System.Windows.Forms.Panel();
      this.lbMessageContainer = new System.Windows.Forms.Label();
      this.lbMessage = new System.Windows.Forms.Label();
      this.iconPicture = new System.Windows.Forms.PictureBox();
      this.btShowDetails = new System.Windows.Forms.Button();
      this.btOk = new System.Windows.Forms.Button();
      this.tree = new System.Windows.Forms.TreeView();
      this.properties = new System.Windows.Forms.ListView();
      this.columnKeys = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.columnValues = ( (System.Windows.Forms.ColumnHeader) ( new System.Windows.Forms.ColumnHeader() ) );
      this.splitter2 = new System.Windows.Forms.Splitter();
      this.label1 = new System.Windows.Forms.Label();
      this.centerPanel = new System.Windows.Forms.SplitContainer();
      this.application = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.pages = new System.Windows.Forms.TabControl();
      this.pageProperties = new System.Windows.Forms.TabPage();
      this.PageStack = new System.Windows.Forms.TabPage();
      this.stack = new System.Windows.Forms.TreeView();
      this.stackTools = new System.Windows.Forms.ToolStrip();
      this.btStackCollapseAll = new System.Windows.Forms.ToolStripButton();
      this.btStackExpandAll = new System.Windows.Forms.ToolStripButton();
      this.firstPanel.SuspendLayout();
      ( (System.ComponentModel.ISupportInitialize) ( this.iconPicture ) ).BeginInit();
      ( (System.ComponentModel.ISupportInitialize) ( this.centerPanel ) ).BeginInit();
      this.centerPanel.Panel1.SuspendLayout();
      this.centerPanel.Panel2.SuspendLayout();
      this.centerPanel.SuspendLayout();
      this.pages.SuspendLayout();
      this.pageProperties.SuspendLayout();
      this.PageStack.SuspendLayout();
      this.stackTools.SuspendLayout();
      this.SuspendLayout();
      // 
      // firstPanel
      // 
      this.firstPanel.Controls.Add( this.lbMessageContainer );
      this.firstPanel.Controls.Add( this.lbMessage );
      this.firstPanel.Controls.Add( this.iconPicture );
      this.firstPanel.Controls.Add( this.btShowDetails );
      this.firstPanel.Controls.Add( this.btOk );
      this.firstPanel.Dock = System.Windows.Forms.DockStyle.Top;
      this.firstPanel.Location = new System.Drawing.Point( 0, 0 );
      this.firstPanel.MinimumSize = new System.Drawing.Size( 392, 66 );
      this.firstPanel.Name = "firstPanel";
      this.firstPanel.Size = new System.Drawing.Size( 536, 82 );
      this.firstPanel.TabIndex = 1;
      // 
      // lbMessageContainer
      // 
      this.lbMessageContainer.AutoSize = true;
      this.lbMessageContainer.ForeColor = System.Drawing.Color.Red;
      this.lbMessageContainer.Location = new System.Drawing.Point( 61, 63 );
      this.lbMessageContainer.Name = "lbMessageContainer";
      this.lbMessageContainer.Size = new System.Drawing.Size( 103, 13 );
      this.lbMessageContainer.TabIndex = 8;
      this.lbMessageContainer.Text = "lbMessageContainer";
      // 
      // lbMessage
      // 
      this.lbMessage.AutoSize = true;
      this.lbMessage.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
      this.lbMessage.Location = new System.Drawing.Point( 61, 12 );
      this.lbMessage.MaximumSize = new System.Drawing.Size( 230, 0 );
      this.lbMessage.MinimumSize = new System.Drawing.Size( 230, 40 );
      this.lbMessage.Name = "lbMessage";
      this.lbMessage.Size = new System.Drawing.Size( 230, 40 );
      this.lbMessage.TabIndex = 7;
      this.lbMessage.Text = "lbMessage";
      // 
      // iconPicture
      // 
      this.iconPicture.Location = new System.Drawing.Point( 10, 12 );
      this.iconPicture.Name = "iconPicture";
      this.iconPicture.Size = new System.Drawing.Size( 32, 32 );
      this.iconPicture.TabIndex = 6;
      this.iconPicture.TabStop = false;
      // 
      // btShowDetails
      // 
      this.btShowDetails.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.btShowDetails.Location = new System.Drawing.Point( 451, 24 );
      this.btShowDetails.Name = "btShowDetails";
      this.btShowDetails.Size = new System.Drawing.Size( 75, 23 );
      this.btShowDetails.TabIndex = 5;
      this.btShowDetails.Text = "Détails";
      this.btShowDetails.UseVisualStyleBackColor = true;
      this.btShowDetails.Click += new System.EventHandler( this.btShowDetails_Click );
      // 
      // btOk
      // 
      this.btOk.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btOk.Location = new System.Drawing.Point( 451, 53 );
      this.btOk.Name = "btOk";
      this.btOk.Size = new System.Drawing.Size( 75, 23 );
      this.btOk.TabIndex = 4;
      this.btOk.Text = "Ok";
      this.btOk.UseVisualStyleBackColor = true;
      // 
      // tree
      // 
      this.tree.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.tree.BackColor = System.Drawing.SystemColors.Window;
      this.tree.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ( (byte) ( 0 ) ) );
      this.tree.FullRowSelect = true;
      this.tree.HideSelection = false;
      this.tree.Indent = 15;
      this.tree.Location = new System.Drawing.Point( 10, 19 );
      this.tree.Name = "tree";
      this.tree.ShowNodeToolTips = true;
      this.tree.ShowPlusMinus = false;
      this.tree.ShowRootLines = false;
      this.tree.Size = new System.Drawing.Size( 516, 94 );
      this.tree.TabIndex = 1;
      this.tree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler( this.tree_AfterSelect );
      // 
      // properties
      // 
      this.properties.Columns.AddRange( new System.Windows.Forms.ColumnHeader[] {
            this.columnKeys,
            this.columnValues} );
      this.properties.Dock = System.Windows.Forms.DockStyle.Fill;
      this.properties.FullRowSelect = true;
      listViewGroup1.Header = "Propriétés de l\'objet exception";
      listViewGroup1.Name = "GroupProperties";
      listViewGroup2.Header = "Données complémentaires";
      listViewGroup2.Name = "GroupData";
      listViewGroup3.Header = "Trace de la pile d\'exécution";
      listViewGroup3.Name = "GroupStack";
      this.properties.Groups.AddRange( new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2,
            listViewGroup3} );
      this.properties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
      this.properties.LabelWrap = false;
      this.properties.Location = new System.Drawing.Point( 3, 3 );
      this.properties.MultiSelect = false;
      this.properties.Name = "properties";
      this.properties.ShowGroups = false;
      this.properties.ShowItemToolTips = true;
      this.properties.Size = new System.Drawing.Size( 516, 101 );
      this.properties.TabIndex = 3;
      this.properties.UseCompatibleStateImageBehavior = false;
      this.properties.View = System.Windows.Forms.View.Details;
      // 
      // columnKeys
      // 
      this.columnKeys.Text = "Nom";
      // 
      // columnValues
      // 
      this.columnValues.Text = "Valeur";
      this.columnValues.Width = 12;
      // 
      // splitter2
      // 
      this.splitter2.BackColor = System.Drawing.SystemColors.ButtonShadow;
      this.splitter2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
      this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
      this.splitter2.Location = new System.Drawing.Point( 0, 82 );
      this.splitter2.Name = "splitter2";
      this.splitter2.Size = new System.Drawing.Size( 536, 3 );
      this.splitter2.TabIndex = 3;
      this.splitter2.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 7, 3 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 170, 13 );
      this.label1.TabIndex = 2;
      this.label1.Text = "Arbre des exceptions enveloppées";
      // 
      // centerPanel
      // 
      this.centerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
      this.centerPanel.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
      this.centerPanel.Location = new System.Drawing.Point( 0, 85 );
      this.centerPanel.MinimumSize = new System.Drawing.Size( 0, 230 );
      this.centerPanel.Name = "centerPanel";
      this.centerPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // centerPanel.Panel1
      // 
      this.centerPanel.Panel1.Controls.Add( this.application );
      this.centerPanel.Panel1.Controls.Add( this.label2 );
      this.centerPanel.Panel1.Controls.Add( this.label1 );
      this.centerPanel.Panel1.Controls.Add( this.tree );
      this.centerPanel.Panel1MinSize = 60;
      // 
      // centerPanel.Panel2
      // 
      this.centerPanel.Panel2.Controls.Add( this.pages );
      this.centerPanel.Panel2MinSize = 120;
      this.centerPanel.Size = new System.Drawing.Size( 536, 281 );
      this.centerPanel.SplitterDistance = 138;
      this.centerPanel.TabIndex = 7;
      // 
      // application
      // 
      this.application.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.application.AutoEllipsis = true;
      this.application.AutoSize = true;
      this.application.Location = new System.Drawing.Point( 72, 120 );
      this.application.MaximumSize = new System.Drawing.Size( 0, 13 );
      this.application.Name = "application";
      this.application.Size = new System.Drawing.Size( 35, 13 );
      this.application.TabIndex = 4;
      this.application.Text = "label3";
      this.application.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // label2
      // 
      this.label2.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 7, 120 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 68, 13 );
      this.label2.TabIndex = 3;
      this.label2.Text = " Application :";
      // 
      // pages
      // 
      this.pages.Anchor = ( (System.Windows.Forms.AnchorStyles) ( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                  | System.Windows.Forms.AnchorStyles.Left )
                  | System.Windows.Forms.AnchorStyles.Right ) ) );
      this.pages.Controls.Add( this.pageProperties );
      this.pages.Controls.Add( this.PageStack );
      this.pages.Location = new System.Drawing.Point( 3, 3 );
      this.pages.Name = "pages";
      this.pages.SelectedIndex = 0;
      this.pages.Size = new System.Drawing.Size( 530, 133 );
      this.pages.TabIndex = 5;
      // 
      // pageProperties
      // 
      this.pageProperties.Controls.Add( this.properties );
      this.pageProperties.Location = new System.Drawing.Point( 4, 22 );
      this.pageProperties.Name = "pageProperties";
      this.pageProperties.Padding = new System.Windows.Forms.Padding( 3 );
      this.pageProperties.Size = new System.Drawing.Size( 522, 107 );
      this.pageProperties.TabIndex = 0;
      this.pageProperties.Text = "Propriétés";
      this.pageProperties.UseVisualStyleBackColor = true;
      // 
      // PageStack
      // 
      this.PageStack.Controls.Add( this.stack );
      this.PageStack.Controls.Add( this.stackTools );
      this.PageStack.Location = new System.Drawing.Point( 4, 22 );
      this.PageStack.Name = "PageStack";
      this.PageStack.Padding = new System.Windows.Forms.Padding( 3 );
      this.PageStack.Size = new System.Drawing.Size( 522, 107 );
      this.PageStack.TabIndex = 1;
      this.PageStack.Text = "Trace de pile";
      this.PageStack.UseVisualStyleBackColor = true;
      // 
      // stack
      // 
      this.stack.Dock = System.Windows.Forms.DockStyle.Fill;
      this.stack.ForeColor = System.Drawing.Color.Black;
      this.stack.Location = new System.Drawing.Point( 3, 23 );
      this.stack.Name = "stack";
      this.stack.Size = new System.Drawing.Size( 516, 81 );
      this.stack.TabIndex = 0;
      // 
      // stackTools
      // 
      this.stackTools.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.btStackCollapseAll,
            this.btStackExpandAll} );
      this.stackTools.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
      this.stackTools.Location = new System.Drawing.Point( 3, 3 );
      this.stackTools.Name = "stackTools";
      this.stackTools.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.stackTools.Size = new System.Drawing.Size( 516, 20 );
      this.stackTools.TabIndex = 1;
      this.stackTools.Text = "toolStrip1";
      // 
      // btStackCollapseAll
      // 
      this.btStackCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btStackCollapseAll.Image = ( (System.Drawing.Image) ( resources.GetObject( "btStackCollapseAll.Image" ) ) );
      this.btStackCollapseAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btStackCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btStackCollapseAll.Name = "btStackCollapseAll";
      this.btStackCollapseAll.Size = new System.Drawing.Size( 23, 17 );
      this.btStackCollapseAll.Text = "toolStripButton1";
      this.btStackCollapseAll.ToolTipText = "Replier tous les noeuds de l\'arbre";
      this.btStackCollapseAll.Click += new System.EventHandler( this.btStackCollapseAll_Click );
      // 
      // btStackExpandAll
      // 
      this.btStackExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btStackExpandAll.Image = ( (System.Drawing.Image) ( resources.GetObject( "btStackExpandAll.Image" ) ) );
      this.btStackExpandAll.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btStackExpandAll.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btStackExpandAll.Name = "btStackExpandAll";
      this.btStackExpandAll.Size = new System.Drawing.Size( 23, 17 );
      this.btStackExpandAll.Text = "toolStripButton2";
      this.btStackExpandAll.ToolTipText = "Déplier tous les noeuds de l\'arbre";
      this.btStackExpandAll.Click += new System.EventHandler( this.btStackExpandAll_Click );
      // 
      // ExceptionBox
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 536, 366 );
      this.Controls.Add( this.centerPanel );
      this.Controls.Add( this.splitter2 );
      this.Controls.Add( this.firstPanel );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Name = "ExceptionBox";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ExceptionBox";
      this.Resize += new System.EventHandler( this.ExceptionBox_Resize );
      this.firstPanel.ResumeLayout( false );
      this.firstPanel.PerformLayout();
      ( (System.ComponentModel.ISupportInitialize) ( this.iconPicture ) ).EndInit();
      this.centerPanel.Panel1.ResumeLayout( false );
      this.centerPanel.Panel1.PerformLayout();
      this.centerPanel.Panel2.ResumeLayout( false );
      ( (System.ComponentModel.ISupportInitialize) ( this.centerPanel ) ).EndInit();
      this.centerPanel.ResumeLayout( false );
      this.pages.ResumeLayout( false );
      this.pageProperties.ResumeLayout( false );
      this.PageStack.ResumeLayout( false );
      this.PageStack.PerformLayout();
      this.stackTools.ResumeLayout( false );
      this.stackTools.PerformLayout();
      this.ResumeLayout( false );

    }

    #endregion

    private System.Windows.Forms.Panel firstPanel;
    private System.Windows.Forms.ListView properties;
    private System.Windows.Forms.ColumnHeader columnKeys;
    private System.Windows.Forms.ColumnHeader columnValues;
    private System.Windows.Forms.TreeView tree;
    private System.Windows.Forms.Button btShowDetails;
    private System.Windows.Forms.Button btOk;
    private System.Windows.Forms.Splitter splitter2;
    private System.Windows.Forms.PictureBox iconPicture;
    private System.Windows.Forms.Label lbMessage;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.SplitContainer centerPanel;
    private System.Windows.Forms.TabControl pages;
    private System.Windows.Forms.TabPage pageProperties;
    private System.Windows.Forms.TabPage PageStack;
    private System.Windows.Forms.ToolStrip stackTools;
    private System.Windows.Forms.TreeView stack;
    private System.Windows.Forms.ToolStripButton btStackCollapseAll;
    private System.Windows.Forms.ToolStripButton btStackExpandAll;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label application;
    private System.Windows.Forms.Label lbMessageContainer;
  }
}