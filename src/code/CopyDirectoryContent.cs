// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.FileUtility
{
    [Alias("cdc")]
    [Cmdlet(VerbsCommon.Copy, "DirectoryContent", DefaultParameterSetName = "File")]
    public sealed class CopyDirectoryContentCommand : FileUtilityCommandBase
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        [Parameter(Position=0, Mandatory=true)]
        [Alias("Source")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the destination path.
        /// </summary>
        [Parameter(Position=1, Mandatory=true)]
        public string DestinationPath { get; set; }

        /// <summary>
        /// Gets or sets the overwrite flag.
        /// </summary>
        [Parameter]
        public SwitchParameter Overwrite { get; set; }
        protected override void ProcessRecord()
        {

            Path = Util.NormalizePath(Path, SessionState.Path.CurrentFileSystemLocation.Path);
            DestinationPath = Util.NormalizePath(DestinationPath, SessionState.Path.CurrentFileSystemLocation.Path);
            WriteVerbose($"Copying from '{Path}' to '{DestinationPath}'");

            if (!Directory.Exists(DestinationPath))
            {
                WriteVerbose($"Creating directory '{DestinationPath}'");
                Directory.CreateDirectory(DestinationPath);
            }

            List<FileSystemInfo> files = new();

            foreach (FileSystemInfo file in Util.EnumerateDirectory(Path, _enumerationOptions, _enumType))
            {
                files.Add(file);
            }

            int totalFiles = files.Count;
            int copiedFiles = 0;
            string currentFolder = string.Empty;
            foreach (FileSystemInfo file in files)
            {
                copiedFiles++;
                if (file is DirectoryInfo dir)
                {
                    if (!currentFolder.Equals(dir.FullName))
                    {
                        currentFolder = dir.FullName;
                        ProgressRecord progress = new ProgressRecord(0, "Copying", $"Directory '{currentFolder}'");
                        progress.PercentComplete = 100 * copiedFiles / totalFiles;
                        WriteProgress(progress);
                    }

                    DirectoryInfo destDir = new DirectoryInfo(System.IO.Path.Combine(DestinationPath, dir.FullName.Substring(Path.Length + 1)));
                    if (!destDir.Exists)
                    {
                        WriteVerbose($"Creating directory '{destDir.FullName}'");
                        try
                        {
                            Directory.CreateDirectory(destDir.FullName);
                        }
                        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                        {
                            WriteWarning($"Failed to create directory '{destDir.FullName}': {ex.Message}");
                        }
                    }
                }   
                else if (file is FileInfo fileInfo)
                {
                    FileInfo destFile = new FileInfo(System.IO.Path.Combine(DestinationPath, fileInfo.FullName.Substring(Path.Length + 1)));
                    if (destFile.Exists && !Overwrite)
                    {
                        WriteWarning($"File '{destFile.FullName}' already exists. Use -Overwrite to overwrite.");
                    }
                    else
                    {
                        try
                        {
                            WriteVerbose($"Copying file '{fileInfo.FullName}' to '{destFile.FullName}'");
                            fileInfo.CopyTo(destFile.FullName, Overwrite);
                        }
                        catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                        {
                            WriteWarning($"Failed to copy '{fileInfo.FullName}': {ex.Message}");
                        }
                    }
                }
                else
                {
                    WriteWarning($"Skipping '{file.FullName}'");
                }
            }
        }
    }
}
