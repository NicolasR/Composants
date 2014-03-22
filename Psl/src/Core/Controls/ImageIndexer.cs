using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Psl.Drawing;

namespace Psl.Controls {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                     IImageListProvider                                      //
  //                        Interface pour un fournisseur de liste d'images                      //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Interface donnant accès à une liste d'images <see cref="ImageList"/>
  /// </summary>
  public interface IImageListProvider {

    /// <summary>
    /// Obtient la référence sur la liste d'images <see cref="ImageList"/>
    /// </summary>
    ImageList ImageList { get; }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                       ImageIndexer                                          //
  //          Détermination d'une image comme Image ou comme élément d'une liste d'images        //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  /// <summary>
  /// Gestion d'une image identifiée soit par une référence soit au sein d'une liste d'images
  /// </summary>
  public class ImageIndexer {

    /// <summary>
    /// Enumération des genres possibles d'une token d'image
    /// </summary>
    public enum Kind {

      /// <summary>
      /// Le token d'image est un objet de type <see cref="System.Drawing.Image"/>.
      /// </summary>
      Image,

      /// <summary>
      /// Le token d'image est un index dans une liste d'images <see cref="ImageList"/>.
      /// </summary>
      Index,

      /// <summary>
      /// Le token d'image est une clé dans une liste d'images <see cref="ImageList"/>.
      /// </summary>
      Key
    }

    // fournisseur de la liste d'images ou null
    private IImageListProvider provider;

    // liste d'images fournie directement ou null
    private ImageList imageList;

    // token associé à l'image
    private object imageToken;

    // type de contenu du token
    private Kind imageKind;

    //
    // Construction/finalisation
    //

    /// <summary>
    /// Construction avec indication d'un fournisseur d'images.
    /// </summary>
    /// <param name="provider">fournisseur d'une liste d'images</param>
    public ImageIndexer( IImageListProvider provider ) {
      this.provider = provider;
    }

    /// <summary>
    /// Constructeur sans fournisseur d'image spécifié.
    /// </summary>
    public ImageIndexer()
      : this( null ) {
    }

    //
    // Service
    //

    /// <summary>
    /// Procédure centralisée d'assignation du token d'image
    /// </summary>
    /// <param name="kind">genre du token</param>
    /// <param name="value">valeur du token</param>
    /// <returns>true si le token d'image a changé</returns>
    private bool DoSetImageToken( Kind kind, object value ) {
      bool changed = imageKind != kind || imageToken != value;

      imageKind = kind;
      imageToken = value;
      return changed;
    }

    /// <summary>
    /// Assignation d'un token avec contrôle et identification de son genre.
    /// </summary>
    /// <param name="value">valeur de token à assigner</param>
    /// <returns>true si le token d'image a changé</returns>
    private bool DoSetImageToken( object value ) {

      // préparer les arguments
      Kind kind = Kind.Image;
      object token = value;

      // récupérer le TypeCode du token
      TypeCode typeCode = Convert.GetTypeCode( token );

      // agir selon le TypeCode
      switch ( typeCode ) {

        // référence null
        case TypeCode.Empty:
          break;

        // types entiers
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.Int32:
        case TypeCode.Int64:
        case TypeCode.SByte:
        case TypeCode.UInt16:
        case TypeCode.UInt32:
        case TypeCode.UInt64:
          try {
            token = Convert.ChangeType( token, TypeCode.Int32 );
            kind = Kind.Index;
          }
          catch ( Exception ex ) {
            throw new ArithmeticException( string.Format( "Un token d'image exprimé comme un nombre doit être convertible en Int32, valeur : {0}", token ), ex );
          }
          break;

        // type chaîne
        case TypeCode.String:
          kind = Kind.Key;
          break;

        // autres types
        default:
          if ( token is Image )
            kind = Kind.Image;
          else if ( token is Icon ) {
            token = ( (Icon) token ).ToBitmap();
            kind = Kind.Image;
          }
          else
            throw new ArgumentException( string.Format( "Un token d'image doit être un entier, une chaîne, un objet Image ou un objet Icon, type : {0}", token.GetType() ) );
          break;
      }

      // déterminer l'image
      return DoSetImageToken( kind, token );
    }

    /// <summary>
    /// Obtient l'image actuelle
    /// </summary>
    /// <param name="defaultImage">image par défaut à retourner si l'image actuelle est null</param>
    /// <returns>référence sur l'image actuelle</returns>
    private Image DoGetImage( Image defaultImage ) {
      switch ( imageKind ) {
        case Kind.Index:
          int index = (int) imageToken;
          return ImageList == null || index < 0 || index >= ImageList.Images.Count ? defaultImage : ImageList.Images[ index ];
        case Kind.Key:
          string key = (string) imageToken;
          return ImageList == null || key == string.Empty ? defaultImage : ImageList.Images[ key ];
        default:
          return imageToken == null ? defaultImage : (Image) imageToken;
      }
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Obtient l'image actuelle avec possibilité d'une image par défaut
    /// </summary>
    /// <param name="defaultImage">image par défaut</param>
    /// <returns>la référence sur l'image actuelle</returns>
    public Image GetImage( Image defaultImage ) {
      return DoGetImage( defaultImage );
    }

    /// <summary>
    /// Assigne le token d'image
    /// </summary>
    /// <param name="value">valeur du token</param>
    /// <returns>true si le token d'image a changé</returns>
    public bool SetImageToken( object value ) {
      return DoSetImageToken( value );
    }

    /// <summary>
    /// Assignation d'un token avec identification de son genre.
    /// </summary>
    /// <param name="kind">genre du token</param>
    /// <param name="value">valeur de token à assigner</param>
    /// <returns>true si le token d'image a changé</returns>
    public bool SetImageToken( Kind kind, object value ) {
      return DoSetImageToken( kind, value );
    }

    //
    // Propriétés exposées
    //

    /// <summary>
    /// Obtient ou détermine la liste d'images associée au token d'image.
    /// </summary>
    public virtual ImageList ImageList {
      get { return imageList == null ? provider == null ? null : provider.ImageList : imageList; }
      set { imageList = value; }
    }

    /// <summary>
    /// Obtient l'image actuelle associée au token d'image.
    /// </summary>
    public Image Image {
      get { return DoGetImage( null ); }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image.
    /// </summary>
    public object ImageToken {
      get { return imageToken; }
      set { DoSetImageToken( value ); }
    }

    /// <summary>
    /// Obtient ou détermine l'image comme un objet de type <see cref="Image"/>.
    /// </summary>
    public Image ImageObject {
      get { return imageKind == Kind.Image ? (Image) imageToken : null; }
      set { DoSetImageToken( Kind.Image, value ); }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image comme un index dans la liste d'images <see cref="ImageList"/>.
    /// </summary>
    public int ImageIndex {
      get { return imageKind == Kind.Index ? (int) imageToken : -1; }
      set { DoSetImageToken( Kind.Index, value ); }
    }

    /// <summary>
    /// Obtient ou détermine le token d'image comme une clé dans la liste d'images <see cref="ImageList"/>.
    /// </summary>
    public string ImageKey {
      get { return imageKind == Kind.Key ? (string) imageToken : string.Empty; }
      set { DoSetImageToken( Kind.Key, value ); }
    }
  }
}
