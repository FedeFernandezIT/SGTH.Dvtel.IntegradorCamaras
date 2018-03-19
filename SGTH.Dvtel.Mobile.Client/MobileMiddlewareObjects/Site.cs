using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class Site
    {
        [XmlAttribute] public Guid Id;

        [XmlAttribute] public string Name;
    }
}