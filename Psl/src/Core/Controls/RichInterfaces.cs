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
using System.Collections;
using System.Text;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Interface d'un propriétaire de RichItem                         //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface implémentée par une collection propriétaire d'éléments <see cref="RichItem"/>
  /// </summary>
  /// <remarks>
  /// Cette interface correspond à ce qu'un élément <see cref="RichItem"/> voit de la collection.
  /// </remarks>
  public interface IRichCollectionOwner : IList {

    /// <summary>
    /// Obtient le contrôle d'affichage hôte de la collection.
    /// </summary>
    IRichControlHost Host { get; }

    /// <summary>
    /// Indique si la collection est hébergée (c'est-à-dire si <see cref="Host"/> est non null.
    /// </summary>
    bool IsHosted { get; }

    /// <summary>
    /// Permet à un <see cref="RichItem"/> de notifier à son propriétaire qu'il a changé d'état.
    /// </summary>
    /// <param name="item">élément modifié</param>
    void OnItemChanged( RichItem item );
  }


  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //      Interface implémentée par un contrôle hôte hébergeant une collection de RichItem       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface implémentée par un contrôle qui héberge une collection de <see cref="RichItem"/>
  /// </summary>
  /// <remarks>
  /// Cette interface permet de retransmettre les valeurs héritées pour les <see cref="RichItem"/>.
  /// Cette retransmission est en particulier nécessaire en mode conception pour l'affichage des
  /// propriétés.
  /// </remarks>
  public interface IRichControlHost {

    /// <summary>
    /// Commence un niveau de verrouillage des réaffichages
    /// </summary>
    void BeginUpdate() ;

    /// <summary>
    /// Termine un niveau de verrouillage des réaffichages
    /// </summary>
    void EndUpdate() ;

    /// <summary>
    /// Obtient la collection réelle des éléments du contrôle hôte.
    /// </summary>
    IList Items { get; }

    /// <summary>
    /// Obtient la liste d'images du contrôle hôte.
    /// </summary>
    ImageList ImageList { get; }

    /// <summary>
    /// Obtient la font par défaut pour les <see cref="RichItem"/>
    /// </summary>
    Font ItemFont { get; }

    /// <summary>
    /// Obtient la couleur d'avant-plan par défaut pour les <see cref="RichItem"/>
    /// </summary>
    Color ItemForeColor { get; }

    /// <summary>
    /// Obtient la couleur d'arrière-plan par défaut pour les <see cref="RichItem"/>
    /// </summary>
    Color ItemBackColor { get; }

    /// <summary>
    /// Notifie le changement d'état d'un <see cref="RichItem"/> impliquant son réaffichage
    /// </summary>
    /// <param name="item">référence sur l'item modifié</param>
    void OnItemChanged( RichItem item );
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                Interface implémentée par un hôte de peinture d'items riches                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface implémentée par un hôte utilisant les services d'un <see cref="IRichListPainter"/>.
  /// </summary>
  public interface IRichListPainterHost {

    /// <summary>
    /// Indique si la peinture doit se faire de droite à gauche.
    /// </summary>
    bool IsRightToLeft { get; }

    /// <summary>
    /// Obtient la largeur en pixels d'un niveau d'indentation.
    /// </summary>
    int IndentWidth { get; }

    /// <summary>
    /// Indique si les images doivent être affichées.
    /// </summary>
    bool ImageShow { get; }

    /// <summary>
    /// Obtient la font par défaut pour l'affichage des items.
    /// </summary>
    Font Font { get; }

    /// <summary>
    /// Obtient le mode de peinture.
    /// </summary>
    DrawMode DrawMode { get; }

    // pour la peinture de la boîte d'édition

    /// <summary>
    /// Obtient la hauteur (exprimée en pixels) d'un item quand il a une hauteur fixe.
    /// </summary>
    int ItemHeight { get; }

    /// <summary>
    /// Couleur d'avant-plan pour la boîte d'édition
    /// </summary>
    Color ForeColor { get; }

    /// <summary>
    /// Couleur d'arrière-plan pour la boîte d'édition
    /// </summary>
    Color BackColor { get; }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                   Interface implémentée par un peintre d'items riches                       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface implémentée par un peintre d'items enrichis <see cref="RichItem"/>.
  /// </summary>
  public interface IRichListPainter {

    /// <summary>
    /// Détermine les tailles horizontales et verticales de la peinture d'un item.
    /// </summary>
    /// <param name="e">descripteur issue d'un événement MeasureItem de l'hôte</param>
    /// <param name="item">référence non null sur l'item à peindre</param>
    void MeasureRichItem( MeasureItemEventArgs e, RichItem item );

    /// <summary>
    /// Peint un item enrichi <see cref="RichItem"/>.
    /// </summary>
    /// <param name="e">descripteur issu d'un événement DrawItem de l'hôte</param>
    /// <param name="item">référence non null sur l'item à peindre</param>
    void DrawRichItem( DrawItemEventArgs e, RichItem item );
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //               Interface implémentée par un peintre de boîte d'édition riche                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface implémentée par un peintre de boîte d'édition riche
  /// </summary>
  public interface IRichEditPainter {

    /// <summary>
    /// Invalider les informations gérées en cache
    /// </summary>
    /// <param name="hWnd">handle de la fenêtre d'édition</param>
    void InvalidateEditImage( IntPtr hWnd );

    /// <summary>
    /// Peindre l'image
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="image">image à peindre</param>
    void DrawImage( Graphics graphics, RectangleF itemRect, Image image );

    /// <summary>
    /// Peindre le texte
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="text">texte à peindre</param>
    void DrawText( Graphics graphics, RectangleF itemRect, string text );

    /// <summary>
    /// Peindre l'image et le texte
    /// </summary>
    /// <param name="graphics">contexte graphique</param>
    /// <param name="itemRect">rectangle client de la boîte</param>
    /// <param name="image">image à peindre</param>
    /// <param name="text">texte à peindre</param>
    /// <param name="asFocused">si true, peindre la boîte comme ayant le focus</param>
    void DrawContent( Graphics graphics, RectangleF itemRect, Image image, string text, bool asFocused );
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                   Enumération pour la localisation d'un impact de clic souris               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Enumération pour la localisation de l'impact d'un clic souris
  /// </summary>
  public enum RichControlHitTest {

    /// <summary>
    /// Le clic n'a eu lieu ni dans la zone de texte ni dans la zoné d'image
    /// </summary>
    None,

    /// <summary>
    /// Le clic s'est produit dans la zone du texte
    /// </summary>
    Text,

    /// <summary>
    /// Le clic s'est produit dans la zone de l'image
    /// </summary>
    Image
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                  Descripteur et type de délégué pour les événements InputKey                //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Descripteur d'événement pour l'événement <see cref="RichComboBox.InputKey"/>
  /// </summary>
  public class InputKeyEventArgs : EventArgs {

    private Keys keyData;
    private bool isInputKey;

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="keyData">touche concernée avec ses flags de modification</param>
    /// <param name="isInputKey">détermine si la touche correspond à un caractère d'entrée</param>
    public InputKeyEventArgs( Keys keyData, bool isInputKey ) {
      this.keyData = keyData;
      this.isInputKey = isInputKey;
    }

    /// <summary>
    /// Touche concerné avec ses flags de modification
    /// </summary>
    public Keys KeyData {
      get { return keyData; }
    }

    /// <summary>
    /// Détermine si la touche correspond à un caractère d'entrée de la boîte d'édition
    /// </summary>
    /// <remarks>
    /// Basculer cette propriété à true pour que la touche soit considérée comme un
    /// caractère d'entrée pour la boîte d'édition.
    /// </remarks>
    public bool IsInputKey {
      get { return isInputKey; }
      set { isInputKey = value; }
    }
  }

  /// <summary>
  /// Type de délégué pour l'événement <see cref="RichComboBox.InputKey"/>
  /// </summary>
  /// <param name="sender">source de l'événement</param>
  /// <param name="e">descripteur de l'événement</param>
  public delegate void InputKeyEventHandler( object sender, InputKeyEventArgs e ) ;

}
