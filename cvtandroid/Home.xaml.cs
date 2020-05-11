using System;
using Constructivity.Access;
using MonkeyCache.FileStore;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace cvtandroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : MasterDetailPage
    {
        public Home()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as HomeMenuItem;
            if (item == null)
                return;

            if(item.Title == "Sign Out")
            {
                bool answer = await DisplayAlert("Confirm", "Are you sure you want to sign out?", "Yes", "No");
                if (answer)
                {
                    Barrel.Current.EmptyAll();
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                    return;
                }
            }
            if(item.Title == "Terms")
            {
                await Browser.OpenAsync("https://www.constructivity.com/Terms", BrowserLaunchMode.SystemPreferred);
            }

            MasterPage.ListView.SelectedItem = null;
        }
    }
}
