using System;
using MonkeyCache.FileStore;
using Xamarin.Forms;

namespace cvtandroid
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            if (Application.Current.Properties.ContainsKey("UserId"))
            {
                //Load if Logged In
                string UserId = Application.Current.Properties["UserId"] as string;
                Barrel.ApplicationId = "Constructivity" + UserId;
                if (Barrel.Current.Exists(key: "token") && !Barrel.Current.IsExpired(key: "token"))
                {
                    MainPage = new NavigationPage(new Home());
                }
                else
                {
                    MainPage = new NavigationPage(new MainPage());
                }
            }
            else
            {
                //Load if Not Logged In
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
