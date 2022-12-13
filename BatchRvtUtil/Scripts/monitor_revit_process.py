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

import global_test_mode
import monitor_process

MONITOR_INTERVAL_IN_SECONDS = 0.25
UNRESPONSIVE_THRESHHOLD_IN_SECONDS = 10

REVIT_BUSY_HANDLER_PREFIX = "[ REVIT BUSY MONITOR ]"


def OnBeginUnresponsive(output):
    output()
    output("Revit process appears to be busy or unresponsive...")
    output()
    return


def OnEndUnresponsive(unresponsiveTimeInSeconds, output):
    output()
    output(
        "Revit process appeared busy or unresponsive for about " +
        unresponsiveTimeInSeconds.ToString() +
        " seconds."
    )
    output()
    return


def MonitorHostRevitProcess(hostRevitProcess, monitoringAction, output):
    output()
    output("Monitoring host Revit process (PID: " + str(hostRevitProcess.Id) + ")")
    output()

    busyOutput = global_test_mode.PrefixedOutputForGlobalTestMode(output, REVIT_BUSY_HANDLER_PREFIX)

    monitor_process.MonitorProcess(
        hostRevitProcess,
        monitoringAction,
        MONITOR_INTERVAL_IN_SECONDS,
        UNRESPONSIVE_THRESHHOLD_IN_SECONDS,
        lambda: OnBeginUnresponsive(busyOutput),
        lambda unresponsiveTimeInSeconds: OnEndUnresponsive(unresponsiveTimeInSeconds, busyOutput)
    )

    output()
    output("Revit process (PID: " + str(hostRevitProcess.Id) + ") has exited!")

    # TODO: do something with last pending read line task if it exists?

    return
