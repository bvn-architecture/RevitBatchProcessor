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

from System.Diagnostics import Process, ProcessStartInfo

from batch_rvt_util import RevitVersion


def StartRevitProcess(revitVersion, initEnvironmentVariables):
    revitExecutableFilePath = RevitVersion.GetRevitExecutableFilePath(revitVersion)
    psi = ProcessStartInfo(revitExecutableFilePath)
    psi.UseShellExecute = False
    psi.RedirectStandardError = True
    psi.RedirectStandardOutput = True
    psi.WorkingDirectory = RevitVersion.GetRevitExecutableFolderPath(revitVersion)
    initEnvironmentVariables(psi.EnvironmentVariables)
    revitProcess = Process.Start(psi)
    return revitProcess
