using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using SGTH.Dvtel.Mobile.Client;
using SGTH.Dvtel.Mobile.Client.Exceptions;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;
using SGTH.Dvtel.Mobile.Client.VideoProviders;

namespace SGTH.Dvtel.Rest.Services
{
    public class DvtelMobileAdapter : IDvtelMobileAdapter
    {
        private readonly UnitedVmsProvider _vms;
        private readonly string _username;
        private readonly string _password;

        public DvtelMobileAdapter()
        {
            string endpoint = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Endpoint"];
            _username = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Username"];
            _password = ConfigurationManager.AppSettings["Dvtel.Mobile.Client.Password"];

            string connectionCredentials = $"{_username}:{_password};{endpoint}";

            _vms = UnitedVmsProvider.GetInstance(connectionCredentials);
        }


        public string BaseAddress
        {
            get { return _vms.BaseAddress; }
            set { _vms.BaseAddress = value; }
        }

        public string Credentials => $"{_username}:{_password}";

        public Guid SessionId { get; set; }

        public List<Camera> Cameras
        {
            get { return _vms.Cameras; }
            set { _vms.Cameras = value; }
        }

        public event EventHandler<List<Camera>> CameraListUpdated;

        public async Task Authenticate()
        {
            if (!await _vms.Authenticate())
            {
                throw new DvtelVmsException("Dvtel Vms Provider Authenticate Error: " + ErrorType.Unknown);
            }            
        }

        public async Task Logout()
        {
            await _vms.Logout();
        }

        public async Task GetCameras()
        {
            await _vms.GetCameras();
        }

        public async Task<string> StartLive(Guid cameraId, string compression)
        {
            return await _vms.StartLive(cameraId, compression);
        }

        public Task<ArchiveStream> StartArchive(Guid cameraId, string compression, DateTime startTime, DateTime endTime = default(DateTime))
        {
            throw new NotImplementedException();
        }

        public Task<bool> ControlHttpArchive(Guid playbackSessionId, Consts.Speeds speedValue)
        {
            throw new NotImplementedException();
        }

        public Task<bool> StopArchive(Guid playbackSessionId)
        {
            throw new NotImplementedException();
        }
    }
}