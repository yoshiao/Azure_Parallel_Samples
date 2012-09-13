#Uses Windows Azure Service Management Cmdlets
$account = 'STORAGE_ACCOUNT'
$key = 'STORAGE_ACCOUNT_KEY'
$deploymentId = 'DEPLOYMENT_ID'
$roleName = 'WEB_ROLE_NAME'
$bufferQuotaInMB = 100
$transferPeriod = 60
$logLevelFilter = 5

Add-PSSnapin AzureManagementToolsSnapIn

Set-WindowsAzureLog -BufferQuotaInMB $bufferQuotaInMB -TransferPeriod $transferPeriod -LogLevelFilter $logLevelFilter -DeploymentId $deploymentId -RoleName $roleName -StorageAccountname $account -StorageAccountKey $key