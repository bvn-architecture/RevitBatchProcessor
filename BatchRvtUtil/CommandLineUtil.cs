//
// Revit Batch Processor
//
// Copyright (c) 2017  Daniel Rumery, BVN
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

namespace BatchRvtUtil
{
    public static class CommandLineUtil
    {
        public static string GetCommandLineOption(string optionSwitch)
        {
            string optionValue = null;
            
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                var optionSwitchIndex = Array.FindIndex(args, 1, arg => arg.ToLower() == ("--" + optionSwitch.ToLower()));
    
                if (optionSwitchIndex != -1 && (optionSwitchIndex + 1) < args.Length)
                {
                    optionValue = args[optionSwitchIndex + 1];
                }
            }

            return optionValue;
        }
    }
}
