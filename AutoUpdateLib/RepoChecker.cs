using AutoUpdateLib.Interfaces;
using AutoUpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdateLib
{
    public class RepoChecker : IRepoChecker
    {
        public string VersionFileName { get; set; }
        public string ApplicationPath { get; set; }


        public RequestResult<bool> CheckForUpdates(string url)
        {
            var versionFilePath = Path.Combine(ApplicationPath, VersionFileName);
            if (!File.Exists(versionFilePath))
                return new RequestResult<bool>(new FileNotFoundException
                    ("Version File Not Found!", versionFilePath));

            var updateAvailable = false;
            var newVersionFile = @".\newVersion.txt";
            

            using (var client = new WebClient())
            {
                client.DownloadFile(url, newVersionFile);
            }
            Thread.Sleep(2000);
            var newVersion = new Version(File.ReadAllText(newVersionFile));
            var oldVersion = new Version(File.ReadAllText(versionFilePath));
            updateAvailable = newVersion > oldVersion;

            return new RequestResult<bool>(updateAvailable);
        }

        public RequestResult<string> DownloadUpdates(string url)
        {
            //return new RequestResult<string>(new Exception());
            //using (var client = new WebClient())
            //{
            //    client.DownloadFile("https://drive.google.com/open?id=1jM0uP1FKqETv09GF5YUq0nE6RRweYNCq", @".\bot.zip");
            //}
            return new RequestResult<string>();
        }

        public RequestResult<bool> UpdateApplication(string sourcePath)
        {
            //return new RequestResult<bool>(new Exception());
            using (var strm = File.OpenRead(sourcePath))
            using (ZipArchive a = new ZipArchive(strm))
            {
                a.Entries.Where(
                    o => 
                        o.Name == string.Empty &&
                        !Directory.Exists(Path.Combine(sourcePath, o.FullName)))
                    .ToList()
                    .ForEach(
                        o => 
                            Directory.CreateDirectory(Path.Combine(sourcePath, o.FullName)));
                a.Entries.Where(
                    o => 
                        o.Name != string.Empty)
                    .ToList()
                    .ForEach(
                        e => 
                            e.ExtractToFile(Path.Combine(sourcePath, e.FullName), true));
            }
            return new RequestResult<bool>(true);
        }
    }
}
