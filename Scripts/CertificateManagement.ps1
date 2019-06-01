$subscriptionName = ""
$keyVaultName = "NoSecrets-Vault05"
$signingCertificateName = "SigningCertificate01"
$encryptionCertificateName = "EncryptionCertificate02"
$sslCertificateName = "SslCertificate03"

Connect-AzureRmAccount
Set-AzureRmContext -SubscriptionName $subscriptionName

### Create Certificate for signing
$certificatepolicySigning = New-AzureKeyVaultCertificatePolicy -IssuerName Self -SubjectName "CN=am@bluehands.de, O=bluehands"  -SecretContentType "application/x-pkcs12"  -ReuseKeyOnRenewal $false -KeyUsage DigitalSignature  -Ekus "Document Signing" -KeyType "RSA-HSM" -KeyNotExportable 
Add-AzureKeyVaultCertificate -VaultName $keyVaultName  -Name $signingCertificateName -CertificatePolicy $certificatepolicySigning
$cert = Get-AzureKeyVaultCertificate -VaultName $keyVaultName -Name $signingCertificateName
Write-Output $cert

### Create Certificate for encryption
$certificatepolicyEncryption = New-AzureKeyVaultCertificatePolicy -IssuerName Self -SubjectName "CN=am@bluehands.de, O=bluehands"  -SecretContentType "application/x-pkcs12"  -ReuseKeyOnRenewal $false -KeyUsage DataEncipherment -KeyType "RSA-HSM" -KeyNotExportable 
Add-AzureKeyVaultCertificate -VaultName $keyVaultName  -Name $encryptionCertificateName -CertificatePolicy $certificatepolicyEncryption
$cert = Get-AzureKeyVaultCertificate -VaultName $keyVaultName -Name $encryptionCertificateName
Write-Output $cert

### Create Certificate for SSL
$certificatepolicySSL = New-AzureKeyVaultCertificatePolicy -IssuerName Unknown -SubjectName "CN=www.bluehands.de, O=bluehands"  -SecretContentType "application/x-pkcs12"  -ReuseKeyOnRenewal $false -KeyUsage DigitalSignature, KeyEncipherment -Ekus "Server Authentication", "Client Authentication" -KeyType "RSA-HSM" -KeyNotExportable 
$certificateOperation = Add-AzureKeyVaultCertificate -VaultName $keyVaultName  -Name $sslCertificateName -CertificatePolicy $certificatepolicySSL
$certificateOperation | Select-Object -ExpandProperty CertificateSigningRequest | Set-Content -Path "c:\Temp\request.csr"

## Send to CA

## Import crt
Import-AzureKeyVaultCertificate -VaultName $keyVaultName -Name $sslCertificateName -FilePath C:\Temp\request.crt 
