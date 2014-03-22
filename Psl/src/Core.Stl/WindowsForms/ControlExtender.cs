/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 29 10 2009 : version inititiale
 * 13 11 2009 : amélioration de IsInActiveChain
 */                                                                            // <wao never.end>
using System.Windows.Forms;

namespace Psl.Controls {

  /// <summary>
  /// Classe d'extension pour la classe <see cref="Control"/>.
  /// </summary>
  public static class ControlExtender {

    private static bool IsInParentChain( Control control, Control anchor ) {
      if ( control == null ) return false;
      while (anchor != null) {
        if ( control == anchor ) return true;
        anchor = anchor.Parent;
      }
      return false;
    }

    /// <summary>
    /// Obtient le contrôle actif du formulaire hébergeant le contrôle this
    /// </summary>
    /// <param name="self">référence sur le contrôle déterminant le formulaire</param>
    /// <returns>la référence sur le contrôle courant ou null</returns>
    public static Control GetFormActiveControl( this Control self ) {
      Control result = null;
      if ( self == null ) return result;

      Control host = self.TopLevelControl;
      ContainerControl container = host as ContainerControl;
      while ( container != null ) {
        result = container.ActiveControl;
        container = result as ContainerControl;
      }

      return result;
    }

    /// <summary>
    /// Obtient le dernier contrôle actif du formulaire hébergeant le contrôle this
    /// </summary>
    /// <param name="self">référence sur le contrôle déterminant le formulaire</param>
    /// <returns>la référence sur le dernier contrôle actif ou null</returns>
    public static Control GetFormLastActiveControl( this Control self ) {
      Control result = null;
      if ( self == null ) return result;

      Control host = self.TopLevelControl;
      ContainerControl container = host as ContainerControl;
      while ( container != null ) {
        Control active = container.ActiveControl;
        if ( active == null ) break;
        result = active;
        container = result as ContainerControl;
      }

      return result;
    }

    /// <summary>
    /// Détermine si le contrôle this est dans la chaîne des contrôles actifs de sa fenêtre hôte.
    /// </summary>
    /// <param name="self">la référence sur le contrôle</param>
    /// <returns>true si le contrôle est dans la chaîne active de sa fenêtre hôte</returns>
    public static bool IsInActiveChain( this Control self ) {
      return IsInParentChain( self, GetFormLastActiveControl( self ) );
      //return IsInParentChain( self, GetFormActiveControl( self ) );
    }
  }
}
