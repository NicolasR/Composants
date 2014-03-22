/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
namespace Psl.Tools {

  /// <summary>
  /// Classe de service fournissant des numéros uniques.
  /// </summary>
  public class IdProvider {

    /// <summary>
    /// Mémorisation du décompte
    /// </summary>
    private static int count = 0 ;

    /// <summary>
    /// Retourne un nouvel ID à chaque consultation
    /// </summary>
    public  static int NewId { 
      get { 
        count ++ ; 
        return count ; 
      } 
    }

    /// <summary>
    /// Réinitialise le décompte
    /// </summary>
    public static void Reset() {
      count = 0 ;
    }
  }

} // namespace