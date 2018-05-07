using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LRCEditor.View
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.settings.SaveSettings();
            App.PM.Dispose();
        }

        private void tb_lyric_TextChanged(object sender, TextChangedEventArgs e)
        {
            App.MainWinVM.LyricChanged = true;
        }

        public void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (App.MainWinVM.LyricChanged)
            {
                MessageBoxResult result = MessageBox.Show((string)Resources["m_c_confirmExit"], (string)Resources["m_t_confirmExit"], MessageBoxButton.YesNo);
                if (result != MessageBoxResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }

        private void tb_lyric_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            App.MainWinVM.AdjustLyricEditArea();
        }
    }
}
