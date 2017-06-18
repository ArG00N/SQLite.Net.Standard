using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using System.IO;

namespace XamForms.Droid
{
    [Activity (Label = "XamForms", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			global::Xamarin.Forms.Forms.Init (this, bundle);


            var app = new XamForms.App();
            app.DBPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "stocks.db");
            app.SQLitePlatform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();

            LoadApplication (app);
		}
	}
}

