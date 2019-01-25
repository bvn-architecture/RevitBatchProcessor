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
