using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using MessageSender.Model;

namespace MessageSender.ViewModel
{
    public class BaseViewModel
    {
        public Config Config
        {
            get
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Config));
                try
                {
                    using (TextReader reader = new StreamReader(GetPath("./config.xml")))
                    {
                        return (Config)deserializer.Deserialize(reader);
                    }
                }
                catch (IOException)
                {
                    return new Config { ServerAddress = "" };
                }
            }
            set
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                using (TextWriter writer = new StreamWriter(GetPath("./config.xml")))
                {
                    serializer.Serialize(writer, value);
                }
            }
        }
        public string ServerIp
        {
            get
            {
                return Config.ServerAddress;
            }
            set
            {
                var config = Config;
                config.ServerAddress = value;
                Config = config;
            }
        }
        private string GetPath(string path)
        {
            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            return Path.Combine(basePath, path);
        }
    }
}
