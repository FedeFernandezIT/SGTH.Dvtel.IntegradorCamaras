using System;
using System.Xml.Serialization;

namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    [Serializable]
    public class LiveUrl
    {
        [XmlElement("StreamStatus")] public StreamStatus StreamStatus;

        [XmlElement("TranscoderStreamResolution")] public string TranscoderStreamResolution;
        [XmlElement("Url")] public string Url;

        [XmlElement("VideoProfileResolution")] public string VideoProfileResolution;

        [XmlElement("VideoSourceResolution")] public string VideoSourceResolution;

        public LiveUrl()
        {
            Url = "";
            StreamStatus = StreamStatus.GeneralError;
            TranscoderStreamResolution = string.Empty;
            VideoProfileResolution = string.Empty;
            VideoSourceResolution = string.Empty;
        }
    }

    public enum StreamStatus
    {
        Ok = 0,
        [Obsolete] Logical = 1,
        InitializingInProgress = 2,
        GeneralError = 3,
        NoRecordingFound = 4,
        NoAvailableResources = 5,
        SwitchingFailure = 6,
        SwitchingFailure_SourceUnrecognised = 7,
        SwitchingFailure_NoPathAvailable = 8,
        SwitchingFailure_AccessToSourceBarred = 9,
        SwitchingFailure_TransmissionFailure = 10,
        SwitchingFailure_UnableToEstablishRoute = 11,
        NotAuthorized = 12,
        ArchiverUnreachable = 13,
        TranscoderUnreachable = 14,
        UnitDisconnected = 15,
        TryingToConnect = 16,
        NoStreamRequested = 17,
        Testing = 18,
        Unknown = 19,
    }
}