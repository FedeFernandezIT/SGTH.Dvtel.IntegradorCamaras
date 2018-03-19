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
        }

        public Uri StartLive(Guid camera, Guid session, string compression)
        {
            return new Uri("http://localhost:8081/live/test");
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