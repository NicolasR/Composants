/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 02 11 2010 : version inititiale
 * 05 11 2011 : adjonction de GetPropertyDescriptor(Type,string)
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;

namespace Psl.Controls {

  /// <summary>
  /// Recueil de quelques méthodes de classe pour la sérialisation et la désérialisation des propriétés
  /// </summary>
  public class SerialHelper {

    //
    // Service
    //

    /// <summary>
    /// Obtient le descripteur de propriété <see cref="TypeDescriptor"/> d'une propriété d'un type d'objet
    /// </summary>
    /// <param name="type">type exposant la propriété</param>
    /// <param name="propertyName">nom de la propriété</param>
    /// <returns>le descripteur de propriété de la propriété</returns>
    public static PropertyDescriptor GetPropertyDescriptor( Type type, string propertyName ) {
      if ( type == null ) throw new ArgumentNullException( "type" );
      if ( string.IsNullOrEmpty( propertyName ) ) throw new ArgumentException( "Null ou vide", "propertyName" );

      // obtenir le descripteur de la propriété
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( type );
      PropertyDescriptor propertyDescriptor = properties.Find( propertyName, false );
      if ( propertyDescriptor == null ) throw new EControlWrapper( null, "La propriété \"{0}\" est introuvable dans le type \"{1}\"", propertyName, type.FullName );

      // retourner le descripteur trouvé
      return propertyDescriptor;
    }

    /// <summary>
    /// Obtient le descripteur de propriété <see cref="TypeDescriptor"/> d'une propriété d'un objet
    /// </summary>
    /// <param name="target">objet exposant la propriété</param>
    /// <param name="propertyName">nom de la propriété</param>
    /// <returns>le descripteur de propriété de la propriété</returns>
    public static PropertyDescriptor GetPropertyDescriptor( object target, string propertyName ) {
      if ( target == null ) throw new ArgumentNullException( "target" );
      if ( string.IsNullOrEmpty( propertyName ) ) throw new ArgumentException( "Null ou vide", "propertyName" );

      // obtenir le descripteur de la propriété
      PropertyDescriptorCollection properties = TypeDescriptor.GetProperties( target );
      PropertyDescriptor propertyDescriptor = properties.Find( propertyName, false );
      if ( propertyDescriptor == null ) throw new EControlWrapper( null, "La propriété \"{0}\" introuvable dans l'objet de type \"{1}\"", propertyName, target.GetType().FullName );

      // retourner le descripteur trouvé
      return propertyDescriptor;
    }

    //
    // Sérialisation de la valeur d'une propriété en une chaîne
    //

    /// <summary>
    /// Sérialise une valeur en utilisant le descripteur de propriété d'une propriété.
    /// </summary>
    /// <param name="propertyDescriptor">descripteur de propriété de la propriété fournissant les informations nécessaires à la sérialisation</param>
    /// <param name="value">valeur à sérialiser</param>
    /// <returns>la chaîne de caractères obtenue par sérialisation de la valeur, ou null si la valeur ne peut être sérialisée</returns>
    public static string PropertyValueToString( PropertyDescriptor propertyDescriptor, object value ) {
      if ( propertyDescriptor == null ) throw new ArgumentNullException( "propertyDescriptor" );

      // contrôler que la propriété peut être sérialisée/désérialisée par cette méthode
      if ( !propertyDescriptor.PropertyType.IsSerializable || propertyDescriptor.SerializationVisibility != DesignerSerializationVisibility.Visible )
        return null;

      // contrôler que le convertisseur asslocié à la propriété assure la sérialisation et la désérialisation
      TypeConverter typeConverter = TypeDescriptor.GetConverter( propertyDescriptor.PropertyType );
      if ( !typeConverter.CanConvertTo  ( typeof( string ) ) ) return null;
      if ( !typeConverter.CanConvertFrom( typeof( string ) ) ) return null;

      // sérialiser la valeur en utilisant le convertisseur du type
      return typeConverter.ConvertToInvariantString( value );
    }

    /// <summary>
    /// Sérialise la valeur d'une propriété en une chaîne de caractères.
    /// </summary>
    /// <param name="target">objet exposant la propriété à sérialiser</param>
    /// <param name="propertyDescriptor">descripteur de la propriété à sérialiser</param>
    /// <returns>la chaîne obtenue par sérialisation, ou null si la valeur de la propriété ne peut être sérialisée</returns>
    public static string PropertyToString( object target, PropertyDescriptor propertyDescriptor ) {
      if ( target == null ) throw new ArgumentNullException( "target" );
      if ( propertyDescriptor == null ) throw new ArgumentNullException( "propertyDescriptor" );

      // sérialiser la valeur de la propriété
      return PropertyValueToString( propertyDescriptor, propertyDescriptor.GetValue( target ) );
    }

    /// <summary>
    /// Sérialise la valeur d'une propriété en une chaîne.
    /// </summary>
    /// <param name="target">objet exposant la propriété à sérialiser</param>
    /// <param name="propertyName">nom de la propriété à sérialiser</param>
    /// <returns>la chaîne obtenue par sérialisation, ou null si la valeur de la propriété n'a pas pu être sérialisée</returns>
    public static string PropertyToString( object target, string propertyName ) {
      return PropertyToString( target, GetPropertyDescriptor( target, propertyName ) );
    }

    //
    // Désérialisation de la valeur d'une propriété à partir d'une chaîne
    //

    /// <summary>
    /// Désérialise une chaîne en une valeur, en utilisant le descripteur de propriété d'une propriété.
    /// </summary>
    /// <param name="propertyDescriptor">descripteur de propriété à utiliser pour la désérialisation</param>
    /// <param name="value">chaîne à désérialiser</param>
    /// <returns>la valeur obtenue par désérialisation</returns>
    public static object StringToPropertyValue( PropertyDescriptor propertyDescriptor, string value ) {
      if ( propertyDescriptor == null ) throw new ArgumentNullException( "propertyDescriptor" );

      // contrôler que la propriété peut être sérialisée/désérialisée par cette méthode
      if ( !propertyDescriptor.PropertyType.IsSerializable || propertyDescriptor.SerializationVisibility != DesignerSerializationVisibility.Visible ) 
        throw new ArgumentException( string.Format( "La propriété \"{0}\" du type \"{1}\" n'est pas sérialisable par cette méthode", propertyDescriptor.Name, propertyDescriptor.ComponentType.Name ), "propertyDescriptor" );

      // désérialiser la valeur en utilisant le convertisseur du type
      TypeConverter typeConverter = TypeDescriptor.GetConverter( propertyDescriptor.PropertyType );
      return typeConverter.ConvertFromInvariantString( value );
    }

    /// <summary>
    /// Désérialisation de la valeur d'une propriété à partir d'une chaîne.
    /// </summary>
    /// <param name="target">objet exposant la propriété</param>
    /// <param name="propertyDescriptor">descripteur de propriété de la propriété</param>
    /// <param name="value">chaîne à désérialiser</param>
    public static void StringToProperty( object target, PropertyDescriptor propertyDescriptor, string value ) {
      if ( target == null ) throw new ArgumentNullException( "target" );
      if ( propertyDescriptor == null ) throw new ArgumentNullException( "propertyDescriptor" );
      if ( propertyDescriptor.IsReadOnly ) throw new ArgumentException( string.Format( "La propriété \"{0}\" est en lecture seule", propertyDescriptor.Name ), "propertyDescriptor" );

      object propertyValue = StringToPropertyValue( propertyDescriptor, value );
      propertyDescriptor.SetValue( target, propertyValue );
    }

    /// <summary>
    /// Désérialise la valeur d'une propriété à partir d'une chaîne.
    /// </summary>
    /// <param name="target">objet exposant la propriété</param>
    /// <param name="propertyName">nom de la propriété</param>
    /// <param name="value">chaîne à désérialiser</param>
    public static void StringToProperty( object target, string propertyName, string value ) {
      StringToProperty( target, GetPropertyDescriptor( target, propertyName ), value );
    }

  }

}
