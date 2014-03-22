using System;
using System.Collections;

namespace Psl.Collections {

  /// <summary>
  /// Liste impl�mentant l'interface {@link IList} en tableaux dynamiques. <br/>
  /// </summary>
  /// <remarks>
  /// L'impl�mentation est r�alis�e comme une extension de la classe {@link ArrayStorageTools}.
  /// Dans cette impl�mentation, les �l�ments de liste ne sont jamais clon�s. <br/>
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
    ///  Cr�ation d'une liste vide
    /// </summary>
  	public ArrayListCustom() {                                                 // <wao ArrayListCustomHide.&body>
  	}

    /// <summary>
    /// Cr�ation d'une liste avec les �l�ments d'un tableau de {@link object}.
    /// </summary>
    /// <param name="values">tableau des �l�ments initiaux</param>
  	public ArrayListCustom( object[] values ) {                                // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
  	  DoInsert( 0, values ) ; 
  	}

    /// <summary>
    /// Cr�ation d'une liste avec les �l�ments d'une {@link ICollection}.
    /// </summary>
    /// <param name="values">collection des �l�ments initiaux</param>
  	public ArrayListCustom( ICollection values ) {                             // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
  	  DoInsert( 0, values ) ; 
  	}

    /// <summary>
    /// Obtenir ou remplacer un �l�ment d'un index donn�.
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
    /// Copie les �l�ments de la liste dans le tableau where � partir de index.
    /// </summary>
    /// <param name="where">tableau de destination</param>
    /// <param name="index">index � partir duquel ins�rer dans le tableau where</param>
    public void CopyTo( Array where, int index ) {                             // <wao ArrayListCustomHide.&body>
      DoCheckInside( index ) ;
      Array.Copy( items, 0, where, index, count ) ;
    }
    
    /// <summary>
    /// Recherche l'index d'un �l�ment dans la liste.
    /// </summary>
    /// <param name="value">�l�ment � rechercher</param>
    /// <returns>l'index de l'�l�ment si trouv�, sinon -1</returns>
    public int IndexOf( object value ) {                                       // <wao ArrayListCustomHide.&body> 
      return DoIndexOf( value ) ; 
    }

    /// <summary>
    /// Recherche l'index d'un �l�ment dans la liste.
    /// </summary>
    /// <param name="value">�l�ment � rechercher</param>
    /// <returns>true si trouv�, sinon false</returns>
    public bool Contains( object value ) {                                     // <wao ArrayListCustomHide.&body> 
      return DoIndexOf( value ) != -1 ; 
    }

    /// <summary>
    /// Ins�re un �l�ment � une position donn�e.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="value">�l�ment � ins�rer</param>
    public void Insert( int index, object value ) {                            // <wao ArrayListCustomHide.&body> 
      DoCheckInsert( index ) ;
      DoInsert     ( index, value ) ;
    }

    /// <summary>
    /// Ins�re les �l�ments d'un tableau de {@link object} � une position donn�e.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="values">tableau des �l�ments � ins�rer</param>
    public void Insert( int index, object[] values ) {                         // <wao ArrayListCustomHide.&body>
  	  DoCheckNull  ( values ) ; 
      DoCheckInsert( index  ) ;
      DoInsert     ( index, values ) ;
    }

    /// <summary>
    /// Ins�re les �l�ments d'une collection {@link ICollection} � une position donn�e.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="values">collection des �l�ments � ins�rer</param>
    public void Insert( int index, ICollection values ) {                      // <wao ArrayListCustomHide.&body>
  	  DoCheckNull  ( values ) ; 
      DoCheckInsert( index  ) ;
      DoInsert     ( index, values ) ;
    }

    /// <summary>
    /// Ajoute un �l�ment en fin de liste.
    /// </summary>
    /// <param name="value">�l�ment � ajouter</param>
    /// <returns>l'index auquel l'�l�ment a �t� ins�r�</returns>
    public int Add( object value ) {                                           // <wao ArrayListCustomHide.&body> 
      return DoAdd( value ) ;
    }
	
    /// <summary>
    /// Ajoute les �l�ments d'un tableau de {@link object} � la fin de la liste.
    /// </summary>
    /// <param name="values">tableau des �l�ments � ajouter en fin de liste</param>
    public void Add( object[] values ) {                                       // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
      DoInsert   ( count, values ) ;
    }

    /// <summary>
    /// Ajoute les �l�ments d'une collection {@link ICollection} � la fin de la liste.
    /// </summary>
    /// <param name="values">collection des �l�ments � ajouter en fin de liste</param>
    public void Add( ICollection values ) {                                    // <wao ArrayListCustomHide.&body>
  	  DoCheckNull( values ) ; 
      DoInsert   ( count, values ) ;
    }
	
    /// <summary>
    /// Supprime un �l�ment et l'entr�e de la liste associ�e.
    /// </summary>
    /// <param name="value">�l�ment � supprimer</param>
    public void Remove( object value ) {                                       // <wao ArrayListCustomHide.&body> 
      DoRemove( value ) ; 
    }
    
    /// <summary>
    /// Supprime une entr�e de la liste.
    /// </summary>
    /// <param name="index">index de l'�l�ment � supprimer</param>
    public void RemoveAt( int index ) {                                        // <wao ArrayListCustomHide.&body>
		  DoCheckInside( index    ) ;		
      DoRemoveAt   ( index, 1 ) ;
    }
                                                                               // <wao ArrayListCustomHide.end>
  }
} // namespace
