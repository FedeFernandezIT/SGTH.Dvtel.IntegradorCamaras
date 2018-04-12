using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using SGTH.Dvtel.Rest.Filters;
using WebApi.Hal;

//using System.Web.Http.Cors;

namespace SGTH.Dvtel.Rest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuración y servicios de API web            

            config.Filters.Add(new ValidacionesExcepcionesHandler());
            //config.MessageHandlers.Add(new LogRequestResponseFilter());

            // Clear XML as Supported Media Type
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();            

            // Formating indent and camelCase for Json
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));            

            // Rutas de API web
            config.MapHttpAttributeRoutes();            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );            
        }
    }
}