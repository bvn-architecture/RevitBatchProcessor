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

# NOTE: must not add any references to Revit API modules here because this module
# is allowed to run outside of a Revit application.

def GetRevitVersionNumber(uiapp):
    return uiapp.Application.VersionNumber

def GetSessionUIApplication():
    uiapp = None
    try:
        uiapp = __revit__
    except NameError, e:
        pass
    return uiapp

def GetSessionRevitVersionNumber():
    uiapp = GetSessionUIApplication()
    revitVersionNumber = GetRevitVersionNumber(uiapp) if uiapp is not None else None
    return revitVersionNumber
