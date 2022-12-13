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

import sys

import logging_util
import time_util

SHOW_OUTPUT = True

ORIGINAL_STDOUT = sys.stdout
ORIGINAL_STDERR = sys.stderr


def RedirectScriptOutput(output):
    sys.stdout.flush()
    sys.stderr.flush()
    sys.stdout = output
    sys.stderr = output
    return


def RestoreScriptOutput():
    sys.stderr.flush()
    sys.stdout.flush()
    sys.stderr = ORIGINAL_STDOUT
    sys.stdout = ORIGINAL_STDERR
    return


def Output(m="", msgId=""):
    timestamp = time_util.GetDateTimeNow().ToString("HH:mm:ss")
    message = timestamp + " : " + (("[" + str(msgId) + "]" + " ") if msgId != "" else "") + m + "\n"
    if SHOW_OUTPUT:
        ORIGINAL_STDOUT.write(message)
    if logging_util.LOG_FILE[0] is not None:
        logging_util.LOG_FILE[0].WriteMessage({"msgId": msgId, "message": m})
    return
