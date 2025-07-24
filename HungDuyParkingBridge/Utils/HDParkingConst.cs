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
