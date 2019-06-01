# Some instructions to start with Passwordless and Azure KeyVault

## Use Azure CLI to logon and get an Access Token
az login
az account set --subscription [subscription-id]
az account list
az account get-access-token --resource https://storage.azure.com/

Inspect the token in http://jwt.ms

## Azure Services and Azure Resources support MI
See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities to get a list of Services and Ressources which supports MI & RBAC

### Use Postman to get an Access Token in a VM
Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://storage.azure.com/ with header metadata set to true

Go to https://nosecretsstorage01.blob.core.windows.net/secrets/MyBlobContent.txt with the Access Token as bearer and header x-ms-version set to 2018-11-09

### Use Azure CLI to set key vault policy for VM Principal
az vm identity show --name vs2019 --resource-group nosecrets

az keyvault set-policy --name NoSecrets-MyVault01 --object-id ed9d105a-fe50-4128-8547-120627d00919 --secret-permissions get list

Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://vault.azure.net/ with header metadata set to true

Go to https://nosecrets-myvault01.vault.azure.net/secrets/mysecret?api-version=2016-10-01 with the Access Token as bearer 

## Use Secrets in Configuration
Add user secrets to the project. See https://docs.microsoft.com/de-de/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows

Inspect %APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

Add key vault extension to project. See https://docs.microsoft.com/de-de/aspnet/core/security/key-vault-configuration?view=aspnetcore-2.2

## Bind Azure Function to Key Vault
### Use special syntax for key vault setting
Deploy the Function as usual. Update the Config with the special syntax for key vault setting. See https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references#reference-syntax

Connection to SB: ServiceBusConnection = @Microsoft.KeyVault(SecretUri=https://nosecrets-vault03.vault.azure.net/secrets/ServiceBusConnection/f647aa1d6f6e4de99260cb9a6e98f390)

### Use key vault extension
Replace the default IConfiguration with the key vault configuration. See https://blog.wille-zone.de/post/azure-keyvault-for-azure-functions/

AzureKeyVault_Uri = https://nosecrets-vault04.vault.azure.net/

## Use Service Principal for not supported Services
Create a Service Principal az ad sp create-for-rbac --name NoSecretsService01

Remember AppId and PWD

### Use Key Vault to store SSL Certificates
Add a certificate to key vault
Import certificate in to app service
Add binding to certificate 

### Use Key Vault for cryptographic operations 
Create certificates for signng and encryption

Get KeyIndentifier & SecretIdentifier

Add a implementaion for RSA with Key Vault to the project. See https://github.com/onovotny/RSAKeyVaultProvider

### Use DB with always encrypted and key vault
Create a Sql Db in Azure Portal
Create a Key Vault

Deploy Table
Run Sample. In Code the data is in plain text
In SSMS data is encrypted

Add Column Encryption Setting=enabled to connection String