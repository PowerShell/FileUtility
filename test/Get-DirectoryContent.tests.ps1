Describe 'Get-DirectoryContent tests' {
    BeforeEach {
        $dirStructure = @(
            @{ ItemType = 'Directory'; Name = 'dir1' }
            @{ ItemType = 'Directory'; Name = 'dir1/dir2' }
            @{ ItemType = 'File'; Name = 'file1' }
            @{ ItemType = 'File'; Name = 'dir1/file2' }
            @{ ItemType = 'SymbolicLink'; Name = 'link1'; Value = 'file1' }
        )

        foreach ($item in $dirStructure) {
            $params = @{
                ItemType = $item.ItemType;
                Path = (Join-Path $TestDrive $item.Name);
            }

            if ($item.ItemType -eq 'SymbolicLInk') {
                $params.Value = $item.Value;
            }

            $null = New-Item @params -Force
        }
    }

    It 'Output should be same as Get-ChildItem with Recurse <Recurse> for absolute path: <AbsolutePath>' -TestCases @(
        @{ AbsolutePath = $true ; Recurse = $false }
        @{ AbsolutePath = $false; Recurse = $false }
        @{ AbsolutePath = $true ; Recurse = $true }
        @{ AbsolutePath = $false; Recurse = $true }
    ){
        param($AbsolutePath, $Recurse)

        try {
            Push-Location $TestDrive
            if ($AbsolutePath) {
                $Path = $TestDrive
            }
            else {
                $Path = '.'
            }

            $gdc = Get-DirectoryContent -Path $Path -Recurse:$Recurse | Sort-Object Name | Out-String
            $gdc.Count | Should -BeGreaterThan 0
            $gci = Get-ChildItem -Path $Path -Recurse:$Recurse | Sort-Object Name | Out-String
            $gdc | Should -BeExactly $gci
        }
        finally {
            Pop-Location
        }
    }

    It 'Error if path does not exist' {
        { Get-DirectoryContent -Path (Join-Path $TestDrive (New-Guid)) } | Should -Throw -ErrorId 'System.IO.DirectoryNotFoundException,Microsoft.PowerShell.FileUtility.GetDirectoryContentCommand'
    }

    It 'Works with hidden items' {
        $hiddenFilePath = (Join-Path $TestDrive '.hiddenFile')
        $hiddenDirPath = (Join-Path $TestDrive '.hiddenDir')

        $null = New-Item -Path $hiddenFilePath -ItemType File
        $null = New-Item -Path $hiddenDirPath -ItemType Directory

        if ($IsWindows) {
            [System.IO.File]::SetAttributes($hiddenFilePath, 'Hidden')
            [System.IO.File]::SetAttributes($hiddenDirPath, 'Hidden')    
        }

        Get-DirectoryContent -Path $TestDrive/*hid* | Should -BeNullOrEmpty
        $h = Get-DirectoryContent -Path $TestDrive/*hid* -IncludeHidden
        $h.Count | Should -Be 2
        $h.FullName | Should -Contain $hiddenFilePath
        $h.FullName | Should -Contain $hiddenDirPath
    }

    It 'Specifying both -FileOnly and -DirectoryOnly should fail' {
        { Get-DirectoryContent . -FileOnly -DirectoryOnly } | Should -Throw -ErrorId 'AmbiguousParameterSet'
    }

    It 'Specifying -FileOnly returns files only' {
        $f = Get-DirectoryContent -Path $TestDrive -FileOnly -Recurse
        $f.Count | Should -Be 2
        $f | Should -BeOfType [System.IO.FileInfo]
    }

    It 'Specifying -DirectoryOnly returns directories only' {
        $d = Get-DirectoryContent -Path $TestDrive -DirectoryOnly -Recurse
        $d.Count | Should -Be 2
        $d | Should -BeOfType [System.IO.DirectoryInfo]
    }

    It 'On Unix the stat information should be returned' -Skip:$IsWindows {
        $f = Get-DirectoryContent -Path $TestDrive -FileOnly | Select-Object -First 1
        $f.UnixMode | Should -Not -BeNullOrEmpty
    }
}
