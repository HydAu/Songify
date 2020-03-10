﻿using Newtonsoft.Json;
using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;

namespace Songify_Slim
{
    public partial class SettingsWindow
    {
        private readonly string[] _colors = {
                                                   "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald",
                                                   "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta",
                                                   "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve",
                                                   "Taupe", "Sienna"
                                       };

        private readonly FolderBrowserDialog _fbd = new FolderBrowserDialog();
        private Window _mW;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        public void SetControls()
        {
            // Sets all the controls from settings
            ThemeToggleSwitch.IsChecked = Settings.Theme == "BaseDark";
            TxtbxOutputdirectory.Text = Assembly.GetEntryAssembly()?.Location ?? throw new InvalidOperationException();
            if (!string.IsNullOrEmpty(Settings.Directory))
                TxtbxOutputdirectory.Text = Settings.Directory;
            ChbxAutostart.IsChecked = Settings.Autostart;
            ChbxMinimizeSystray.IsChecked = Settings.Systray;
            ChbxCustomPause.IsChecked = Settings.CustomPauseTextEnabled;
            ChbxTelemetry.IsChecked = Settings.Telemetry;
            TxtbxCustompausetext.Text = Settings.CustomPauseText;
            TxtbxOutputformat.Text = Settings.OutputString;
            txtbx_nbuser.Text = Settings.NbUser;
            ChbxUpload.IsChecked = Settings.Upload;
            NudChrome.Value = Settings.ChromeFetchRate;
            ChbxCover.IsChecked = Settings.DownloadCover;
            ChbxSplit.IsChecked = Settings.SplitOutput;
            if (Settings.NbUserId != null)
            {
                lbl_nightbot.Content = "Nightbot (ID: " + Settings.NbUserId + ")";
            }
            if (APIHandler.spotify != null)
                lbl_SpotifyAcc.Content = "Linked account: " + APIHandler.spotify.GetPrivateProfile().DisplayName;

            ThemeHandler.ApplyTheme();
        }

        private void AppendText(System.Windows.Controls.TextBox tb, string text)
        {
            // Appends Rightclick-Text from the output text box (parameters)
            tb.AppendText(text);
            tb.Select(TxtbxOutputformat.Text.Length, 0);
            if (tb.ContextMenu != null) tb.ContextMenu.IsOpen = false;
        }

        private void Btn_ExportConfig_Click(object sender, RoutedEventArgs e)
        {
            // calls confighandler
            ConfigHandler.SaveConfig();
        }

        private void Btn_ImportConfig_Click(object sender, RoutedEventArgs e)
        {
            // calls confighandler
            ConfigHandler.LoadConfig();
        }

        private void Btn_nblink_Click(object sender, RoutedEventArgs e)
        {
            // Links the nightbot account using username
            try
            {
                // accessing nightbot API with username to get user id
                string jsn;
                using (WebClient wc = new WebClient())
                {
                    jsn = wc.DownloadString("https://api.nightbot.tv/1/channels/t/" + Settings.NbUser);
                }

                NbObj json = JsonConvert.DeserializeObject<NbObj>(jsn);
                string temp = json.Channel._id;
                temp = temp.Replace("{", "").Replace("}", "");
                Settings.NbUserId = temp;
                Lbl_Status.Content = @"Account " + Settings.NbUser + " linked.";
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

            SetControls();
        }

        private void BtnCopyToClipClick(object sender, RoutedEventArgs e)
        {
            // Copies the txt path to the clipboard and shows a notification
            if (string.IsNullOrEmpty(Settings.Directory))
            {
                System.Windows.Clipboard.SetDataObject(Assembly.GetEntryAssembly()?.Location.Replace("Songify Slim.exe", "Songify.txt") ?? throw new InvalidOperationException());
            }
            else
            {
                System.Windows.Clipboard.SetDataObject(Settings.Directory + "\\Songify.txt");
            }
            Lbl_Status.Content = @"Path copied to clipboard.";
        }

        private void BtnCopyURL_Click(object sender, RoutedEventArgs e)
        {
            // Copies the song info URL to the clipboard and shows notification
            System.Windows.Clipboard.SetDataObject("https://songify.bloemacher.com/getsong.php?id=" + Settings.Uuid);
            Lbl_Status.Content = @"URL copied to clipboard.";
        }

        private void BtnOutputdirectoryClick(object sender, RoutedEventArgs e)
        {
            // Where the user wants the text file to be saved in
            _fbd.Description = @"Path where the text file will be located.";
            _fbd.SelectedPath = Assembly.GetExecutingAssembly().Location;

            if (_fbd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;
            TxtbxOutputdirectory.Text = _fbd.SelectedPath;
            Settings.Directory = _fbd.SelectedPath;
        }

        private void BtnUpdatesClick(object sender, RoutedEventArgs e)
        {
            // checks for updates
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.GetType() != typeof(MainWindow)) continue;
                if (!((MainWindow)window).WorkerUpdate.IsBusy)
                {
                    ((MainWindow)window).WorkerUpdate.RunWorkerAsync();
                }
            }
        }

        private void ChbxAutostartChecked(object sender, RoutedEventArgs e)
        {
            // checkbox for autostart
            bool? chbxAutostartIsChecked = ChbxAutostart.IsChecked;
            MainWindow.RegisterInStartup(chbxAutostartIsChecked != null && (bool)chbxAutostartIsChecked);
        }

        private void ChbxCustompauseChecked(object sender, RoutedEventArgs e)
        {
            // enables / disables custom pause
            if (ChbxCustomPause.IsChecked == null) return;
            Settings.CustomPauseTextEnabled = (bool)ChbxCustomPause.IsChecked;
            if (!(bool)ChbxCustomPause.IsChecked)
            {
                TxtbxCustompausetext.IsEnabled = false;
            }
            else
            {
                TxtbxCustompausetext.IsEnabled = true;
            }
        }

        private void ChbxMinimizeSystrayChecked(object sender, RoutedEventArgs e)
        {
            // enables / disbales minimize to systray
            bool? isChecked = ChbxMinimizeSystray.IsChecked;
            Settings.Systray = isChecked != null && (bool)isChecked;
        }

        private void ChbxTelemetry_IsCheckedChanged(object sender, EventArgs e)
        {
            // enables / disables telemetry
            if (ChbxTelemetry.IsChecked == null) return;
            Settings.Telemetry = (bool)ChbxTelemetry.IsChecked;
        }

        private void ChbxUpload_Checked(object sender, RoutedEventArgs e)
        {
            // enables / disables upload
            if (ChbxUpload.IsChecked != null)
                Settings.Upload = (bool)ChbxUpload.IsChecked;
            ((MainWindow)_mW).UploadSong(((MainWindow)_mW).CurrSong);
        }

        private void ComboBoxColorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // sets the color, when selecting yellow it changes foreground color because else its hard to read
            Settings.Color = ComboBoxColor.SelectedValue.ToString();
            ThemeHandler.ApplyTheme();
            if (Settings.Color != "Yellow")
            {
                ((MainWindow)_mW).LblStatus.Foreground = Brushes.White;
                ((MainWindow)_mW).LblCopyright.Foreground = Brushes.White;
            }
            else
            {
                ((MainWindow)_mW).LblStatus.Foreground = Brushes.Black;
                ((MainWindow)_mW).LblCopyright.Foreground = Brushes.Black;
            }
        }

        private void MenuBtnArtist_Click(object sender, RoutedEventArgs e)
        {
            // appends text
            AppendText(TxtbxOutputformat, "{artist}");
        }

        private void MenuBtnExtra_Click(object sender, RoutedEventArgs e)
        {
            // appends text
            AppendText(TxtbxOutputformat, "{extra}");
        }

        private void MenuBtnTitle_Click(object sender, RoutedEventArgs e)
        {
            // appends text
            AppendText(TxtbxOutputformat, "{title}");
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // assing mw to mainwindow for calling methods and setting texts etc
            foreach (Window window in System.Windows.Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    _mW = window;
                }
            }

            // add colors to the combobox
            foreach (string s in _colors)
            {
                ComboBoxColor.Items.Add(s);
            }

            // select the current color
            foreach (string s in ComboBoxColor.Items)
            {
                if (s != Settings.Color) continue;
                ComboBoxColor.SelectedItem = s;
                Settings.Color = s;
            }


            SetControls();
        }

        private void ThemeToggleSwitchIsCheckedChanged(object sender, EventArgs e)
        {
            // set the theme (BaseLight / BaseDark)
            if (ThemeToggleSwitch.IsChecked != null && (bool)ThemeToggleSwitch.IsChecked)
            {
                Settings.Theme = "BaseDark";
            }
            else
            {
                Settings.Theme = "BaseLight";

            }

            ThemeHandler.ApplyTheme();
        }

        private void Txtbx_nbuser_TextChanged(object sender, TextChangedEventArgs e)
        {
            // write Nightbot username to settings
            Settings.NbUser = txtbx_nbuser.Text;
        }

        private void TxtbxCustompausetext_TextChanged(object sender, TextChangedEventArgs e)
        {
            // write CustomPausetext to settings
            Settings.CustomPauseText = TxtbxCustompausetext.Text;
        }

        private void TxtbxOutputformat_TextChanged(object sender, TextChangedEventArgs e)
        {
            // write custom output format to settings
            Settings.OutputString = TxtbxOutputformat.Text;
        }

        // nightbot JSON object
        public class NbObj
        {
            public dynamic Channel { get; set; }
        }

        private void NudChrome_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            // Sets the source (Spotify, Youtube, Nightbot)
            if (!IsLoaded)
            {
                // This prevents that the selected is always 0 (initialize components)
                return;
            }

            if (NudChrome.Value != null) Settings.ChromeFetchRate = (int)NudChrome.Value;
        }

        private void ChbxCover_Checked(object sender, RoutedEventArgs e)
        {
            // enables / disables telemetry
            if (ChbxCover.IsChecked == null) return;
            Settings.DownloadCover = (bool)ChbxCover.IsChecked;
        }

        private void btn_spotifyLink_Click(object sender, RoutedEventArgs e)
        {
            Settings.RefreshToken = "";
            try
            {
                APIHandler.DoAuthAsync();
                SetControls();

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

        }

        private void ChbxSplit_Checked(object sender, RoutedEventArgs e)
        {
            // enables / disables telemetry
            if (ChbxSplit.IsChecked == null) return;
            Settings.SplitOutput = (bool)ChbxCover.IsChecked;
        }
    }
}