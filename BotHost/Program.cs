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

            try
            {
                DialogResult retry;
                //Check for updates
                do
                {
                    retry = DialogResult.Cancel;
                    dynamic requestResult = repoChecker.CheckForUpdates();
                    if (requestResult.Success)
                    {
                        if (requestResult.Result && MessageBox.Show(null,
                            "Update Bot to newer version?",
                            "Updates available",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            requestResult = repoChecker.DownloadUpdate();
                            if (requestResult.Success)
                            {
                                requestResult = repoChecker.UpdateApplication(requestResult.Result,
                                    System.IO.Path.GetDirectoryName(Settings.Default.BotExePath));
                                if (requestResult.Success)
                                {
                                    if (requestResult.Result)
                                    {
                                        MessageBox.Show(null,
                                        "Bot Updated to newer version!",
                                        null,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                                    }
                                    else
                                    {

                                        MessageBox.Show(null,
                                        "Bot was nto updated to newer version!",
                                        null,
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(null, requestResult.Exception.Message,
                                    "Can't update Bot",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                                }
                            }
                            else
                            {
                                MessageBox.Show(null, requestResult.Exception.Message,
                                "Can't download Update",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                            }
                        }
                    }
                    else
                    {
                        retry = MessageBox.Show(null, requestResult.Exception.Message,
                        "Checking for Updates",
                        MessageBoxButtons.RetryCancel,
                        MessageBoxIcon.Exclamation);
                    }
                } while (retry == DialogResult.Retry);
            }
            finally
            {
                //Run Bot Application
                try
                {
                    System.Diagnostics.Process.Start(Settings.Default.BotExePath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(null, e.Message,
                        "Fatal Application Error!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
