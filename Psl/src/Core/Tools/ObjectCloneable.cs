/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
namespace Psl.Tools {

  /// <summary>
  /// Extension directe de Object pour faciliter les clonages.
  /// </summary>
  public class ObjectCloneable : System.ICloneable {

    /// <summary>
    /// Méthode publique et virtuelle relayant la méthode MemberwiseClone.
    /// </summary>
    /// <returns>une copie en surface de l'instance courante</returns>
    public virtual object Clone() {
      return MemberwiseClone();
    }

    /// <summary>
    /// Clonage en profondeur d'un tableau de références.
    /// </summary>
    /// <remarks>
    /// Cette méthode de classe permet d'obtenir une méthode générale pour le
    /// clonage en profondeur des tableaux.
    /// Dans le tableau, seules les instances qui implémentent l'interface ICloneable
    /// sont dupliquées, dans les autres cas la référence est simplement recopiée. 
    /// Le clonage d'une référence null produit une référence null.
    /// </remarks>
    /// <param name="array">tableau à dupliquer en profondeur</param>
    /// <returns>clone en profondeur du tableau à dupliquer</returns>
    public static object[] ArrayDeepClone( object[] array ) {
      if (array == null) return null;
      object[] result = (object[]) array.Clone();
      for (int ix = 0 ; ix < array.Length ; ix++)
        try { result[ ix ] = ((System.ICloneable) array[ ix ]).Clone(); }
        catch { }
      return result;
    }
  }

} // namespace