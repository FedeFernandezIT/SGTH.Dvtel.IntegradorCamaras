using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;
using SGTH.Dvtel.Rest.Services;

namespace SGTH.Dvtel.Rest.Tests.Services
{
    [TestClass]
    public class DvtelMobileServiceTest
    {
        [TestMethod]
        public async Task GetCameras_Success_Test()
        {
            // Arrange
            Mock<IDvtelMobileAdapter> mockMobile = new Mock<IDvtelMobileAdapter>();
            mockMobile.Setup(mbl => mbl.Authenticate()).ReturnsAsync(true);
            mockMobile.SetupProperty(mbl => mbl.Cameras, new List<Camera>
            {
                new Camera
                {
                    CanPlayback = true,
                    CanRecord = true,
                    Description = "Ptz Camera",
                    Id = Guid.NewGuid(),
                    IsAccessible = true,
                    IsGhost = false,
                    IsPTZEnabled = true,
                    IsRecording = true,
                    Name = "Dhua Ptz",
                    SiteId = Guid.NewGuid()
                },
                new Camera
                {
                    CanPlayback = true,
                    CanRecord = true,
                    Description = "Other Camera",
                    Id = Guid.NewGuid(),
                    IsAccessible = true,
                    IsGhost = false,
                    IsPTZEnabled = false,
                    IsRecording = true,
                    Name = "Dhua",
                    SiteId = Guid.NewGuid()
                }
            });

            // Act
            DvtelMobileService srv = new DvtelMobileService(mockMobile.Object);
            List<Camera> cameras = await srv.GetCameras();

            // Assert
            Assert.IsNotNull(cameras);
            Assert.AreEqual(2, cameras.Count);
            mockMobile.Verify(mbl => mbl.Authenticate(), Times.Once);
            mockMobile.Verify(mbl => mbl.Cameras, Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task GetCameras_AuthenticationFailed_Test()
        {
            // Arrange
            Mock<IDvtelMobileAdapter> mockMobile = new Mock<IDvtelMobileAdapter>();
            mockMobile.Setup(mbl => mbl.Authenticate()).ReturnsAsync(false);


            // Act
            DvtelMobileService srv = new DvtelMobileService(mockMobile.Object);
            List<Camera> cameras = await srv.GetCameras();
        }

        [TestMethod]
        public async Task StartLive_Sucess_Test()
        {
            // Arrange
            Mock<IDvtelMobileAdapter> mockMobile = new Mock<IDvtelMobileAdapter>();
            mockMobile.Setup(mbl => mbl.Authenticate()).ReturnsAsync(true);
            mockMobile.Setup(mbl => mbl.StartLive(It.IsAny<Guid>(), "mjpeg")).ReturnsAsync("http://localhost:8081/live/test");

            // Act
            DvtelMobileService srv = new DvtelMobileService(mockMobile.Object);
            Uri url = await srv.StartLive(Guid.NewGuid(), "mjpeg");

            // Assert
            Assert.IsNotNull(url);
            Assert.AreEqual(Uri.UriSchemeHttp ,url.Scheme);
            Assert.AreEqual("http://localhost:8081/live/test", url.AbsoluteUri);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task StartLive_AuthenticationFailed_Test()
        {
            // Arrange
            Mock<IDvtelMobileAdapter> mockMobile = new Mock<IDvtelMobileAdapter>();
            mockMobile.Setup(mbl => mbl.Authenticate()).ReturnsAsync(false);            

            // Act
            DvtelMobileService srv = new DvtelMobileService(mockMobile.Object);
            Uri url = await srv.StartLive(Guid.NewGuid(), "mjpeg");            
        }
    }
}
