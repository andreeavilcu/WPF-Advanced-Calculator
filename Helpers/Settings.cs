using CalculatorApp.Properties;
using System;
using System.IO;
using System.Xml.Serialization;

namespace CalculatorApp.Helpers
{
    public class Settings
    {
        private static readonly string SettingsFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CalculatorApp",
            "settings.xml");

        public bool IsDigitGroupingEnabled { get; set; }
        public bool IsProgrammerMode { get; set; }
        public int CurrentBase { get; set; }


        public static Settings Load()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    using (var stream = File.OpenRead(SettingsFilePath))
                    {
                        var serializer = new XmlSerializer(typeof(Settings));
                        var settings = (Settings)serializer.Deserialize(stream);

                        if (settings == null)
                            return GetDefaultSettings();

                        return settings;
                    }
                }
            }
            catch
            {
                
            }

           
            return GetDefaultSettings();
        }

        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(SettingsFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var stream = File.Create(SettingsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(stream, this);
                }
            }
            catch
            {
                
            }
        }

        private static Settings GetDefaultSettings()
        {
            return new Settings
            {
                IsDigitGroupingEnabled = false,
                IsProgrammerMode = false,
                CurrentBase = 10
            };
        }
    }
}