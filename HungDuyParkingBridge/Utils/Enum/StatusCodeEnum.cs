using System;
using System.ComponentModel;

namespace HungDuyParkingBridge.Utils.Enum
{
    public enum StatusCodeEnum
    {
        [Description("OK")]
        OK,
        
        [Description("ERROR")]
        ERROR
    }
    
    // Extension methods for getting enum descriptions
    public static class StatusCodeEnumExtensions
    {
        public static string GetDescription(this StatusCodeEnum statusCode)
        {
            var field = statusCode.GetType().GetField(statusCode.ToString());
            if (field != null)
            {
                var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
                return attribute?.Description ?? statusCode.ToString();
            }
            return statusCode.ToString();
        }
        
        public static StatusCodeEnum ParseFromString(string code)
        {
            return code?.ToUpperInvariant() switch
            {
                "OK" => StatusCodeEnum.OK,
                "ERROR" => StatusCodeEnum.ERROR,
                _ => StatusCodeEnum.ERROR
            };
        }
    }
}
