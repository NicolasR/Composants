/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 11 2008 : version initiale
 * 12 10 2009 : intégration de ImageToCrossCursor
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Windows.Forms;
using Psl.Windows;

namespace Psl.Controls {

  /// <summary>
  /// Classe de service pour la gestion des curseurs
  /// </summary>
  public class CursorHelper {                                                  // <wao spécif>

    /// <summary>
    /// Fournit un curseur à partir d'un bitmap en spécifiant le "hot spot"
    /// </summary>
    /// <param name="bmp">bitmap à partir duquel générer le curseur</param>
    /// <param name="xHotSpot">position horizontale du "hot spot" dans le curseur</param>
    /// <param name="yHotSpot">position verticale du "hot spot" dans le curseur</param>
    /// <returns></returns>
    public static Cursor FromBitmap( Bitmap bmp, int xHotSpot, int yHotSpot ) { // <wao spécif code.&body>
      Win.IconInfo iconInfo = new Win.IconInfo();
      IntPtr hIcon = bmp.GetHicon();
      Win.GetIconInfo( hIcon, ref iconInfo );
      Win.DeleteObject( hIcon );
      iconInfo.xHotspot = xHotSpot;
      iconInfo.yHotspot = yHotSpot;
      iconInfo.fIcon = false;
      return new Cursor( Win.CreateIconIndirect( ref iconInfo ) );
    }

    /// <summary>
    /// Compose un curseur 32x32 à partir d'une image (max 26x26) avec une croix 
    /// </summary>
    /// <remarks>
    /// Le curseur obtenu est dans le style curseur de dépôt de composant.
    /// L'image initiale est calée en 6:6.
    /// La croix est calée en 0:0.
    /// Le point chaud est en 4:4
    /// </remarks>
    /// <param name="image">image (max 26x26) servant de base au curseur</param>
    /// <returns>le curseur obtenu</returns>
    public static Cursor ImageToCrossCursor( Image image ) {                   // <wao spécif code.&body>
      Bitmap bmp = new Bitmap(32,32);
      using ( Graphics grf = Graphics.FromImage( bmp ) ) {
        grf.DrawImageUnscaled( image, new Point( 6, 6 ) );
        grf.DrawLine( new Pen( Color.Black ), new Point( 4, 0 ), new Point( 4, 8 ) );
        grf.DrawLine( new Pen( Color.Black ), new Point( 0, 4 ), new Point( 8, 4 ) );
      }
      return FromBitmap( bmp, 4, 4 );
    }

    /// <summary>
    /// Obtient les images et curseur associé à un composant.
    /// </summary>
    /// <param name="type">descripteur de type du composant</param>
    /// <param name="small">image bitmap 16x16 associée au composant (ou null)</param>
    /// <param name="large">image bitmap 32x32 associée au composant (ou null)</param>
    /// <param name="cursor">curseur de dépôt associé au composant (ou <see cref="Cursors.Cross"/>)</param>
    /// <returns>true si l'image du composant a pu être obtenue</returns>
    public static bool GetComponentImages(                                     // <wao spécif>
      Type type, out Image small, out Image large, out Cursor cursor ) {       // <wao spécif code.&body>

      // valeurs par défaut
      small = null;
      large = null;
      cursor = Cursors.Cross;

      // tenter de récupérer l'image du composant via l'attribut ToolboxBitmap
      object[] attrs = type.GetCustomAttributes( typeof( ToolboxBitmapAttribute ), false );
      if ( attrs.Length > 0 ) {
        small = (attrs[ 0 ] as ToolboxBitmapAttribute).GetImage( type, false );
        large = (attrs[ 0 ] as ToolboxBitmapAttribute).GetImage( type, true );
      }

      // tenter de récupérer l'image du composant via son nom par défaut
      if ( small == null ) {
        small = ToolboxBitmapAttribute.GetImageFromResource( type, type.Name + ".bmp", false );
        large = ToolboxBitmapAttribute.GetImageFromResource( type, type.Name + ".bmp", true  );
      }

      // aucune image trouvée
      if ( small == null ) return false;

      // composer le cross-cursor pour le dépôt
      cursor = CursorHelper.ImageToCrossCursor( small );
      return true;
    }
  }                                                                            // <wao spécif>
}
