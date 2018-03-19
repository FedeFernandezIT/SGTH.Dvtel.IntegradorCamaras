using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;

namespace SGTH.Dvtel.Mobile.Client.VideoProviders
{
    // Single Provider per Directory (identified by unique Connection Details)
    public class UnitedVmsProvider
    {
        // Multiton patten - lazy / thread safe
        // ** There will only be a single instance of this class per unique [connectionDetails] string **
        private static readonly ConcurrentDictionary<string, Lazy<UnitedVmsProvider>> Instances = new ConcurrentDictionary<string, Lazy<UnitedVmsProvider>>();

        //  Format must be "<user>:<password>;<hostname>:<port>" (i.e. admin:;localhost:1116)
        public static UnitedVmsProvider GetInstance(string connectionDetails)
        {
            return Instances.GetOrAdd(connectionDetails, connDetails => new Lazy<UnitedVmsProvider>(() => new UnitedVmsProvider(connectionDetails))).Value;
        }

        private readonly HttpClient _httpClient = new HttpClient();

        private readonly Timer _camerasRefreshTimer;

        public string BaseAddress
        {
            get { return _httpClient.BaseAddress.ToString(); }
            set { _httpClient.BaseAddress = new Uri(value); }
        }

        public Guid SessionId { get; set; }

        public List<Camera> Cameras { get; set; }

        // Format must be "<user>:<password>;<hostname>:<port>" (i.e. admin:;localhost:1116)
        private readonly string _connectionDetails; // Value for this multiton instance (it serves as a key for the multiton so we do not allow it to change)

        private event EventHandler<List<Camera>> _cameraListUpdated;
        public event EventHandler<List<Camera>> CameraListUpdated
        {
            add
            {
                if (_cameraListUpdated == null || !_cameraListUpdated.GetInvocationList().Contains(value))
                {
                    _cameraListUpdated += value;
                }
            }
            remove { _cameraListUpdated -= value; }
        }

        private UnitedVmsProvider(string connectionDetails)
        {
            _connectionDetails = connectionDetails;
            BaseAddress = GetBaseAddress();
            Cameras = new List<Camera>();

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));

            _camerasRefreshTimer = new Timer();
            _camerasRefreshTimer.Elapsed += CamerasRefreshTimerElapsed;
        }

        public async Task<bool> Authenticate()
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(GetCredentials())));

                var response = await _httpClient.GetAsync(string.Format("/Initialize?Latitude=0&Longitude=0&DeviceId={0}", Guid.NewGuid()));
                response.EnsureSuccessStatusCode();    // Throw if not a success code.                

                var stream = await response.Content.ReadAsStreamAsync();
                var serializer = new XmlSerializer(typeof(WebResponse));
                var webResponse = serializer.Deserialize(stream) as WebResponse;
                if (webResponse != null)
                {
                    if (webResponse.Header.Error == ErrorType.None)
                    {
                        _httpClient.DefaultRequestHeaders.Authorization = null; // Need to clear so following commands won't be treated as Initialize requests
                        SessionId = webResponse.Header.SessionId;

                        Cameras.Clear();
                        foreach (var camera in webResponse.Body.Cameras)
                        {
                            Utils.Trace(camera.Name + " " + camera.Id);
                            Cameras.Add(camera);
                        }
                        // We must periodically refresh the camera list, otherwise our session will timeout after 5*SRI if AutoLogoff set in AC
                        _camerasRefreshTimer.Interval = TimeSpan.FromSeconds(webResponse.Body.ClientConfiguration.StatusRefreshInterval).TotalMilliseconds;
                        _camerasRefreshTimer.Start();

                        return true; // Authenticated successfully
                    }
                    Utils.Trace("UnitedVmsProvider Authenticate Error: " + webResponse.Header.Error);
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider Authenticate Error: " + ex.Message, ex);
            }
            return false; // Failed authenticating
        }

        public async Task Logout()
        {
            try
            {
                _camerasRefreshTimer.Stop();

                if (SessionId != Guid.Empty)
                {
                    var response = await _httpClient.GetAsync("/Stop?SessionId=" + SessionId);
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.         

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            Cameras.Clear();
                            SessionId = Guid.Empty;
                        }
                        else
                        {
                            Utils.Trace("UnitedVmsProvider Logout Error: " + webResponse.Header.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider Logout Error", ex);
            }
        }

        public async Task GetCameras()
        {
            try
            {
                if (SessionId != Guid.Empty)
                {
                    var response = await _httpClient.GetAsync("/GetCameras?SessionId=" + SessionId);
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.         

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            Cameras.Clear();
                            foreach (var camera in webResponse.Body.Cameras)
                            {
                                Utils.Trace(camera.Name + " " + camera.Id);
                                Cameras.Add(camera);
                            }
                            // Report to registered listeners the updated list of cameras
                            if (_cameraListUpdated != null)
                            {
                                _cameraListUpdated.Invoke(this, Cameras);
                            }
                        }
                        else
                        {
                            Utils.Trace("UnitedVmsProvider GetCameras Error: " + webResponse.Header.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider GetCameras Error", ex);
            }
        }

        private async void CamerasRefreshTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await GetCameras();
        }

        // TODO: MUST HANDLE AUTO-RECOVERY SCENARIOS HERE (CLIENT SIDE)... EXCEPTIONS UPON START_LIVE IS A GOOD EXAMPLE WE MAY NEED A NEW SESSION ...

        /// <summary>
        /// Starts live video stream for the requested cameraId
        /// </summary>
        /// <param name="cameraId"></param>
        /// <param name="compression"></param>        
        /// <returns>The HTTP MJPEG or RTSP H256 Url need to acquire the stream from the Transcoder</returns>
        public async Task<string> StartLive(Guid cameraId, string compression)
        {
            try
            {
                if (SessionId != Guid.Empty)
                {
                    var compressionParam = string.IsNullOrWhiteSpace(compression) ? string.Empty : string.Format("&Compression={0}", compression);
                    var response = await _httpClient.GetAsync(string.Format("/StartLive?CameraGuid={0}&SessionId={1}{2}", cameraId, SessionId, compressionParam));
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.         

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            return webResponse.Body.LiveUrl.Url;
                        }
                        Utils.Trace("UnitedVmsProvider StartLive Error: " + webResponse.Header.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider StartLive  Error", ex);
            }
            return string.Empty;
        }

        /// <summary>
        /// Starts recorded video stream for the requested cameraId
        /// </summary>
        /// <param name="cameraId"></param>       
        /// /// <param name="compression">mjpeg or h264 - if null provided will default to transcoded MJPEG</param>
        /// <param name="startTime">Start time in UTC</param>
        /// <param name="endTime">End time in UTC - optional</param>
        /// <returns>The HTTP MJPEG or RTSP H256 Url need to acquire the stream from the Transcoder - or null upon failure</returns>
        public async Task<ArchiveStream> StartArchive(Guid cameraId, string compression, DateTime startTime, DateTime endTime = default(DateTime))
        {
            ArchiveStream archiveStreamDetails = null;
            try
            {
                if (SessionId != Guid.Empty)
                {
                    var startTimeStr = startTime == default(DateTime) ? "instantreplay" : startTime.ToString(CultureInfo.InvariantCulture);
                    var endTimeStr = endTime == default(DateTime) ? string.Empty : string.Format("&EndTime={0}", endTime);
                    var compressionParam = string.IsNullOrWhiteSpace(compression) ? string.Empty : string.Format("&Compression={0}", compression);

                    // compressionParam is optional (defaults to MJPEG) and endTimeStr is also optional
                    var response = await _httpClient.GetAsync(string.Format("/StartArchive?CameraGuid={0}&SessionId={1}{2}&StartTime={3}{4}", cameraId, SessionId, compressionParam, startTimeStr, endTimeStr));
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        archiveStreamDetails = webResponse.Body.ArchiveStream;
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            Utils.Trace("UnitedVmsProvider StartArchive Url: " + webResponse.Body.ArchiveStream.Url);
                            Utils.Trace("UnitedVmsProvider StartArchive StreamStatus: " + webResponse.Body.ArchiveStream.StreamStatus);
                        }
                        else
                        {
                            Utils.Trace("UnitedVmsProvider StartArchive Error Response: " + webResponse.Header.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider StartArchive  Error", ex);
            }
            return archiveStreamDetails;
        }

        public async Task<bool> ControlHttpArchive(Guid playbackSessionId, Consts.Speeds speedValue)
        {
            bool isSuccess = false;
            try
            {
                if (SessionId != Guid.Empty)
                {
                    var response = await _httpClient.GetAsync(string.Format("/Archive?PlaybackSessionId={0}&SessionId={1}&Speed={2}", playbackSessionId, SessionId, (int)speedValue));
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            isSuccess = true;
                        }
                        else
                        {
                            Utils.Trace("UnitedVmsProvider ControlHttpArchive Error: " + webResponse.Header.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider ControlHttpArchive  Error", ex);
            }
            return isSuccess;
        }

        public async Task<bool> StopArchive(Guid playbackSessionId)
        {
            bool isSuccess = false;
            try
            {
                if (SessionId != Guid.Empty)
                {
                    var response = await _httpClient.GetAsync(string.Format("/StopArchive?PlaybackSessionId={0}&SessionId={1}", playbackSessionId, SessionId));
                    response.EnsureSuccessStatusCode(); // Throw if not a success code.

                    var stream = await response.Content.ReadAsStreamAsync();
                    var serializer = new XmlSerializer(typeof(WebResponse));
                    var webResponse = serializer.Deserialize(stream) as WebResponse;
                    if (webResponse != null)
                    {
                        if (webResponse.Header.Error == ErrorType.None)
                        {
                            isSuccess = true;
                        }
                        else
                        {
                            Utils.Trace("UnitedVmsProvider StopArchive Error: " + webResponse.Header.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider StopArchive  Error", ex);
            }
            return isSuccess;
        }

        private string GetCredentials()
        {
            try
            {
                // m_connectionDetails Format must be "<user>:<password>;<hostname>:<port>" (i.e. admin:;localhost:1116)
                return _connectionDetails.Split(';')[0];
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider GetCredentials Error", ex);
            }
            return string.Empty;
        }

        private string GetBaseAddress()
        {
            try
            {
                // m_connectionDetails Format must be "<user>:<password>;<hostname>:<port>" (i.e. admin:;localhost:1116)
                return "http://" + _connectionDetails.Split(';')[1];
            }
            catch (Exception ex)
            {
                Utils.Trace("UnitedVmsProvider GetBaseAddress Error", ex);
            }
            return string.Empty;
        }
    }
}
