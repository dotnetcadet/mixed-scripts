

$Path = 'C:\Users\ccrawford\source\repos\assimalign\github\dotnet-packages\src\libraries\Assimalign.Text\Assimalign.Text.Json\*'


Get-ChildItem -Path $Path -Include *.cs -Recurse | 
ForEach-Object{

    $content = Get-Content $_.FullName -Raw

    $start = $content.IndexOf('#region License',0)
    $end = $content.IndexOf('#endregion',0)


    if($start -ge 0 -and $end -gt 0){
       
        Write-Host $content
        Set-Content $_.FullName -Value(
            $content.Substring($content.Substring($start, ($end + '#endregion'.Length)).Length,  
            $content.Length -  $content.Substring($start, ($end + '#endregion'.Length)).Length))
    }
}
