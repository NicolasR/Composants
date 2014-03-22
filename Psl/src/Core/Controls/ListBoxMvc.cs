/*                                                                           // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 30 11 2002 : version initiale
 * 07 12 2003 : adaptation au contexte vjs
 * 23 04 2005 : portage sur cs et adaptation
 * 29 05 2006 : compléments et contournement de difficultés
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 28 11 2008 : normalisation des identificateurs
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;

namespace Psl.Controls {

  /// <summary>
  /// Extension de la classe ListBox adaptée à une gestion mvc.
  /// </summary>
  /// <remarks>
  /// L'adaptation consiste simplement à inhiber le déclenchement de l'événement
  /// SelectedIndexChanged lorsque les changements de la boîte liste sont provoqués par 
  /// l'application en cours et non par l'effet d'action utilisateur. 
  /// <br/>
  /// A cause du fait que les méthodes qui permettent de modifier la boîte liste
  /// par programme sont liées à une collection (propriété Items) dont la référence
  /// est accessible en lecture seule, il est impossible de redéclarer (donc de redéfinir)
  /// ces méthodes. 
  /// <br/>
  /// L'extension consiste à ajouter, au niveau de la classe ListBoxMvc, différentes méthodes
  /// dont l'identificateur est préfixé par "mvc". Dans ces méthodes, le booléen asView 
  /// permet de déterminer si la méthode se comporte comme la méthode correspondante provenant
  /// du composant ListBox d'origine (asView = false) ou si la méthode inhibe le déclenchement 
  /// de l'événement SelectedIndexChanged (asView = true).
  /// </remarks>
  [
  Description( "Extension du composant ListBox adaptée à une gestion mvc" )
  ]
  public class ListBoxMvc : Psl.Controls.ListBoxEnh {

    /// <summary>
    /// Compteur des emboîtements pour le verrouillage des événements.
    /// </summary>
    private int mvcLocker = 0 ;

    /// <summary>
    /// Requis par le concepteur Windows Forms
    /// </summary>
    private System.ComponentModel.Container components = null;

    /// <summary>
    /// Constructeur avec insertion dans un conteneur.
    /// </summary>
    /// <param name="container">Référence sur le conteneur</param>
    public ListBoxMvc(System.ComponentModel.IContainer container) {
      container.Add(this);
      InitializeComponent();
    }

    /// <summary>
    /// Constructeur sans insertion dans un conteneur.
    /// </summary>
    public ListBoxMvc() {
      InitializeComponent();
    }

    /// <summary>
    /// Verrouille le déclenchement de l'événement SelectedIndexChanged.
    /// </summary>
    /// <remarks>
    /// Incrémente le compteur d'emboîtements.
    /// </remarks>
    public void MvcLockOn() {
      mvcLocker ++ ;
    }
  
    /// <summary>
    /// Déverrouille le déclenchement de l'événement SelectedIndexChanged.
    /// </summary>
    /// <remarks>
    /// Décrémente le compteur d'emboîtements 
    /// </remarks>
    public void MvcLockOff() {
      mvcLocker -- ;
    }
  
    /// <summary>
    /// Permet de savoir si le déclenchement de l'événement SelectedIndexChanged est inhibé.
    /// </summary>
    /// <value>Retourne true si le compteur de verrouillage est diférent de 0, false sinon.</value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public bool MvcIsLocked {
      get { return mvcLocker != 0 ; }
    }
  
    /// <summary>
    /// Redéfinition de la méthode protégée héritée de ListBox.
    /// </summary>
    /// <remarks>
    /// Filtrage du déclenchement de l'événement selon la valeur du compteur de
    /// verrouillage #mvcLocker. 
    ///
    /// Si le verrou n'est pas armé (mvcLocker == 0),
    /// la méthode se borne à appeler la méthode héritée (ce qui aura pour effet de
    /// déclencher les event handlers associés à l'événement) ; si le verrou est armé
    /// (mvcLocker != 0), la méthode ne fait rien, de sorte que les event handlers associés
    /// à l'événement ne seront pas déclenchés. 
    /// </remarks>
    /// <param name="e">Descripteur de l'événement</param>
    protected  override void OnSelectedIndexChanged(EventArgs e) {
      if (mvcLocker == 0) 
        base.OnSelectedIndexChanged(e) ;
    }

    /// <summary>
    /// Redéfinition de la méthode protégée héritée de ListBox.
    /// </summary>
    /// <remarks>
    /// Filtrage du déclenchement de l'événement selon la valeur du compteur de
    /// verrouillage #mvcLocker. 
    ///
    /// Si le verrou n'est pas armé (mvcLocker == 0),
    /// la méthode se borne à appeler la méthode héritée (ce qui aura pour effet de
    /// déclencher les event handlers associés à l'événement) ; si le verrou est armé
    /// (mvcLocker != 0), la méthode ne fait rien, de sorte que les event handlers associés
    /// à l'événement ne seront pas déclenchés. 
    /// </remarks>
    /// <param name="e">Descripteur de l'événement</param>
    protected  override void OnSelectedValueChanged(EventArgs e) {
      if (mvcLocker == 0) 
        base.OnSelectedValueChanged(e) ;
    }

    /// <summary>
    /// Ajoute un item à la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.Add(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item à ajouter à la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    /// <returns>l'index où l'item a été inséré</returns>
    public int MvcAdd(object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { return Items.Add( item ) ; } 
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime un item de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.RemoveAt(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item à retirer de la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcRemoveAt(int index, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.RemoveAt( index ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime un item de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.Remove(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item à retirer de la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcRemove(object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Remove( item ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime tous les items de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.Clear(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcClear(bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Clear() ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Insère un item dans la boîte liste. 
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.Insert(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index d'insertion de l'item</param>
    /// <param name="item">item à insérer dans la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcInsert(int index, object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Insert( index, item ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Modifie un item de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.set_Item(...) 
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item à modifier</param>
    /// <param name="item">nouvel item de remplacement</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetItem(int index, object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items[ index ] = item ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Ajoute à la boîte liste les éléments d'un tableau.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme Items.AddRange(...)
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChange</param>
    public void MvcAddRange(object[] items, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.AddRange( items ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Détermine tous les items de la boîte liste et tente de restaurer l'élément sélectionné. 
    /// </summary>
    /// <remarks>
    /// Cette méthode utilise la méthode SetRange du composant ListBoxEnh qui enchaîne 
    /// (1) le nettoyage de la liste (via Clear), (2) l'adjonction des éléments transmis via 
    /// le tableau items, et (3) tente de resélectionner l'élément antérieurement sélectionné. 
    /// <br/>
    /// Dans cette extension légère de ListBox, la tentative de re-sélection ne porte que sur un
    /// élément au plus (les sélections mutltiples ne sont pas prises en charge. 
    /// <br/>
    /// Lorsque asView est true, l'événement SelectedIndexChanged  est inhibé de manière à permettre 
    /// une utilisation sobre du composant dans  un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="reselect">si true, tente de restaurer la sélection</param>
    /// <param name="asView">inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetRange(object[] items, bool reselect, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { SetRange( items, reselect ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Détermine tous les items de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme la méthode SetRange héritée de ListBoxEnh,
    /// sans tentative de resélection, quand asView est false. Lorsque asView est true, 
    /// l'événement SelectedIndexChanged  est inhibé de manière à permettre une utilisation 
    /// sobre du composant dans  un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetRange(object[] items, bool asView) {
      MvcSetRange( items, false, asView ) ;
    }
  
    /// <summary>
    /// Sélectionne ou désélectionne un item de la boîte liste.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme SetSelected(...)
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item dont on veut modifier la sélection</param>
    /// <param name="value">si true, sélctionner, si false désélectionner</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetSelected( int index, bool value, bool asView ) {
      if (asView) mvcLocker ++ ;
      try { SetSelected( index, value ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Détermine l'index de l'élément sélectionné.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme set_SelectedIndex(...)
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item à sélectionner, désélectionner tout si -1</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetSelectedIndex( int index, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedIndex = index ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Détermine l'item sélectionné.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme l'accesseur set de la propriété SelectedItem
    /// si asView est false. Lorsque asView est true, l'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item à sélectionner, désélectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetSelectedItem( object item, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedItem = item ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// Détermine l'item sélectionné en tant que chaîne de caractères.
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme l'accesseur set de la propriété SelectedString
    /// héritée du composant ListBoxEnh si asView est false. Lorsque asView est true, l'événement 
    /// SelectedIndexChanged est inhibé de manière à permettre une utilisation sobre du composant 
    /// dans un contexte mvc.
    /// </remarks>
    /// <param name="item">item à sélectionner, désélectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetSelectedString( string item, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedString = item ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// Détermine l'item sélectionné selon la valeur (MemberValue).
    /// </summary>
    /// <remarks>
    /// Cette méthode se comporte exactement comme l'accesseur set de la propriété SelectedValue
    /// héritée du composant ListBoxEnh si asView est false. Lorsque asView est true, l'événement 
    /// SelectedIndexChanged est inhibé de manière à permettre une utilisation sobre du composant 
    /// dans un contexte mvc.
    /// </remarks>
    /// <param name="value">item à sélectionner, désélectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'événement SelectedIndexChanged</param>
    public void MvcSetSelectedValue( object value, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedValue = value ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// Accès à l'index de l'élément sélectionné en tant que vue.
    /// </summary>
    /// <value>
    /// En lecture, cette propriété se comporte exactement comme SelectedInex.
    /// En écriture, cette propiété se comporte exactement comme MvcSetSelectedIndex(...)
    /// avec asView à true. L'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public int MvcSelectedIndex {
      get { return SelectedIndex ; }
      set { MvcSetSelectedIndex( value, true ) ; }
    }

    /// <summary>
    /// Accéder à ou déterminer l'item sélectionné en tant que référence de type object.
    /// </summary>
    /// <value>
    /// En lecture, cette propriété se comporte exactement comme la propriété SelectedItem
    /// héritée de ListBoxEnh (qui redéclare la propriété SelectedItem de ListBox).
    /// En écriture, cette propriété se comporte exactement comme MvcSetSelectedItem(...)
    /// avec asView à true. L'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public object MvcSelectedItem {
      get { return SelectedItem ; }
      set { MvcSetSelectedItem( value, true ) ; }
    }

    /// <summary>
    /// Accéder à ou déterminer l'item sélectionné en tant que chaîne de caractères.
    /// </summary>
    /// <value>
    /// En lecture, cette propriété se comporte exactement comme la propriété SelectedString
    /// héritée de ListBoxEnh.
    /// En écriture, cette propriété se comporte exactement comme MvcSetSelectedString(...)
    /// avec asView à true. L'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public string MvcSelectedString {
      get { return SelectedString ; }
      set { MvcSetSelectedString( value, true ) ; }
    }

    /// <summary>
    /// Accéder à ou déterminer l'item sélectionné selon la valeur (MemberValue).
    /// </summary>
    /// <value>
    /// En lecture, cette propriété se comporte exactement comme la propriété SelectedValue.
    /// En écriture, cette propriété se comporte exactement comme MvcSetSelectedValue(...)
    /// avec asView à true. L'événement SelectedIndexChanged 
    /// est inhibé de manière à permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public object MvcSelectedValue {
      get { return SelectedValue ; }
      set { MvcSetSelectedValue( value, true ) ; }
    }

    /// <summary>
    /// Nettoyage du composant
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être libérées</param>
    protected override void Dispose( bool disposing ) {
      if( disposing )	{
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Code généré par le Concepteur de composants
    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
    }
    #endregion
  }

}