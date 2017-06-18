using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace XamForms.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            var app = new XamForms.App();
            app.DBPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Stocks.db"); ;
            app.SQLitePlatform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(app.DBPath);

            LoadApplication(app);
        }
    }
}
