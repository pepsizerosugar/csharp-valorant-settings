using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using valorant_settings.GPU;
using valorant_settings.Setting;

namespace valorant_settings.App
{
    public partial class MainForm : Form
    {
        private readonly AppSetting _appSetting;
        private readonly IGpu _gpu = GpuDevice.Instance;
        private readonly ProcessMonitor _pMonitor = ProcessMonitor.Instance;

        private bool _minimizeOnStart;

        public MainForm()
        {
            InitializeComponent();

            #region Load App Settings

            // Load Settings
            _appSetting = AppSetting.Load();

            Brightness = _appSetting.Brightness;
            Contrast = _appSetting.Contrast;
            Gamma = _appSetting.Gamma;
            Dvl = _appSetting.Saturation;
            _minimizeOnStart = _appSetting.MinimizeOnStart;
            minimizeStartCheckBox.Checked = _minimizeOnStart;

            #endregion

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Text = $@"Valorant Settings {version}";
            _ = new UpdateNotifier(version);

            // Saturation Initialize
            if (_gpu.Vendor != GpuVendor.Nvidia)
                DVLGroupBox.Enabled = false;

            #region Initialize Display

            // Initialize Display Dropdown
            foreach (var display in Display.Display.Displays) DisplayCombo.Items.Add(display);

            if (DisplayCombo.FindString(_appSetting.Display) != -1)
                DisplayCombo.SelectedIndex = DisplayCombo.FindString(_appSetting.Display);

            Display.Display.Primary = (string)DisplayCombo.SelectedItem;

            #endregion

            // Initialize Process Monitor
            _pMonitor.Parent = this;
            foreach (var pTarget in _appSetting.PTargets) _pMonitor.Add(pTarget.ToLower());
            _pMonitor.Init();
        }

        public sealed override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public bool IsEnabled => enableToolStripMenuItem.Checked;

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (!_minimizeOnStart) return;
            Visible = false;
            ShowInTaskbar = false;
            trayIcon.ShowBalloonTip(
                2500,
                "Valorant Settings Initialized!",
                "Check out tray to modify your color setting",
                ToolTipIcon.Info
            );
        }

        private void ShowForm(object sender, EventArgs e)
        {
            Visible = true;
            ShowInTaskbar = true;
        }

        private void ExitFormClicked(object sender, EventArgs e)
        {
            _appSetting.Brightness = Brightness;
            _appSetting.Contrast = Contrast;
            _appSetting.Gamma = Gamma;
            _appSetting.Saturation = Dvl;
            _appSetting.Display = (string)DisplayCombo.SelectedItem;
            _appSetting.MinimizeOnStart = _minimizeOnStart;
            _appSetting.Save();

            Application.Exit();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
            else
            {
                Console.WriteLine("\n[INFO][MainForm] CloseReason: " + e.CloseReason);
                trayIcon.Dispose();
                Console.WriteLine(@"[INFO][MainForm] Closing pMonitor");
                _pMonitor.Close();
            }
        }

        private void CheckOnMinimizeToTray(object sender, EventArgs e)
        {
            _minimizeOnStart = minimizeStartCheckBox.Checked;
        }

        #region BCGS Getter/Setter

        private double Brightness
        {
            get => BrightnessBar.Value / 100.0;
            set => BrightnessBar.Value = (int)(value * 100);
        }

        private double Contrast
        {
            get => ContrastBar.Value / 100.0;
            set => ContrastBar.Value = (int)(value * 100);
        }

        private double Gamma
        {
            get => GammaBar.Value / 100.0;
            set => GammaBar.Value = (int)(value * 100);
        }

        private int Dvl
        {
            get => DVLBar.Value;
            set => DVLBar.Value = value;
        }

        public (double, double, double, int) GetColorValue()
        {
            return (
                BrightnessBar.Value / 100.0,
                ContrastBar.Value / 100.0,
                GammaBar.Value / 100.0,
                DVLBar.Value
            );
        }

        #endregion

        #region Control Event Handlers

        private void ColorLabel_DClick(object sender, EventArgs e)
        {
            var label = sender as Label;

            Debug.Assert(label != null, nameof(label) + " != null");
            if (label.Equals(BrightnessLabel))
                BrightnessBar.Value = 50;
            else if (label.Equals(ContrastLabel))
                ContrastBar.Value = 50;
            else if (label.Equals(GammaLabel))
                GammaBar.Value = 100;
            else if (label.Equals(DVLLabel)) DVLBar.Value = 0;
        }

        private void TrackBar_ValueChanged(object sender, EventArgs e)
        {
            var trackBar = sender as TrackBar;

            Debug.Assert(trackBar != null, nameof(trackBar) + " != null");
            if (trackBar.Equals(BrightnessBar))
                BrightnessText.Text = (BrightnessBar.Value / 100.0).ToString("0.00");
            else if (trackBar.Equals(ContrastBar))
                ContrastText.Text = (ContrastBar.Value / 100.0).ToString("0.00");
            else if (trackBar.Equals(GammaBar))
                GammaText.Text = (GammaBar.Value / 100.0).ToString("0.00");
            else if (trackBar.Equals(DVLBar)) DVLText.Text = DVLBar.Value.ToString();
        }

        private void DisplayCombo_SelectedValueChanged(object sender, EventArgs e)
        {
            var selectedDisplay = (string)DisplayCombo.SelectedItem;
            Display.Display.Primary = selectedDisplay;

            if (Display.Display.Primary != selectedDisplay)
                DisplayCombo.SelectedIndex = DisplayCombo.FindString(Display.Display.Primary);
        }

        #endregion
    }
}