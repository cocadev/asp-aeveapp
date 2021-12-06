using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using AevApp.Helper.Interface;

namespace AevApp.Helper.Implementation
{
    public class ConfigManager : IConfigManager
    {private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();
		private static bool _isLoading = false;
		private const string ConfigFileName = "config.xml";

		public ConfigManager() : this(null)
		{	
		}

		public ConfigManager(Type mainType)
		{
			EnsureAppSettingsAreLoaded(mainType);
		}

		private static void EnsureAppSettingsAreLoaded(Type mainType)
		{
			if (_isLoading) return;

			_isLoading = true;
			var type = mainType ?? typeof(App);
			var resource = $"{type.Namespace}.{ConfigFileName}";

			using (var stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(resource))
			using (var reader = new StreamReader(stream))
			{
				var doc = XDocument.Parse(reader.ReadToEnd());
				foreach (var setting in doc.Element("configuration").Element("appSettings").Elements())
				{
					if (!setting.HasAttributes) continue;

					var settingKey = setting.Attribute("key");
					var settingValue = setting.Attribute("value");
					if (ShouldAddSetting(settingKey, settingValue))
						Settings.Add(settingKey.Value.ToLower(), settingValue.Value);
				}
			}

			_isLoading = false;
		}

		private static bool ShouldAddSetting(XAttribute settingKey, XAttribute settingValue)
		{
			return settingKey != null && settingValue != null 
				&& !string.IsNullOrEmpty(settingKey.Value) && !string.IsNullOrEmpty(settingValue.Value)
				&& !Settings.ContainsKey(settingKey.Value.ToLower());
		}

		public string Get(string settingName)
		{
			if (string.IsNullOrEmpty(settingName)) return string.Empty;

			var name = settingName.ToLower();
			return Settings.ContainsKey(name) ? Settings[name] : string.Empty;
		}

		public bool GetBoolean(string settingName)
		{
			var setting = Get(settingName);
			if (!string.IsNullOrEmpty(setting)) return Convert.ToBoolean(setting);

			return false;
		}

		public int GetInt(string settingName)
		{
			var setting = Get(settingName);
			if (!string.IsNullOrEmpty(setting)) return Convert.ToInt32(setting);

			return int.MinValue;
		}
    }
}