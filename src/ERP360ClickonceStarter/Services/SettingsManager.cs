using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PortableClickonce.Services
{
	public static class SettingsManager
	{
		public static Models.ClickonceSettings Load()
		{
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.ClickonceSettings));
			var settingFile = System.IO.Path.Combine(System.Environment.CurrentDirectory, "settings.xml");
			Models.ClickonceSettings result = null;
			if (!System.IO.File.Exists(settingFile))
			{
				result = CreateDefault();
			}
			else
			{
				try
				{
					using (var fs = System.IO.File.Open(settingFile, System.IO.FileMode.Open, System.IO.FileAccess.Read))
					{
						result = (Models.ClickonceSettings)xmlSerializer.Deserialize(fs);
						fs.Close();
					}
				}
				catch
				{
					result = CreateDefault();
				}
			}
			return result;
		}

		public static void SaveSettings(Models.ClickonceSettings settings)
		{
			var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Models.ClickonceSettings));
			var settingFile = System.IO.Path.Combine(System.Environment.CurrentDirectory, "settings.xml");

			try
			{
				using (var fs = System.IO.File.Open(settingFile, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					xmlSerializer.Serialize(fs, settings);
					fs.Close();
				}
			}
			catch(Exception ex)
			{
				// Flush error to disk
			}
		}

		private static Models.ClickonceSettings CreateDefault()
		{
			var result = new Models.ClickonceSettings();
			result.DestinationDirectory = System.Environment.CurrentDirectory;
			result.ClickonceUrl = "http://www.sample.net/my.application";
			return result;
		}
	}
}
