using System;

namespace SGTH.Dvtel.Rest.Services
{
    public class DvtelMobileService : IDvtelMobileService
    {
        public Uri StartLive(Guid camera, Guid session, string compression)
        {
            return new Uri("http://localhost:8081/live/test");
        }
    }
}