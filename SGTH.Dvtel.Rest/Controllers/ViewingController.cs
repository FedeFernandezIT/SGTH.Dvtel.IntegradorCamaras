using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SGTH.Dvtel.Mobile.Client.MobileMiddlewareObjects;
using SGTH.Dvtel.Rest.Filters;
using SGTH.Dvtel.Rest.Models;
using SGTH.Dvtel.Rest.Services;

namespace SGTH.Dvtel.Rest.Controllers
{
    [Authorize]
    [ModuloVBasicAuthentication(Realm = "API REST Seguritech - Dvtel")]
    [RoutePrefix("api/dvtel/viewing")]
    public class ViewingController : ApiController
    {
        private IDvtelMobileService _mobile;

        public ViewingController()
            : this(new DvtelMobileService())
        { }

        public ViewingController(IDvtelMobileService mobile)
        {
            _mobile = mobile;
        }


        /**
        * @api {GET} api/dvtel/viewing/startlive/:camera StartLive        
        * @apiSampleRequest http://148.244.123.138:17606/api/dvtel/viewing/startlive
        * @apiDescription Obtiene la URL HTTP de <b>streaming</b> de video de una cámara particular.
        * @apiVersion 2.0.0
        * @apiName StartLive
        * @apiGroup Viewing
        * 
        * @apiHeader {string} Authorization Protocolo y datos de authorización.        
        * @apiParam {guid} camera Identificador único de la cámara.</br>Ejemplo: <code>550e8400-e29b-41d4-a716-446655440000</code>                
        *                
        * @apiExample {url} Request-Example:
        *   GET //api/dvtel/viewing/startlive/550e8400-e29b-41d4-a716-446655440000 HTTP/1.1
        *   Host: localhost:9100
        *   Authorization: Basic feT33Ii9lLpVmnBSFr042mku
        *
        * @apiSuccess {string} status Estado del resultado de la solicitud.
        * @apiSuccess {string} [msg] Mensaje descriptivo del resultado de la solicitud.
        * @apiSuccess {string} data URL HTTP de streaming en vivo.
        *
        * @apiSuccessExample Success-Response:
        *     HTTP/1.1 200 OK
        * {
        *   "status": "Ok",
        *   "msg": "La operación se realizó con éxito.",
        *   "data": {
        *       "CodRta": "00",
        *       "NroTarjeta": "450799xxxxxx7787",
        *       "Token": "Z3geSSzRL05KFnc/+/gZJVBIpf59zeTQzK+GQOs0PYg=",
        *       "transactionId": "106"
        *   }
        * }
        *
        * @apiError (Error 4xx | 5xx) {string} status Estado del resultado de la solicitud.
        * @apiError (Error 4xx | 5xx) {string} [msg] Mensaje descriptivo del error de la solicitud.
        * @apiError (Error 4xx | 5xx) {dynamic} [data] Información adicional del error.
        * 
        * @apiErrorExample {json} Error-Response: Service Unavailable
        * HTTP/1.1 503 Service Unavailable
        * {
        *   "status": "Error",
        *   "msg": "Mensaje descriptivo.",
        *   "data": ""
        * }
        * 
        * @apiErrorExample {json} Error-Response: Bad Request
        * HTTP/1.1 400 Bad Request
        * {
        *   "status": "Bad Request",
        *   "msg": "La solicitud es inválida.",
        *   "data": [
        *       "El campo XXXX es obligatorio.",
        *       "El campo XXXX es obligatorio."
        *   ]
        * } 
        *         
        */
        [HttpGet]
        [Route("startlive/{camera:guid}")]
        public async Task<IHttpActionResult> StartLive(Guid camera)
        {
            Uri url = await _mobile.StartLive(camera, "mjpeg");
            return Ok(new ModelResponseMethod
            {
                Status = CodeStatus.OK,
                Msg = "La operación se realizó con éxito.",
                Data = url.ToString()
            });
        }


        /**
        * @api {GET} api/dvtel/viewing/cameras Cameras        
        * @apiSampleRequest http://148.244.123.138:17606/api/dvtel/viewing/cameras
        * @apiDescription Obtiene listado de cámaras disponibles.
        * @apiVersion 2.0.0
        * @apiName Cameras
        * @apiGroup Viewing
        *                         
        * @apiHeader {string} Authorization Protocolo y datos de authorización.        
        * @apiExample {url} Request-Example:
        *   GET //api/dvtel/viewing/cameras HTTP/1.1
        *   Host: localhost:9100
        *   Authorization: Basic feT33Ii9lLpVmnBSFr042mku
        *
        * @apiSuccess {string} status Estado del resultado de la solicitud.
        * @apiSuccess {string} [msg] Mensaje descriptivo del resultado de la solicitud.
        * @apiSuccess {array} data Listado detallado de cámaras disponibles.
        * @apiSuccess {bool} canPlayback Si el usuario tiene privilegios para Reproducir un clip archivado.
        * @apiSuccess {bool} canRecord Si el usuario tiene privilegios para detener / iniciar la grabación manual en la cámara.
        * @apiSuccess {guid} id La propiedad Id. de la cámara.
        * @apiSuccess {bool} isAccessible El estado de accesibilidad de la cámara.
        * @apiSuccess {bool} isGosth Si la cámara es una escena "offline" (es decir, separada de la gestión de un Archiver).
        * @apiSuccess {bool} isPTZEnabled Si la capacidad PTZ de la cámara está habilitada.
        * @apiSuccess {bool} isRecording Si la cámara está grabando.        
        * @apiSuccess {string} name Nombre de la cámara.
        * @apiSuccess {guid} siteId El sitio principal de la cámara.        
        *
        * @apiSuccessExample Success-Response:
        *     HTTP/1.1 200 OK
        * {
        *   "status": "Ok",
        *   "msg": "La operación se realizó con éxito.",
        *   "data": [
        *       {
        *           "CanPlayback": true,
        *           "CanRecord": true,
        *           "Description": null,
        *           "Id": "c4f86f5d-6a0c-4845-a77a-a1d4893e3201",
        *           "IsAccessible": true,
        *           "IsGhost": false,
        *           "IsPTZEnabled": false,
        *           "IsRecording": true,
        *           "Name": "4K Cubo",
        *           "SiteId": "00000000-0000-0000-0000-000000000000"
        *       },
        *       {
        *           "CanPlayback": true,
        *           "CanRecord": true,
        *           "Description": "CAMARA DAHUA PTZ",
        *           "Id": "fbb198d7-73d2-458c-8e2d-c9f400e779db",
        *           "IsAccessible": true,
        *           "IsGhost": false,
        *           "IsPTZEnabled": false,
        *           "IsRecording": true,
        *           "Name": "CAMARA DAHUA",
        *           "SiteId": "00000000-0000-0000-0000-000000000000"
        *       },
        *       {
        *           "CanPlayback": true,
        *           "CanRecord": true,
        *           "Description": "CAMARA DAHUA PTZ",
        *           "Id": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *           "IsAccessible": true,
        *           "IsGhost": false,
        *           "IsPTZEnabled": true,
        *           "IsRecording": true,
        *           "Name": "CAMARA DAHUA PTZ",
        *           "SiteId": "00000000-0000-0000-0000-000000000000"
        *       }
        *   ]
        * }
        *
        * @apiError (Error 4xx | 5xx) {string} status Estado del resultado de la solicitud.
        * @apiError (Error 4xx | 5xx) {string} [msg] Mensaje descriptivo del error de la solicitud.
        * @apiError (Error 4xx | 5xx) {dynamic} [data] Información adicional del error.
        * 
        * @apiErrorExample {json} Error-Response: 502 Bad Gateway
        * HTTP/1.1 502 Bad Gateway
        * {
        *   "status": "Bad Gateway",
        *   "msg": "Dvtel Vms Authenticate Error: Unknown. Error al enviar la solicitud. No es posible conectar con el servidor remoto. No se puede establecer una conexión ya que el equipo de destino denegó expresamente dicha conexión 127.0.0.1:8081.",
        *   "data": ""
        * }   
        * 
        * @apiErrorExample {json} Error-Response: Bad Request
        * HTTP/1.1 400 Bad Request
        * {
        *   "status": "Bad Request",
        *   "msg": "La solicitud es inválida.",
        *   "data": [
        *       "El campo XXXX es obligatorio.",
        *       "El campo XXXX es obligatorio."
        *   ]
        * } 
        *         
        */
        [HttpGet]
        [Route("cameras")]
        public async Task<IHttpActionResult> Cameras()
        {
            List<Camera> cameras = await _mobile.GetCameras();

            string message = cameras.Any()
                ? "La operación se realizó con éxito."
                : "No hay cámaras disponibles.";

            return Ok(new ModelResponseMethod
            {
                Status = CodeStatus.OK,
                Msg = message,
                Data = cameras
            });
        }
    }
}
