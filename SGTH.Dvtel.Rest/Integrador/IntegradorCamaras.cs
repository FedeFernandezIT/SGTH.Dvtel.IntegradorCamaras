using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using DVTel.API;
using DVTel.API.Entities.Physical;
using DVTel.API.Entities.Physical.Enums;
using DVTel.API.Entities.SystemObjects;
using DVTel.Common.AssemblyLoader;
using SGTH.Dvtel.Rest.Models;

//TODO No se encuentra namespace
//using DvTelIntegradorCamaras.Helpers;
namespace SGTH.Dvtel.Rest.Integrador
{
    public class IntegradorCamaras : IIntegradorCamaras
    {
        #region Data Members
        private IDvtelSystemId m_DvtelSystem;
        private PTZDevice objCameraPTZ = new PTZDevice();

        private ModelResponseMethod objModelResponseMethod = new ModelResponseMethod();


        #endregion

        #region Constantes estáticas
        public const string MP4_FILE_FORMAT = "MP4";
        #endregion


        private Guid federationIdSystemDvTel;
        private string usernameSystemDvTel;
        private string passwordSystemDvTel;
        private string directorySystemDvTel;
        private string ipDirectory = ConfigurationManager.AppSettings["IpDirectory"];
        private string pathSnapshot;
        private string ipFtp = ConfigurationManager.AppSettings["IpFtp"];
        private double waitForFinish = double.Parse(ConfigurationManager.AppSettings["WaitForFinish"]);
        private int intentosReconexion = 0;
        IVideoInSceneEntity camera;
        IConfigurationEntity entity;
        public IntegradorCamaras()
        {
            DVTelInit.InitApplication();
        }

        #region Methods Login/Logout System DvTel
        /// <summary>
        /// Login a sistema DvTel con Directorio o IP, Username y Password, pasados como parámetros
        /// </summary>        
        public object Login(string directory, string username, string password)
        {
            try
            {
                m_DvtelSystem = DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Login(directory, username, password, new LoginCacheRequest(false));
                return true;
            }
            catch (Exception errorLoginId)
            {
                if (errorLoginId.Message.Contains("login"))
                {
                    return ResponseMethod(CodeStatus.UNAUTHORIZED, errorLoginId.Message, null);
                }

                if (intentosReconexion < 3)
                {
                    if (CopyDll())
                    {
                        Login(directory, username, password);
                    }
                }
                return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorLoginId.Message, null);
            }
        }

        /// <summary>
        /// Login (basico) a sistema DvTel con ID de Federation (Guid), Username y Password, pasados como parámetros
        /// </summary>
        public object LoginGuid(Guid federationId, string username, string password)
        {
            intentosReconexion++;
            try
            {
                m_DvtelSystem = DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Login(federationId, username, password, new LoginCacheRequest(false));
                return true;
            }
            catch (Exception errorLoginGuid)
            {
                if (errorLoginGuid.Message.Contains("login"))
                {
                    return ResponseMethod(CodeStatus.UNAUTHORIZED, errorLoginGuid.Message, null);
                }

                if (intentosReconexion < 3)
                {
                    if (CopyDll())
                    {
                        LoginGuid(federationId, username, password);
                    }
                }
                return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorLoginGuid.Message, null);
            }
        }

        /// <summary>
        /// Login (basico) a sistema DvTel con ID de Federation (Guid), Username y Password obtenidos desde web.config
        /// </summary>
        public object LoginGuid()
        {
            intentosReconexion++;
            try
            {
                federationIdSystemDvTel = Guid.Parse(ConfigurationManager.AppSettings["FederationId"]);
                usernameSystemDvTel = ConfigurationManager.AppSettings["Username"];
                passwordSystemDvTel = ConfigurationManager.AppSettings["Password"];
                m_DvtelSystem = DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Login(federationIdSystemDvTel, usernameSystemDvTel, passwordSystemDvTel, new LoginCacheRequest(false));
                return true;
            }
            catch (Exception errorLoginGuid)
            {
                if (errorLoginGuid.Message.Contains("login"))
                {
                    return ResponseMethod(CodeStatus.UNAUTHORIZED, errorLoginGuid.Message, null);
                }
                if (intentosReconexion < 3)
                {
                    if (CopyDll())
                    {
                        LoginGuid();
                    }
                }
                return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorLoginGuid.Message, null);
            }
        }

        /// <summary>
        /// Cierre de session del sistema DvTel
        /// </summary>
        public void Logout()
        {
            try
            {
                DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Logout(m_DvtelSystem);
            }
            catch { }
            try
            {
                //DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Shutdown();
            }
            catch
            {
            }
        }
        #endregion

        #region -----------------------------Methods PTZ device
        #region Methods Information PTZ

        /// <summary>
        /// Obtiene los datos de un dispositivo de video ,sirve de método auxiliar para la obtención de información.
        /// </summary>
        private PTZDevice GetDataPtz(IDvtelSystemId latitudeSystem, IVideoInSceneEntity camera)
        {
            objCameraPTZ = new PTZDevice();
            var deviceApi = latitudeSystem.GetAPI<IDeviceAPI>();
            var ptzClient = deviceApi.AcquirePTZControl(camera);

            var m_playerAPI = latitudeSystem.GetAPI<IPlayerAPI>();
            
            /*if (ptzClient != null)
            {*/
            objCameraPTZ.canFocus = ptzClient!=null? ptzClient.CanFocus: false;
                objCameraPTZ.canIris = ptzClient != null ? ptzClient.CanIris : false;
                objCameraPTZ.canPanTilt = ptzClient != null ? ptzClient.CanPanTilt : false;
                objCameraPTZ.canZoom = ptzClient != null ? ptzClient.CanZoom : false;
                objCameraPTZ.isLocked = ptzClient != null ? ptzClient.IsLocked : false;
                objCameraPTZ.numberOfPatterns = ptzClient != null ? ptzClient.NumberOfPatterns:0;
                objCameraPTZ.numberOfPresets = ptzClient != null ? ptzClient.NumberOfPresets:0;
                objCameraPTZ.sceneId = ptzClient != null ? ptzClient.SceneId:Guid.Empty;
                objCameraPTZ.sessionId = ptzClient != null ? ptzClient.SessionId : Guid.Empty;


                var locationBaseEntity = ptzClient != null ? ptzClient.GetCurrentLocation():null;
            if (locationBaseEntity is IPtzMotorNormalized3DLocationEntity)
            {
                var location = (IPtzMotorNormalized3DLocationEntity)locationBaseEntity;
                objCameraPTZ.panValue = location.PanValue.ToString();
                objCameraPTZ.tiltValue = location.TiltValue.ToString();
                objCameraPTZ.zoomValue = location.ZoomValue.ToString();
            }

            if (locationBaseEntity is IPtzMotor3DLocationEntity)
            {
                var absLocation = (IPtzMotor3DLocationEntity)locationBaseEntity;
                objCameraPTZ.panValue = absLocation.PanValue;
                objCameraPTZ.tiltValue = absLocation.TiltValue;
                objCameraPTZ.zoomValue = absLocation.ZoomValue;
            }
            
            

            PTZDevice.geoLocation geographicLocation = new PTZDevice.geoLocation();
            entity = m_DvtelSystem.AdministrationAPI.GetCachedEntity(camera.Id);


            var objIPTZMotorEntity2 = entity as IPTZMotorEntity;
            IAdminCenterEntity objIAdminCenterEntity = entity as IAdminCenterEntity;

            bool isReady = false;
            ISceneGeoLocationEntity objISceneGeoLocationEntity;
            objISceneGeoLocationEntity = entity as ISceneGeoLocationEntity;
            if (objISceneGeoLocationEntity!=null)
            {
                isReady = true;                
            }

            objISceneGeoLocationEntity = camera as ISceneGeoLocationEntity;
            if (objISceneGeoLocationEntity != null)
            {
                isReady = true;
            }
            dynamic ipstream = camera.GetStreamSourceIp();

            var eje=camera.DvtelSystem.CreateEntity<ISceneGeoLocationEntity>();

            

            if (isReady)
            {
                geographicLocation.latitude = objISceneGeoLocationEntity.Latitude;
                geographicLocation.altitude = objISceneGeoLocationEntity.Altitude;
                geographicLocation.longitude = objISceneGeoLocationEntity.Longitude;
                objCameraPTZ.geographicLocation = geographicLocation;
            }
            eje = entity as ISceneGeoLocationEntity;

            dynamic entityReferencer = entity.GetEntityReferencers()[0].Entity;
            try
            {
                objCameraPTZ.firmware = entityReferencer.FirmwareVersion;
            } catch
            {
                objCameraPTZ.firmware =  string.Empty;
            }
            try
            {
                objCameraPTZ.model = entityReferencer != null ? entityReferencer.Model : string.Empty;
            }
            catch
            {
                objCameraPTZ.model = string.Empty;
            }
            try
            {
                objCameraPTZ.pluginName = entityReferencer != null ? entityReferencer.Plugin.Name : string.Empty;
            }
            catch
            {
                objCameraPTZ.pluginName = string.Empty;
            }
            try
            {
                objCameraPTZ.pluginVersion = entityReferencer != null ? entityReferencer.pluginVersion : string.Empty;
            }
            catch
            {
                objCameraPTZ.pluginVersion = string.Empty;
            }

            

            var DataEntities = camera.ExternalDataEntities;
            var ExternalPluginWorkspaces=camera.ExternalPluginWorkspaces;
            var GetActiveSchedules = camera.GetActiveSchedules();
            var extendedTypes=camera.GetAllExtendedTypes();
            var detetionurl=camera.GetCameraMotionDetectionSettingsUrl();
            var metadata=camera.GetEntityMetaData();
            var cam_entityref=camera.GetEntityReferencers();

            var locationCamera = camera.GetPropertyLocalizationEnum(SceneGeoLocationEntityEnum.Altitude);
            locationCamera = camera.GetPropertyLocalizationEnum(SceneGeoLocationEntityEnum.Latitude);
            locationCamera = camera.GetPropertyLocalizationEnum(SceneGeoLocationEntityEnum.Longitude);

            
            objCameraPTZ.description = camera.Description;
            objCameraPTZ.guid = camera.Id;
            objCameraPTZ.isPtzOnline = camera.IsPtzOnline;
            objCameraPTZ.logicalId = camera.LogicalId;
            objCameraPTZ.name = camera.Name;
            objCameraPTZ.creationTime = camera.CreationTime;
            objCameraPTZ.isViewable = camera.IsViewable;
            objCameraPTZ.deviceUrl = camera.GetCameraGeneralPageUrl();
            
            dynamic dyCamera = camera;
            dynamic dyEntity = entity;
            dynamic entityDynamic = entity;
            var logiclaloc = entityDynamic.LogicalLocation;
            
            try
            {
                foreach (var vEvent in camera.EventHandlers.Values.ToList())
                {
                    objCameraPTZ.supportedEvents += vEvent.Caption + " ";
                }
            }
            catch
            { objCameraPTZ.supportedEvents = ""; }

            objCameraPTZ.clientsConnectionType = camera.ClientsConnectionType!=null? camera.ClientsConnectionType.ToString():string.Empty;
            objCameraPTZ.detailedInformation = camera.DetailedInformation;
            objCameraPTZ.deviceDriverExternalId =camera.DeviceDriver!=null?camera.DeviceDriver.ExternalId.ToString():string.Empty;
            objCameraPTZ.isAccessible = camera.IsAccessible;
            objCameraPTZ.isEnabled = camera.IsEnabled;
            objCameraPTZ.linkedUrl = camera.LinkedUrl!=null? camera.LinkedUrl.Url:string.Empty;
            objCameraPTZ.videoSourceFormat = camera.VideoSourceFormat.ToString();

           // dynamic dymLoc=GetLoc

            try
            {
                foreach (var vAddress in camera.Addresses.Values.ToList())
                {
                    objCameraPTZ.address += vAddress.Caption + " ";
                }
            }
            catch
            {
                objCameraPTZ.address = "";
            }
            objCameraPTZ.timeZone = camera.TimeZone.ToString();            
            objCameraPTZ.IsPtzLocked = camera.IsPtzLocked;
            objCameraPTZ.IsPtzEnabled = camera.IsPtzEnabled;
            //}
            return objCameraPTZ;
        }
        /// <summary>
        /// Obtiene la información de los dispositivos de video en el sistema.
        /// </summary>
        public object GetCamerasPTZ()
        {
            if (LoginGuid() is bool)
            {
                var listCamaras = new List<object>();
                foreach (IVideoInSceneEntity camera in m_DvtelSystem.AdministrationAPI.GetCachedEntitiesOfType(typeof(IVideoInSceneEntity), null).Values)
                {
                    /*if (camera.IsPtzEnabled)
                    {*/
                    objCameraPTZ = new PTZDevice();
                    objCameraPTZ = GetDataPtz(m_DvtelSystem, camera);
                    listCamaras.Add(objCameraPTZ);
                    //}
                }
                if (listCamaras.Count < 1)
                {
                    return ResponseMethod(CodeStatus.NOT_FOUND, "No se encontraron dispositivos tipo PTZ", null);
                }
                return ResponseMethod(CodeStatus.OK, string.Empty, listCamaras);
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Permite consultar la información de un dispositivo de video determinado por su Id .
        /// </summary>
        public object GetPTZUnitByGuid(Guid guid)
        {
            if (LoginGuid() is bool)
            {
                objCameraPTZ = new PTZDevice();
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    objCameraPTZ = GetDataPtz(m_DvtelSystem, camera);
                    return ResponseMethod(CodeStatus.OK, string.Empty, objCameraPTZ);
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Permite consultar la información de un dispositivo de video determinado por su Id lógico.
        /// </summary>
        public object GetPTZUnitById(int id)
        {
            if (LoginGuid() is bool)
            {
                objCameraPTZ = new PTZDevice();
                try
                {
                    foreach (IVideoInSceneEntity camera in m_DvtelSystem.AdministrationAPI.GetCachedEntitiesOfType(typeof(IVideoInSceneEntity), null).Values)
                    {
                        if (camera.LogicalId == id)
                        {
                            if (EstadoDispositivoPtz(camera.Id) is bool)
                            {
                                objCameraPTZ = GetDataPtz(m_DvtelSystem, camera);
                                return ResponseMethod(CodeStatus.OK, string.Empty, objCameraPTZ);
                            }
                            return objModelResponseMethod;
                        }
                    }
                    if (objCameraPTZ == null)
                    {
                        return ResponseMethod(CodeStatus.NOT_FOUND, "El dispositivo no es un dispositivo de video", null);
                    }
                    return ResponseMethod(CodeStatus.OK, string.Empty, objCameraPTZ);
                }
                catch (Exception errorGet)
                {
                    return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorGet.Message, null);
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Permite consultar sobre la existencia de un dispositivo de video.
        /// </summary>
        public object ExistsPTZ(Guid guid)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    return ResponseMethod(CodeStatus.OK, string.Empty, null);
                }
            }
            return objModelResponseMethod;
        }
        #endregion

        #region Methods for movement...                
        /// <summary>
        /// Mueve horizontalmente el dispositivo de video.
        /// </summary>
        public object GetTiltPTZ(Guid guid, int tiltAction, int tiltSpeed)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);
                    try
                    {
                        switch (tiltAction)
                        {
                            case 0:
                                client.Tilt(TiltAction.Up, tiltSpeed);
                                break;
                            case 1:
                                client.Tilt(TiltAction.Down, tiltSpeed);
                                break;
                            case 2:
                                client.Tilt(TiltAction.Stop, tiltSpeed);
                                break;
                            default:
                                return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "TiltAction inválido. Valores válidos [0 | 1 | 2]", null);
                        }
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorTilt)
                    {
                        return ResponseMethod(CodeStatus.ERROR, errorTilt.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }

        /// <summary>
        /// Mueve verticalmente el dispositivo de video.
        /// </summary>
        public object GetPanPTZ(Guid guid, int panAction, int panSpeed)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);
                    try
                    {
                        switch (panAction)
                        {
                            case 0:
                                client.Pan(PanAction.Left, panSpeed);
                                break;
                            case 1:
                                client.Pan(PanAction.Right, panSpeed);
                                break;
                            case 2:
                                client.Pan(PanAction.Stop, panSpeed);
                                break;
                            default:
                                return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "PanAction inválido. Valores válidos [0 | 1 | 2]", null);
                        }
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorPan)
                    {
                        return ResponseMethod(CodeStatus.ERROR, errorPan.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Realiza el zoom del dispositivo de video.
        /// </summary>
        public object GetZoomPTZ(Guid guid, int zoomAction, int zoomSpeed)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);
                    try
                    {
                        switch (zoomAction)
                        {
                            case 0:
                                client.Zoom(ZoomAction.Wide, zoomSpeed);
                                break;
                            case 1:
                                client.Zoom(ZoomAction.Tele, zoomSpeed);
                                break;
                            case 2:
                                client.Zoom(ZoomAction.Stop, zoomSpeed);
                                break;
                            default:
                                return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "ZoomAction inválido. Valores válidos [0 | 1 | 2]", null);
                        }
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorPan)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorPan.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }

        /// <summary>
        /// Mueve (horizontal o verticalmente) y realiza el zoom del dispositivo de video.
        /// </summary>
        public object GetzoomAndMove(Guid guid, int panSpeed, int tiltSpeed, int zoomSpeed)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);

                    var location = camera.DvtelSystem.CreateEntity<IPtzMotorNormalized3DLocationEntity>();
                    location.InitializeAsNew(Guid.NewGuid());
                    location.PanValue = panSpeed;
                    location.TiltValue = tiltSpeed;
                    location.ZoomValue = zoomSpeed;
                    try
                    {
                        client.GotoLocation(location);
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorGotoLocation)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorGotoLocation.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Detiene el movimiento (horizontal o vertical) del dispositivo de video.
        /// </summary>
        public object GetStopPTZ(Guid guid)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);
                    try
                    {
                        client.Pan(PanAction.Stop, 0);
                        client.Tilt(TiltAction.Stop, 0);
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorGotoLocation)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorGotoLocation.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Mueve el dispositivo de video a una posición predeterminada
        /// </summary>
        public object GoToPreset(Guid guid, int idPreset)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var client = deviceApi.AcquirePTZControl(camera);
                    try
                    {
                        client.GotoPresetNumber(idPreset);
                        return ResponseMethod(CodeStatus.OK, string.Empty, null);
                    }
                    catch (Exception errorPreset)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorPreset.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }

        #endregion

        #region Frame, Streaming PTZ
        /// <summary>
        /// Obtiene un frame (snapshot) online de un dispositivo de video determinado
        /// </summary>
        public object GetFrameLivePTZ(Guid guid)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    try
                    {
                        var objResponseFramePtz = new ResponseFramePtz();
                        //var fileName = "Snapshot_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";
                        var fileName = camera.Name + "(" + camera.LogicalId + ") " + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".jpg";

                        pathSnapshot = ConfigurationManager.AppSettings["PathSnapshot"] + "\\" + fileName;
                        var directoryCompartido = ConfigurationManager.AppSettings["DirectorioCompartido"];
                        //creo archivo con datos del dispositivo PTZ y el nombre de archivo/path del snapshot
                        FileToWfDvtel(".spl", guid, directoryCompartido, pathSnapshot, "", "");
                        
                        //bucle de espera para darle tiempo al winforms a que tome la foto
                        var t2 = 0;
                        while (t2 < waitForFinish)
                        {
                            // Un respiro para el sitema
                            System.Windows.Forms.Application.DoEvents();
                            t2++;
                        }
                        try
                        {
                            var imageArray = System.IO.File.ReadAllBytes(pathSnapshot.Replace(".jpg", "Compress.jpg"));
                            var base64ImageRepresentation = Convert.ToBase64String(imageArray);

                            objResponseFramePtz.Base64ImageRepresentation = base64ImageRepresentation;
                            objResponseFramePtz.TypeImage = "jpg";
                            return ResponseMethod(CodeStatus.OK, string.Empty, objResponseFramePtz);
                        }
                        catch (Exception errorSnapshot)
                        {
                            return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorSnapshot.Message, null);
                        }
                    }
                    catch (Exception errorFrame)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorFrame.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Obtiene un frame (snapshot) de un periodo determinado y de un dispositivo de video determinado
        /// </summary>
        public object GetFramePTZ(Guid guid, string dateFrame)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    try
                    {
                        var objResponseFramePtz = new ResponseFramePtz();

                        var fileName = camera.Name + "(" + camera.LogicalId + ") " + DateTime.Parse(dateFrame).ToString("yyyyMMdd_HHmmss") + ".jpg";
                        pathSnapshot = ConfigurationManager.AppSettings["PathSnapshot"] + "\\" + fileName;
                        var directoryCompartido = ConfigurationManager.AppSettings["DirectorioCompartido"];
                        //creo archivo con datos del dispositivo PTZ, credenciales para conectarse a DvTel SyStem y el nombre de archivo/path del snapshot
                        FileToWfDvtel(".spt", guid, directoryCompartido, pathSnapshot, dateFrame, "");
                        //bucle de espera para darle tiempo al winforms a que tome la foto
                        var t2 = 0;
                        while (t2 < waitForFinish)
                        {
                            // Un respiro para el sitema
                            System.Windows.Forms.Application.DoEvents();
                            t2++;
                        }
                        try
                        {
                            var imageArray = File.ReadAllBytes(pathSnapshot.Replace(".jpg", "Compress.jpg"));
                            var base64ImageRepresentation = Convert.ToBase64String(imageArray);

                            objResponseFramePtz.Base64ImageRepresentation = base64ImageRepresentation;
                            objResponseFramePtz.TypeImage = "jpg";
                            return ResponseMethod(CodeStatus.OK, string.Empty, objResponseFramePtz);
                        }
                        catch (Exception errorSnapshot)
                        {
                            return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorSnapshot.Message, null);
                        }
                    }
                    catch (Exception errorFrame)
                    {
                        return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, errorFrame.Message, null);
                    }
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Obtiene los datos necesarios para la conexión streaming hacia un dispositivo de video
        /// </summary>
        public object GetConnectionStreamingPTZ(Guid guid)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    var objResponseConnectionStreaming = new ResponseConnectionStreaming();
                    var deviceApi = m_DvtelSystem.GetAPI<IDeviceAPI>();
                    var ptzClient = deviceApi.AcquirePTZControl(camera);
                    objResponseConnectionStreaming.guid = camera.Id;
                    objResponseConnectionStreaming.isPtzOnline = camera.IsPtzOnline;
                    objResponseConnectionStreaming.logicalId = camera.LogicalId;
                    objResponseConnectionStreaming.name = camera.Name;
                    objResponseConnectionStreaming.isViewable = camera.IsViewable;
                    objResponseConnectionStreaming.deviceUrl = camera.GetCameraGeneralPageUrl();
                    try
                    {
                        objResponseConnectionStreaming.clientsConnectionType = camera.ClientsConnectionType.ToString();
                    }
                    catch
                    {
                        objResponseConnectionStreaming.clientsConnectionType = "";
                    }
                    try
                    {
                        objResponseConnectionStreaming.deviceDriverExternalId = camera.DeviceDriver.ExternalId.ToString();
                    }
                    catch { objResponseConnectionStreaming.deviceDriverExternalId = ""; }
                    objResponseConnectionStreaming.isAccessible = camera.IsAccessible;
                    objResponseConnectionStreaming.isEnabled = camera.IsEnabled;
                    objResponseConnectionStreaming.isRecording = camera.IsRecording;
                    try
                    {
                        objResponseConnectionStreaming.linkedUrl = camera.LinkedUrl.Url;
                    }
                    catch { objResponseConnectionStreaming.linkedUrl = ""; }
                    objResponseConnectionStreaming.videoSourceFormat = camera.VideoSourceFormat.ToString();

                    objResponseConnectionStreaming.IsPtzLocked = camera.IsPtzLocked;
                    objResponseConnectionStreaming.IsPtzEnabled = camera.IsPtzEnabled;
                    objResponseConnectionStreaming.sceneId = ptzClient.SceneId;
                    objResponseConnectionStreaming.sessionId = ptzClient.SessionId;
                    objResponseConnectionStreaming.url = "dvnp://" + usernameSystemDvTel + ":" + passwordSystemDvTel + "@" + ipDirectory + "?id=" + camera.LogicalId + "&Trace=debug";//urlEntity.Url;
                    return ResponseMethod(CodeStatus.OK, string.Empty, objResponseConnectionStreaming);
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Exporta un clip de video de un periodo determinado y de un dispositivo de video determinado
        /// </summary>
        public object ExportVideoPTZ(Guid guid, string fromDate, string toDate)
        {
            if (LoginGuid() is bool)
            {
                if (EstadoDispositivoPtz(guid) is bool)
                {
                    //obtener video/clips almacenado
                    IReadWriteEntitiesCollection<IRecordableSceneEntity> recordableScenes = DVTel.API.Common
                        .DVTelObjectsFactory.Instance
                        .CreateObject<IReadWriteEntitiesCollection<IRecordableSceneEntity>>();
                    recordableScenes.Add((IRecordableSceneEntity)entity);
                    IRecordableSceneEntity scene = (IRecordableSceneEntity)entity;
                    IDvtelSystemId dvtelSystem = scene.DvtelSystem; // get a Recording API that belongs to our specific federation (system)
                    IRecordingAPI recordingApi = dvtelSystem.GetAPI<IRecordingAPI>();
                    // create a filter for the query
                    QueryClipsFilter filter = new QueryClipsFilter(recordableScenes.AsReadOnly(),
                        DateTime.Parse(fromDate), DateTime.Parse(toDate), 1);
                    filter.ArchivingReasons = ArchivingReason.All;
                    filter.ProtectionFilter = ProtectionFilterEnum.NotProtected;
                    bool cropped;

                    var objResponseExportVideo = new ResponseExportVideo();
                    IReadonlyEntitiesCollection<IClipEntity> clips = recordingApi.QueryClips(filter, out cropped);
                    if (clips != null)
                    {
                        var directoryCompartido = ConfigurationManager.AppSettings["DirectorioCompartido"];
                        var guidAleatorio = Guid.NewGuid();
                        var fileName = guidAleatorio + ".mp4";
                        pathSnapshot = ConfigurationManager.AppSettings["PathSnapshot"] + "\\" + fileName;
                        FileToWfDvtel(".evc", guid, directoryCompartido, pathSnapshot, fromDate, toDate);
                        objResponseExportVideo.idExportSession = guidAleatorio;
                        objResponseExportVideo.path = null;
                        objModelResponseMethod.Status = CodeStatus.OK;
                        objModelResponseMethod.Msg = string.Empty;
                        objModelResponseMethod.Data = objResponseExportVideo;
                        return ResponseMethod(CodeStatus.OK, string.Empty, objResponseExportVideo);                        
                    }
                    return ResponseMethod(CodeStatus.NOT_FOUND, "Video no encontrado con esos parámetros", null);
                }
            }
            return objModelResponseMethod;
        }
        /// <summary>
        /// Descarga un video exportado previamente (o permite consultar sobre su estado de exportacion)
        /// </summary>
        public object DownloadVideo(Guid idExport)
        {
            var targetFilePath = ConfigurationManager.AppSettings["PathSnapshot"];
            var objDirectoryInfo = new DirectoryInfo(targetFilePath);
            if (objDirectoryInfo.Exists)
            {
                var videosExport = objDirectoryInfo.GetFiles(idExport + ".mp4");
                if (videosExport.Length > 0)
                {
                    foreach (var path in videosExport.ToList())
                    {
                        //elimino el .tmp
                        File.Delete(path.FullName.Replace(".mp4", ".tmp"));

                        /*var client = new WebClient();
                        client.DownloadFileAsync(new Uri(path.FullName), @"c:\" + path.Name);*/

                        // TODO: Se comenta porque no se encuentra el objeto Utils.cs en el namespace DvTelIntegradorCamaras.Helpers
                        //var objUtils = new Utils();
                        //objUtils.Download(path.FullName);
                    }
                    return ResponseMethod(CodeStatus.OK, "", null);
                }
                var videosTmpExport = objDirectoryInfo.GetFiles(idExport + ".tmp");
                if (videosTmpExport.Length > 0)
                {
                    return ResponseMethod(CodeStatus.NOT_FOUND, "El proceso de exportación aún no finalizó.", null);
                }
            }
            return ResponseMethod(CodeStatus.INTERNAL_SERVER_ERROR, "No se encontró el archivo exportado.", null);
        }

        #endregion

        #endregion -----------------------------Methods PTZ device

        #region Methods private
        /// <summary>
        /// Reemplaza 'bin\*.dll' por 'bckp\*.dll' en ejecucion por problemas con cierre de sdk
        /// </summary>
        private bool CopyDll()
        {
            //reemplazo la dll DVTel.API.Entities.SystemObjects xq de seguro se clavó
            var sourceFile = System.Web.Hosting.HostingEnvironment.MapPath("~/bckp/");
            var destFile = System.Web.Hosting.HostingEnvironment.MapPath("~/bin/");
            var pathDll = System.Web.Hosting.HostingEnvironment.MapPath("~/bckp");
            var objDirectoryInfo = new DirectoryInfo(pathDll);

            File.Delete(destFile+ "DVTel.API.Entities.SystemSettings.dll");


            if (objDirectoryInfo.Exists)
            {
                foreach (var nameDll in objDirectoryInfo.GetFiles().ToList())
                {
                    try
                    {
                        //copio el archivo y sobreescribo el q ya está lojado alli
                        File.Copy(sourceFile + nameDll.Name, destFile + nameDll.Name, true);
                    }
                    catch (Exception erroCopy) { }
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// Controla el estado del dispositivo de video que está por utilizar.
        /// </summary>
        private object EstadoDispositivoPtz(Guid guid)
        {
            if (m_DvtelSystem == null)
            {
                return objModelResponseMethod;
            }
            try
            {
                entity = m_DvtelSystem.AdministrationAPI.GetCachedEntity(guid);
            }
            catch
            {
                return ResponseMethod(CodeStatus.NOT_FOUND, "No se encontró el dispositivo solicitado.", null);
            }
            if (entity is IVideoInSceneEntity)
            {
                camera = (IVideoInSceneEntity)entity;
                /*if (!camera.IsPtzEnabled)
                {
                    return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo " + camera.Name + " no es un dispositivo PTZ.", null);
                }*/
                if (!camera.IsAccessible)
                {
                    return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo " + camera.Name + "  no está disponible.", null);
                }
                if (!camera.IsEnabled)
                {
                    return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo " + camera.Name + "  no está habilitado.", null);
                }
                if (camera.IsPtzLocked)
                {
                    return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo " + camera.Name + "  está bloqueado.", null);
                }
                if (!camera.IsPtzOnline)
                {
                    return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo " + camera.Name + "  no está Onlime.", null);
                }
            }
            else
            {
                return ResponseMethod(CodeStatus.METHOD_NOT_ALLOWED, "El dispositivo no es un dispositivo de video.", null);
            }
            return true;
        }
        /// <summary>
        /// Objeto que retornará la respuesta del resultado del método.
        /// </summary>
        public ModelResponseMethod ResponseMethod(string codeStatus, string message, object result)
        {
            Logout();
            objModelResponseMethod.Status = codeStatus;
            objModelResponseMethod.Msg = message;
            objModelResponseMethod.Data = result;
            return objModelResponseMethod;
        }
        /// <summary>
        /// Crea archivos de configuracion para snapshot y exportar video en carpeta compartida con la aplicacion de escritorio
        /// </summary>        
        private void FileToWfDvtel(string extension, Guid guid, string path, string pathFile, string dateFrame, string toDate)
        {
            var objDirectoryInfo = new DirectoryInfo(path);
            if (!objDirectoryInfo.Exists)
            {
                objDirectoryInfo.Create();
            }
            var text = "";
            switch (extension)
            {
                case ".spt"://snapshot
                    text = guid + "|" + pathFile + "|" + dateFrame;
                    break;
                case ".spl"://snapshot live
                    text = guid + "|" + pathFile;
                    break;
                case ".evc"://export videoclip
                    text = guid + "|" + pathFile + "|" + dateFrame + "|" + toDate;
                    break;
            }
            File.WriteAllText(path + @"\data" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + extension, text);
        }


        #endregion
    }
}