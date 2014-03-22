/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 * 15 10 2010 : adjonction du filtrage de AutoDragTabs
 * 13 02 2011 : adjonction du filtrage de AutoDragStart
 */                                                                            // <wao never.end>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Psl.Design;
using Psl.Windows;
using Psl.Controls;

namespace Psl.Design {

  /// <summary>
  /// Designer pour le contrôle <see cref="TabDocker"/>
  /// </summary>
  public class TabDockerDesigner : ParentControlDesignerCache<TabDocker, TabDockerDesigner> {

    /// <summary>
    /// Descripteur du verbe d'adjonction d'une page
    /// </summary>
    private DesignerVerb verbAddPage = null ;

    /// <summary>
    /// Descripteur du verbe de suppression d'une page
    /// </summary>
    private DesignerVerb verbRemovePage = null;

    /// <summary>
    /// Descripteur du verbe d'édition de la collection des pages
    /// </summary>
    private DesignerVerb verbEditPages = null;

    /// <summary>
    /// Constructeur
    /// </summary>
    public TabDockerDesigner() {
      verbAddPage    = cache.AddVerb( new DesignerVerb( "Ajouter un onglet docker"     , DoVerbAddPage    ) );
      verbRemovePage = cache.AddVerb( new DesignerVerb( "Supprimer l'onglet docker"    , DoVerbRemovePage ) );
      verbEditPages  = cache.AddVerb( new DesignerVerb( "Gérer la collection des pages", DoVerbEditPages  ) );
    }

    /// <summary>
    /// Mise à jour de l'état des verbes
    /// </summary>
    protected override void DoUpdateVerbs() {
      verbRemovePage.Enabled = cache.Control.Controls.Count > 0;
    }

    //
    // Redéfinition de membres hérités
    //

    /// <summary>
    /// Indique si le contrôle spécifié peut être un enfant du contrôle managé par ce concepteur.
    /// </summary>
    /// <param name="control"><see cref="Control"/> à tester</param>
    /// <returns>true si le contrôle spécifié peut être un enfant du contrôle managé par ce concepteur ; sinon, false</returns>
    public override bool CanParent( Control control ) {
      return control is TabDockerPage;
    }

    /// <summary>
    /// Indique si le contrôle managé par le concepteur spécifié peut être un enfant du contrôle managé par ce concepteur.
    /// </summary>
    /// <param name="designer">concepteur du contrôle à tester</param>
    /// <returns>true si le contrôle managé par le concepteur spécifié peut être un enfant du contrôle managé par ce concepteur ; sinon, false</returns>
    public override bool CanParent( ControlDesigner designer ) {
      return designer is TabDockerPageDesigner;
    }

    /// <summary>
    /// Redéfinition de WndProc pour l'affichage du menu de page
    /// </summary>
    /// <param name="msg">message Windows</param>
    protected override void WndProc( ref Message msg ) {
      switch ( msg.Msg ) {

        // ouvrir le menu contextuel lors d'un clic droit sur un onglet
        case Win.WM_CONTEXTMENU:
          int x = Win.Util.SignedLOWORD( (int) msg.LParam );
          int y = Win.Util.SignedHIWORD( (int) msg.LParam );
          if ( (x == -1) && (y == -1) ) {
            Point position = Cursor.Position;
            x = position.X;
            y = position.Y;
          }
          OnContextMenu( x, y );
          return;

        // sélectionner le tab control quand le clic a lieu en-dehors des onglets
        case Win.WM_NCHITTEST:
          base.WndProc( ref msg );
          if ( msg.Result.ToInt32() == Win.HTTRANSPARENT ) msg.Result = (IntPtr) Win.HTCLIENT;
          break;

        // autres cas
        default:
          base.WndProc( ref msg );
          break;
      }
    }

    /// <summary>
    /// Redéfinition de GetHitTest pour déterminer si le contrôle doit gérer un clic souris à cet endroit.
    /// </summary>
    /// <param name="point">coordonnées du clic souris</param>
    /// <returns>true si le contrôle doit gérer un clic souris, false sinon</returns>
    protected override bool GetHitTest( Point point ) {
      if ( cache.SelectionService.PrimarySelection != this.Control ) return false;

      Win.TCHITTESTINFO hti = new Win.TCHITTESTINFO();

      hti.point = this.Control.PointToClient( point );
      hti.flags = 0;

      Message msg = new Message();
      msg.HWnd = this.Control.Handle;
      msg.Msg = Win.TCM_HITTEST;

      IntPtr lparam = Marshal.AllocHGlobal( Marshal.SizeOf( hti ) );
      Marshal.StructureToPtr( hti, lparam, false );
      msg.LParam = lparam;

      base.WndProc( ref msg );
      Marshal.FreeHGlobal( lparam );

      if ( msg.Result.ToInt32() != -1 ) return hti.flags != Win.TabControlHitTest.TCHT_NOWHERE;
      return false;
    }

    //
    // Propriétés filtrées
    //
    // AllowDrop est une propriété filtrée dans ControlDesigner, d'où le filtrage des
    // propriétés AutoDragStart et AutoDragTabs qui doivent pouvoir affecter AllowDrop.
    //

    private bool AutoDragStart {
      get {
        return (bool) base.ShadowProperties[ "AutoDragStart" ];
      }
      set {
        base.ShadowProperties[ "AutoDragStart" ] = value;
        if ( value ) base.ShadowProperties[ "AllowDrop" ] = true;
      }
    }

    private bool AutoDragTabs {
      get {
        return (bool) base.ShadowProperties[ "AutoDragTabs" ];
      }
      set {
        base.ShadowProperties[ "AutoDragTabs" ] = value;
        if ( value ) AutoDragStart = true;
      }
    }

    /// <summary>
    /// Substitution des descripteurs des propriétés filtrées
    /// </summary>
    /// <param name="properties">dictionnaire des propriétés du composant</param>
    protected override void PreFilterProperties( IDictionary properties ) {
      base.PreFilterProperties( properties );

      string[] strArray = new string[] { "AutoDragStart", "AutoDragTabs" };
      Attribute[] attributes = new Attribute[ 0 ];

      for ( int i = 0 ; i < strArray.Length ; i++ ) {
        PropertyDescriptor oldPropertyDescriptor = (PropertyDescriptor) properties[ strArray[ i ] ];
        if ( oldPropertyDescriptor != null ) {
          properties[ strArray[ i ] ] = TypeDescriptor.CreateProperty( typeof( TabDockerDesigner ), oldPropertyDescriptor, attributes );
        }
      }
    }
 
    //
    // Verbes
    //

    /// <summary>
    /// Réalisation du verbe d'adjonction d'une page
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbAddPage( Object sender, EventArgs e ) {

      // création de la transaction
      DesignerTransaction transaction;
      if ( !cache.CreateTransaction( out transaction, "Adjonction d'un onglet docker à {0}" ) ) return;
      try {

        MemberDescriptor member = TypeDescriptor.GetProperties( Control )[ "Controls" ];

        // création de la page
        TabDockerPage page = cache.DesignerHost.CreateComponent( typeof( TabDockerPage ) ) as TabDockerPage;

        // notifier le commencement des changements apportés au docker
        RaiseComponentChanging( member );

        // affecter la propriété Text à partir de Name
        cache.AssignPropertyValue( page, "Name", page, "Text", typeof( string ), false );

        // ajouter la page au docker
        Control.Controls.Add( page );

        // notifier la fin des changements apportés au docker
        RaiseComponentChanged( member, null, null );

        // sélectionner la page créée
        cache.Control.SelectedDockerTab = page;
        cache.SelectionService.SetSelectedComponents( new IComponent[] { page } );
      }
      finally {
        transaction.Commit();
      }
    }

    /// <summary>
    /// Réalisation du verbe de suppression d'une page
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbRemovePage( Object sender, EventArgs e ) {
      if ( cache.Control.SelectedDockerTab == null ) return;

      // création de la transaction
      DesignerTransaction transaction;
      if ( !cache.CreateTransaction( out transaction, "Suppression d'un onglet docker à {0}" ) ) return;
      try {

        // notifier le commencement des changements apportés au docker
        MemberDescriptor member = TypeDescriptor.GetProperties( Control )[ "Controls" ];
        RaiseComponentChanging( member );

        // supprimer la page
        cache.DesignerHost.DestroyComponent( cache.Control.SelectedDockerTab );

        // notifier la fin des changements apportés au docker
        RaiseComponentChanged( member, null, null );

        // sélectionner l'onglet qui suivait l'onglet supprimé
        Control toBeSelected = cache.Control.SelectedTab == null ? Control : cache.Control.SelectedTab;
        cache.SelectionService.SetSelectedComponents( new IComponent[] { toBeSelected } );
      }
      finally {
        transaction.Commit();
      }
    }

    /// <summary>
    /// Implémente le verbe d'édition de la collection des pages.
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    internal void DoVerbEditPages( object sender, EventArgs e ) {
      cache.EditCollection( "TabPages" );
    }
  }
}
