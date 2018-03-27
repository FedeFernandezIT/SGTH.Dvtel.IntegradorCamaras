using System;

namespace SGTH.Dvtel.Rest.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Devuelve el mensaje de error del nivel más interno de la excepción.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns>Error message del nivel más interno.</returns>
        public static string FetchDeepMessage(this Exception ex)
        {
            return ex.InnerException != null
                ? ex.InnerException.FetchDeepMessage()
                : ex.Message;
        }

        /// <summary>
        /// Devuelve todos los mensajes de error en una única cadena.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string CollectMessages(this Exception ex)
        {
            var message = ex.Message != null && !ex.Message.EndsWith(".") 
                ? ex.Message + "." 
                : ex.Message;

            return ex.InnerException != null
                ? message + " " + ex.InnerException.CollectMessages()
                : message;
        }
    }
}