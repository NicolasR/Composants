/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 09 05 2010 : version initiale
 * 19 03 2011 : adjonction de méthodes dans les extensions de IDataObject
 */                                                                            // <wao never.end>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using Psl.Controls;

namespace Psl.DragDrop {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                   Interface IDragDropExtender                               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface d'extension pour la prise en charge du démarrage des opérations drag-drop
  /// </summary>
  /// <remarks>
  /// Cette interface est destinée à être implémentée par les contrôles qui intègrent une détection
  /// automatique du démarrage des opérations drag-drop. Cela permet d'associer des coupleurs drag-drop
  /// à ces contrôles et de relayer, au niveau de ces coupleurs, l'événement <see cref="DragStart"/>
  /// ainsi que les autres événements du protocole drag-drop.
  /// </remarks>
  public interface IDragDropExtender {

    //
    // Côté source
    //

    /// <summary>
    /// Applique un prootocole drag-drop
    /// </summary>
    /// <param name="data">objet du drag-drop</param>
    /// <param name="allowedEffects">effets autorisés pour l'opération drag-drop</param>
    /// <returns>l'effet réalisé par la cible</returns>
    DragDropEffects DoDragDrop( object data, DragDropEffects allowedEffects );

    /// <summary>
    /// Obtient ou détermine si le contrôle source autorise les opérations drag-drop
    /// </summary>
    bool AllowDrop { get; set; }

    /// <summary>
    /// Obtient ou détermine côté source si la détection du démarrage d'une opération drag-drop est automatique
    /// </summary>
    bool AutoDragStart { get; set; }

    /// <summary>
    /// Déclenché côté source lorsqu'une opération de glissement commence.
    /// </summary>
    event DragStartEventHandler DragStart;

    /// <summary>
    /// Déclenché côté source lorsqu'il faut réfléchir les informations sur l'opération drag-drop en cours
    /// </summary>
    event GiveFeedbackEventHandler GiveFeedback;

    //
    // Côté cible
    //

    /// <summary>
    /// Déclenché côté cible lorsqu'une opération drag-drop entre dans la surface du contrôle.
    /// </summary>
    event DragEventHandler DragEnter;

    /// <summary>
    /// Déclenché côté cible lorsqu'une opéation drag-drop glisse au-dessus de la surface du contrôle.
    /// </summary>
    event DragEventHandler DragOver;

    /// <summary>
    /// Déclenché côté cible lorsqu'une opération drag-drop lâche son objet au-dessus de la surface du contrôle.
    /// </summary>
    event DragEventHandler DragDrop;

    /// <summary>
    /// Déclenché côté cible lorsqu'une opération drag-drop sort de la surface du contrôle.
    /// </summary>
    event EventHandler DragLeave;

    /// <summary>
    /// Déclenché côté source à l'issue d'une opération drag-drop si un drop a été effectué côté cible.
    /// </summary>
    event DragEventHandler DragTerminate;
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //           DragWatcher : assistant pour la détection et le suivi d'un drag/drop              //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Assistant pour la détection du démarrage d'un drag/drop et pour son suivi
  /// </summary>
  /// <remarks>
  /// Mode d'emploi : 
  /// <br/>
  /// 1) déclarer et initialiser un champ du type <see cref="DragWatcher"/>
  /// <br/>
  /// 2) relayer les événements <see cref="Control.MouseDown"/>, <see cref="Control.MouseMove"/>,
  /// <see cref="Control.MouseUp"/> et <see cref="Control.MouseLeave"/> sur les méthodes WhenXXX correspondantes
  /// <br/>
  /// 3) une opération de glissement commence si <see cref="WhenMouseMove"/> retourne true
  /// <br/>
  /// 4) lorsque <see cref="WhenMouseMove"/> retourne true, appeler <see cref="DragStart"/>
  /// si on veut se servir de l'assistant jusqu'au relâchement pour le suivi dans l'état 
  /// <see cref="DragState.Dragging"/>
  /// <br/>
  /// 5) appeler <see cref="DragTerminate"/> en fin d'opération de glissement pour réinitialiser l'assistant
  /// </remarks>
  public struct DragWatcher {

    /// <summary>
    /// Enumération des états de l'assistant de détection de démarrage <see cref="DragWatcher"/>
    /// </summary>
    public enum DragState {

      /// <summary>
      /// Pas de détection ou d'opération de glissement en cours
      /// </summary>
      Quiet,

      /// <summary>
      /// Détection du démarrage d'une opération de glissement en cours
      /// </summary>
      Detecting,

      /// <summary>
      /// Opération de glissement en cours
      /// </summary>
      Dragging
    }

    // état de l'assistant
    private DragState dragState;

    // rectangle de détection autour de l'impact souris initial
    private Rectangle dragArea;

    /// <summary>
    /// Assistance sur événment <see cref="Control.MouseDown"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement <see cref="Control.MouseDown"/> original</param>
    public void WhenMouseDown( MouseEventArgs e ) {
      dragState = DragState.Detecting;
      Location = e.Location;
      Button = e.Button;
      Size dragSize = SystemInformation.DragSize;
      dragArea = new Rectangle( new Point( e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2) ), dragSize );
    }

    /// <summary>
    /// Assistance sur un événement <see cref="Control.MouseMove"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement <see cref="Control.MouseMove"/> original</param>
    /// <returns>true si un démarrage d'opération de glissement a été détecté</returns>
    public bool WhenMouseMove( MouseEventArgs e ) {
      if ( dragState != DragState.Detecting ) return false;
      return !dragArea.Contains( e.Location );
    }

    /// <summary>
    /// Assistance sur un événement <see cref="Control.MouseUp"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement <see cref="Control.MouseUp"/> original</param>
    public void WhenMouseUp( MouseEventArgs e ) {
      DragTerminate();
    }

    /// <summary>
    /// Assistance sur un événement <see cref="Control.MouseLeave"/>
    /// </summary>
    /// <param name="e">descripteur de l'événement <see cref="Control.MouseLeave"/> original</param>
    public void WhenMouseLeave( EventArgs e ) {
      //DragTerminate();
    }

    /// <summary>
    /// Bascule d'assistant dans l'état <see cref="DragState.Dragging"/>
    /// </summary>
    public void DragStart() {
      dragState = DragState.Dragging;
    }

    /// <summary>
    /// Bascule d'assistant dans l'état <see cref="DragState.Quiet"/>
    /// </summary>
    public void DragTerminate() {
      dragState = DragState.Quiet;
      Button = MouseButtons.None;
      dragArea = Rectangle.Empty;
    }

    /// <summary>
    /// Indique si l'assistant est dans l'état <see cref="DragState.Detecting"/>
    /// </summary>
    public bool IsDetecting {
      get { return dragState == DragState.Detecting; }
    }

    /// <summary>
    /// Indique si l'assistant est dans l'état <see cref="DragState.Dragging"/>
    /// </summary>
    public bool IsDragging {
      get { return dragState == DragState.Dragging; }
    }

    /// <summary>
    /// Obtient les coordonnées client de l'impact initial de l'enfoncement du bouton
    /// </summary>
    public Point Location { get; private set; }

    /// <summary>
    /// Obtient l'indication du bouton lié à l'impact initial de l'enfoncement
    /// </summary>
    public MouseButtons Button { get; private set; }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //               DragEventArgsExtender : méthodes d'extension pour DragEventArgs               //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Recueil de méthodes d'extensions pour les formats des données transmises via <see cref="IDataObject"/>.
  /// </summary>
  public static class DataObjectExtender {

    /// <summary>
    /// Paramétrage des formats acceptés pour une chaîne simple
    /// </summary>
    public static string[] FormatsForString = new string[] { DataFormatsEx.String, DataFormatsEx.FileName };

    /// <summary>
    /// Paramétrage des formats acceptés pour un tableau de chaînes
    /// </summary>
    public static string[] FormatsForStringArray = new string[] { DataFormatsEx.StringArray, DataFormatsEx.FileNameArray, DataFormatsEx.String, DataFormatsEx.FileName };

    /// <summary>
    /// Paramétrage des formats acceptés comme un nom de fichier
    /// </summary>
    public static string[] FormatsForFileName = new string[] { DataFormatsEx.FileName };

    /// <summary>
    /// Paramétrage des formats acceptés comme un tableau de noms de fichiers
    /// </summary>
    public static string[] FormatsForFileNameArray = new string[] { DataFormatsEx.FileNameArray, DataFormatsEx.FileName };

    /// <summary>
    /// Paramétrage des formats acceptés comme une url
    /// </summary>
    public static string[] FormatsForUrl = new string[] { DataFormatsEx.Url, DataFormatsEx.UrlW, DataFormatsEx.FileName };

    /// <summary>
    /// Paramétrage des formats acceptés comme un tableau d'url
    /// </summary>
    public static string[] FormatsForUrlArray = new string[] { DataFormatsEx.UrlArray, DataFormatsEx.FileNameArray, DataFormatsEx.Url, DataFormatsEx.UrlW, DataFormatsEx.FileName };

    //
    // Contrôle
    // 

    private static void DoCheckZis( IDataObject value, string argName = "zis" ) {
      if ( value == null ) throw new ArgumentNullException( "La valeur de l'argument ne doit pas être null", argName );
    }

    private static void DoCheckValues( string value, string argName = "value" ) {
      if (value == null) throw new ArgumentException( "La valeur de l'argument ne doit pas être null", argName);
      if ( string.IsNullOrEmpty( value ) ) throw new ArgumentException( "La valeur de l'argument ne doit pas être une chaîne vide", argName );
    }

    private static void DoCheckValues( string[] value, string argName = "values" ) {
      if ( value == null ) throw new ArgumentException( "La valeur de l'argument ne doit pas être null", argName );
      if ( value.Length == 0 ) throw new ArgumentException( "La valeur de l'argument ne doit pas être un tableau vide", argName );
      for ( int ix = 0 ; ix < value.Length ; ix++ )
        if ( string.IsNullOrEmpty( value[ ix ] ) ) throw new ArgumentException( "Aucun élément du tableau passé en argument ne doit contenir de chaîne null ou vide", string.Format( argName + "[{0}]", ix ) );
    }

    private static void DoCheckValues( Array value, string argName = "values" ) {
      if ( value == null ) throw new ArgumentException( "La valeur de l'argument ne doit pas être null", argName );
      if ( value.Length == 0 ) throw new ArgumentException( "La valeur de l'argument ne doit pas être un tableau vide", argName );
      for ( int ix = 0 ; ix < value.Length ; ix++ )
        if ( value.GetValue( ix ) == null ) throw new ArgumentException( "Aucun élément du tableau passé en argument ne doit être null", string.Format( argName + "[{0}]", ix ) );
    }

    private static void DoCheckFormats( string[] value, string argName = "formats" ) {
      if ( value == null ) throw new ArgumentException( "La valeur de l'argument ne doit pas être null", argName );
      if ( value.Length == 0 ) throw new ArgumentException( "La valeur de l'argument ne doit pas être un tableau vide", argName );
      for ( int ix = 0 ; ix < value.Length ; ix++ )
        if ( string.IsNullOrEmpty( value[ ix ] ) ) throw new ArgumentException( "Aucun élément du tableau des formats ne peut être une chaîne null ou vide", string.Format( argName + "[{0}]", ix ) );
    }

    //
    // Streaming
    //

    private static MemoryStream ToMemoryStream( string value ) {
      byte[] bytes = Encoding.Unicode.GetBytes( value );
      return new MemoryStream( bytes );
    }

    private static string FromMemoryStream( MemoryStream stream ) {
      byte[] bytes = stream.ToArray();
      return Encoding.Unicode.GetString( bytes );
    }

    //
    // Analyse des formats de données
    //

    private static bool GetDataInfos( this IDataObject zis, bool forceGetData, string[] formats, out object data ) {
      data = null;

      for ( int ix = 0 ; ix < formats.Length ; ix++ ) {
        string format = formats[ ix ];
        if ( !zis.GetDataPresent( format ) ) continue;
        if ( forceGetData ) data = zis.GetData( format );
        return true;
      }
      return false;
    }

    private static string GetDataAsString( this IDataObject zis, string[] formats ) {
      object data;
      if ( !GetDataInfos( zis, true, formats, out data ) ) throw new InvalidOperationException( "Drag drop : aucune donnée correspondant au format n'a été trouvée" );

      if ( data is string )
        return data as string;
      else if ( data is string[] && ( data as string[] ).Length > 0 )
        return ( data as string[] )[ 0 ];
      else if ( data is MemoryStream )
        return FromMemoryStream( data as MemoryStream );
      else
        throw new InvalidOperationException( "Drag drop : la donnée n'est convertible ni en string ni en string[] ni en MemoryStream" );
    }

    private static string[] GetDataAsStringArray( this IDataObject zis, string[] formats ) {
      object data;
      if ( !GetDataInfos( zis, true, formats, out data ) ) throw new InvalidOperationException( "Drag drop : aucune donnée correspondant au format n'a été trouvée" );

      if ( data is string )
        return new string[] { data as string };
      else if ( data is string[] )
        return data as string[];
      else if ( data is MemoryStream )
        return new string[] { FromMemoryStream( data as MemoryStream ) };
      else
        throw new InvalidOperationException( "Drag drop : la donnée n'est convertible ni en string ni en string[] ni en MemoryStream" );
    }

    //
    // Constitution de l'objet de dragging
    //

    /// <summary>
    /// Ajoute aux données à glisser un tableau d'objets selon les formats fournis dans un tableau.
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="formats">tableau des formats</param>
    /// <param name="values">tableau des valeurs</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">
    /// si l'argument <paramref name="formats"/> est null, ou vide ou contient des chaînes null ou vides,
    /// ou si l'argument <paramref name="values"/> est null, ou vide, ou contient des références null
    /// </exception>
    public static void SetDataAs( this IDataObject zis, string[] formats, Array values ) {
      DoCheckZis( zis );
      DoCheckValues( values );
      DoCheckFormats( formats );
      foreach ( string format in formats )
        zis.SetData( format, values );
    }

    /// <summary>
    /// Ajoute aux données à glisser une chaîne de caractères
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="value">donnée à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="value"/> est null ou chaîne vide</exception>
    public static void SetDataAsString( this IDataObject zis, string value ) {
      DoCheckZis( zis );
      DoCheckValues( value );
      zis.SetData( DataFormatsEx.String, true, value );
    }

    /// <summary>
    /// Ajoute aux données à glisser un tableau de chaînes de caractères
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="values">données à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="values"/> est null, vide, ou contient des chaînes null ou vides</exception>
    public static void SetDataAsString( this IDataObject zis, string[] values ) {
      DoCheckZis( zis );
      DoCheckValues( values );
      zis.SetData( DataFormatsEx.StringArray, true, values );
    }

    /// <summary>
    /// Ajoute aux données à glisser un nom de fichier
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="value">donnée à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="value"/> est null ou chaîne vide</exception>
    public static void SetDataAsFileName( this IDataObject zis, string value ) {
      DoCheckZis( zis );
      DoCheckValues( value );
      zis.SetData( DataFormatsEx.String, true, value );
      zis.SetData( DataFormatsEx.FileName, true, value );
    }

    /// <summary>
    /// Ajoute aux données à glisser un tableau de noms de fichiers
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="values">données à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="values"/> est null, vide, ou contient des chaînes null ou vides</exception>
    public static void SetDataAsFileName( this IDataObject zis, string[] values ) {
      DoCheckZis( zis );
      DoCheckValues( values );
      zis.SetData( DataFormatsEx.StringArray, true, values );
      zis.SetData( DataFormatsEx.FileNameArray, true, values );
    }

    /// <summary>
    /// Ajoute aux données à glisser une url
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="value">donnée à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="value"/> est null ou chaîne vide</exception>
    public static void SetDataAsUrl( this IDataObject zis, string value ) {
      DoCheckZis( zis );
      DoCheckValues( value );
      zis.SetData( DataFormatsEx.String, true, value );
      zis.SetData( DataFormatsEx.Url, true, value );
      zis.SetData( DataFormatsEx.FileNameArray, true, new string[] { value } ); // pour IE
    }

    /// <summary>
    /// Ajoute aux données à glisser un tableau d'url
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à compléter</param>
    /// <param name="values">données à glisser</param>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si l'argument <paramref name="values"/> est null, vide, ou contient des chaînes null ou vides</exception>
    public static void SetDataAsUrl( this IDataObject zis, string[] values ) {
      DoCheckZis( zis );
      DoCheckValues( values );
      zis.SetData( DataFormatsEx.StringArray, true, values );
      zis.SetData( DataFormatsEx.UrlArray, true, values );
      zis.SetData( DataFormatsEx.FileNameArray, true, values ); // pour IE
    }

    //
    // Analyse des objets de dragging
    //

    /// <summary>
    /// Détermine s'il y a des données glissées pour au moins un format de données. 
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="formats">tableau des formats souhaités</param>
    /// <returns>true si l'objet <see cref="IDataObject"/> contient des données pour au moins l'un des formats</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    public static bool HasDataAs( this IDataObject zis, string[] formats ) {
      DoCheckZis( zis );

      object data;
      return GetDataInfos( zis, false, formats, out data );
    }

    /// <summary>
    /// Détermine s'il y a des données glissées pour le format chaîne de caractères
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>true si l'objet <see cref="IDataObject"/> contient des données pour le format chaîne</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    public static bool HasDataAsString( this IDataObject zis, bool acceptCollection = false ) {
      DoCheckZis( zis );
      return zis.HasDataAs( acceptCollection ? FormatsForStringArray : FormatsForString );
    }

    /// <summary>
    /// Détermine s'il y a des données glissées pour le format nom de fichier
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>true si l'objet <see cref="IDataObject"/> contient des données pour le format nom de fichier</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    public static bool HasDataAsFileName( this IDataObject zis, bool acceptCollection = false ) {
      DoCheckZis( zis );
      return zis.HasDataAs( acceptCollection ? FormatsForFileNameArray : FormatsForFileName );
    }

    /// <summary>
    /// Détermine s'il y a des données glissées pour le format url
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>true si l'objet <see cref="IDataObject"/> contient des données pour le format url</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    public static bool HasDataAsUrl( this IDataObject zis, bool acceptCollection = false ) {
      DoCheckZis( zis );
      return zis.HasDataAs( acceptCollection ? FormatsForUrlArray : FormatsForUrl );
    }

    //
    // Récupération des données draggées
    //

    /// <summary>
    /// Récupère une donnée pour au moins un format de données parmi les données glissées.
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="formats">tableau des formats souhaités</param>
    /// <returns>les données récupérées pour l'un des formats, ou null si introuvable</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="ArgumentException">si le tableau des formats <paramref name="formats"/> est null, ou vide, ou contient des chaînes null ou vides</exception>
    public static object GetDataAs( this IDataObject zis, string[] formats ) {
      DoCheckZis( zis );
      DoCheckFormats( formats );

      object data;
      GetDataInfos( zis, true, formats, out data );
      return data;
    }

    /// <summary>
    /// Récupère une donnée au format chaîne parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <returns>une chaîne de caractères extraite des données glissées</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string GetDataAsString( this IDataObject zis ) {
      DoCheckZis( zis );
      return zis.GetDataAsString( FormatsForString );
    }

    /// <summary>
    /// Récupère toutes les chaînes de caractères trouvées parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>un tableau des chaînes de caractères trouvées</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string[] GetDataAsString( this IDataObject zis, bool acceptCollection ) {
      DoCheckZis( zis );
      return zis.GetDataAsStringArray( acceptCollection ? FormatsForStringArray : FormatsForString );
    }

    /// <summary>
    /// Récupère une donnée au format nom de fichier parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <returns>un nom de fichier extrait des données glissées</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string GetDataAsFileName( this IDataObject zis ) {
      DoCheckZis( zis );
      return zis.GetDataAsString( FormatsForUrl );
    }

    /// <summary>
    /// Récupère tous les noms de fichiers trouvés parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>un tableau des noms de fichiers trouvés</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string[] GetDataAsFileName( this IDataObject zis, bool acceptCollection ) {
      DoCheckZis( zis );
      return zis.GetDataAsStringArray( acceptCollection ? FormatsForFileNameArray : FormatsForFileName );
    }

    /// <summary>
    /// Récupère une donnée au format url parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <returns>une url extraite des données glissées</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string GetDataAsUrl( this IDataObject zis ) {
      DoCheckZis( zis );
      return zis.GetDataAsString( FormatsForUrl );
    }

    /// <summary>
    /// Récupère toutes les url trouvées parmi les données glissées
    /// </summary>
    /// <param name="zis">object <see cref="IDataObject"/> à examiner</param>
    /// <param name="acceptCollection">true si les collections sont acceptées</param>
    /// <returns>un tableau des url trouvées</returns>
    /// <exception cref="ArgumentNullException">si l'argument <paramref name="zis"/> est null</exception>
    /// <exception cref="InvalidOperationException">si aucune donnée au format voulu n'a été trouvée</exception>
    public static string[] GetDataAsUrl( this IDataObject zis, bool acceptCollection ) {
      DoCheckZis( zis );
      return zis.GetDataAsStringArray( acceptCollection ? FormatsForUrlArray : FormatsForUrl );
    }

  }
}
