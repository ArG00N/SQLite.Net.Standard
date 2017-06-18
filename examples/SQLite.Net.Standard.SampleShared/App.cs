using SQLite.Net.Interop;
using System;
using System.IO;

using Xamarin.Forms;

namespace XamForms
{
    public class App : Application
    {
        public Button MainButton;
        public ISQLitePlatform SQLitePlatform;
        public string DBPath;

        public App()
        {
            MainButton = new Button
            {
                Text = "Click Me",
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            MainButton.Clicked += MainButton_Clicked;

            // The root page of your application
            MainPage = new ContentPage
            {
                Content = MainButton
            };
        }

        private void MainButton_Clicked(object sender, EventArgs e)
        {
            MainButton.Text = string.Empty;

            if (File.Exists(DBPath))
            {
                File.Delete(DBPath);
            }

            MainButton.Text += ("Creating database and valuation table...\r\n");

            var database = new Stocks.Database(DBPath, SQLitePlatform);

            MainButton.Text += ("Downloading data and inserting in to table...\r\n");

            database.UpdateStock("GE");

            MainButton.Text += ("Getting data from database...\r\n");

            var data = database.GetData();

            foreach (var row in data.Data)
            {
                MainButton.Text += ($"Price: {row["Price"]}\r\n");
            }

            MainButton.Text += ("Done.");
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
