using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using System.Configuration;
namespace DvTelIntegradorCamaras.Filters
{
    public class LogRequestResponseFilter : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
           var response = await Task.Run(() => base.SendAsync(request, cancellationToken));
           return response;
        }
    }
}