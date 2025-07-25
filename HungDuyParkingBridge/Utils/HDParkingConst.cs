using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HungDuyParkingBridge.Utils
{
    internal class HDParkingConst
    {
        public static readonly string version = "1.0.2";
        public static readonly string key = "012233";
        public static readonly string nameSoftware = "HD Parking Bridge";
        public static readonly string pathSaveFile = @"C:\HungDuyParkingReceivedFiles";
        public static readonly string portHttp = "http://localhost:5000";
        public static readonly string portWebSocket = "ws://localhost:5001/ws";
        // Admin access control
        public static bool IsAdminAuthenticated { get; set; } = false;
        
        public static void SetAdminAccess(bool authenticated)
        {
            IsAdminAuthenticated = authenticated;
        }
        
        public static bool ValidatePrivateKey(string inputKey)
        {
            return string.Equals(inputKey, key, StringComparison.Ordinal);
        }
    }
}
