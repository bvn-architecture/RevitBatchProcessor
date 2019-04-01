//
// Revit Batch Processor
//
// Copyright (c) 2019  Daniel Rumery, BVN
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
        public const string OptionSwitchPrefix = "--";

        private static int FindArgOptionSwitch(string[] args, string optionSwitch)
        {
            return Array.FindIndex(args, 1, arg => arg.ToLower() == (OptionSwitchPrefix + optionSwitch.ToLower()));
        }

        private static string GetArgOptionValue(string [] args, string optionSwitch)
        {
            string optionValue = null;

            int optionSwitchIndex = FindArgOptionSwitch(args, optionSwitch);

            if (optionSwitchIndex != -1)
            {
                if ((optionSwitchIndex + 1) < args.Length)
                {
                    optionValue = args[optionSwitchIndex + 1];

                    if (optionValue.StartsWith(OptionSwitchPrefix))
                    {
                        optionValue = null;
                    }
                }
            }

            return optionValue;
        }

        public static bool HaveArguments()
        {
            var args = Environment.GetCommandLineArgs();

            return (args.Length > 1);
        }

        public static string GetCommandLineOption(string optionSwitch, bool expectOptionValue = true)
        {
            string optionValue = null;

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                var optionSwitchIndex = FindArgOptionSwitch(args, optionSwitch);

                if (optionSwitchIndex != -1)
                {
                    if (expectOptionValue)
                    {
                        optionValue = GetArgOptionValue(args, optionSwitch);
                    }
                    else
                    {
                        optionValue = string.Empty; // Indicates a value-less option exists.
                    }

                }
            }

            return optionValue;
        }

        public static IEnumerable<string> GetAllCommandLineOptionSwitches()
        {
            var allOptionSwitches = Enumerable.Empty<string>();

            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                allOptionSwitches = args.Where(arg => arg.StartsWith(OptionSwitchPrefix));
            }

            return allOptionSwitches.Select(optionSwitch => optionSwitch.Substring(OptionSwitchPrefix.Length).ToLower()).ToList();
        }

        public static bool HasCommandLineOption(string optionSwitch, bool expectOptionValue = true)
        {
            return GetCommandLineOption(optionSwitch, expectOptionValue) != null;
        }
    }
}
