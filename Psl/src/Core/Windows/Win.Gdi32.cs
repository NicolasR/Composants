/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 08 12 2008 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Runtime.InteropServices;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  // Regroupement de déclarations liées aux méthodes natives Windows
  partial class Win {

    [DllImport( "GDI32.dll" )]
    public static extern int DeleteObject( IntPtr hObject );

    [DllImport( "gdi32.dll", EntryPoint = "CreateSolidBrush", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true )]
    public static extern IntPtr CreateSolidBrush( int crColor );

    [DllImport( "gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true )]
    public static extern int SetBkColor( IntPtr hDC, int clr );

    [DllImport( "gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true )]
    public static extern int SetTextColor( IntPtr hDC, int crColor );

  }
}
