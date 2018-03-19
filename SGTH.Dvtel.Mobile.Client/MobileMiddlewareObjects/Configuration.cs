using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class Configuration
    {
        [XmlAttribute("ExternalCapabilities")] public bool ExternalCapabilities;
        [XmlAttribute("InstantReplayInterval")] public uint InstantReplayInterval;

        [XmlAttribute("SendGPS")] public bool SendGPS;
        [XmlAttribute("StatusRefreshInterval")] public uint StatusRefreshInterval;

        public Configuration()
        {
            //default = every minute
            StatusRefreshInterval = 60;
            InstantReplayInterval = 30;
            ExternalCapabilities = false;
            SendGPS = true;
        }
    }
}