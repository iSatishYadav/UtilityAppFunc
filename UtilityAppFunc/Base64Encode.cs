using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace UtilityAppFunc
{
    public static class Base64Encode
    {
        [FunctionName("Base64Encode")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            Guid correlationId = Guid.NewGuid();
            log.LogInformation("{0} - C# HTTP trigger function processed a request.", correlationId);

            string plainText = req.Query["text"];
            log.LogInformation("{0} - Query string text: {1}", correlationId, plainText);

            if (string.IsNullOrEmpty(plainText))
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                log.LogInformation("{0} - Post body: {1}", correlationId, requestBody);
                dynamic data = JsonConvert.DeserializeObject(requestBody);
                plainText = plainText ?? data?.text;
            }

            return plainText != null
                ? (ActionResult)new OkObjectResult(plainText.ToBase64())
                : new BadRequestObjectResult("Please pass a text on the query string or in the request body");
        }

        public static string ToBase64(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }
    }
}
