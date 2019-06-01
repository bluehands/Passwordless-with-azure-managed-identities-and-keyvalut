using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Fallback_with_Service_Principal
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args?.Length != 3)
            {
                Console.WriteLine("Please provide <TenantId> <AppId> <Secret> as command line args");
                return;
            }
            var tenantId = args[0];
            var appId = args[1];
            var clientSecret = args[2];
            var connectionString = $"RunAs=App;AppId={appId};TenantId={tenantId};AppKey={clientSecret} ";
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider(connectionString);
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net");
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            string vaultName = "NoSecrets-Vault01";
            string secret = "MySecret";
            var secretBundle = await keyVaultClient.GetSecretAsync($"https://{vaultName}.vault.azure.net/secrets/{secret}");
            Console.WriteLine($"The secret is: {secretBundle.Value}");
            Console.WriteLine("Press any key to continue");
            Console.Read();
        }
    }
}
