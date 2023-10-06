using System;
using System.Diagnostics;
using System.Net.Http;
using System.Windows.Forms;

namespace valorant_settings.App
{
    public partial class UpdateNotifier : Form
    {
        private const string CheckUrl = @"https://raw.githubusercontent.com/pepsizerosugar/csharp-valorant-settings/main/version";

        private readonly Version _current;

        // Utilize repository's file as version notifier
        // I know it sounds very dangerous. but i am broke as hell.
        private string _downloadUrl = @"https://github.com/pepsizerosugar/csharp-valorant-settings/releases/latest";
        private Version _latest;

        public UpdateNotifier(Version current)
        {
            InitializeComponent();
            _current = current;
            CurrentVersionLabel.Text = current.ToString();
            CheckUpdate();
        }

        private async void CheckUpdate()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(CheckUrl);
                    response.EnsureSuccessStatusCode();

                    var version = await response.Content.ReadAsStringAsync();
                    LatestVersionLabel.Text = version;
                    _latest = new Version(version);
                    if (_latest > _current) ShowDialog();
                }
                catch (Exception)
                {
                    // when response returns error or invalid string
                    Text = "Update Check Failure";
                    UpdateNotifyLabel.Text = "Update Check Failure\nPlease Visit Github Repository";
                    latestLabel.Visible = LatestVersionLabel.Visible = false;
                    UpdateButton.Text = "Visit";
                    _downloadUrl = @"https://github.com/pepsizerosugar/csharp-valorant-settings";
                    ShowDialog();
                }
            }
        }

        private void UpdateCancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            Process.Start(_downloadUrl);
            Application.Exit();
        }
    }
}