/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 19 01 2010 : version initiale aihm 2009-2010
 */                                                                            // <wao never.end>
using System;
using System.Collections.Generic;
using System.Text;

namespace Psl.Controls {

  /// <summary>
  /// Type de délégué pour la notification des changements
  /// </summary>
  public delegate void NotifyHandler();

  /// <summary>
  /// Structure de service réalisant un verrou de notifications
  /// </summary>
  /// <remarks>
  /// Mode l'emploi : <br/>
  /// 1) initialiser le verrou en transmettant le délégué des notifications des changement <br/>
  /// 2) verrouiller via des parenthèses LockOn/LockOff ou Locked=true/Locker=false <br/>
  /// 3) utiliser un bloc protégé pour garantir le déverrouillage dans un bloc finally <br/>
  /// 4) Appeler Changed pour indiquer un changement, qu'il y ait ou non un verrouillage en cours <br/>
  /// <br/>
  /// Exemple :
  /// <code>
  /// Locker locker = new Locker( delegate(){ OnSomethingChanged(...) ; } ) ;
  /// locker.Locker = true ;
  /// try {
  ///   :
  ///   locker.Changed() ;
  ///   :
  ///  }
  ///  finally { locker.Locked = false ; }
  /// </code>
  /// </remarks>
  public struct Locker {

    //
    // Champs
    //

    // niveau de verrouillage
    private int locker;

    // délégué de notification
    private NotifyHandler notifyHandler;

    // true si un changement est intervenu pendant le verrouillage
    private bool changed;

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="notifyHandler">délégué de notification des changements</param>
    public Locker( NotifyHandler notifyHandler )
      : this() {
      this.notifyHandler = notifyHandler;
    }

    //
    // Service
    //

    private void OnChanged() {
      if ( notifyHandler != null ) notifyHandler();
    }

    //
    // Fonctionnalités
    //

    /// <summary>
    /// Commence un niveau de verrouillage
    /// </summary>
    public void LockOn() {
      if ( locker == 0 ) changed = false;
      locker++;
    }

    /// <summary>
    /// Méthode à appeler pour indiquer un changement
    /// </summary>
    /// <remarks>
    /// Si la méthode est appelée lorsque le verrou est libre, le délégué de notification 
    /// est immédiatement appelé. Sinon, le déclenchement du délégué de notification est
    /// retardé jusqu'à ce que le verrou soit libéré. 
    /// </remarks>
    public void Changed() {
      if ( locker == 0 )
        OnChanged();
      else
        changed = true;
    }

    /// <summary>
    /// Termine un niveau de verrouillage
    /// </summary>
    /// <remarks>
    /// Déclenche le délégué de notification si le verrou revient à 0
    /// </remarks>
    /// <exception cref="InvalidOperationException">si le verrou est déjà libéré</exception>
    public void LockOff() {
      if ( locker == 0 ) throw new InvalidOperationException( "Le verrou est déjà 0" );
      locker--;
      if ( locker == 0 && changed ) OnChanged();
    }

    /// <summary>
    /// Obtient ou détermine l'état de verrouillage du verrou
    /// </summary>
    public bool Locked { 
      get { 
        return locker > 0; 
      }
      set {
        if ( value )
          LockOn();
        else
          LockOff();
      }
    }
  }
}
