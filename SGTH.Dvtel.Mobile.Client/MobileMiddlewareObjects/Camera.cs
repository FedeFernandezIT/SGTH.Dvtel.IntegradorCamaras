using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class Camera
    {
        [XmlAttribute]
        public bool CanPlayback { get; set; }
        [XmlAttribute]
        public bool CanRecord { get; set; }
        [XmlAttribute]
        public string Description { get; set; }
        [XmlAttribute]
        public Guid Id { get; set; }
        [XmlAttribute]
        public bool IsAccessible { get; set; }
        [XmlAttribute]
        public bool IsGhost { get; set; }
        [XmlAttribute]
        public bool IsPTZEnabled { get; set; }
        [XmlAttribute]
        public bool IsRecording { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public Guid SiteId { get; set; }
    }
}