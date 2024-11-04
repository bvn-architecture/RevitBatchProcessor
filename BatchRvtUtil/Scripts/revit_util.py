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

clr.AddReference("RevitAPI")
from Autodesk.Revit.DB import *


def PurgeReleasedAPIObjects(app):
    app.PurgeReleasedAPIObjects()
    return


def InTransaction(transaction, action):
    result = None
    transaction.Start()
    try:
        result = action()
    except Exception as e:
        transaction.RollBack()
        transaction.Dispose()
        raise
    transaction.Commit()
    transaction.Dispose()
    return result
