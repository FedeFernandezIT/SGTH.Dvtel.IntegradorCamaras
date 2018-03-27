using System;
using System.Net;

namespace SGTH.Dvtel.Rest.Exceptions
{
    public class UnauthorizedException : WebApiException
    {
        public UnauthorizedException(string msg, Exception ex) : base(msg, ex, HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedException(string msg) : base(msg, HttpStatusCode.Unauthorized)
        {
        }
    }
}