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
    public static class SyncFromUrl
    {
        public static void ContinouslySync(string queryUrl, string fileHost, string localDirectory, int interval)
        {
            Console.WriteLine("Begin ContinousSync");
            while (true)
            {
                try
                {
                    Sync(queryUrl, fileHost, localDirectory);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                Thread.Sleep(interval);
            }
        }

        public static void Sync(string queryUrl, string fileHost, string localDirectory)
        {
            using (var client = new WebClient())
            {
                var syncInfo = client.DownloadString(queryUrl);
                var syncFiles = syncInfo.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach(var syncPath in syncFiles)
                {
                    var localPath = Path.Combine(localDirectory, syncPath.Replace("/", "\\"));
                    if(File.Exists(localPath))
                        continue;

                    Console.WriteLine("found " + syncPath);

                    var localPathsDirectory = localPath.Substring(0, localPath.LastIndexOf('\\'));
                    if (!Directory.Exists(localPathsDirectory))
                        Directory.CreateDirectory(localPathsDirectory);
                    
                    var serverPath = Path.Combine(fileHost, syncPath);
                    var localTempPath = localPath + ".SyncFromUrl.TEMP";
                    client.DownloadFile(serverPath, localTempPath);
                    File.Move(localTempPath, localPath);

                    Console.WriteLine("downloaded to " + localPath);
                }
            }
        }
    }
}
