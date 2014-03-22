/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 09 05 2010 : version initiale
 */                                                                           // <wao never.end>
using System;
using System.Drawing;
using System.Windows.Forms;
using Psl.DragDrop;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                             Descripteur d'événements DragStartEventArgs                     //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Descripteur d'événement pour l'événement <see cref="TabDocker.DragStart"/>
  /// </summary>
  public class DragStartEventArgs : EventArgs {

    /// <summary>
    /// Constructeur du descripteur d'événement
    /// </summary>
    /// <param name="allowedEffects">valeur initiale des effets possibles</param>
    /// <param name="data">objet transitionnel de suivi de l'opération de glissement</param>
    /// <param name="button">bouton lié au démarrage de l'opération de glissement</param>
    /// <param name="location">coordonnées client de l'impact initial du bouton enfoncé</param>
    /// <param name="cancel">valeur initiale de la propriété <see cref="Cancel"/></param>
    public DragStartEventArgs( DragDropEffects allowedEffects, IDataObject data, MouseButtons button, Point location, bool cancel ) {
      AllowedEffects = allowedEffects;
      Data = data;
      Button = button;
      Location = location;
      Cancel = cancel;
    }

    /// <summary>
    /// Obtient ou détermine les effets possible de l'opération de glissement
    /// </summary>
    public DragDropEffects AllowedEffects { get; set; }

    /// <summary>
    /// Obtient l'objet transitionnel de suivi de l'opération de glissement
    /// </summary>
    public IDataObject Data { get; private set; }

    /// <summary>
    /// Obtient le bouton lié au déclenchement de l'opération de glissement
    /// </summary>
    public MouseButtons Button { get; private set; }

    /// <summary>
    /// Obtient les coordonnées client de l'impact initial du bouton enfoncé
    /// </summary>
    public Point Location { get; private set; }

    /// <summary>
    /// Obtient ou détermine si l'opération de glissement doit être abandonnée
    /// </summary>
    public bool Cancel { get; set; }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                              Type de délégué DragStartEventHandler                          //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Type de délégué pour les événements <see cref="IDragDropExtender.DragStart"/>
  /// </summary>
  /// <param name="sender">source de l'événement</param>
  /// <param name="e">descripteur de l'événement</param>
  public delegate void DragStartEventHandler( object sender, DragStartEventArgs e );

}
