/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 02 11 2010 : version inititiale pour ControlExtender
 * 17 02 2011 : version initiale pour Range
 */                                                                           // <wao never.end>
using System;
using System.Windows.Forms;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                  ControlExtender : classe d'extension pour les contrôles                    //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe d'extension fournissant des méthodes de service pour les <see cref="Control"/>.
  /// </summary>
  public static class ControlExtender {

    /// <summary>
    /// Tenter de rendre le contrôle this le contrôle actif de l'application
    /// </summary>
    /// <remarks>
    /// Cette méthode d'extension a principalement pour but de prendre charge le problème des pages
    /// des <see cref="TabControl"/> : si un contrôle est directement ou indirectement contenu dans
    /// une page <see cref="TabPage"/> qui n'est pas la page courante d'un contrôleur de pages
    /// (propriété <see cref="TabControl.SelectedTab"/>), ce contrôle ne peut pas être rendu visible
    /// par les méthodes ordinaires de la classe <see cref="Control"/>. Il faut donc parcourir la chaîne
    /// des parents du contrôle à activer pour forcer éventuellement la page courante des contrôleurs
    /// de pages qui en sont les conteneurs indirects.
    /// <br/>
    /// Cette méthode d'extension force aussi à true la propriété <see cref="Control.Visible"/> 
    /// de tous les conteneurs direct et indirects du contrôle à activer. 
    /// </remarks>
    /// <param name="zis">le contrôle à rendre actif</param>
    /// <exception cref="ArgumentNullException">la référence sur le contrôle à activer est null</exception>
    public static void TryActivate( this Control zis ) {
      if ( zis == null ) throw new ArgumentNullException( "zis" );

      if ( !zis.Visible ) zis.Visible = true;

      Control root = null;
      Control active = zis;
      Control anchor = zis.Parent;

      while ( anchor != null ) {
        if ( !anchor.Visible ) anchor.Visible = true;

        // cas des ContainerControl : agir sur la propriété ActiveControl
        IContainerControl anchorAsIContainerControl = anchor as IContainerControl;
        if ( anchorAsIContainerControl != null ) {
          if ( anchorAsIContainerControl.ActiveControl != active ) anchorAsIContainerControl.ActiveControl = active;
          active = anchor;
        }
        else {

          // cas des pages des contrôleurs de page : sélectionner la page
          TabPage anchorAsTabPage = anchor as TabPage;
          if ( anchorAsTabPage != null ) {
            TabControl anchorParentAsTabControl = anchor.Parent as TabControl;
            if ( anchorParentAsTabControl != null )
              if ( anchorParentAsTabControl.SelectedTab != anchorAsTabPage ) anchorParentAsTabControl.SelectedTab = anchorAsTabPage;
          }
        }

        // passer au conteneur en prélevant le contrôle présumé top-level
        Control anchorParent = anchor.Parent;
        if ( anchorParent == null ) root = anchor;
        anchor = anchorParent;
      }

      // vérifier que le contrôle présumé top-level est du type Form
      Form rootAsForm = root as Form;
      if ( rootAsForm == null ) return;

      if ( !rootAsForm.Visible ) rootAsForm.Visible = true;
      if ( rootAsForm.WindowState == FormWindowState.Minimized )
        rootAsForm.WindowState = FormWindowState.Normal;

      rootAsForm.Activate();
    }

    /// <summary>
    /// Obtient le premier conteneur <see cref="IContainerControl"/> d'un contrôle en remontant la chaîne de ses parents.
    /// </summary>
    /// <param name="zis">contrôle à partir duquel s'effectue la recherche</param>
    /// <returns>le premier conteneur <see cref="IContainerControl"/>, ou null si introuvable</returns>
    /// <exception cref="ArgumentException">la référence sur le contrôle concerné est null</exception>
    public static IContainerControl GetFirstContainerControl( this Control zis ) {
      if ( zis == null ) throw new ArgumentNullException( "zis" );

      Control anchor = zis;
      IContainerControl container = null;
      while ( anchor != null && container == null ) {
        anchor = anchor.Parent;
        container = anchor as IContainerControl;
      }

      return container;
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Structure Range pour OpticalZoomRange                           //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Structure permettant de déterminer les bornes minimales et maximales d'un intervalle
  /// </summary>
  public struct Range {

    /// <summary>
    /// Borne minimale de l'intervalle
    /// </summary>
    public int LowerBound;

    /// <summary>
    /// Borne maximale de l'intervalle
    /// </summary>
    public int HigherBound;

    /// <summary>
    /// Obtient la différence entre la borne mainimale et la borne maximale
    /// </summary>
    public int Difference {
      get { return HigherBound - LowerBound; }
    }

    /// <summary>
    /// Détermine si une valeur figure dans l'intervalle (bornes comprises) ;
    /// </summary>
    /// <param name="value">valeur à tester</param>
    /// <returns>true si la valeur est dans l'intervalle bornes comprises</returns>
    public bool Contains( int value ) {
      return LowerBound <= value && value <= HigherBound;
    }

    /// <summary>
    /// Initialisation de la structure
    /// </summary>
    /// <param name="lowerBound">borne inférieure de l'intervalle</param>
    /// <param name="higherBound">borne supérieure de l'intervalle</param>
    public Range( int lowerBound, int higherBound )
      : this() {
      LowerBound = lowerBound;
      HigherBound = higherBound;
    }
  }

}
