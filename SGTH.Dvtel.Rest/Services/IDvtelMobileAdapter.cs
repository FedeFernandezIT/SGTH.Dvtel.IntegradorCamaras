using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.UI;
using SGTH.Dvtel.Mobile.Client;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Rest.Services
{
    public interface IDvtelMobileAdapter
    {
        string BaseAddress { get; set; }

        string Credentials { get; }

        Guid SessionId { get; set; }

        List<Camera> Cameras { get; set; }

        event EventHandler<List<Camera>> CameraListUpdated;

        Task<bool> Authenticate();

        Task Logout();

        Task GetCameras();

        /// <summary>
        /// Starts live video stream for the requested cameraId
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="compression"></param>        
        /// <returns>The HTTP MJPEG or RTSP H256 Url need to acquire the stream from the Transcoder</returns>
        Task<string> StartLive(Guid cameraId, string compression);

        /// <summary>
        /// Starts recorded video stream for the requested cameraId
        /// </summary>
        /// <param name="cameraId"></param>       
        /// /// <param name="compression">mjpeg or h264 - if null provided will default to transcoded MJPEG</param>
        /// <param name="startTime">Start time in UTC</param>
        /// <param name="endTime">End time in UTC - optional</param>
        /// <returns>The HTTP MJPEG or RTSP H256 Url need to acquire the stream from the Transcoder - or null upon failure</returns>
        Task<ArchiveStream> StartArchive(Guid cameraId, string compression, DateTime startTime, DateTime endTime = default(DateTime));

        Task<bool> ControlHttpArchive(Guid playbackSessionId, Consts.Speeds speedValue);

        Task<bool> StopArchive(Guid playbackSessionId);
    }
}