# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

[CmdletBinding]
function Enable-FileUtilityAlias {
    process {
        $aliases = @(
            @{
                Name = 'dir'
                Value = 'Microsoft.PowerShell.FileUtility\Get-DirectoryContent'
            }
            @{
                Name = 'copy'
                Value = 'Microsoft.PowerShell.FileUtility\Copy-DirectoryContent'
            }
        )

        foreach ($alias in $aliases) {
            if ($null -ne (Get-Alias -Name $alias.Name -ErrorAction Ignore))
            {
                Remove-Alias -Name $alias.Name -Scope Global
            }

            Set-Alias -Name $alias.Name -Value $alias.Value -Option AllScope -Scope Global
        }
    }
}
