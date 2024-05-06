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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil;

public class LogFile
{
    private const string DateFormat = "dd/MM/yyyy";
    private const string TimeFormat = "HH:mm:ss";
    private static readonly string SessionId = Guid.NewGuid().ToString();
    private readonly string logFileName_;
    private readonly string logFilePath_;

    private readonly string logFolderPath_;
    private readonly string logName_;

    private StreamWriter appendTextStreamWriter_;

    public LogFile(string logName, string logFolderPath, bool includeUsernamePrefix = true)
    {
        logFolderPath_ = logFolderPath;
        logName_ = logName;

        var logFilenamePrefix = includeUsernamePrefix ? Environment.UserName : string.Empty;
        var separator = logFilenamePrefix != string.Empty ? "_" : string.Empty;

        logFileName_ = logFilenamePrefix + separator + logName + ".log";

        logFilePath_ = Path.Combine(
            logFolderPath_,
            logFileName_
        );
    }

    private void Open()
    {
        Close();

        try
        {
            appendTextStreamWriter_ = new FileInfo(logFilePath_).AppendText();
        }
        catch (Exception)
        {
            appendTextStreamWriter_ = null;
        }
    }

    private static string GetSerializedLogEntry(DateTime dateTime, string sessionId, object message)
    {
        return SerializeAsJson(
            GetLogObject(dateTime, sessionId, message)
        );
    }

    public string GetLogFilePath()
    {
        return logFilePath_;
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
            sessionId,
            message
        };

        return logEntry;
    }

    private static string SerializeAsJson(object logObject)
    {
        return JObject.FromObject(logObject).ToString(Formatting.None);
    }

    private bool WriteMessage(string sessionId, object message)
    {
        var success = false;

        var useExistingOpenStream = appendTextStreamWriter_ != null;

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

            if (!useExistingOpenStream) Open();

            if (appendTextStreamWriter_ != null)
            {
                appendTextStreamWriter_.WriteLine(logEntry);
                appendTextStreamWriter_.Flush();
            }

            success = true;
        }
        catch (Exception)
        {
            success = false;
        }

        if (!useExistingOpenStream) Close();

        return success;
    }

    public bool WriteMessage(object message)
    {
        return WriteMessage(GetSessionId(), message);
    }

    private void Close()
    {
        if (appendTextStreamWriter_ == null) return;
        try
        {
            appendTextStreamWriter_.Close();
        }
        catch (Exception)
        {
            // ignored
        }

        appendTextStreamWriter_ = null;
    }

    private static string GetSessionId()
    {
        return SessionId;
    }

    private static string ReadLineAsPlainText(string logLine, bool useUniversalTime)
    {
        var plainTextLine = logLine;

        var jobject = JsonUtil.DeserializeFromJson(logLine);

        if (jobject == null) return plainTextLine;
        var dateString = jobject["date"][useUniversalTime ? "utc" : "local"];
        var timeString = jobject["time"][useUniversalTime ? "utc" : "local"];
        var message = jobject["message"]["message"];

        plainTextLine = dateString + " " + timeString + " : " + message;

        return plainTextLine;
    }

    public static IEnumerable<string> ReadLinesAsPlainText(string logFilePath, bool useUniversalTime = false)
    {
        try
        {
            return File.ReadAllLines(logFilePath)
                .Select(line => ReadLineAsPlainText(line, useUniversalTime))
                .ToList();
        }
        catch (Exception e)
        {
            return null;
        }


    }
}