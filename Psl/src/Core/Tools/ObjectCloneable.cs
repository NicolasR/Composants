/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
namespace Psl.Tools {

  /// <summary>
  /// Extension directe de Object pour faciliter les clonages.
  /// </summary>
  public class ObjectCloneable : System.ICloneable {

    /// <summary>
    /// M�thode publique et virtuelle relayant la m�thode MemberwiseClone.
    /// </summary>
    /// <returns>une copie en surface de l'instance courante</returns>
    public virtual object Clone() {
      return MemberwiseClone();
    }

    /// <summary>
    /// Clonage en profondeur d'un tableau de r�f�rences.
    /// </summary>
    /// <remarks>
    /// Cette m�thode de classe permet d'obtenir une m�thode g�n�rale pour le
    /// clonage en profondeur des tableaux.
    /// Dans le tableau, seules les instances qui impl�mentent l'interface ICloneable
    /// sont dupliqu�es, dans les autres cas la r�f�rence est simplement recopi�e. 
    /// Le clonage d'une r�f�rence null produit une r�f�rence null.
    /// </remarks>
    /// <param name="array">tableau � dupliquer en profondeur</param>
    /// <returns>clone en profondeur du tableau � dupliquer</returns>
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