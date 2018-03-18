using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Filters;

namespace SGTH.Dvtel.Rest.Tests.Filters
{
    [TestClass]
    public class ModuloVBasicAuthenticationAttributeTest : BasicAuthenticatationAttributeTest<ModuloVBasicAuthenticationAttribute>
    {
        [TestMethod]
        public override async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Valid_Credentials_Test()
        {
            Username = "usertest";
            Password = "passwordtest";
            await base.AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Valid_Credentials_Test();
        }

        [TestMethod]
        public override async Task AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Invalid_UsernameOrPassword_Test()
        {
            Username = "usertest";
            Password = "nomatch";
            await base.AuthenticateAsync_From_HttpAuthenticationContext_Basic_With_Invalid_UsernameOrPassword_Test();
        }
    }
}
