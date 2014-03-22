/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Drawing;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  // Regroupement de déclarations liées aux méthodes natives Windows
  public partial class Win {

    /// <summary>
    /// Regroupement d'utilitaires liées aux méthodes natives de Windows
    /// </summary>
    public class Util {

      public static Point LParamToPoint( int lparam ) {
        return new Point( lparam & 0xFFFF, (lparam & 0x7FFF0000) >> 16 );
      }

      public static int PointToLParam( Point point ) {
        return (point.X & 0xFFFF) | ((point.Y & 0xFFFF) << 16);
      }

      public static short LOWORD( int dw ) {
        short loWord = 0;
        ushort andResult = (ushort) (dw & 0x00007FFF);
        ushort mask = 0x8000;
        if ( (dw & 0x8000) != 0 )
          loWord = (short) (mask | andResult);
        else
          loWord = (short) andResult;
        return loWord;
      }

      public static int MAKELONG( int wLow, int wHigh ) {
        int low = (int) LOWORD( wLow );
        short high = LOWORD( wHigh );
        int product = 0x00010000 * (int) high;
        int makeLong = (int) (low | product);
        return makeLong;
      }

      public static int SignedHIWORD( int n ) {
        int num = (short) ((n >> 0x10) & 0xffff);
        num = num << 0x10;
        return (num >> 0x10);
      }

      public static int SignedLOWORD( int n ) {
        int num = (short) (n & 0xffff);
        num = num << 0x10;
        return (num >> 0x10);
      }

    }

  }
}
