Import-Module SqlServer

function Add-ReadOnlyEntity {
    
    Param(

        [Parameter(Mandatory=$true)]
        [string]$ConnectionString,

        [Parameter(Mandatory=$true)]
        [string]$Database,

        [Parameter(Mandatory=$true)]
        [string]$Schema,

        [Parameter(Mandatory=$true)]
        [string]$Table,

        [Parameter(Mandatory=$true)]
        [string]$Path,

        [Parameter(Mandatory=$true)]
        [string]$Namespace
    )

    # Step 01 : Get Sql Script in Root Module Folder (Replace all Variables with passed through Parameters)
    foreach($ModulePath in $env:PSModulePath.Split(';')) {
        
        $Path = "$ModulePath\EntityFrameworkToolsExtensions\sql.scripts\schema.info.sql"

        if((Test-Path $Path -ErrorAction SilentlyContinue) -eq $true) {
            
            $Query = (Get-Content -Path $Path )
            $Query = $Query.Replace('$(Schema)', $Schema)
            $Query = $Query.Replace('$(Table)',$Table)
            $Query = $Query.Replace('''$(Database)''',$Database)
            $Query = [string]::join(" ",$Query)
        }
    }

    # Step 02 : Invoke Query and Retrieve Schema Information
    $Rows = Invoke-Sqlcmd -ConnectionString $ConnectionString  -Query $Query
    $ColumnProperties = New-Object Collections.Generic.List[string]

    # Step 03 : Begin Looping through Each Row and scaffolding Property for CSharp Model
    foreach($Row in $Rows) {
        
        $PropertyType = $Row.PropertyType
        $PrimaryKeyAttribute = $Row.KeyAttribute
        $ColumnAttributeType = $Row.ColumnAttributeType
        $OriginalColumnName = $Row.OrginalColumn
        $PropertyName = $Row.TransformedColumnName
        $PropertyDefaultValue = $Row.DefaultPropertyType
        $Property = @"

        $PrimaryKeyAttribute
        [Column("$OriginalColumnName", TypeName = "$ColumnAttributeType")]
        public $PropertyType $PropertyName { get; set; } $PropertyDefaultValue
        
"@;
        # Add Property to Generic List
        $ColumnProperties.Add($Property)    
    }

    $class = @"
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace $Namespace
{
    [Table("$Table", Schema = "$Schema")]
    public class $Table
    {

"@;
    
    # Step 04 : Begin writing all properties to csharp class
    foreach($prop in $ColumnProperties) {
        $class = $class + $prop 
    }

    # Step 05 : Add the ending Brackets to finish encapsulation
    $class = $class + @"
    }
}
"@;

    $Path = $Path + "\$Table.cs" # Add Model name with csharp file extension
    $Path = $Path.Replace("\\","\") #Remove double backslashes

    if((Test-Path $Path -ErrorAction SilentlyContinue) -eq $false) {
        New-Item -Path $Path -ItemType File
    }

    Set-Content -Path $Path -Value($class) 
}

function Add-ReadOnlyEntities {
    Param (

        [Parameter(Mandatory=$true)]
        [string]$ConnectionString,

        [Parameter(Mandatory=$true)]
        [string]$Database,

        [Parameter(Mandatory=$true)]
        [string]$Schema,

        [Parameter(Mandatory=$true)]
        [string]$Path,

        [Parameter(Mandatory=$true)]
        [string]$Namespace
    )
}

function Add-DataContextFromModels {
    Param (
        [Parameter(Mandatory=$true)]
        [string]$Path
    )
}

Export-ModuleMember `
    -Function Add-ReadOnlyEntity, Add-DataContextFromModels, Add-ReadOnlyEntities