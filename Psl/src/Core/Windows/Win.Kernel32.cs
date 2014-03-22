/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * 12 01 2010 : version initiale
 */                                                                            // <wao never.end>

using System;
using System.Runtime.InteropServices;
using System.Drawing;

#pragma warning disable 1591 // commentaire xml absent

namespace Psl.Windows {

  // Regroupement de déclarations liées aux méthodes natives Windows
  partial class Win {

    [DllImport( "kernel32.dll", EntryPoint = "GetModuleHandle" )]
    public static extern int GetModuleHandle( [MarshalAs( UnmanagedType.LPStr )] string lpModuleName );


    [DllImport( "kernel32.dll", EntryPoint = "FreeLibrary" )]
    public static extern bool FreeLibrary( int hModule );

  }
}