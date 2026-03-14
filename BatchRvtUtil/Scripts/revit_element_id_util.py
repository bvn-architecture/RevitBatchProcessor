#
# Revit Batch Processor
#
# Copyright (c) 2026  
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
from Autodesk.Revit.DB import ElementId, WorksetId


def get_element_id_as_int(element_id):
    """
    Gets the integer ID of a Revit element. 
    This function is required because Revit element id has changed in Revit API 2025 and the element id integer value is no longer accessible via the IntegerValue property, but instead via the Value property.
    
    :param element_id: The Revit element ID.
    :return: The integer ID of the element. Or -1 if the input is not a valid ElementId or WorksetId.
    :rtype: int

    """
    
    if not isinstance(element_id, ElementId) and not isinstance(element_id, WorksetId):
        return -1

    # WorksetId.IntegerValue is likely returning 0 for some workset (Workset1 it appears), and 0 is falsy in Python - so the condition fails and falls through to the elif and then the raise
    # The fix is to use is not None:

    if getattr(element_id, "IntegerValue", None) is not None:
        return element_id.IntegerValue
    elif getattr(element_id, "Value", None) is not None:
        return int(element_id.Value) # Value is a Int64, so we convert to int for consistency with previous IntegerValue output. This should be safe...
    else:
        raise ValueError("Element id property: {} does not have an IntegerValue or Value attribute.".format(type(element_id)))
    
