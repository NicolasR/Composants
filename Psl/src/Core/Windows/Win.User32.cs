/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Text;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  // Regroupement de déclarations liées aux méthodes natives Windows
  partial class Win {

    //
    // Général
    //

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.I4 )]
    public static extern int SendMessage( IntPtr hwnd, int msg, int wparam, int lparam );

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.I4 )]
    public static extern int PostMessage( IntPtr hwnd, int msg, int wparam, int lparam );

    //
    // Coordonnées
    //

    [StructLayout( LayoutKind.Sequential )]
    public struct RECT {
      public int left;
      public int top;
      public int right;
      public int bottom;

      public RECT( int left, int top, int right, int bottom ) {
        this.left = left;
        this.top = top;
        this.right = right;
        this.bottom = bottom;
      }

      public RECT( Rectangle r ) {
        this.left = r.Left;
        this.top = r.Top;
        this.right = r.Right;
        this.bottom = r.Bottom;
      }

      public static RECT FromXYWH( int x, int y, int width, int height ) {
        return new RECT( x, y, x + width, y + height );
      }

      public Size Size {
        get { return new Size( this.right - this.left, this.bottom - this.top ); }
      }
    }

    //
    // Gestion des fenêtres
    //

    public const uint WS_VISIBLE = 0x10000000;
    public static readonly IntPtr HWND_MESSAGE = new IntPtr( -3 );

    public delegate bool EnumWindowsCallBack( IntPtr hWnd, int lParam );

    [DllImport( "user32.dll" )]
    public static extern IntPtr CreateWindowEx( uint dwExStyle, string lpClassName, string lpWindowName, uint dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam );

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static extern bool DestroyWindow( IntPtr hWnd );

    [DllImport( "user32.dll" )]
    public static extern IntPtr WindowFromPoint( Point point );

    [DllImport( "user32.dll" )]
    public static extern bool EnumChildWindows( HandleRef hWndParent, EnumWindowsCallBack lpEnumFunc, int lParam );

    [DllImport( "User32.Dll" )]
    public static extern void GetClassName( HandleRef hWnd, StringBuilder param, int length );

    [DllImport( "user32.dll" )]
    public static extern IntPtr SetParent( HandleRef hRefChild, HandleRef hRefNewParent );

    [DllImport( "user32.dll" )]
    public static extern IntPtr SetParent( IntPtr hWndChild, IntPtr hWndNewParent );

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static extern bool GetWindowRect( HandleRef hwnd, ref RECT rect );

    [DllImport( "user32.dll" )]
    [return: MarshalAs( UnmanagedType.Bool )]
    public static extern bool GetClientRect( HandleRef hwnd, ref RECT rect );

    // constantes pour SetWindowPos
    public const int SWP_NOSIZE = 0x0001;
    public const int SWP_NOMOVE = 0x0002;
    public const int SWP_NOZORDER = 0x0004;
    public const int SWP_NOREDRAW = 0x0008;
    public const int SWP_NOACTIVATE = 0x0010;
    public const int SWP_FRAMECHANGED = 0x0020;
    public const int SWP_SHOWWINDOW = 0x0040;
    public const int SWP_HIDEWINDOW = 0x0080;
    public const int SWP_NOCOPYBITS = 0x0100;
    public const int SWP_NOOWNERZORDER = 0x0200;
    public const int SWP_NOSENDCHANGING = 0x0400;
    public const int SWP_DRAWFRAME = 0x0020;
    public const int SWP_NOREPOSITION = 0x0200;
    public const int SWP_DEFERERASE = 0x2000;
    public const int SWP_ASYNCWINDOWPOS = 0x400;

    public enum ZOrderPos {
      HWND_TOP = 0,
      HWND_BOTTOM = 1,
      HWND_TOPMOST = -1,
      HWND_NOTOPMOST = -2
    }

    [DllImport( "user32.dll" )]
    public static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int Width, int Height, int flags );

    // constantes pour ShowWindow
    public const int SW_ERASE = 4;
    public const int SW_HIDE = 0;
    public const int SW_INVALIDATE = 2;
    public const int SW_MAX = 10;
    public const int SW_MAXIMIZE = 3;
    public const int SW_MINIMIZE = 6;
    public const int SW_NORMAL = 1;
    public const int SW_RESTORE = 9;
    public const int SW_SCROLLCHILDREN = 1;
    public const int SW_SHOW = 5;
    public const int SW_SHOWMAXIMIZED = 3;
    public const int SW_SHOWMINIMIZED = 2;
    public const int SW_SHOWMINNOACTIVE = 7;
    public const int SW_SHOWNA = 8;
    public const int SW_SHOWNOACTIVATE = 4;
    public const int SW_SMOOTHSCROLL = 0x10;

    [DllImport( "User32.dll" )]
    public static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
    public static extern bool SetForegroundWindow(IntPtr hwnd);

    [StructLayout( LayoutKind.Sequential )]
    public struct WINDOWPOS {
      public IntPtr   hwnd;
      public IntPtr   hwndAfter;
      public int      x;
      public int      y;
      public int      cx;
      public int      cy;
      public uint     flags;
    }

    //
    // Peinture des fenêtres
    //

    // flags pour RedrawWindow
    public const int RDW_ALLCHILDREN = 0x80;
    public const int RDW_ERASE = 4;
    public const int RDW_ERASENOW = 0x200;
    public const int RDW_FRAME = 0x400;
    public const int RDW_INVALIDATE = 1;
    public const int RDW_UPDATENOW = 0x100;

    [DllImport( "User32.dll" )]
    public static extern bool UpdateWindow( IntPtr hwnd );

    [DllImport( "User32.dll" )]
    public static extern bool InvalidateRect( IntPtr hWnd, IntPtr lpRect, bool bErase );

    [DllImport("user32.dll", CharSet=CharSet.Auto, ExactSpelling=true)]
    public static extern bool ValidateRect(IntPtr hWnd, [In, Out] ref RECT rect);

    [DllImport( "user32.dll", CharSet = CharSet.Auto, ExactSpelling = true )]
    public static extern bool RedrawWindow( IntPtr hwnd, IntPtr rcUpdate, IntPtr hrgnUpdate, int flags );

    //
    // Souris
    //

    [DllImport( "user32.dll" )]
    public static extern IntPtr SetCapture( IntPtr hWnd );

    [DllImport( "user32.dll" )]
    public static extern IntPtr GetCapture();

    [DllImport( "user32.dll" )]
    public static extern bool ReleaseCapture();

    [DllImport( "user32.dll" )]
    public static extern IntPtr SetFocus( IntPtr hWnd );

    //
    // Curseur
    //

    [DllImport( "user32.dll" )]
    public static extern IntPtr SetCursor( IntPtr hCursor );

    //
    // Graphique
    //

    public struct IconInfo {
      public bool fIcon;
      public int xHotspot;
      public int yHotspot;
      public IntPtr hbmMask;
      public IntPtr hbmColor;
    }

    [DllImport( "user32.dll" )]
    public static extern bool GetIconInfo( IntPtr hIcon, ref IconInfo pIconInfo );

    [DllImport( "user32.dll" )]
    public static extern IntPtr CreateIconIndirect( ref IconInfo icon );


    /// <summary>
    /// Provides access to function required to delete handle. 
    /// This method is used internally and is not required to be called separately.
    /// </summary>
    /// <param name="hIcon">Pointer to icon handle.</param>
    /// <returns>N/A</returns>
    [DllImport( "User32.dll" )]
    public static extern int DestroyIcon( IntPtr hIcon );

    //
    // Hit test
    //

    public const int HTTRANSPARENT = -1;
    public const int HTCLIENT = 1;

    //
    // Tab Control
    //

    public enum TabControlHitTest {
      TCHT_NOWHERE = 0x1,
      TCHT_ONITEMICON = 0x2,
      TCHT_ONITEMLABEL = 0x4,
      TCHT_ONITEM = TCHT_ONITEMICON | TCHT_ONITEMLABEL
    }

    public struct TCHITTESTINFO {
      public Point point;
      public TabControlHitTest flags;
    }

    //
    // ComboBox
    //

    [DllImport( "user32" )]
    public static extern bool GetComboBoxInfo( IntPtr hwndCombo, ref ComboBoxInfo info );

    [StructLayout( LayoutKind.Sequential )]
    public struct ComboBoxInfo {
      public int cbSize;
      public RECT rcItem;
      public RECT rcButton;
      public int stateButton;
      public IntPtr hwndCombo;
      public IntPtr hwndEdit;
      public IntPtr hwndList;
    }

    public static ComboBoxInfo GetComboBoxInfos( IntPtr handle ) {
      Win.ComboBoxInfo result = new Win.ComboBoxInfo();
      result.cbSize = Marshal.SizeOf( result );
      Win.GetComboBoxInfo( handle, ref result );
      return result;
    }

    //
    // Dialogues
    //

    [DllImport( "User32.Dll" )]
    public static extern int GetDlgCtrlID( IntPtr hWndCtl );

  }
}
