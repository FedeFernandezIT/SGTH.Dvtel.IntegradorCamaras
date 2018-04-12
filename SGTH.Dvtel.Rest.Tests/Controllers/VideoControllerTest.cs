using System;
using System.Reflection;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Controllers;
using SGTH.Dvtel.Rest.Filters;

namespace SGTH.Dvtel.Rest.Tests.Controllers
{
    [TestClass]
    public class VideoControllerTest
    {
        [TestMethod]
        public void VideoController_Decorate_With_Authorize_Test()
        {
            // Arrange
            Type type = typeof(VideoController);

            // Act            
            AuthorizeAttribute authorize = type.GetCustomAttribute<AuthorizeAttribute>();
            ModuloVBasicAuthenticationAttribute authentication = type.GetCustomAttribute<ModuloVBasicAuthenticationAttribute>();

            // Assert
            Assert.IsNotNull(authorize);
            Assert.IsNotNull(authentication);
        }
    }
}
