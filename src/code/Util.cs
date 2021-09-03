// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Microsoft.PowerShell.FileUtility
{
    internal sealed class Util
    {
        internal const string FileSystemProviderPrefix = "Microsoft.PowerShell.Core\\FileSystem::";

        internal static IEnumerable<FileSystemInfo> EnumerateDirectory(string path, EnumerationOptions options, EnumType enumType)
        {
            if (path.Contains("*") || path.Contains("?"))
            {
                foreach (FileSystemInfo file in EnumerateWildcardDirectory(path, options, enumType))
                {
                    yield return file;
                }
            }
            else
                {
                DirectoryInfo dir = new DirectoryInfo(path);
                IEnumerable<FileSystemInfo> files;
                switch (enumType)
                {
                    case EnumType.File:
                        files = dir.EnumerateFiles("*", options);
                        break;

                    case EnumType.Directory:
                        files = dir.EnumerateDirectories("*", options);
                        break;

                    case EnumType.All:
                        files = dir.EnumerateFileSystemInfos("*", options);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException("Unknown enum type!");
                }

                foreach (FileSystemInfo file in files)
                {
                    yield return file;
                }
            }
        }

        internal static IEnumerable<FileSystemInfo> EnumerateWildcardDirectory(string path, EnumerationOptions options, EnumType enumType)
        {
            // to handle wildcards, we rely on the .NET API, but need to break it up per directory segment
            string[] segments = path.Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder rootPath = new();
            if (path.StartsWith(Path.DirectorySeparatorChar))
            {
                rootPath.Append(Path.DirectorySeparatorChar);
            }

            bool subPathWildcard = false;
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Contains("*") || segments[i].Contains("?"))
                {
                    DirectoryInfo dir = new DirectoryInfo(rootPath.ToString());
                    if (dir.Exists)
                    {
                        // if there was a folder wildcard, we don't want to enumerate the root folder
                        if (!subPathWildcard && i == segments.Length - 1)
                        {
                            IEnumerable<FileSystemInfo> files;
                            switch (enumType)
                            {
                                case EnumType.File:
                                    files = dir.EnumerateFiles(segments[i], options);
                                    break;

                                case EnumType.Directory:
                                    files = dir.EnumerateDirectories(segments[i], options);
                                    break;

                                case EnumType.All:
                                    files = dir.EnumerateFileSystemInfos(segments[i], options);
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException("Unknown enum type!");
                            }

                            foreach (FileSystemInfo file in files)
                            {
                                yield return file;
                            }
                        }
                        else
                        {
                            subPathWildcard = true;
                            foreach (DirectoryInfo subDir in dir.EnumerateDirectories(segments[i]))
                            {
                                string remainingSegments = string.Join(Path.DirectorySeparatorChar, segments, i + 1, segments.Length - i - 1);
                                foreach (FileSystemInfo file in EnumerateWildcardDirectory(subDir.FullName + Path.DirectorySeparatorChar + remainingSegments, options, enumType))
                                {
                                    yield return file;
                                }
                            }
                        }
                    }
                }
                else
                {
                    rootPath.Append(segments[i]);
                    rootPath.Append(Path.DirectorySeparatorChar);
                }
            }
        }

        internal static string NormalizePath(string path, string basePath)
        {
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);

            if (path.Equals("~"))
            {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else if (path.StartsWith("~" + Path.DirectorySeparatorChar))
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), path.Substring(2));
            }

            // if path is relative, make it absolute
            if (!Path.IsPathRooted(path))
            {

                path = Path.GetFullPath(Path.Combine(basePath, path));
            }
 
            return path;
        }

        private static Assembly _SmaAssembly;
        private static Type _unixType;
        private static MethodInfo _GetLStatMethod;

        internal static object GetLStat(string path)
        {
            if (_SmaAssembly == null)
            {
                _SmaAssembly = Assembly.Load(new AssemblyName("System.Management.Automation"));
                _unixType = _SmaAssembly.GetType("System.Management.Automation.Platform+Unix");
                if (_unixType != null)
                {
                    _GetLStatMethod = _unixType.GetMethod("GetLStat");
                }
            }

            if (_GetLStatMethod != null)
            {
                return _GetLStatMethod.Invoke(_unixType, new object[] { path });
            }

            return null;
        }
    }
}
