using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HungDuyParkingBridge.Utils.Enum;

namespace HungDuyParkingBridge.Models
{
    internal class ResponeModel <T>
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public ResponeModel(string? code, string? message, T? data)
        {
            Code = code;
            Message = message;
            Data = data;
        }
        public ResponeModel(string? code, string? message)
        {
            Code = code;
            Message = message;
            Data = default(T);
        }
        // Convenience constructors with StatusCodeEnum
        public ResponeModel(StatusCodeEnum statusCode, string? message, T? data)
        {
            Code = statusCode.GetDescription();
            Message = message;
            Data = data;
        }
        
        public ResponeModel(StatusCodeEnum statusCode, string? message)
        {
            Code = statusCode.GetDescription();
            Message = message;
            Data = default(T);
        }
        
        // Helper methods
        public bool IsSuccess => Code == StatusCodeEnum.OK.GetDescription();
        public bool IsError => Code == StatusCodeEnum.ERROR.GetDescription();
        
        // Static factory methods for common responses
        public static ResponeModel<T> Success(T? data, string? message = "Operation completed successfully")
        {
            return new ResponeModel<T>(StatusCodeEnum.OK, message, data);
        }
        
        public static ResponeModel<T> Error(string? message = "An error occurred")
        {
            return new ResponeModel<T>(StatusCodeEnum.ERROR, message);
        }
    }
}
