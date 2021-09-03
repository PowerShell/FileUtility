// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.FileUtility
{
    [Alias("gdc")]
    [Cmdlet(VerbsCommon.Get, "DirectoryContent", DefaultParameterSetName = "File")]
    [OutputType(typeof(FileSystemInfo[]))]
    public sealed class GetDirectoryContentCommand : FileUtilityCommandBase
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        [Parameter(Position=0, Mandatory=false)]
        public string Path { get; set; } = "."; 

        protected override void ProcessRecord()
        {
            Path = Util.NormalizePath(Path, SessionState.Path.CurrentFileSystemLocation.Path);

            foreach (FileSystemInfo file in Util.EnumerateDirectory(Path, _enumerationOptions, _enumType))
            {
                PSObject result = new PSObject(file);
                if (Environment.OSVersion.Platform == PlatformID.Unix)
                {
                    result.Properties.Add(new PSNoteProperty("UnixStat", Util.GetLStat(file.FullName)));
                }

                if (file is DirectoryInfo dir)
                {
                    result.Properties.Add(new PSNoteProperty("PSParentPath", Util.FileSystemProviderPrefix + dir.Parent));
                }
                else if (file is FileInfo fileInfo)
                {
                    result.Properties.Add(new PSNoteProperty("PSParentPath", Util.FileSystemProviderPrefix + fileInfo.Directory));
                }

                WriteObject(result);
            }
        }
    }
}
