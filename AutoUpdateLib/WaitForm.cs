#region Copyright Syncfusion Inc. 2001-2018.
// Copyright Syncfusion Inc. 2001-2018. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AutoUpdateLib
{
    public partial class WaitForm : Syncfusion.Windows.Forms.MetroForm
    {
        public int Interval { get; set; } = 100;

        public WaitForm()
        {
            InitializeComponent();
        }

        private void WaitForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        private void WaitForm_Shown(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBarAdv1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = progressBarAdv1.Minimum; i <= progressBarAdv1.Maximum; i++)
            {
                backgroundWorker1.ReportProgress(i);
                Thread.Sleep(Interval);
            }
        }
    }
}
