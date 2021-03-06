﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;

namespace GV.AspNet.Configuration.Contrib.ConfigurationManager
{
	public class AppSettingsConfigurationProvider : ConfigurationProvider
	{
		private const string ConfigurationKeyPrefix = "AppSettings";
		private static readonly string ConfigurationKeyDelimiter = Constants.KeyDelimiter;

		public AppSettingsConfigurationProvider(NameValueCollection appSettings, string appSettingsKeyDelimiter, params string[] appSettingsSectionPrefixes)
		{
			if (appSettings == null)
			{
				throw new ArgumentNullException(nameof(appSettings));
			}

			AppSettings = appSettings;
			AppSettingsKeyDelimiter = appSettingsKeyDelimiter;
			AppSettingsSectionPrefixes = appSettingsSectionPrefixes;
		}

		private NameValueCollection AppSettings { get; }
		private string AppSettingsKeyDelimiter { get; }
		private IEnumerable<string> AppSettingsSectionPrefixes { get; }

		public override void Load()
		{
			foreach (var appSettingKey in AppSettings.AllKeys)
			{
				var configurationKey = GetConfigurationKey(appSettingKey);
				var value = GetValue(appSettingKey);
				Data[configurationKey] = value;
			}
		}

		private string GetConfigurationKey(string appSettingsKey)
		{
			var configurationKey = appSettingsKey;

			if (!string.IsNullOrEmpty(AppSettingsKeyDelimiter))
			{
				configurationKey = configurationKey.Replace(AppSettingsKeyDelimiter, ConfigurationKeyDelimiter);
			}

			foreach (var appSettingsSectionPrefix in AppSettingsSectionPrefixes)
			{
				if (!configurationKey.StartsWith(appSettingsSectionPrefix, StringComparison.CurrentCultureIgnoreCase))
				{
					continue;
				}

				configurationKey = string.Concat(appSettingsSectionPrefix, ConfigurationKeyDelimiter, configurationKey.Substring(appSettingsSectionPrefix.Length));
				break;
			}

			configurationKey = string.Concat(ConfigurationKeyPrefix, ConfigurationKeyDelimiter, configurationKey.Trim());
			return configurationKey;
		}

		private string GetValue(string appSettingsKey) => AppSettings[appSettingsKey];
	}
}