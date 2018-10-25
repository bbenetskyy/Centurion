using AutoUpdateLib;
using AutoUpdateLib.Interfaces;
using BotHost.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotHost
{
    public partial class EmptyForm : Form
    {
        public EmptyForm()
        {
            InitializeComponent();
        }


        private void EmptyForm_Shown(object sender, EventArgs e)
        {
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
                catch (Exception exception)
                {
                    MessageBox.Show(this, exception.Message,
                        "Fatal Application Error!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                this.Close();
            }
        }

        private void CheckForUpdates(IRepoChecker repoChecker)
        {
            DialogResult retry;
            do
            {
                retry = DialogResult.Cancel;

                var waitForm = new WaitForm(50);
                var task = Task.Run(() => { waitForm.ShowDialog(); });

                var requestResult = repoChecker.CheckForUpdates(Settings.Default.CheckUrl);

                waitForm.Invoke(new Action(() => waitForm.Close()));

                if (requestResult.Success)
                {
                    if (requestResult.Result && MessageBox.Show(this,
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
                    retry = MessageBox.Show(this,requestResult.Exception.Message,
                    "Checking for Updates",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Exclamation);
                }
            } while (retry == DialogResult.Retry);
        }

        private void DownloadUpdates(IRepoChecker repoChecker)
        {
            var requestResult = repoChecker.DownloadUpdates(Settings.Default.DownloadUrl);
            if (requestResult.Success)
            {
                UpdateApplication(repoChecker, requestResult.Result);
            }
            else
            {
                MessageBox.Show(this,requestResult.Exception.Message,
                "Can't download Update",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }

        private void UpdateApplication(IRepoChecker repoChecker, string sourcePath)
        {
            var requestResult = repoChecker.UpdateApplication(sourcePath);
            if (requestResult.Success)
            {
                FinishUpdate(requestResult.Result);
            }
            else
            {
                MessageBox.Show(this,requestResult.Exception.Message,
                "Can't update Bot",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }

        private void FinishUpdate(bool success)
        {
            if (success)
            {
                MessageBox.Show(this,
                "Bot Updated to newer version!",
                "Update Completed",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            else
            {

                MessageBox.Show(this,
                "Bot was not updated to newer version!",
                "Update canceled",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }
    }
}
