function Check-AssemblyReferences
{
<#
    Arthur:
        Chase R. Crawford
    Overview:
        The following function it to check for available assemblies under the current domain of the user
#>

Param
(
    [Parameter(Mandatory=$True)]
    [string]$TypeName

)


$AssemblyExist = $false

$Obj = [System.AppDomain]::CurrentDomain.GetAssemblies().GetTypes() | Select $_.BaseType

    Foreach ($O in $Obj) 
    {
        $AssemblyList = $O.ToString()

        If (($AssemblyList.Trim()) -eq $TypeName)
        {
            $AssemblyExist = $true
            return $AssemblyExist
            break
    
        }
    }

    return $AssemblyExist
}