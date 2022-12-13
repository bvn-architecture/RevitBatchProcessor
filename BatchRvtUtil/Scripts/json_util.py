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

clr.AddReference("Newtonsoft.Json")

from Newtonsoft.Json import JsonConvert, Formatting
from Newtonsoft.Json.Linq import JObject, JValue


def GetValueFromJValue(jvalue):
    return JValue.Value.GetValue(jvalue)


def ToJObject(pythonObject):
    return JObject.FromObject(pythonObject)


def DeserializeToJObject(text):
    return JsonConvert.DeserializeObject(text)


def ToString(jobject, prettyPrint=False):
    return (
        JObject.ToString(jobject)
        if prettyPrint else
        JObject.ToString(jobject, Formatting.None)
    )


def SerializeObject(pythonObject, prettyPrint=False):
    return ToString(ToJObject(pythonObject), prettyPrint)
