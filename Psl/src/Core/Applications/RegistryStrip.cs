/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 26 02 2007 : version aihm 2006-2007 pour net 2.0
 * 09 05 2007 : correction de bogues dans la compression des séparateurs
 * 10 02 2008 : méthode MergeTools réduite (utilisation de ToolStripPanelEnh)
 * 29 10 2009 : adjonction des méthodes ManageSeparators pour la gestion dynamique des séparateurs
 */                                                                            // <wao never.end>
using Psl.Controls;
using System.Windows.Forms;

namespace Psl.Applications {

  // Boîte à outils pour les immersions de menus et de barres d'outils
  public partial class Registry {

    #region Compression des séparateurs

    /// <summary>
    /// Recherche une suite de séparateurs consécutifs
    /// </summary>
    /// <param name="items">collection d'éléments de menus où s'effectue la recherche</param>
    /// <param name="from">index du premier élément à considérer</param>
    /// <param name="start">index du premier séparateur de la suite</param>
    /// <param name="end">index du dernier séparateur de la suite</param>
    /// <returns>true si au moins un séparateur a été trouvé</returns>
    private static bool DoHasSeparators( ToolStripItemCollection items, int from, ref int start, ref int end ) {
      start = from;
      end = start - 1;
      bool found = false;

      // rechercher le premier séparateur
      for (int ix = from ; ix < items.Count ; ix++) {
        if (!(items[ ix ] is ToolStripSeparator)) continue;
        start = ix;
        found = true;
        break;
      }

      // pas de séparateur trouvé
      if (!found) return false;

      // rechercher le dernier séparateur de la suite
      end = start;
      for (int ix = start + 1 ; ix < items.Count ; ix++) {
        if (!(items[ ix ] is ToolStripSeparator)) break;
        end = ix;
      }

      return true;
    }

    /// <summary>
    /// Recherche une suite d'items consécutifs (autres que des séparateurs) et, si oui,
    /// s'il y en a au moins un qui est visible
    /// </summary>
    /// <remarks>
    /// La propriété Visible accessible en mode conception se traduit par la propriété Available
    /// à l'exécution, la propriété Visible, à l'exécution, traduit la visibilité actuelle de l'élément
    /// si le menu est déplié.
    /// </remarks>
    /// <param name="items">collection d'éléments de menus où s'effectue la recherche</param>
    /// <param name="from">index du premier élément à considérer</param>
    /// <param name="start">index du premier itemReport de la suite</param>
    /// <param name="end">index du dernier itemReport de la suite</param>
    /// <param name="hasVisible">true si au moins un des éléments est visible</param>
    /// <returns>true si au moins un itemReport a été trouvé</returns>
    private static bool DoHasItems( ToolStripItemCollection items, int from, ref int start, ref int end, ref bool hasVisible ) {
      start = from;
      end = start - 1;
      hasVisible = false;

      // recherche du dernier élément de la suite
      for (int ix = from ; ix < items.Count ; ix++) {
        ToolStripItem item = items[ ix ];
        if (item is ToolStripSeparator) break ;
        end = ix;
        hasVisible = hasVisible || item.Available;
      }

      return end >= start;
    }

    /// <summary>
    /// Détermine si une collection commence par une suite d'items autres que des séparateurs,
    /// et si, au sein de cette suite, il y a au moins un itemReport qui est visible
    /// </summary>
    /// <param name="items">collection d'éléments de menus où s'effectue la recherche</param>
    /// <returns>true si la collection commence par une suite d'items (non séparateurs) comportant</returns>
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
    /// Détermine la valeur de la propriété Available pour une suite de séparateurs
    /// </summary>
    /// <remarks>
    /// Tous les séparateurs de la suite de séparateurs déterminée par start..end
    /// sont rendus invisbles sauf le dernier pour lequel la propriété Visible est
    /// affectée à la valeur de l'argument show
    /// </remarks>
    /// <param name="items">collection d'éléments de menus</param>
    /// <param name="start">index du premier séparateur de la suite</param>
    /// <param name="end">index du dernier spérateur de la suite</param>
    /// <param name="show">true si un séparateur doit être rendu visible, false sinon</param>
    private static void DoShowSeparators( ToolStripItemCollection items, int start, int end, bool show ) {

      // rendre invisibles les n-1 premiers spérateurs de la suite
      for (int ix = start ; ix < end ; ix++)
        items[ ix ].Available = false;

      // déterminer la visibilité du dernier élément de la suite
      items[ end ].Available = show;
    }

    /// <summary>
    /// Gère l'état de visibilité des séparateurs de la collection items
    /// </summary>
    /// <remarks>
    /// Au plus un séparateur entre deux plages d'items dont un au moins est visible.
    /// Aucun séparateur visible avant le premier items visible ou après le dernier itemReport visible
    /// </remarks>
    /// <param name="items">collection d'éléments de menus à traiter</param>
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

        // recherche de la prochaine suite de séparateurs
        hasSeps = DoHasSeparators( items, endItems + 1, ref startSeps, ref endSeps );
        if (!hasSeps) return;

        // recherche de la prochaine suite d'éléments autres que des séparateurs
        hasItems = DoHasItems( items, endSeps + 1, ref startItems, ref endItems, ref hasVisible );

        // déterminer la visibilité de la suite de séparateurs
        bool showSep = hasVisible && previousVisible && hasItems;
        DoShowSeparators( items, startSeps, endSeps, showSep );

        // mémoriser le fait qu'au moins un itemReport visible a déjà été rencontré
        previousVisible = previousVisible || hasVisible;
      }
    }

    #endregion

    #region Immersion des menus

    /// <summary>
    /// Obtenir l'index dans la collection source de l'itemReport à mettre en correspondance avec l'itemReport source
    /// </summary>
    /// <param name="target">collection des items cible</param>
    /// <param name="sourceItem">itemReport source à mettre en correspondance</param>
    /// <returns>index de l'itemReport trouvé dans la collection cible ou -1</returns>
    private static int IndexOfMatch( ToolStripItemCollection target, ToolStripItem sourceItem ) {

      // cas des items MatchOnly : commencer par rechercher la correspondance des propriétés Text
      if (sourceItem.MergeAction == MergeAction.MatchOnly)
        for (int ix = 0 ; ix < target.Count ; ix++)
          if (target[ ix ].Text == sourceItem.Text)
            return ix;

      // autres cas : rechercher la correspondance sur les propriétés MergeIndex
      for (int ix = 0 ; ix < target.Count ; ix++)
        if (target[ ix ].MergeIndex == sourceItem.MergeIndex)
          return ix;

      // aucune correspondance trouvée
      return -1;
    }

    /// <summary>
    /// Détermine le point d'insertion de sourceMergeIndex sur l'ensemble de la collection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Cette méthode concerne le cas où aucun match n'a préalablement été trouvé. 
    /// La recherche de l'ancre d'insertion s'effectue sur l'ensemble de la collection.
    /// </para>
    /// <para>
    /// Le point d'insertion est soit la fin de la collection, soit l'index qui suit une suite
    /// d'items dont MergeIndex a même valeur que sourceMergeIndex
    /// </para>
    /// </remarks>
    /// <param name="target"></param>
    /// <param name="sourceMergeIndex"></param>
    /// <returns></returns>
    private static int IndexOfAnchor( ToolStripItemCollection target, int sourceMergeIndex ) {

      // source MergeIndex = -1 : simple adjonction en fin de collection
      if (sourceMergeIndex == -1) return target.Count;

      // rechercher le premier itemReport dont la propriété MergeIndex est supérieure au source MergeIndex
      for (int ix = 0 ; ix < target.Count ; ix++)
        if (target[ ix ].MergeIndex > sourceMergeIndex) return ix;

      // ancre introuvable : insertion en fin de collection
      return target.Count;
    }

    /// <summary>
    /// Déterminer l'index d'insertion à partir de l'index de correspondance
    /// </summary>
    /// <param name="target">cible de l'insertion</param>
    /// <param name="indexMatch">index de correspondance ou -1</param>
    /// <param name="sourceMergeIndex">index d'insertion dans la source</param>
    /// <returns>index d'insertion</returns>
    private static int IndexOfAnchor( ToolStripItemCollection target, int indexMatch, int sourceMergeIndex ) {

      // aucun match péalable trouvé : rechercher sur l'ensemble de la collection
      if (indexMatch == -1) return IndexOfAnchor( target, sourceMergeIndex );

      // aucune indication d'index d'insertion dans l'élément source : ajouter en fin de collection
      if (sourceMergeIndex == -1) return target.Count;

      // index MergeIndex associé au match déjà trouvé
      int baseIndex = target[ indexMatch ].MergeIndex;

      // recherche le point d'insertion en ignorant les éléments dont le MergeIndex est -1
      for (int ix = indexMatch + 1 ; ix < target.Count ; ix++) {
        int targetMergeIndex = target[ ix ].MergeIndex;
        if (targetMergeIndex != -1 && targetMergeIndex > baseIndex) return ix;
      }

      // aucun index d'insertion trouvé : ajouter en fin de collection
      return target.Count;
    }

    /// <summary>
    /// Immerge l'itemReport dans la collection cible source
    /// </summary>
    /// <param name="target">collection cible de l'immersion</param>
    /// <param name="sourceItem">élément à immerger dans la collection cible</param>
    /// <param name="kept">true si l'élément à immerger a été retenu</param>
    private static void DoMergeItem( ToolStripItemCollection target, ToolStripItem sourceItem, out bool kept ) {
      int targetMatch;  // -1 ou index de l'itemReport cible mis en correspondance avec l'itemReport source
      int targetAnchor; // -1 ou index d'insertion éventuel dans la collection cible

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
    /// Méthode agissant de manière récursive lorsqu'elle rencontre des noeuds à immerger.
    /// </para>
    /// </remarks>
    /// <param name="source">collection cible de l'immersion</param>
    /// <param name="target">collection source de l'immersion</param>
    private static void DoMergeItems( ToolStripItemCollection target, ToolStripItemCollection source ) {
      int sourceIndex = 0; // index de parcours de la collection source
      bool sourceKept;     // true si l'itemReport source a été laissé dans sa collection d'origine

      // immerger les items de la collection source
      while (sourceIndex < source.Count) {
        DoMergeItem( target, source[ sourceIndex ], out sourceKept );
        if (sourceKept) sourceIndex++;
      }

      // gérer la compression des séparateurs
      DoManageSeparators( target );
    }

    /// <summary>
    /// Immersion des menus de type <see cref="MenuStrip"/>.
    /// </summary>
    /// <param name="host">menu hôte cible de l'immersion</param>
    /// <param name="source">menu source de l'immersion</param>
    public static void MergeMenu( MenuStrip host, MenuStrip source ) {
      DoMergeItems( host.Items, source.Items );
    }

    /// <summary>
    /// Immersion des menus contextuels de type <see cref="ContextMenuStrip"/>.
    /// </summary>
    /// <param name="host">menu hôte cible de l'immersion</param>
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
    /// Utiliser un panneau <see cref="ToolStripPanelEnh"/> pour bénéficier du placement amélioré
    /// et des barres d'outils à éléments étirables.
    /// </remarks>
    /// <param name="host">panneau hôte</param>
    /// <param name="tools">barre d'outils à immerger</param>
    public static void MergeTools( ToolStripPanel host, ToolStrip tools ) {
      ToolStripPanelEnh hostEnh = host as ToolStripPanelEnh;
      if ( hostEnh == null )
        host.Controls.Add( tools );
      else
        hostEnh.Merge( tools );
    }

    #endregion

    #region Gestion dynamique des séparateurs

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
    /// Gestion dynamique des séparateurs pour les barres de menu.
    /// </summary>
    /// <remarks>
    /// Cette méthode est en principe à appeler dans un gestionnaire 
    /// associé à l'événement <see cref="MenuStrip.MenuActivate"/>.
    /// </remarks>
    /// <param name="menu">barre de menu à mettre à jour</param>
    public static void ManageSeparators( MenuStrip menu ) {
      ManageSeparators( menu.Items );
    }

    /// <summary>
    /// Gestion dynamique des séparateurs pour les menus contextuels.
    /// </summary>
    /// <remarks>
    /// Cette méthode est en principe à appeler dans un gestionnaire 
    /// associé à l'événement <see cref="ToolStripDropDown.Opening"/>.
    /// </remarks>
    /// <param name="menu">menu contextuel à mettre à jour</param>
    public static void ManageSeparators( ContextMenuStrip menu ) {
      ManageSeparators( menu.Items );
    }

    #endregion
  }
}
