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
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil
{
    public class LogFile
    {
        private static readonly string SessionId = Guid.NewGuid().ToString();

        private const string DateFormat = "dd/MM/yyyy";
        private const string TimeFormat = "HH:mm:ss";

        private readonly string logFolderPath_;
        private readonly string logFileName_;
        private readonly string logFilePath_;
        private readonly string logName_;

        private StreamWriter appendTextStreamWriter_ = null;

        public LogFile(string logName, string logFolderPath, bool includeUsernamePrefix = true)
        {
            this.logFolderPath_ = logFolderPath;
            this.logName_ = logName;

            var logFilenamePrefix = includeUsernamePrefix ? Environment.UserName : String.Empty;
            var separator = (logFilenamePrefix != String.Empty) ? "_" : String.Empty;

            this.logFileName_ = logFilenamePrefix + separator + logName + ".log";

            this.logFilePath_ = Path.Combine(
                    this.logFolderPath_,
                    this.logFileName_
                );
        }

        public void Open()
        {
            Close();

            try
            {
                appendTextStreamWriter_ = new FileInfo(this.logFilePath_).AppendText();
            }
            catch (Exception)
            {
                appendTextStreamWriter_ = null;
            }
        }

        public static string GetSerializedLogEntry(DateTime dateTime, string sessionId, object message)
        {
            return SerializeAsJson(
                    GetLogObject(dateTime, sessionId, message)
                );
        }

        public string GetLogFilePath()
        {
            return this.logFilePath_;
        }

        private static object GetLogObject(DateTime dateTime, string sessionId, object message)
        {
            var utcDateTime = dateTime.ToUniversalTime();

            var logEntry = new
            {
                date = new
                {
                    local = dateTime.ToString(DateFormat),
                    utc = utcDateTime.ToString(DateFormat)
                },
                time = new
                {
                    local = dateTime.ToString(TimeFormat),
                    utc = utcDateTime.ToString(TimeFormat)
                },
                sessionId = sessionId,
                message = message
            };

            return logEntry;
        }

        private static string SerializeAsJson(object logObject)
        {
            return JObject.FromObject(logObject).ToString(Formatting.None);
        }

        public bool WriteMessage(string sessionId, object message)
        {
            bool success = false;

            bool useExistingOpenStream = (appendTextStreamWriter_ != null);

            try
            {
                var dateTimeNow = DateTime.Now;

                string logEntry = null;

                try
                {
                    logEntry = GetSerializedLogEntry(dateTimeNow, sessionId, message);
                }
                catch (Exception e)
                {
                    var errorMessage = new
                    {
                        error = "FAILED TO PARSE LOG MESSAGE OBJECT",
                        exceptionType = e.GetType(),
                        exceptionMessage = e.Message
                    };

                    logEntry = GetSerializedLogEntry(dateTimeNow, sessionId, errorMessage);
                }

                if (!useExistingOpenStream)
                {
                    Open();
                }

                appendTextStreamWriter_.WriteLine(logEntry);
                appendTextStreamWriter_.Flush();

                success = true;
            }
            catch (Exception)
            {
                success = false;
            }

            if (!useExistingOpenStream)
            {
                Close();
            }

            return success;
        }

        public bool WriteMessage(object message)
        {
            return WriteMessage(GetSessionId(), message);
        }

        public void Close()
        {
            if (appendTextStreamWriter_ != null)
            {
                try
                {
                    appendTextStreamWriter_.Close();
                }
                catch (Exception)
                {
                }
                appendTextStreamWriter_ = null;
            }

            return;
        }

        private static string GetSessionId()
        {
            return SessionId;
        }
    }
}