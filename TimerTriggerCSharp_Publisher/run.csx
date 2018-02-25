#r "Newtonsoft.Json"
#load "common.csx"
using System;

public static void Run(TimerInfo myTimer, TraceWriter log, out string outputEventHubMessage)
{
    log.Info($"Timer trigger function executed at: {DateTime.Now}");
    var logmsg = new LogDetails
     {
      LogMsg = "Publisher Msg",
      LogMsgTime = DateTime.UtcNow,
      Level = LogLevel.Info
     };
     
     outputEventHubMessage= Newtonsoft.Json.JsonConvert.SerializeObject(logmsg);
}
