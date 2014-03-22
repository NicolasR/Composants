/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 22 01 2009 : version initiale
 * 28 05 2010 : modification de HexaStringToImage pour les exceptions dans Bitmap.MakeTransparent
 */                                                                            // <wao never.end>

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Psl.Tools {

  /// <summary>
  /// Classe de service proposant quelques fonctionnalités relatives aux images
  /// </summary>
  public class ImageHelper : IconHelper {

    /// <summary>
    /// Convertit un byte (dont les 4 bits de poids fort sont 0) en un chiffre hexadécimal majuscule
    /// </summary>
    /// <param name="value">byte à convertir</param>
    /// <returns>le chiffre hexa obtenu</returns>
    public static char ByteToHexaDigit( byte value )  {
      return (char) ((value <= 9 ? ((int) '0') : ((int) 'A') - 10) + value); 
    }

    /// <summary>
    /// Convertit un chiffre hexadécimal majuscule en sa valeur binaire
    /// </summary>
    /// <param name="digit">chiffre hexadécimal à convertir</param>
    /// <returns>la valeur binaire obtenue</returns>
    public static byte HexaDigitToByte( char digit ) {
      if ( digit >  'F' ) throw new ArgumentException( "Le caractère n'est pas un chiffre hexa", "digit" );
      if ( digit >= 'A' ) return (byte) (digit - (int) 'A' + 10);
      if ( digit >  '9' ) throw new ArgumentException( "Le caractère n'est pas un chiffre hexa", "digit" );
      if ( digit >= '0' ) return (byte) (digit - (int) '0');
      throw new ArgumentException( "Le caractère n'est pas un chiffre hexa", "digit" );
    }

    /// <summary>
    /// Convertit un tableau de bytes en une chaîne de chiffres hexadécimaux
    /// </summary>
    /// <param name="bytes">tableau de bytes à convertir</param>
    /// <returns>la suite de chiffres haxadécimaux obtenue</returns>
    public static string BytesToHexaDigits( byte[] bytes ) {
      StringBuilder data = new StringBuilder( 2 * bytes.Length );
      for ( int ix = 0 ; ix < bytes.Length ; ix++ ) {
        byte value = bytes[ ix ];
        char hiChar = ByteToHexaDigit( (byte) ((value & 0xF0) >> 4) );
        char loChar = ByteToHexaDigit( (byte)  (value & 0x0F)       );
        data.Append( hiChar );
        data.Append( loChar );
      }
      return data.ToString();
    }

    /// <summary>
    /// Convertit une suite de chiffres hexasécimaux en un tableau de bytes
    /// </summary>
    /// <param name="hexa">suite de chiffres hexadécimaux à convertir</param>
    /// <returns>le tableau de bytes obtenu</returns>
    public static byte[] HexaDigitsToBytes( string hexa ) {
      if ( hexa.Length % 2 != 0 ) throw new ArgumentException( "Un codage hexa doit comporter un nompbre pair de caractères", "hexa" );
      byte[] result = new byte[ hexa.Length / 2 ];
      int iy = 0;
      for ( int ix = 0 ; ix < hexa.Length - 1 ; ix += 2 ) {
        int hiByte = HexaDigitToByte( hexa[ ix     ] );
        int loByte = HexaDigitToByte( hexa[ ix + 1 ] );
        result[ iy ] = (byte) ((hiByte << 4) + loByte);
        iy++;
      }
      return result;
    }

    /// <summary>
    /// Convertit une image en une image png convertie en une chaîne de chiffres hexadécimaux
    /// </summary>
    /// <param name="image">image à convertir</param>
    /// <returns>la suite de chiffres hexadécimaux obtenue</returns>
    public static string ImageToHexaString( Image image ) {
      string result;
      using ( MemoryStream stream = new MemoryStream() ) {
        image.Save( stream, ImageFormat.Png );
        result = BytesToHexaDigits( stream.ToArray() );
      }
      return result;
    }

    /// <summary>
    /// Convertit une suite de chiffres hexadécimaux en une image
    /// </summary>
    /// <remarks>
    /// La création de l'image en deux temps est destinée à éviter que la méthode
    /// <see cref="Bitmap.MakeTransparent()"/> ne déclenche une exception indiquant
    /// qu'une stream fermée ne peut être lue. 
    /// </remarks>
    /// <param name="value">suite de chiffres hexadécimaux à convertir</param>
    /// <returns>l'image obtenue</returns>
    public static Image HexaStringToImage( string value ) {
      Image result;
      byte[] bytes = HexaDigitsToBytes( value );

      using ( MemoryStream stream = new MemoryStream( bytes ) ) {
        using ( Image image = Image.FromStream( stream ) ) {
          result = new Bitmap( image );
        }
      }

      return result;
    }

    /// <summary>
    /// sérialise une image en une chaîne Base64
    /// </summary>
    /// <param name="image">image à sérialiser</param>
    /// <returns>la chaîne obtenue par sérialisation de l'image en Base64</returns>
    public static string ImageToBase64String( Image image ) {
      string result;
      IFormatter formatter = new BinaryFormatter();
      using ( MemoryStream stream = new MemoryStream() ) {
        formatter.Serialize( stream, image );
        stream.Position = 0;
        byte[] bytes = new byte[ stream.Length ] ;
        stream.Read( bytes, 0, (int) stream.Length ) ;
        result = Convert.ToBase64String( bytes );
        stream.Close();
      }
      return result;
    }

    /// <summary>
    /// Désérialise une chaîne Base64 comme une image de type <see cref="Image"/>.
    /// </summary>
    /// <param name="value">la chaîne codée en Base64</param>
    /// <returns>l'image obtenue par désérialisation</returns>
    public static Image Base64StringToImage( string value ) {
      Image image = null;
      byte[] bytes = Convert.FromBase64String( value );
      IFormatter formatter = new BinaryFormatter();
      using ( MemoryStream stream = new MemoryStream( bytes ) ) {
        image = (Image) formatter.Deserialize( stream );
        stream.Close();
      }
      return image;
    }

  }
}
