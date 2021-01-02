function Apply-Credentials
{

<#
    Arthur: 
        Chase R. Crawford
    Overview:
        The following function is a way to apply stored credentials at runtime to spcific functions that 
        hold the "-Credential" parameter.
    Example:
        Connect-AzAccount -Subscription 'SomeSubscriptionId' `
                          -Tenant 'SomeTenantId' `
                          -Environment 'AzureCloud' `
                          -Credential(Apply-Credentials -Username "YourUsername" -Password "YourPassword")
#>

[CmdletBinding()]
Param 
(
    [Parameter(Mandatory=$false)]
    [string]$Domain,

    [Parameter(Mandatory=$true)]
    [string]$Username,

    [Parameter(Mandatory=$true)]
    [string]$Password

)
    # Check for Required Assemblies
    Begin 
    {
        
    
    
    }

    Process
    {
        If ($Domain -gt "")
        {
            $Un = $Domain + '\' + $Username
        }
        else 
        {
            $Un = $Username
        }

        $Pwd = ConvertTo-SecureString $Password -AsPlainText -Force
        $Credentials = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList($Un,$Pwd)

        return $Credentials
    }

}

$RemoteCredentials  = Apply-Credentials -Domain  -Username  -Password 

Invoke-Command -ComputerName  `
               -Port 443 `
               -Credential $RemoteCredentials `
               -ScriptBlock{
                                
                                $UserFullName = ""
                                $Username = ""
                                $Password = ""

                                $SecPassword = ConvertTo-SecureString $Password -AsPlainText -Force

                                New-LocalUser -AccountNeverExpires -FullName $UserFullName -Name $Username -Password $SecPassword

                                # Enable Remote Desktop connections
                                If ((Get-ItemPropertyValue ‘HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\‘ -Name “fDenyTSConnections”) -eq 1) 
                                {
                                    Set-ItemProperty ‘HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\‘ -Name “fDenyTSConnections” -Value 0
                                }

                                # Enable Network Level Authentication
                                If((Get-ItemPropertyValue ‘HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\‘ -Name “UserAuthentication” ) -eq 0)
                                {
                                    Set-ItemProperty ‘HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\WinStations\RDP-Tcp\‘ -Name “UserAuthentication” -Value 1
                                }

                                Get-LocalGroup -Name “Remote Desktop Users"
                                Enable-NetFirewallRule -DisplayGroup “Remote Desktop"

                                Add-LocalGroupMember -Group "Remote Desktop Users" -Member $Username
                                Get-ItemPropertyValue ‘HKLM:\SYSTEM\CurrentControlSet\Control\Terminal Server\‘ -Name “fDenyTSConnections”
                            }