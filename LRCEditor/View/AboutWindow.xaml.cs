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
using System.Windows.Shapes;

namespace LRCEditor.View
{
    /// <summary>
    /// AboutWindow.xaml 的互動邏輯
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }
        public string version => App.settings.version;
        private void btn_about_close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
