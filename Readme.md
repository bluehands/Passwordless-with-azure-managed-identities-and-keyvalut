# Some instructions to start with Passwordless and Azure KeyVault

## Use Azure CLI to logon and get an Access Token
az login
az account set --subscription [subscription-id]
az account list
az account get-access-token --resource https://storage.azure.com/

Inspect the token in http://jwt.ms

## Azure Services and Azure Resources support MI
See https://docs.microsoft.com/en-us/azure/active-directory/managed-identities-azure-resources/services-support-managed-identities to get a list of Services and Ressources which supports MI & RBAC

## Use Azure CLI to set key vault policy for VM Principal

az vm identity show --name vs1019 --resource-group nosecrets
az keyvault set-policy --name NoSecrets-MyVault01 --object-id ed9d105a-fe50-4128-8547-120627d00919 --secret-permissions get list

## Use Postman to get an Access Token in a VM
Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://storage.azure.com/ with header metadata set to true
Go to https://nosecretsstorage.blob.core.windows.net/secrets/MyPassword.txt with the Access Token as bearer and header x-ms-version set to 2018-11-09

Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://vault.azure.net/ with header metadata set to true
Go to https://nosecrets-myvault01.vault.azure.net/secrets/mysecret?api-version=2016-10-01 with the Access Token as bearer 


