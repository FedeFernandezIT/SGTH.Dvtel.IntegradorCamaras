using System.Configuration;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace SGTH.Dvtel.Rest.Filters
{
    public class ModuloVBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected override async Task<IPrincipal> AuthenticateAsync(string username, string password, CancellationToken cancellationToken)
        {
            if (!CheckPassword(username, password))
            {
                return null;
            }

            IIdentity identity = new GenericIdentity(username);
            return new GenericPrincipal(identity, null);
        }

        private bool CheckPassword(string username, string password)
        {
            string mvUsername = ConfigurationManager.AppSettings["ModuloV.Username"];
            string mvPassword = ConfigurationManager.AppSettings["ModuloV.Password"];

            return username.Equals(mvUsername) && password.Equals(mvPassword);
        }
    }
}