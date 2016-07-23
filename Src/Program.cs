using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SyncFromUrl
{
    class Program
    {
        static void Main(string[] argsRaw)
        {
            var configPath = "config.txt";
            Config config;
            
            if(File.Exists(configPath))
                config = Config.FromFile(configPath);
            else
            {
                config = Config.Default;
                config.Save(configPath);
            }

            if (string.IsNullOrWhiteSpace(config.LocalDirectory))
                config.LocalDirectory = Directory.GetCurrentDirectory();

            var interval = 1000;
            if (!int.TryParse(config.SyncInterval, out interval))
                Console.WriteLine(string.Format("failed to parse SyncInterval {0} as int", config.SyncInterval));

            SyncFromUrl.ContinouslySync(config.QueryUrl, config.FileHostUrl, config.LocalDirectory, interval);
        }
    }
}
