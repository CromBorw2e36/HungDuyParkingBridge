using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HungDuyParkingBridge.Utils
{
    /// <summary>
    /// Helper class for network-related operations, especially port availability checking
    /// </summary>
    public static class NetworkHelper
    {
        /// <summary>
        /// Checks if a specific TCP port is already in use on the local machine
        /// </summary>
        /// <param name="port">The port number to check</param>
        /// <returns>True if the port is in use, false otherwise</returns>
        public static bool IsPortInUse(int port)
        {
            // Method 1: Use IPGlobalProperties to check active connections
            try
            {
                var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnections = ipGlobalProperties.GetActiveTcpConnections();
                var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();

                // Check if any active TCP connections are using this port
                if (tcpConnections.Any(c => c.LocalEndPoint.Port == port))
                {
                    return true;
                }

                // Check if any TCP listeners are using this port
                if (tcpListeners.Any(l => l.Port == port))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking port connections: {ex.Message}");
                // Fall through to the socket method as a backup
            }

            // Method 2: Try to bind to the port
            try
            {
                using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(new IPEndPoint(IPAddress.Loopback, port));
                socket.Close();
                return false; // Port is available
            }
            catch (SocketException)
            {
                return true; // Port is in use
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in socket test: {ex.Message}");
                return true; // Assume port is in use to be safe
            }
        }

        /// <summary>
        /// Checks if multiple ports are already in use
        /// </summary>
        /// <param name="ports">Array of port numbers to check</param>
        /// <returns>A list of ports that are in use, empty if all are available</returns>
        public static List<int> GetPortsInUse(params int[] ports)
        {
            var portsInUse = new List<int>();
            
            foreach (var port in ports)
            {
                if (IsPortInUse(port))
                {
                    portsInUse.Add(port);
                }
            }
            
            return portsInUse;
        }

        /// <summary>
        /// Extracts the port number from a URI string
        /// </summary>
        /// <param name="uriString">URI string like "http://localhost:5000"</param>
        /// <returns>Port number or -1 if not found</returns>
        public static int ExtractPortFromUri(string uriString)
        {
            try
            {
                if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
                {
                    return uri.Port;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error extracting port: {ex.Message}");
            }
            
            return -1;
        }
    }
}