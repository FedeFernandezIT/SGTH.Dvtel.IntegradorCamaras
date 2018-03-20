using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Rest.Services
{
    public class DvtelMobileService : IDvtelMobileService
    {
        private readonly IDvtelMobileAdapter _mobile;

        public DvtelMobileService()
            : this(new DvtelMobileAdapter())
        {
        }

        public DvtelMobileService(IDvtelMobileAdapter mobile)
        {
            _mobile = mobile;
            //IsAuthenticated = IsAuthenticatedAsync();
        }

        public Task<bool> IsAuthenticated { get; private set; }

        private async Task<bool> IsAuthenticatedAsync()
        {
            return await _mobile.Authenticate();
        }

        public async Task<Uri> StartLive(Guid camera, Guid session, string compression)
        {
            if (!await _mobile.Authenticate())
            {
                throw new Exception();
            }
            var streamUrLive = await _mobile.StartLive(camera, compression);
            return new Uri(streamUrLive);
        }

        public async Task<List<Camera>> GetCameras()
        {            
            if (!await _mobile.Authenticate())
            {
                throw new Exception();
            }
            return _mobile.Cameras;
        }
    }
}