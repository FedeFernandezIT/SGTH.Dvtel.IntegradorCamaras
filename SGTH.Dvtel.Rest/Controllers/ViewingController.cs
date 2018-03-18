using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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

        [HttpGet]
        [Route("startlive/{camera:guid}/{session:guid}")]
        public IHttpActionResult StartLive(Guid camera, Guid session)
        {
            Uri url = _mobile.StartLive(camera, session, "mjpeg");
            return Ok(new ModelResponseMethod
            {
                Status = CodeStatus.OK,
                Msg = "La operación se realizó con éxito",
                Data = url.ToString()
            });
        }
    }
}
