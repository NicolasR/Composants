/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaud�ne
 * Credit : Serge Gorbenko, Code Guru
 * 
 * 08 11 2008 : version initiale
 */                                                                            // <wao never.end>
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;

namespace Psl.Converters {

  /// <summary>
  /// Flags enumeration type converter.
  /// </summary>
  public class EnumFlagsConverter : EnumConverter {

    /// <summary>
    /// This class represents an enumeration field in the property grid.
    /// </summary>
    protected class EnumFieldDescriptor : SimplePropertyDescriptor {

      #region Fields

      /// <summary>
      /// Stores the context which the enumeration field descriptor was created in.
      /// </summary>
      ITypeDescriptorContext fContext;

      #endregion

      #region Methods

      /// <summary>
      /// Creates an instance of the enumeration field descriptor class.
      /// </summary>
      /// <param name="componentType">The type of the enumeration.</param>
      /// <param name="name">The name of the enumeration field.</param>
      /// <param name="context">The current context.</param>
      public EnumFieldDescriptor( Type componentType, string name, ITypeDescriptorContext context )
        : base( componentType, name, typeof( bool ) ) {
        fContext = context;
      }

      /// <summary>
      /// Retrieves the value of the enumeration field.
      /// </summary>
      /// <param name="component">
      /// The instance of the enumeration type which to retrieve the field value for.
      /// </param>
      /// <returns>
      /// True if the enumeration field is included to the enumeration; 
      /// otherwise, False.
      /// </returns>
      public override object GetValue( object component ) {
        return ((int) component & (int) Enum.Parse( ComponentType, Name )) != 0;
      }

      /// <summary>
      /// Sets the value of the enumeration field.
      /// </summary>
      /// <param name="component">
      /// The instance of the enumeration type which to set the field value to.
      /// </param>
      /// <param name="value">
      /// True if the enumeration field should included to the enumeration; 
      /// otherwise, False.
      /// </param>
      public override void SetValue( object component, object value ) {
        bool myValue = (bool) value;
        int myNewValue;
        if ( myValue )
          myNewValue = ((int) component) | (int) Enum.Parse( ComponentType, Name );
        else
          myNewValue = ((int) component) & ~(int) Enum.Parse( ComponentType, Name );

        FieldInfo myField = component.GetType().GetField( "value__", BindingFlags.Instance | BindingFlags.Public );
        myField.SetValue( component, myNewValue );
        fContext.PropertyDescriptor.SetValue( fContext.Instance, component );
      }

      /// <summary>
      /// Retrieves a value indicating whether the enumeration field is set to a non-default value.
      /// </summary>
      /// <param name="component">the instance of the enumeration</param>
      /// <remarks>true if the value is the non-default value</remarks>
      public override bool ShouldSerializeValue( object component ) {
        return (bool) GetValue( component ) != GetDefaultValue();
      }

      /// <summary>
      /// Resets the enumeration field to its default value.
      /// </summary>
      /// <param name="component">The instance of the enumeration type which to reset the field value</param>
      public override void ResetValue( object component ) {
        SetValue( component, GetDefaultValue() );
      }

      /// <summary>
      /// Retrieves a value indicating whether the enumeration 
      /// field can be reset to the default value.
      /// </summary>
      /// <remarks>true if the value can be reset</remarks>
      public override bool CanResetValue( object component ) {
        return ShouldSerializeValue( component );
      }

      /// <summary>
      /// Retrieves the enumerations field�s default value.
      /// </summary>
      /// <remarks>the default value</remarks>
      private bool GetDefaultValue() {
        object myDefaultValue = null;
        string myPropertyName = fContext.PropertyDescriptor.Name;
        Type myComponentType = fContext.PropertyDescriptor.ComponentType;

        // Get DefaultValueAttribute
        DefaultValueAttribute myDefaultValueAttribute = (DefaultValueAttribute) Attribute.GetCustomAttribute(
          myComponentType.GetProperty( myPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ),
          typeof( DefaultValueAttribute ) );
        if ( myDefaultValueAttribute != null )
          myDefaultValue = myDefaultValueAttribute.Value;

        if ( myDefaultValue != null )
          return ((int) myDefaultValue & (int) Enum.Parse( ComponentType, Name )) != 0;
        return false;
      }

      #endregion

      #region Properties

      /// <summary>
      /// Obtient la collection des attributs
      /// </summary>
      public override AttributeCollection Attributes {
        get {
          return new AttributeCollection( new Attribute[] { RefreshPropertiesAttribute.Repaint } );
        }
      }

      #endregion
    }

    #region Methods

    /// <summary>
    /// Creates an instance of the EnumFlagsConverter class.
    /// </summary>
    /// <param name="type">The type of the enumeration.</param>
    public EnumFlagsConverter( Type type ) : base( type ) { }

    /// <summary>
    /// Retrieves the property descriptors for the enumeration fields. 
    /// These property descriptors will be used by the property grid 
    /// to show separate enumeration fields.
    /// </summary>
    /// <param name="context">The current context.</param>
    /// <param name="value">A value of an enumeration type.</param>
    /// <param name="attributes">liste d'attributs</param>
    /// <returns>the property descriptor collection pour the enumeration fields</returns>
    public override PropertyDescriptorCollection GetProperties( ITypeDescriptorContext context, object value, Attribute[] attributes ) {
      if ( context != null ) {
        Type type = value.GetType();
        string[] names = Enum.GetNames( type );
        Array values = Enum.GetValues( type );
        if ( names != null ) {
          PropertyDescriptorCollection collection = new PropertyDescriptorCollection( null );
          for ( int i = 0 ; i < names.Length ; i++ ) {
            if ( (int) values.GetValue( i ) != 0 && names[ i ] != "All" )
              collection.Add( new EnumFieldDescriptor( type, names[ i ], context ) );
          }
          return collection;
        }
      }
      return base.GetProperties( context, value, attributes );
    }

    /// <summary>
    /// D�termine si le convertisseur supporte un type de propri�t� 
    /// </summary>
    /// <param name="context">contexte associ� au type</param>
    /// <returns>true si le type support�</returns>
    public override bool GetPropertiesSupported( ITypeDescriptorContext context ) {
      if ( context != null ) {
        return true;
      }
      return base.GetPropertiesSupported( context );
    }

    /// <summary>
    /// D�termine si le convertisseur supporte les valeurs standard du type
    /// </summary>
    /// <param name="context">contexte du type</param>
    /// <returns>true si les valeurs standard sont support�es</returns>
    public override bool GetStandardValuesSupported( ITypeDescriptorContext context ) {
      return false;
    }

    #endregion
  }
}
