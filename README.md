# FileUtility

This is an experimental module for optimizing using PowerShell with the file system.

## Goals

- Optimize for performance including throughput and memory usage
- Address usability and intuitive-ness of existing cmdlets that work with the file system
- Work side-by-side with existing file system cmdlets
- Target compatibility with most common cases for existing file system cmdlet usage
- Only expose built-in filtering that can leverage .NET API filtering

## Non-Goals

- Full compatibility with existing cmdlets that work with the FileSystemProvider
- Support for PowerShell provider paths

## Cmdlets

### Get-DirectoryContent

This is equivalent to the Get-ChildItem cmdlet, but optimized for the file system.
Compatible with existing formatting for file system objects for both Unix and Windows.

### Copy-DirectoryContent

This is equivalent to the Copy-Item cmdlet, but optimized for the file system.
Enable use of multiple threads to improve throughput.
Enable useful progress reporting.

### Get-DiskUsage

Given a path (or pattern), returns folders with their total size, number of files, and number of sub-directories.
The top level object with path `.` represents the current folder if it contains files.
It does not represent the total size of the current folder and sub-folders.

### Enable-FileUtilityAlias

Individual cmdlets will already have their own aliases.
What this cmdlet does is overwrite the built-in aliases with the cmdlets that are provided by this module.
Specifically:

| Alias | Cmdlet                |
| ----- | --------------------- |
| dir   | Get-DirectoryContent  |
| copy  | Copy-DirectoryContent |
