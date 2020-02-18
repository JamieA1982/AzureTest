using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFuncTestProject
{
  public static class Function1
  {
    [FunctionName("Function1")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
        [Queue("TestMsgQueue")] IAsyncCollector<msg> msgQueue,
        ILogger log)
    {
      log.LogInformation("C# HTTP trigger function processed a request.");

      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var msg = JsonConvert.DeserializeObject<msg>(requestBody);
      await msgQueue.AddAsync(msg);

      return msg != null
          ? (ActionResult)new OkObjectResult($"Message Sent To, {msg.to}")
          : new BadRequestObjectResult("Crap data");
    }
  }

  public class msg
  {
    public string from;
    public string to;
    public string message;
  }
}
