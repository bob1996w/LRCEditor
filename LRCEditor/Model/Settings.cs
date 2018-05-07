using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LRCEditor.Model
{
    public class Settings
    {
        public static string XMLpath = "settings.xml";
        public string version = "0.2";
        private string lang = "en_US";

        public string Lang { get => lang; set => lang = value; }
        public void SaveSettings()
        {
            SaveSettingsAsXml(this, XMLpath);
        }
        public static void SaveSettingsAsXml(Settings settings, string filename)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(settings.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, settings);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(filename);
                    stream.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something went wrong when saving xml: {e}");
            }
        }
        public static Settings LoadSettingsFromXml(string filename)
        {
            if (string.IsNullOrEmpty(filename)) { throw new ArgumentNullException("No argument 'filename' supplied"); }
            Settings settings = new Settings();
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(filename);
                string xmlString = xmlDocument.OuterXml;
                using (StringReader read = new StringReader(xmlString))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        settings = (Settings)serializer.Deserialize(reader);
                        reader.Close();
                    }
                    read.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return settings;
        }
        public static Settings LoadOrNew(string filename)
        {
            Settings settings;
            if (File.Exists(filename))
            {
                Console.WriteLine("Load from xml");
                settings = LoadSettingsFromXml(filename);
            }
            else
            {
                Console.WriteLine("No xml, new one");
                settings = new Settings();
            }
            return settings;
        }
    }
}
