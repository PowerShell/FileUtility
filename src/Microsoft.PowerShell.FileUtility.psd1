# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

@{
    RootModule = './Microsoft.PowerShell.FileUtility.dll'
    NestedModules = @(
        './Microsoft.PowerShell.FileUtility.psm1'
    )
    ModuleVersion = '0.1.0'
    CompatiblePSEditions = @('Core')
    GUID = 'c0d47756-1042-40ac-8290-1ea5b9f34977'
    Author = 'Microsoft Corporation'
    CompanyName = 'Microsoft Corporation'
    Copyright = '(c) Microsoft Corporation.'
    Description = "This module contains cmdlets optimized for working with the file system."
    PowerShellVersion = '7.0'
    FormatsToProcess = @('Microsoft.PowerShell.FileUtility.format.ps1xml')
    CmdletsToExport = @(
        'Copy-DirectoryContent'
        'Get-DirectoryContent'
        'Get-DiskUsage'
        'Get-DriveInfo'
        'Enable-FileUtilityAlias'
    )
    PrivateData = @{
        PSData = @{
            LicenseUri = 'https://github.com/SteveL-MSFT/FileUtility/blob/main/LICENSE'
            ProjectUri = 'https://github.com/SteveL-MSFT/FileUtility'
            ReleaseNotes = 'Initial beta release'
            Prerelease = 'Beta1'
        }
    }
}
