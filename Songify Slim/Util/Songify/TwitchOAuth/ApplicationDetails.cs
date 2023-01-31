using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Songify_Slim;
using Songify_Slim.Util.General;

namespace VonRiddarn.Twitch.ImplicitOAuth
{
    public static class ApplicationDetails
    {
        // The client ID you get from your Twitch developer console (https://dev.twitch.tv/).
        public static string twitchClientId = "sgiysnqpffpcla6zk69yn8wmqnx56o";


        private static readonly List<int> RedirectPorts = new List<int> { 4003, 49180, 54321, 57689, 64567, 65100 };
        private static readonly List<int> FetchPorts = new List<int> { 4004, 49181, 54322, 57690, 64568, 65101 };

        // The URI you entered when registering your application in the twitch console.
        // Default is fine.
        public static string redirectUri = $"http://localhost:{GetRedirectPort()}/";

        // Any URI you want to fetch results on.
        // Default is fine.
        public static string fetchUri = $"http://localhost:{GetFetchPort()}/";

        private static int GetFetchPort()
        {
            Logger.LogStr("TWITCH AUTH: Checking Fetch ports");
            foreach (int FetchPort in FetchPorts.Where(PortIsFree))
            {
                return FetchPort;
            }
            return 4004;
        }

        private static int GetRedirectPort()
        {
            Logger.LogStr("TWITCH AUTH: Checking Redirect ports");
            foreach (int redirectPort in RedirectPorts.Where(PortIsFree))
            {
                return redirectPort;
            }
            return 4003;
        }

        private static bool PortIsFree(int port)
        {
            // Get the IP global properties for the local network
            var properties = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();

            // Get a list of active TCP connections
            var connections = properties.GetActiveTcpConnections();

            // Check if the specified port is blocked
            bool isBlocked = connections.All(connection => connection.LocalEndPoint.Port != port);

            Logger.LogStr($"TWITCH AUTH: Port {port} is {(isBlocked ? "free" : "blocked")}");
            
            //Debug.WriteLine($"PortFree: {isBlocked}");
            return isBlocked;
        }
    }
}