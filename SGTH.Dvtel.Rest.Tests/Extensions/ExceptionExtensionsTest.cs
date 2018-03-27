using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Rest.Extensions;

namespace SGTH.Dvtel.Rest.Tests.Extensions
{
    [TestClass]
    public class ExceptionExtensionsTest
    {
        [TestMethod]
        public void CollectMessages_Tests()
        {
            // Arrange
            Exception inner = new Exception("Inner message.");
            Exception root = new Exception("Root message", inner);

            // Act
            string message = root.CollectMessages();

            // Assert
            Assert.AreEqual("Root message. Inner message.", message);
        }
    }
}
