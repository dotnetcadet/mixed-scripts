


$Path = "C:\Users\ccrawford\source\repos\eastdil.secured\eastdil-packages\src\libraries\Eastdil.AspNetCore.Mvc\"



#------------------------------------------------------------------------------------#

$using_references = @{}
$attribute_references = @{}

if($Path.Substring($Path.Length - 1, 1) -ne '\'){
    $Path = ($Path + '\')
}


# Replacement 01: Replace 'namespace' keyword references
Get-ChildItem $Path -Recurse -Include *.cs | 
ForEach-Object{

    $begin_write = $false
    $namespace = "namespace "
    $project_name = $_.FullName.Substring($Path.Length, $_.FullName.Substring($Path.Length, $_.FullName.Length - $Path.Length).IndexOf('\'))

    # Step 01: Build New Namespace off of Folder Structure
    foreach($folder in $_.FullName.Split('\')) {
        
        if($project_name -eq $folder -or $begin_write -eq $true){

            $begin_write = $true

            if($folder.Contains(".cs") -eq $false){
                $namespace = $namespace + $folder + '.'
            }
            else{
                $namespace = $namespace.Substring(0, $namespace.Length - 1)
            }
        }
    }

    # Step 02: Get Namespace Line from Current File
    $namespace_line = Get-Content $_.FullName | Select-String namespace | Select-Object -ExpandProperty Line

    # Step 03: Get Raw Content From Cs File
    $old_content = Get-Content $_.FullName -Raw


    # Step 04: Validate Content is not Null & Contains 'namespace' keyword, then replace
    if($null -ne $old_content -and $old_content.Contains("namespace") -eq $true){
    
        $new_content = $old_content.Replace($namespace_line, $namespace)

        # Step 04:1 : Get Using Reference old & new and add to Hash Table for Reiteration to replace values
        $reference_replacement = (
               ($namespace_line.Replace("namespace","using").Trim() + ';') + '|' + 
               ($namespace.Replace("namespace","using").Trim() + ';')
        )


        if($null -ne $using_references -and $using_references.ContainsValue($reference_replacement) -ne $true){
            $using_references.Add(($using_references.Count + 1), $reference_replacement)
        }
         
        Set-Content $_.FullName -Value $new_content
    }
}


# Replacement 02: Replace 'using' keyword references
Get-ChildItem $Path -Recurse -Include *.cs | 
ForEach-Object{
    
    $count = 0
    $content = Get-Content $_.FullName -Raw
    $replacement = $null

    $using_references.GetEnumerator() | ForEach-Object{
        
        if($null -ne $using_references -and $null -ne $content){
            
            #Write-Host $_.Value

            $old_reference = $_.Value.Split('|')[0]
            $new_reference = $_.Value.Split('|')[1]

            if($count -eq 0){
                $replacement = $content.Replace($old_reference, $new_reference) 
                $ount++
            }
            else {
                $replacement = $replacement.Replace($old_reference, $new_reference)
            }
        }
    }

    Set-Content $_.FullName -Value $replacement
}



<# Replacement 03: Replace Project References
Get-ChildItem $Path -Recurse -Include *.csproj | 
ForEach-Object{

    $project_directory = $_.FullName.Substring($Path.Length, $_.FullName.Length - $Path.Length)
}
#>