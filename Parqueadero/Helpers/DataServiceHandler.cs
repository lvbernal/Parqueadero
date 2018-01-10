using System;
using System.Net.Http;

namespace Parqueadero.Helpers
{
    public class DataServiceHandler : DelegatingHandler
    {
        protected override async System.Threading.Tasks.Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            Console.WriteLine("------");
            Console.WriteLine("DATA SERVICE REQUEST:");
            Console.WriteLine("Uri: " + request.RequestUri);
            Console.WriteLine("Method: " + request.Method);

            if (request.Content != null)
            {
                var content = await request.Content.ReadAsStringAsync();
                Console.WriteLine("Content: " + content ?? "None");
            }

            Console.WriteLine("------");

            // Sends the actual request to the Mobile Service.
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
