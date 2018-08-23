//
// Revit Batch Processor
//
// Copyright (c) 2017  Daniel Rumery, BVN
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

        private static readonly Dictionary<string, Func<string, object>> OPTION_PARSERS =
            new Dictionary<string, Func<string, object>>() {
                { SETTINGS_FILE_PATH_OPTION, ParseExistingFilePathOptionValue },
                { LOG_FOLDER_PATH_OPTION, ParseExistingFolderPathOptionValue },
                { SESSION_ID_OPTION, ParseTextOptionValue },
                { TASK_DATA_OPTION, ParseTextOptionValue },
                { TEST_MODE_FOLDER_PATH_OPTION, ParseTextOptionValue }
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

        // TODO: figure out a way for the return type to be bool while satisfying
        // the requirement that this function be convertible to Func<string, object> in
        // the OPTION_PARSERS variable.
        public static object ParseBooleanOptionValue(string booleanOptionValue)
        {
            bool parsedValue = false;

            if (!string.IsNullOrWhiteSpace(booleanOptionValue))
            {
                parsedValue =
                    new[] { "TRUE", "YES" }
                    .Any(s => s.ToUpper() == booleanOptionValue.ToUpper()
                );
            }
            
            return parsedValue;
        }

        public static string ParseRevitVersionOptionValue(string revitVersionOptionValue)
        {
            string parsedValue = null;

            if (!string.IsNullOrWhiteSpace(revitVersionOptionValue))
            {
                if (RevitVersion.IsSupportedRevitVersionNumber(revitVersionOptionValue))
                {
                    parsedValue = revitVersionOptionValue;
                }
            }

            return parsedValue;
        }

        public static Dictionary<string, object> GetCommandLineOptions()
        {
            return OPTION_PARSERS
                .Select(
                    kv => new { Option = kv.Key, Parser = kv.Value }
                ).ToDictionary(
                    optionAndValue => optionAndValue.Option,
                    optionAndValue => optionAndValue.Parser(CommandLineUtil.GetCommandLineOption(optionAndValue.Option))
                );
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
