/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
namespace Psl.Tools {

  /// <summary>
  /// Classe de service fournissant des num�ros uniques.
  /// </summary>
  public class IdProvider {

    /// <summary>
    /// M�morisation du d�compte
    /// </summary>
    private static int count = 0 ;

    /// <summary>
    /// Retourne un nouvel ID � chaque consultation
    /// </summary>
    public  static int NewId { 
      get { 
        count ++ ; 
        return count ; 
      } 
    }

    /// <summary>
    /// R�initialise le d�compte
    /// </summary>
    public static void Reset() {
      count = 0 ;
    }
  }

} // namespace