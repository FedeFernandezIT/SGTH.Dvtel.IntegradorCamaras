using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SGTH.Dvtel.Mobile.Client.Exceptions;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Mobile.Client.Tests.Exceptions
{
    [TestClass]
    public class DvtelVmsExceptionTest
    {
        [TestMethod]
        public void DvtelVmsException_Constructor_Message_Default_Tests()
        {
            // Arrange
            ErrorType error = ErrorType.AuthorizationFailed;
            
            // Act
            DvtelVmsException exception = new DvtelVmsException(error);

            // Assert
            Assert.AreEqual(ErrorType.AuthorizationFailed, exception.Error);
            Assert.AreEqual(ErrorType.AuthorizationFailed.ToString(), exception.Message);
        }

        [TestMethod]
        public void DvtelVmsException_Constructor_InnerExpeption_Tests()
        {
            // Arrange
            Exception inner = new Exception("Inner message.");

            // Act
            DvtelVmsException exception = new DvtelVmsException("Dvtel Vms no responde.", inner);

            // Assert
            Assert.AreEqual(ErrorType.Unknown, exception.Error);
            Assert.AreEqual("Dvtel Vms no responde.", exception.Message);

            Assert.IsNotNull(exception.InnerException);            
            Assert.AreEqual("Inner message.", exception.InnerException.Message);
        }
    }
}
