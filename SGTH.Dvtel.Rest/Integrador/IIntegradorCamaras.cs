using System;

namespace SGTH.Dvtel.Rest.Integrador
{
    public interface IIntegradorCamaras
    {
        #region -----------------------------Methods PTZ device
        #region Methods Information PTZ
        object GetPTZUnitById(int id);
        object GetPTZUnitByGuid(Guid guid);
        object ExistsPTZ(Guid guid);
        object GetCamerasPTZ();
        #endregion

        object GetTiltPTZ(Guid guid, int tilt, int speed);
        object GetPanPTZ(Guid guid, int pan, int speed);
        object GetZoomPTZ(Guid guid, int zoom, int speed);
        object GetzoomAndMove(Guid guid, int panSpeed, int tiltSpeed, int zoomSpeed);
        object GoToPreset(Guid guid, int idPreset);
        object GetStopPTZ(Guid guid);

        object GetFramePTZ(Guid guid,string dateFrame);
        object GetConnectionStreamingPTZ(Guid guid);
        object GetFrameLivePTZ(Guid guid);
        object ExportVideoPTZ(Guid guid, string fromDate, string toDate);
        object DownloadVideo(Guid idExport);

        #endregion -----------------------------Methods PTZ device

        #region Methods LPR device

        #endregion
    }
}