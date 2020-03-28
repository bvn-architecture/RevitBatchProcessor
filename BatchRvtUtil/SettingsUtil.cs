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
using Newtonsoft.Json.Linq;

namespace BatchRvtUtil
{
    public interface IPersistent
    {
        void Load(JObject jobject);
        void Store(JObject jobject);
    }

    public interface ISetting<T> : IPersistent
    {
        string GetName();
        T GetValue();
        void SetValue(T value);
    }

    public class Setting<T> : ISetting<T>
    {
        private readonly string name;
        private readonly Action<JObject, string, T> serialize;
        private readonly Func<JObject, string, T> deserialize;
        private readonly T defaultValue;
        private T value;

        public Setting(string name, Action<JObject, string, T> serialize, Func<JObject, string, T> deserialize, T defaultValue)
        {
            this.name = name;
            this.serialize = serialize;
            this.deserialize = deserialize;
            this.defaultValue = defaultValue;
            this.value = defaultValue;
        }

        public string GetName()
        {
            return this.name;
        }

        public void Load(JObject jobject)
        {
            this.value = this.deserialize(jobject, this.name);
        }

        public void Store(JObject jobject)
        {
            this.serialize(jobject, this.name, this.value);
        }

        public virtual T GetValue()
        {
            return this.value;
        }

        public virtual void SetValue(T value)
        {
            this.value = value;
        }
    }

    public class OptionalSetting<T> : Setting<T>
    {
        public OptionalSetting(string name, Action<JObject, string, T> serialize, Func<JObject, string, T> deserialize, T defaultValue)
            : base(
                name,
                serialize,
                (jobject, propertyName) => TryDeserialize(deserialize, jobject, propertyName, defaultValue),
                defaultValue
            )
        {
        }

        private static T TryDeserialize(Func<JObject, string, T> deserialize, JObject jobject, string propertyName, T defaultValue)
        {
            T value = defaultValue;

            try
            {
                value = deserialize(jobject, propertyName);
            }
            catch (Exception e)
            {
            }

            return value;
        }
    }

    public class BooleanSetting : OptionalSetting<bool>
    {
        public BooleanSetting(string name)
            : base(name, SetBooleanPropertyValue, GetBooleanPropertyValue, false)
        {
        }

        private static bool GetBooleanPropertyValue(JObject jobject, string propertyName)
        {
            return (jobject[propertyName] as JValue).ToObject<bool>();
        }

        private static void SetBooleanPropertyValue(JObject jobject, string propertyName, bool value)
        {
            jobject[propertyName] = value;
        }
    }

    public class IntegerSetting : OptionalSetting<int>
    {
        public IntegerSetting(string name)
            : base(name, SetIntegerPropertyValue, GetIntegerPropertyValue, 0)
        {
        }

        private static int GetIntegerPropertyValue(JObject jobject, string propertyName)
        {
            return (jobject[propertyName] as JValue).ToObject<int>();
        }

        private static void SetIntegerPropertyValue(JObject jobject, string propertyName, int value)
        {
            jobject[propertyName] = value;
        }
    }

    public class StringSetting : OptionalSetting<string>
    {
        public StringSetting(string name)
            : base(name, SetStringPropertyValue, GetStringPropertyValue, string.Empty)
        {
        }

        public override void SetValue(string value)
        {
            base.SetValue(InitializeFromString((value)));
        }

        public override string GetValue()
        {
            return InitializeFromString(base.GetValue());
        }

        private static string GetStringPropertyValue(JObject jobject, string propertyName)
        {
            return InitializeFromString((jobject[propertyName] as JValue).Value as string);
        }

        private static void SetStringPropertyValue(JObject jobject, string propertyName, string value)
        {
            jobject[propertyName] = InitializeFromString(value);
        }

        private static string InitializeFromString(string value)
        {
            return !string.IsNullOrWhiteSpace(value) ? value : string.Empty;
        }
    }

    public class EnumSetting<T> : OptionalSetting<T>
        where T : struct, IConvertible
    {
        public EnumSetting(string name)
            : base(name, SetEnumPropertyValue, GetEnumPropertyValue, default(T))
        {
        }

        private static T GetEnumPropertyValue(JObject jobject, string propertyName)
        {
            return StringToEnum((jobject[propertyName] as JValue).Value as string);
        }

        private static void SetEnumPropertyValue(JObject jobject, string propertyName, T value)
        {
            jobject[propertyName] = EnumToString(value);
        }

        private static T StringToEnum(string value)
        {
            var enumValue = default(T);

            if (!string.IsNullOrWhiteSpace(value))
            {
                bool isParsed = Enum.TryParse<T>(value, true, out enumValue);
            }

            return enumValue;
        }

        private static string EnumToString(T value)
        {
            return Enum.GetName(typeof(T), value);
        }
    }

    public class ListSetting<T> : OptionalSetting<List<T>>
    {
        public ListSetting(string name)
            : base(name, SetListPropertyValue, GetListPropertyValue, Enumerable.Empty<T>().ToList())
        {
        }

        public override void SetValue(List<T> value)
        {
            base.SetValue(InitializeFromList((value)));
        }

        public override List<T> GetValue()
        {
            return InitializeFromList(base.GetValue());
        }

        private static List<T> GetListPropertyValue(JObject jobject, string propertyName)
        {
            var jarray = (jobject[propertyName] as JArray);

            return InitializeFromList(jarray.Select(jvalue => jvalue.ToObject<T>()).ToList());
        }

        private static void SetListPropertyValue(JObject jobject, string propertyName, List<T> value)
        {
            jobject[propertyName] = new JArray(InitializeFromList(value));
        }

        private static List<T> InitializeFromList(IEnumerable<T> value)
        {
            return (value ?? Enumerable.Empty<T>()).ToList();
        }
    }

    public class PersistentSettings : IPersistent
    {
        private readonly IEnumerable<IPersistent> persistentSettings;

        public PersistentSettings(IEnumerable<IPersistent> persistentSettings)
        {
            this.persistentSettings = persistentSettings.ToList();
        }

        public void Load(JObject jobject)
        {
            foreach (var persistent in this.persistentSettings)
            {
                persistent.Load(jobject);
            }
        }

        public void Store(JObject jobject)
        {
            foreach (var persistent in this.persistentSettings)
            {
                persistent.Store(jobject);
            }
        }
    }

    public interface IUIConfigItem
    {
        void UpdateUI();
        void UpdateConfig();
    }

    public class UIConfigItem : IUIConfigItem
    {
        public UIConfigItem(Action updateUI, Action updateConfig)
        {
            updateUI_ = updateUI;
            updateConfig_ = updateConfig;
        }

        public void UpdateUI()
        {
            updateUI_();
        }

        public void UpdateConfig()
        {
            updateConfig_();
        }

        private Action updateUI_;
        private Action updateConfig_;
    }

    public class UIConfig : IUIConfigItem
    {
        private readonly IEnumerable<IUIConfigItem> uiConfigItems;

        public UIConfig(IEnumerable<IUIConfigItem> uiConfigItems)
        {
            this.uiConfigItems = uiConfigItems.ToList();
        }

        public void UpdateUI()
        {
            foreach (var uiConfigItem in this.uiConfigItems)
            {
                uiConfigItem.UpdateUI();
            }
        }

        public void UpdateConfig()
        {
            foreach (var uiConfigItem in this.uiConfigItems)
            {
                uiConfigItem.UpdateConfig();
            }
        }
    }
}
