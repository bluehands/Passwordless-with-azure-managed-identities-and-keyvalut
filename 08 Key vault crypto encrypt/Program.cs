using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace _08_Key_vault_crypto_operiation_encrypt
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var keyIdentifier = new KeyIdentifier("https://nosecrets-vault05.vault.azure.net:443/keys/EncryptionCertificate02/5cc49b815ea349c5bda9d08366ea780f");
            var secretIdentifier = new SecretIdentifier("https://nosecrets-vault05.vault.azure.net:443/secrets/EncryptionCertificate02/5cc49b815ea349c5bda9d08366ea780f");

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(secretIdentifier.Identifier);
            var publicCertificate = new X509Certificate2(Convert.FromBase64String(secret.Value), string.Empty, X509KeyStorageFlags.Exportable);

            var rsa = keyVaultClient.ToRSA(keyIdentifier, publicCertificate);

            var plainText = "Hello World";
            var encryptedData = Encrypt(rsa, plainText);
            var decryptedData = Decrypt(rsa, encryptedData);
            Console.WriteLine("-------------decrypted string------------");
            Console.WriteLine(decryptedData);

            Console.WriteLine("Press any key to continue");
            Console.Read();
        }

        private static byte[] Encrypt(RSA rsa, string plainText)
        {
            return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.OaepSHA256);
        }
        private static string Decrypt(RSA rsa, byte[] data)
        {
            var decryptedData = rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
            return Encoding.UTF8.GetString(decryptedData);
        }
       
    }
}
