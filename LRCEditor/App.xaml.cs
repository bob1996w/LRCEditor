using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LRCEditor.View;
using LRCEditor.ViewModel;

namespace LRCEditor
{
    /// <summary>
    /// App.xaml 的互動邏輯
    /// </summary>
    public partial class App : Application
    {
        public static PlayerModule PM;
        //public LRCParser lrcParser;

        public static MainWindow MainWin;
        public static MainWindowVM MainWinVM;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            PM = new PlayerModule();
            MainWin = new MainWindow();
            MainWin.DataContext = MainWinVM = new MainWindowVM();
            MainWin.Show();
        }
    }
}
