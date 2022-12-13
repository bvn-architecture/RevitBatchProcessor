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

from System.IO import StringReader


def Try(f):
    result = None
    try:
        result = f()
    except:
        result = None
    return result


def FindAllIndicesOf(text, value):
    indices = []
    currentIndex = 0
    index = text.IndexOf(value)
    while index != -1:
        indices.append(index)
        index = text.IndexOf(value, index + 1)
    return indices


def ReadLinesFromText(text):
    lines = []
    with StringReader(text) as reader:
        line = reader.ReadLine()
        while line is not None:
            lines.append(line)
            line = reader.ReadLine()
    return lines
