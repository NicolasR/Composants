/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 21 05 2009 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Collections;

namespace Psl.Controls {

  /*
  public class RichItemCollection : IList, ICollection, IEnumerable                          // <wao RichItemCollection_abrégé.+header>
   */

  /// <summary>
  /// Collection d'éléments de type <see cref="RichItem"/>
  /// </summary>
  /// <remarks>
  /// Cette collection est un wrapper pour les collections originales des contrôles hôtes.
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
    /// <param name="host">hôte de la collection</param>
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
    // Implémentation de ICollection
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
    // Implémentation de IList
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
    // Implémentation de IRichCollectionOwner
    //

    /// <summary>
    /// Indique si la collection est hébergée, c'est-à-dire si <see cref="Host"/> est non null.
    /// </summary>
    bool IRichCollectionOwner.IsHosted {
      get { return host != null; }
    }

    void IRichCollectionOwner.OnItemChanged( RichItem item ) {
      host.OnItemChanged( item );
    }

    //
    // Implémentation de la collection
    //
   
    /// <summary>
    /// Indique si la collection est en lecture seule.
    /// </summary>
    public bool IsReadOnly {
      get { return host.Items.IsReadOnly; }
    }

    // Liaison au contrôle hôte                                                          // <wao code.&comitem>

    /// <summary>
    /// Obtient la référence sur le contrôle hôte de la collection.
    /// </summary>
    public IRichControlHost Host {                                                       // <wao code.&body:ro>
      get { return host; }
    }

    // Gestion générale de la collection                                                 // <wao code.&comitem>

    /// <summary>
    /// Obtient le nombre d'éléments de la liste.
    /// </summary>
    public int Count {                                                                  // <wao code.&body:ro RichItemCollection_abrégé.+body:ro>
      get { return host.Items.Count; }
    }

    /// <summary>
    /// Obtient ou détermine un élément de la collection.
    /// </summary>
    /// <param name="index">index de l'élément</param>
    /// <returns>l'élément à la position index</returns>
    public RichItem this[ int index ] {                                                  // <wao code.&body:rw RichItemCollection_abrégé.+body:rw>
      get { return (RichItem) host.Items[ index ]; }
      set {
        if ( value == null ) throw new ArgumentNullException( "value", "Indexeur this[int]" );
        host.Items[ index ] = value;
      }
    }

    /// <summary>
    /// Indique si un élément figure dans la collection.
    /// </summary>
    /// <param name="item">référence sur l'élément concerné</param>
    /// <returns>true si l'élément appartient à la collection, false sinon</returns>
    public bool Contains( object item ) {                                                // <wao code.&body RichItemCollection_abrégé.+body>
      return host.Items.Contains( item );
    }

    /// <summary>
    /// Obtient l'index d'un item dans la collection.
    /// </summary>
    /// <param name="item">référence sur l'item à rechercher</param>
    /// <returns>l'index de l'item dans la collection, -1 si l'item ne figure pas dans la collection</returns>
    public int IndexOf( object item ) {                                                  // <wao code.&body RichItemCollection_abrégé.+body>
      return host.Items.IndexOf( item );
    }

    /// <summary>
    /// Retire un item de la collection
    /// </summary>
    /// <param name="item">référence sur l'item à retirer</param>
    public void Remove( RichItem item ) {                                                // <wao code.&body RichItemCollection_abrégé.+body>
      DoWireItemOff( item );
      host.Items.Remove( item );
    }

    /// <summary>
    /// Retire l'item à une position donnée.
    /// </summary>
    /// <param name="index">index de l'élément à supprimer</param>
    public void RemoveAt( int index ) {                                                  // <wao code.&body RichItemCollection_abrégé.+body>
      DoWireItemOff( this[ index ] );
      host.Items.RemoveAt( index );
    }

    /// <summary>
    /// Insère un item à une position donnée.
    /// </summary>
    /// <param name="index">index d'insertion</param>
    /// <param name="item">référence sur l'item à ajouter</param>
    public void Insert( int index, RichItem item ) {                                     // <wao code.&body RichItemCollection_abrégé.+body>
      if ( item == null ) throw new ArgumentNullException( "item", "Méthode Insert" );
      if ( index < 0 || Count < index ) throw new ArgumentOutOfRangeException( "index", "méthode RichItemCollection.Insert" );
      DoWireItemOn( item );
      host.Items.Insert( index, item );
    }

    /// <summary>
    /// Ajoute un item à la fin de la collection.
    /// </summary>
    /// <param name="item">référence sur l'item à ajouter</param>
    /// <returns>l'index d'adjonction</returns>
    public int Add( RichItem item ) {                                                    // <wao code.&body RichItemCollection_abrégé.+body>
      if ( item == null ) throw new ArgumentNullException( "item", "Méthode Add" );
      DoWireItemOn( item );
      return host.Items.Add( item );
    }

    /// <summary>
    /// Ajoute une collection d'items à la fin de la collection
    /// </summary>
    /// <param name="items">tableau d'items à ajouter</param>
    public void AddRange( RichItem[] items ) {                                           // <wao code.&body RichItemCollection_abrégé.+body>
      if ( items == null ) return;
      host.BeginUpdate();
      try {
        for ( int ix = 0 ; ix < items.Length ; ix++ )
          Add( items[ ix ] );
      }
      finally { host.EndUpdate(); }
    }

    /// <summary>
    /// Retire tous les éléments de la collection
    /// </summary>
    public void Clear() {                                                                // <wao code.&body RichItemCollection_abrégé.+body>
      host.BeginUpdate();
      try {
        for ( int ix = 0 ; ix < Count ; ix++ )
          DoWireItemOff( this[ ix ] );
        host.Items.Clear();
      }
      finally { host.EndUpdate(); }
    }

    /// <summary>
    /// Obtient un énumérateur sur la collection.
    /// </summary>
    /// <returns>un énumérateur sur la collection</returns>
    public IEnumerator GetEnumerator() {                                                 // <wao code.&body RichItemCollection_abrégé.+body>
      return host.Items.GetEnumerator();
    }
  }                                                                                      // <wao code.&ender RichItemCollection_abrégé.+ender>
}
