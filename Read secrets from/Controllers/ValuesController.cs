using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;

namespace NoSecrets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new[] { "file", "keyvault", "servicebus" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            //Environment.SetEnvironmentVariable("AzureServicesAuthConnectionString", "RunAs=Developer; DeveloperTool=AzureCli");
            switch (id.ToLower())
            {
                case "file":
                    return await ReadSecretFromFile();
                case "keyvault":
                    return await ReadSecretFromKeyVault();
                case "servicebus":
                    return await SendSecretToServiceBus();
                default:
                    return "Unknown switch";
            }

        }

        private async Task<string> SendSecretToServiceBus()
        {
            var tokenProvider = TokenProvider.CreateManagedServiceIdentityTokenProvider();
            string sbName = "nosecrets";
            string queueName = "myqueue";
            QueueClient client = new QueueClient($"sb://{sbName}.servicebus.windows.net/", queueName, tokenProvider);
            await client.SendAsync(new Message(Encoding.UTF8.GetBytes("Don't do this at home")));
            await client.CloseAsync();
            return "Message send to Service Bus. Check with ServiceBusExplorer";
        }

        private async Task<string> ReadSecretFromKeyVault()
        {
            // ReSharper disable once UnusedVariable
            var connectionStringInternalUsedByAzureServiceTokenProvider = Environment.GetEnvironmentVariable("AzureServicesAuthConnectionString");

            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            string vaultName = "NoSecrets-MyVault01";
            string secret = "MySecret";
            var secretBundle = await keyVaultClient.GetSecretAsync($"https://{vaultName}.vault.azure.net/secrets/{secret}").ConfigureAwait(false);
            return secretBundle.Value;
        }

        private async Task<string> ReadSecretFromFile()
        {
            var provider = new AzureServiceTokenProvider();
            var accessToken = await provider.GetAccessTokenAsync("https://storage.azure.com/").ConfigureAwait(false);
            TokenCredential tokenCredential = new TokenCredential(accessToken);
            StorageCredentials storageCredentials = new StorageCredentials(tokenCredential);

            var storageAccountName = "nosecretsstorage";
            var containerName = "secrets";
            var fileName = "MyPassword.txt";

            var blob = new CloudBlockBlob(new Uri($"https://{storageAccountName}.blob.core.windows.net/{containerName}/{fileName}"), storageCredentials);
            using (var s = await blob.OpenReadAsync())
            {
                using (var ms = new MemoryStream())
                {
                    await s.CopyToAsync(ms).ConfigureAwait(false);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

    }
}
