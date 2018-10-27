using AutoUpdateLib;
using AutoUpdateLib.Interfaces;
using BotHost.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotHost
{
    public partial class EmptyForm : Form
    {
        IRepoChecker repoChecker = new RepoChecker();

        public EmptyForm()
        {
            InitializeComponent();

            repoChecker.ApplicationPath = Settings.Default.BotPath;
            repoChecker.VersionFileName = Settings.Default.VersionFileName;
        }


        private void EmptyForm_Shown(object sender, EventArgs e)
        {
            try
            {
                //Check for updates
                CheckForUpdates();
            }
            finally
            {
                //Run Bot Application
                try
                {
                    System.Diagnostics.Process.Start(
                        Path.Combine(Settings.Default.BotPath, Settings.Default.BotFileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message,
                        "Fatal Application Error!",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                }
                Close();
            }
        }

        private void CheckForUpdates()
        {
            DialogResult retry;
            do
            {
                retry = DialogResult.Cancel;

                var waitForm = new WaitForm(30);
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
                        DownloadUpdates();
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

        private void DownloadUpdates()
        {
            var waitForm = new WaitForm(50);
            var task = Task.Run(() => { waitForm.ShowDialog(); });

            var requestResult = repoChecker.DownloadUpdates(Settings.Default.DownloadUrl);

            waitForm.Invoke(new Action(() => waitForm.Close()));

            if (requestResult.Success)
            {
                UpdateApplication(requestResult.Result);
            }
            else
            {
                MessageBox.Show(this,requestResult.Exception.Message,
                "Can't download Update",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
            }
        }

        private void UpdateApplication(string sourcePath)
        {
            var waitForm = new WaitForm(100);
            var task = Task.Run(() => { waitForm.ShowDialog(); });

            var requestResult = repoChecker.UpdateApplication(sourcePath);

            waitForm.Invoke(new Action(() => waitForm.Close()));

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
                repoChecker.UpdateAppVersionFile();
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
