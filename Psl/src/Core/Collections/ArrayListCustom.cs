using System;
using System.Collections;

namespace Psl.Collections {

  /// <summary>
  /// Liste implémentant l'interface {@link IList} en tableaux dynamiques. <br/>
  /// </summary>
  /// <remarks>
  /// L'implémentation est réalisée comme une extension de la classe {@link ArrayStorageTools}.
  /// Dans cette implémentation, les éléments de liste ne sont jamais clonés. <br/>
  ///
  /// <pre>
  /// 2000 12 06 : version initiale pour Java
  /// 2002 09 01 : portage pour J# et adaptation
  /// 2005 04 11 : portage pour C# et adaptation aux collections vsn
  /// </pre>
  /// </remarks>
  public class ArrayListCustom : ArrayStorageTools, 
                                 ICloneable, IEnumerable, ICollection, IList {

                                                                               // <wao ArrayListCustomHide.begin>
    /// <summary>
    ///  Création d'une liste vide
    /// </summary>
  	public ArrayListCustom() {                                                 // <wao ArrayListCustomHide.&body>
  	}

    /// <summary>
    /// Création d'une liste avec les éléments d'un tableau de {@link object}.
    /// </summary>
    /// <param name="values">tableau des éléments initiaux</param>
  	public ArrayListCustom( object[] values ) {                                // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
  	  DoInsert( 0, values ) ; 
  	}

    /// <summary>
    /// Création d'une liste avec les éléments d'une {@link ICollection}.
    /// </summary>
    /// <param name="values">collection des éléments initiaux</param>
  	public ArrayListCustom( ICollection values ) {                             // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
  	  DoInsert( 0, values ) ; 
  	}

    /// <summary>
    /// Obtenir ou remplacer un élément d'un index donné.
    /// </summary>
  	public object this[ int index ] {                                          // <wao ArrayListCustomHide.&body>
  	  get {
    		DoCheckInside( index ) ;
    		return DoGet( index ) ;
  	  }
  	  set {
    		DoCheckInside( index ) ;
  	  	DoSet ( index, value ) ;
  	  }
  	}
	
    /// <summary>
    /// Copie les éléments de la liste dans le tableau where à partir de index.
    /// </summary>
    /// <param name="where">tableau de destination</param>
    /// <param name="index">index à partir duquel insérer dans le tableau where</param>
    public void CopyTo( Array where, int index ) {                             // <wao ArrayListCustomHide.&body>
      DoCheckInside( index ) ;
      Array.Copy( items, 0, where, index, count ) ;
    }
    
    /// <summary>
    /// Recherche l'index d'un élément dans la liste.
    /// </summary>
    /// <param name="value">élément à rechercher</param>
    /// <returns>l'index de l'élément si trouvé, sinon -1</returns>
    public int IndexOf( object value ) {                                       // <wao ArrayListCustomHide.&body> 
      return DoIndexOf( value ) ; 
    }

    /// <summary>
    /// Recherche l'index d'un élément dans la liste.
    /// </summary>
    /// <param name="value">élément à rechercher</param>
    /// <returns>true si trouvé, sinon false</returns>
    public bool Contains( object value ) {                                     // <wao ArrayListCustomHide.&body> 
      return DoIndexOf( value ) != -1 ; 
    }

    /// <summary>
    /// Insère un élément à une position donnée.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="value">élément à insérer</param>
    public void Insert( int index, object value ) {                            // <wao ArrayListCustomHide.&body> 
      DoCheckInsert( index ) ;
      DoInsert     ( index, value ) ;
    }

    /// <summary>
    /// Insère les éléments d'un tableau de {@link object} à une position donnée.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="values">tableau des éléments à insérer</param>
    public void Insert( int index, object[] values ) {                         // <wao ArrayListCustomHide.&body>
  	  DoCheckNull  ( values ) ; 
      DoCheckInsert( index  ) ;
      DoInsert     ( index, values ) ;
    }

    /// <summary>
    /// Insère les éléments d'une collection {@link ICollection} à une position donnée.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="values">collection des éléments à insérer</param>
    public void Insert( int index, ICollection values ) {                      // <wao ArrayListCustomHide.&body>
  	  DoCheckNull  ( values ) ; 
      DoCheckInsert( index  ) ;
      DoInsert     ( index, values ) ;
    }

    /// <summary>
    /// Ajoute un élément en fin de liste.
    /// </summary>
    /// <param name="value">élément à ajouter</param>
    /// <returns>l'index auquel l'élément a été inséré</returns>
    public int Add( object value ) {                                           // <wao ArrayListCustomHide.&body> 
      return DoAdd( value ) ;
    }
	
    /// <summary>
    /// Ajoute les éléments d'un tableau de {@link object} à la fin de la liste.
    /// </summary>
    /// <param name="values">tableau des éléments à ajouter en fin de liste</param>
    public void Add( object[] values ) {                                       // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
      DoInsert   ( count, values ) ;
    }

    /// <summary>
    /// Ajoute les éléments d'une collection {@link ICollection} à la fin de la liste.
    /// </summary>
    /// <param name="values">collection des éléments à ajouter en fin de liste</param>
    public void Add( ICollection values ) {                                    // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
      DoInsert   ( count, values ) ;
    }
	
    /// <summary>
    /// Supprime un élément et l'entrée de la liste associée.
    /// </summary>
    /// <param name="value">élément à supprimer</param>
    public void Remove( object value ) {                                       // <wao ArrayListCustomHide.&body> 
      DoRemove( value ) ; 
    }
    
    /// <summary>
    /// Supprime une entrée de la liste.
    /// </summary>
    /// <param name="index">index de l'élément à supprimer</param>
    public void RemoveAt( int index ) {                                        // <wao ArrayListCustomHide.&body>
		  DoCheckInside( index    ) ;		
      DoRemoveAt   ( index, 1 ) ;
    }
                                                                               // <wao ArrayListCustomHide.end>
  }
} // namespace
