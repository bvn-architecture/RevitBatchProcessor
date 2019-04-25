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

namespace BatchRvtUtil
{
    public static class CommandSettings
    {
        public const string SETTINGS_FILE_PATH_OPTION = "settings_file";
        public const string LOG_FOLDER_PATH_OPTION = "log_folder";
        public const string SESSION_ID_OPTION = "session_id";
        public const string TASK_DATA_OPTION = "task_data";
        public const string TEST_MODE_FOLDER_PATH_OPTION = "test_mode_folder_path";
        public const string REVIT_FILE_LIST_OPTION = "file_list";
        public const string REVIT_VERSION_OPTION = "revit_version";
        public const string TASK_SCRIPT_FILE_PATH_OPTION = "task_script";
        public const string DETACH_OPTION = "detach";
        public const string CREATE_NEW_LOCAL_OPTION = "create_new_local";
        public const string WORKSETS_OPTION = "worksets";
        public const string CLOSE_ALL_WORKSETS_OPTION_VALUE = "close_all";
        public const string OPEN_ALL_WORKSETS_OPTION_VALUE = "open_all";
        public const string OPEN_LAST_VIEWED_WORKSETS_OPTION_VALUE = "last_viewed";
        public const string AUDIT_ON_OPENING_OPTION = "audit";
        public const string PER_FILE_PROCESSING_TIMEOUT_OPTION = "per_file_timeout";
        public const string HELP_OPTION = "help";
        public static readonly string[] ALL_VALID_OPTONS = new [] {
                SETTINGS_FILE_PATH_OPTION,
                LOG_FOLDER_PATH_OPTION,
                SESSION_ID_OPTION,
                TASK_DATA_OPTION,
                TEST_MODE_FOLDER_PATH_OPTION,
                REVIT_FILE_LIST_OPTION,
                REVIT_VERSION_OPTION,
                TASK_SCRIPT_FILE_PATH_OPTION,
                DETACH_OPTION,
                CREATE_NEW_LOCAL_OPTION,
                WORKSETS_OPTION,
                AUDIT_ON_OPENING_OPTION,
                PER_FILE_PROCESSING_TIMEOUT_OPTION,
                HELP_OPTION
            };

        private static readonly Dictionary<string, Func<string, object>> OPTION_PARSERS =
            new Dictionary<string, Func<string, object>>() {
                { SETTINGS_FILE_PATH_OPTION, ParseExistingFilePathOptionValue },
                { LOG_FOLDER_PATH_OPTION, ParseExistingFolderPathOptionValue },
                { SESSION_ID_OPTION, ParseTextOptionValue },
                { TASK_DATA_OPTION, ParseTextOptionValue },
                { TEST_MODE_FOLDER_PATH_OPTION, ParseTextOptionValue },
                { REVIT_VERSION_OPTION, optionValue => ParseRevitVersionOptionValue(optionValue) },
                { REVIT_FILE_LIST_OPTION, ParseExistingFilePathOptionValue },
                { TASK_SCRIPT_FILE_PATH_OPTION, ParseExistingFilePathOptionValue },
                { DETACH_OPTION, null },
                { CREATE_NEW_LOCAL_OPTION, null },
                { WORKSETS_OPTION, optionValue => ParseWorksetsOptionValue(optionValue) },
                { AUDIT_ON_OPENING_OPTION, null },
                { PER_FILE_PROCESSING_TIMEOUT_OPTION, optionValue => ParsePositiveIntegerOptionValue(optionValue) },
                { HELP_OPTION, null }
            };

        public static string ParseTextOptionValue(string textOptionValue)
        {
            string parsedValue = null;
            
            if (!string.IsNullOrWhiteSpace(textOptionValue))
            {
                parsedValue = textOptionValue.Trim();
            }

            return parsedValue;
        }

        public static int? ParsePositiveIntegerOptionValue(string integerOptionValue)
        {
            int parsedValue;

            bool parsed = int.TryParse(integerOptionValue, out parsedValue);

            return (parsed && parsedValue > 0) ? parsedValue : (int?)null;
        }

        public static string ParseExistingFilePathOptionValue(string filePathOptionValue)
        {
            string parsedValue = null;

            if (!string.IsNullOrWhiteSpace(filePathOptionValue))
            {
                var fullFilePath = PathUtil.GetFullPath(filePathOptionValue);
                
                if (PathUtil.FileExists(fullFilePath))
                {
                    parsedValue = fullFilePath;
                }
            }

            return parsedValue;
        }

        public static string ParseExistingFolderPathOptionValue(string folderPathOptionValue)
        {
            string parsedValue = null;

            if (!string.IsNullOrWhiteSpace(folderPathOptionValue))
            {
                var fullFolderPath = PathUtil.GetFullPath(folderPathOptionValue);

                if (PathUtil.DirectoryExists(fullFolderPath))
                {
                    parsedValue = fullFolderPath;
                }
            }
            return parsedValue;
        }

        public static bool? ParseBooleanOptionValue(string booleanOptionValue)
        {
            bool? parsedValue = null;

            if (!string.IsNullOrWhiteSpace(booleanOptionValue))
            {
                if (new[] { "TRUE", "YES" }.Any(s => s.ToUpper() == booleanOptionValue.ToUpper()))
                {
                    parsedValue = true;
                }
                else if (new[] { "FALSE", "NO" }.Any(s => s.ToUpper() == booleanOptionValue.ToUpper()))
                {
                    parsedValue = false;
                }
            }
            
            return parsedValue;
        }

        public static RevitVersion.SupportedRevitVersion? ParseRevitVersionOptionValue(string revitVersionOptionValue)
        {
            RevitVersion.SupportedRevitVersion? revitVersion = null;

            if (!string.IsNullOrWhiteSpace(revitVersionOptionValue))
            {
                if (RevitVersion.IsSupportedRevitVersionNumber(revitVersionOptionValue))
                {
                    revitVersion = RevitVersion.GetSupportedRevitVersion(revitVersionOptionValue);
                }
            }

            return revitVersion;
        }

        public static BatchRvt.WorksetConfigurationOption? ParseWorksetsOptionValue(string worksetsOptionValue)
        {
            BatchRvt.WorksetConfigurationOption? worksetsOption = null;

            var parsedTextOptionValue = ParseTextOptionValue(worksetsOptionValue);

            if (parsedTextOptionValue == OPEN_ALL_WORKSETS_OPTION_VALUE)
            {
                worksetsOption = BatchRvt.WorksetConfigurationOption.OpenAllWorksets;
            }
            else if (parsedTextOptionValue == CLOSE_ALL_WORKSETS_OPTION_VALUE)
            {
                worksetsOption = BatchRvt.WorksetConfigurationOption.CloseAllWorksets;
            }
            else if (parsedTextOptionValue == OPEN_LAST_VIEWED_WORKSETS_OPTION_VALUE)
            {
                worksetsOption = BatchRvt.WorksetConfigurationOption.OpenLastViewed;
            }

            return worksetsOption;
        }

        public static Dictionary<string, object> GetCommandLineOptions()
        {
            return OPTION_PARSERS
                .Select(
                    kv => new { Option = kv.Key, Parser = kv.Value }
                ).ToDictionary(
                    optionAndValue => optionAndValue.Option,
                    optionAndValue => (
                        (optionAndValue.Parser != null) ?
                        optionAndValue.Parser(CommandLineUtil.GetCommandLineOption(optionAndValue.Option, expectOptionValue: true)) :
                        CommandLineUtil.HasCommandLineOption(optionAndValue.Option, expectOptionValue: false)
                    )
                );
        }

        public static IEnumerable<string> GetInvalidOptions()
        {
            return CommandLineUtil.GetAllCommandLineOptionSwitches()
                .Where(optionSwitch => !ALL_VALID_OPTONS.Contains(optionSwitch))
                .ToList();
        }

        public class Data
        {
            // Command line options
            public string SettingsFilePath = null;
            public string LogFolderPath = null;
            public string SessionId = null;
            public string TaskData = null;
            public string TestModeFolderPath = null;

            // Programmatic options
            public BatchRvtSettings Settings = null;
            public IEnumerable<string> RevitFileList = null;

            // BatchRvt itself will set this to the path of the generated log file.
            public string GeneratedLogFilePath = null;
        }
    }
}
