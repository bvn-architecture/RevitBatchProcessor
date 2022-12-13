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

clr.AddReference("System.Core")
import System.Linq

clr.ImportExtensions(System.Linq)

vs = [100, 46, 251, 158, 222, 21, 19, 117, 64, 40, 66, 181, 106, 90, 61, 24, 214, 197, 89, 209, 166, 184, 70, 144, 0,
      235, 44, 111, 135, 24, 148, 252, 200, 213, 252, 188, 254, 252, 199, 129, 121, 23, 39, 18, 88, 152, 177, 138, 144,
      217, 160, 248, 48, 192, 6, 161, 224, 105, 45, 213, 97, 174, 111, 68, 74, 43, 217, 178, 220, 68, 27, 187, 27, 103,
      40, 214, 20, 29, 115, 61, 15, 206, 56, 179, 250, 120, 65, 139, 68, 92, 234, 83, 51, 44, 163, 234, 134, 141, 44,
      233, 189, 189, 195, 229, 42, 133, 134, 47, 32, 21, 106, 35, 142, 151, 27, 14, 2, 65, 50, 7, 139, 76, 38, 188, 113,
      112, 125, 112, 56, 97, 64, 240, 39, 8, 122, 209, 167, 44, 150, 58, 128, 192, 239, 126, 20, 136, 220, 115, 118,
      189, 205, 2, 75, 125, 95, 186, 135, 110, 9, 184, 5, 232, 170, 197, 151, 244, 117, 62, 193, 177, 184, 151, 200,
      234, 211, 206, 90, 20, 252, 232, 22, 142, 189, 72, 54, 95, 79, 26, 141, 242, 141, 217, 56, 120, 100, 38, 49, 241]


def GetValues(values):
    ps = [1, 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37]
    return list(values[sum(ps[:i]):][:p] for i, p in enumerate(ps))


def Transform(values, v, n):
    return reduce(lambda x, y: x ^ y, (value[n % len(value)] for value in values), v)


def IntsToBase64(ints):
    return System.Convert.ToBase64String([System.Byte(i) for i in ints].ToArray[System.Byte]())


def Base64ToInts(base64):
    return [int(b) for b in System.Convert.FromBase64String(base64)]


def Pack(values, t, n):
    return IntsToBase64(Transform(values, ord(t[i]), n + i) for i in xrange(0, len(t)))


def Unpack(values, t, n):
    ints = Base64ToInts(t)
    return ''.join(chr(Transform(values, ints[i], n + i)) for i in xrange(0, len(ints)))


def GetPacker(n):
    i = [n]
    values = GetValues(vs)

    def packer(t):
        packed = Pack(values, t, i[0])
        i[0] += len(t)
        return packed

    return packer


def GetUnpacker(n):
    i = [n]
    values = GetValues(vs)

    def unpacker(t):
        unpacked = Unpack(values, t, i[0])
        i[0] += len(unpacked)
        return unpacked

    return unpacker
