using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using DvTelIntegradorCamaras.Exceptions;
using DvTelIntegradorCamaras.Models;
namespace DvTelIntegradorCamaras.Filters
{
    public class ValidacionesExcepcionesHandler : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            var exceptionWebApi = actionExecutedContext.Exception as WebApiException;


            if (exceptionWebApi != null)
            {
                response = SetMessage(exceptionWebApi);
            }
            else
            {
                response.Content = new StringContent(JsonConvert.SerializeObject(new { status = CodeStatus.ERROR, msg = "Comunicarse con el Administrador de DvTel .NET", data = "" }), System.Text.Encoding.UTF8, "application/json");
            }

            actionExecutedContext.Response = response;
        }

        public HttpResponseMessage SetMessage(WebApiException exception)
        {
            var response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            response.StatusCode = exception.StatusCode;
            response.Content = new StringContent(JsonConvert.SerializeObject(new { status = CodeStatus.ERROR, msg = exception.Message, data = "" }), System.Text.Encoding.UTF8, "application/json");

            return response;

        }
    }
}