#
# Revit Batch Processor
#
# Copyright (c) 2020  Dan Rumery, BVN
#
# This program is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program.  If not, see <http://www.gnu.org/licenses/>.
#
#

import System
import clr
from System.Runtime.InteropServices import Marshal

clr.AddReference("Microsoft.Office.Interop.Excel")
import Microsoft.Office.Interop.Excel as Excel

MissingValue = System.Reflection.Missing.Value

xlUnicodeText = 42
xlNoChange = 1


def FindFirstValue(range, searchOrder, searchDirection):
    afterCell = range.Cells[
        range.Rows.Count if (searchDirection == Excel.XlSearchDirection.xlNext) else 1,
        range.Columns.Count if (searchDirection == Excel.XlSearchDirection.xlNext) else 1
    ]
    return range.Find(
        "*",
        afterCell,
        Excel.XlFindLookIn.xlValues,
        MissingValue,
        searchOrder,
        searchDirection,
        MissingValue,
        MissingValue,
        MissingValue
    )


# Returns the row number of the first non-blank row in the range.
# If all rows are blank, returns 0.
def GetFirstUsedRowNumber(range):
    firstUsedCell = FindFirstValue(range, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext)
    return firstUsedCell.Row if (firstUsedCell is not None) else 0


# Returns the column number of the first non-blank column in the range.
# If all columns are blank, returns 0.
def GetFirstUsedColumnNumber(range):
    firstUsedCell = FindFirstValue(range, Excel.XlSearchOrder.xlByColumns, Excel.XlSearchDirection.xlNext)
    return firstUsedCell.Column if (firstUsedCell is not None) else 0


# Returns the row number of the last non-blank row in the range.
# If all rows are blank, returns 0.
def GetLastUsedRowNumber(range):
    lastUsedCell = FindFirstValue(range, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious)
    return lastUsedCell.Row if (lastUsedCell is not None) else 0


# Returns the column number of the last non-blank column in the range.
# If all columns are blank, returns 0.
def GetLastUsedColumnNumber(range):
    lastUsedCell = FindFirstValue(range, Excel.XlSearchOrder.xlByColumns, Excel.XlSearchDirection.xlPrevious)
    return lastUsedCell.Column if (lastUsedCell is not None) else 0


def WriteRowsToWorksheet(worksheet, rows):
    for rowIndex, row in enumerate(rows):
        excelRows = worksheet.Rows
        excelRow = excelRows[rowIndex + 1]
        excelRow.NumberFormat = "@"  # Set type to 'Text'
        for cellIndex, cellValue in enumerate(row):
            cells = excelRow.Cells
            cell = cells[cellIndex + 1]
            cell.Value2 = cellValue
            Marshal.FinalReleaseComObject(cell)
            Marshal.FinalReleaseComObject(cells)
        Marshal.FinalReleaseComObject(excelRow)
        Marshal.FinalReleaseComObject(excelRows)
    return


def ReadRowsFromWorksheet(worksheet):
    usedRange = worksheet.UsedRange
    rows = []
    for excelRow in usedRange.Rows:
        row = []
        for cellValue in excelRow.Value2:
            row.append(cellValue)
        rows.append(row)
    return rows


def GetNumberOfRowsAndColumns(worksheet):
    usedRange = worksheet.UsedRange
    return (
        usedRange.Row + usedRange.Rows.Count - 1,
        usedRange.Column + usedRange.Columns.Count - 1
    )


def GetWorksheetRange(worksheet, firstRowNumber, firstColumnNumber, lastRowNumber, lastColumnNumber):
    return worksheet.Range[
        worksheet.Cells[firstRowNumber, firstColumnNumber],
        worksheet.Cells[lastRowNumber, lastColumnNumber]
    ]


def ReadRowsText(range):
    rows = []
    for excelRow in range.Rows:
        row = []
        for excelCell in excelRow.Cells:
            row.Add(excelCell.Text)
        rows.Add(row)
    return rows


def ReadRowsTextFromWorksheet(worksheet):
    rows = []
    usedRange = worksheet.UsedRange
    lastUsedRowNumber = GetLastUsedRowNumber(usedRange)
    lastUsedColumnNumber = GetLastUsedColumnNumber(usedRange)
    if (lastUsedRowNumber != 0 and lastUsedColumnNumber != 0):
        rows = ReadRowsText(GetWorksheetRange(worksheet, 1, 1, lastUsedRowNumber, lastUsedColumnNumber))
    return rows


def WithExcelApp(excelAppAction):
    result = None
    app = Excel.ApplicationClass()
    app.Visible = False  # Set this to False so Excel isn't visible to the user.
    app.DisplayAlerts = False
    app.ScreenUpdating = False
    app.AskToUpdateLinks = False  # Suppress prompt to update data links.
    try:
        result = excelAppAction(app)
    finally:
        app.DisplayAlerts = True
        app.ScreenUpdating = True
        app.Quit()
        Marshal.FinalReleaseComObject(app)
    return result


def WithExcelWorkbook(excelFilePath, workbookAction, saveChanges=False):
    result = None

    def excelAppAction(app):
        result = None
        workbooks = None
        workbook = None
        try:
            workbooks = app.Workbooks
            workbook = workbooks.Open(excelFilePath)
            result = workbookAction(workbook)
        finally:
            if workbook is not None:
                workbook.Close(saveChanges)
                Marshal.FinalReleaseComObject(workbook)
            if workbooks is not None:
                Marshal.FinalReleaseComObject(workbooks)
        return result

    result = WithExcelApp(excelAppAction)
    return result


def WithNewExcelWorkbook(excelFilePath, workbookAction, saveChanges=False):
    result = None

    def excelAppAction(app):
        result = None
        workbooks = None
        workbook = None
        try:
            workbooks = app.Workbooks
            workbook = workbooks.Add()
            result = workbookAction(workbook)
        finally:
            if workbook is not None:
                if saveChanges:
                    workbook.SaveAs(excelFilePath)
                workbook.Close(saveChanges)
                Marshal.FinalReleaseComObject(workbook)
            if workbooks is not None:
                Marshal.FinalReleaseComObject(workbooks)
        return result

    result = WithExcelApp(excelAppAction)
    return result


def ReadRowsTextFromWorkbook(excelFilePath, worksheetName=None):
    def excelWorkbookAction(workbook):
        worksheets = workbook.Worksheets
        worksheet = worksheets[worksheetName] if worksheetName is not None else worksheets[1]
        rows = ReadRowsTextFromWorksheet(worksheet)
        Marshal.FinalReleaseComObject(worksheet)
        Marshal.FinalReleaseComObject(worksheets)
        return rows

    rows = WithExcelWorkbook(excelFilePath, excelWorkbookAction)
    return rows


def WriteRowsTextToWorkbook(excelFilePath, rows, worksheetName=None):
    def excelWorkbookAction(workbook):
        worksheets = workbook.Worksheets
        worksheet = worksheets[worksheetName] if worksheetName is not None else worksheets[1]
        WriteRowsToWorksheet(worksheet, rows)
        Marshal.FinalReleaseComObject(worksheet)
        Marshal.FinalReleaseComObject(worksheets)
        return

    WithExcelWorkbook(excelFilePath, excelWorkbookAction, saveChanges=True)
    return


def WriteRowsTextToNewWorkbook(excelFilePath, rows, worksheetName=None):
    def excelWorkbookAction(workbook):
        newWorksheetName = "Sheet1" if worksheetName is None else worksheetName
        worksheets = workbook.Worksheets
        worksheets.Add()
        worksheet = worksheets[1]
        worksheet.Name = newWorksheetName
        WriteRowsToWorksheet(worksheet, rows)
        Marshal.FinalReleaseComObject(worksheet)
        Marshal.FinalReleaseComObject(worksheets)
        return

    WithNewExcelWorkbook(excelFilePath, excelWorkbookAction, saveChanges=True)
    return
