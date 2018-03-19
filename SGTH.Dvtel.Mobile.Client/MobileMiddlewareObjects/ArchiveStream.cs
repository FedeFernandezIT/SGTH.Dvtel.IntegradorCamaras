using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class ArchiveStream
    {
        [XmlAttribute] public Guid CameraId;
        [XmlAttribute] public DateTime EndTime;
        [XmlAttribute] public Guid SessionId;
        [XmlAttribute] public DateTime StartTime;
        [XmlAttribute] public string StreamStatus;
        [XmlAttribute] public string Url;
    }
}