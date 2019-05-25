# Some instructions to start with Passwordless and Azure KeyVault

## Use Azure CLI to logon and get an Access Token
az login
az account set --subscription [subscription-id]
az account list
az account get-access-token --resource https://storage.azure.com/

Inspect the token in http://jwt.ms
Inspect MSI_ENDPOINT and MSI_SECRET Environment variables in VM to see the auth url

## Use Postman to get an Access Token in a VM
Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://storage.azure.com/ with header metadata set to true
Go to https://nosecretsstorage.blob.core.windows.net/secrets/MyPassword.txt with the Access Token as bearer and header x-ms-version set to 2018-11-09

Go to http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https://vault.azure.net/ with header metadata set to true
Go to https://nosecrets-myvault01.vault.azure.net/secrets/mysecret?api-version=2016-10-01 with the Access Token as bearer 

