#Cerebrata cmdlets
$account = "STORAGE_ACCOUNT"
$key = "STORAGE_ACCOUNT_KEY"
$deploymentId = "DEPLOYMENT_ID"
$roleName = "WEB_ROLE_NAME"
$instanceId = "INSTANCE_ID";
$bufferQuotaInMB = 102
$scheduledTransferPeriod = 60
$logLevelFilter = "Verbose"

Add-PSSnapin AzureManagementCmdletsSnapIn

Set-WindowsAzureLog -BufferQuotaInMB $bufferQuotaInMB -ScheduledTransferPeriod $scheduledTransferPeriod -LogLevelFilter $logLevelFilter -DeploymentId $deploymentId -RoleName $roleName -InstanceId $instanceId -AccountName $account -AccountKey $key