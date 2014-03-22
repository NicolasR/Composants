/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 15 11 2008 : version initiale
 */                                                                            // <wao never.end>

//#define DEBUGWRAPPER

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Psl.Controls {

  #region Classe des exceptions
  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                 Classe des exceptions                                       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe des exceptions du wrapper de contrôles 
  /// </summary>
  public class EControlWrapper : Exception {

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="text">message associé à l'exception</param>
    public EControlWrapper( string text ) : base( text ) { }

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="inner">référence sur l'objet exception original</param>
    /// <param name="text">message à associer à l'exception pour formatage</param>
    /// <param name="args">arguments du formatage du message de l'exception</param>
    public EControlWrapper( Exception inner, string text, params object[] args ) : base( string.Format( text, args ), inner ) { }
  }

  #endregion

  #region Classe enveloppe pour une liste de contrôles
  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                        Classe enveloppe pour une liste de contrôles                         //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe de service pour envelopper les contrôles en vue de les sérialiser via Soap
  /// </summary>
  /// <remarks>
  /// De manière native, les contrôles Windows Forms ne sont pas sérialisables.
  /// Pour contourner cette difficulté, une solution consiste à sérialiser, à la place du contrôle, un objet
  /// qui contient la valeur des propriétés du contrôle à sérialiser. Lors de la désérialisation, un
  /// contrôle de même type que l'original est instancié, et ses champs et propriétés sont affectés
  /// à partir des valeurs mémorisées dans le wrapper. 
  /// <br/>
  /// De manière à pouvoir mémoriser une liste de contrôles, la classe ControlWrapper ne mémorise en
  /// fait qu'un tableau d'enveloppes de type ControlWrapper_Control : c'est la classe Wapper qui enveloppe les propriétés
  /// d'un contrôle donné. 
  /// <br/>
  /// En outre, il s'avère que certains types de propriétés des contrôles provoquent des difficultés lors de la 
  /// dé-sérialisation (en particulier les propriétés de type Font et Cursor). Les valeurs de ces propriétés
  /// sont spécialement enveloppées dans une enveloppe ControlWrapper_Property. 
  /// <br/>
  /// Cette classe d'enveloppement n'est pas complètement générale et comporte donc des restrictions :
  /// (1) elle ne sérialise pas les contrôles enfants ;
  /// (2) elle ne sérialise pas les items des collections des contrôles (chaînes d'une ListBox, par exemple). 
  /// </remarks>
  [Serializable()]
  public partial class ControlWrapper : IEnumerable {

    //
    // Champs de classe
    //

    /// <summary>
    /// Constante d'identification de la version courante des enveloppes.
    /// </summary>
    private const string CurrentVersion = "ControlWrapper - version 000 du 2008 10 19" ;

    //
    // Paramétrage des enveloppes
    //

    /// <summary>
    /// Obtient ou détermine l'option pour une sérialisation partielle (false) ou complète (true)
    /// </summary>
    /// <remarks>
    /// Quand cette option n'est pas armée (false), le processus d'enveloppement ne procède à l'enveloppement
    /// de la valeur d'une propriété sérialisable que si la méthode ShouldSerializeValue du descripteur 
    /// de la propriété (type PropertyDescriptor) renvoie true.
    /// <br/>
    /// Quand cette option est armée (true), le processus d'enveloppement enveloppe toutes les propriétés
    /// sérialisables.
    /// <br/>
    /// Par défaut, cette option a la valeur false.
    /// </remarks>
    public static bool IgnoreShouldSerialiseValue { get; set; }

    //
    // Méthodes de classe publiques
    //

    /// <summary>
    /// Construit une enveloppe de sérialisation pour un ou plusieurs contrôles
    /// </summary>
    /// <param name="controls">le ou les contrôles à envelopper</param>
    /// <returns>une enveloppe des contrôles</returns>
    public static ControlWrapper Wrap( params Control[] controls ) {
      return new ControlWrapper( controls );
    }

    /// <summary>
    /// Construit une enveloppe de sérialisation pour une liste de contrôles
    /// </summary>
    /// <param name="controls">énumérateur de la liste des contrôles à envelopper</param>
    /// <returns>une enveloppe de la liste des contrôles</returns>
    public static ControlWrapper Wrap( IEnumerable controls ) {
      return new ControlWrapper( controls );
    }

    /// <summary>
    /// Construit le clone d'un contrôle
    /// </summary>
    /// <param name="control">contrôle à cloner</param>
    /// <returns>un clone du contrôle</returns>
    public static Control Clone( Control control ) {
      return Wrap( control ).Control;
    }

    /// <summary>
    /// Construit le clone d'un ou de plusieurs contrôles
    /// </summary>
    /// <param name="controls">le ou les contrôles à cloner</param>
    /// <returns>un tableau des contrôles clonés</returns>
    public static Control[] Clone( params Control[] controls ) {
      return Wrap( controls ).Controls;
    }

    //
    // Champs
    //

    /// <summary>
    /// Signature de sécurité pour le contrôle de la version des enveloppes
    /// </summary>
    private string Version = CurrentVersion;

    /// <summary>
    /// Tableau des enveloppes des contrôles
    /// </summary>
    private ControlWrapper_Control[] wrappers = null;

    /// <summary>
    /// Tableau des contrôles reconstruits après dé-enveloppement
    /// </summary>
    /// <remarks>
    /// Ce tableau n'est pas sérialisé, car il est construit après désérialisation
    /// par dé-enveloppement des contrôles.
    /// </remarks>
    [NonSerialized]
    private Control[] controls = null;

    //
    // Méthodes de service
    //

    /// <summary>
    /// Dé-enveloppement des contrôles du tableau des enveloppes
    /// </summary>
    private void DoUnwrap() {
      if ( Version != CurrentVersion ) throw new EControlWrapper( null, "La version des enveloppes sérialisées \"{0}\" ne correspond pas à la version courante \"{1}\"", Version, CurrentVersion );
      if ( wrappers == null ) throw new EControlWrapper( "Aucun wrapper disponible" ); 
      if ( controls != null ) return;

      List<Control> list = new List<Control>();
      foreach (ControlWrapper_Control wrapper in wrappers )
        if (wrapper  != null)
          list.Add( wrapper.GetControl() ) ;

      controls = list.ToArray() ;
    }

    //
    // Constructeur internal
    //

    /// <summary>
    /// Constructeur de l'enveloppe
    /// </summary>
    /// <param name="controls">énumérateur des contrôles à envelopper</param>
    internal ControlWrapper( IEnumerable controls ) {
      List<ControlWrapper_Control> list = new List<ControlWrapper_Control>();
      if ( controls != null )
        foreach ( Control control in controls )
          if ( control != null )
            list.Add( new ControlWrapper_Control( control ) );
      wrappers = list.ToArray();
    }

    //
    // Propriétés et méthodes publiques
    //

    /// <summary>
    /// Obtient le nombre de contrôles reconstruits après dé-enveloppement.
    /// </summary>
    public int Count {
      get {
        DoUnwrap();
        return controls.Length;
      }
    }

    /// <summary>
    /// Obtient le premier contrôle reconstruit après dé-enveloppement.
    /// </summary>
    /// <remarks>
    /// Si l'enveloppe ne contient aucun contrôle enveloppé, cette propriété retourne null. 
    /// </remarks>
    public Control Control {
      get {
        DoUnwrap();
        return controls.Length == 0 ? null : controls[ 0 ];
      }
    }

    /// <summary>
    /// Obtient un tableau des contrôles reconstruits après dé-enveloppement
    /// </summary>
    /// <remarks>
    /// Cette propriété ne retourne jamais null : 
    /// elle retournera un tableau vide si l’enveloppe ne contient aucun contrôle
    /// </remarks>
    public Control[] Controls {
      get {
        DoUnwrap();
        return controls;
      }
    }

    /// <summary>
    /// Obtient l'un des contrôles reconstruits après dé-enveloppement
    /// </summary>
    /// <param name="index">index du contrôle</param>
    /// <returns>le contrôle reconstruit</returns>
    public Control this[ int index ] {
      get {
        DoUnwrap();
        return controls[ index ];
      }
    }

    /// <summary>
    /// Obtient un énumérateur des contrôles reconstruits après dé-enveloppement
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetEnumerator() {
      DoUnwrap();
      return controls.GetEnumerator();
    }
  }

  #endregion

  #region Classe enveloppe pour une propriété "spéciale"
  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                           Classe enveloppe de propriété "spéciale"                          //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Classe pour l'enveloppement de la valeur de certaines propriétés "spéciales"
  /// </summary>
  /// <remarks>
  /// Cette classe est destinée à contourner les problèmes de dé-sérialisation de certains types de propriétés 
  /// qui provoquent des exceptions lors de la dé-sérialisation. 
  /// Lors de la sérialisation, la valeur de la propriété est convertie en chaîne via la méthode 
  /// ConvertToInvariantString fournie par le convertisseur de ce type.
  /// Lors de la dé-sérialisation, la valeur de la propriété est reconstruite via la méthode
  /// ConvertFromInvariantString fournie par le convertisseur de ce type. 
  /// La classe d'enveloppement mémorise le nom du type de la propriété et la valeur chaîne invariante.
  /// </remarks>
  [Serializable]
  internal class ControlWrapper_Property {

    /// <summary>
    /// Nom complètement qualifié du type de la propriété avec indication de l'assembly
    /// </summary>
    private string TypeName;

    /// <summary>
    /// Valeur de la propriété sous la forme d'une chaîne invariante
    /// </summary>
    private string InvariantValue;

    /// <summary>
    /// Constructeur d'une enveloppe à partir d'une valeur de propriété
    /// </summary>
    /// <param name="type">description du type de la propriété</param>
    /// <param name="value">valeur originale de la propriété</param>
    public ControlWrapper_Property( Type type, object value ) {
      TypeConverter converter = TypeDescriptor.GetConverter( type );
      InvariantValue = converter.ConvertToInvariantString( value );
      TypeName = type.AssemblyQualifiedName;
    }

    /// <summary>
    /// Dé-enveloppement de la valeur de la propriété
    /// </summary>
    /// <returns>la valeur reconstruite de la propriété</returns>
    public object GetValue() {
      Type type = Type.GetType( TypeName );
      if ( type == null ) throw new EControlWrapper( "Le type est introuvable : " + TypeName );

      TypeConverter converter = TypeDescriptor.GetConverter( type );
      return converter.ConvertFromInvariantString( InvariantValue );
    }
  }

  #endregion

  #region Classe enveloppe pour un contrôle élémentaire
  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                           Classe enveloppe pour un contrôle                                 //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Enveloppe pour un contrôle individuel
  /// </summary>
  [Serializable()]
  internal class ControlWrapper_Control {

    /// <summary>
    /// Nom complètement qualifié du type de contrôle enveloppé (avec indication de l'assembly)
    /// </summary>
    private string name;

    /// <summary>
    /// Table de correspondance entre nom et valeurs de propriétés
    /// </summary>
    private Hashtable properties = new Hashtable();

    /// <summary>
    /// Constructeur de l'enveloppe
    /// </summary>
    /// <param name="control">référence sur le contrôle à envelopper</param>
    public ControlWrapper_Control( Control control ) {
      object propertyValue = null;
      string propertyName = null;
      Type propertyType = null;

      // récupérer le nom complètement qualifié du type du contrôle
      name = control.GetType().AssemblyQualifiedName;

      // Obtenir la liste des propriétés du contrôle
      PropertyDescriptorCollection propertyList = TypeDescriptor.GetProperties( control );

      // Elaborer le dictionnaire nom/valeur des propriétés
      foreach ( PropertyDescriptor propertyDescriptor in propertyList ) {

        propertyType = propertyDescriptor.PropertyType;

        if ( !propertyType.IsSerializable ) continue;
        if ( !ControlWrapper.IgnoreShouldSerialiseValue && !propertyDescriptor.ShouldSerializeValue( control ) ) continue;

        propertyName = propertyDescriptor.Name;

        // Tenter d'obtenir la valeur de la propriété : ignorer les propriétés qui déclenchent une exception
        try {
          propertyValue = propertyDescriptor.GetValue( control );
        }
#if DEBUGWRAPPER
        catch ( Exception ex ) {
          System.Diagnostics.Debug.WriteLine( "wrapper*** " + properties.Count + ", " + propertyName + " = ?" );
          System.Diagnostics.Debug.WriteLine( "wrapper*** " + ex.Message );
        }
#else
        catch {
          continue;
        }
#endif

        // Contourner les difficultés de sérialisation Soap de certaines propriétés
        if ( propertyType == typeof( Font ) || propertyType == typeof( Cursor ) )
          propertyValue = new ControlWrapper_Property( propertyType, propertyValue );

        // Ajouter au dictionnaire nom/valeur
        properties.Add( propertyName, propertyValue );
#if DEBUGWRAPPER
        System.Diagnostics.Debug.WriteLine( "wrapper... " + properties.Count + ", " + propertyName + "=" + propertyValue );
#endif
      }
    }

    /// <summary>
    /// Instancier le contrôle enveloppé
    /// </summary>
    /// <returns>la référence sur le contrôle instancié</returns>
    public Control GetControl() {
      Control control = null;
      object propertyValue = null;
      string propertyName = null;

      // Obtenir la description du type nommé name
      Type type = Type.GetType( name );
      if ( type == null ) throw new EControlWrapper( "Le type est introuvable : " + name );

      // Produire une instance de ce type 
      try {
        control = (Control) Activator.CreateInstance( type );
      }
      catch ( Exception ex ) {
        throw new EControlWrapper( ex, "L'instanciation du type {0} a échoué", name );
      }

      // Récupérer la liste des description de propriétés du contrôle
      PropertyDescriptorCollection propertyList = TypeDescriptor.GetProperties( control );

      // Assigner les valeurs envoppées au nouveau contrôle
      foreach ( PropertyDescriptor property in propertyList ) {
        propertyName = property.Name;
        if ( !properties.Contains( property.Name ) ) continue;

        propertyValue = properties[ propertyName ];

        // Contourner les difficultés de sérialisation Soap de certaines propriétés
        if ( propertyValue is ControlWrapper_Property )
          propertyValue = ((ControlWrapper_Property) propertyValue).GetValue();

        // Assigner la valeur de la propriété
        try {
          property.SetValue( control, propertyValue );
        }
#if DEBUGWRAPPER
        catch ( Exception ex ) {
          System.Diagnostics.Debug.WriteLine( "wrapper*** " + propertyName + "=" + propertyValue );
          System.Diagnostics.Debug.WriteLine( "wrapper*** " + ex.Message );
        }
#else
        catch { }
#endif
      }
      return control;
    }
  }
  #endregion
}
