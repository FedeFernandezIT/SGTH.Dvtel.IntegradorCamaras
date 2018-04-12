using DVTel.API;

namespace SGTH.Dvtel.Rest.Helpers
{
    public class LoginManager
    {
        public IDvtelSystemId Login(string directory, string username, string password)
        {
            return DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Login(directory, username, password);
        }
    }
}