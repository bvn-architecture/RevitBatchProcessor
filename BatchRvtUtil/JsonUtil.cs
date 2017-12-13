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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil
{
    public static class JsonUtil
    {
        public static string SerializeToJson(JObject jobject, bool prettyPrint = false)
        {
            return prettyPrint ? jobject.ToString() : jobject.ToString(Formatting.None);
        }

        public static string SerializeToJson(JArray jarray, bool prettyPrint = false)
        {
            return prettyPrint ? jarray.ToString() : jarray.ToString(Formatting.None);
        }

        public static JObject DeserializeFromJson(string text)
        {
            return JsonConvert.DeserializeObject(text) as JObject;
        }

        public static JArray DeserializeArrayFromJson(string text) 
        {
            return JsonConvert.DeserializeObject(text) as JArray;
        }
    }
}
