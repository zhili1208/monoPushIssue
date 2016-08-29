using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MacPushTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string apikey = args[1];
            string pathToPackage = args[0];
            string source = args[2];

            var fileStream = new FileStream(pathToPackage, FileMode.Open, FileAccess.Read, FileShare.Read);

            var request = new HttpRequestMessage(HttpMethod.Put, source);
            var content = new MultipartFormDataContent();

            var packageContent = new StreamContent(fileStream);
            packageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
            //"package" and "package.nupkg" are random names for content deserializing
            //not tied to actual package name.
            content.Add(packageContent, "package", "package.nupkg");
            request.Content = content;

            // Send the data in chunks so that it can be canceled if auth fails.
            // Otherwise the whole package needs to be sent to the server before the PUT fails.
            request.Headers.TransferEncodingChunked = true;

            request.Headers.Add("X-NuGet-ApiKey", apikey);
            var httpClient = new HttpClient();

            var response = httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("push Success!");
            }
            else
            {
                Console.WriteLine("push failed");
            }
        }
    }
}
