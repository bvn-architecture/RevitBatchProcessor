#
# Revit Batch Processor
#
# Copyright (c) 2017  Dan Rumery, BVN
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
clr.AddReference("System.Core")
clr.ImportExtensions(System.Linq)

from System.Net import IPAddress

import batch_rvt_util
from batch_rvt_util import Network

def IsSpecialAddress(address):
  return (
    address.Equals(IPAddress.Any) or
    address.Equals(IPAddress.Broadcast) or
    address.Equals(IPAddress.IPv6Any) or
    address.Equals(IPAddress.IPv6Loopback) or
    address.Equals(IPAddress.IPv6None) or
    address.Equals(IPAddress.Loopback)
  )

def GetGatewayAddresses():
  return list(
      address for address in 
      Network.GetGatewayAddresses()
      .Where(lambda a: not IsSpecialAddress(a))
      .Select(lambda a: a.ToString())
      .Distinct()
      .OrderBy(lambda a: a)
    )

def GetIPAddresses():
  return list(
      address for address in 
      Network.GetIPAddresses()
      .Where(lambda a: not IsSpecialAddress(a))
      .Select(lambda a: a.ToString())
      .Distinct()
      .OrderBy(lambda a: a)
    )

