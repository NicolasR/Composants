/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 19 01 2010 : version initiale aihm 2009-2010
 */                                                                            // <wao never.end>
using System;
using System.Collections.Generic;
using System.Text;

namespace Psl.Controls {

  /// <summary>
  /// Type de d�l�gu� pour la notification des changements
  /// </summary>
  public delegate void NotifyHandler();

  /// <summary>
  /// Structure de service r�alisant un verrou de notifications
  /// </summary>
  /// <remarks>
  /// Mode l'emploi : <br/>
  /// 1) initialiser le verrou en transmettant le d�l�gu� des notifications des changement <br/>
  /// 2) verrouiller via des parenth�ses LockOn/LockOff ou Locked=true/Locker=false <br/>
  /// 3) utiliser un bloc prot�g� pour garantir le d�verrouillage dans un bloc finally <br/>
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

    // d�l�gu� de notification
    private NotifyHandler notifyHandler;

    // true si un changement est intervenu pendant le verrouillage
    private bool changed;

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="notifyHandler">d�l�gu� de notification des changements</param>
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
    // Fonctionnalit�s
    //

    /// <summary>
    /// Commence un niveau de verrouillage
    /// </summary>
    public void LockOn() {
      if ( locker == 0 ) changed = false;
      locker++;
    }

    /// <summary>
    /// M�thode � appeler pour indiquer un changement
    /// </summary>
    /// <remarks>
    /// Si la m�thode est appel�e lorsque le verrou est libre, le d�l�gu� de notification 
    /// est imm�diatement appel�. Sinon, le d�clenchement du d�l�gu� de notification est
    /// retard� jusqu'� ce que le verrou soit lib�r�. 
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
    /// D�clenche le d�l�gu� de notification si le verrou revient � 0
    /// </remarks>
    /// <exception cref="InvalidOperationException">si le verrou est d�j� lib�r�</exception>
    public void LockOff() {
      if ( locker == 0 ) throw new InvalidOperationException( "Le verrou est d�j� 0" );
      locker--;
      if ( locker == 0 && changed ) OnChanged();
    }

    /// <summary>
    /// Obtient ou d�termine l'�tat de verrouillage du verrou
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
