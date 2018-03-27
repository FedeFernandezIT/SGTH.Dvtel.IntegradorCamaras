using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using SGTH.Dvtel.Rest.Models;

namespace SGTH.Dvtel.Rest.Filters
{
    public class ValidationModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ModelState.IsValid)
            {

                var json = new ModelResponseMethod
                {
                    Status = CodeStatus.BAD_REQUEST,
                    Msg = "La solicitud es inválida.",
                    Data = GetErrorMessages(actionContext.ModelState)
                };

                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, json);
            }
        }

        private string[] GetErrorMessages(ModelStateDictionary model)
        {
            var messages = new List<string>();
            var modelEnumerate = model.GetEnumerator();
            while (modelEnumerate.MoveNext())
            {
                var current = modelEnumerate.Current;
                var errors = current.Value.Errors.GetEnumerator();
                while (errors.MoveNext())
                {
                    var error = errors.Current;
                    if (error == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        messages.Add(error.ErrorMessage);
                    }
                    else
                    {
                        messages.Add(error.Exception.InnerException?.Message ?? error.Exception.Message);
                    }
                }
                errors.Dispose();
            }
            modelEnumerate.Dispose();
            return messages.ToArray();
        }
    }
}