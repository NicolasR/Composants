/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * Crédit : Microsoft pour FontNameExConverter
 * 
 * 21 05 2009 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Collections;

namespace Psl.Converters {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                           Convertisseur pour les noms de fonts                              //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Convertisseur de type pour un nom de police
  /// </summary>
  /// <remarks>
  /// Ce convertisseur reprend le convertisseur <see cref="FontConverter.FontNameConverter"/> du framework
  /// de manière à en faire une classe publique (et non pas scellée) munie de la prise en charge éventuelle
  /// des valeurs null ou chaîne vide.
  /// </remarks>
  public class FontNameExConverter : TypeConverter, IDisposable {

    // 
    // Champs
    //

    // collection des valeurs acceptées
    private TypeConverter.StandardValuesCollection standardValues;

    // inclure null ou vide comme valeur standard
    private bool includeNoneAsStandardValue = false;

    // chaîne à associer à null ou vide
    private string noneAsStringValue = "(aucun)";

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Constructeur
    /// </summary>
    public FontNameExConverter() {
      Microsoft.Win32.SystemEvents.InstalledFontsChanged += new EventHandler( this.OnInstalledFontsChanged );
    }

    /// <summary>
    /// Finalisation avec suppression des abonnements
    /// </summary>
    void IDisposable.Dispose() {
      Microsoft.Win32.SystemEvents.InstalledFontsChanged -= new EventHandler( this.OnInstalledFontsChanged );
    }

    /// <summary>
    /// Handler pour la prise en charge d'un changement dans les fonts installées.
    /// </summary>
    /// <param name="sender">émetteur de l'événement</param>
    /// <param name="e">descripteur de l'événement</param>
    private void OnInstalledFontsChanged( object sender, EventArgs e ) {
      standardValues = null;
    }

    //
    // Service
    //

    /// <summary>
    /// Recherche le meilleur nom de font normalisé dans la liste des font installées
    /// </summary>
    /// <param name="name">nom de font à rechercher</param>
    /// <param name="context">contexte de la conversion</param>
    /// <returns>le nom normalisé de la font si trouvée, le nom initial sinon</returns>
    private string MatchFontName( string name, ITypeDescriptorContext context ) {
      string str = null;
      name = name.ToLower( System.Globalization.CultureInfo.InvariantCulture );
      IEnumerator enumerator = this.GetStandardValues( context ).GetEnumerator();
      while ( enumerator.MoveNext() ) {
        string str2 = enumerator.Current.ToString().ToLower( System.Globalization.CultureInfo.InvariantCulture );
        if ( str2.Equals( name ) ) return enumerator.Current.ToString();
        if ( str2.StartsWith( name ) && ((str == null) || (str2.Length <= str.Length)) )
          str = enumerator.Current.ToString();
      }
      if ( str == null ) str = name;
      return str;
    }

    //
    // Redéfinitions
    //

    /// <summary>
    /// Indique si la conversion est possible à partir d'un type donné.
    /// </summary>
    /// <param name="context">contexte de la conversion</param>
    /// <param name="sourceType">type source de la valeur à convertir</param>
    /// <returns>true si la conversion est possible, false sinon</returns>
    public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType ) {
      return ((sourceType == typeof( string )) || base.CanConvertFrom( context, sourceType ));
    }

    /// <summary>
    /// Conversion à partir d'un type donné
    /// </summary>
    /// <param name="context">contexte de la conversion</param>
    /// <param name="culture">culture de la conversion</param>
    /// <param name="value">valeur à convertir</param>
    /// <returns>la valeur convertie</returns>
    public override object ConvertFrom( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value ) {
      if ( value is string ) {
        string valueAsString = (string) value;
        if ( string.IsNullOrEmpty( valueAsString ) || (includeNoneAsStandardValue && valueAsString == noneAsStringValue) )
          return string.Empty;
        else
          return this.MatchFontName( (string) value, context );
      }
      return base.ConvertFrom( context, culture, value );
    }

    /// <summary>
    /// Indique si la conversion est possible vers un type donné.
    /// </summary>
    /// <param name="context">contexte de la conversion</param>
    /// <param name="destinationType">type destination de la valeur à convertir</param>
    /// <returns>true si la conversion est possible, false sinon</returns>
    public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType ) {
      return ((destinationType == typeof( InstanceDescriptor )) || base.CanConvertFrom( context, destinationType ));
    }

    /// <summary>
    /// Conversion vers un type donné
    /// </summary>
    /// <param name="context">contexte de la conversion</param>
    /// <param name="culture">culture de la conversion</param>
    /// <param name="value">valeur à convertir</param>
    /// <param name="destinationType">type destination de la valeur à convertir</param>
    /// <returns>la valeur convertie</returns>
    public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType ) {

      if ( destinationType == typeof( string ) && value is string ) {
        string valueAsString = (string) value;
        if ( includeNoneAsStandardValue && string.IsNullOrEmpty( valueAsString ) )
          return noneAsStringValue;
      }
      return base.ConvertTo( context, culture, value, destinationType );
    }

    /// <summary>
    /// Obtient la collection des valeurs standard : la liste des noms des fonts installées 
    /// </summary>
    /// <param name="context">contexte d'utilisation</param>
    /// <returns>la collection des noms des fonts installées</returns>
    public override TypeConverter.StandardValuesCollection GetStandardValues( ITypeDescriptorContext context ) {
      if ( standardValues == null ) {
        FontFamily[] families = FontFamily.Families;
        Hashtable hashtable = new Hashtable();
        for ( int i = 0 ; i < families.Length ; i++ ) {
          string name = families[ i ].Name;
          hashtable[ name.ToLower( System.Globalization.CultureInfo.InvariantCulture ) ] = name;
        }

        object[] array;
        if ( includeNoneAsStandardValue ) {
          array = new object[ hashtable.Values.Count + 1 ];
          array[ array.Length - 1 ] = string.Empty;
        }
        else
          array = new object[ hashtable.Values.Count ];

        hashtable.Values.CopyTo( array, 0 );
        Array.Sort( array, Comparer.Default );
        standardValues = new TypeConverter.StandardValuesCollection( array );
      }
      return standardValues;
    }

    /// <summary>
    /// Indique si seules les valeur standard peuvent être prises en compte
    /// </summary>
    /// <param name="context">contexte d'utilisation</param>
    /// <returns>true</returns>
    public override bool GetStandardValuesExclusive( ITypeDescriptorContext context ) {
      return true;
    }

    /// <summary>
    /// Indique si l'affichage des valeurs standard doit être pris en charge
    /// </summary>
    /// <param name="context">contexte d'utilisation</param>
    /// <returns>true</returns>
    public override bool GetStandardValuesSupported( ITypeDescriptorContext context ) {
      return true;
    }

    /// <summary>
    /// Indique si la valeur "none" est à inclure dans la liste des valeurs standard
    /// </summary>
    protected virtual bool IncludeNoneAsStandardValue {
      get { return includeNoneAsStandardValue; }
      set {
        if ( value == includeNoneAsStandardValue ) return;
        includeNoneAsStandardValue = value;
        standardValues = null;
      }
    }

    /// <summary>
    /// Obtient ou détermine la valeur "none" en tant que chaîne de caractères
    /// </summary>
    protected virtual string NoneAsStringValue {
      get { return noneAsStringValue; }
      set {
        if ( value == noneAsStringValue ) return;
        noneAsStringValue = value;
        standardValues = null;
      }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                           Convertisseur pour les noms de fonts                              //
  //                               La valeur none est "(aucun)"                                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Convertisseur de nom de font autorisant la valeur "none" libellée "(aucun)"
  /// </summary>
  public class FontNameWithNoneConverter : FontNameExConverter {

    /// <summary>
    /// Constructeur
    /// </summary>
    public FontNameWithNoneConverter() {
      IncludeNoneAsStandardValue = true;
      NoneAsStringValue = "(aucun)";
    }

    /// <summary>
    /// Indique si seules les valeur standard peuvent être prises en compte
    /// </summary>
    /// <param name="context">contexte d'utilisation</param>
    /// <returns>false</returns>
    public override bool GetStandardValuesExclusive( ITypeDescriptorContext context ) {
      return false;
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                           Convertisseur pour les noms de fonts                              //
  //                              La valeur none est "(hérité)"                                  //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Convertisseur de nom de font autorisant la valeur "none" libellée "(hérité)"
  /// </summary>
  public class FontNameWithInheritConverter : FontNameExConverter {

    /// <summary>
    /// Constructeur
    /// </summary>
    public FontNameWithInheritConverter() {
      IncludeNoneAsStandardValue = true;
      NoneAsStringValue = "(hérité)";
    }

    /// <summary>
    /// Indique si seules les valeur standard peuvent être prises en compte
    /// </summary>
    /// <param name="context">contexte d'utilisation</param>
    /// <returns>false</returns>
    public override bool GetStandardValuesExclusive( ITypeDescriptorContext context ) {
      return false;
    }
  }
}
