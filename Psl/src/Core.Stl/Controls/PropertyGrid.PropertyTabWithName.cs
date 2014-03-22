/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Credit : Microsoft
 * Onglet dérivé de l'exemple associé à la documentation de l'énumération PropertyTabScope
 * 
 * 24 10 2010 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Psl.Tools;

namespace Psl.Controls {

  /// <summary>
  /// Onglet de composant <see cref="PropertyGrid"/> avec énumération de la propriété <see cref="Control.Name"/> des contrôles.
  /// </summary>
  /// <remarks>
  /// Cet onglet doit être ajouté par programme à la collection <see cref="PropertyGrid.PropertyTabs"/> d'un composant <see cref="PropertyGrid"/> : 
  /// <example>
  /// <code>
  /// properties.PropertyTabs.AddTabType( typeof( PropertyTabWithName ), PropertyTabScope.Document ) ;
  /// </code>
  /// La collection de descripteurs de propriétés incluant le descripteur de la propriété Name peut être obtenu 
  /// en invoquant la méthode <see cref="PropertyTab.GetProperties(object)"/> de l'onglet. 
  /// </example>
  /// </remarks>
  [System.Security.Permissions.PermissionSet( System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust" )]
  public class PropertyTabWithName : PropertyTab {

    // image de l'onglet sérialisée en Base64
    private const string tabImageBase64 = "AAEAAAD/////AQAAAAAAAAAMAgAAAFFTeXN0ZW0uRHJhd2luZywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWIwM2Y1ZjdmMTFkNTBhM2EFAQAAABVTeXN0ZW0uRHJhd2luZy5CaXRtYXABAAAABERhdGEHAgIAAAAJAwAAAA8DAAAANgUAAAJCTTYFAAAAAAAANgQAACgAAAAQAAAAEAAAAAEACAAAAAAAAAAAAMQOAADEDgAAAAEAAAABAABjMDH/azQx/2s0Of9CQUL/SkVK/4kUAP+lGAD/nJpr/5yea/+Mhoz/jIqM/7Wytf+1trX/vba9/86anP/Wmpz/96GR/9aepf/Oz5z/1s+c/9bPpf/3z63/wMDA///Pzv//z9b//+/O///v1v/v6+//9/P3///3////+////////wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/AAAA/wAAAP8AAAD/FhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhAFFhYFEBYWFhYWFhYWFhYFFhYWFgUWFgcCAQIBAgECBQIBAhYFFhYPHBwbGhkaFwUXFQAWBRYWDh0DBBsKCQoFChcCFgUWFgwcHBwcGRoZBRcYABYFFhYOHgMEHAoJChAFFwIGEBYWDx4eHBwcHBsaGRoAFhYWFgsfAwQcCgkKCQoZAhYWFhYMHx8eHhwcHBwbGgAWFhYWEh8DBB4KCQoJChsCFhYWFhMfHx8fHh4cHBwcABYWFhYSFBITCxEODQ4RDggWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFhYWFgs=";

    // filtre par défaut pour les propriétés
    private static Attribute[] DefaultFilter = new Attribute[] { BrowsableAttribute.Yes };

    /// <summary>
    /// Retourne la collection des propriétés à afficher pour l'objet
    /// </summary>
    /// <remarks>
    /// Cette méthode ajoute le descripteur de la propriété "Name" uniquement pour les objets qui sont au moins <see cref="Control"/>.
    /// </remarks>
    /// <param name="component">objet dont les propriétés sont à afficher</param>
    /// <param name="attributes">null ou tableau de filtrage selon les attributs</param>
    /// <returns>la collection des descripteurs des propriétés à afficher</returns>
    public override PropertyDescriptorCollection GetProperties( object component, System.Attribute[] attributes ) {
      PropertyDescriptorCollection properties;

      // récupérer le propriétés à partir du descripteur de type
      if ( attributes == null ) attributes = DefaultFilter ;
      properties = TypeDescriptor.GetProperties( component, attributes );

      // l'objet dont les propriétés sont à afficher n'est pas Control --> rien d'autre à faire
      if (! (component is Control) ) return properties;

      // si la propriété Name figure déjà dans la collection --> rien d'autre à faire
      PropertyDescriptor oldDescriptorOfName = properties.Find( "Name", false ) ;
      if (oldDescriptorOfName != null) return properties ;

      // rechercher le descripteur de la propriété Name dans la collection de tous les descripteurs de propriétés
      PropertyDescriptorCollection allProperties = TypeDescriptor.GetProperties( component );
      oldDescriptorOfName = allProperties.Find( "Name", false );
      if ( oldDescriptorOfName == null ) return properties;

      // composer le nouveau descripteur de la propriété Name
      PropertyDescriptor newDescriptorOfName = TypeDescriptor.CreateProperty( oldDescriptorOfName.ComponentType, oldDescriptorOfName, new ParenthesizePropertyNameAttribute( true ), CategoryAttribute.Design );
      //PropertyDescriptor newDescriptorOfName = TypeDescriptor.CreateProperty( oldDescriptorOfName.ComponentType, oldDescriptorOfName, BrowsableAttribute.Yes, new ParenthesizePropertyNameAttribute( true ), CategoryAttribute.Design );
      
      // composer la collection des descripteurs de propriétés obtenu en ajoutant le descripteur de la propriété Name
      PropertyDescriptor[] propertiesArray = new PropertyDescriptor[ properties.Count ];
      properties.CopyTo( propertiesArray, 0 );
      properties = new PropertyDescriptorCollection( propertiesArray );
      properties.Add( newDescriptorOfName );
      return properties;
    }

    /// <summary>
    /// Retourne la collection de toutes les propriétés de l'objet sans aucun filtrage.
    /// </summary>
    /// <param name="component">objet dont les propriétés sont obtenir</param>
    /// <returns>la collection des descripteurs de toutes les propriétés</returns>
    public override System.ComponentModel.PropertyDescriptorCollection GetProperties( object component ) {
      return this.GetProperties( component, null );
    }

    /// <summary>
    /// Obtient le libellé de l'onglet.
    /// </summary>
    public override string TabName {
      get { return "Propriétés (avec Name)"; }
    }

    /// <summary>
    /// Obtient l'image associée à l'onglet.
    /// </summary>
    public override Bitmap Bitmap {
      get { return new Bitmap( ImageHelper.Base64StringToImage( tabImageBase64 ) ); }
    }
  }
}

