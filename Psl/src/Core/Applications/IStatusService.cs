/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 09 12 2008 : version initiale
 */                                                                           // <wao never.end>

using System;                                                                 // <wao never>
using System.Drawing;                                                         // <wao never>
using System.Windows.Forms;                                                   // <wao never>

namespace Psl.Applications {

  ///////////////////////////////////////////////////////////////////////////////
  //                                                                           //
  //       ProgressData : données d'ajustage d'une jauge de progression        //
  //                                                                           //
  ///////////////////////////////////////////////////////////////////////////////
                                                                               
  /// <summary>
  /// Données d'ajustage d'une jauge de progression
  /// </summary>
  public struct ProgressData {                                                 // <wao ProgressData.begin strip.begin>

    /// <summary>
    /// Borne minimale de la jauge
    /// </summary>
    public int Minimum;

    /// <summary>
    /// Valeur courante de la jauge
    /// </summary>
    public int Value;

    /// <summary>
    /// Borne maximale de la jauge
    /// </summary>
    public int Maximum;

    /// <summary>
    /// Variation de valeur associée à un pas élémentaire de la jauge
    /// </summary>
    public int Step;

    /// <summary>
    /// Initialisation des données d'ajustage d'une jauge de progression
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="value">valeur courante de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    /// <param name="step">variation associée à un pas</param>
    public ProgressData( int minimum, int value, int maximum, int step )      // <wao ProgressData.+body>
      : this() {                                                              // <wao never.begin>
      Minimum = minimum;
      Value = value;
      Maximum = maximum;
      Step = step;
    }                                                                          // <wao never.end>
  }                                                                            // <wao ProgressData.end strip.end>

                                                                               // <wao StatusLabels.begin>
  /// <summary>
  /// Identification des volets d'affichage de texte standard sous forme énumérée. 
  /// </summary>
  public enum StatusLabels {                                                   // <wao strip.begin>

    /// <summary>
    /// Identifie le volet d'affichage de texte "Left"
    /// </summary>
    Left,

    /// <summary>
    /// Identifie le volet d'affichage de texte "Middle"
    /// </summary>
    Middle,

    /// <summary>
    /// Identifie le volet d'affichage de texte "Right"
    /// </summary>
    Right,

    /// <summary>
    /// Identifie le volet d'affichage de texte "Infos"
    /// </summary>
    Infos
  }                                                                            // <wao StatusLabels.end strip.end>

  /// <summary>
  /// Interface du service d'affichage de l'état courant d'une application.
  /// </summary>
  /// <remarks>
  /// Ce service une interface simplifiée pour l'affichage de l'état courant d'une application
  /// via quatre chaînes de caractères et une jauge de progression. 
  /// La spécification est formulée en supposant que ces informations sont affichées via une barre d'état, 
  /// même s'il ne s'agit en fait que d'une indication.
  /// Une implémentation de cette interface est proposée par le composant <see cref="Psl.Controls.StatusReporter"/>
  /// De gauche à droite, les différents volets de cette barre d'état sont les suivants :
  /// <br/>
  /// volet "Left" : volet de taille fixe affichant un texte court ;
  /// <br/>
  /// volet "Middle" : volet de taille fixe affichant un texte court ;
  /// <br/>
  /// volet "Right" : volet escamotable de taille variable affichant un texte court ou peu long ; 
  /// ce volet se masque automatiquement lorsque le texte est une chaîne vide ;
  /// <br/>
  /// volet "Infos" : volet de taille variable affichant un texte et occupant la place restante de la barre d'état ;
  /// <br/>
  /// volet de progression : volet de taille fixe affichant une jauge de progression ; 
  /// cette jauge se masque automatiquement quand elle est parvenue à son terme. 
  /// </remarks>
  public interface IStatusService {                                            // <wao Textes Images Jauge>
                                                                               // <wao Textes.begin>
    /// <summary>
    /// Obtient ou détermine le texte d'un volet de texte
    /// </summary>
    /// <param name="labelEnumOrName">code énuméré <see cref="StatusLabels"/>ou nom du volet</param>
    /// <returns>le texte du label</returns>
    string this[ object labelEnumOrName ] { get; set; }

    /// <summary>
    /// Obtient ou détermine le texte du volet de texte fixe "Left".
    /// </summary>
    string TextLeft { get;  set; }

    /// <summary>
    /// Obtient ou détermine le texte du volet de texte fixe "Middle".
    /// </summary>
    string TextMiddle { get;  set; }

    /// <summary>
    /// Obtient ou détermine le texte du volet de texte élastique masquable "Right".
    /// </summary>
    string TextRight { get;  set; }

    /// <summary>
    /// Obtient ou détermine le texte du volet de texte élastique "Infos".
    /// </summary>
    string TextInfos { get;  set; }

    /// <summary>
    /// Détermine le texte des trois volets "Left", "Middle" et "Infos".
    /// </summary>
    /// <param name="left">texte à affecter au volet "Left"</param>
    /// <param name="middle">texte à affecter au volet "Middle"</param>
    /// <param name="infos">texte à affecter au volet "Infos"</param>
    void StatusUpdate( string left, string middle, string infos );
                                                                               // <wao Textes.end Images.begin>
    /// <summary>
    /// Obtient la liste d'images associée à l'affichage de l'état
    /// </summary>
    ImageList ImageList { get; }

    /// <summary>
    /// Détermine quelle image de la liste d'images est à associer à un label
    /// </summary>
    /// <param name="labelEnumOrName">code énuméré <see cref="StatusLabels"/>ou nom du label</param>
    /// <param name="imageIndexOrKey">index ou clé de l'image dans la liste d'images <see cref="ImageList"/></param>
    void SetImage( object labelEnumOrName, object imageIndexOrKey );

    /// <summary>
    /// Détermine l'image isolée à associer à un label
    /// </summary>
    /// <param name="labelEnumOrName">code énuméré <see cref="StatusLabels"/>ou nom du label</param>
    /// <param name="image">image à associer au volet</param>
    void SetImage( object labelEnumOrName, Image image );
                                                                               // <wao Images.end Jauge.begin>
    /// <summary>
    /// Initialise les valeurs de la jauge de progression.
    /// </summary>
    /// <param name="mininimum">borne minimale de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    void ProgressInitialize( int mininimum, int maximum );

    /// <summary>
    /// Initialise la jauge de progression et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    /// <param name="step">valeur de l'incrément à chaque étape</param>
    void ProgressInitialize( int minimum, int maximum, int step );

    /// <summary>
    /// Met à jour les valeurs de la jauge et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="minimum">borne minimale de la jauge</param>
    /// <param name="value">valeur courante de la jauge</param>
    /// <param name="maximum">borne maximale de la jauge</param>
    void ProgressUpdate( int minimum, int value, int maximum );

    /// <summary>
    /// Met à jour les valeurs de la jauge et affiche éventuellement la jauge.
    /// </summary>
    /// <param name="data">données fournissant les bornes et la valeur courante de la jauge</param>
    void ProgressUpdate( ProgressData data );

    /// <summary>
    /// Obtient ou détermine la valeur courante de la jauge de progression.
    /// </summary>
    int ProgresseValue { get;  set; }

    /// <summary>
    /// Incrémente la jauge de progression d'un pas.
    /// </summary>
    void ProgressStep();

    /// <summary>
    /// Masque la jauge de progression.
    /// </summary>
    void ProgressFinish();
  }                                                                            // <wao Textes Images Jauge.end>
}
