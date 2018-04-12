using System;
using DVTel.API;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Helpers;

namespace SGTH.Dvtel.Rest.Tests.Helpers
{
    [TestClass]
    public class LoginManagerTest
    {
        [TestMethod]
        public void Login_Success_Test()
        {
            // Arrange
            string directory = "localhost";
            string username = "usertest";
            string password = "passtest";

            // Act
            LoginManager lm = new LoginManager();
            IDvtelSystemId sys = lm.Login(directory, username, password);

            // Assert
            Assert.IsNotNull(sys);
        }
    }
}
