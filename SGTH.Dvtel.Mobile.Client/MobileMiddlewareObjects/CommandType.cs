namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    public enum CommandType
    {
        None,
        Initialize,
        StartLive,
        StartArchive,
        StopArchive,
        Archive,
        Recording,
        PTZ,
        GetCameras,
        GetCamera,
        GetSites,
        QueryClips,
        ControlClip,
        GoToPreset,
        StartPattern,
        StopPattern,
        Stop,
        ReportLocation
    }
}