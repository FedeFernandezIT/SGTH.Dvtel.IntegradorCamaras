using System;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Mobile.Client.Exceptions
{
    public class DvtelVmsException : Exception
    {
        public ErrorType Error { get; set; }

        public DvtelVmsException() : this(ErrorType.Unknown)
        {            
        }

        public DvtelVmsException(ErrorType error) : base(error.ToString())
        {
            Error = error;
        }

        public DvtelVmsException(string message) : this(message, ErrorType.Unknown)
        {            
        }

        public DvtelVmsException(string message, ErrorType error) : base(message)
        {
            Error = error;
        }

        public DvtelVmsException(string message, Exception innerException) : base(message, innerException)
        {
            Error = ErrorType.Unknown;
        }
    }
}