


<#

function Add-FunctionAppConfiguratios {

    params(
    
    [parameter(Mandatory=$true)]
    [string]$Username, 

    [parameter(Mandatory=$true)]
    [string]$Password, 

    [parameter(Mandatory=$true)]
    [string]$Password, 

    [parameter(Mandatory=$true)]
    [string]$ResourceGroup, 

    [parameter(Mandatory=$true)]
    [string]$FunctionName,

    [parameter(Mandatory=$true)]
    [string]$Variables 
    
    )

    $Hashed




    Begin {

        

    }

    Process {


    }

}
#>


$Password = 'EScc$9984'
$Username = "ccrawford@eastdilsecured.tech"
$ResourceGroup = "es2-rg-app-dev-wu-01"
$Functions = "es2-func-app-dev-wu-smsmail"


$Password = $Password | ConvertTo-SecureString -AsPlainText -Force
$Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList($Username, $Password)

Connect-AzAccount -Credential $Credentials -Subscription 001769cb-d53d-43ff-9d32-d76b9883a527

foreach($Function in $Functions){


$Configurations = @{}
$Configurations.Add("Smtp:Host", "eastdilsecured-com.mail.eo.outlook.com")
$Configurations.Add("Smtp:Port", 25)
$Configurations.Add("Smtp:Sender", "noreply@eastdilsecured.tech")
$Configurations.Add("Sendgrid:Key", "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")
$Configurations.Add("ServiceBusConntection", "Endpoint=sb://es2-servicebus-dev-wu.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=F5gwk7jq0jhU428lGb+MGkFZ3d+1mCCzJu/TAFVOXgQ=")


Update-AzFunctionAppSetting -Name $Function -ResourceGroupName $ResourceGroup -AppSetting $Configurations -SubscriptionId 001769cb-d53d-43ff-9d32-d76b9883a527

}