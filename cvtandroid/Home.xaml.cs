using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            if(item.Title == "Log Out")
            {
                bool answer = await DisplayAlert("Confirm", "Are you sure you want to log out?", "Yes", "No");
                if (answer)
                {
                    Application.Current.Properties.Clear();
                    Application.Current.MainPage = new NavigationPage(new MainPage());
                    return;
                }
            }

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }
    }
}
