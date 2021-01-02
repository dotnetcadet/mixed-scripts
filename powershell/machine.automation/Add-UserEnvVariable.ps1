
function Add-UserEnvVariable {

    Param(
       [Parameter(Mandatory=$true)]
       [string]$Variable,
       
       [Parameter(Mandatory=$true)]
       [string]$Value 
    )


    $Target = [System.EnvironmentVariableTarget]::User

    [System.Environment]::SetEnvironmentVariable(
        $Variable,[string]::Join(';',
            $Variable,
            [System.Environment]::GetEnvironmentVariable($Variable, $Target)), $Target)

    foreach($var in [System.Environment]::GetEnvironmentVariable($Variable,$Target).Split(';')){

        Write-Host $var
    }
}


Add-UserEnvVariable -Variable Path -Value "C:\Windows\System32"