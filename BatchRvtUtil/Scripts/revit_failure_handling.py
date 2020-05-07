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

import clr
import System

clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

from System import EventHandler

clr.AddReference("RevitAPI")
from Autodesk.Revit.DB import *
from Autodesk.Revit.DB.Events import FailuresProcessingEventArgs

import global_test_mode
import exception_util

REVIT_WARNINGS_MESSAGE_HANDLER_PREFIX = "[ REVIT WARNINGS HANDLER ]"

def ElementIdsToSemicolonDelimitedText(elementIds):
  return str.Join("; ", [str(elementId.IntegerValue) for elementId in elementIds])

def ReportFailureWarning(failure, failureDefinition, output):
  failureSeverity = failure.GetSeverity()
  # TODO: more thorough reporting?
  output()
  output(
      "\t" +
      str(failureSeverity) +
      " - " +
      str(failure.GetDescriptionText()) +
      " - " +
      "(" + "GUID: " + str(failure.GetFailureDefinitionId().Guid) + ")"
    )

  if failureSeverity == FailureSeverity.Error or failureSeverity == FailureSeverity.Warning:
    failingElementIds = failure.GetFailingElementIds()
    if len(failingElementIds) > 0:
      output()
      output("\t" + "Failing element ids: " + ElementIdsToSemicolonDelimitedText(failingElementIds))
    additionalElementIds = failure.GetAdditionalElementIds()
    if len(additionalElementIds) > 0:
      output()
      output("\t" + "Additional element ids: " + ElementIdsToSemicolonDelimitedText(additionalElementIds))

  if failureSeverity == FailureSeverity.Error:
    if failure.HasResolutions():
      output()
      output("\t" + "Applicable resolution types:")
      output()
      defaultResolutionType = failureDefinition.GetDefaultResolutionType()
      for resolutionType in failureDefinition.GetApplicableResolutionTypes():
        output(
            "\t\t" +
            str(resolutionType) +
            (" (Default)" if (resolutionType == defaultResolutionType) else str.Empty) +
            " - " +
            "'" + failureDefinition.GetResolutionCaption(resolutionType) + "'"
          )
    else:
      output()
      output("\t" + "WARNING: no resolutions available")

  return

def ProcessFailures(failuresAccessor, output, rollBackOnWarning=False):
  try:
    result = FailureProcessingResult.Continue
    doc = failuresAccessor.GetDocument()
    app = doc.Application
    failureReg = app.GetFailureDefinitionRegistry()
    failures = failuresAccessor.GetFailureMessages()
    if failures.Any():
      output()
      output("Processing Revit document warnings / failures (" + str(failures.Count) + "):")
      for failure in failures:
        failureDefinition = failureReg.FindFailureDefinition(failure.GetFailureDefinitionId())
        ReportFailureWarning(failure, failureDefinition, output)
        failureSeverity = failure.GetSeverity()
        if failureSeverity == FailureSeverity.Warning and not rollBackOnWarning:
          failuresAccessor.DeleteWarning(failure)
        elif (
            failureSeverity == FailureSeverity.Error
            and
            failure.HasResolutions()
            and
            result != FailureProcessingResult.ProceedWithRollBack
            and
            not rollBackOnWarning
          ):
          # If Unlock Constraints is a valid resolution type for the current failure, use it.
          if failure.HasResolutionOfType(FailureResolutionType.UnlockConstraints):
            failure.SetCurrentResolutionType(FailureResolutionType.UnlockConstraints)
          elif failureDefinition.IsResolutionApplicable(FailureResolutionType.UnlockConstraints):
            output()
            output("\t" + "WARNING: UnlockConstraints is not a valid resolution for this failure despite the definition reporting that it is an applicable resolution!")
          output()
          output("\t" + "Attempting to resolve error using resolution: " + str(failure.GetCurrentResolutionType()))
          failuresAccessor.ResolveFailure(failure)
          result = FailureProcessingResult.ProceedWithCommit
        else:
          result = FailureProcessingResult.ProceedWithRollBack
    else:
      result = FailureProcessingResult.Continue
  except Exception, e:
    output()
    output("ERROR: the failure handler generated an error!")
    exception_util.LogOutputErrorDetails(e, output)
    result = FailureProcessingResult.Continue
  return result

class FailuresPreprocessor(IFailuresPreprocessor):
  def __init__(self, output):
    self.output = output
    return

  def PreprocessFailures(self, failuresAccessor):
    result = ProcessFailures(failuresAccessor, self.output)
    return result

def SetTransactionFailureOptions(transaction, output):
  failureOptions = transaction.GetFailureHandlingOptions()
  failureOptions.SetForcedModalHandling(True)
  failureOptions.SetClearAfterRollback(True)
  failureOptions.SetFailuresPreprocessor(FailuresPreprocessor(output))
  transaction.SetFailureHandlingOptions(failureOptions)
  return

def SetFailuresAccessorFailureOptions(failuresAccessor):
  failureOptions = failuresAccessor.GetFailureHandlingOptions()
  failureOptions.SetForcedModalHandling(True)
  failureOptions.SetClearAfterRollback(True)
  failuresAccessor.SetFailureHandlingOptions(failureOptions)
  return

def FailuresProcessingEventHandler(sender, args, output):
  app = sender
  failuresAccessor = args.GetFailuresAccessor()
  SetFailuresAccessorFailureOptions(failuresAccessor)
  result = ProcessFailures(failuresAccessor, output)
  args.SetProcessingResult(result)
  return

def WithFailuresProcessingHandler(app, action, output_):
  output = global_test_mode.PrefixedOutputForGlobalTestMode(output_, REVIT_WARNINGS_MESSAGE_HANDLER_PREFIX)
  result = None
  failuresProcessingEventHandler = (
      EventHandler[FailuresProcessingEventArgs](
          lambda sender, args: FailuresProcessingEventHandler(sender, args, output)
        )
    )
  app.FailuresProcessing += failuresProcessingEventHandler
  try:
    result = action()
  finally:
    app.FailuresProcessing -= failuresProcessingEventHandler
  return result

