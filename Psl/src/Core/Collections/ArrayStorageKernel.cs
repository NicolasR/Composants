using System;
using System.Collections;

                                                                               // <wao ArrayStorageKernel.begin>
namespace Psl.Collections {

  /** 
   * Noyau d'implémentation pour la gestion de tableaux dynamiques. 
   * <br/>
   * Sont regroupées dans ce noyau les méthodes d'implémentation "profondes" 
   * réalisant une gestion dynamiques de tableaux. 
   * <br/>
   * Ces méthodes sont programmées sans aucun contrôle de la validité des arguments 
   * qui leur sont transmis, et sont supposées n'être appliquées qu'à des arguments 
   * préalablement contrôlés. 
   * 
   * <pre>
   * 2000 12 06 : version initiale pour Java
   * 2002 09 01 : portage pour J# et adaptation
   * 2005 04 11 : portage pour C# et extraction de ArrayStorageKernel
   * </pre>
   */
  public abstract class ArrayStorageKernel : Psl.Tools.ObjectCloneable {
	
  /** Incrément de capacité lorsque le tableau doit croître */
	  protected int capacityIncrement = 0 ;

  /** Nombre d'éléments actuellement présents dans la liste */
	  protected int count = 0 ;

  /** Timbrage du suivi des modifications */
	  protected uint stamp = 0 ;

  /** Tableau courant pour l'implémentation de la liste */
	  protected object[] items = new object[ 0 ] ;

  /**
   * Signale une modification de la liste.
   *
   * Méthode service interne à l'implémentation permettant de tenir à jour
   * simultanément le nombre d'éléments de la liste et le timbrage du suivi
   * des modifications. 
   */
    protected void DoChanged( int newCount ) {
      count = newCount ;
      stamp ++ ;
    }
   
  /**
   * Détermination de la nouvelle capacité du tableau.
   *
   * Cette méthode de service calcule la prochaine capacité, au moins égale à minCapacity,
   * que doit avoir le tableau {@link #items} compte tenu de la capacité actuelle du
   * tableau et de l'incrément de capacité. Si la capacité actuelle du tableau est 
   * suffisante, cette méthode retourne cette capacité actuelle. 
   * 
   * @param minCapacity capacité minimale du tableau à garantir
   * @return la prochaine capacité du tableau (supérieure ou égale à minCapacity)
   */
  	protected int DoGetNewCapacity( int minCapacity ) {
  		int dftCapacity = count + (capacityIncrement == 0 ? count : capacityIncrement ) ;
  		return ( 
  		  minCapacity <= items.Length ? 
  		    items.Length 
  		  : ( minCapacity <= dftCapacity ?
  		        dftCapacity 
  		      : minCapacity 
  		    )
  		) ;
  	}
	
  /**
   * Ajuste le tableau selon capacity et recopier les headerCount premiers éléments.
   *
   * Si la capacité actuelle du tableau est identique à la capacité souhaitée capacity,
   * cette méthode ne fait rien.
   *
   * Sinon, un nouveau tableau de capacity entrées est alloué, et les références des 
   * headerCount premiers éléments du tableau original sont recopiés dans les headerCount
   * premières entrées du nouveau tableau. 
   *
   * A l'issue de cette opération, le champ {@link #items} repère le nouveau tableau.
   * 
   * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
   *
   * @param capacity capacité souhaitée
   * @param headerCount nombre d'entrées à recopier en tête du nouveau tableau
   */
  	protected void DoAdjustAndCopyHeader( int capacity, int headerCount ) {
  		if (capacity == items.Length) return ; 
  		object[] old = items ;
  		items = new object[ capacity ]  ;
  		if (headerCount == 0) return ;
  		Array.Copy( old, 0, items, 0, headerCount ) ;
  	}

  /**
   * Ménage un gap de gapCount places à partir de la position index.
   * 
   * Cette méthode ménage un espace libre (gap) de gapCount entrées à 
   * partir de la position index après avoir ajusté, si nécessaire, la
   * capacité du tableau {@link #items}.
   * 
   * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
   *
   * @param index position à laquelle doit commencer le gap
   * @param gapCount nombre d'entrées libres à ménager
   */
  	protected void DoGapAt( int index, int gapCount ) {
  	  if (gapCount == 0) return ;
  		object[] old = items ;
  		DoAdjustAndCopyHeader( DoGetNewCapacity( count + gapCount ), index ) ;
  		Array.Copy( old, index, items, index + gapCount, count - index ) ;
  		DoChanged( count + gapCount ) ;
  	}

  /**
   * Supprime un gap de gapCount éléments commençant à la position index.
   *
   * Cette méthode supprime un espace de gapCount éléments à partir de la
   * position index sans réduire la capacité du tableau.
   * 
   * Cette méthode n'effectue aucun contrôle sur la validité des arguments.
   * 
   * @param index position où commence le gap à supprimer
   * @param gapCount nombres d'entrées à supprimer
   */
  	protected void DoUnGapAt( int index, int gapCount ) {
  	  if (gapCount == 0) return ;
  		int moveCount = count - gapCount - index ;
  		if (moveCount > 0) Array.Copy( items, index + gapCount, items, index, moveCount ) ;
  		Array.Clear( items, count - gapCount, gapCount ) ;
  		DoChanged( count - gapCount ) ;
  	}
  }
} // namespace
                                                                               // <wao ArrayStorageKernel.end>
