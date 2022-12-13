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

clr.AddReference("System.Windows.Forms")
import System.Windows.Forms as WinForms

import thread_util
import time_util


def IsProcessResponding(process):
    isResponding = False
    try:
        isResponding = process.Responding
    except Exception, e:
        pass
    return isResponding


def MonitorProcess(
        process,
        monitoringAction,
        monitorIntervalInSeconds,
        unresponsiveThreshholdInSeconds,
        onBeginUnresponsive,
        onEndUnresponsive
):
    wasResponding = True
    isResponding = True
    unresponsiveStartTime = None
    haveNotifiedBeginUnresponsive = False

    process.Refresh()

    while not process.HasExited:

        wasResponding = isResponding
        isResponding = IsProcessResponding(process)

        if wasResponding and not isResponding:  # responsive -> unresponsive
            unresponsiveStartTime = time_util.GetDateTimeNow()
            haveNotifiedBeginUnresponsive = False

        elif isResponding and not wasResponding:  # unresponsive -> responsive
            if haveNotifiedBeginUnresponsive:  # notify end of unresponsiveness
                onEndUnresponsive(time_util.GetSecondsElapsedSince(unresponsiveStartTime))
                haveNotifiedBeginUnresponsive = False

        elif not isResponding and not wasResponding:  # continuing unresponsiveness
            if not haveNotifiedBeginUnresponsive:  # notify unresponsiveness beyond threshold
                if time_util.GetSecondsElapsedSince(unresponsiveStartTime) >= unresponsiveThreshholdInSeconds:
                    onBeginUnresponsive()
                    haveNotifiedBeginUnresponsive = True

        thread_util.SleepForSeconds(monitorIntervalInSeconds)

        WinForms.Application.DoEvents()

        monitoringAction()

        process.Refresh()

    # Was notified of beginning of unresponsiveness, therefore need to notify end of unresponsiveness.
    if haveNotifiedBeginUnresponsive:
        onEndUnresponsive(time_util.GetSecondsElapsedSince(unresponsiveStartTime))
        haveNotifiedBeginUnresponsive = False

    return
