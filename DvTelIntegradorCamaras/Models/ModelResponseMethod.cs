using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DvTelIntegradorCamaras.Models
{
    public class ModelResponseMethod
    {
        public string status { get; set; }
        public string msg { get; set; }
        public object data { get; set; }
    }
    public static class CodeStatus
    {
        public const string OK_STRING = "Ok";
        public const string ERROR = "Error";
        public const string OK = "200";//Unauthorized
        public const string UNAUTHORIZED = "401";
        public const string NOT_FOUND = "404";
        public const string METHOD_NOT_ALLOWED = "405";
        public const string INTERNAL_SERVER_ERROR = "500";
    }
}