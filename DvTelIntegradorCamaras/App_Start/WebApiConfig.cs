using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
//using System.Web.Http.Cors;
using System.Web.Http.Routing;
using WebApi.Hal;
using DvTelIntegradorCamaras.Filters;
namespace DvTelIntegradorCamaras.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Filters.Add(new ValidacionesExcepcionesHandler());
            config.MessageHandlers.Add(new LogRequestResponseFilter());

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            //config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            // config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling  = Newtonsoft.Json.ReferenceLoopHandling.Serialize;

            config.MapHttpAttributeRoutes();
            ((Newtonsoft.Json.Serialization.DefaultContractResolver)config.Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Configuración y servicios de API web

            // Rutas de API web
            //*config.MapHttpAttributeRoutes();

            /*config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );*/

            // Clear XML as Supported Media Type
           // config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();

            // Formating indent and camelCase for Json
            /*config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.Add(new JsonHalMediaTypeFormatter());*/
        }
    }
}