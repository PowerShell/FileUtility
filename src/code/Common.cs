// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace Microsoft.PowerShell.FileUtility
{
    internal enum EnumType
    {
        All,
        File,
        Directory,
    }

    public class FileUtilityCommandBase : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the recurse flag.
        /// </summary>
        [Parameter]
        public SwitchParameter Recurse { get; set; }

        /// <summary>
        /// Gets or sets the include hidden flag.
        /// </summary>
        [Parameter]
        public SwitchParameter IncludeHidden { get; set; }

        /// <summary>
        /// Gets or sets the traverse symlinks flag.
        /// </summary>
        [Parameter]
        public SwitchParameter TraverseSymlink { get; set; }

        /// <summary>
        /// Gets or sets the file flag.
        /// </summary>
        [Parameter(ParameterSetName = "File")]
        public SwitchParameter FileOnly { get; set; }

        /// <summary>
        /// Gets or sets the directory flag.
        /// </summary>
        [Parameter(ParameterSetName = "Directory")]
        public SwitchParameter DirectoryOnly { get; set; }

        internal EnumType _enumType = EnumType.All;
        internal EnumerationOptions _enumerationOptions = new EnumerationOptions();

        protected override void BeginProcessing()
        {
            if (Recurse)
            {
                _enumerationOptions.RecurseSubdirectories = true;
            }

            if (IncludeHidden)
            {
                _enumerationOptions.AttributesToSkip &= ~FileAttributes.Hidden | ~FileAttributes.System;
            }

            if (!TraverseSymlink)
            {
                _enumerationOptions.AttributesToSkip |= FileAttributes.ReparsePoint;
            }

            if (FileOnly)
            {
                _enumType = EnumType.File;
            }

            if (DirectoryOnly)
            {
                _enumType = EnumType.Directory;
            }
        }
    }
}
