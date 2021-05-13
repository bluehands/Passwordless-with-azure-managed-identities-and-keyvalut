using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Read_from_blob_with_managed_identity
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var provider = new AzureServiceTokenProvider();
            var accessToken = await provider.GetAccessTokenAsync("https://storage.azure.com/").ConfigureAwait(false);
            var tokenCredential = new TokenCredential(accessToken);
            var storageCredentials = new StorageCredentials(tokenCredential);
            
            var storageAccountName = "nosecretsstorage01";
            var containerName = "secrets";
            var fileName = "MyBlobContent.txt";

            var blob = new CloudBlockBlob(new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}/{fileName}"), storageCredentials);
            await using (var s = await blob.OpenReadAsync())
            {
                await using var ms = new MemoryStream();
                await s.CopyToAsync(ms).ConfigureAwait(false);
                var secret = Encoding.UTF8.GetString(ms.ToArray());
                Console.WriteLine($"Content of {fileName} is '{secret}'");
            }
            Console.WriteLine("Press any key to continue");
            Console.Read();
        }
    }
}
