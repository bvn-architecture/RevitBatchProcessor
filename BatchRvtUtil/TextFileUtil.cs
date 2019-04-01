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
using System.IO;
using System.Text;

namespace BatchRvtUtil
{
    public static class TextFileUtil
    {
        public const string TEXT_FILE_EXTENSION = ".txt";
        public const string TEXT_FILE_FILTER = "Text files (*.txt)|*.txt";

        public static void WriteToCSVFile(
                IEnumerable<IEnumerable<object>> rows,
                string csvFilePath,
                string delimiter,
                Encoding encoding
            )
        {
            var lines = (
                    rows
                    .Select(row => string.Join(delimiter, row.Select(value => value.ToString())))
                    .ToList()
                );

            File.WriteAllLines(csvFilePath, lines, encoding);

            return;
        }

        public static void WriteToTabDelimitedTxtFile(
                IEnumerable<IEnumerable<object>> rows,
                string txtFilePath,
                Encoding encoding = null
            )
        {
            WriteToCSVFile(rows, txtFilePath, "\t", encoding ?? Encoding.UTF8);

            return;
        }
    }
}
