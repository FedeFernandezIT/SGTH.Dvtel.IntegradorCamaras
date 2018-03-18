using System;

namespace SGTH.Dvtel.Rest.Services
{
    public interface IDvtelMobileService
    {
        Uri StartLive(Guid camera, Guid session, string compression);
    }
}