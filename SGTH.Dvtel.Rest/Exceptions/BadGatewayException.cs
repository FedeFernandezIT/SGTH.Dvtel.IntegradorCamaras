using System;
using System.Net;

namespace SGTH.Dvtel.Rest.Exceptions
{
    public class BadGatewayException : WebApiException
    {
        public BadGatewayException(string msg, Exception ex) : base(msg, ex, HttpStatusCode.BadGateway)
        {
        }

        public BadGatewayException(string msg) : base(msg, HttpStatusCode.BadGateway)
        {
        }
    }
}