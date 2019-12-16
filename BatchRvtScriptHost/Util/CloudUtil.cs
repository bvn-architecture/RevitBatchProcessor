using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.IO.Pipes;

using Newtonsoft.Json;

namespace BatchRvt.ScriptHost
{
    public static class CloudUtil
    {
        private static Dictionary<string, dynamic> scriptDataCache = null;
        private const string SCRIPT_DATA_ENV_KEY = "BATCHRVT__SCRIPT_DATA_FILE_PATH";

        public static string GetScriptDataPath()
        {
            try
            {
                return ScriptHostUtil.GetEnvironmentVariables()[SCRIPT_DATA_ENV_KEY];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void GetScriptDataValue<T>(string sdKey, out T val)
        {
            if (scriptDataCache == null)
                scriptDataCache = GetScriptDataDict(GetScriptDataPath());
            if (scriptDataCache.TryGetValue(sdKey, out var _val))
                val = (T)_val;
            else
                val = default(T);
        }

        public static bool IsLastFileInBatch(string progrKey = "progressNumber", string maxKey = "progressMax")
        {
            GetScriptDataValue<int>(progrKey, out var progr);
            GetScriptDataValue<int>(maxKey, out var maxProgr);
            return progr == maxProgr;
        }

        public static Dictionary<string, dynamic> GetScriptDataDict(string sdPath)
        {
            try
            {
                var x = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(File.ReadAllText(sdPath));
                return x.FirstOrDefault();
            }
            catch (Exception e)
            {
                return new Dictionary<string, dynamic>();
            }
        }

        public static void OpenWithWhite(string rvtPath, bool keepRunning)
        {
            var procId = System.Diagnostics.Process.GetCurrentProcess().Id;
            using (var client = new NamedPipeClientStream(".", "__OPEN_CLOUD_MODEL__", PipeDirection.Out))
            {
                client.Connect();
                using (var sw = new StreamWriter(client))
                {
                    sw.AutoFlush = true;
                    sw.WriteLine(procId);
                    sw.WriteLine(rvtPath);
                    sw.WriteLine(keepRunning);
                }
            }
        }

        public static bool IsCloudPath(string p)
        {
            return p.StartsWith("BIM 360:") || p.StartsWith("BIM360:");
        }
    }
}
