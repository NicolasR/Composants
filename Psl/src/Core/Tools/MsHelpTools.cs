/*                                                                            // <wao never.begin>
 * Librairie Psl
 * Auteur : Didier Vaudène
 * 
 * Credits
 * Microsoft Help Viewer SDK 1.4
 * 
 * 03 03 2011 : version initiale
 */                                                                           // <wao never.end>
using System;
using System.Runtime.InteropServices;

namespace Psl.Tools {

  /// <summary>
  /// Adaptation des url liées à l'aide en ligne MS Help View 1.0
  /// </summary>
  /// <remarks>
  /// <para>
  /// L'adaptation des url est due au fait que l'accès à l'aide locale passe par l'agent de bibliothèque
  /// Help Library Agent : les url réelles sont liées au localhost (127.0.0.1) et incluent le process ID (pid)
  /// de l'agent de bibliothèque lui-même. Il s'ensuit que ces url ne sont pas permanentes et changent à chaque session 
  /// de l'agent de bibliothèque, puisque le pid de l'agent de bibliothèque change à chaque session.
  /// </para>
  /// <para>
  /// La classe <see cref="MsHelpTools"/> regroupe les méthodes permettant de convertir les url permanentes
  /// (commençant par "ms-xhelp: ///?") en des url liées à l'agent de librairie (commençant par "http: //127.0.0.1:47873/help/"),
  /// et vice-versa.
  /// </para>
  /// <para>
  /// Internet Explorer est capable de reconnaître des url qui commencent par le préfixe "ms-xhelp: ///?",
  /// et route ces url vers l'agent de librairie. Ces url peuvent être considérées comme des url permanentes.
  /// Ce routage est également effectué par le composant <see cref="System.Windows.Forms.WebBrowser"/>.
  /// Les méthodes de cette classe permettent d'obtenir le même effet dans un explorateur personnalisé,
  /// sans que le routage ne conduise à ouvrir Internet Explorer. Pour ce faire, intervenir comme suit au niveau de 
  /// l'événement <see cref="System.Windows.Forms.WebBrowser.Navigating"/> lorsque l'url cible de la
  /// navigation est une url "ms-xhelp" : (1) annuler la navigation en basculant à false 
  /// la propriété Cancel du descripteur d'événement <see cref="System.Windows.Forms.WebBrowserNavigatingEventArgs"/>, (2) convertir l'url
  /// via <see cref="GetSessionUrl"/> pour obtenir une url de session de l'agent de librairie, et (3) déclencher
  /// une navigation vers cette url de session via la méthode <see cref="System.Windows.Forms.WebBrowser.Navigate(string)"/>.
  /// </para>
  /// <para>
  /// Le code de cette classe est une adaptation des snippets proposés dans le "Help Viewer DSK 1.4" de Microsoft
  /// </para>
  /// </remarks>
  public class MsHelpTools {

    //
    // Constantes exposées
    //

    /// <summary>
    /// Préfixe des url permanentes "ms-xhelp: ///?"
    /// </summary>
    public const string BasePermnentUrl = "ms-xhelp:///?";

    /// <summary>
    /// Préfixe des url réelles pour l'agent de librairie en "http: //127.0.0.1:47873/help/"
    /// </summary>
    public const string BaseOfflineUrl = "http://127.0.0.1:47873/help/";

    //
    // Méthodes natives de Wtsapî.32
    //

    private enum WTSInfoClass {
      WTSInitialProgram,
      WTSApplicationName,
      WTSWorkingDirectory,
      WTSOEMId,
      WTSSessionId,
      WTSUserName,
      WTSWinStationName,
      WTSDomainName,
      WTSConnectState,
      WTSClientBuildNumber,
      WTSClientName,
      WTSClientDirectory,
      WTSClientProductId,
      WTSClientHardwareId,
      WTSClientAddress,
      WTSClientDisplay,
      WTSClientProtocolType
    }

    [DllImport( "Wtsapi32.dll" )]
    private static extern bool WTSQuerySessionInformation( System.IntPtr hServer, int sessionId, WTSInfoClass wtsInfoClass, out System.IntPtr ppBuffer, out uint pBytesReturned );

    [DllImport( "wtsapi32.dll", ExactSpelling = true, SetLastError = false )]
    private static extern void WTSFreeMemory( IntPtr memory );

    //
    // Gestion du MS Help Agent
    //

    private static string GetHelpAgentLocation() {
      string agent = string.Empty;
      Microsoft.Win32.RegistryKey key = null;

      using ( key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey( @"MS-XHelp\shell\open\command", false ) ) {

        //trim off quotes
        agent = key.GetValue( null ).ToString().ToLower().Replace( "\"", "" );

        //get rid of everything after the ".exe"
        if ( !agent.EndsWith( "exe" ) ) agent = agent.Substring( 0, agent.LastIndexOf( ".exe" ) + 4 );
      }

      return agent;
    }

    private static bool IsProcessOpened( string name ) {
      foreach ( System.Diagnostics.Process process in System.Diagnostics.Process.GetProcesses() )
        if ( process.ProcessName.Contains( name ) ) return true;
      return false;
    }

    private static void StartHelpAgent() {
      if ( IsProcessOpened( "HelpLibAgent" ) ) return;

      using ( System.Diagnostics.Process process = new System.Diagnostics.Process() ) {
        process.EnableRaisingEvents = false;
        process.StartInfo.FileName = GetHelpAgentLocation();
        process.Start();
      }
    }

    //
    // Conversion des url liées à la session
    //

    private static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
    private static int WTS_CURRENT_SESSION = -1;

    private static int GetSessionNumber() {
      IntPtr ptrSessionId = IntPtr.Zero;
      Int32 sessionId = 0;
      uint bytesReturned;

      try {
        bool retVal = WTSQuerySessionInformation( WTS_CURRENT_SERVER_HANDLE, WTS_CURRENT_SESSION, WTSInfoClass.WTSSessionId, out ptrSessionId, out bytesReturned );
        if ( retVal ) sessionId = Marshal.ReadInt32( ptrSessionId );
      } finally {
        if ( ptrSessionId != IntPtr.Zero ) WTSFreeMemory( ptrSessionId );
      }
      return sessionId;
    }

    private const string MsHelpScript = "/ms.help?";

    private static string GetBaseSessionUrl() {
      StartHelpAgent();
      System.Diagnostics.Process[] helpProcesses = System.Diagnostics.Process.GetProcessesByName( "helplibagent" );
      string pid = helpProcesses[ 0 ].Id.ToString();
      return BaseOfflineUrl + GetSessionNumber().ToString() + "-" + pid + "/ms.help?";
    }

    //
    // Fonctionnalités exposées
    //

    /// <summary>
    /// Convertit si nécessaire une url permanente "ms-xhelp" en un url pour la session courante de l'agent de librairie
    /// </summary>
    /// <remarks>
    /// Aucune conversion n'est appliquée si l'url à convertir le commence pas par le préfixe <see cref="BasePermnentUrl"/>.
    /// </remarks>
    /// <param name="url">url à convertir</param>
    /// <returns>l'url d'origine si elle n'est pas en "ms-xhelp", sinon l'url pour la session courante de l'agent de librairie</returns>
    public static string GetSessionUrl( string url ) {
      if ( string.IsNullOrEmpty( url ) ) throw new ArgumentException( "L'url est null ou vide", "url" );
      if ( !url.StartsWith( BasePermnentUrl, StringComparison.InvariantCultureIgnoreCase ) ) return url;
      return GetBaseSessionUrl() + url.Substring( BasePermnentUrl.Length );
    }

    /// <summary>
    /// Convertit si nécessaire une url de session de l'agent de librairie en une url permanente "ms-xhelp"
    /// </summary>
    /// <remarks>
    /// <para>
    /// Aucune conversion n'est appliquée si l'url à convertir le commence pas par le préfixe <see cref="BaseOfflineUrl"/>.
    /// </para>
    /// <para>
    /// Cette méthode provoque la mise en route de l'agent de librairie si ce n'est pas encore le cas. 
    /// </para>
    /// </remarks>
    /// <param name="url">url à convertir</param>
    /// <returns>l'url permamente en "ms-xhelp" si l'url à convertir est une url de session de l'agent de librairie, sinon retourne l'url fournie</returns>
    public static string GetPermanentUrl( string url ) {
      if ( string.IsNullOrEmpty( url ) ) throw new ArgumentException( "L'url est null ou vide", "url" );

      if ( !url.StartsWith( BaseOfflineUrl, StringComparison.InvariantCultureIgnoreCase ) ) return url;

      int index = url.IndexOf( MsHelpScript, StringComparison.InvariantCultureIgnoreCase );
      if ( index == -1 ) throw new ArgumentException( string.Format( "L'url est probablement incorrecte (sous-chaîne \"{0}\" introuvable", MsHelpScript ), "url" );

      return BasePermnentUrl + url.Substring( index + MsHelpScript.Length );
    }
  }
}
