using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SGTH.Dvtel.Mobile.Client.Exceptions;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;
using SGTH.Dvtel.Rest.Exceptions;
using SGTH.Dvtel.Rest.Extensions;

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
        

        public async Task<Uri> StartLive(Guid camera, string compression)
        {
            try
            {
                await _mobile.Authenticate();

                var streamUrLive = await _mobile.StartLive(camera, compression);
                
                return new Uri(streamUrLive);
            }
            catch (DvtelVmsException ex)
            {
                if (ex.Error == ErrorType.AuthorizationFailed)
                {
                    throw new UnauthorizedException(ex.CollectMessages());
                }
                else
                {
                    throw new BadGatewayException(ex.CollectMessages());
                }
            }
            finally
            {
                await _mobile.Logout();
            }
        }

        public async Task<List<Camera>> GetCameras()
        {
            try
            {
                await _mobile.Authenticate();

                // Generamos un nueva List, para no perder los datos
                // después de desconectarnos.
                var cameras = _mobile.Cameras.ToList();                
                return cameras;
            }
            catch (DvtelVmsException ex)
            {
                if (ex.Error == ErrorType.AuthorizationFailed)
                {
                    throw new UnauthorizedException(ex.CollectMessages());
                }
                else
                {
                    throw new BadGatewayException(ex.CollectMessages());
                }
            }
            finally
            {
                await _mobile.Logout();
            }            
        }
    }
}