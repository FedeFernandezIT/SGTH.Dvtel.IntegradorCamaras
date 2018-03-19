using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class Clip
    {
        [XmlAttribute]
        public DateTime StartTime { get; set; }

        [XmlAttribute]
        public DateTime EndTime { get; set; }

        [XmlAttribute]
        public bool IsCurrentlyRecording { get; set; }

        [XmlAttribute]
        public Guid CameraId { get; set; }
    }
}