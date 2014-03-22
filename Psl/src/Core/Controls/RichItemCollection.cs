/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 21 05 2009 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Collections;

namespace Psl.Controls {

  /*
  public class RichItemCollection : IList, ICollection, IEnumerable                          // <wao RichItemCollection_abr�g�.+header>
   */

  /// <summary>
  /// Collection d'�l�ments de type <see cref="RichItem"/>
  /// </summary>
  /// <remarks>
  /// Cette collection est un wrapper pour les collections originales des contr�les h�tes.
  /// </remarks>
  public class RichItemCollection : IList, ICollection, IEnumerable, IRichCollectionOwner {  // <wao code.&header>

    //
    // Champs
    //

    private IRichControlHost host = null;

    //
    // Constructeur                                                                      // <wao code.&comitem>
    //

    /// <summary>
    /// Constructeur                                                                     
    /// </summary>
    /// <param name="host">h�te de la collection</param>
    public RichItemCollection( IRichControlHost host ) {                                 // <wao code.&body>
      this.host = host;
    }

    //
    // Service
    //

    private void DoWireItemOff( RichItem item ) {
      IRichCollectionOwner itemOwner = item == null ? null : item.Owner;
      if ( itemOwner == null ) return;
      item.InternalWireToOwner( null );
      if (itemOwner != this) itemOwner.Remove( item );
    }

    private void DoWireItemOn( RichItem item ) {
      if ( item == null || item.Owner == this ) return;
      DoWireItemOff( item );
      item.InternalWireToOwner( this );
    }

    //
    // Impl�mentation de ICollection
    //

    void ICollection.CopyTo( Array array, int index ) {

      for ( IEnumerator e = GetEnumerator() ; e.MoveNext() ; )
        array.SetValue( e.Current, index++ );
    }

    bool ICollection.IsSynchronized {
      get { return false; }
    }

    object ICollection.SyncRoot {
      get { return this; }
    }    

    //
    // Impl�mentation de IList
    //

    object IList.this[ int index ] {
      get { return this[ index ]; }
      set { this[ index ] = (RichItem) value; }
    }

    bool IList.Contains( object item ) {
      return this.Contains( item );
    }

    int IList.Add( object item ) {
      return this.Add( (RichItem) item );
    }

    bool IList.IsFixedSize {
      get { return false; }
    }

    int IList.IndexOf( object item ) {
      return this.IndexOf( item );
    }

    void IList.Insert( int index, object item ) {
      this.Insert( index, (RichItem) item );
    }

    void IList.Remove( object item ) {
      this.Remove( (RichItem) item );
    }

    void IList.RemoveAt( int index ) {
      this.RemoveAt( index );
    }

    //
    // Impl�mentation de IRichCollectionOwner
    //

    /// <summary>
    /// Indique si la collection est h�berg�e, c'est-�-dire si <see cref="Host"/> est non null.
    /// </summary>
    bool IRichCollectionOwner.IsHosted {
      get { return host != null; }
    }

    void IRichCollectionOwner.OnItemChanged( RichItem item ) {
      host.OnItemChanged( item );
    }

    //
    // Impl�mentation de la collection
    //
   
    /// <summary>
    /// Indique si la collection est en lecture seule.
    /// </summary>
    public bool IsReadOnly {
      get { return host.Items.IsReadOnly; }
    }

    // Liaison au contr�le h�te                                                          // <wao code.&comitem>

    /// <summary>
    /// Obtient la r�f�rence sur le contr�le h�te de la collection.
    /// </summary>
    public IRichControlHost Host {                                                       // <wao code.&body:ro>
      get { return host; }
    }

    // Gestion g�n�rale de la collection                                                 // <wao code.&comitem>

    /// <summary>
    /// Obtient le nombre d'�l�ments de la liste.
    /// </summary>
    public int Count {                                                                  // <wao code.&body:ro RichItemCollection_abr�g�.+body:ro>
      get { return host.Items.Count; }
    }

    /// <summary>
    /// Obtient ou d�termine un �l�ment de la collection.
    /// </summary>
    /// <param name="index">index de l'�l�ment</param>
    /// <returns>l'�l�ment � la position index</returns>
    public RichItem this[ int index ] {                                                  // <wao code.&body:rw RichItemCollection_abr�g�.+body:rw>
      get { return (RichItem) host.Items[ index ]; }
      set {
        if ( value == null ) throw new ArgumentNullException( "value", "Indexeur this[int]" );
        host.Items[ index ] = value;
      }
    }

    /// <summary>
    /// Indique si un �l�ment figure dans la collection.
    /// </summary>
    /// <param name="item">r�f�rence sur l'�l�ment concern�</param>
    /// <returns>true si l'�l�ment appartient � la collection, false sinon</returns>
    public bool Contains( object item ) {                                                // <wao code.&body RichItemCollection_abr�g�.+body>
      return host.Items.Contains( item );
    }

    /// <summary>
    /// Obtient l'index d'un item dans la collection.
    /// </summary>
    /// <param name="item">r�f�rence sur l'item � rechercher</param>
    /// <returns>l'index de l'item dans la collection, -1 si l'item ne figure pas dans la collection</returns>
    public int IndexOf( object item ) {                                                  // <wao code.&body RichItemCollection_abr�g�.+body>
      return host.Items.IndexOf( item );
    }

    /// <summary>
    /// Retire un item de la collection
    /// </summary>
    /// <param name="item">r�f�rence sur l'item � retirer</param>
    public void Remove( RichItem item ) {                                                // <wao code.&body RichItemCollection_abr�g�.+body>
      DoWireItemOff( item );
      host.Items.Remove( item );
    }

    /// <summary>
    /// Retire l'item � une position donn�e.
    /// </summary>
    /// <param name="index">index de l'�l�ment � supprimer</param>
    public void RemoveAt( int index ) {                                                  // <wao code.&body RichItemCollection_abr�g�.+body>
      DoWireItemOff( this[ index ] );
      host.Items.RemoveAt( index );
    }

    /// <summary>
    /// Ins�re un item � une position donn�e.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="item">r�f�rence sur l'item � ajouter</param>
    public void Insert( int index, RichItem item ) {                                     // <wao code.&body RichItemCollection_abr�g�.+body>
      if ( item == null ) throw new ArgumentNullException( "item", "M�thode Insert" );
      if ( index < 0 || Count < index ) throw new ArgumentOutOfRangeException( "index", "m�thode RichItemCollection.Insert" );
      DoWireItemOn( item );
      host.Items.Insert( index, item );
    }

    /// <summary>
    /// Ajoute un item � la fin de la collection.
    /// </summary>
    /// <param name="item">r�f�rence sur l'item � ajouter</param>
    /// <returns>l'index d'adjonction</returns>
    public int Add( RichItem item ) {                                                    // <wao code.&body RichItemCollection_abr�g�.+body>
      if ( item == null ) throw new ArgumentNullException( "item", "M�thode Add" );
      DoWireItemOn( item );
      return host.Items.Add( item );
    }

    /// <summary>
    /// Ajoute une collection d'items � la fin de la collection
    /// </summary>
    /// <param name="items">tableau d'items � ajouter</param>
    public void AddRange( RichItem[] items ) {                                           // <wao code.&body RichItemCollection_abr�g�.+body>
      if ( items == null ) return;
      host.BeginUpdate();
      try {
        for ( int ix = 0 ; ix < items.Length ; ix++ )
          Add( items[ ix ] );
      }
      finally { host.EndUpdate(); }
    }

    /// <summary>
    /// Retire tous les �l�ments de la collection
    /// </summary>
    public void Clear() {                                                                // <wao code.&body RichItemCollection_abr�g�.+body>
      host.BeginUpdate();
      try {
        for ( int ix = 0 ; ix < Count ; ix++ )
          DoWireItemOff( this[ ix ] );
        host.Items.Clear();
      }
      finally { host.EndUpdate(); }
    }

    /// <summary>
    /// Obtient un �num�rateur sur la collection.
    /// </summary>
    /// <returns>un �num�rateur sur la collection</returns>
    public IEnumerator GetEnumerator() {                                                 // <wao code.&body RichItemCollection_abr�g�.+body>
      return host.Items.GetEnumerator();
    }
  }                                                                                      // <wao code.&ender RichItemCollection_abr�g�.+ender>
}
