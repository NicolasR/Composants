/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 * 23 05 2010 : adjonction de la classe ImageDrawer
 */                                                                            // <wao never.end>

using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace Psl.Drawing {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                         ImageDrawer                                         //
  //                     Peinture d'une image avec prise en charge des animations                //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Assistant pour la peinture d'images prenant en charge les images animées
  /// </summary>
  /// <remarks>
  /// Cette classe ne prétend pas être générale. Elle vise seulement à assister le dessin d'images
  /// typiquement 16x16 pour prendre en charge le cas des images animées. 
  /// <br/>
  /// Dans une méthode <see cref="Control.OnPaint"/> ou dans un événement de peinture, invoquer la
  /// méthode <see cref="DrawImage"/> au lieu de peindre l'image. Les informations concernant 
  /// l'image et la peinture se mettront ainsi automatiquement à jour. 
  /// <br/>
  /// Pour éviter de repeindre tout un contrôle à chaque événement de peinture, et comportant
  /// une image, invoquer la méthode <see cref="IsPaintForAnimation"/> : si le résultat est true, 
  /// ne pas peindre le contrôle et appeler <see cref="DrawImage"/>; sinon, laisser le contrôle se 
  /// peindre et appeler normalement la méthode <see cref="DrawImage"/> pour peindre l'image.
  /// </remarks>
  public class ImageDrawer : IDisposable {

    /// <summary>
    /// Contrôle devant effectuer la peinture de l'image
    /// </summary>
    private Control painter = null;

    /// <summary>
    /// Image originale à peindre
    /// </summary>
    private Image image = null;

    /// <summary>
    /// Image animée à peindre ou null
    /// </summary>
    private bool isAnimated = false;

    /// <summary>
    /// Rectangle dans lequel peindre l'image
    /// </summary>
    private Rectangle rectangle = Rectangle.Empty;

    /// <summary>
    /// Restitution des ressources et désabonnements
    /// </summary>
    public void Dispose() {
      DoSetValues( null, null, Rectangle.Empty );
    }

    /// <summary>
    /// Handler de notification de la destruction du contrôle de peinture <see cref="Painter"/>
    /// </summary>
    /// <param name="sender">émetteur de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnPainterDisposed( object sender, EventArgs e ) {
      DoSetValues( null, null, Rectangle.Empty );
    }

    /// <summary>
    /// Handler de notification qu'une frame d'animation doit être peinte
    /// </summary>
    /// <remarks>
    /// Cette méthode est déclenchée par <see cref="ImageAnimator"/>.
    /// </remarks>
    /// <param name="sender">émetteur de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnFrameChanged( object sender, EventArgs e ) {
      if (painter != null) painter.Invalidate( rectangle );
    }

    /// <summary>
    /// Mise à jour centralisée des données.
    /// </summary>
    /// <param name="newPainter">référence sur le contrôle de peinture</param>
    /// <param name="newImage">référence sur l'image à peindre</param>
    /// <param name="newRect">rectancle où peindre l'image</param>
    /// <returns>true si un changement est intervenu</returns>
    protected virtual bool DoSetValues( Control newPainter, Image newImage, Rectangle newRect ) {
      if ( painter == newPainter && image == newImage && rectangle == newRect ) return false;

      bool unanimate = image != newImage && isAnimated;
      if ( unanimate ) {
        ImageAnimator.StopAnimate( image, OnFrameChanged );
        isAnimated = false;
      }

      rectangle = newRect;

      if ( painter != newPainter ) {
        if ( painter != null ) painter.Disposed -= OnPainterDisposed;
        painter = newPainter;
        if ( painter != null ) painter.Disposed += OnPainterDisposed;
      }

      bool cananimate = ImageAnimator.CanAnimate( newImage );
      bool reanimate = newImage != null && newImage != image && cananimate;
      image = newImage;
      if ( reanimate ) {
        isAnimated = true;
        ImageAnimator.Animate( image, OnFrameChanged );
      }

      return true;
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Détermine si un événement de peinture (méthode <see cref="Control.OnPaint"/>) ne concerne que la peinture d'une frame d'animation.
    /// </summary>
    /// <remarks>
    /// La méthode retourne true si le ClipRect indiqué dans le descripteur d'événement de l'événement de peinture
    /// coïncide exactement avec le rectangle de l'image à peindre. Cela suppose donc que la méthode ne soit
    /// appellée que dans le contexte de l'hôte de peinture.
    /// </remarks>
    /// <param name="e">descripteur de l'événement de peinture</param>
    /// <returns>true si l'événement de peinture ne concerne que la peinture d'une frame d'animation.</returns>
    public bool IsPaintForAnimation( PaintEventArgs e ) {
      return isAnimated && rectangle != Rectangle.Empty && e.ClipRectangle == rectangle;
    }

    /// <summary>
    /// Peinture de l'image avec prise en charge des frames d'animation.
    /// </summary>
    /// <param name="newPainter">référence sur le contrôle de peinture</param>
    /// <param name="g">contexte graphique de peinture</param>
    /// <param name="newImage">image à peindre</param>
    /// <param name="newRect">rectangle où peindre l'image</param>
    /// <param name="data">données complémentaires éventuelles</param>
    public void DrawImage( Control newPainter, Graphics g, Image newImage, Rectangle newRect, params object[] data ) {

      // mettre à jour toutes les valeurs
      DoSetValues( newPainter, newImage, newRect );
      if ( image == null ) return;

      // mettre à jour les frames d'animation
      if ( isAnimated ) ImageAnimator.UpdateFrames( image );

      // peindre l'image
      g.DrawImage( image, rectangle );
    }

    //
    // Propriétés
    //

    /// <summary>
    /// Détermine si l'image courante est animée ou non.
    /// </summary>
    public bool IsAnimated {
      get { return isAnimated; }
    }

    /// <summary>
    /// Contrôle responsable de l'image à peindre
    /// </summary>
    public Control Painter {
      get { return painter; }
      set { DoSetValues( value, image, rectangle ); }
    }

    /// <summary>
    /// Image à peindre
    /// </summary>
    public Image Image {
      get { return image; }
      set { DoSetValues( painter, value, rectangle ); }
    }

    /// <summary>
    /// Rectangle où peindre l'image dans les coordonnées du contrôle de peinture
    /// </summary>
    public Rectangle Rectangle {
      get { return rectangle; }
      set { DoSetValues( painter, image, value ); }
    }
  }
}
