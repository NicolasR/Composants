/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 28 05 2010 : utilisation de TextRenderer au lieu de Graphics.DrawString
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Windows.Forms;
using Psl;
using Psl.Controls;
using Psl.Windows;

namespace Psl.Drawing {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                        Peintre d'items pour les listes d'items enrichis                     //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Peintre pour les listes d'items enrichis <see cref="RichItem"/>.
  /// </summary>
  public class RichListPainter : IRichListPainter {

    // contrôle hôte de la peinture
    private IRichListPainterHost host;

    // espace vertical minimal à laisser au texte entre deux items pour qu'ils ne collent pas
    private const int textVSpace = 2;

    // espace vertical entre les images pour qu'elles ne collent pas
    private const int imageVSpace = 1; 

    // seuil à partir duquel des images seront étirées
    // les images seront étirées si le nombre de pixels qui leur manque est inférieur ou ègal à ce seuil
    private const int imageVStretchMin = 4;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="host">référence non null sur le contrôle hôte de la peinture</param>
    public RichListPainter( IRichListPainterHost host ) {
      this.host = host;
    }

    /// <summary>
    /// Calcul de la hauteur en pixels d'un élément de la liste
    /// </summary>
    /// <param name="e">descripteur issue d'un événement MeasureItem de l'hôte</param>
    /// <param name="item">référence non null sur l'item à peindre</param>
    public void MeasureRichItem( MeasureItemEventArgs e, RichItem item ) {

      // déterminer la font associée à l'item
      Font itemFont = item.Font;
      if ( itemFont == null ) itemFont = host.Font;

      // dimensions requises pour le texte
      Size textMetrics = TextRenderer.MeasureText( item.Text, itemFont );
      e.ItemHeight = textMetrics.Height;
      e.ItemWidth = textMetrics.Width;

      // forcer les chaînes vides à être visibles
      if ( e.ItemHeight == 0 )
        e.ItemHeight = (int) System.Math.Ceiling( itemFont.GetHeight( e.Graphics ) );

      // assurer l'espce vertical entre les items en ce qui concerne le texte
      e.ItemHeight += textVSpace;

      // considérer l'image si nécessaire
      Image itemImage = host.ImageShow ? item.Image : null;
      if (itemImage == null) return ;

      // éviter de stretcher les images pour un petit nombre de pixels
      int imageMissing = itemImage.Height + imageVSpace - e.ItemHeight;
      if ( 0 < imageMissing && imageMissing < imageVStretchMin ) 
        e.ItemHeight = itemImage.Height + imageVSpace;
    }

    /// <summary>
    /// Peinture d'un élément de la liste déroulante
    /// </summary>
    /// <param name="e">descripteur issu d'un événement DrawItem de l'hôte</param>
    /// <param name="item">référence non null sur l'item à peindre</param>
    public void DrawRichItem( DrawItemEventArgs e, RichItem item ) {

      // peinture du fond de l'item ou de l'indication de sélection
      e.DrawBackground();

      // indentation du texte
      int textIndent = item.IndentLevel * host.IndentWidth;

      // espace à réserver pour l'image
      int imageWidth = 0;
      int imageOffset = 0;

      if ( host.ImageShow ) {
        imageWidth = e.Bounds.Height;
        imageOffset = 2;
      }

      // déterminer la font associée à l'item
      Font itemFont = item.Font;
      if ( itemFont == null ) itemFont = host.Font;

      // déterminer la hauteur du texte en pixels
      float textHeight = host.DrawMode == DrawMode.OwnerDrawFixed ? host.Font.GetHeight( e.Graphics) : itemFont.GetHeight( e.Graphics );
      textHeight = (float) Math.Floor( textHeight - 1 );

      // déterminer la font d'affichage
      // todo (RichListPainter) : optimiser l'obtention de la font
      Font textFont = new Font( itemFont.FontFamily, textHeight, itemFont.Style, GraphicsUnit.Pixel );

      // déterminer le calage du texte
      int textTop = textHeight > e.Bounds.Height ? 0 : (int) (e.Bounds.Height - textHeight) / 2;
      int textWidth = e.Bounds.Width - imageWidth - textIndent - imageOffset;
      int textLeft = host.IsRightToLeft ? e.Bounds.X - imageOffset : e.Bounds.X + imageWidth + textIndent + imageOffset;
      Rectangle textRect = new Rectangle( textLeft, e.Bounds.Y, textWidth, e.Bounds.Height );
      TextFormatFlags textFlags = host.IsRightToLeft ? TextFormatFlags.Right : TextFormatFlags.Left;

      // peindre le texte
      TextRenderer.DrawText( e.Graphics, item.Text, textFont, textRect, item.ForeColor, textFlags );

      // partie image
      Image itemImage = host.ImageShow ? item.Image : null;
      if ( itemImage == null ) return;

      // inverser l'image si orientation droite à gauche 
      // utiliser un clone de l'image pour éviter les effets de bord 
      //if ( host.IsRightToLeft ) {
      //  itemImage = itemImage.Clone() as Image;
      //  itemImage.RotateFlip( RotateFlipType.RotateNoneFlipX );
      //}

      // éviter de stretcher les images pour un petit nombre de pixels
      int imageRemaining = e.Bounds.Height - imageVSpace - itemImage.Height;
      int imageSize = imageRemaining < 0 || imageVStretchMin < imageRemaining ? e.Bounds.Height - imageVSpace : itemImage.Height;

      // ajuster le calage de l'image
      int imageLeft = host.IsRightToLeft ? e.Bounds.Width - (imageOffset + itemImage.Width + textIndent) : imageOffset + textIndent;
      int imageTop = imageSize >= e.Bounds.Height ? 0 : (e.Bounds.Height - imageSize) / 2;
      Rectangle imageRect = new Rectangle( e.Bounds.X + imageLeft, e.Bounds.Y + imageTop, imageSize, imageSize );

      // peindre l'image
      e.Graphics.DrawImage( itemImage, imageRect );
    }
  }
}
