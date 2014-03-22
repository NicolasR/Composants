/*                                                                           // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 30 11 2002 : version initiale
 * 07 12 2003 : adaptation au contexte vjs
 * 23 04 2005 : portage sur cs et adaptation
 * 29 05 2006 : compl�ments et contournement de difficult�s
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 28 11 2008 : normalisation des identificateurs
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;

namespace Psl.Controls {

  /// <summary>
  /// Extension de la classe ListBox adapt�e � une gestion mvc.
  /// </summary>
  /// <remarks>
  /// L'adaptation consiste simplement � inhiber le d�clenchement de l'�v�nement
  /// SelectedIndexChanged lorsque les changements de la bo�te liste sont provoqu�s par 
  /// l'application en cours et non par l'effet d'action utilisateur. 
  /// <br/>
  /// A cause du fait que les m�thodes qui permettent de modifier la bo�te liste
  /// par programme sont li�es � une collection (propri�t� Items) dont la r�f�rence
  /// est accessible en lecture seule, il est impossible de red�clarer (donc de red�finir)
  /// ces m�thodes. 
  /// <br/>
  /// L'extension consiste � ajouter, au niveau de la classe ListBoxMvc, diff�rentes m�thodes
  /// dont l'identificateur est pr�fix� par "mvc". Dans ces m�thodes, le bool�en asView 
  /// permet de d�terminer si la m�thode se comporte comme la m�thode correspondante provenant
  /// du composant ListBox d'origine (asView = false) ou si la m�thode inhibe le d�clenchement 
  /// de l'�v�nement SelectedIndexChanged (asView = true).
  /// </remarks>
  [
  Description( "Extension du composant ListBox adapt�e � une gestion mvc" )
  ]
  public class ListBoxMvc : Psl.Controls.ListBoxEnh {

    /// <summary>
    /// Compteur des embo�tements pour le verrouillage des �v�nements.
    /// </summary>
    private int mvcLocker = 0 ;

    /// <summary>
    /// Requis par le concepteur Windows Forms
    /// </summary>
    private System.ComponentModel.Container components = null;

    /// <summary>
    /// Constructeur avec insertion dans un conteneur.
    /// </summary>
    /// <param name="container">R�f�rence sur le conteneur</param>
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
    /// Verrouille le d�clenchement de l'�v�nement SelectedIndexChanged.
    /// </summary>
    /// <remarks>
    /// Incr�mente le compteur d'embo�tements.
    /// </remarks>
    public void MvcLockOn() {
      mvcLocker ++ ;
    }
  
    /// <summary>
    /// D�verrouille le d�clenchement de l'�v�nement SelectedIndexChanged.
    /// </summary>
    /// <remarks>
    /// D�cr�mente le compteur d'embo�tements 
    /// </remarks>
    public void MvcLockOff() {
      mvcLocker -- ;
    }
  
    /// <summary>
    /// Permet de savoir si le d�clenchement de l'�v�nement SelectedIndexChanged est inhib�.
    /// </summary>
    /// <value>Retourne true si le compteur de verrouillage est dif�rent de 0, false sinon.</value>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public bool MvcIsLocked {
      get { return mvcLocker != 0 ; }
    }
  
    /// <summary>
    /// Red�finition de la m�thode prot�g�e h�rit�e de ListBox.
    /// </summary>
    /// <remarks>
    /// Filtrage du d�clenchement de l'�v�nement selon la valeur du compteur de
    /// verrouillage #mvcLocker. 
    ///
    /// Si le verrou n'est pas arm� (mvcLocker == 0),
    /// la m�thode se borne � appeler la m�thode h�rit�e (ce qui aura pour effet de
    /// d�clencher les event handlers associ�s � l'�v�nement) ; si le verrou est arm�
    /// (mvcLocker != 0), la m�thode ne fait rien, de sorte que les event handlers associ�s
    /// � l'�v�nement ne seront pas d�clench�s. 
    /// </remarks>
    /// <param name="e">Descripteur de l'�v�nement</param>
    protected  override void OnSelectedIndexChanged(EventArgs e) {
      if (mvcLocker == 0) 
        base.OnSelectedIndexChanged(e) ;
    }

    /// <summary>
    /// Red�finition de la m�thode prot�g�e h�rit�e de ListBox.
    /// </summary>
    /// <remarks>
    /// Filtrage du d�clenchement de l'�v�nement selon la valeur du compteur de
    /// verrouillage #mvcLocker. 
    ///
    /// Si le verrou n'est pas arm� (mvcLocker == 0),
    /// la m�thode se borne � appeler la m�thode h�rit�e (ce qui aura pour effet de
    /// d�clencher les event handlers associ�s � l'�v�nement) ; si le verrou est arm�
    /// (mvcLocker != 0), la m�thode ne fait rien, de sorte que les event handlers associ�s
    /// � l'�v�nement ne seront pas d�clench�s. 
    /// </remarks>
    /// <param name="e">Descripteur de l'�v�nement</param>
    protected  override void OnSelectedValueChanged(EventArgs e) {
      if (mvcLocker == 0) 
        base.OnSelectedValueChanged(e) ;
    }

    /// <summary>
    /// Ajoute un item � la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.Add(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item � ajouter � la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    /// <returns>l'index o� l'item a �t� ins�r�</returns>
    public int MvcAdd(object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { return Items.Add( item ) ; } 
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime un item de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.RemoveAt(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item � retirer de la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcRemoveAt(int index, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.RemoveAt( index ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime un item de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.Remove(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item � retirer de la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcRemove(object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Remove( item ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Supprime tous les items de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.Clear(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcClear(bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Clear() ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Ins�re un item dans la bo�te liste. 
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.Insert(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index d'insertion de l'item</param>
    /// <param name="item">item � ins�rer dans la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcInsert(int index, object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.Insert( index, item ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Modifie un item de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.set_Item(...) 
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item � modifier</param>
    /// <param name="item">nouvel item de remplacement</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetItem(int index, object item, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items[ index ] = item ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// Ajoute � la bo�te liste les �l�ments d'un tableau.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme Items.AddRange(...)
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChange</param>
    public void MvcAddRange(object[] items, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { Items.AddRange( items ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// D�termine tous les items de la bo�te liste et tente de restaurer l'�l�ment s�lectionn�. 
    /// </summary>
    /// <remarks>
    /// Cette m�thode utilise la m�thode SetRange du composant ListBoxEnh qui encha�ne 
    /// (1) le nettoyage de la liste (via Clear), (2) l'adjonction des �l�ments transmis via 
    /// le tableau items, et (3) tente de res�lectionner l'�l�ment ant�rieurement s�lectionn�. 
    /// <br/>
    /// Dans cette extension l�g�re de ListBox, la tentative de re-s�lection ne porte que sur un
    /// �l�ment au plus (les s�lections mutltiples ne sont pas prises en charge. 
    /// <br/>
    /// Lorsque asView est true, l'�v�nement SelectedIndexChanged  est inhib� de mani�re � permettre 
    /// une utilisation sobre du composant dans  un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="reselect">si true, tente de restaurer la s�lection</param>
    /// <param name="asView">inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetRange(object[] items, bool reselect, bool asView) {
      if (asView) mvcLocker ++ ;
      try     { SetRange( items, reselect ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// D�termine tous les items de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme la m�thode SetRange h�rit�e de ListBoxEnh,
    /// sans tentative de res�lection, quand asView est false. Lorsque asView est true, 
    /// l'�v�nement SelectedIndexChanged  est inhib� de mani�re � permettre une utilisation 
    /// sobre du composant dans  un contexte mvc.
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetRange(object[] items, bool asView) {
      MvcSetRange( items, false, asView ) ;
    }
  
    /// <summary>
    /// S�lectionne ou d�s�lectionne un item de la bo�te liste.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme SetSelected(...)
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item dont on veut modifier la s�lection</param>
    /// <param name="value">si true, s�lctionner, si false d�s�lectionner</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetSelected( int index, bool value, bool asView ) {
      if (asView) mvcLocker ++ ;
      try { SetSelected( index, value ) ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// D�termine l'index de l'�l�ment s�lectionn�.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme set_SelectedIndex(...)
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="index">index de l'item � s�lectionner, d�s�lectionner tout si -1</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetSelectedIndex( int index, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedIndex = index ; }
      finally { if (asView) mvcLocker -- ; }
    }
  
    /// <summary>
    /// D�termine l'item s�lectionn�.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme l'accesseur set de la propri�t� SelectedItem
    /// si asView est false. Lorsque asView est true, l'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
    /// un contexte mvc.
    /// </remarks>
    /// <param name="item">item � s�lectionner, d�s�lectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetSelectedItem( object item, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedItem = item ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// D�termine l'item s�lectionn� en tant que cha�ne de caract�res.
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme l'accesseur set de la propri�t� SelectedString
    /// h�rit�e du composant ListBoxEnh si asView est false. Lorsque asView est true, l'�v�nement 
    /// SelectedIndexChanged est inhib� de mani�re � permettre une utilisation sobre du composant 
    /// dans un contexte mvc.
    /// </remarks>
    /// <param name="item">item � s�lectionner, d�s�lectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetSelectedString( string item, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedString = item ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// D�termine l'item s�lectionn� selon la valeur (MemberValue).
    /// </summary>
    /// <remarks>
    /// Cette m�thode se comporte exactement comme l'accesseur set de la propri�t� SelectedValue
    /// h�rit�e du composant ListBoxEnh si asView est false. Lorsque asView est true, l'�v�nement 
    /// SelectedIndexChanged est inhib� de mani�re � permettre une utilisation sobre du composant 
    /// dans un contexte mvc.
    /// </remarks>
    /// <param name="value">item � s�lectionner, d�s�lectionner tout si null</param>
    /// <param name="asView">si true, inhibe l'�v�nement SelectedIndexChanged</param>
    public void MvcSetSelectedValue( object value, bool asView ) {
      if (asView) mvcLocker ++ ;
      try     { SelectedValue = value ; }
      finally { if (asView) mvcLocker -- ; }
    }

    /// <summary>
    /// Acc�s � l'index de l'�l�ment s�lectionn� en tant que vue.
    /// </summary>
    /// <value>
    /// En lecture, cette propri�t� se comporte exactement comme SelectedInex.
    /// En �criture, cette propi�t� se comporte exactement comme MvcSetSelectedIndex(...)
    /// avec asView � true. L'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
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
    /// Acc�der � ou d�terminer l'item s�lectionn� en tant que r�f�rence de type object.
    /// </summary>
    /// <value>
    /// En lecture, cette propri�t� se comporte exactement comme la propri�t� SelectedItem
    /// h�rit�e de ListBoxEnh (qui red�clare la propri�t� SelectedItem de ListBox).
    /// En �criture, cette propri�t� se comporte exactement comme MvcSetSelectedItem(...)
    /// avec asView � true. L'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
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
    /// Acc�der � ou d�terminer l'item s�lectionn� en tant que cha�ne de caract�res.
    /// </summary>
    /// <value>
    /// En lecture, cette propri�t� se comporte exactement comme la propri�t� SelectedString
    /// h�rit�e de ListBoxEnh.
    /// En �criture, cette propri�t� se comporte exactement comme MvcSetSelectedString(...)
    /// avec asView � true. L'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
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
    /// Acc�der � ou d�terminer l'item s�lectionn� selon la valeur (MemberValue).
    /// </summary>
    /// <value>
    /// En lecture, cette propri�t� se comporte exactement comme la propri�t� SelectedValue.
    /// En �criture, cette propri�t� se comporte exactement comme MvcSetSelectedValue(...)
    /// avec asView � true. L'�v�nement SelectedIndexChanged 
    /// est inhib� de mani�re � permettre une utilisation sobre du composant dans 
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
    /// <param name="disposing">true si les ressources manag�es doivent �tre lib�r�es</param>
    protected override void Dispose( bool disposing ) {
      if( disposing )	{
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

    #region Code g�n�r� par le Concepteur de composants
    /// <summary>
    /// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette m�thode avec l'�diteur de code.
    /// </summary>
    private void InitializeComponent() {
      components = new System.ComponentModel.Container();
    }
    #endregion
  }

}