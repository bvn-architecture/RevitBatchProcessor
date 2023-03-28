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
using System.Threading.Tasks;

namespace BatchRvtUtil;

public static class StreamIOUtil
{
    public static Tuple<List<string>, Task<string>> ReadAvailableLines(StreamReader streamReader,
        Task<string> pendingReadLineTask = null)
    {
        Task<string> nextPendingReadLineTask;
        var lines = new List<string>();

        Task<string> readLineTask = null;

        readLineTask = pendingReadLineTask ?? streamReader.ReadLineAsync();

        var reachedEndOfStream = false;

        while (readLineTask.Status == TaskStatus.RanToCompletion && !reachedEndOfStream)
        {
            var line = readLineTask.Result;
            if (line != null)
            {
                lines.Add(line);
                readLineTask = streamReader.ReadLineAsync();
            }
            else
            {
                reachedEndOfStream = true;
            }
        }

        if (reachedEndOfStream)
            nextPendingReadLineTask = null;
        else
            switch (readLineTask.Status)
            {
                case TaskStatus.Faulted:
                case TaskStatus.Canceled:
                    nextPendingReadLineTask = null;
                    break;
                case TaskStatus.Created:
                case TaskStatus.WaitingForActivation:
                case TaskStatus.WaitingToRun:
                case TaskStatus.Running:
                case TaskStatus.WaitingForChildrenToComplete:
                case TaskStatus.RanToCompletion:
                default:
                    nextPendingReadLineTask = readLineTask;
                    break;
            }

        return Tuple.Create(lines, nextPendingReadLineTask);
    }
}