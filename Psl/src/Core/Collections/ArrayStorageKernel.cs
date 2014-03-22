using System;
using System.Collections;

                                                                               // <wao ArrayStorageKernel.begin>
namespace Psl.Collections {

  /** 
   * Noyau d'impl�mentation pour la gestion de tableaux dynamiques. 
   * <br/>
   * Sont regroup�es dans ce noyau les m�thodes d'impl�mentation "profondes" 
   * r�alisant une gestion dynamiques de tableaux. 
   * <br/>
   * Ces m�thodes sont programm�es sans aucun contr�le de la validit� des arguments 
   * qui leur sont transmis, et sont suppos�es n'�tre appliqu�es qu'� des arguments 
   * pr�alablement contr�l�s. 
   * 
   * <pre>
   * 2000 12 06 : version initiale pour Java
   * 2002 09 01 : portage pour J# et adaptation
   * 2005 04 11 : portage pour C# et extraction de ArrayStorageKernel
   * </pre>
   */
  public abstract class ArrayStorageKernel : Psl.Tools.ObjectCloneable {
	
  /** Incr�ment de capacit� lorsque le tableau doit cro�tre */
	  protected int capacityIncrement = 0 ;

  /** Nombre d'�l�ments actuellement pr�sents dans la liste */
	  protected int count = 0 ;

  /** Timbrage du suivi des modifications */
	  protected uint stamp = 0 ;

  /** Tableau courant pour l'impl�mentation de la liste */
	  protected object[] items = new object[ 0 ] ;

  /**
   * Signale une modification de la liste.
   *
   * M�thode service interne � l'impl�mentation permettant de tenir � jour
   * simultan�ment le nombre d'�l�ments de la liste et le timbrage du suivi
   * des modifications. 
   */
    protected void DoChanged( int newCount ) {
      count = newCount ;
      stamp ++ ;
    }
   
  /**
   * D�termination de la nouvelle capacit� du tableau.
   *
   * Cette m�thode de service calcule la prochaine capacit�, au moins �gale � minCapacity,
   * que doit avoir le tableau {@link #items} compte tenu de la capacit� actuelle du
   * tableau et de l'incr�ment de capacit�. Si la capacit� actuelle du tableau est 
   * suffisante, cette m�thode retourne cette capacit� actuelle. 
   * 
   * @param minCapacity capacit� minimale du tableau � garantir
   * @return la prochaine capacit� du tableau (sup�rieure ou �gale � minCapacity)
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
   * Ajuste le tableau selon capacity et recopier les headerCount premiers �l�ments.
   *
   * Si la capacit� actuelle du tableau est identique � la capacit� souhait�e capacity,
   * cette m�thode ne fait rien.
   *
   * Sinon, un nouveau tableau de capacity entr�es est allou�, et les r�f�rences des 
   * headerCount premiers �l�ments du tableau original sont recopi�s dans les headerCount
   * premi�res entr�es du nouveau tableau. 
   *
   * A l'issue de cette op�ration, le champ {@link #items} rep�re le nouveau tableau.
   * 
   * Cette m�thode n'effectue aucun contr�le sur la validit� des arguments. 
   *
   * @param capacity capacit� souhait�e
   * @param headerCount nombre d'entr�es � recopier en t�te du nouveau tableau
   */
  	protected void DoAdjustAndCopyHeader( int capacity, int headerCount ) {
  		if (capacity == items.Length) return ; 
  		object[] old = items ;
  		items = new object[ capacity ]  ;
  		if (headerCount == 0) return ;
  		Array.Copy( old, 0, items, 0, headerCount ) ;
  	}

  /**
   * M�nage un gap de gapCount places � partir de la position index.
   * 
   * Cette m�thode m�nage un espace libre (gap) de gapCount entr�es � 
   * partir de la position index apr�s avoir ajust�, si n�cessaire, la
   * capacit� du tableau {@link #items}.
   * 
   * Cette m�thode n'effectue aucun contr�le sur la validit� des arguments. 
   *
   * @param index position � laquelle doit commencer le gap
   * @param gapCount nombre d'entr�es libres � m�nager
   */
  	protected void DoGapAt( int index, int gapCount ) {
  	  if (gapCount == 0) return ;
  		object[] old = items ;
  		DoAdjustAndCopyHeader( DoGetNewCapacity( count + gapCount ), index ) ;
  		Array.Copy( old, index, items, index + gapCount, count - index ) ;
  		DoChanged( count + gapCount ) ;
  	}

  /**
   * Supprime un gap de gapCount �l�ments commen�ant � la position index.
   *
   * Cette m�thode supprime un espace de gapCount �l�ments � partir de la
   * position index sans r�duire la capacit� du tableau.
   * 
   * Cette m�thode n'effectue aucun contr�le sur la validit� des arguments.
   * 
   * @param index position o� commence le gap � supprimer
   * @param gapCount nombres d'entr�es � supprimer
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
