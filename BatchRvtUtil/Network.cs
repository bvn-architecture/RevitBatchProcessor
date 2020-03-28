//
// Revit Batch Processor
//
// Copyright (c) 2020  Daniel Rumery, BVN
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace BatchRvtUtil
{
    public static class Network
    {
        public static IEnumerable<IPAddress> GetGatewayAddresses(
                NetworkInterface networkInterface
            )
        {
            return networkInterface
                .GetIPProperties()
                .GatewayAddresses
                .Select(ga => ga.Address);
        }

        public static IEnumerable<IPAddress> GetGatewayAddresses()
        {
            var gatewayAddresses = Enumerable.Empty<IPAddress>();

            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                gatewayAddresses = networkInterfaces
                    .SelectMany(GetGatewayAddresses)
                    .ToList();
            }
            catch (Exception)
            {
                gatewayAddresses = Enumerable.Empty<IPAddress>();
            }

            return gatewayAddresses;
        }

        public static IEnumerable<IPAddress> GetIPAddresses(
                NetworkInterface networkInterface
            )
        {
            return networkInterface
                .GetIPProperties()
                .UnicastAddresses
                .Select(ua => ua.Address);
        }

        public static IEnumerable<IPAddress> GetIPAddresses()
        {
            var ipAddresses = Enumerable.Empty<IPAddress>();

            try
            {
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                ipAddresses = networkInterfaces
                    .SelectMany(GetIPAddresses)
                    .ToList();
            }
            catch (Exception)
            {
                ipAddresses = Enumerable.Empty<IPAddress>();
            }

            return ipAddresses;
        }
    }
}
