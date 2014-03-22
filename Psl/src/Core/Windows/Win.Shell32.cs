/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 12 12 2008 : version initiale
 * 04 04 2009 : adjonction de l'accès à ShellExecute
 */                                                                            // <wao never.end>

using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  public partial class Shell32 {

		public const int 	MAX_PATH = 256;

		[StructLayout(LayoutKind.Sequential)]
		public struct SHITEMID {
			public ushort cb;
			[MarshalAs(UnmanagedType.LPArray)]
			public byte[] abID;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct ITEMIDLIST {
			public SHITEMID mkid;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct BROWSEINFO { 
			public IntPtr		hwndOwner; 
			public IntPtr		pidlRoot; 
			public IntPtr 	pszDisplayName;
			[MarshalAs(UnmanagedType.LPTStr)] 
			public string 	lpszTitle; 
			public uint 		ulFlags; 
			public IntPtr		lpfn; 
			public int			lParam; 
			public IntPtr 	iImage; 
		} 

		// Browsing for directory.
		public const uint BIF_RETURNONLYFSDIRS   =	0x0001;
		public const uint BIF_DONTGOBELOWDOMAIN  =	0x0002;
		public const uint BIF_STATUSTEXT         =	0x0004;
		public const uint BIF_RETURNFSANCESTORS  =	0x0008;
		public const uint BIF_EDITBOX            =	0x0010;
		public const uint BIF_VALIDATE           =	0x0020;
		public const uint BIF_NEWDIALOGSTYLE     =	0x0040;
		public const uint BIF_USENEWUI           =	(BIF_NEWDIALOGSTYLE | BIF_EDITBOX);
		public const uint BIF_BROWSEINCLUDEURLS  =	0x0080;
		public const uint BIF_BROWSEFORCOMPUTER  =	0x1000;
		public const uint BIF_BROWSEFORPRINTER   =	0x2000;
		public const uint BIF_BROWSEINCLUDEFILES =	0x4000;
		public const uint BIF_SHAREABLE          =	0x8000;

		[StructLayout(LayoutKind.Sequential)]
		public struct SHFILEINFO { 
			public const int NAMESIZE = 80;
			public IntPtr	hIcon; 
			public int		iIcon; 
			public uint	dwAttributes; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=MAX_PATH)]
			public string szDisplayName; 
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=NAMESIZE)]
			public string szTypeName; 
		}

		public const uint SHGFI_ICON				        = 0x000000100;     // get icon
		public const uint SHGFI_DISPLAYNAME			    = 0x000000200;     // get display name
		public const uint SHGFI_TYPENAME          	= 0x000000400;     // get type name
		public const uint SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
		public const uint SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
		public const uint SHGFI_EXETYPE           	= 0x000002000;     // return exe type
		public const uint SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
		public const uint SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
		public const uint SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
		public const uint SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
		public const uint SHGFI_LARGEICON         	= 0x000000000;     // get large icon
		public const uint SHGFI_SMALLICON         	= 0x000000001;     // get small icon
		public const uint SHGFI_OPENICON          	= 0x000000002;     // get open icon
		public const uint SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
		public const uint SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
		public const uint SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
		public const uint SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
		public const uint SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay

		public const uint FILE_ATTRIBUTE_DIRECTORY  = 0x00000010;  
		public const uint FILE_ATTRIBUTE_NORMAL     = 0x00000080;  

		[DllImport("Shell32.dll")]
		public static extern IntPtr SHGetFileInfo( string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo,	uint uFlags );

    public static bool GetItemInfo( string path, uint attribute, out Shell32.SHFILEINFO infos, uint flags ) {
      infos = new SHFILEINFO();
      IntPtr result = SHGetFileInfo( path, attribute, ref infos, (uint) Marshal.SizeOf( infos ), flags );
      return (int) result != 0;
    }

    public static IntPtr GetItemIconHandle( string path, bool imageLarge, bool linkOverlay, bool isFolder, bool asFolderOpened ) {
      Shell32.SHFILEINFO infos;

      uint attribute = isFolder ? FILE_ATTRIBUTE_DIRECTORY : FILE_ATTRIBUTE_NORMAL;
      uint flags = SHGFI_ICON | SHGFI_USEFILEATTRIBUTES;
      flags += imageLarge ? SHGFI_LARGEICON : SHGFI_SMALLICON ;
      if ( linkOverlay ) flags += SHGFI_LINKOVERLAY;
      if ( isFolder && asFolderOpened ) flags += SHGFI_OPENICON;

      bool result = GetItemInfo( path, attribute, out infos, flags );
      if ( !result ) return IntPtr.Zero;

      return infos.hIcon;
    }


    public const int SW_HIDE             = 0;
    public const int SW_SHOWNORMAL       = 1;
    public const int SW_NORMAL           = 1;
    public const int SW_SHOWMINIMIZED    = 2;
    public const int SW_SHOWMAXIMIZED    = 3;
    public const int SW_MAXIMIZE         = 3;
    public const int SW_SHOWNOACTIVATE   = 4;
    public const int SW_SHOW             = 5;
    public const int SW_MINIMIZE         = 6;
    public const int SW_SHOWMINNOACTIVE  = 7;
    public const int SW_SHOWNA           = 8;
    public const int SW_RESTORE          = 9;
    public const int SW_SHOWDEFAULT      = 10;
    public const int SW_FORCEMINIMIZE    = 11;
    public const int SW_MAX              = 11;

    [DllImport("shell32.dll", SetLastError=true)]
    public static extern int ShellExecute(int hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

    public static void ShellOpen( string fileName, int show ) {
      ShellExecute( 0, "open", fileName, string.Empty, string.Empty, show);
    }

  }
}
