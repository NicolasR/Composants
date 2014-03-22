/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System.ComponentModel;

namespace Psl.Controls {

	/// <summary>
	/// Extension du composant ListBox apportant diverses facilit�s et contournant quelques difficult�s.
	/// </summary>
  [
  Description( "Extension du composant ListBox apportant diverses facilit�s" )
  ]
	public class ListBoxEnh : System.Windows.Forms.ListBox {

		/// <summary>
		/// Variable n�cessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;

    /// <summary>
    /// Constructeur pour insertion dans un conteneur.
    /// </summary>
    /// <param name="container">acc�s au conteneur</param>
		public ListBoxEnh(System.ComponentModel.IContainer container) {
			container.Add(this);
			InitializeComponent();
		}

    /// <summary>
    /// Constructeur sans insertion dans un conteneur.
    /// </summary>
		public ListBoxEnh() {
			InitializeComponent();
		}

    /// <summary>
    /// Acc�der � ou d�terminer l'�l�ment s�lectionn� via une r�f�rence
    /// </summary>
    /// <remarks>
    /// <p>Cette propri�t� est une red�claration de la propri�t� SelectedItem du composant ListBox
    /// destin�e � contourner une difficult� (une bogue ?) du composant ListBox dans le 
    /// contexte d'un r�affichage centralis� sur l'�v�nement Application.Idle.</p>
    /// 
    /// <p>La consultation de la propri�t� SelectedItem originale provoque une exception lors d'une s�lection par la
    /// souris apr�s suppression d'un ou de plusieurs �l�ments.</p>
    /// </remarks>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public new object SelectedItem {                                           // <wao SelectedItem.begin>
      get { return SelectedIndex == -1 ? null : Items[ SelectedIndex ] ; }
      set { base.SelectedItem = value ; }
    }                                                                          // <wao SelectedItem.end>

    /// <summary>
    /// Acc�der � ou d�terminer l'�l�ment s�lectionn� comme une cha�ne de caract�res.
    /// </summary>
    /// <remarks>
    /// <p>Cette propri�t� est destin�e � faciliter l'acc�s et la d�termination de l'�l�ment s�lectionn�
    /// lorsqu'on op�re sur des �l�ments qui sont des cha�nes de caract�res.</p>
    /// 
    /// <p>(1) cette propri�t� permet d'op�rer directement sur des cha�nes de caract�res, sans 
    /// avoir � expliciter de transtypage ou de conversions (puisque le composant ListBox op�re sur 
    /// des �l�ments de type object).</p>
    /// 
    /// <p>(2) en �criture, cette propri�t� filtre les r�f�rence null � les cha�nes vides de mani�re � ne
    /// jamais introduire de r�f�rence null ou de cha�ne vide dans la bo�te liste : affecter une 
    /// r�f�rence null ou une cha�ne vide � cette propri�t� revient simplement � supprimer la s�lection.</p>
    /// 
    /// <p>(3) en �criture toujours, l'affectation d'une cha�ne introuvable dans la bo�te liste a pour effet
    /// de supprimer la s�lection (et non de la laisser en l'�tat comme targetAsControl'est le cas pour SelectedItem).</p>
    /// 
    /// <p>(4) en lecture, cette propri�t� ne retourne jamais null (au pire une cha�ne vide), et retourne
    /// comme cha�ne la cha�ne obtenue en appliquant la m�thode ToString � l'�l�ment (cette propri�t�
    /// ne d�clenche donc pas d'exceptions li�es au cast m�me si les �l�ments de la bo�te liste ne
    /// sont pas du type string).</p>  
    /// </remarks>
    [
      Browsable(false),
      DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
    ]
    public string SelectedString {                                            // <wao SelectedString.begin>
      get { 
        return 
          SelectedIndex == -1 || Items[ SelectedIndex ] == null 
          ? string.Empty 
          : Items[ SelectedIndex ].ToString() ; 
      }
      set { 
        SelectedIndex = 
          value == null || value == string.Empty  
          ? -1 
          : FindStringExact( value ) ; 
      }
    }                                                                          // <wao SelectedString.end>

    /// <summary>
    /// D�termine tous les items de la bo�te liste et tente de restaurer l'�l�ment s�lectionn�. 
    /// </summary>
    /// <remarks>
    /// Cette m�thode est un raccourci pratique qui encha�ne (1) le nettoyage de la liste (via Clear),
    /// (2) l'adjonction des �l�ments transmis via le tableau items, et (3) tente de res�lectionner
    /// l'�l�ment ant�rieurement s�lectionn�. 
    /// <br/>
    /// Dans cette extension l�g�re de ListBox, la tentative de re-s�lection ne porte que sur un
    /// �l�ment au plus (les s�lections mutltiples ne sont pas prises en charge. 
    /// <br/>
    /// L'ensemble de l'op�ration est inclus dans un verrouillage BeginUpdate... EndUpdate pour
    /// optimiser les r�affichages de la bo�te. 
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="reselect">si true, tente de restaurer la s�lection</param>
    public void SetRange(object[] items, bool reselect) {                      // <wao SetRange.begin>
      string selection = reselect ? SelectedString : null ;

      BeginUpdate() ;
      try {
        Items.Clear() ;
        Items.AddRange( items ) ; 
        SelectedString = selection ;
      } finally { EndUpdate() ; }
    }                                                                          // <wao SetRange.end>
  
		#region Code g�n�r� par le Concepteur de composants

    /// <summary> 
    /// Nettoyage des ressources utilis�es.
    /// </summary>
    /// <param name="disposing">true si les ressources manag�es doivent �tre lib�r�es</param>
    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

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
