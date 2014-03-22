/*
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * A compléter :
 * - adapter la méthode DoStackTracePopulate pour les traces de piles en anglais
 * 
 * 12 11 2008 : version initiale
 * 15 11 2008 : adjonction du nom de l'application dans l'affichage des détails
 * 22 01 2009 : adaptation pour les conteneurs d'exceptions ExceptionContainer
 * 26 01 2010 : adjonction de la prise en charge du premier message des ExceptionContainer
 */

using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace Psl.Applications {

  /// <summary>
  /// Boîte de dialogue modale pour l'affichage détaillé des informations contenues dans un objet exception.
  /// </summary>
  /// <remarks>
  /// Cette boîte de dialogue prend en charge les conteneurs d'exceptions <see cref="ExceptionContainer"/>
  /// qui est utilisée en particulier dans le protocole des actions (classe <see cref="Psl.Actions.Action"/>) 
  /// qui est susceptible de mémoriser plusieurs objets exception au cours d'une action. 
  /// <br/>
  /// Cette boîte de dialogue traite tous les objets exceptions figurant dans la liste <see cref="Exception.Data"/> 
  /// des données additionnelles des exceptions ainsi que tous les objets exception figurant dans la liste
  /// <see cref="ExceptionContainer.ContainedExceptions"/> (classe <see cref="ExceptionContainer"/>) si l'exception
  /// à afficher est au moins du type <see cref="ExceptionContainer"/>, comme une liste (non emboîtée) d'exceptions 
  /// inner prenant place avant l'exceprion inner proprement dite (en principe une exception liées aux actions n'a 
  /// pas d'exceptions inner). 
  /// </remarks>
  public partial class ExceptionBox : Form {

    #region Membres de classe

    //
    // Membres de classe
    //

    /// <summary>
    /// Mémorisation de la dernière largeur connue pour la boîte (et largeur par défaut)
    /// </summary>
    private static int lastWidth = 600;

    /// <summary>
    /// mémorisation de la dernière hauteur de détails connue pour la boîte (et hauteur par défaut)
    /// </summary>
    private static int lastDetailsHeight = 400;

    /// <summary>
    /// Méthode unique de création et d'affichage de la boîte de dialogue
    /// </summary>
    /// <param name="owner">null ou fenêtre relativement à laquelle aficher le dialogue</param>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <param name="caption">intitulé à faire figurer dans la barre de titre</param>
    /// <param name="message">message additionnel à afficher avec le message d'exception</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="buttons">boutons à afficher dans la boîte de dialogue</param>
    /// <param name="icon">icône décorative à afficher dans la boîte de dialogue</param>
    /// <returns>le code de conclusion du dialogue</returns>
    protected static DialogResult DoShow( IWin32Window owner, bool detailed, string caption, string message, Exception exception, ExceptionBoxButton buttons, ExceptionBoxIcon icon ) {
      using ( ExceptionBox box = new ExceptionBox() ) {
        box.SetData( detailed, caption, message, exception, buttons, icon );
        return box.ShowDialog( owner );
      }
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception ) {
      return DoShow( null, true, "Exception", string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception, bool detailed ) {
      return DoShow( null, detailed, "Exception", string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception, string caption ) {
      return DoShow( null, false, caption, string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception, string caption, string message ) {
      return DoShow( null, false, caption, message, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <param name="buttons">détermine quels sont les boutons à afficher (Details et Ok par défaut)</param>
    /// <param name="icon">détermine l'icône à afficher (Error, par défaut)</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception, string caption, string message, ExceptionBoxButton buttons, ExceptionBoxIcon icon ) {
      return DoShow( null, false, caption, message, exception, buttons, icon );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <param name="buttons">détermine quels sont les boutons à afficher (Details et Ok par défaut)</param>
    /// <param name="icon">détermine l'icône à afficher (Error, par défaut)</param>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( Exception exception, string caption, string message, ExceptionBoxButton buttons, ExceptionBoxIcon icon, bool detailed ) {
      return DoShow( null, detailed, caption, message, exception, buttons, icon );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception ) {
      return DoShow( owner, true, "Exception", string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception, bool detailed ) {
      return DoShow( owner, detailed, "Exception", string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception, string caption ) {
      return DoShow( owner, false, caption, string.Empty, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception, string caption, string message ) {
      return DoShow( owner, false, caption, message, exception, ExceptionBoxButton.DetailsOK, ExceptionBoxIcon.Error );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <param name="buttons">détermine quels sont les boutons à afficher (Details et Ok par défaut)</param>
    /// <param name="icon">détermine l'icône à afficher (Error, par défaut)</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception, string caption, string message, ExceptionBoxButton buttons, ExceptionBoxIcon icon ) {
      return DoShow( owner, false, caption, message, exception, buttons, icon );
    }

    /// <summary>
    /// Réaliser le dialogue modal d'affichage d'une exception
    /// </summary>
    /// <param name="owner">fenêtre propriétaire du dialogue (si null, fenêtre active)</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <param name="buttons">détermine quels sont les boutons à afficher (Details et Ok par défaut)</param>
    /// <param name="icon">détermine l'icône à afficher (Error, par défaut)</param>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <returns>le code de conclusion du dialogue</returns>
    public static DialogResult Show( IWin32Window owner, Exception exception, string caption, string message, ExceptionBoxButton buttons, ExceptionBoxIcon icon, bool detailed ) {
      return DoShow( owner, detailed, caption, message, exception, buttons, icon );
    }

    #endregion

    #region Membres d'instance

    //
    // Champs
    //

    /// <summary>
    /// Indique si le mode d'affichage courant
    /// </summary>
    private bool detailsShowing = false;

    /// <summary>
    /// Objet exception à afficher
    /// </summary>
    private Exception exception = null ;

    //
    // Méthodes
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public ExceptionBox() {
      InitializeComponent();
    }

    /// <summary>
    /// Retourne le maximum de deux valeurs
    /// </summary>
    /// <param name="value1">première valeur</param>
    /// <param name="value2">seconde valeur</param>
    /// <returns>le maximum des deux valeurs</returns>
    private int Max( int value1, int value2 ) {
      return value1 > value2 ? value1 : value2;
    }

    private bool resizing = false ;

    /// <summary>
    /// Redimensionnement des éléments en fonction de la taille de la fenêtre
    /// </summary>
    private void DoResizeBox() {
      if ( resizing ) return;

      resizing = true;
      try {

        // reformater le message en fonction de la largeur
        lbMessage.MaximumSize = new Size( btOk.Left - 10 - lbMessage.Left, 0 );

        // recalculer la borne maximum Height du premier panneau en fonction des messages
        int messagesHeight = lbMessage.Height + lbMessageContainer.Height;
        firstPanel.MaximumSize = new Size(
          0,
          Max( firstPanel.MinimumSize.Height, (messagesHeight + 3 * lbMessage.Top)  )
          );

        // déterminer la taille du premier panneau
        firstPanel.ClientSize = new Size(
          Max( firstPanel.MinimumSize.Width, firstPanel.ClientSize.Width ),
          firstPanel.MaximumSize.Height
          );

        // caler le messageContainer dans le premier paneau
        lbMessageContainer.Top = lbMessage.Height + 2 * lbMessage.Top;

        // recalculer la dimension de la fenêtre
        ClientSize = new Size(
          Max( firstPanel.MinimumSize.Width, firstPanel.Width ),
          detailsShowing ? Max( ClientSize.Height, firstPanel.Height + centerPanel.MinimumSize.Height ) : firstPanel.Height
          );
      }
      finally { resizing = false; }

      // mémoriser les dernières dimensions caractéristiques connues
      ExceptionBox.lastWidth = Size.Width;
      if ( detailsShowing ) lastDetailsHeight = centerPanel.Height;
    }

    /// <summary>
    /// Bascule l'affichage des détails
    /// </summary>
    private void DoToggleDetails() {
      detailsShowing = !detailsShowing;
      if ( detailsShowing ) DoTreeNodePopulate();
      ClientSize = new Size( ClientSize.Width, firstPanel.Height + (detailsShowing ? ExceptionBox.lastDetailsHeight : 0) );
      DoResizeBox();
    }

    /// <summary>
    /// Peuple récursivement l'arbre des exceptions
    /// </summary>
    /// <remarks>
    /// Prend en charge l'affichage : <br/>
    /// 1) de tous les objets exception figurant la collection <see cref="Exception.Data"/> de l'exception; <br/>
    /// 2) de tous les objets exception figurant dans la collection <see cref="ExceptionContainer.ContainedExceptions"/>
    /// si l'objet exception à afficher est au moins du type <see cref="ExceptionContainer"/> (utilisée en particulier 
    /// par le protocole des actions, classe Action); <br/>
    /// 3) de l'exception <see cref="Exception.InnerException"/> s'il y en a une.
    /// </remarks>
    /// <param name="parentNodes">collection des noeuds enfant du noeud parent</param>
    /// <param name="exception">object exception à ajouter</param>
    private void DoTreeNodePopulate( TreeNodeCollection parentNodes, Exception exception ) {
      if ( exception == null ) return;

      // créer le noeud
      TreeNode node = parentNodes.Add( exception.Message );
      node.Tag = exception;

      // rechercher les objets exception dans la collection Data
      foreach ( object value in exception.Data.Values )
        if ( value is Exception )
          DoTreeNodePopulate( node.Nodes, value as Exception );

      // ajouter les objets exception de la collection ContainedException 
      ExceptionContainer container = exception as ExceptionContainer;
      if ( container != null )
        foreach ( Exception item in container.ContainedExceptions )
          DoTreeNodePopulate( node.Nodes, item );

      // ajouter l'exception InnerException s'il y en a une
      DoTreeNodePopulate( node.Nodes, exception.InnerException );
    }

    /// <summary>
    /// Méthode racine pour peupler l'arbre des exceptions
    /// </summary>
    private void DoTreeNodePopulate() {
      if ( tree.Nodes.Count > 0 ) return;

      application.Text = string.Format(
        "{0}  ({1})",
        Path.GetFileName( Application.ExecutablePath ),
        Path.GetDirectoryName( Application.ExecutablePath )
        );

      tree.BeginUpdate();
      try {
        DoTreeNodePopulate( tree.Nodes, exception );
        if ( tree.Nodes.Count == 0 ) return;
        tree.SelectedNode = tree.Nodes[ 0 ];
        tree.SelectedNode.EnsureVisible();
        tree.ExpandAll();
      }
      finally { tree.EndUpdate(); }
    }

    /// <summary>
    /// Ajoute une ligne à la boîte liste des détails
    /// </summary>
    /// <param name="key">clé de l'élément ou chaîne vide</param>
    /// <param name="value">valeur de l'élément</param>
    /// <param name="bold">détermine si l'item doit être affiché en gras</param>
    /// <param name="red">détermine si l'item doit être affiché en rouge</param>
    private void DoPropertiesAdd( string key, string value, bool bold, bool red ) {
      if ( value == string.Empty || value == null ) return;
      ListViewItem item = properties.Items.Add( key );      
      item.SubItems.Add( value );
      if ( bold ) item.Font = new Font( item.Font, FontStyle.Bold );
      if ( red ) item.ForeColor = Color.Red;
    }

    /// <summary>
    /// Eclate la chaîne en lignes et les ajouter à la boîte liste des détails
    /// </summary>
    /// <param name="key">clé de l'élément ou chaîne vide</param>
    /// <param name="value">valeur de l'élément</param>
    /// <param name="bold">détermine si l'item doit être affiché en gras</param>
    /// <param name="red">détermine si l'item doit être affiché en rouge</param>
    private void DoSplitAndAdd( string key, string value, bool bold, bool red ) {

      value = value.Replace( "\r\n", "!!!return!!!" );
      value = value.Replace( "\n\r", "!!!return!!!" );
      value = value.Replace( "\n"  , "!!!return!!!" );
      value = value.Replace( "\r"  , "!!!return!!!" );

      string[] parts = value.Split( new string[] { "!!!return!!!", }, StringSplitOptions.None );

      foreach ( string part in parts ) {
        string line = part.Trim();
        if ( line == string.Empty ) continue;
        DoPropertiesAdd( key, line, bold, red );
        key = string.Empty;
      }

      if ( key != string.Empty ) DoPropertiesAdd( key, "[aucun message]", bold, red );
    }

    /// <summary>
    /// Peuple la liste des propriétés de l'objet eception
    /// </summary>
    /// <param name="exception">objet exception source des informations</param>
    private void DoPropertiesPopulate( Exception exception ) {
      properties.BeginUpdate();
      try {

        // nettoyer la liste
        properties.Items.Clear();
        if ( exception == null ) return;

        // calculer les éléments qui dépendent de TargetSite !!! peut être null ou lever des exceptions !!!
        MethodBase targetSiteRef = null;
        string targetSiteMsg = string.Empty;
        try {
          targetSiteRef = exception.TargetSite;
          if ( targetSiteRef == null ) targetSiteMsg = "[non spécifié dans l'exception]";
        }
        catch ( Exception x ) { targetSiteMsg = "[" + x.Message + "]"; }

        // ajouter en premier les propriété de l'objet exception
        DoSplitAndAdd( "Message", exception.Message, true, true );
        DoPropertiesAdd( "Classe", exception.GetType().FullName, false, false );

        // ajouter ensuite les inforamtion sur la source de l'exception
        DoPropertiesAdd( string.Empty, " ", false, false );
        DoPropertiesAdd( "Source", exception.Source, false, false );
        DoPropertiesAdd( "Méthode", targetSiteRef == null ? targetSiteMsg : targetSiteRef.ReflectedType.FullName + "." + targetSiteRef.Name, false, targetSiteRef == null );
        DoPropertiesAdd( "Module", targetSiteRef == null ? targetSiteMsg : targetSiteRef.Module.FullyQualifiedName, false, targetSiteRef == null );
        DoPropertiesAdd( "Aide", exception.HelpLink, false, false );

        // ajouter enfin les propriétés additionnelles (en filtrant les objets exception)
        DoPropertiesAdd( string.Empty, " ", false, false );
        foreach ( string key in exception.Data.Keys ) {
          object value = exception.Data[ key ];
          if ( value is Exception ) continue;
          DoPropertiesAdd( key, value.ToString(), false, false );
        }

        // redimensionner automatiquement la colonne des valeurs
        columnValues.AutoResize( ColumnHeaderAutoResizeStyle.ColumnContent );
      }
      finally { properties.EndUpdate(); }
    }

    /// <summary>
    /// Crée un noeud pour l'affichage d'un élément de la trace de la pile
    /// </summary>
    /// <remarks>
    /// Filtre certains intitulés pour les afficher en rouge
    /// </remarks>
    /// <param name="text">libellé du noeud</param>
    /// <param name="color">couleur de la police pour le noeud</param>
    /// <returns>le noeud qui vient d'être créé</returns>
    private TreeNode DoStackTraceNodeCreate( string text, Color color  ) {
      TreeNode node = stack.Nodes.Add( text );
      if ( text.StartsWith( "Server stack trace" ) || text.StartsWith( "Exception rethrown at" ) )
        node.ForeColor = Color.Red;
      else
        node.ForeColor = color;
      return node;
    }

    /// <summary>
    /// Ajouter un élément de la trace de la pile
    /// </summary>
    /// <param name="data">tableau des différentes parties de l'élément</param>
    private void DoStackTraceAdd( params string[] data ) {
      if ( data.Length == 0 ) return;

      // créer le noeud pour l'élément de trace
      TreeNode node = DoStackTraceNodeCreate( data[ 0 ], data.Length == 1 ? Color.DimGray : stack.ForeColor );
      if ( data.Length == 1 ) return;

      // ajouter les parties de cet élément comme des sous-noeuds
      for (int ix = 1; ix < data.Length; ix++)
        node.Nodes.Add( data[ ix ] );
    }

    /// <summary>
    /// Peuple la vue en arbre avec les éléments de la trace de la pile
    /// </summary>
    /// <param name="exception">objet exception source de la trace de pile</param>
    private void DoStackTracePopulate( Exception exception ) {
      stack.BeginUpdate();
      try {

        // nettoyer l'affichage
        stack.Nodes.Clear();
        if ( exception == null ) return;

        // récupérer la chaîne donnant la trace de pile
        string stackTrace = exception.StackTrace;

        // cas d'une trace de pile vide
        if ( stackTrace == null || stackTrace == string.Empty ) {
          DoStackTraceAdd( "[pas de trace de pile dans l'objet exception]" );
          return;
        }

        // découper les éléments de la trace de pile
        string[] traces = stackTrace.Split( new string[] { "\r\n" }, StringSplitOptions.None );
        foreach ( string s in traces ) {

          // filtrer le début de la chaîne *** à transposer pour l'anglais ***
          string trace = s.Replace( "   à ", string.Empty ).Trim();
          if ( trace == string.Empty ) continue;

          // décomposer l'indication de la méthode et la référence du texte source *** transposer pour l'anglais ***
          string[] parts = trace.Split( new string[] { " dans " }, StringSplitOptions.None );

          // afficher de manière appropriée
          if ( parts.Length != 2 )
            DoStackTraceAdd( parts[ 0 ] );
          else {
            string[] subparts = parts[ 1 ].Split( new string[] { ":ligne " }, StringSplitOptions.None );
            if ( subparts.Length == 2 )
              DoStackTraceAdd( parts[ 0 ], "source : " + subparts[ 0 ], "ligne : " + subparts[ 1 ] );
            else
              DoStackTraceAdd( parts );
          }
        }
      }
      finally { stack.EndUpdate(); }
    }

    /// <summary>
    /// Peuple les informations relatives au détail d'une exception
    /// </summary>
    /// <param name="exception">object exception source des informations</param>
    private void DoInfosPopulate( Exception exception ) {
      DoPropertiesPopulate( exception );
      DoStackTracePopulate( exception );
    }

    /// <summary>
    /// Prise en charge de la détermination des boutons à afficher
    /// </summary>
    /// <param name="buttons">vecteur de flags des boutons à afficher</param>
    private void DoSetButtons( ExceptionBoxButton buttons ) {
      btShowDetails.Visible = (buttons & ExceptionBoxButton.Details) != 0;
      btOk.Visible = (buttons & ExceptionBoxButton.OK) != 0;
    }

    /// <summary>
    /// Prise en charge de la détermination de l'icône à afficher
    /// </summary>
    /// <param name="icon">constante d'énumération de l'icône à afficher</param>
    private void DoSetIcon( ExceptionBoxIcon icon ) {
      if ( icon == ExceptionBoxIcon.None ) return;

      // filtrer la valeur proposée
      switch ( icon ) {
        case ExceptionBoxIcon.Error:
        case ExceptionBoxIcon.Info:
        case ExceptionBoxIcon.Question:
        case ExceptionBoxIcon.Warning:
        case ExceptionBoxIcon.Danger: break;
        default: icon = ExceptionBoxIcon.Error; break;
      }

      // récupérer l'icône dans les ressources incorporées
      iconPicture.Image = new Icon( this.GetType().Assembly.GetManifestResourceStream( "Psl.Resources." + Enum.GetName( typeof( ExceptionBoxIcon ), icon ) + ".ico" ) ).ToBitmap();
    }

    private void DoGetExceptionMessages( out string first, out string sub ) {
      first = string.Empty;
      sub = string.Empty;
      if ( exception == null ) return;

      // récupérer les messages d'exception
      ExceptionContainer container = exception as ExceptionContainer;
      if ( container == null )
        first = exception.Message;
      else {
        first = container.FirstMessage;
        sub = container.Message;
      }

      // recaler le premier message s'il est encore vide
      if ( first == string.Empty && sub != string.Empty ) {
        first = sub;
        sub = string.Empty;
      }

      // normaliser : ne jamais retourner une chaîne vide
      if ( first == null ) first = string.Empty;
      if (sub == null) sub = string.Empty ;
    }

    /// <summary>
    /// Prise en charge du message à afficher dans le panneau permanent
    /// </summary>
    /// <param name="optional">message optionnel complémentaire</param>
    private void DoSetMessage( string optional ) {
      string main;
      string sub;

      if ( optional == null ) optional = string.Empty;
      DoGetExceptionMessages( out main, out sub );
      string newLine = optional != string.Empty && main != string.Empty ? "\r\n\r\n" : string.Empty;
      main = optional + newLine + main ;

      if ( main == string.Empty ) main = exception == null ? "[Aucun message dans l'objet exception]" : "[Aucun objet exception]";

      lbMessage.Text = main;
      lbMessageContainer.Text = sub;
    }

    /// <summary>
    /// Prépare la boîte de dialogue selon les paramètres
    /// </summary>
    /// <param name="detailed">si true, affichage détaillé à l'ouverture de la boîte</param>
    /// <param name="caption">intitulé de la boîte de dialogue</param>
    /// <param name="message">message additionnel à afficher avant le message de l'objet exception</param>
    /// <param name="exception">objet exception à afficher</param>
    /// <param name="buttons">détermine quels sont les boutons à afficher (Details et Ok par défaut)</param>
    /// <param name="icon">détermine l'icône à afficher (Error, par défaut)</param>
    public void SetData( bool detailed, string caption, string message, Exception exception, ExceptionBoxButton buttons, ExceptionBoxIcon icon ) {

      this.Text = caption == null || caption == string.Empty ? "Exception" : caption;
      this.exception = exception;
      DoSetButtons( buttons );
      DoSetIcon( icon );
      DoSetMessage( message );

      btShowDetails.Enabled = exception != null;

      if ( detailed ) DoToggleDetails();

      Width = ExceptionBox.lastWidth;
      DoResizeBox();
    }

    /// <summary>
    /// Changement de la sélection d'un noeud de l'arbre des exceptions
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void tree_AfterSelect( object sender, TreeViewEventArgs e ) {
      if ( e.Node == null ) return;
      DoInfosPopulate( e.Node.Tag as Exception );
    }

    /// <summary>
    /// Basculer l'affichage des détails
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void btShowDetails_Click( object sender, EventArgs e ) {
      DoToggleDetails();
    }

    /// <summary>
    /// Replie tous les noeuds de la vue en arbre sur la trace de pile
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void btStackCollapseAll_Click( object sender, EventArgs e ) {
      stack.CollapseAll();
    }

    /// <summary>
    /// Déplie tous les noeuds de la vue en arbre sur la trace de pile
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void btStackExpandAll_Click( object sender, EventArgs e ) {
      stack.ExpandAll();
    }

    /// <summary>
    /// Redimensionnement de la boîte de dialogue
    /// </summary>
    /// <param name="sender">source de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void ExceptionBox_Resize( object sender, EventArgs e ) {
      DoResizeBox();
    }

    #endregion

  }

  #region Enumérations du paramétrage de la boîte

  /// <summary>
  /// Détermine quels sont les boutons à afficher
  /// </summary>
  [Flags]
  public enum ExceptionBoxButton {

    /// <summary>
    /// Afficher le bouton "OK"
    /// </summary>
    OK = 0x01,

    /// <summary>
    /// Afficher le boutons "Details"
    /// </summary>
    Details = 0x02,

    /// <summary>
    /// Afficher les boutons "Details" et "OK"
    /// </summary>
    DetailsOK = Details | OK,

    /// <summary>
    /// Afficher tous les boutons
    /// </summary>
    All = 0x0F
  }

  /// <summary>
  /// Détermine l'icône à afficher dans la boîte de dialogue
  /// </summary>
  public enum ExceptionBoxIcon {

    /// <summary>
    /// N'afficher aucune icône
    /// </summary>
    None,

    /// <summary>
    /// Afficher l'icône "Information"
    /// </summary>
    Info,

    /// <summary>
    /// Afficher l'icône "Question"
    /// </summary>
    Question,

    /// <summary>
    /// Afficher l'icône "Avertissement"
    /// </summary>
    Warning,

    /// <summary>
    /// Afficher l'icône "Danger"
    /// </summary>
    Danger,

    /// <summary>
    /// Afficher l'icône "Erreur"
    /// </summary>
    Error
  }

  #endregion

  #region Procédure standard

  // <exclude
  /// <summary>
  /// Affichage standard des exceptions non traitées d'une application
  /// </summary>
  public class ExceptionBoxAuto : ExceptionBox {                                         // <wao ExceptionBoxAuto>

    /// <summary>
    /// Affichage centralisé des exceptions non gérées au niveau de l'application
    /// </summary>
    /// <param name="sender">Emetteur de l'événement</param>
    /// <param name="e">Arguments de l'événement</param>
    private static void OnThreadException( object sender, ThreadExceptionEventArgs e ) { // <wao ExceptionBoxAuto code.&body>
      if ( !(e.Exception is ECancelled) ) 
        ExceptionBox.Show( 
          e.Exception,
          Path.GetFileName( Application.ExecutablePath ) + " : exception non traitée",
          string.Empty,
          ExceptionBoxButton.DetailsOK,
          ExceptionBoxIcon.Danger,
          true
          );
    }

    /// <summary>
    /// Autoriser l'affichage des exceptions retransmises au niveau application
    /// </summary>
    public static void Enable() {                                                        // <wao ExceptionBoxAuto code.&body>
      Application.ThreadException += OnThreadException;
      Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException, true );
    }

    /// <summary>
    /// Inhiber l'affichage des exceptions retransmises au niveau application
    /// </summary>
    public static void Disable() {                                                       // <wao ExceptionBoxAuto code.&body>
      Application.ThreadException -= OnThreadException;
    }
  }                                                                                      // <wao ExceptionBoxAuto>

  #endregion

}
