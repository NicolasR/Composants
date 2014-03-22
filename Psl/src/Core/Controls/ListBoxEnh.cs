/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 */                                                                            // <wao never.end>
using System.ComponentModel;

namespace Psl.Controls {

	/// <summary>
	/// Extension du composant ListBox apportant diverses facilités et contournant quelques difficultés.
	/// </summary>
  [
  Description( "Extension du composant ListBox apportant diverses facilités" )
  ]
	public class ListBoxEnh : System.Windows.Forms.ListBox {

		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.Container components = null;

    /// <summary>
    /// Constructeur pour insertion dans un conteneur.
    /// </summary>
    /// <param name="container">accès au conteneur</param>
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
    /// Accéder à ou déterminer l'élément sélectionné via une référence
    /// </summary>
    /// <remarks>
    /// <p>Cette propriété est une redéclaration de la propriété SelectedItem du composant ListBox
    /// destinée à contourner une difficulté (une bogue ?) du composant ListBox dans le 
    /// contexte d'un réaffichage centralisé sur l'événement Application.Idle.</p>
    /// 
    /// <p>La consultation de la propriété SelectedItem originale provoque une exception lors d'une sélection par la
    /// souris après suppression d'un ou de plusieurs éléments.</p>
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
    /// Accéder à ou déterminer l'élément sélectionné comme une chaîne de caractères.
    /// </summary>
    /// <remarks>
    /// <p>Cette propriété est destinée à faciliter l'accès et la détermination de l'élément sélectionné
    /// lorsqu'on opère sur des éléments qui sont des chaînes de caractères.</p>
    /// 
    /// <p>(1) cette propriété permet d'opérer directement sur des chaînes de caractères, sans 
    /// avoir à expliciter de transtypage ou de conversions (puisque le composant ListBox opère sur 
    /// des éléments de type object).</p>
    /// 
    /// <p>(2) en écriture, cette propriété filtre les référence null à les chaînes vides de manière à ne
    /// jamais introduire de référence null ou de chaîne vide dans la boîte liste : affecter une 
    /// référence null ou une chaîne vide à cette propriété revient simplement à supprimer la sélection.</p>
    /// 
    /// <p>(3) en écriture toujours, l'affectation d'une chaîne introuvable dans la boîte liste a pour effet
    /// de supprimer la sélection (et non de la laisser en l'état comme targetAsControl'est le cas pour SelectedItem).</p>
    /// 
    /// <p>(4) en lecture, cette propriété ne retourne jamais null (au pire une chaîne vide), et retourne
    /// comme chaîne la chaîne obtenue en appliquant la méthode ToString à l'élément (cette propriété
    /// ne déclenche donc pas d'exceptions liées au cast même si les éléments de la boîte liste ne
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
    /// Détermine tous les items de la boîte liste et tente de restaurer l'élément sélectionné. 
    /// </summary>
    /// <remarks>
    /// Cette méthode est un raccourci pratique qui enchaîne (1) le nettoyage de la liste (via Clear),
    /// (2) l'adjonction des éléments transmis via le tableau items, et (3) tente de resélectionner
    /// l'élément antérieurement sélectionné. 
    /// <br/>
    /// Dans cette extension légère de ListBox, la tentative de re-sélection ne porte que sur un
    /// élément au plus (les sélections mutltiples ne sont pas prises en charge. 
    /// <br/>
    /// L'ensemble de l'opération est inclus dans un verrouillage BeginUpdate... EndUpdate pour
    /// optimiser les réaffichages de la boîte. 
    /// </remarks>
    /// <param name="items">tableau des nouveaux items de la liste</param>
    /// <param name="reselect">si true, tente de restaurer la sélection</param>
    public void SetRange(object[] items, bool reselect) {                      // <wao SetRange.begin>
      string selection = reselect ? SelectedString : null ;

      BeginUpdate() ;
      try {
        Items.Clear() ;
        Items.AddRange( items ) ; 
        SelectedString = selection ;
      } finally { EndUpdate() ; }
    }                                                                          // <wao SetRange.end>
  
		#region Code généré par le Concepteur de composants

    /// <summary> 
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être libérées</param>
    protected override void Dispose( bool disposing ) {
      if( disposing ) {
        if(components != null) {
          components.Dispose();
        }
      }
      base.Dispose( disposing );
    }

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
