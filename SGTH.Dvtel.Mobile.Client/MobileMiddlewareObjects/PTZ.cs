using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class PTZ
    {
        [XmlAttribute("NumberOfPresets")] public int NumberOfPresets;

        public PTZ()
        {
            NumberOfPresets = 0;
        }
    }
}