/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * *
 * Crédits : 
 * Jeroen Landheer pour Code Project 
 * Microsoft Corp.
 * 
 * 10 12 2008 : version initiale
 * 12 01 2010 : version complète
 */                                                                            // <wao never.end>

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                Interface IWebBrowser2                                       //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  public static partial class NativeMethods {

    /// <summary>
    /// Filtrage des exceptions systématiques de IE
    /// </summary>
    /// <param name="exception">objet exception à filtrer</param>
    /// <returns>true si l'objet exception n'est pas une exception systématique</returns>
    public static bool IsNotIEStandardException( Exception exception ) {

      COMException asCOMException = exception as COMException;
      if ( asCOMException != null && (uint) asCOMException.ErrorCode == 0x80040100 ) return false;

      return true;
    }

    /// <summary>
    /// Résultats retournés par certaines opérations accessibles via IWebBrowser2.ExecWB
    /// </summary>
    public enum OLECMDF {
      OLECMDF_DEFHIDEONCTXTMENU = 0x20,
      OLECMDF_ENABLED = 0x02,
      OLECMDF_INVISIBLE = 0x10,
      OLECMDF_LATCHED = 0x04,
      OLECMDF_NINCHED = 0x08,
      OLECMDF_SUPPORTED = 0x01
    }

    /// <summary>
    /// Options pour certaines opérations exposées par IWebBrowser2.ExecWB
    /// </summary>
    public enum OLECMDEXECOPT {
      OLECMDEXECOPT_DODEFAULT = 0,
      OLECMDEXECOPT_DONTPROMPTUSER = 2,
      OLECMDEXECOPT_PROMPTUSER = 1,
      OLECMDEXECOPT_SHOWHELP = 3
    }

    /// <summary>
    /// Code des opérations accessibles via IWebBrowser2.ExecWB 
    /// </summary>
    public enum OLECMDID {
      OLECMDID_OPEN = 1,
      OLECMDID_NEW = 2,
      OLECMDID_SAVE = 3,
      OLECMDID_SAVEAS = 4,
      OLECMDID_SAVECOPYAS = 5,
      OLECMDID_PRINT = 6,
      OLECMDID_PRINTPREVIEW = 7,
      OLECMDID_PAGESETUP = 8,
      OLECMDID_SPELL = 9,
      OLECMDID_PROPERTIES = 10,
      OLECMDID_CUT = 11,
      OLECMDID_COPY = 12,
      OLECMDID_PASTE = 13,
      OLECMDID_PASTESPECIAL = 14,
      OLECMDID_UNDO = 15,
      OLECMDID_REDO = 16,
      OLECMDID_SELECTALL = 17,
      OLECMDID_CLEARSELECTION = 18,
      OLECMDID_ZOOM = 19,
      OLECMDID_GETZOOMRANGE = 20,
      OLECMDID_UPDATECOMMANDS = 21,
      OLECMDID_REFRESH = 22,
      OLECMDID_STOP = 23,
      OLECMDID_HIDETOOLBARS = 24,
      OLECMDID_SETPROGRESSMAX = 25,
      OLECMDID_SETPROGRESSPOS = 26,
      OLECMDID_SETPROGRESSTEXT = 27,
      OLECMDID_SETTITLE = 28,
      OLECMDID_SETDOWNLOADSTATE = 29,
      OLECMDID_STOPDOWNLOAD = 30,
      OLECMDID_FIND = 32,
      OLECMDID_DELETE = 33,
      OLECMDID_PRINT2 = 49,
      OLECMDID_PRINTPREVIEW2 = 50,
      OLECMDID_PAGEACTIONBLOCKED = 55,
      OLECMDID_PAGEACTIONUIQUERY = 56,
      OLECMDID_FOCUSVIEWCONTROLS = 57,
      OLECMDID_FOCUSVIEWCONTROLSQUERY = 58,
      OLECMDID_SHOWPAGEACTIONMENU = 59,
      OLECMDID_ADDTRAVELENTRY = 60,
      OLECMDID_UPDATETRAVELENTRY = 61,
      OLECMDID_UPDATEBACKFORWARDSTATE = 62,
      OLECMDID_OPTICAL_ZOOM = 63,
      OLECMDID_OPTICAL_GETZOOMRANGE = 64,
      OLECMDID_WINDOWSTATECHANGED = 65,
      OLECMDID_ACTIVEXINSTALLSCOPE = 66,
      OLECMDID_UPDATETRAVELENTRY_DATARECOVERY = 67
      // OLECMDID_SHOWSCRIPTERROR = 40
    }

    /// <summary>
    /// Interface des événements DWebBrowser2 pour <see cref="Psl.Controls.WebBrowserEx"/>.
    /// </summary>
    /// <remarks>
    /// Cette interface est motivée par un souci de compatibilité avec les anciennes installations
    /// antérieures à Windows XP SP2 qui ne déclenchent pas l'événement NewWindow3 muni d'informations complémentaires.
    /// L'événement NewWindow2 est suffisant pour permettre la navigation multi-onglets.
    /// </remarks>
    [ComImport, TypeLibType( (short) 0x1010 ), InterfaceType( (short) 2 ), Guid( "34A715A0-6587-11D0-924A-0020AFC7AC4D" )]
    public interface DWebBrowserEvents2 {

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x66 )]
      void StatusTextChange( [In, MarshalAs( UnmanagedType.BStr )] string Text );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x6c )]
      void ProgressChange( [In] int Progress, [In] int ProgressMax );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x69 )]
      void CommandStateChange( [In] int Command, [In] bool Enable );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x6a )]
      void DownloadBegin();

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x68 )]
      void DownloadComplete();

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x71 )]
      void TitleChange( [In, MarshalAs( UnmanagedType.BStr )] string Text );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x70 )]
      void PropertyChange( [In, MarshalAs( UnmanagedType.BStr )] string szProperty );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 250 )]
      void BeforeNavigate2( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object URL, [In, MarshalAs( UnmanagedType.Struct )] ref object Flags, [In, MarshalAs( UnmanagedType.Struct )] ref object TargetFrameName, [In, MarshalAs( UnmanagedType.Struct )] ref object PostData, [In, MarshalAs( UnmanagedType.Struct )] ref object Headers, [In, Out] ref bool Cancel );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xfb )]
      void NewWindow2( [In, Out, MarshalAs( UnmanagedType.IDispatch )] ref object ppDisp, [In, Out] ref bool Cancel );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xfc )]
      void NavigateComplete2( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object URL );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x103 )]
      void DocumentComplete( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object URL );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xfd )]
      void OnQuit();

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xfe )]
      void OnVisible( [In] bool Visible );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xff )]
      void OnToolBar( [In] bool ToolBar );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x100 )]
      void OnMenuBar( [In] bool MenuBar );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x101 )]
      void OnStatusBar( [In] bool StatusBar );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x102 )]
      void OnFullScreen( [In] bool FullScreen );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 260 )]
      void OnTheaterMode( [In] bool TheaterMode );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x106 )]
      void WindowSetResizable( [In] bool Resizable );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x108 )]
      void WindowSetLeft( [In] int Left );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x109 )]
      void WindowSetTop( [In] int Top );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x10a )]
      void WindowSetWidth( [In] int Width );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x10b )]
      void WindowSetHeight( [In] int Height );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x107 )]
      void WindowClosing( [In] bool IsChildWindow, [In, Out] ref bool Cancel );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x10c )]
      void ClientToHostWindow( [In, Out] ref int CX, [In, Out] ref int CY );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x10d )]
      void SetSecureLockIcon( [In] int SecureLockIcon );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 270 )]
      void FileDownload( [In, Out] ref bool Cancel );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x10f )]
      void NavigateError( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object URL, [In, MarshalAs( UnmanagedType.Struct )] ref object Frame, [In, MarshalAs( UnmanagedType.Struct )] ref object StatusCode, [In, Out] ref bool Cancel );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xe1 )]
      void PrintTemplateInstantiation( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xe2 )]
      void PrintTemplateTeardown( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0xe3 )]
      void UpdatePageStatus( [In, MarshalAs( UnmanagedType.IDispatch )] object pDisp, [In, MarshalAs( UnmanagedType.Struct )] ref object nPage, [In, MarshalAs( UnmanagedType.Struct )] ref object fDone );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x110 )]
      void PrivacyImpactedStateChange( [In] bool bImpacted );

      [PreserveSig, MethodImpl( MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime ), DispId( 0x111 )]
      void NewWindow3( [In, Out, MarshalAs( UnmanagedType.IDispatch )] ref object ppDisp, [In, Out] ref bool Cancel, [In] uint dwFlags, [In, MarshalAs( UnmanagedType.BStr )] string bstrUrlContext, [In, MarshalAs( UnmanagedType.BStr )] string bstrUrl );
    }

    /// <summary>
    /// Interface du contrôle ActiveX associé au navigateur web
    /// </summary>
    /// <remarks>
    /// La reproduction de cette interface est due au fait qu'elle est déclarée dans une classe inacessible.
    /// <br/>
    /// L'odre des méthodes doit être conservé. On pourrait se permettre de ne pas déclarer les méthodes ou propriétés
    /// qui viennent après la dernière méthode ou propriété effectivement utilisée. 
    /// </remarks>
    [ComImport, SuppressUnmanagedCodeSecurity, TypeLibType( TypeLibTypeFlags.FOleAutomation | (TypeLibTypeFlags.FDual | TypeLibTypeFlags.FHidden) ), Guid( "D30C1661-CDAF-11d0-8A3E-00C04FC9E26E" )]
    public interface IWebBrowser2 {

      [DispId( 100 )]
      void GoBack();

      [DispId( 0x65 )]
      void GoForward();

      [DispId( 0x66 )]
      void GoHome();

      [DispId( 0x67 )]
      void GoSearch();

      [DispId( 0x68 )]
      void Navigate( [In] string Url, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers );

      [DispId( -550 )]
      void Refresh();

      [DispId( 0x69 )]
      void Refresh2( [In] ref object level );

      [DispId( 0x6a )]
      void Stop();

      [DispId( 200 )]
      object Application { [return: MarshalAs( UnmanagedType.IDispatch )] get; }

      [DispId( 0xc9 )]
      object Parent { [return: MarshalAs( UnmanagedType.IDispatch )] get; }

      [DispId( 0xca )]
      object Container { [return: MarshalAs( UnmanagedType.IDispatch )] get; }

      [DispId( 0xcb )]
      object Document { [return: MarshalAs( UnmanagedType.IDispatch )] get; }

      [DispId( 0xcc )]
      bool TopLevelContainer { get; }

      [DispId( 0xcd )]
      string Type { get; }

      [DispId( 0xce )]
      int Left { get; set; }

      [DispId( 0xcf )]
      int Top { get; set; }

      [DispId( 0xd0 )]
      int Width { get; set; }

      [DispId( 0xd1 )]
      int Height { get; set; }

      [DispId( 210 )]
      string LocationName { get; }

      [DispId( 0xd3 )]
      string LocationURL { get; }

      [DispId( 0xd4 )]
      bool Busy { get; }

      [DispId( 300 )]
      void Quit();

      [DispId( 0x12d )]
      void ClientToWindow( out int pcx, out int pcy );

      [DispId( 0x12e )]
      void PutProperty( [In] string property, [In] object vtValue );

      [DispId( 0x12f )]
      object GetProperty( [In] string property );

      [DispId( 0 )]
      string Name { get; }

      [DispId( -515 )]
      int HWND { get; }

      [DispId( 400 )]
      string FullName { get; }

      [DispId( 0x191 )]
      string Path { get; }

      [DispId( 0x192 )]
      bool Visible { get; set; }

      [DispId( 0x193 )]
      bool StatusBar { get; set; }

      [DispId( 0x194 )]
      string StatusText { get; set; }

      [DispId( 0x195 )]
      int ToolBar { get; set; }

      [DispId( 0x196 )]
      bool MenuBar { get; set; }

      [DispId( 0x197 )]
      bool FullScreen { get; set; }

      [DispId( 500 )]
      void Navigate2( [In] ref object URL, [In] ref object flags, [In] ref object targetFrameName, [In] ref object postData, [In] ref object headers );

      [DispId( 0x1f5 )]
      NativeMethods.OLECMDF QueryStatusWB( [In] NativeMethods.OLECMDID cmdID );

      [DispId( 0x1f6 )]
      void ExecWB( [In] NativeMethods.OLECMDID cmdID, [In] NativeMethods.OLECMDEXECOPT cmdexecopt, ref object pvaIn, ref object pvaOut );

      [DispId( 0x1f7 )]
      void ShowBrowserBar( [In] ref object pvaClsid, [In] ref object pvarShow, [In] ref object pvarSize );

      [DispId( -525 )]
      WebBrowserReadyState ReadyState { get; }

      [DispId( 550 )]
      bool Offline { get; set; }

      [DispId( 0x227 )]
      bool Silent { get; set; }

      [DispId( 0x228 )]
      bool RegisterAsBrowser { get; set; }

      [DispId( 0x229 )]
      bool RegisterAsDropTarget { get; set; }

      [DispId( 0x22a )]
      bool TheaterMode { get; set; }

      [DispId( 0x22b )]
      bool AddressBar { get; set; }

      [DispId( 0x22c )]
      bool Resizable { get; set; }
    }
  }

  /////////////////////////////////////////////////////////////////////////////////////////////////
  //                                                                                             //
  //                                Interface IOleCommandTarget                                  //
  //                           http://support.microsoft.com/kb/329014/en-us                      //
  //                                                                                             //
  /////////////////////////////////////////////////////////////////////////////////////////////////

  public static partial class NativeMethods {

    [
     StructLayout( LayoutKind.Sequential, CharSet = CharSet.Unicode )
    ]
    public struct OLECMDTEXT {
      public uint cmdtextf;
      public uint cwActual;
      public uint cwBuf;
      [MarshalAs( UnmanagedType.ByValTStr, SizeConst = 100 )]
      public char rgwz;
    }

    [
     StructLayout( LayoutKind.Sequential )
    ]
    public struct OLECMD {
      public uint cmdID;
      public uint cmdf;
    }

    // conserver l'ordre des méthodes dans la déclaration de l'interface
    [
     ComImport,
     Guid( "b722bccb-4e68-101b-a2bc-00aa00404770" ),
     InterfaceType( ComInterfaceType.InterfaceIsIUnknown )
    ]
    public interface IOleCommandTarget {
      void QueryStatus( ref Guid pguidCmdGroup, UInt32 cCmds, [MarshalAs( UnmanagedType.LPArray, SizeParamIndex = 1 )] OLECMD[] prgCmds, ref OLECMDTEXT CmdText );
      void Exec( ref Guid pguidCmdGroup, uint nCmdId, uint nCmdExecOpt, ref object pvaIn, ref object pvaOut );
    }

    //
    // Commandes complémentaires pour WebBrowser
    //

    // Warning This sample uses an undocumented command-group GUID that is subject to change in the future. 
    // Although this sample was tested to work correctly with Internet Explorer 6 and earlier, 
    // there is no guarantee that these techniques will continue to work successfully in future versions
    public static Guid ExtraWebBrowserOleCommandGuid = new Guid( "ED016940-BD5B-11CF-BA4E-00C04FD70816" );

    public enum ExtraWebBrowserOleCommand {
      Find = 1,
      ViewSource,
      Options
    }

  }

}
