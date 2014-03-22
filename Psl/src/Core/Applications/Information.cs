/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 12 2007 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace Psl.Applications {

  // <exclude
  /// <summary>
	/// Boîte générique d'affichage d'informations.
	/// </summary>
	/// <remarks>
	/// Comporte une méthode de classe pour la réalisation du dialogue
	/// </remarks>
	internal class Information : System.Windows.Forms.Form {

    #region Champs gérés par le concepteur Windows Forms

    private System.Windows.Forms.Label lbStatut;
    private System.Windows.Forms.Button btOk;
    private System.Windows.Forms.Label lbVersion;
    private System.Windows.Forms.Label lbWhat;
    private PictureBox pictureBox1;
    private System.ComponentModel.IContainer components = null;

    #endregion

    /// <summary>
    /// Déclenchement du dialogue modal par une méthode de classe
    /// </summary>
    /// <param name="what">à propos de quoi le dialogue est affiché</param>
    /// <param name="version">identification de la version associée</param>
    /// <param name="statut">statut du composant</param>
    /// <returns>code de conclusion du dialogue</returns>
    public static DialogResult ShowDialog( string what, string statut, string version ) {
      using (Form box = new Information( what, statut, version ) ) {
        return box.ShowDialog() ;
      }
    }

    /// <summary>
    /// Constructeur de la boîte de dialogue
    /// </summary>
    /// <param name="what">à propos de quoi le dialogue est affiché</param>
    /// <param name="statut">information sur le statut de la version associée</param>
    /// <param name="version">identification de la version associée</param>
    public Information( string what, string statut, string version ) {
			InitializeComponent();
      lbWhat   .Text = what    ;
      lbVersion.Text = version ;
      lbStatut .Text = statut  ;
      lbStatut.ForeColor = statut == "corrigé" ? Color.Red : Color.Blue;
		}

		#region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    protected override void Dispose( bool disposing )	{
      if( disposing )	{
        if(components != null) components.Dispose();
      }
      base.Dispose( disposing );
    }

    /// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( Information ) );
      this.lbWhat = new System.Windows.Forms.Label();
      this.lbStatut = new System.Windows.Forms.Label();
      this.lbVersion = new System.Windows.Forms.Label();
      this.btOk = new System.Windows.Forms.Button();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // lbWhat
      // 
      this.lbWhat.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lbWhat.BackColor = System.Drawing.Color.FromArgb( ((int) (((byte) (255)))), ((int) (((byte) (255)))), ((int) (((byte) (192)))) );
      this.lbWhat.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.lbWhat.Font = new System.Drawing.Font( "Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)) );
      this.lbWhat.ForeColor = System.Drawing.Color.FromArgb( ((int) (((byte) (0)))), ((int) (((byte) (0)))), ((int) (((byte) (192)))) );
      this.lbWhat.Location = new System.Drawing.Point( 12, 94 );
      this.lbWhat.Name = "lbWhat";
      this.lbWhat.Size = new System.Drawing.Size( 403, 22 );
      this.lbWhat.TabIndex = 0;
      this.lbWhat.Text = "libellé";
      this.lbWhat.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lbStatut
      // 
      this.lbStatut.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lbStatut.BackColor = System.Drawing.SystemColors.Control;
      this.lbStatut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.lbStatut.Font = new System.Drawing.Font( "Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)) );
      this.lbStatut.ForeColor = System.Drawing.Color.Black;
      this.lbStatut.Location = new System.Drawing.Point( 103, 27 );
      this.lbStatut.Name = "lbStatut";
      this.lbStatut.Size = new System.Drawing.Size( 312, 18 );
      this.lbStatut.TabIndex = 1;
      this.lbStatut.Text = "statut";
      this.lbStatut.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lbVersion
      // 
      this.lbVersion.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.lbVersion.BackColor = System.Drawing.Color.FromArgb( ((int) (((byte) (255)))), ((int) (((byte) (255)))), ((int) (((byte) (192)))) );
      this.lbVersion.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.lbVersion.Location = new System.Drawing.Point( 12, 125 );
      this.lbVersion.Name = "lbVersion";
      this.lbVersion.Size = new System.Drawing.Size( 403, 14 );
      this.lbVersion.TabIndex = 2;
      this.lbVersion.Text = "version";
      this.lbVersion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btOk
      // 
      this.btOk.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
      this.btOk.Location = new System.Drawing.Point( 174, 150 );
      this.btOk.Name = "btOk";
      this.btOk.Size = new System.Drawing.Size( 75, 22 );
      this.btOk.TabIndex = 3;
      this.btOk.Text = "Ok";
      // 
      // pictureBox1
      // 
      this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pictureBox1.Image = ((System.Drawing.Image) (resources.GetObject( "pictureBox1.Image" )));
      this.pictureBox1.Location = new System.Drawing.Point( 12, 11 );
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size( 96, 72 );
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // Information
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size( 5, 13 );
      this.ClientSize = new System.Drawing.Size( 427, 184 );
      this.Controls.Add( this.pictureBox1 );
      this.Controls.Add( this.btOk );
      this.Controls.Add( this.lbVersion );
      this.Controls.Add( this.lbStatut );
      this.Controls.Add( this.lbWhat );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "Information";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "A propos...";
      ((System.ComponentModel.ISupportInitialize) (this.pictureBox1)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }
		#endregion
	}

  // <exclude
  /// <summary>
  /// Enregistrement dans le registre d'application.
  /// </summary>
  public class InformationKeys {

    /// <summary>
    /// Obtient la clé d'enregistrement.
    /// </summary>
    public static string KeyShowDialog {
      get { return "Psl.Applications.Information.ShowDialog"; }
    }

    static InformationKeys() {
      MethodInfo method = typeof( Information ).GetMethod( "ShowDialog", new Type[] { typeof( string ), typeof( string ), typeof( string ) } );
      BaseRegistry.Add( InformationKeys.KeyShowDialog, method );
    }
  }
}
