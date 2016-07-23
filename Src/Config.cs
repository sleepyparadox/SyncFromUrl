using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyncFromUrl
{
    public class Config
    {
        [Description("# Url to return newline seperated short paths like \"dubstep/1-rickroll.mp3\"")]
        public string QueryUrl;

        [Description("# Url to prefix paths when downloading")]
        public string FileHostUrl;

        [DescriptionAttribute("# If no local directory is set it will use applications current directory")]
        public string LocalDirectory;

        [Description("# Intervall between sync attempts in milliseconds")]
        public string SyncInterval;

        public static Config Default
        {
            get
            {
                return new Config()
                {
                    QueryUrl = "http://example.com:8000/",
                    FileHostUrl = "http://example.com:8000/songs",
                    LocalDirectory = "",
                    SyncInterval = "1000",
                };
            }
        }

        public static Config FromFile(string path)
        {
            var config = Default;
            var fields = config.GetType().GetFields();

            var lines = File.ReadAllLines(path);
            foreach(var line in lines.Where(s => !string.IsNullOrEmpty(s)))
            {
                if (line[0] == '#')
                    continue;

                var key = line.Split(new[] { ' ' }).First();
                var val = line.Substring(key.Length, line.Length - key.Length);

                if (string.IsNullOrWhiteSpace(val))
                {
                    Console.WriteLine(string.Format("no value found for {0}, skipping", key));
                    continue;
                }

                var field = fields.FirstOrDefault(f => f.Name == key);

                if(field == null)
                {
                    Console.WriteLine(string.Format("Config {0}:", key));
                    foreach(var f in fields)
                        Console.WriteLine("\t" + f.Name);
                    continue;
                }

                Console.WriteLine(string.Format("Config {0}: {1}", key, val));

                field.SetValue(config, val);
            }

            return config;
        }

        public void Save(string path)
        {
            var lines = GetType().GetFields().SelectMany(field => 
            {
                var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), true).First() as DescriptionAttribute;
                
                var desc = attribute.Description;
                
                var val = field.GetValue(this) as string; ;

                var fieldLines = new string[]
                {
                    attribute.Description,
                    field.Name + " " + field.GetValue(this) as string,
                    "",
                };

                return fieldLines;
            });

            File.WriteAllLines(path, lines.ToArray());
        }
    }
}
