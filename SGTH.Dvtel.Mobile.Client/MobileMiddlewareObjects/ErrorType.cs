namespace SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects
{
    public enum ErrorType
    {
        None,
        BadRequest,
        CameraNotAccessible,
        CameraIsGhost,
        CameraNotPTZ,
        AuthorizationFailed,
        CameraNotCached,
        InternalError,
        ArgumentError,
        InvalidSession
    }
}