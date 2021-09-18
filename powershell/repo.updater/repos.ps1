
$variable = "REPOS"
$target = [System.EnvironmentVariableTarget]::User
$repos = [System.Environment]::GetEnvironmentVariable($variable, $target)

if ($repos -ne $null) {

    foreach($repo in $repos.Split(';')) {

        Set-Location $repo
        Write-Host  (Get-Location).Path
        Start-Process 'C:\Users\ccrawford\AppData\Local\Programs\Git\cmd\git.exe' -ArgumentList('pull') -Wait -WindowStyle Hidden
    }
} 
else {
    [System.Environment]::SetEnvironmentVariable($variable, $target)
}