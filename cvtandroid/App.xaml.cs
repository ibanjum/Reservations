using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cvtandroid
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage())
            {
                BarTextColor = Color.Goldenrod
            };
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
