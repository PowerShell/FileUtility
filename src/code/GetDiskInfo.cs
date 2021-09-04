// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.FileUtility
{
    [Alias("gdi")]
    [Cmdlet(VerbsCommon.Get, "DriveInfo")]
    [OutputType(typeof(DriveInfo))]
    public sealed class GetDriveInfoCommand : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
            {
                WriteObject(driveInfo);
            }
        }
    }
}
