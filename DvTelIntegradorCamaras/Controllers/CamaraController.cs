using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using DVTel.API;
using DVTel.API.Entities.Physical;
using DVTel.API.Entities.SystemObjects;
using DVTel.Common.AssemblyLoader;
using DvTelIntegradorCamaras.Integrador;
using DvTelIntegradorCamaras.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DvTelIntegradorCamaras.Controllers
{
    [System.Web.Http.Authorize]
    public class CamaraController : ApiController
    {
        private readonly IntegradorCamaras _integrador;
        
        public CamaraController()
        {
            _integrador = new IntegradorCamaras();
        }
        public object ResultMethod(ModelResponseMethod result)
        {
            switch (result.status)
            {
                case "200":
                    return Content(HttpStatusCode.OK, new ModelResponseMethod
                    {
                        msg = result.msg,
                        status = CodeStatus.OK_STRING,
                        data = result.data
                    });
                case "401":
                    return Content(HttpStatusCode.Unauthorized, new ModelResponseMethod
                    {
                        msg = result.msg,
                        status = CodeStatus.ERROR,
                        data = null
                    });
                case "404":
                    return Content(HttpStatusCode.NotFound, new ModelResponseMethod
                    {
                        msg = result.msg,
                        status = CodeStatus.ERROR,
                        data = null
                    });
                case "405":
                    return Content(HttpStatusCode.MethodNotAllowed, new ModelResponseMethod
                    {
                        msg = result.msg,
                        status = CodeStatus.ERROR,
                        data = null
                    });;
                default:
                    return Content(HttpStatusCode.InternalServerError, new ModelResponseMethod
                    {
                        msg = result.msg,
                        status = CodeStatus.ERROR,
                        data = null
                    });
            }
        }

        #region -----------------------------Methods PTZ device

        /**
     * @api {get} api/dvtel/getConnectionStreamingPTZ/{guid} Get Connection Streaming PTZ
     * @apiName getConnectionStreamingPTZ
     * @apiGroup IntegradorCamaras
     * @apiDescription Permite obtener los datos necesarios para la conexión streaming de un dispositivo PTZ, determinado por su GUID
     * @apiVersion 0.1.0
     * @apiParam {guid} guid   Guid del dispositivo Ptz .
     * @apiHeader {string} Authorization Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
     *  @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   {
        *     "guid": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *     "logicalId": 1,
        *     "name": "CAMARA DAHUA PTZ",
        *     "isPtzOnline": true,
        *     "isViewable": true,
        *     "isAccessible": true,
        *     "isEnabled": true,
        *     "isRecording": true,
        *     "isPtzLocked": false,
        *     "isPtzEnabled": true,
        *     "deviceUrl": "http://172.17.0.231",
        *     "linkedUrl": "",
        *     "url": "dvnp://admin:123456@172.17.0.238?id=1&Trace=debug",
        *     "clientsConnectionType": "UnicastUDP",
        *     "deviceDriverExternalId": "",
        *     "videoSourceFormat": "NTSC",
        *     "sceneId": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *     "sessionId": "f63659c3-0be1-43cd-ad95-7cfee1c5a09e"
        *   }
        * }
        * 
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        * 
        * HTTP/1.1 404 Not Found
        * {
        *    "status": "Error",
        *    "msg": "No se encontró el dispositivo solicitado.",
        *    "data": null
        * }
        * 
        * HTTP/1.1 405 Method Not Allowed
        * {
        *   "status": "ERROR",
        *   "msg":"El dispositivo no es un dispositivo de video.",
        *   "data":null
        * }
        * 
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }     
     */
        [HttpGet]
        [Route("api/dvtel/getConnectionStreamingPTZ/{guid:guid}")]
        public object GetConnectionStreamingPTZ(Guid guid)
        {
            var result =(ModelResponseMethod) _integrador.GetConnectionStreamingPTZ(guid);
            return ResultMethod(result);
        }

        /** 
        * @api {Get} api/dvtel/getCamerasPTZ Get Cameras PTZ
        * @apiDescription Retorna una lista con información de los dispositivos PTZ encontrados en el sistema.
        * @apiName GetCameras
        * @apiVersion 0.1.0
        * @apiGroup IntegradorCamaras
        * @apiHeader {string} Authorization Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   [
        *       {
        *           "guid": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *           "name": "CAMARA DAHUA PTZ",
        *           "address": null,
        *           "timeZone": "CentralDaylightTime_Mexico",
        *           "clientsConnectionType": "UnicastUDP",
        *           "description": "CAMARA DAHUA PTZ",
        *           "detailedInformation": "CAMARA DAHUA PTZ",
        *           "deviceDriverExternalId": "",
        *           "isAccessible": true,
        *           "isEnabled": true,
        *           "isRecording": true,
        *           "linkedUrl": "",
        *           "deviceUrl": "http://172.17.0.231",
        *           "logicalId": 1,
        *           "videoSourceFormat": "NTSC",
        *           "canFocus": true,
        *           "canIris": true,
        *           "canPanTilt": true,
        *           "canZoom": true,
        *           "isLocked": false,
        *           "numberOfPatterns": 0,
        *           "numberOfPresets": 0,
        *           "sceneId": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *           "sessionId": "71351fa4-5c12-4866-b2ad-f917eeb72d2b",
        *           "panValue": "180",
        *           "tiltValue": "0",
        *           "zoomValue": "6.1897001964269E+26",
        *           "isPtzOnline": true,
        *           "IsPtzLocked": false,
        *           "IsPtzEnabled": true,
        *           "creationTime": "0001-01-01T00:00:00",
        *           "isViewable": true,
        *           "supportedEvents": null,
        *           "geographicLocation": null
        *       }
        * ]
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        * 
        * HTTP/1.1 404 Not Found
        * {
        *   "status": "ERROR",
        *   "msg":"No se encontraron dispositivos Ptz.",
        *   "data":null
        * }
        *  
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }        
        */
        [HttpGet]
        [Route("api/dvtel/getCamerasPTZ")]
        public object GetCamerasPTZ()
        {
            var result =(ModelResponseMethod) _integrador.GetCamerasPTZ();
            return ResultMethod(result);
            MiDelegadoAsincrono delegado = LogoutSystemDvTel;
            IAsyncResult result2 = delegado.BeginInvoke(null, null);
        }

        public delegate void MiDelegadoAsincrono();
        private static void LogoutSystemDvTel()
        {
            //await Task.Run(;)
            DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Shutdown();
            //await DvtelSystemsManagerProvider.Instance.DvtelSystemsManager.Shutdown();
        }

        /** 
        * @api {Get} api/dvtel/getPTZUnitById/{id} Get PTZ By Id
        * @apiDescription Retorna la información de un dispositivo PTZ según su ID lógico.
        * @apiName GetCamaraPTZById
        * @apiVersion 0.1.0
        * @apiGroup IntegradorCamaras
        *
        * @apiParam {int} id    Id lógico del dispositivo Ptz. 
        * @apiHeader {string} Authorization Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   {             
        *       "guid": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *       "name": "CAMARA DAHUA PTZ",
        *       "address": null,
        *       "timeZone": "CentralDaylightTime_Mexico",
        *       "clientsConnectionType": "UnicastUDP",
        *       "description": "CAMARA DAHUA PTZ",
        *       "detailedInformation": "CAMARA DAHUA PTZ",
        *       "deviceDriverExternalId": "",
        *       "isAccessible": true,
        *       "isEnabled": true,
        *       "isRecording": true,
        *       "linkedUrl": "",
        *       "deviceUrl": "http://172.17.0.231",
        *       "logicalId": 1,
        *       "videoSourceFormat": "NTSC",
        *       "canFocus": true,
        *       "canIris": true,
        *       "canPanTilt": true,
        *       "canZoom": true,
        *       "isLocked": false,
        *       "numberOfPatterns": 0,
        *       "numberOfPresets": 0,
        *       "sceneId": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *       "sessionId": "71351fa4-5c12-4866-b2ad-f917eeb72d2b",
        *       "panValue": "180",
        *       "tiltValue": "0",
        *       "zoomValue": "6.1897001964269E+26",
        *       "isPtzOnline": true,
        *       "IsPtzLocked": false,
        *       "IsPtzEnabled": true,
        *       "creationTime": "0001-01-01T00:00:00",
        *       "isViewable": true,
        *       "supportedEvents": null,
        *       "geographicLocation": null
        *    }
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        *  
        * HTTP/1.1 404 Not Found
        * {
        *    "status": "Error",
        *    "msg": "No se encontró el dispositivo solicitado.",
        *    "data": null
        * }
        * 
        * HTTP/1.1 405 Method Not Allowed
        * {
        *   "status": "ERROR",
        *   "msg":"El dispositivo no es un dispositivo de video.",
        *   "data":null
        * }
        * 
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }        
        */
        [HttpGet]
        [Route("api/dvtel/getPTZUnitById/{id}")]
        public object GetPTZUnitById(int id)
        {           
            var result = (ModelResponseMethod)_integrador.GetPTZUnitById(id);
            return ResultMethod(result);
        }
        /** 
        * @api {Get} api/dvtel/getPTZUnitByGuid/{guid} Get PTZ By Guid
        * @apiDescription Retorna la información de un dispositivo PTZ según su GUID.
        * @apiName GetCamaraPTZByGuid
        * @apiVersion 0.1.0
        * @apiGroup IntegradorCamaras
        *
        * @apiParam {guid} guid     Guid del dispositivo Ptz.  
        * @apiHeader {string} Authorization Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.     
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   {
        *       "guid": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *       "name": "CAMARA DAHUA PTZ",
        *       "address": null,
        *       "timeZone": "CentralDaylightTime_Mexico",
        *       "clientsConnectionType": "UnicastUDP",
        *       "description": "CAMARA DAHUA PTZ",
        *       "detailedInformation": "CAMARA DAHUA PTZ",
        *       "deviceDriverExternalId": "",
        *       "isAccessible": true,
        *       "isEnabled": true,
        *       "isRecording": true,
        *       "linkedUrl": "",
        *       "deviceUrl": "http://172.17.0.231",
        *       "logicalId": 1,
        *       "videoSourceFormat": "NTSC",
        *       "canFocus": true,
        *       "canIris": true,
        *       "canPanTilt": true,
        *       "canZoom": true,
        *       "isLocked": false,
        *       "numberOfPatterns": 0,
        *       "numberOfPresets": 0,
        *       "sceneId": "7617ff98-47f5-40e8-a7b2-0ca3ac159132",
        *       "sessionId": "71351fa4-5c12-4866-b2ad-f917eeb72d2b",
        *       "panValue": "180",
        *       "tiltValue": "0",
        *       "zoomValue": "6.1897001964269E+26",
        *       "isPtzOnline": true,
        *       "IsPtzLocked": false,
        *       "IsPtzEnabled": true,
        *       "creationTime": "0001-01-01T00:00:00",
        *       "isViewable": true,
        *       "supportedEvents": null,
        *       "geographicLocation": null
        *   }      
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        * 
        * HTTP/1.1 404 Not Found
        * {
        *    "status": "Error",
        *    "msg": "No se encontró el dispositivo solicitado.",
        *    "data": null
        * }
        *
        * HTTP/1.1 405 Method Not Allowed
        * {
        *   "status": "ERROR",
        *   "msg":"El dispositivo no es un dispositivo de video.",
        *   "data":null
        * }
        * 
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }        
        */
        [HttpGet]
        [Route("api/dvtel/getPTZUnitByGuid/{guid:guid}")]
        public object GetPTZUnitByGuid(Guid guid)
        {
            var result = (ModelResponseMethod)_integrador.GetPTZUnitByGuid(guid);
            return ResultMethod(result);            
        }

        /** 
        * @api {Get} api/dvtel/existsPTZ/{guid} Exists PTZ
        * @apiDescription Retorna un objeto que determina la existencia o no de un dispositivo PTZ según su GUID.
        * @apiName ExistsCamara
        * @apiVersion 0.1.0
        * @apiGroup IntegradorCamaras
        *
        * @apiParam {guid} guid     Guid del dispositivo Ptz.   
        * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":null
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        * 
        * HTTP/1.1 404 Not Found
        * {
        *    "status": "Error",
        *    "msg": "No se encontró el dispositivo solicitado.",
        *    "data": null
        * } 
        *  
        * HTTP/1.1 405 Method Not Allowed
        * {
        *   "status": "ERROR",
        *   "msg":"El dispositivo no es un dispositivo de video.",
        *   "data":null
        * }
        * 
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }        
        */
        [HttpGet]
        [Route("api/dvtel/existsPTZ/{guid:guid}")]
        public object ExistsPTZ(Guid guid)
        {
            var result = (ModelResponseMethod)_integrador.ExistsPTZ(guid);
            return ResultMethod(result);
        }

        /**
        * @api {GET} api/dvtel/GetTiltPTZ/{guid}/{tilt}/{speed} Get Tilt PTZ
        * @apiGroup IntegradorCamaras
        * @apiName GetTiltPTZ          
        * @apiDescription Ejecuta el comando de movimiento vertical (<code>tilt</code>) de un dispositivo PTZ.                 
        * @apiVersion 0.1.0
        * 
        * @apiParam {Guid} guid Guid del dispositvo PTZ.
        * @apiParam {int{0..2}} tilt Tipo de <code>tilt</code> a ejecutar.
        *          <b>Up</b> = 0, <b>Down</b> = 1, <b>Stop</b> = 2.
        * @apiParam {int{1..100}} speed Velocidad del movimiento en porcentaje.       
        * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.             
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":null
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
        * {
        *     "Message": "Authorization has been denied for this request."
        * }
        * 
        * HTTP/1.1 404 Not Found
        * {
        *    "status": "Error",
        *    "msg": "No se encontró el dispositivo solicitado.",
        *    "data": null
        * }
        * 
        * HTTP/1.1 405 Method Not Allowed
        * {
        *   "status": "ERROR",
        *   "msg":"El dispositivo no es un dispositivo de video.",
        *   "data":null
        * }
        * 
        * HTTP/1.1 500 Internal Server Error
        * {
        *   "status": "ERROR",
        *   "msg":"Error no controlado por la Api.",
        *   "data":null
        * }        
        */
        [HttpGet]
        [Route("api/dvtel/getTiltPTZ/{guid:guid}/{tilt:int}/{speed:int}")]
        public object GetTiltPTZ(Guid guid, int tilt, int speed)
        {
            var result = (ModelResponseMethod)_integrador.GetTiltPTZ(guid, tilt, speed);
            return ResultMethod(result);            
        }

        /**
         * @api {GET} api/dvtel/getPanPTZ/{guid}/{pan}/{speed} Get Pan PTZ
         * @apiGroup IntegradorCamaras
         * @apiName GetPanPTZ          
         * @apiDescription Ejecuta el comando de movimiento horizontal (<code>pan</code>) de una dispositivo PTZ.                 
         * @apiVersion 0.1.0
         * 
         * @apiParam {Guid} guid Guid del dispositvo PTZ.
         * @apiParam {int{0..2}} pan Tipo de <code>pan</code> a ejecutar.
         *          <b>Left</b> = 0, <b>Right</b> = 1, <b>Stop</b> = 2.
         * @apiParam {int{1..100}} speed Velocidad del movimiento en porcentaje. 
         * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK
         * {
         *   "status": "OK",
         *   "msg":"",
         *   "data":null
         * }
         * @apiErrorExample {json} Error-Response
         * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         * 
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }  
         */
        [HttpGet]
        [Route("api/dvtel/getPanPTZ/{guid:guid}/{pan:int}/{speed:int}")]
        public object GetPanPTZ(Guid guid, int pan, int speed)
        {
            var result = (ModelResponseMethod)_integrador.GetPanPTZ(guid, pan, speed);
            return ResultMethod(result);
        }

        /**
         * @api {GET} api/dvtel/getZoomPTZ/{guid}/{zoom}/{speed} Get Zoom PTZ
         * @apiGroup IntegradorCamaras
         * @apiName GetZoomPTZ          
         * @apiDescription Ejecuta el comando de movimiento <code>zoom</code> de un dispositivo PTZ.                 
         * @apiVersion 0.1.0
         * 
         * @apiParam {Guid} guid Guid del dispositvo PTZ.
         * @apiParam {int{0..2}} zoom Tipo de <code>zoom</code> a ejecutar.
         *          <b>Wide</b> = 0, <b>Tele</b> = 1, <b>Stop</b> = 2.
         * @apiParam {int{1..100}} speed Velocidad del movimiento en porcentaje.   
         * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK
         * {
         *   "status": "OK",
         *   "msg":"",
         *   "data":null
         * }
         * @apiErrorExample {json} Error-Response
         * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }                             
         */
        [HttpGet]
        [Route("api/dvtel/getZoomPTZ/{guid:guid}/{zoom:int}/{speed:int}")]
        public object GetZoomPTZ(Guid guid, int zoom, int speed)
        {
            var result = (ModelResponseMethod)_integrador.GetZoomPTZ(guid, zoom, speed);
            return ResultMethod(result);
        }

        /**
         * @api {GET} api/dvtel/zoomAndMove/{guid:guid}/{panSpeed:int}/{tiltSpeed:int}/{zoomSpeed:int} Zoom and move PTZ
         * @apiGroup IntegradorCamaras
         * @apiName zoomAndMove          
         * @apiDescription Mueve el dispositivo PTZ a la ubicación especificada con zoom
         * @apiVersion 0.1.0
         * @apiParam {Guid} guid Guid del dispositvo PTZ.         
         * @apiParam {int{0..360}} panSpeed Valor de Pan .  
         * @apiParam {int{-100..100}} tiltSpeed Valor de Tilt.  
         * @apiParam {int} zoomSpeed Valor de Zoom .  
         * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK
         * {
         *   "status": "OK",
         *   "msg":"",
         *   "data":null
         * }
         * @apiErrorExample {json} Error-Response
         * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }                                                
         */
        [HttpGet]
        [Route("api/dvtel/zoomAndMove/{guid:guid}/{panSpeed:int}/{tiltSpeed:int}/{zoomSpeed:int}")]
        public object GetzoomAndMove(Guid guid, int panSpeed, int tiltSpeed, int zoomSpeed)
        {
            var result = (ModelResponseMethod)_integrador.GetzoomAndMove(guid, panSpeed, tiltSpeed, zoomSpeed);
            return ResultMethod(result);
        }

        /**
         * @api {GET} api/dvtel/getStopPTZ/{guid:guid} Stop move PTZ
         * @apiGroup IntegradorCamaras
         * @apiName StopPTZ          
         * @apiDescription Detiene el movimiento (horizontal o vertical) del dispositivo PTZ.
         * @apiVersion 0.1.0
         * @apiParam {Guid} guid Guid del dispositvo PTZ.         
         * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK
         * {
         *   "status": "OK",
         *   "msg":"",
         *   "data":null
         * }
         * @apiErrorExample {json} Error-Response
         * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }                                                
         */
        [HttpGet]
        [Route("api/dvtel/getStopPTZ/{guid:guid}")]
        public object GetStopPTZ(Guid guid)
        {
            var result = (ModelResponseMethod)_integrador.GetStopPTZ(guid);
            return ResultMethod(result);
        }


        /**
         * @api {GET} api/dvtel/goToPreset/{guid:guid}/{idPreset:int} Got to Preset PTZ
         * @apiGroup IntegradorCamaras
         * @apiName goToPreset          
         * @apiDescription Mueve el dispositivo PTZ al valor predeterminado especificado
         * @apiVersion 0.1.0
         * @apiParam {Guid} guid Guid del dispositvo PTZ.         
         * @apiParam {int} idPreset Valor predeterminado especificado.  
         * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
         * @apiSuccessExample {json} Success-Response:
         * HTTP/1.1 200 OK
         * {
         *   "status": "OK",
         *   "msg":"",
         *   "data":null
         * }
         * @apiErrorExample {json} Error-Response
         * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }                                          
         */
        [HttpGet]
        [Route("api/dvtel/goToPreset/{guid:guid}/{idPreset:int}")]
        public object GoToPreset(Guid guid, int idPreset)
        {
            var result = (ModelResponseMethod)_integrador.GoToPreset(guid, idPreset);
            return ResultMethod(result);
        }
        
        /** 
        * @api {Get} api/dvtel/getFrameLivePTZ/{guid} Get Frame Live PTZ
        * @apiDescription Permite obtener un frame (snapshot) de un dispositivo PTZ online, determinado por su GUID.
        * @apiName GetFrameLivePTZ
        * @apiVersion 0.1.0
        * @apiGroup IntegradorCamaras
        *
        * @apiParam {guid} guid   Guid del dispositivo Ptz.    
        * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   {
        *       "typeImage":"jpg",
        *       "base64ImageRepresentation": "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"
        *   }
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }        
        */
        [HttpGet]
        [Route("api/dvtel/getFrameLivePTZ/{guid:guid}")]
        public object GetFrameLivePTZ(Guid guid)
        {
            var result = (ModelResponseMethod)_integrador.GetFrameLivePTZ(guid);
            return ResultMethod(result);
        }

        /**
        * @api {GET} api/dvtel/getFramePTZ/{guid}/{dateFrame} Get Frame PTZ
        * @apiGroup IntegradorCamaras
        * @apiName getFramePTZ          
        * @apiDescription Permite obtener un frame (snapshot) de un dispositivo PTZ determinado por su GUID y de un periodo determinado (fecha y hora).             
        * @apiVersion 0.1.0
        * 
        * @apiParam {guid} guid      Guid del dispositivo Ptz.  
        * @apiParam {string[18]} dateFrame Fecha y hora del frame de donde se realizará el snapshot. Formato YYYY-MM-DD HH:MM:SS PM/AM   
        * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
        * @apiSuccessExample {json} Success-Response:
        * HTTP/1.1 200 OK     
        * {
        *   "status": "OK",
        *   "msg":"",
        *   "data":
        *   {
        *       "typeImage":"jpg",
        *       "base64ImageRepresentation": "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"
        *   }
        * }
        * @apiErrorExample {json} Error-Response
        * HTTP/1.1 401 Unauthorized
         * {
         *     "Message": "Authorization has been denied for this request."
         * }
         * 
         * HTTP/1.1 404 Not Found
         * {
         *    "status": "Error",
         *    "msg": "No se encontró el dispositivo solicitado.",
         *    "data": null
         * }
         *  
         * HTTP/1.1 405 Method Not Allowed
         * {
         *   "status": "ERROR",
         *   "msg":"El dispositivo no es un dispositivo de video.",
         *   "data":null
         * }
         * 
         * HTTP/1.1 500 Internal Server Error
         * {
         *   "status": "ERROR",
         *   "msg":"Error no controlado por la Api.",
         *   "data":null
         * }                           
         */

        [HttpGet]
        [Route("api/dvtel/getFramePTZ/{guid:guid}/{dateFrame}")]
        public object GetFramePTZ(Guid guid, string dateFrame)
        {
            var result = (ModelResponseMethod)_integrador.GetFramePTZ(guid, dateFrame);
            return ResultMethod(result);
        }
        /** 
      * @api {Get} api/dvtel/exportVideoPTZ/{guid}/{fromDate}/{toDate} Export Video PTZ
      * @apiDescription Permite exportar el video (en formato MP4) de un determinado periodo, de un dispositivo PTZ.
      * @apiName ExportVideo
      * @apiVersion 0.1.0
      * @apiGroup IntegradorCamaras
      * @apiParam {guid} guid   Guid del dispositivo Ptz.   
      * @apiParam {string[18]} fromDate  Fecha inicio del video. Formato YYYY-MM-DD HH:MM:SS PM/AM
      * @apiParam {string[18]}  toDate   Fecha final del video.  Formato YYYY-MM-DD HH:MM:SS PM/AM
      * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
      * @apiSuccessExample {json} Success-Response:
      * HTTP/1.1 200 OK
      * {
      *   "status": "OK",
      *   "msg":"",
      *   "data":
      *   {
      *     "idExportSession": "87a22bdf-d14f-473f-9918-a2289f1b6719",
      *     "path": 
      *     [
      *         "ftp:\\172.17.0.238:21\\CAMARA DAHUA PTZ-20170722-080000.mp4",
      *         "ftp:\\172.17.0.238:21\\CAMARA DAHUA PTZ-20170722-080000(1).mp4"
      *     ]
      *   }
      * }
      * @apiErrorExample {json} Error-Response
      * HTTP/1.1 401 Unauthorized
      * {
      *     "Message": "Authorization has been denied for this request."
      * }
      * 
      * HTTP/1.1 404 Not Found
      * {
      *    "status": "Error",
      *    "msg": "No se encontró el dispositivo solicitado.",
      *    "data": null
      * }
      *  
      * HTTP/1.1 405 Method Not Allowed
      * {
      *   "status": "ERROR",
      *   "msg":"El dispositivo no es un dispositivo de video.",
      *   "data":null
      * }
      * 
      * HTTP/1.1 500 Internal Server Error
      * {
      *   "status": "ERROR",
      *   "msg":"Error no controlado por la Api.",
      *   "data":null
      * }   
      */
        [HttpGet]
        [Route("api/dvtel/exportVideoPTZ/{guid:guid}/{fromDate}/{toDate}")]
        public object ExportVideoPTZ(Guid guid, string fromDate, string toDate)
        {
            var result = (ModelResponseMethod)_integrador.ExportVideoPTZ(guid, fromDate, toDate);
            return ResultMethod(result);
        }

        /** 
     * @api {Get} api/dvtel/downloadVideo/{idExport} Download Video PTZ
     * @apiDescription Permite consultar el estado de exportacion de un video( o descargarlo).
     * @apiName DownloadVideo
     * @apiVersion 0.1.0
     * @apiGroup IntegradorCamaras
     * @apiParam {guid} idExport   Guid de la sesión de exportación
     * @apiHeader {string} Authorization  Credenciales válidas del usuario del sistema Latitude de DvTel/FLIR.
     * @apiSuccessExample {json} Success-Response:
     * HTTP/1.1 200 OK
     * {
     *   "status": "OK",
     *   "msg":"",
     *   "data"null
     * }
     * @apiErrorExample {json} Error-Response
     * HTTP/1.1 401 Unauthorized
     * {
     *     "Message": "Authorization has been denied for this request."
     * }
     * 
     * HTTP/1.1 404 Not Found
     * {
     *    "status": "Error",
     *    "msg": "El proceso de exportación aún no finalizó.",
     *    "data": null
     * }
     *  
     * HTTP/1.1 500 Internal Server Error
     * {
     *   "status": "ERROR",
     *   "msg":"No se encontró el archivo exportado.",
     *   "data":null
     * }   
     */
        [HttpGet]
        [Route("api/dvtel/downloadVideo/{idExport:guid}")]
        public object DownloadVideo(Guid idExport)
        {
            var result = (ModelResponseMethod)_integrador.DownloadVideo(idExport);
            return ResultMethod(result);
        }

        #endregion
    }

}