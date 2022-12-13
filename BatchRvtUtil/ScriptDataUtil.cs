//
// Revit Batch Processor
//
// Copyright (c) 2020  Daniel Rumery, BVN
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

using Newtonsoft.Json.Linq;

namespace BatchRvtUtil;

public static class ScriptDataUtil
{
    private const string SCRIPT_DATA_FILENAME_PREFIX = "Session.ScriptData.";
    private const string SESSION_PROGRESS_RECORD_PREFIX = "Session.ProgressRecord.";
    private const string JSON_FILE_EXTENSION = ".json";

    public class ScriptData : IPersistent
    {
        private readonly BooleanSetting _auditOnOpening = new("auditOnOpening");
        private readonly ListSetting<string> _associatedData = new("associatedData");

        private readonly EnumSetting<BatchRvt.CentralFileOpenOption>
            _centralFileOpenOption = new("centralFileOpenOption");

        private readonly StringSetting _cloudModelId = new("cloudModelId");
        private readonly StringSetting _cloudProjectId = new("cloudProjectId");
        private readonly StringSetting _dataExportFolderPath = new("dataExportFolderPath");
        private readonly BooleanSetting _deleteLocalAfter = new("deleteLocalAfter");
        private readonly BooleanSetting _discardWorksetsOnDetach = new("discardWorksetsOnDetach");
        private readonly BooleanSetting _enableDataExport = new("enableDataExport");
        private readonly BooleanSetting _isCloudModel = new("isCloudModel");
        private readonly BooleanSetting _openInUi = new("openInUI");
        private readonly PersistentSettings _persistentSettings;
        private readonly IntegerSetting _progressMax = new("progressMax");
        private readonly IntegerSetting _progressNumber = new("progressNumber");
        private readonly StringSetting _revitFilePath = new("revitFilePath");

        private readonly EnumSetting<BatchRvt.RevitProcessingOption>
            _revitProcessingOption = new("revitProcessingOption");

        private readonly StringSetting _sessionDataFolderPath = new("sessionDataFolderPath");

        private readonly StringSetting _sessionId = new("sessionId");
        private readonly BooleanSetting _showMessageBoxOnTaskScriptError = new("showMessageBoxOnTaskError");
        private readonly StringSetting _taskData = new("taskData");
        private readonly StringSetting _taskScriptFilePath = new("taskScriptFilePath");

        private readonly EnumSetting<BatchRvt.WorksetConfigurationOption> _worksetConfigurationOption =
            new("worksetConfigurationOption");


        public ScriptData()
        {
            _persistentSettings = new PersistentSettings(
                new IPersistent[]
                {
                    _sessionId,
                    _revitFilePath,
                    _isCloudModel,
                    _cloudProjectId,
                    _cloudModelId,
                    _enableDataExport,
                    _taskScriptFilePath,
                    _taskData,
                    _sessionDataFolderPath,
                    _dataExportFolderPath,
                    _showMessageBoxOnTaskScriptError,
                    _revitProcessingOption,
                    _centralFileOpenOption,
                    _deleteLocalAfter,
                    _discardWorksetsOnDetach,
                    _worksetConfigurationOption,
                    _openInUi,
                    _auditOnOpening,
                    _progressNumber,
                    _progressMax,
                    _associatedData
                }
            );
        }

        public void Load(JObject jobject)
        {
            _persistentSettings.Load(jobject);
        }

        public void Store(JObject jobject)
        {
            _persistentSettings.Store(jobject);
        }
    }
}