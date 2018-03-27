using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Newtonsoft.Json;
using SGTH.Dvtel.Rest.Exceptions;
using SGTH.Dvtel.Rest.Extensions;
using SGTH.Dvtel.Rest.Models;

namespace SGTH.Dvtel.Rest.Filters
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
                response.Content = new StringContent(
                    JsonConvert.SerializeObject(new
                    {
                        Status = CodeStatus.ERROR,
                        Msg = "Ha ocurrido un error, comuníquese con el Administrador.",
                        Data = actionExecutedContext.Exception.CollectMessages()
                    }), System.Text.Encoding.UTF8, "application/json");
            }

            actionExecutedContext.Response = response;
        }

        public HttpResponseMessage SetMessage(WebApiException exception)
        {
            var response = new HttpResponseMessage();
            response.StatusCode = exception.StatusCode;
            string statusMessage;
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    statusMessage = CodeStatus.BAD_REQUEST;
                    break;
                case HttpStatusCode.NotFound:
                    statusMessage = CodeStatus.NOT_FOUND;
                    break;
                case HttpStatusCode.BadGateway:
                    statusMessage = CodeStatus.BAD_GATEWAY;
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    statusMessage = CodeStatus.METHOD_NOT_ALLOWED;
                    break;
                case HttpStatusCode.InternalServerError:
                    statusMessage = CodeStatus.INTERNAL_SERVER_ERROR;
                    break;
                case HttpStatusCode.Unauthorized:
                    statusMessage = CodeStatus.UNAUTHORIZED;
                    break;
                default:
                    statusMessage = CodeStatus.ERROR;
                    break;
            }

            response.Content =
                new StringContent(
                    JsonConvert.SerializeObject(new { Status = statusMessage, Msg = exception.Message, Data = "" }),
                    System.Text.Encoding.UTF8, "application/json");

            return response;

        }
    }
}