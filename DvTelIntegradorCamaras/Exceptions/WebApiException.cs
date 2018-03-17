using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
namespace DvTelIntegradorCamaras.Exceptions
{
    public class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Logged { get; set; }
        public string LogId { get; set; }

        public WebApiException(string msg, Exception ex, HttpStatusCode status) : base(msg, ex)
        {
            StatusCode = status;
            Logged = false;
        }

        public WebApiException(string msg, HttpStatusCode status) : base(msg)
        {
            StatusCode = status;
            Logged = false;
        }
    }
}