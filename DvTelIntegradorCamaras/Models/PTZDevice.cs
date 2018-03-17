using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DVTel.API;
using DVTel.API.Application.Common.Driver;
using DVTel.API.Entities.Physical.Enums;
using DvTelIntegradorCamaras.Exceptions;

namespace DvTelIntegradorCamaras.Models
{
    public class PTZDevice
    {
        public Guid guid { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string timeZone { get; set; }
        public string clientsConnectionType { get; set; }
        public string description { get; set; }
        public string detailedInformation { get; set; }
        public string deviceDriverExternalId { get; set; }
        public bool isAccessible { get; set; }
        public bool isEnabled { get; set; }
        public bool isRecording { get; set; }
        public string linkedUrl { get; set; }
        public string deviceUrl { get; set; }
        public int logicalId { get; set; }
        public string videoSourceFormat { get; set; }
        public bool canFocus { get; set; }
        public bool canIris { get; set; }
        public bool canPanTilt { get; set; }
        public bool canZoom { get; set; }
        public bool isLocked { get; set; }
        public int numberOfPatterns { get; set; }
        public int numberOfPresets { get; set; }
        public Guid sceneId { get; set; }
        public Guid sessionId { get; set; }
        public string panValue { get; set; }
        public string tiltValue { get; set; }
        public string zoomValue { get; set; }
        public bool isPtzOnline { get; set; }
        public bool IsPtzLocked { get; set; }
        public bool IsPtzEnabled { get; set; }
        public DateTime creationTime { get; set; }
        public bool isViewable { get; set; }
        public object supportedEvents { get; set; }
        public geoLocation geographicLocation { get; set; }


        public class geoLocation
        { 
            public double latitude { get; set; }
            public double altitude { get; set; }

            public double longitude { get; set; }
        }



        /*IPTZClient _ptz;

        public PTZDevice(IPTZClient client)
        {
            _ptz = client;
        }

        public PTZDevice()
        {
        }

        public void Tilt(TiltAction tiltAction, int tiltSpeedPercentage)
        {
            try
            {
                _ptz.Tilt(tiltAction, tiltSpeedPercentage);
                Console.WriteLine("[DONE TILT] PTZ=" + _ptz.Id + " action=" + tiltAction + " speedPercentage=" + tiltSpeedPercentage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw new DvTelPTZMalfunctionException();
            }
        }

        public void Pan(PanAction panAction, int panSpeedPercentage)
        {
            try
            {
                _ptz.Pan(panAction, panSpeedPercentage);
                Console.WriteLine("[DONE PAN] PTZ=" + _ptz.Id + " action=" + panAction + " speedPercentage=" + panSpeedPercentage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw new DvTelPTZMalfunctionException();
            }
        }

        public void Zoom(ZoomAction zoomAction, int zoomSpeedPercentage)
        {
            try
            {
                _ptz.Zoom(zoomAction, zoomSpeedPercentage);
                Console.WriteLine("[DONE ZOOM] PTZ=" + _ptz.Id + " action=" + zoomAction + " speedPercentage=" + zoomSpeedPercentage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw new DvTelPTZMalfunctionException();
            }
        }

        public void ZoomLensSpeed(int zoomSpeedPercentage)
        {
            // TODO: verificar speedPercentage (1-100)
            try
            {
                _ptz.ZoomLensSpeed(zoomSpeedPercentage);
                Console.WriteLine("[DONE ZOOM LENS SPEED] PTZ=" + _ptz.Id + " speedPercentage=" + zoomSpeedPercentage);
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
                throw new DvTelPTZMalfunctionException();
            }
        }*/
    }
}