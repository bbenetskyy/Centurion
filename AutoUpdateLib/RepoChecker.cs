using AutoUpdateLib.Interfaces;
using AutoUpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdateLib
{
    public class RepoChecker : IRepoChecker
    {
        public RequestResult<bool> CheckForUpdates()
        {
            //return new RequestResult<bool>(new Exception());
            return new RequestResult<bool>(true);
        }

        public RequestResult<string> DownloadUpdates()
        {
            //return new RequestResult<string>(new Exception());
            //using (var client = new WebClient())
            //{
            //    client.DownloadFile("http://something", @"D:\Downloads\1.zip");
            //}
            return new RequestResult<string>();
        }

        public RequestResult<bool> UpdateApplication(string sourcePath, string destinationPath)
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
