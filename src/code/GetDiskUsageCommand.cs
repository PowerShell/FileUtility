// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.FileUtility
{
    public struct DiskUsageInfo
    {
        public long TotalSize;
        public long TotalFiles;
        public long TotalDirectories;
        public string Path;

        public DiskUsageInfo(string path, long totalSize, long totalFiles, long totalDirectories)
        {
            Path = path;
            TotalSize = totalSize;
            TotalFiles = totalFiles;
            TotalDirectories = totalDirectories;
        }
    }


    [Alias("gdu")]
    [Cmdlet(VerbsCommon.Get, "DiskUsage")]
    [OutputType(typeof(DiskUsageInfo))]
    public sealed class GetDiskUsageCommand : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        [Parameter(Position=0)]
        public string Path { get; set; } = ".";

        protected override void ProcessRecord()
        {
            Path = Util.NormalizePath(Path, SessionState.Path.CurrentFileSystemLocation.Path);
            List<FileSystemInfo> files = new();
            long currentDirSize = 0;
            long currentDirFiles = 0;
            EnumerationOptions enumerationOptions = new();
            enumerationOptions.RecurseSubdirectories = false;
            enumerationOptions.AttributesToSkip &= ~FileAttributes.Hidden | ~FileAttributes.System;

            List<DirectoryInfo> directories = new();
            foreach (DirectoryInfo dir in Util.EnumerateDirectory(Path, enumerationOptions, EnumType.Directory))
            {
                directories.Add(dir);
            }

            int totalDirectories = directories.Count;

            foreach (FileInfo file in Util.EnumerateDirectory(Path, enumerationOptions, EnumType.File))
            {
                currentDirSize += file.Length;
                currentDirFiles++;
            }

            WriteObject(new DiskUsageInfo(path: ".", totalSize: currentDirSize, totalFiles: currentDirFiles, totalDirectories: totalDirectories));

            int calculatedDirs = 0;
            enumerationOptions.RecurseSubdirectories = true;

            foreach (DirectoryInfo dir in directories)
            {
                ProgressRecord progress = new ProgressRecord(0, "Calculating", $"Directory '{dir.Name}'");
                progress.PercentComplete = 100 * calculatedDirs / totalDirectories;
                WriteProgress(progress); 

                long currentDirDirs = 0;
                currentDirSize = 0;
                currentDirFiles = 0;
                foreach (FileSystemInfo file in Util.EnumerateDirectory(dir.FullName, enumerationOptions, EnumType.All))
                {
                    if (file is FileInfo fileInfo)
                    {
                        currentDirSize += fileInfo.Length;
                        currentDirFiles++;
                    }

                    if (file is DirectoryInfo dirInfo)
                    {
                        currentDirDirs++;
                    }
                }

                WriteObject(new DiskUsageInfo(path: dir.FullName, totalSize: currentDirSize, totalFiles: currentDirFiles, totalDirectories: currentDirDirs));
                calculatedDirs++;
            }
        }
    }
}
