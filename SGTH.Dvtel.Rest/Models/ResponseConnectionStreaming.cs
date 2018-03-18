using System;

namespace SGTH.Dvtel.Rest.Models
{
    public class ResponseConnectionStreaming
    {
        public Guid guid { get; set; }
        public int logicalId { get; set; }
        public string name { get; set; }
        public bool isPtzOnline { get; set; }
        public bool isViewable { get; set; }
        public bool isAccessible { get; set; }
        public bool isEnabled { get; set; }
        public bool isRecording { get; set; }
        public bool IsPtzLocked { get; set; }
        public bool IsPtzEnabled { get; set; }
        public string deviceUrl{ get; set; }
        public string linkedUrl { get; set; }
        public string url { get; set; }
        //public System.Net.IPAddress url2 { get; set; }
        public string clientsConnectionType { get; set; }
        public string deviceDriverExternalId { get; set; }
        public string videoSourceFormat { get; set; }
        public Guid sceneId { get; set; }
        public Guid sessionId { get; set; }
    }
}