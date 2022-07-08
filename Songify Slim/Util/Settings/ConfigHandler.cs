﻿using Songify_Slim.Util.Settings;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using static System.Convert;
using Application = System.Windows.Application;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace Songify_Slim
{
    /// <summary>
    ///     This class is for writing, exporting and importing the config file
    ///     The config file is XML and has a single config tag with attributes
    /// </summary>
    internal class ConfigHandler
    {
        public static void SaveConfig(string path = "")
        {
            // Saving the Config file
            if (path != "")
            {
                WriteXml(path);
            }
            else
            {
                // Importing the SaveFileDialog and giving it filter, directory and window title
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Filter = "XML (*.xml)|*.xml",
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Title = "Export Config"
                };

                // Opneing the dialog and if the user clicked on "save" this code gets executed
                if (saveFileDialog.ShowDialog() != null ? saveFileDialog.ShowDialog().Value : false) WriteXml(saveFileDialog.FileName);
            }
        }

        public static void WriteXml(string path, bool hidden = false)
        {
            if (!File.Exists(path))
                File.Create(path).Close();
            FileInfo myFile = new FileInfo(path);
            // Remove the hidden attribute of the file
            myFile.Attributes &= ~FileAttributes.Hidden;

            // XML-Writer settings
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineOnAttributes = true
            };

            // Writing the XML, Attributnames are somewhat equal to Settings.
            using (XmlWriter writer = XmlWriter.Create(path, xmlWriterSettings))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("Songify_Config");
                writer.WriteStartElement("Config");
                Config cfg = Settings.Export();
                foreach (PropertyInfo prop in cfg.GetType().GetProperties())
                {
                    writer.WriteAttributeString(prop.Name, prop.GetValue(cfg, null).ToString());
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
            }

            myFile.Attributes &= ~FileAttributes.Hidden;
        }

        public static void ReadXml(string path)
        {
            try
            {
                if (new FileInfo(path).Length == 0)
                {
                    WriteXml(path);
                    return;
                }

                // reading the XML file, attributes get saved in Settings
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                if (doc.DocumentElement == null) return;
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.Name != "Config") continue;
                    //Create a new Config object and set the attributes
                    Config config = new Config();
                    if (node.Attributes != null)
                    {
                        config.Directory = node.Attributes["directory"] != null ? node.Attributes["directory"].Value : Settings.Directory;
                        config.Color = node.Attributes["color"] != null ? node.Attributes["color"].Value : Settings.Color;
                        config.Theme = node.Attributes["theme"] != null ? node.Attributes["theme"].Value : Settings.Theme;
                        config.Autostart = ToBoolean(node.Attributes["autostart"] != null ? node.Attributes["autostart"].Value : Settings.Autostart.ToString());
                        config.Systray = ToBoolean(node.Attributes["systray"] != null ? node.Attributes["systray"].Value : Settings.Systray.ToString());
                        config.CustomPauseTextEnabled = ToBoolean(node.Attributes["customPause"] != null ? node.Attributes["customPause"].Value : Settings.CustomPauseTextEnabled.ToString());
                        config.CustomPauseText = node.Attributes["customPauseText"] != null ? node.Attributes["customPauseText"].Value : Settings.CustomPauseText;
                        config.OutputString = node.Attributes["outputString"] != null ? node.Attributes["outputString"].Value : Settings.OutputString;
                        config.Uuid = node.Attributes["uuid"] != null ? node.Attributes["uuid"].Value : Settings.Uuid;
                        config.Telemetry = ToBoolean(node.Attributes["telemetry"] != null ? node.Attributes["telemetry"].Value : Settings.Telemetry.ToString());
                        config.NbUser = node.Attributes["nbuser"] != null ? node.Attributes["nbuser"].Value : Settings.NbUser;
                        config.NbUserId = node.Attributes["nbuserid"] != null ? node.Attributes["nbuserid"].Value : Settings.NbUserId;
                        config.Upload = ToBoolean(node.Attributes["uploadSonginfo"] != null ? node.Attributes["uploadSonginfo"].Value : Settings.Upload.ToString());
                        config.UploadHistory = ToBoolean(node.Attributes["uploadhistory"] != null ? node.Attributes["uploadhistory"].Value : Settings.UploadHistory.ToString());
                        config.SaveHistory = ToBoolean(node.Attributes["savehistory"] != null ? node.Attributes["savehistory"].Value : Settings.SaveHistory.ToString());
                        config.DownloadCover = ToBoolean(node.Attributes["downloadcover"] != null ? node.Attributes["downloadcover"].Value : Settings.DownloadCover.ToString());
                        config.RefreshToken = node.Attributes["refreshtoken"] != null ? node.Attributes["refreshtoken"].Value : Settings.RefreshToken;
                        config.SplitOutput = ToBoolean(node.Attributes["splitoutput"] != null ? node.Attributes["splitoutput"].Value : Settings.SplitOutput.ToString());
                        config.AccessToken = node.Attributes["accesstoken"] != null ? node.Attributes["accesstoken"].Value : Settings.AccessToken;
                        config.TwAcc = node.Attributes["twacc"] != null ? node.Attributes["twacc"].Value : Settings.TwAcc;
                        config.TwOAuth = node.Attributes["twoauth"] != null ? node.Attributes["twoauth"].Value : Settings.TwOAuth;
                        config.TwChannel = node.Attributes["twchannel"] != null ? node.Attributes["twchannel"].Value : Settings.TwChannel;
                        config.TwRewardId = node.Attributes["twrewardid"] != null ? node.Attributes["twrewardid"].Value : Settings.TwRewardId;
                        config.TwSrReward = ToBoolean(node.Attributes["twsrreward"] != null ? node.Attributes["twsrreward"].Value : Settings.TwSrReward.ToString());
                        config.TwSrCommand = ToBoolean(node.Attributes["twsrcommand"] != null ? node.Attributes["twsrcommand"].Value : Settings.TwSrCommand.ToString());
                        if (int.TryParse(node.Attributes["twsrmaxreq"] != null ? node.Attributes["twsrmaxreq"].InnerText : Settings.TwSrMaxReq.ToString(), out int value))
                            config.TwSrMaxReq = value;
                        if (int.TryParse(node.Attributes["twsrcooldown"] != null ? node.Attributes["twsrcooldown"].InnerText : Settings.TwSrCooldown.ToString(), out value))
                            config.TwSrCooldown = value;
                        config.MsgLoggingEnabled = ToBoolean(node.Attributes["msglogging"] != null ? node.Attributes["msglogging"].Value : Settings.MsgLoggingEnabled.ToString());
                        config.TwAutoConnect = ToBoolean(node.Attributes["twautoconnect"] != null ? node.Attributes["twautoconnect"].Value : Settings.TwAutoConnect.ToString());
                        config.ArtistBlacklist = node.Attributes["artistblacklist"] != null ? node.Attributes["artistblacklist"].Value : Settings.ArtistBlacklist;
                        config.UserBlacklist = node.Attributes["userblacklist"] != null ? node.Attributes["userblacklist"].Value : Settings.UserBlacklist;
                        config.PosX = ToInt32(node.Attributes["posx"] != null ? node.Attributes["posx"].Value : Settings.PosX.ToString(CultureInfo.CurrentCulture));
                        config.PosY = ToInt32(node.Attributes["posy"] != null ? node.Attributes["posy"].Value : Settings.PosY.ToString(CultureInfo.CurrentCulture));
                        config.AutoClearQueue = ToBoolean(node.Attributes["autoclearqueue"] != null ? node.Attributes["autoclearqueue"].Value : Settings.AutoClearQueue.ToString());
                        config.SpotifyDeviceId = node.Attributes["spotifydeviceid"] != null ? node.Attributes["spotifydeviceid"].Value : Settings.SpotifyDeviceId;
                        config.Language = node.Attributes["lang"] != null ? node.Attributes["lang"].Value : Settings.Language;
                        config.AppendSpaces = ToBoolean(node.Attributes["spacesenabled"] != null ? node.Attributes["spacesenabled"].Value : Settings.AppendSpaces.ToString());
                        config.SpaceCount = ToInt32(node.Attributes["Spacecount"] != null ? node.Attributes["Spacecount"].Value : Settings.SpaceCount.ToString(CultureInfo.CurrentCulture));
                        config.UseOwnApp = ToBoolean(node.Attributes["ownApp"] != null ? node.Attributes["ownApp"].Value : Settings.UseOwnApp.ToString());
                        config.ClientId = node.Attributes["clientid"] != null ? node.Attributes["clientid"].Value : Settings.ClientId;
                        config.ClientSecret = node.Attributes["clientsecret"] != null ? node.Attributes["clientsecret"].Value : Settings.ClientSecret;
                        config.MaxSongLength = ToInt32(node.Attributes["maxsonglength"] != null ? node.Attributes["maxsonglength"].Value : Settings.MaxSongLength.ToString(CultureInfo.CurrentCulture));
                        config.AnnounceInChat = ToBoolean(node.Attributes["announceinchat"] != null ? node.Attributes["announceinchat"].Value : Settings.AnnounceInChat.ToString());
                        config.BotCmdNext = ToBoolean(node.Attributes["botcmdnext"] != null ? node.Attributes["botcmdnext"].Value : Settings.BotCmdNext.ToString());
                        config.BotCmdPos = ToBoolean(node.Attributes["botcmdpos"] != null ? node.Attributes["botcmdpos"].Value : Settings.BotCmdPos.ToString());
                        config.BotCmdSong = ToBoolean(node.Attributes["botcmdsong"] != null ? node.Attributes["botcmdsong"].Value : Settings.BotCmdSong.ToString());
                    }

                    //Set the config object to the current config
                    Settings.Import(config);
                }
            }
            catch (Exception ex)
            {
                Logger.LogExc(ex);
            }
        }

        public static void LoadConfig(string path = "")
        {
            if (path != "")
            {
                ReadXml(path);
            }
            else
            {
                // OpenfileDialog with settings initialdirectory is the path were the exe is located
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = AppDomain.CurrentDomain.BaseDirectory,
                    Filter = @"XML files (*.xml)|*.xml|All files (*.*)|*.*"
                };

                // Opening the dialog and when the user hits "OK" the following code gets executed
                if (openFileDialog.ShowDialog() == DialogResult.OK) ReadXml(openFileDialog.FileName);

                // This will iterate through all windows of the software, if the window is typeof 
                // Settingswindow (from there this class is called) it calls the method SetControls
                foreach (Window window in Application.Current.Windows)
                    if (window.GetType() == typeof(Window_Settings))
                        ((Window_Settings)window).SetControls();
            }
        }
    }

    public class Config
    {
        // Create fields for each setting in the config file
        public string Directory { get; set; }
        public string Color { get; set; }
        public string Theme { get; set; }
        public bool Autostart { get; set; }
        public bool Systray { get; set; }
        public bool CustomPauseTextEnabled { get; set; }
        public string CustomPauseText { get; set; }
        public string OutputString { get; set; }
        public string Uuid { get; set; }
        public bool Telemetry { get; set; }
        public string NbUser { get; set; }
        public string NbUserId { get; set; }
        public bool Upload { get; set; }
        public bool UploadHistory { get; set; }
        public bool SaveHistory { get; set; }
        public bool DownloadCover { get; set; }
        public string RefreshToken { get; set; }
        public bool SplitOutput { get; set; }
        public string AccessToken { get; set; }
        public string TwAcc { get; set; }
        public string TwOAuth { get; set; }
        public string TwChannel { get; set; }
        public string TwRewardId { get; set; }
        public bool TwSrReward { get; set; }
        public bool TwSrCommand { get; set; }
        public int TwSrMaxReq { get; set; }
        public int TwSrCooldown { get; set; }
        public bool MsgLoggingEnabled { get; set; }
        public bool TwAutoConnect { get; set; }
        public string ArtistBlacklist { get; set; }
        public string UserBlacklist { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public bool AutoClearQueue { get; set; }
        public string SpotifyDeviceId { get; set; }
        public string Language { get; set; }
        public bool AppendSpaces { get; set; }
        public int SpaceCount { get; set; }
        public bool UseOwnApp { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int MaxSongLength { get; set; }
        public bool AnnounceInChat { get; set; }
        public bool BotCmdNext { get; set; }
        public bool BotCmdPos { get; set; }
        public bool BotCmdSong { get; set; }
    }
}