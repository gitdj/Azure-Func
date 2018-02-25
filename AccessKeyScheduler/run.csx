using System;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request at: {DateTime.Now}.");
    System.Diagnostics.Process process = new System.Diagnostics.Process();
    process.StartInfo.FileName = @"D:\home\site\wwwroot\AccessKeyScheduler\AzureFuncScheduler.exe";
    process.StartInfo.Arguments = "";
    process.StartInfo.UseShellExecute = false;
    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;
    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    process.WaitForExit();
    log.Info($"C# Timer trigger function execution Completed at: {DateTime.Now}");
}
