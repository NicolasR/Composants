/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System.Windows.Forms;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  // Regroupement de déclarations liées aux méthodes natives Windows
  public partial class Win {

    /// <summary>
    /// Utilitaires pour la gestion des UICues (keyboard et focus)
    /// </summary>
    public class UICues {

      protected const int UIS_SET = 1;
      protected const int UIS_CLEAR = 2;

      protected const short UISF_HIDEFOCUS = 0x0001;
      protected const short UISF_HIDEACCEL = 0x0002;
      protected const short UISF_ACTIVE = 0x0004;

      public static void MakeAcceleratorsVisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_CLEAR, UISF_HIDEACCEL ), 0 );
      }

      public static void MakeAcceleratorsInvisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_SET, UISF_HIDEACCEL ), 0 );
      }

      public static void MakeFocusVisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_CLEAR, UISF_HIDEFOCUS ), 0 );
      }

      public static void MakeFocusInvisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_SET, UISF_HIDEFOCUS ), 0 );
      }

      public static void MakeActiveVisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_SET, UISF_ACTIVE ), 0 );
      }

      public static void MakeActiveInvisible( Control c ) {
        Win.SendMessage( c.Handle, WM_CHANGEUISTATE, Win.Util.MAKELONG( UIS_CLEAR, UISF_ACTIVE ), 0 );
      }
    }
  }
} 


