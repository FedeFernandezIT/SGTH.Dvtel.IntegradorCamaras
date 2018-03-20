using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Rest.Services
{
    public interface IDvtelMobileService
    {
        Task<Uri> StartLive(Guid camera, Guid session, string compression);

        Task<List<Camera>> GetCameras();
    }
}