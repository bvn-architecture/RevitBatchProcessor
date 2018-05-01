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

namespace BatchRvtUtil
{
    public static class ExcelUtil
    {
        public static bool IsExcelInstalled()
        {
            return Type.GetTypeFromProgID("Excel.Application") != null;
        }

        public static bool HasExcelExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();

            return new[] { ".xls", ".xlsx" }.Any(excelExtension => extension == excelExtension.ToLower());
        }
    }
}
