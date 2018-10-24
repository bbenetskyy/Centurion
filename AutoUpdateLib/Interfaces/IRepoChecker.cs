using AutoUpdateLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdateLib.Interfaces
{
    public interface IRepoChecker
    {
        string VersionFileName { get; set; }
        string ApplicationPath { get; set; }
        RequestResult<bool> CheckForUpdates(string url);
        RequestResult<string> DownloadUpdates(string url);
        RequestResult<bool> UpdateApplication(string sourcePath);
    }
}
