using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Services;

namespace SGTH.Dvtel.Rest.Tests.Services
{
    [TestClass]
    public class DvtelMobileAdapterTest
    {
        [TestMethod]
        public void VmsProvider_Initilization_Test()
        {
            // Arrange
            string endpoint = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Endpoint"];
            string username = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Username"];
            string password = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Password"];

            // Act
            DvtelMobileAdapter mobile = new DvtelMobileAdapter();

            // Assert
            Assert.AreEqual($@"http://{endpoint}/", mobile.BaseAddress);
            Assert.AreEqual($"{username}:{password}", mobile.Credentials);
        }
    }
}
