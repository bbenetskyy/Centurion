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
        public Version NewVersion { get; set; }

        public RequestResult<bool> CheckForUpdates(string url)
        {
            try
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

            NewVersion = new Version(File.ReadAllText(newVersionFile));
            var oldVersion = new Version(File.ReadAllText(versionFilePath));
            updateAvailable = NewVersion > oldVersion;

            return new RequestResult<bool>(updateAvailable);

            }
            catch (Exception ex)
            {
                return new RequestResult<bool>(ex);
            }
        }

        public RequestResult<string> DownloadUpdates(string url)
        {
            try
            {
                var botZipFile = @".\bot.zip";
                using (var client = new WebClient())
                {
                    client.DownloadFile(url, botZipFile);
                }

                return new RequestResult<string>(botZipFile);
            }
            catch (Exception ex)
            {
                return new RequestResult<string>(ex);
            }
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
                        !Directory.Exists(Path.Combine(ApplicationPath, o.FullName)))
                    .ToList()
                    .ForEach(
                        o => 
                            Directory.CreateDirectory(Path.Combine(ApplicationPath, o.FullName)));
                a.Entries.Where(
                    o => 
                        o.Name != string.Empty)
                    .ToList()
                    .ForEach(
                        e => 
                            e.ExtractToFile(Path.Combine(ApplicationPath, e.FullName), true));
            }
            return new RequestResult<bool>(true);
        }

        public void UpdateAppVersionFile()
        {
            var versionFilePath = Path.Combine(ApplicationPath, VersionFileName);
            File.WriteAllText(versionFilePath, NewVersion.ToString());
        }
    }
}
