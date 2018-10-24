using AutoUpdateLib;
using AutoUpdateLib.Interfaces;
using BotHost.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IRepoChecker repoChecker = new RepoChecker();

            repoChecker.ApplicationPath = Settings.Default.BotPath;
            repoChecker.VersionFileName = Settings.Default.VersionFileName;

            try
            {
                //Check for updates
                CheckForUpdates(repoChecker);
            }
            finally
            {
                //Run Bot Application
                try
                {
                    System.Diagnostics.Process.Start(Settings.Default.BotPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message,
                        "Fatal Application Error!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }

        private static void CheckForUpdates(IRepoChecker repoChecker)
        {
            DialogResult retry;
            do
            {
                retry = DialogResult.Cancel;
                var requestResult = repoChecker.CheckForUpdates(Settings.Default.CheckUrl);
                if (requestResult.Success)
                {
                    if (requestResult.Result && MessageBox.Show(
                        "Update Bot to newer version?",
                        "Updates available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DownloadUpdates(repoChecker);
                    }
                }
                else
                {
                    retry = MessageBox.Show(requestResult.Exception.Message,
                    "Checking for Updates",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Exclamation);
                }
            } while (retry == DialogResult.Retry);
        }

        private static void DownloadUpdates(IRepoChecker repoChecker)
        {
            var requestResult = repoChecker.DownloadUpdates(Settings.Default.DownloadUrl);
            if (requestResult.Success)
            {
                UpdateApplication(repoChecker, requestResult.Result);
            }
            else
            {
                MessageBox.Show(requestResult.Exception.Message,
                "Can't download Update",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }

        private static void UpdateApplication(IRepoChecker repoChecker, string sourcePath)
        {
            var requestResult = repoChecker.UpdateApplication(sourcePath);
            if (requestResult.Success)
            {
                FinishUpdate(requestResult.Result);
            }
            else
            {
                MessageBox.Show(requestResult.Exception.Message,
                "Can't update Bot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }

        private static void FinishUpdate(bool success)
        {
            if (success)
            {
                MessageBox.Show(
                "Bot Updated to newer version!",
                "Update Completed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            else
            {

                MessageBox.Show(
                "Bot was not updated to newer version!",
                "Update canceled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }
    }
}
