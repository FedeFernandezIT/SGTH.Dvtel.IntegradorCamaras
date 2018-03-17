using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using DvTelIntegradorCamaras.Integrador;

namespace DvTelIntegradorCamaras.Auth
{
    public class BasicAuth : IHttpModule
    {
        private const string Realm = "My Realm";

        public void Init(HttpApplication context)
        {
            // Register event handlers
            context.AuthenticateRequest += OnApplicationAuthenticateRequest;
            context.EndRequest += OnApplicationEndRequest;
        }

        private static void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        // TODO: Here is where you would validate the username and password.
        private static bool CheckPassword(Guid federationId,string directory, string username, string password,bool isLoginGuid)
        {
            var integrador = new IntegradorCamaras();
            if (isLoginGuid)
            {
                var resultLoginGuid = integrador.LoginGuid(federationId, username, password);
                if (resultLoginGuid is bool)
                {
                    return true;
                }
                return false;
            }
            var resultLogin = integrador.Login(directory, username, password);
            if (resultLogin is bool)
            {
                return true;
            }
            return false;
        }

        private static void AuthenticateUser(string credentials)
        {
            try
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
                credentials = encoding.GetString(Convert.FromBase64String(credentials));

                var separator = credentials.IndexOf(':');
                var name = credentials.Substring(0, separator);
                var password = credentials.Substring(separator + 1);
                var isLoginGuid = false;
                var federationId = new Guid();
                var directory = "";
                try
                {
                    federationId = Guid.Parse(ConfigurationManager.AppSettings["FederationId"]);
                    isLoginGuid = true;
                }
                catch
                {
                    directory= ConfigurationManager.AppSettings["Directory"];
                    isLoginGuid = false;
                }
                if (CheckPassword(federationId, directory, name, password, isLoginGuid))
                {
                    var identity = new GenericIdentity(name);
                    SetPrincipal(new GenericPrincipal(identity, null));
                }
                else
                {
                    // Invalid username or password.
                    HttpContext.Current.Response.StatusCode = 401;
                }
            }
            catch (FormatException)
            {
                // Credentials were not formatted correctly.
                HttpContext.Current.Response.StatusCode = 401;
            }
        }

        private static void OnApplicationAuthenticateRequest(object sender, EventArgs e)
        {
            var request = HttpContext.Current.Request;
            var authHeader = request.Headers["Authorization"];
            if (authHeader != null)
            {
                var authHeaderVal = AuthenticationHeaderValue.Parse(authHeader);

                // RFC 2617 sec 1.2, "scheme" name is case-insensitive
                if (authHeaderVal.Scheme.Equals("basic",
                        StringComparison.OrdinalIgnoreCase) &&
                    authHeaderVal.Parameter != null)
                {
                    AuthenticateUser(authHeaderVal.Parameter);
                }
            }
        }

        // If the request was unauthorized, add the WWW-Authenticate header 
        // to the response.
        private static void OnApplicationEndRequest(object sender, EventArgs e)
        {
            var response = HttpContext.Current.Response;
            if (response.StatusCode == 401)
            {
                response.Headers.Add("WWW-Authenticate",string.Format("Basic realm=\"{0}\"", Realm));
                //var result = "{\"status\":\"Error\",\"message\":\"Acceso denegado.No se pueden reconocer las credenciales para iniciar la sesión. Asegúrese de que el nombre de usuario y la contraseña proporcionados son correctos. Si continúa el problema, solicite ayuda al administrador del servidor Web.\"}";
                //response.Headers.Add("WWW-Authenticate", result);
            }
        }

        public void Dispose()
        {
        }
    }
}