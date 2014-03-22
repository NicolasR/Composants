using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Psl.Controls {

  /// <summary>
  /// Barre reflétant l'activité du composant hébergeant le barre.
  /// </summary>
  public class ActiveBar : Panel {

    /// <summary>
    /// Couleur par défaut de la barre lorsque le parent est actif
    /// </summary>
    public static readonly Color DefaultActiveColor = Color.Blue;

    /// <summary>
    /// Couleur par défaut de la barre lorsque le parent est inactif
    /// </summary>
    public static readonly Color DefaultInactiveColor = Color.Silver;

    // variable nécessaire au concepteur.
    private System.ComponentModel.IContainer components = null;

    // true si le dernier examen indiquait que le parent état actif
    private bool isActive = false;

    // true jusqu'à ce que l'activité du parent ait été testée une première fois
    private bool firstCheck = true;

    // couleur de la barre lorsque le parent est actif
    private Color activeColor = DefaultActiveColor;

    // couleur de la barre lorsque le parent est inactif
    private Color inactiveColor = DefaultInactiveColor;

    /// <summary>
    /// Initialisation de l'objet. 
    /// </summary>
    public ActiveBar() {
      InitializeComponent();
      AutoSize = false;
      Text = string.Empty;
      Dock = DockStyle.Top;
      Height = 3;
      BackColor = DefaultInactiveColor;
      Application.Idle += OnIdle;
    }

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
    }

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose( bool disposing ) {
      if ( disposing ) Application.Idle -= OnIdle;
      if ( disposing && (components != null) ) components.Dispose();
      base.Dispose( disposing );
    }

    private void DoUpdateState() {
      Color color = isActive ? activeColor : inactiveColor;
      if ( BackColor == color ) return;
      BackColor = color;
    }

    private void DoCheckActive() {
      if ( DesignMode || Parent == null ) return;
      bool nowActive = Parent.IsInActiveChain();
      if ( !firstCheck && nowActive == isActive ) return;

      firstCheck = false;
      isActive = nowActive;
      DoUpdateState();
      OnActiveChanged();
    }

    private void OnIdle( object sender, EventArgs e ) {
      if ( DesignMode ) return;
      DoCheckActive();
    }

    /// <summary>
    /// Déclenchement centralisé de l'événement <see cref="ActiveChanged"/>
    /// </summary>
    protected virtual void OnActiveChanged() {
      if ( ActiveChanged != null ) ActiveChanged( this, EventArgs.Empty );
    }

    /// <summary>
    /// Indique si le parent de la barre est dans la chaîne des composants actifs.
    /// </summary>
    [Browsable(false)]
    public bool IsActive {
      get {
        DoCheckActive();
        return isActive;
      }
    }

    private bool ShouldSerializeActiveColor() {
      return ActiveColor != DefaultActiveColor;
    }

    /// <summary>
    /// Obtient ou détermine la couleur de la barre quand le parent est actif
    /// </summary>
    [
    Description( "Couleur de la barre quand le parent est actif" ),
    ]
    public Color ActiveColor {
      get {
        return activeColor;
      }
      set {
        if ( activeColor == value ) return;
        activeColor = value;
        DoCheckActive();
      }
    }

    private bool ShouldSerializeInactiveColor() {
      return InactiveColor != DefaultInactiveColor;
    }

    /// <summary>
    /// Obtient ou détermine la couleur de la barre quand le parent est inactif
    /// </summary>
    [
    Description( "Couleur de la barre quand le parent est inactif" ),
    ]
    public Color InactiveColor {
      get {
        return inactiveColor;
      }
      set {
        if ( inactiveColor == value ) return;
        inactiveColor = value;
        DoCheckActive();
      }
    }

    /// <summary>
    /// Evénement déclenché quand l'état d'activité du parent a changé.
    /// </summary>
    [
    Description( "Déclenché quand l'état d'activité du parent a changé" ),
    ]
    public event EventHandler ActiveChanged;
  }
}
