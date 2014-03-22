/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 05 2007 : correction de bogues dans la compression des s�parateurs
 * 10 02 2008 : m�thode MergeTools r�duite (utilisation de ToolStripPanelEnh)
 * 29 10 2009 : adjonction des m�thodes ManageSeparators pour la gestion dynamique des s�parateurs
 */                                                                            // <wao never.end>
using Psl.Controls;
using System.Windows.Forms;

namespace Psl.Applications {

  // Bo�te � outils pour les immersions de menus et de barres d'outils
  public partial class Registry {

    #region Compression des s�parateurs

    /// <summary>
    /// Recherche une suite de s�parateurs cons�cutifs
    /// </summary>
    /// <param name="items">collection d'�l�ments de menus o� s'effectue la recherche</param>
    /// <param name="from">index du premier �l�ment � consid�rer</param>
    /// <param name="start">index du premier s�parateur de la suite</param>
    /// <param name="end">index du dernier s�parateur de la suite</param>
    /// <returns>true si au moins un s�parateur a �t� trouv�</returns>
    private static bool DoHasSeparators( ToolStripItemCollection items, int from, ref int start, ref int end ) {
      start = from;
      end = start - 1;
      bool found = false;

      // rechercher le premier s�parateur
      for (int ix = from ; ix < items.Count ; ix++) {
        if (!(items[ ix ] is ToolStripSeparator)) continue;
        start = ix;
        found = true;
        break;
      }

      // pas de s�parateur trouv�
      if (!found) return false;

      // rechercher le dernier s�parateur de la suite
      end = start;
      for (int ix = start + 1 ; ix < items.Count ; ix++) {
        if (!(items[ ix ] is ToolStripSeparator)) break;
        end = ix;
      }

      return true;
    }

    /// <summary>
    /// Recherche une suite d'items cons�cutifs (autres que des s�parateurs) et, si oui,
    /// s'il y en a au moins un qui est visible
    /// </summary>
    /// <remarks>
    /// La propri�t� Visible accessible en mode conception se traduit par la propri�t� Available
    /// � l'ex�cution, la propri�t� Visible, � l'ex�cution, traduit la visibilit� actuelle de l'�l�ment
    /// si le menu est d�pli�.
    /// </remarks>
    /// <param name="items">collection d'�l�ments de menus o� s'effectue la recherche</param>
    /// <param name="from">index du premier �l�ment � consid�rer</param>
    /// <param name="start">index du premier itemReport de la suite</param>
    /// <param name="end">index du dernier itemReport de la suite</param>
    /// <param name="hasVisible">true si au moins un des �l�ments est visible</param>
    /// <returns>true si au moins un itemReport a �t� trouv�</returns>
    private static bool DoHasItems( ToolStripItemCollection items, int from, ref int start, ref int end, ref bool hasVisible ) {
      start = from;
      end = start - 1;
      hasVisible = false;

      // recherche du dernier �l�ment de la suite
      for (int ix = from ; ix < items.Count ; ix++) {
        ToolStripItem item = items[ ix ];
        if (item is ToolStripSeparator) break ;
        end = ix;
        hasVisible = hasVisible || item.Available;
      }

      return end >= start;
    }

    /// <summary>
    /// D�termine si une collection commence par une suite d'items autres que des s�parateurs,
    /// et si, au sein de cette suite, il y a au moins un itemReport qui est visible
    /// </summary>
    /// <param name="items">collection d'�l�ments de menus o� s'effectue la recherche</param>
    /// <returns>true si la collection commence par une suite d'items (non s�parateurs) comportant</returns>
    private static bool DoGetVisibleHeader( ToolStripItemCollection items ) {
      bool result = false;
      int start = 0;
      int end = -1;

      if (items.Count == 0) return result ;
      if (items[ 0 ] is ToolStripSeparator) return result;

      DoHasItems( items, 0, ref start, ref end, ref result );
      return result;
    }

    /// <summary>
    /// D�termine la valeur de la propri�t� Available pour une suite de s�parateurs
    /// </summary>
    /// <remarks>
    /// Tous les s�parateurs de la suite de s�parateurs d�termin�e par start..end
    /// sont rendus invisbles sauf le dernier pour lequel la propri�t� Visible est
    /// affect�e � la valeur de l'argument show
    /// </remarks>
    /// <param name="items">collection d'�l�ments de menus</param>
    /// <param name="start">index du premier s�parateur de la suite</param>
    /// <param name="end">index du dernier sp�rateur de la suite</param>
    /// <param name="show">true si un s�parateur doit �tre rendu visible, false sinon</param>
    private static void DoShowSeparators( ToolStripItemCollection items, int start, int end, bool show ) {

      // rendre invisibles les n-1 premiers sp�rateurs de la suite
      for (int ix = start ; ix < end ; ix++)
        items[ ix ].Available = false;

      // d�terminer la visibilit� du dernier �l�ment de la suite
      items[ end ].Available = show;
    }

    /// <summary>
    /// G�re l'�tat de visibilit� des s�parateurs de la collection items
    /// </summary>
    /// <remarks>
    /// Au plus un s�parateur entre deux plages d'items dont un au moins est visible.
    /// Aucun s�parateur visible avant le premier items visible ou apr�s le dernier itemReport visible
    /// </remarks>
    /// <param name="items">collection d'�l�ments de menus � traiter</param>
    private static void DoManageSeparators( ToolStripItemCollection items ) {
      int startSeps = 0;
      int endSeps = -1;
      bool hasSeps = false;
      int startItems = 0;
      int endItems = -1;
      bool hasItems = false;
      bool hasVisible = false;
      bool previousVisible = DoGetVisibleHeader(items);

      while (true) {

        // recherche de la prochaine suite de s�parateurs
        hasSeps = DoHasSeparators( items, endItems + 1, ref startSeps, ref endSeps );
        if (!hasSeps) return;

        // recherche de la prochaine suite d'�l�ments autres que des s�parateurs
        hasItems = DoHasItems( items, endSeps + 1, ref startItems, ref endItems, ref hasVisible );

        // d�terminer la visibilit� de la suite de s�parateurs
        bool showSep = hasVisible && previousVisible && hasItems;
        DoShowSeparators( items, startSeps, endSeps, showSep );

        // m�moriser le fait qu'au moins un itemReport visible a d�j� �t� rencontr�
        previousVisible = previousVisible || hasVisible;
      }
    }

    #endregion

    #region Immersion des menus

    /// <summary>
    /// Obtenir l'index dans la collection source de l'itemReport � mettre en correspondance avec l'itemReport source
    /// </summary>
    /// <param name="target">collection des items cible</param>
    /// <param name="sourceItem">itemReport source � mettre en correspondance</param>
    /// <returns>index de l'itemReport trouv� dans la collection cible ou -1</returns>
    private static int IndexOfMatch( ToolStripItemCollection target, ToolStripItem sourceItem ) {

      // cas des items MatchOnly : commencer par rechercher la correspondance des propri�t�s Text
      if (sourceItem.MergeAction == MergeAction.MatchOnly)
        for (int ix = 0 ; ix < target.Count ; ix++)
          if (target[ ix ].Text == sourceItem.Text)
            return ix;

      // autres cas : rechercher la correspondance sur les propri�t�s MergeIndex
      for (int ix = 0 ; ix < target.Count ; ix++)
        if (target[ ix ].MergeIndex == sourceItem.MergeIndex)
          return ix;

      // aucune correspondance trouv�e
      return -1;
    }

    /// <summary>
    /// D�termine le point d'insertion de sourceMergeIndex sur l'ensemble de la collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette m�thode concerne le cas o� aucun match n'a pr�alablement �t� trouv�. 
    /// La recherche de l'ancre d'insertion s'effectue sur l'ensemble de la collection.
    /// </para>
    /// <para>
    /// Le point d'insertion est soit la fin de la collection, soit l'index qui suit une suite
    /// d'items dont MergeIndex a m�me valeur que sourceMergeIndex
    /// </para>
    /// </remarks>
    /// <param name="target"></param>
    /// <param name="sourceMergeIndex"></param>
    /// <returns></returns>
    private static int IndexOfAnchor( ToolStripItemCollection target, int sourceMergeIndex ) {

      // source MergeIndex = -1 : simple adjonction en fin de collection
      if (sourceMergeIndex == -1) return target.Count;

      // rechercher le premier itemReport dont la propri�t� MergeIndex est sup�rieure au source MergeIndex
      for (int ix = 0 ; ix < target.Count ; ix++)
        if (target[ ix ].MergeIndex > sourceMergeIndex) return ix;

      // ancre introuvable : insertion en fin de collection
      return target.Count;
    }

    /// <summary>
    /// D�terminer l'index d'insertion � partir de l'index de correspondance
    /// </summary>
    /// <param name="target">cible de l'insertion</param>
    /// <param name="indexMatch">index de correspondance ou -1</param>
    /// <param name="sourceMergeIndex">index d'insertion dans la source</param>
    /// <returns>index d'insertion</returns>
    private static int IndexOfAnchor( ToolStripItemCollection target, int indexMatch, int sourceMergeIndex ) {

      // aucun match p�alable trouv� : rechercher sur l'ensemble de la collection
      if (indexMatch == -1) return IndexOfAnchor( target, sourceMergeIndex );

      // aucune indication d'index d'insertion dans l'�l�ment source : ajouter en fin de collection
      if (sourceMergeIndex == -1) return target.Count;

      // index MergeIndex associ� au match d�j� trouv�
      int baseIndex = target[ indexMatch ].MergeIndex;

      // recherche le point d'insertion en ignorant les �l�ments dont le MergeIndex est -1
      for (int ix = indexMatch + 1 ; ix < target.Count ; ix++) {
        int targetMergeIndex = target[ ix ].MergeIndex;
        if (targetMergeIndex != -1 && targetMergeIndex > baseIndex) return ix;
      }

      // aucun index d'insertion trouv� : ajouter en fin de collection
      return target.Count;
    }

    /// <summary>
    /// Immerge l'itemReport dans la collection cible source
    /// </summary>
    /// <param name="target">collection cible de l'immersion</param>
    /// <param name="sourceItem">�l�ment � immerger dans la collection cible</param>
    /// <param name="kept">true si l'�l�ment � immerger a �t� retenu</param>
    private static void DoMergeItem( ToolStripItemCollection target, ToolStripItem sourceItem, out bool kept ) {
      int targetMatch;  // -1 ou index de l'itemReport cible mis en correspondance avec l'itemReport source
      int targetAnchor; // -1 ou index d'insertion �ventuel dans la collection cible

      // recherche d'un match
      kept = false;
      targetMatch = IndexOfMatch( target, sourceItem );

      switch (sourceItem.MergeAction) {

        // adjonction en fin de collection
        case MergeAction.Append:
          target.Add( sourceItem );
          break;

        // insertion
        case MergeAction.Insert:
          targetAnchor = IndexOfAnchor( target, targetMatch, sourceItem.MergeIndex );
          target.Insert( targetAnchor, sourceItem );
          break;

        // fusion
        case MergeAction.MatchOnly:
          if (targetMatch != -1 && target[ targetMatch ] is ToolStripMenuItem && sourceItem is ToolStripMenuItem) {
            kept = true;
            DoMergeItems( (target[ targetMatch ] as ToolStripMenuItem).DropDownItems, (sourceItem as ToolStripMenuItem).DropDownItems );
          }
          else {
            targetAnchor = IndexOfAnchor( target, targetMatch, sourceItem.MergeIndex );
            target.Insert( targetAnchor, sourceItem );
          }
          break;

        // suppression
        case MergeAction.Remove:
          kept = true;
          if (targetMatch == -1) break;
          target.RemoveAt( targetMatch );
          break;

        // remplacement
        case MergeAction.Replace:
          if (targetMatch == -1)
            target.Add( sourceItem );
          else {
            target.RemoveAt( targetMatch );
            target.Insert( targetMatch, sourceItem );
          }
          break;
      }
    }

    /// <summary>
    /// Immersion de la collection d'items source dans la collection d'items source
    /// </summary>
    /// <remarks>
    /// <para>
    /// M�thode agissant de mani�re r�cursive lorsqu'elle rencontre des noeuds � immerger.
    /// </para>
    /// </remarks>
    /// <param name="source">collection cible de l'immersion</param>
    /// <param name="target">collection source de l'immersion</param>
    private static void DoMergeItems( ToolStripItemCollection target, ToolStripItemCollection source ) {
      int sourceIndex = 0; // index de parcours de la collection source
      bool sourceKept;     // true si l'itemReport source a �t� laiss� dans sa collection d'origine

      // immerger les items de la collection source
      while (sourceIndex < source.Count) {
        DoMergeItem( target, source[ sourceIndex ], out sourceKept );
        if (sourceKept) sourceIndex++;
      }

      // g�rer la compression des s�parateurs
      DoManageSeparators( target );
    }

    /// <summary>
    /// Immersion des menus de type <see cref="MenuStrip"/>.
    /// </summary>
    /// <param name="host">menu h�te cible de l'immersion</param>
    /// <param name="source">menu source de l'immersion</param>
    public static void MergeMenu( MenuStrip host, MenuStrip source ) {
      DoMergeItems( host.Items, source.Items );
    }

    /// <summary>
    /// Immersion des menus contextuels de type <see cref="ContextMenuStrip"/>.
    /// </summary>
    /// <param name="host">menu h�te cible de l'immersion</param>
    /// <param name="source">menu source de l'immersion</param>
    public static void MergeMenu( ContextMenuStrip host, ContextMenuStrip source ) {
      DoMergeItems( host.Items, source.Items );
    }

    #endregion

    #region Immersion des barres d'outils

    /// <summary>
    /// Immersion des barres d'outils dans un <see cref="ToolStripPanel"/>
    /// </summary>
    /// <remarks>
    /// Utiliser un panneau <see cref="ToolStripPanelEnh"/> pour b�n�ficier du placement am�lior�
    /// et des barres d'outils � �l�ments �tirables.
    /// </remarks>
    /// <param name="host">panneau h�te</param>
    /// <param name="tools">barre d'outils � immerger</param>
    public static void MergeTools( ToolStripPanel host, ToolStrip tools ) {
      ToolStripPanelEnh hostEnh = host as ToolStripPanelEnh;
      if ( hostEnh == null )
        host.Controls.Add( tools );
      else
        hostEnh.Merge( tools );
    }

    #endregion

    #region Gestion dynamique des s�parateurs

    private static void ManageSeparators( ToolStripItem item ) {
      ToolStripMenuItem menuItem = item as ToolStripMenuItem ;
      if (menuItem == null) return ;
      DoManageSeparators( menuItem.DropDownItems );
    }

    private static void ManageSeparators( ToolStripItemCollection items ) {
      DoManageSeparators( items );

      foreach ( ToolStripItem item in items ) {
        if ( !item.Available ) continue;
        ToolStripDropDownItem droppable = item as ToolStripDropDownItem;
        if ( droppable == null ) continue;
        ManageSeparators( droppable.DropDownItems );
      }
    }

    /// <summary>
    /// Gestion dynamique des s�parateurs pour les barres de menu.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est en principe � appeler dans un gestionnaire 
    /// associ� � l'�v�nement <see cref="MenuStrip.MenuActivate"/>.
    /// </remarks>
    /// <param name="menu">barre de menu � mettre � jour</param>
    public static void ManageSeparators( MenuStrip menu ) {
      ManageSeparators( menu.Items );
    }

    /// <summary>
    /// Gestion dynamique des s�parateurs pour les menus contextuels.
    /// </summary>
    /// <remarks>
    /// Cette m�thode est en principe � appeler dans un gestionnaire 
    /// associ� � l'�v�nement <see cref="ToolStripDropDown.Opening"/>.
    /// </remarks>
    /// <param name="menu">menu contextuel � mettre � jour</param>
    public static void ManageSeparators( ContextMenuStrip menu ) {
      ManageSeparators( menu.Items );
    }

    #endregion
  }
}
