using System;
using System.Net;

namespace SGTH.Dvtel.Rest.Exceptions
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