using System;
using System.Collections;

                                                                               // <wao ArrayStorageTools.begin>
namespace Psl.Collections {

  /**
   * Boîte à outils d'implémentation pour la gestion de listes en tableaux dynamiques. <br/>
   *
   * Dans cette boîte à outils, les méthodes de service, qui sont programmées sans aucun
   * contrôle de la validité des arguments qui leur sont transmis, sont supposées n'être
   * appliquées qu'à des arguments préalablement contrôlés. 
   * 
   * <pre>
   * 2000 12 06 : version initiale pour Java
   * 2002 09 01 : portage pour J# et adaptation
   * 2005 04 11 : portage pour C# et adaptation aux collections cs
   * </pre>
   */
  public abstract class ArrayStorageTools : ArrayStorageKernel {

    #region Opérations internes (1) : couche intermédiaire avec le noyau

    // <wao ArrayStorageToolsHide.begin>
    /** 
     * Obtient la référence sur l'élément associé à la position aIndex.
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
     *
     * @param index position de l'élément dont on veut obtenir la référence
     * @return la référence sur l'élément se trouvant à la position index
     */
    protected object DoGet( int index ) {							                         // <wao ArrayStorageToolsHide.&body>
      return items[ index ];
    }

    /**
     * Détermine l'élément se trouvant à une position donnée.
     * Cette méthode affecte l'entrée de la liste spécifiée par l'index. 
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments.
     * 
     * @param index position de l'élément à déterminer
     * @param value élément à associer à la position spécifiée
     * @return la référence sur l'élément associé à l'entrée index avant 
     *         l'affectation du nouvel élément
     */
    protected object DoSet( int index, object value ) {				                 // <wao ArrayStorageToolsHide.&body>
      object result = items[ index ];
      items[ index ] = value;
      DoChanged( count ); // pour suivi du timbrage
      return result;
    }

    /**
     * Recherche la première occurrence d'un élément dans la liste. 
     * <br/>
     * Pour cette opération, l'élément trouvé sera, s'il existe, le premier
     * élément d'index ix tel que items[ ix ] == value. 
     *
     * @param value référence de l'objet à rechercher
     * @return l'index de la première occurrence trouvée, 
     *         -1 si l'élément est introuvable dans la liste.
     */
    protected int DoIndexOf( object value ) {							                     // <wao ArrayStorageToolsHide.&body>
      for ( int ix = 0 ; ix < Count ; ix++ )
        if ( items[ ix ] == value ) return ix;
      return -1;
    }

    /**
     * Insère un élément à la position index.
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
     *
     * @param index index d'insertion
     * @param value référence de l'élément à insérer
     */
    protected void DoInsert( int index, object value ) {                       // <wao ArrayStorageToolsHide.&body>
      DoGapAt( index, 1 );
      items[ index ] = value;
    }

    /**
     * Insère tous les éléments d'un tableau à partir d'une position donnée. 
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
     *
     * @param index position d'insertion
     * @param values tableau (éventuellement vide) d'éléments à insérer
     */
    protected void DoInsert( int index, object[] values ) {                    // <wao ArrayStorageToolsHide.&body>
      DoGapAt( index, values.Length );
      Array.Copy( values, 0, items, index, values.Length );
    }

    /**
     * Insère tous les éléments d'une ICollection à partir d'une position donnée. 
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
     *
     * @param index position d'insertion
     * @param values collection (éventuellement vide) d'éléments à insérer
     */
    protected void DoInsert( int index, ICollection values ) {                 // <wao ArrayStorageToolsHide.&body>
      DoGapAt( index, values.Count );
      values.CopyTo( items, index );
    }

    /**
     * Ajoute un élément en fin de liste. 
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments. 
     *
     * @param value élément à ajouter
     * @return index auquel l'élément a été inséré
     */
    protected int DoAdd( object value ) {                                      // <wao ArrayStorageToolsHide.&body>
      DoInsert( count, value );
      return count - 1;
    }

    /**
     * Supprime remCount éléments à partir de la position index.
     * La capacité du tableau reste inchangée. 
     * Cette méthode n'effectue aucun contrôle sur la validité des arguments.
     * 
     * @param index position du premier élément à supprimer
     * @param remCount nombre d'éléments à supprimer
     */
    protected void DoRemoveAt( int index, int remCount ) {                     // <wao ArrayStorageToolsHide.&body>
      DoUnGapAt( index, remCount );
    }

    /**
     * Recherche la première occurrence d'un élément et la supprime. 
     * La recherche de l'élément est celle de {@link #DoIndexOf}. 
     * Le tableau reste inchangé si aucun élément n'est trouvé. 
     *
     * @param value élément à rechercher et à supprimer
     * @return la position de l'élément supprimé (s'il a été trouvé), -1 sinon
     */
    protected void DoRemove( object value ) {							                     // <wao ArrayStorageToolsHide.&body>
      int index = DoIndexOf( value );
      if ( index == -1 ) return;
      DoRemoveAt( index, 1 );
    }
    // <wao ArrayStorageToolsHide.end>
    #endregion

    #region Opérations internes (2) : contrôle de la validité des index

    /**
   * Méthode de service pour vérifier la condition : 0 &lt;= index &lt; count. 
   * Cette méthode contrôle la validité de l'argument index lorsque index
   * doit désigner un élément actuellement présent dans la liste. 
   *
   * Exceptions : ArgumentOutOfRangeException si l'index est non valide
   *
   * @param index position à contrôler
   * @see #DoCheckInsert
   */
    protected void DoCheckInside( int index ) {
      if ( (index < 0) || (count <= index) ) throw new ArgumentOutOfRangeException();
    }

    /**
     * Méthode de service pour vérifier la condition : 0 &lt;= index &lt;= count.
     * Cette méthode contrôle la validité de l'argument index lorsque index
     * doit désigner un élément actuellement présent dans la liste ou un index
     * d'instertion en fin de liste (noter l'égalité possible avec le nombre 
     * d'éléments {@link #count}.
     *
     * Exceptions : ArgumentOutOfRangeException si l'index est non valide
     *
     * @param index position à contrôler
     * @see #DoCheckInside
     */
    protected void DoCheckInsert( int index ) {
      if ( (index < 0) || (count < index) ) throw new ArgumentOutOfRangeException();
    }

    /**
     *
     */
    protected void DoCheckNull( object value ) {
      if ( value == null ) throw new ArgumentNullException();
    }
    #endregion

    #region Opérations internes (3) : l'énumérateur de base

    // <wao ArrayStorageToolsHide.begin>
    /**
     * Enumérateur des éléments de la liste.
     */
    protected class Enumerator : IEnumerator {						                     // <wao ArrayStorageToolsHide.&body>

      /** Référence sur le magasin à énumérer */
      protected ArrayStorageTools store;

      /** Timbre du magasin au moement de la création de l'énumérateur */
      protected uint stamp;

      /** Index de l'élément en cours d'énumération */
      protected int index = -1;

      /**
       * Constructeur
       */
      public Enumerator( ArrayStorageTools astore ) {
        store = astore;
        Reset();
      }

      /**
       * Réinitialise l'énumération.
       *
       * Réinitialise à la fois l'index d'énumération et le timbrage des modifications
       */
      public void Reset() {
        stamp = store.stamp;
        index = -1;
      }

      /** 
       * Avance d'un pas dans l'énumération.
       * S'il y a encore des éléments à énumérer, avance d'un pas et retourne true,
       * sinon retourne false.
       *
       * Exceptions : 
       * InvalidOperationException si la liste a été modifiée depuis la création de l'énumérateur
       */
      public bool MoveNext() {
        if ( stamp != store.stamp ) throw new InvalidOperationException();
        if ( index >= store.count ) return false;
        index++;
        return index < store.count;
      }

      /** 
       * Retourne le prochain élément à énumérer.
       * 
       * Exceptions : 
       * InvalidOperationException si la liste a été modifiée depuis la création de l'énumérateur
       * InvalidOperationException s'il n'y a plus d'élément à énumérer
       */
      public object Current {
        get {
          if ( stamp != store.stamp ) throw new InvalidOperationException();
          if ( index >= store.count ) throw new InvalidOperationException();
          return store.items[ index ];
        }
      }
    }
    // <wao ArrayStorageToolsHide.end>
    #endregion

    #region Opérations et propriétés externes générales
    //	

    /** Consultation ou modification de l'incrément de capacité. */
    public int CapacityIncrement {
      get { return capacityIncrement; }
      set { capacityIncrement = value; }
    }

    /** Consultation de la capacité courante. */
    public int Capacity { get { return items.Length; } }

    /** Consultation du nombre d'éléments actuellement contenus dans la liste */
    public int Count { get { return count; } }

    /** Consultation du timbrage de suivi des modifications */
    public uint Stamp { get { return stamp; } }

    /** Retourne true si la liste est vide, false sinon */
    public bool IsEmpty { get { return count == 0; } }

    /** Propriété IsSynchronized : retourne toujours false. */
    public bool IsSynchronized { get { return false; } }

    /** Propriété SyncRoot : retourne toujours this. */
    public object SyncRoot { get { return this; } }

    /** Propriété IsReadOnly : retourne toujours false. */
    public bool IsReadOnly { get { return false; } }

    /** Propriété IsFixedSize : retourne toujours false. */
    public bool IsFixedSize { get { return false; } }

    // <wao ArrayStorageToolsHide.begin>
    /**
     * Garantit une capacité minimale du tableau	
     */
    public void EnsureCapacity( int minCapacity ) {					                   // <wao ArrayStorageToolsHide.&body>
      DoAdjustAndCopyHeader( DoGetNewCapacity( minCapacity ), count );
    }

    /**
     * Ajuste exactement la taile du tableau au nombre effectif d'éléments
     */
    public void TrimToCount() {									                               // <wao ArrayStorageToolsHide.&body>
      DoAdjustAndCopyHeader( count, count );
    }

    /** 
     * Vide la liste de tous les éléments qu'elle contient.
     * Cette méthode laisse inchangée la capacité du tableau.
     * @see #Reset
     */
    public void Clear() {										                                   // <wao ArrayStorageToolsHide.&body>
      DoUnGapAt( 0, count );
    }

    /**
     * Vide le tableau de tous ses éléments et ramène la capacité du tableau à 0.
     * @see #Clear 
     * @see #Capacity
     */
    public void Reset() {										                                   // <wao ArrayStorageToolsHide.&body>
      items = new object[ 0 ];
      DoChanged( 0 );
    }

    /** 
     * Retourne un tableau reflétant la liste des éléments.
     * Le tableau retourné n'est pas une référence sur le tableau {@link #items}
     * mais un tableau créé au vol. Le tableau retourné ne contient que les
     * références sur les éléments effectivement présents dans la liste. 
     * @see IList#toArray
     */
    public object[] ToArray() {	    							                             // <wao ArrayStorageToolsHide.&body> 
      object[] array = new object[ count ];
      Array.Copy( items, 0, array, 0, count );
      return array;
    }

    /** 
     * Retourne une chaîne correspondant à l'édition de la liste.
     */
    public override string ToString() {	    							                     // <wao ArrayStorageToolsHide.&body> 
      string result = "";
      foreach ( object item in this ) {
        if ( result == "" ) result = result + ", ";
        result = result + item;
      }
      return "(" + result + ")";
    }

    /**
     * Procède à un clonage en surface relativement aux éléments aux éléments de la liste. 
     *
     * Il s'agit, du point de vue de l'implémentation, d'un clonage en "demi-profondeur".
     * Cette méthode retourne un duplicata dans lequel le tableau de stockage items
     * est réduit au nombre d'éléments de la liste et distinct du tableau de stockage original.
     * Le timbrage du suivi des modifications est laissé tel quel. 
     */
    public override object Clone() {	    							                       // <wao ArrayStorageToolsHide.&body>
      ArrayStorageTools result = (ArrayStorageTools) base.Clone();
      items = ToArray();
      return result;
    }

    /** 
     * Retourne un énumérateur {@link IEnumerator} des éléments de la liste.
     * Deux appels distincts à la méthode GetEnumerator retournent toujours
     * deux énumérateurs distincts. 
     */
    public IEnumerator GetEnumerator() {                                       // <wao ArrayStorageToolsHide.&body>
      return new Enumerator( this );
    }                                                                          // <wao ArrayStorageToolsHide.end>
    #endregion
  }

} // namespace
                                                                               // <wao ArrayStorageTools.end>
