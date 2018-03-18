using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SGTH.Dvtel.Rest.Filters
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