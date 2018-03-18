using System;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SGTH.Dvtel.Rest.Controllers;
using SGTH.Dvtel.Rest.Filters;
using SGTH.Dvtel.Rest.Models;
using SGTH.Dvtel.Rest.Services;

namespace SGTH.Dvtel.Rest.Tests.Controllers
{
    [TestClass]
    public class ViewingControllerTest
    {
        [TestMethod]
        public void ViewingController_Decorate_With_Authorize_Test()
        {
            // Arrange
            Type type = typeof(ViewingController);
            
            // Act            
            AuthorizeAttribute authorize = type.GetCustomAttribute<AuthorizeAttribute>();
            ModuloVBasicAuthenticationAttribute authentication = type.GetCustomAttribute<ModuloVBasicAuthenticationAttribute>();
            
            // Assert
            Assert.IsNotNull(authorize);
            Assert.IsNotNull(authentication);
        }

        [TestMethod]
        public void GetStartLive_Test()
        {
            // Arrange
            var mockMobile = new Mock<IDvtelMobileService>();
            mockMobile.Setup(x => x.StartLive(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(new Uri("http://localhost:8081/live"));

            var controller = new ViewingController(mockMobile.Object);

            // Act
            IHttpActionResult actionResult = controller.StartLive(Guid.NewGuid(), Guid.NewGuid());
            var contentResult = actionResult as OkNegotiatedContentResult<ModelResponseMethod>;

            // Assert
            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.IsInstanceOfType(contentResult.Content.Data, typeof(string));
            Assert.AreEqual("http://localhost:8081/live", contentResult.Content.Data);
        }
    }
}
