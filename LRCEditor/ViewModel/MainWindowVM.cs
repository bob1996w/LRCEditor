using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LRCEditor.Utils;
using LRCEditor.View;

namespace LRCEditor.ViewModel
{
    public class MainWindowVM : BasicViewModel
    {
        private void NotifyAllProperties()
        {
            NotifyPropertyChanged(nameof(_len));
            NotifyPropertyChanged(nameof(playIcon));
            NotifyPropertyChanged(nameof(MusicFilename));
            NotifyPropertyChanged(nameof(windowTitle));
            NotifyPropertyChanged(nameof(IsLyricAreaDisplayed));
        }

        private PlayerModule PM => App.PM;
        public string[] validExtention = { ".mp3", ".flac" };
        public TimeSpan _pos { get => PM.Position; set => PM.Position = value; }
        public double d_pos { get => PM.Position.TotalMilliseconds; set => PM.Position = TimeSpan.FromMilliseconds(value); }
        public TimeSpan _len { get => PM.Length; }
        public double _vol { get => PM.Volume; set => PM.Volume = value; }
        private bool isLyricAreaDisplayed = true;
        public bool IsLyricAreaDisplayed { get => isLyricAreaDisplayed; set => isLyricAreaDisplayed = value; }


        public string windowTitle =>
            ((LyricChanged) ? "*" : "") + "LRCEditor " + App.settings.version;
        public string playIcon =>
            (PM.PlaybackState == CSCore.SoundOut.PlaybackState.Playing) ? "pause" :
            (PM.PlaybackState == CSCore.SoundOut.PlaybackState.Paused) ? "play" : "play";
        private string musicFilename = "Not loaded";
        public string MusicFilename { get => musicFilename; set => musicFilename = value; }

        private string lyricEdit = "";
        public string LyricEdit { get => lyricEdit; set => lyricEdit = value; }

        public TextBox tb => App.MainWin.tb_lyric;
        private bool lyricChanged = false;
        public bool LyricChanged { get => lyricChanged; set { lyricChanged = value; NotifyAllProperties(); } }
        private bool hasSaved = false;
        public bool HasSaved { get => hasSaved; set => hasSaved = value; }
        private string savedLyricPath = "";
        public string SavedLyricPath { get => savedLyricPath; set => savedLyricPath = value; }
        private string savedInitialDir = "";
        public string SavedInitialDir { get => savedInitialDir; set => savedInitialDir = value; }
        private string savedLyricPresetFilename = "";
        public string SavedLyricPresetFilename { get => savedLyricPresetFilename; set => savedLyricPresetFilename = value; }

        public double savedCollapsedHeight = SystemParameters.WindowCaptionHeight + SystemParameters.ResizeFrameHorizontalBorderHeight*2 + 118 + SystemParameters.FixedFrameHorizontalBorderHeight*2 + +SystemParameters.BorderWidth*2;
        public double savedWindowHeight;

        public MainWindowVM()
        {
            PM.PositionChangedEvent += (Object sender) =>
            {
                NotifyPropertyChanged(nameof(_pos));
                NotifyPropertyChanged(nameof(d_pos));
            };
            //tb = App.MainWin.tb_lyric;
        }

        public ICommand Cmd_exit { get => new RelayCommand(On_exit, () => true); }
        void On_exit()
        {
            App.MainWin.Close();
        }

        public ICommand Cmd_newLyric { get => new RelayCommand(On_newLyric, () => true); }
        void On_newLyric()
        {
            bool isNewLyric = false;
            if (LyricChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show((String)App.MainWin.Resources["m_c_confirmNewLyric"],
                (String)App.MainWin.Resources["m_t_confirmNewLyric"], MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    isNewLyric = true;
            }
            else
                isNewLyric = true;
            if (isNewLyric)
            {
                tb.Text = "";
                LyricChanged = false;
                HasSaved = false;
            }
        }

        public ICommand Cmd_loadMusicDialog { get => new RelayCommand(On_loadMusicDialog, () => true); }
        void On_loadMusicDialog()
        {
            using (var ofd = new System.Windows.Forms.OpenFileDialog())
            {
                ofd.Filter = (string)App.MainWin.Resources["ofdf_music"];
                switch (ofd.ShowDialog())
                {
                    case System.Windows.Forms.DialogResult.None:
                        break;
                    case System.Windows.Forms.DialogResult.OK:
                        LoadMusicFile(ofd.FileName);
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        break;
                    case System.Windows.Forms.DialogResult.Abort:
                        break;
                    case System.Windows.Forms.DialogResult.Retry:
                        break;
                    case System.Windows.Forms.DialogResult.Ignore:
                        break;
                    case System.Windows.Forms.DialogResult.Yes:
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadMusicFile(string filepath)
        {
            PM.Open(filepath);
            SavedInitialDir = Path.GetDirectoryName(filepath);
            SavedLyricPresetFilename = Path.GetFileNameWithoutExtension(filepath) + ".lrc";
            NotifyAllProperties();
            MusicFilename = filepath;
            NotifyAllProperties();
        }


        public ICommand Cmd_loadLyricDialog { get => new RelayCommand(On_loadLyricDialog, () => true); }
        void On_loadLyricDialog()
        {
            var confirmed = false;
            if (LyricChanged)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show((String)App.MainWin.Resources["m_c_confirmLoadLyric"],
                (String)App.MainWin.Resources["m_t_confirmLoadLyric"], MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                    confirmed = true;
                else
                    confirmed = false;
            }
            else
                confirmed = true;
            if (confirmed)
            {
                using (var ofd = new System.Windows.Forms.OpenFileDialog())
                {
                    ofd.InitialDirectory = SavedInitialDir;
                    ofd.Filter = (string)App.MainWin.Resources["ofdf_lyric"];
                    switch (ofd.ShowDialog())
                    {
                        case System.Windows.Forms.DialogResult.None:
                            break;
                        case System.Windows.Forms.DialogResult.OK:
                            LoadLyricFile(ofd.FileName);
                            break;
                        case System.Windows.Forms.DialogResult.Cancel:
                            break;
                        case System.Windows.Forms.DialogResult.Abort:
                            break;
                        case System.Windows.Forms.DialogResult.Retry:
                            break;
                        case System.Windows.Forms.DialogResult.Ignore:
                            break;
                        case System.Windows.Forms.DialogResult.Yes:
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        void LoadLyricFile(string filepath)
        {
            SavedLyricPath = filepath;
            HasSaved = true;
            string textContent = File.ReadAllText(filepath);
            tb.Text = textContent;
        }

        public ICommand Cmd_saveLyric { get => new RelayCommand(On_saveLyric, () => true); }
        void On_saveLyric()
        {
            if (!HasSaved)
                OpenSaveFileDialog();
            else
                SaveLyric(SavedLyricPath);
        }

        public ICommand Cmd_saveLyricAs { get => new RelayCommand(On_saveLyricAs, () => true); }
        void On_saveLyricAs()
        {
            OpenSaveFileDialog();
        }


        void OpenSaveFileDialog()
        {
            using (var sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.Filter = (string)App.MainWin.Resources["sfdf_lyric"];
                sfd.InitialDirectory = SavedInitialDir;
                sfd.FileName = SavedLyricPresetFilename;
                switch (sfd.ShowDialog())
                {
                    case System.Windows.Forms.DialogResult.None:
                        break;
                    case System.Windows.Forms.DialogResult.OK:
                        SaveLyric(sfd.FileName);
                        break;
                    case System.Windows.Forms.DialogResult.Cancel:
                        break;
                    case System.Windows.Forms.DialogResult.Abort:
                        break;
                    case System.Windows.Forms.DialogResult.Retry:
                        break;
                    case System.Windows.Forms.DialogResult.Ignore:
                        break;
                    case System.Windows.Forms.DialogResult.Yes:
                        break;
                    case System.Windows.Forms.DialogResult.No:
                        break;
                    default:
                        break;
                }
            }

        }

        void SaveLyric(string filepath)
        {
            var fileContent = tb.Text;
            using (var fileWriter = File.CreateText(filepath))
            {
                fileWriter.Write(fileContent);
            }
            SavedLyricPath = filepath;
            HasSaved = true;
            LyricChanged = false;
        }

        public ICommand Cmd_playPause { get => new RelayCommand(On_playPause, () => true); }
        void On_playPause()
        {
            if(PM.PlaybackState == CSCore.SoundOut.PlaybackState.Playing)
            {
                PM.Pause();
            }
            else if (PM.PlaybackState == CSCore.SoundOut.PlaybackState.Paused)
            {
                PM.Play();
            }
            else if (PM.PlaybackState == CSCore.SoundOut.PlaybackState.Stopped)
            {
                PM.Position = TimeSpan.Zero;
                PM.Play();
            }
            NotifyAllProperties();
        }

        public ICommand Cmd_rew { get => new RelayCommand(On_rew, () => true); }
        void On_rew()
        {
            _pos = _pos - TimeSpan.FromSeconds(5);
        }

        public ICommand Cmd_ff { get => new RelayCommand(On_ff, () => true); }
        void On_ff()
        {
            _pos = _pos + TimeSpan.FromSeconds(5);
        }

        public ICommand Cmd_insertTimeTag { get => new RelayCommand(On_insertTimeTag, () => true); }
        void On_insertTimeTag()
        {
            string nowTimeTag = "[" + _pos.ToString(@"mm\:ss\.ff") + "]";
            int lineIdx = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            int lineFirstCharIdx = tb.GetCharacterIndexFromLineIndex(lineIdx);
            tb.Text = tb.Text.Insert(lineFirstCharIdx, nowTimeTag);
            MarkNextLyric(lineIdx);
        }

        public ICommand Cmd_replaceTimeTag { get => new RelayCommand(On_replaceTimeTag, () => true); }
        void On_replaceTimeTag()
        {
            string nowTimeTag = "[" + _pos.ToString(@"mm\:ss\.ff") + "]";
            int lineIdx = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            int lineFirstCharIdx = tb.GetCharacterIndexFromLineIndex(lineIdx);
            int lineLength = tb.GetLineLength(lineIdx);
            var lyricNowLine = tb.GetLineText(lineIdx);
            var lyricText = tb.Text;
            lyricText = lyricText.Remove(lineFirstCharIdx, lineLength);
            var re_Tag = new Regex(@"\[[0-9]*\:[0-9]*[\:\.][0-9]*\]");
            lyricNowLine = re_Tag.Replace(lyricNowLine, "");
            lyricNowLine = nowTimeTag + lyricNowLine;
            lyricText = lyricText.Insert(lineFirstCharIdx, lyricNowLine);
            tb.Text = lyricText;
            MarkNextLyric(tb.GetLineIndexFromCharacterIndex(lineFirstCharIdx + lineLength - 1));
        }
        public ICommand Cmd_insertTagOnNewLine { get => new RelayCommand(On_insertTagOnNewLine, () => true); }
        void On_insertTagOnNewLine()
        {
            string nowTimeTag = "[" + _pos.ToString(@"mm\:ss\.ff") + "]";
            int lineIdx = tb.GetLineIndexFromCharacterIndex(tb.CaretIndex);
            int lineLastCharIdx = tb.GetCharacterIndexFromLineIndex(lineIdx) + tb.GetLineLength(lineIdx);
            tb.Text = tb.Text.Insert(lineLastCharIdx, nowTimeTag + Environment.NewLine);
            int newLineIdx = tb.GetLineIndexFromCharacterIndex(lineLastCharIdx + 1);
            MarkNextLyric(newLineIdx);
        }

        public void MarkNextLyric(int lineIdx)
        {
            Debug.WriteLine($"Line {lineIdx}, tb.LineCount {tb.LineCount}");
            int nextLineIdx = (lineIdx + 1 < tb.LineCount) ? lineIdx + 1: lineIdx;
            Debug.WriteLine($"nextLine {nextLineIdx}");
            tb.CaretIndex = tb.GetCharacterIndexFromLineIndex(nextLineIdx);
            tb.ScrollToLine(nextLineIdx);
            tb.Select(tb.CaretIndex, tb.GetLineLength(nextLineIdx));
        }

        public ICommand Cmd_moveCaretToFileTop { get => new RelayCommand(On_moveCaretToFileTop, () => true); }
        void On_moveCaretToFileTop()
        {
            tb.CaretIndex = 0;
            tb.ScrollToLine(0);
        }

        public ICommand Cmd_moveCaretToLineHead { get => new RelayCommand(On_moveCaretToLineHead, () => true); }
        void On_moveCaretToLineHead()
        {
            tb.CaretIndex = tb.GetCharacterIndexFromLineIndex(tb.GetLineIndexFromCharacterIndex(tb.CaretIndex));
        }

        public ICommand Cmd_removeAllTag { get => new RelayCommand(On_removeAllTag, () => true); }
        void On_removeAllTag()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show((String)App.MainWin.Resources["m_c_confirmRemoveAllTag"],
                (String)App.MainWin.Resources["m_t_confirmRemoveAllTag"], MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                var lyricText = tb.Text;
                var re_Tag = new Regex(@"\[[0-9]*\:[0-9]*[\:\.][0-9]*\]");
                lyricText = re_Tag.Replace(lyricText, "");
                tb.Text = lyricText;
            }
        }

        public ICommand Cmd_changeLanguage { get => new RelayCommand<String>(On_changeLanguage, (lang) => true); }
        public void On_changeLanguage(string lang)
        {
            Console.WriteLine($"Change Language: {lang}");
            try
            {
                App.settings.Lang = lang;
                App.MainWin.Resources.Source =
                    new Uri($@"..\Resources\Strings\lang-{lang}.xaml", UriKind.Relative);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Lang not found. Fallback to 'en_US'.\n{e}");
                App.settings.Lang = "en_US";
            }
            
        }
        

        public void AdjustLyricEditArea()
        {
            tb.ScrollToLine(tb.GetLineIndexFromCharacterIndex(tb.CaretIndex));
        }

        public ICommand Cmd_toggleLyricEditArea { get => new RelayCommand(On_toggleLyricEditArea, () => true); }

        void On_toggleLyricEditArea()
        {
            if (App.MainWin.lyric_edit_area.Visibility == Visibility.Visible)
            {
                savedWindowHeight = App.MainWin.Height;
                App.MainWin.lyric_edit_area.Visibility = Visibility.Collapsed;
                App.MainWin.Height = savedCollapsedHeight;
                IsLyricAreaDisplayed = false;
            }
            else
            {
                App.MainWin.lyric_edit_area.Visibility = Visibility.Visible;
                App.MainWin.Height = savedWindowHeight;
                IsLyricAreaDisplayed = true;
            }
            NotifyAllProperties();
        }
        
        public ICommand Cmd_showAbout { get => new RelayCommand(On_showAbout, () => true); }
        void On_showAbout()
        {
            new AboutWindow().ShowDialog();
        }
    }
}