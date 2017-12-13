#
# Revit Batch Processor
#
# Copyright (c) 2017  Dan Rumery, BVN
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

import clr
import System

from System.Reflection import Missing
from System.Runtime.InteropServices import COMException, Marshal

clr.AddReference("Microsoft.Office.Interop.Excel")
import Microsoft.Office.Interop.Excel as Excel

MissingValue = System.Reflection.Missing.Value

xlUnicodeText = 42
xlNoChange = 1


def WriteRowsToWorksheet(worksheet, rows):
  for rowIndex, row in enumerate(rows):
    excelRow = worksheet.Rows[rowIndex + 1]
    excelRow.NumberFormat = "@" # Set type to 'Text'
    for cellIndex, cellValue in enumerate(row):
      excelRow.Cells[cellIndex + 1].Value2 = cellValue
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

def WithExcelApp(excelAppAction):
  result = None
  app = Excel.ApplicationClass()
  app.Visible = False # Set this to False so Excel isn't visible to the user.
  app.DisplayAlerts = False
  app.ScreenUpdating = False
  app.AskToUpdateLinks = False # Suppress prompt to update data links.
  try:
    result = excelAppAction(app)
  finally:
    app.DisplayAlerts = True
    app.ScreenUpdating = True
    app.Quit()
  return result

def WithExcelWorkbook(excelFilePath, workbookAction, saveChanges=False):
  result = None
  def excelAppAction(app):
    result = None
    workbook = None
    try:
      workbooks = app.Workbooks
      workbook = workbooks.Open(excelFilePath)
      result = workbookAction(workbook)
    finally:
      if workbook is not None:
        workbook.Close(saveChanges)
    return result
  result = WithExcelApp(excelAppAction)
  return result

def WithNewExcelWorkbook(workbookAction, saveChanges=False):
  result = None
  def excelAppAction(app):
    result = None
    workbook = None
    try:
      workbooks = app.Workbooks
      workbook = workbooks.Add()
      result = workbookAction(workbook)
    finally:
      if workbook is not None:
        workbook.Close(saveChanges)
    return result
  result = WithExcelApp(excelAppAction)
  return result

