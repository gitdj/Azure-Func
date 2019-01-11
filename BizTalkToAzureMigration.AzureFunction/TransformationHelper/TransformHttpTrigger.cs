using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using AzureFunction.Helper;
using System.Text;

namespace TransformationHelper
{
    public static class TransformHttpTrigger
    {
        [FunctionName("TransformAPI")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string mapName = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "MapName", true) == 0)
                .Value;
            

            byte[] request = await req.Content.ReadAsByteArrayAsync();

            TransformHelper helperobj = new TransformHelper();
            //name = helperobj.Transform(name, request);

            string response = await helperobj.ExecuteTransform(request, mapName);

            return mapName == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string & in the request body")
                : new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(response, Encoding.Default, @"application/xml"),
                };

        }
    }
}
