using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;

namespace _07_Key_vault_crypto_operation_sign
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var keyIdentifier = new KeyIdentifier("https://nosecrets-vault05.vault.azure.net:443/keys/SigningCertificate01/2c86727652f14814b77018601a2e5ed4");
            var secretIdentifier = new SecretIdentifier("https://nosecrets-vault05.vault.azure.net:443/secrets/SigningCertificate01/2c86727652f14814b77018601a2e5ed4");

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(secretIdentifier.Identifier);
            var publicCertificate = new X509Certificate2(Convert.FromBase64String(secret.Value), string.Empty, X509KeyStorageFlags.Exportable);

            var rsa = keyVaultClient.ToRSA(keyIdentifier, publicCertificate);

            var xml = GetXml();
            var signedXml = SignXml(rsa, xml);
            Console.WriteLine("-------------signed xml------------");
            Console.WriteLine(signedXml);

            Console.WriteLine("Press any key to continue");
            Console.Read();
        }

        static string SignXml(RSA rsa, string xml)
        {
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xml);

            var signedXml = new SignedXml(xmlDoc) { SigningKey = rsa };
            var reference = new Reference { Uri = "" };

            var env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            signedXml.AddReference(reference);
            signedXml.ComputeSignature();
            var xmlDigitalSignature = signedXml.GetXml();

            xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));
            return xmlDoc.OuterXml;
        }
        private static string GetXml()
        {
            var xml =
                @"<root>  
    <person>  
        <name>Aydin</name>  
        <birthday>22.06.1966</birthday>  
    </person>  
</root>";
            return xml;
        }
    }
}
