//
// Revit Batch Processor
//
// Copyright (c) 2019  Daniel Rumery, BVN
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace BatchRvtUtil
{
    public static class RevitVersion
    {
        public enum SupportedRevitVersion {
                Revit2015 = 0,
                Revit2016 = 1,
                Revit2017 = 2,
                Revit2018 = 3,
                Revit2019 = 4,
                Revit2020 = 5
        }

        private const string REVIT_EXECUTABLE_FILE_NAME = "Revit.exe";

        private static readonly Dictionary<SupportedRevitVersion, string> REVIT_ADDINS_RELATIVE_PATHS =
            new Dictionary<SupportedRevitVersion, string>()
            {
                { SupportedRevitVersion.Revit2015, @".\Autodesk\Revit\Addins\2015" },
                { SupportedRevitVersion.Revit2016, @".\Autodesk\Revit\Addins\2016" },
                { SupportedRevitVersion.Revit2017, @".\Autodesk\Revit\Addins\2017" },
                { SupportedRevitVersion.Revit2018, @".\Autodesk\Revit\Addins\2018" },
                { SupportedRevitVersion.Revit2019, @".\Autodesk\Revit\Addins\2019" },
                { SupportedRevitVersion.Revit2020, @".\Autodesk\Revit\Addins\2020" }
            };

        private static readonly Dictionary<SupportedRevitVersion, string> SUPPORTED_REVIT_VERSION_NUMBERS =
            new Dictionary<SupportedRevitVersion, string>()
            {
                { SupportedRevitVersion.Revit2015, "2015" },
                { SupportedRevitVersion.Revit2016, "2016" },
                { SupportedRevitVersion.Revit2017, "2017" },
                { SupportedRevitVersion.Revit2018, "2018" },
                { SupportedRevitVersion.Revit2019, "2019" },
                { SupportedRevitVersion.Revit2020, "2020" }
            };

        private static readonly Dictionary<SupportedRevitVersion, IEnumerable<string>> REVIT_EXECUTABLE_FOLDER_PATHS =
            new Dictionary<SupportedRevitVersion, IEnumerable<string>>()
            {
                {
                    SupportedRevitVersion.Revit2015,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2015",
                        @"C:\Program Files\Autodesk\Revit Architecture 2015",
                        @"D:\Program Files\Autodesk\Revit 2015",
                        @"D:\Program Files\Autodesk\Revit Architecture 2015"
                    }
                },
                {
                    SupportedRevitVersion.Revit2016,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2016",
                        @"C:\Program Files\Autodesk\Revit Architecture 2016",
                        @"D:\Program Files\Autodesk\Revit 2016",
                        @"D:\Program Files\Autodesk\Revit Architecture 2016",
                    }
                },
                {
                    SupportedRevitVersion.Revit2017,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2017",
                        @"D:\Program Files\Autodesk\Revit 2017"
                    }
                },
                {
                    SupportedRevitVersion.Revit2018,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2018",
                        @"D:\Program Files\Autodesk\Revit 2018"
                    }
                },
                {
                    SupportedRevitVersion.Revit2019,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2019",
                        @"D:\Program Files\Autodesk\Revit 2019"

                    }
                },
                {
                    SupportedRevitVersion.Revit2020,
                    new [] {
                        @"C:\Program Files\Autodesk\Revit 2020",
                        @"D:\Program Files\Autodesk\Revit 2020"
                    }
                }
            };

        private static readonly Dictionary<SupportedRevitVersion, string> REVIT_LOCAL_FOLDER_PATHS =
            new Dictionary<SupportedRevitVersion, string>()
            {
                { SupportedRevitVersion.Revit2015, @"C:\REVIT_LOCAL2015" },
                { SupportedRevitVersion.Revit2016, @"C:\REVIT_LOCAL2016" },
                { SupportedRevitVersion.Revit2017, @"C:\REVIT_LOCAL2017" },
                { SupportedRevitVersion.Revit2018, @"C:\REVIT_LOCAL2018" },
                { SupportedRevitVersion.Revit2019, @"C:\REVIT_LOCAL2019" },
                { SupportedRevitVersion.Revit2020, @"C:\REVIT_LOCAL2020" }
            };

        public static string GetRevitExecutableFolderPath(SupportedRevitVersion revitVersion)
        {
            string revitExecutableFolderPath = null;

            if (REVIT_EXECUTABLE_FOLDER_PATHS.ContainsKey(revitVersion))
            {
                revitExecutableFolderPath =
                    REVIT_EXECUTABLE_FOLDER_PATHS[revitVersion]
                    .FirstOrDefault(folderPath => File.Exists(Path.Combine(folderPath, REVIT_EXECUTABLE_FILE_NAME)));
            }

            return revitExecutableFolderPath;
        }

        public static string GetRevitExecutableFilePath(SupportedRevitVersion revitVersion)
        {
            var folderPath = GetRevitExecutableFolderPath(revitVersion);

            return (folderPath != null) ? Path.Combine(folderPath, REVIT_EXECUTABLE_FILE_NAME) : null;
        }

        public static string GetRevitLocalFolderPath(SupportedRevitVersion revitVersion)
        {
            return REVIT_LOCAL_FOLDER_PATHS.ContainsKey(revitVersion) ?
                REVIT_LOCAL_FOLDER_PATHS[revitVersion] : null;
        }

        public static string GetRevitLocalFilePath(SupportedRevitVersion revitVersion, string centralFilePath)
        {
            string localFilePath = null;

            var localFolderPath = GetRevitLocalFolderPath(revitVersion);

            if (localFolderPath != null)
            {
                var localFileName = Path.GetFileNameWithoutExtension(centralFilePath) + "_" + Environment.UserName + Path.GetExtension(centralFilePath);

                localFilePath = Path.Combine(localFolderPath, localFileName);
            }

            return localFilePath;
        }

        public static bool IsRevitVersionInstalled(SupportedRevitVersion revitVersion)
        {
            return File.Exists(GetRevitExecutableFilePath(revitVersion));
        }

        public static SupportedRevitVersion GetMinimumInstalledRevitVersion()
        {
            return GetInstalledRevitVersions().OrderBy(supportedRevitVersion => supportedRevitVersion).FirstOrDefault();
        }

        public static List<SupportedRevitVersion> GetInstalledRevitVersions()
        {
            return REVIT_EXECUTABLE_FOLDER_PATHS.Keys
                .Where(IsRevitVersionInstalled)
                .Where(BatchRvt.IsBatchRvtAddinInstalled)
                .ToList();
        }

        public static string GetRevitVersionText(SupportedRevitVersion revitVersion)
        {
            return SUPPORTED_REVIT_VERSION_NUMBERS.ContainsKey(revitVersion) ?
                SUPPORTED_REVIT_VERSION_NUMBERS[revitVersion] : "UNSUPPORTED";
        }

        public static bool IsSupportedRevitVersionNumber(string revitVersionNumber)
        {
            return SUPPORTED_REVIT_VERSION_NUMBERS.ContainsValue(revitVersionNumber);
        }

        public static SupportedRevitVersion GetSupportedRevitVersion(string revitVersionNumber)
        {
            return SUPPORTED_REVIT_VERSION_NUMBERS.Single(keyValue => keyValue.Value == revitVersionNumber).Key;
        }

        public static string GetRevitAddinsFolderPath(SupportedRevitVersion revitVersion, Environment.SpecialFolder specialFolder)
        {
            return Path.Combine(Environment.GetFolderPath(specialFolder), REVIT_ADDINS_RELATIVE_PATHS[revitVersion]);
        }
    }
}
