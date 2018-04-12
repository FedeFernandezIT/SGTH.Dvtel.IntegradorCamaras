using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SGTH.Dvtel.Rest.Filters;

namespace SGTH.Dvtel.Rest.Controllers
{
    [Authorize]
    [ModuloVBasicAuthentication(Realm = "API REST Seguritech - Dvtel")]
    [RoutePrefix("api/dvtel/video")]
    public class VideoController : ApiController
    {

    }
}
