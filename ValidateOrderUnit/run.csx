#r "Newtonsoft.Json"
#load "Product.csx"
using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Web;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");
       
    dynamic data = await req.Content.ReadAsStringAsync();
    RootObject r = JsonConvert.DeserializeObject<RootObject>(data.ToString());
    
    log.Info($"Req: "+ data);

    UInt32 unit=0;

    if(!UInt32.TryParse(r.Product.Unit, out unit))
    {
         log.Info($"Not able to Parse");
        return req.CreateResponse(HttpStatusCode.BadRequest, new
        {
            Status = "Unit should be of Positive Integer Value."
            + "Can't Parse the given value"
        });
    }   
    log.Info($"Parsed Successfully");
    return req.CreateResponse(HttpStatusCode.OK, new
    {
        Status = "Validation Successful"
    });
}
