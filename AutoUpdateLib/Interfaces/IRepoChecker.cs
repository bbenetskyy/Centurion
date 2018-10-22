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
        RequestResult<bool> CheckForUpdates();
        RequestResult<string> DownloadUpdates();
        RequestResult<bool> UpdateApplication(string sourcePath, string destinationPath);
    }
}
