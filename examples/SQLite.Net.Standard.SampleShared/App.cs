using SQLite.Net.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace XamForms
{
    public class App : Application
    {
        public static Button MainButton;
        public static ISQLitePlatform SQLitePlatform;
        public static string dbPath;

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
            MainButton.Text=string.Empty;

            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            MainButton.Text +=("Creating database and valuation table...");

            var database = new Stocks.Database(dbPath, SQLitePlatform );

            MainButton.Text +=("Downloading data and inserting in to table...");

            database.UpdateStock("GE");

            MainButton.Text +=("Getting data from database...");

            var data = database.GetData();

            foreach (var row in data.Data)
            {
                MainButton.Text +=($"Price: {row["Price"]}");
            }

            MainButton.Text +=("Done.");

            Console.Read();
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
