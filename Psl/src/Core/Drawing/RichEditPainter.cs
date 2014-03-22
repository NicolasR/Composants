/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Ce fichier regroupe diverses interfaces et déclarations communes liées aux items riches
 * 
 * 21 05 2009 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Drawing;
using System.Windows.Forms;
using Psl.Windows;
using Psl.Controls;

namespace Psl.Drawing {

  /// <summary>
  /// Peintre pour une boîte d'édition riche
  /// </summary>
  /// <remarks>
  /// Lorsque la boîte d'édition fonctionne en édition, le texte est peint par le contrôle
  /// d'origine. Le texte n'est à peindre que dans les combos en DropDownList parce que le
  /// contrôle original se borne à recopier l'item tel qu'il figure dans la liste déroulante
  /// </remarks>
  public class RichEditPainter : IRichEditPainter {

    // contrôle hôte de la peinture
    private IRichListPainterHost host;

    // image prête à l'affichage ou null si aucune image à afficher *** gestion en cache ***
    private Image editDrawImage = null;

    // marge à gauche du texte tenant compte de l'image et de l'orientation *** gestion en cache ***
    private int editLeftMargin = 0;

    // marge à droite du texte tenant compte de l'image et de l'orientation *** gestion en cache ***
    private int editRightMargin = 0;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="host">référence sur le contrôle hôte de la peinture</param>
    public RichEditPainter( IRichListPainterHost host ) {
      this.host = host;
    }

    /// <summary>
    /// Invalider les informations gérées en cache
    /// </summary>
    /// <param name="hWnd">handle de la fenêtre d'édition</param>
    public void InvalidateEditImage( IntPtr hWnd ) {
      //Psl.Tracker.Tracker.Track( "RichEditPainter : InvalidateEditImage" );

      // invalider les informations gérées en cache liées à l'image
      editDrawImage = null;
      editLeftMargin = 0;
      editRightMargin = 0;

      // calculer la marge pour l'image
      if ( host.ImageShow && host.DrawMode != DrawMode.Normal )
        if ( host.IsRightToLeft )
          editRightMargin = host.ItemHeight + 4;
        else
          editLeftMargin = host.ItemHeight + 4;

       // déterminer les marges au niveau du composant
      Win.SendMessage( hWnd, Win.EM_SETMARGINS, Win.EC_LEFTMARGIN | Win.EC_RIGHTMARGIN, Win.Util.MAKELONG( editLeftMargin, editRightMargin ) );

      // invalider la surface de la fenêtre
      Win.InvalidateRect( hWnd, IntPtr.Zero, true );
    }

    /// <summary>
    /// Peinture de la boîte d'affichage/édition de la combo
    /// </summary>
    /// <remarks>
    /// Cette méthode n'est utilisée que dans le cas où la boîte est en peinture personnalisée
    /// et où le style est DropDownList. En effet, dans ce cas, la peinture standard du composant natif
    /// consiste simplement à recopier telle quelle la peinture de l'item qui figure dans la liste
    /// en le clippant aux dimension de la boîte d'édition.
    /// <br/>
    /// Cette peinture personnalisée permet de peindre le texte sans indentation et dans une dimension
    /// de font qui correspond à la dimension de la boîte.  
    /// <br/>
    /// Les calculs effectués sont destinés à rester calés sur ceux de l'affichage de la boîte d'édition
    /// tels qu'effectués dans tous les autres cas d'affichage de la combo
    /// </remarks>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle de la boîte</param>
    /// <param name="text">texte à peindre</param>
    public void DrawText( Graphics graphics, RectangleF itemRect, string text ) {
      //Psl.Tracker.Tracker.Track( "RichEditPainter : DrawText" );
      //if ( host.DropDownStyle != ComboBoxStyle.DropDownList || host.DrawMode == DrawMode.Normal ) return;

      // déterminer l'orientation de l'affichage
      bool rightToLeft = host.IsRightToLeft;

      // déterminer la taille de l'affichage du texte
      // la taille de base est celle de la font de la combo, limitée en fonction de la hauteur de la boîte
      float textHeight = host.Font.GetHeight( graphics );
      if ( textHeight > host.ItemHeight - 4 ) textHeight = host.ItemHeight - 4;

      // déterminer la font d'affichage
      // todo (RichComboBox) : optimiser l'obtention de la font
      Font textFont = new Font( host.Font.FontFamily, textHeight, host.Font.Style, GraphicsUnit.Pixel );

      // déterminer le format du texte
      StringFormat textFormat = new StringFormat( StringFormatFlags.NoClip | StringFormatFlags.NoWrap );
      textFormat.Alignment = rightToLeft ? StringAlignment.Far : StringAlignment.Near;

      // déterminer le calage du texte 
      float textLeft = (float) editLeftMargin - 2;
      float textWidth = (float) itemRect.Width - editLeftMargin - editRightMargin;
      float textTop = (float) itemRect.Height <= textHeight ? 0 : (itemRect.Height - textHeight) / 2 ;
      RectangleF textRect = new RectangleF( itemRect.Left + textLeft, itemRect.Y /* + textTop */, textWidth, textHeight );

      // peindre le texte
      graphics.DrawString( text, textFont, new SolidBrush( host.ForeColor ), textRect, textFormat );
    }

    /// <summary>
    /// Peindre l'image
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="image">image à peindre</param>
    public void DrawImage( Graphics graphics, RectangleF itemRect, Image image ) {
      if ( !host.ImageShow || host.DrawMode == DrawMode.Normal ) return;
      //Psl.Tracker.Tracker.Track( "RichEditPainter : DrawImage" );

      // déterminer s'il y a une image à peindre
      if ( editDrawImage == null && image == null) return ;

      // préparer l'image à peindre pour la conserver en cache
      // éviter de comprimer les images si elles sont proches de la taille souhaitée
      // prendre éventuellement une copie de l'image originale pour éviter tout effet de bord
      if (editDrawImage == null) {
        editDrawImage = image;
        bool stretch = Math.Abs( host.ItemHeight - editDrawImage.Height ) > 3;
        editDrawImage = stretch ? new Bitmap( editDrawImage, host.ItemHeight, host.ItemHeight ) : (Image) editDrawImage.Clone();
        //if ( host.IsRightToLeft ) editDrawImage.RotateFlip( RotateFlipType.RotateNoneFlipX );
      }

      // déterminer la position en x de l'affichage selon bidi
      float imageX = host.IsRightToLeft ? itemRect.Width - editDrawImage.Width : 0;

      // afficher
      graphics.DrawImage( editDrawImage, itemRect.Left + imageX, itemRect.Top  );
    }

    /// <summary>
    /// Peindre l'arrière-plan
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="asFocused">si true, connsidérer que le contrôle hôte a le focus</param>
    public void DrawBackGround( Graphics graphics, RectangleF itemRect, bool asFocused ) {
      //Psl.Tracker.Tracker.Track( "RichEditPainter : DrawBackGround" );

      // peindre l'arrière-plan
      using ( Brush backBrush = new SolidBrush( host.BackColor ) ) {
        graphics.FillRectangle( backBrush, itemRect );
      }

      // peindre l'indication de focus sur la zone de texte seule
      if ( asFocused ) {
        RectangleF backRect = new RectangleF( itemRect.Left + editLeftMargin, itemRect.Top, itemRect.Width - editLeftMargin - editRightMargin, itemRect.Height );
        graphics.FillRectangle( SystemBrushes.Highlight, backRect );
      }
    }

    /// <summary>
    /// Peindre l'image et le texte
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="image">image à peindre</param>
    /// <param name="text">texte à peindre</param>
    /// <param name="asFocused">si true, peindre la boîte comme ayant le focus</param>
    public void DrawContent( Graphics graphics, RectangleF itemRect, Image image, string text, bool asFocused ) {
      //Psl.Tracker.Tracker.Track( "RichEditPainter : DrawContent" );
      DrawBackGround( graphics, itemRect, asFocused );
      DrawImage( graphics, itemRect, image );
      DrawText( graphics, itemRect, text );
    }
  }
}
