# FileUtility

This is an experimental module for optimizing using PowerShell with the file system.

> **NOTE!** As this is an experimental module, it is currently not intended to be used in production.
> Use at your own risk!  However, feedback is greatly appreciated.

## Goals

- Optimize for performance including throughput and memory usage
- Address usability and intuitive-ness of existing cmdlets that work with the file system
  - Use standard commands from Unix like `ls`, `cp`, `du`, etc... as baseline
- Work side-by-side with existing file system cmdlets which leverage the FileSystemProvider
- Target compatibility with most common cases for existing file system cmdlet usage
- Only expose built-in filtering that can leverage .NET API filtering (all other filtering should just use PowerShell pipeline)

## Non-Goals

- Full compatibility with existing cmdlets that work with the FileSystemProvider
- Support for PowerShell provider paths

## Why not fix in PowerShell 7?

The current file system cmdlets (`Get-ChildItem`, `Copy-Item`, etc...) are not specific to the file system
which means they are more generalized for different providers (such as Registry, Certificate, etc...) including the file system.
This layering adds complexity and naturally impacts performance.
Making even small bug fixes or enhancements in the file systemn provider has routinely been a source of regressions.
The file system provider was also originally designed for the Windows NTFS relying on calling native Win32 APIs with
alternate code paths for Unix-like operating systems.

Eventually, based on the feedback, some of the learnings and code from this project may be ported to PowerShell 7.

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
