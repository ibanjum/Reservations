using Constructivity.Access;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace cvtandroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserActivity : Xamarin.Forms.TabbedPage
    {
        public UserActivity()
        {
            InitializeComponent();

            SelectedTabColor = Color.Black;
            UnselectedTabColor = Color.Gray;
            BarBackgroundColor = Color.Goldenrod;

        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            User user = DataController.GetUser();
            if (user == null)
            {
                await DisplayAlert("Error", "Could not contact the server, please try again later.", "OK");
                return;
            }
            this.ListViewOrganizations.ItemsSource = user.Organizations;
            this.ListViewProjects.ItemsSource = user.Libraries;
            Title = user.Name.ToUpper();

            this.ListViewOrganizations.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
            this.ListViewProjects.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);

            //update user with icons in the background
            List<CustomOrganization> orglist = new List<CustomOrganization>();
            List<CustomLibrary> liblist = new List<CustomLibrary>();
            await DataController.GetIcons(user.Organizations, user.Libraries, orglist, liblist, ListViewOrganizations, ListViewProjects);
        }
        protected async void OnListViewOrganizationItemTapped(object sender, ItemTappedEventArgs e)
        {
            OrganizationReference orgRef = e.Item as OrganizationReference;

            if (orgRef == null)
                return;

            Organization organization = DataController.GetOrganization(orgRef.Name);
            if (organization == null)
            {
                await DisplayAlert("Error", "Could not contact the server, please try again later.", "OK");
                return;
            }

            await Navigation.PushAsync(new OrganizationContent(organization));
        }
        protected async void OnListViewProjectItemTapped(object sender, ItemTappedEventArgs e)
        {
            LibraryReference reference = e.Item as LibraryReference;

            if (reference == null)
                return;

            string[] split = reference.Name.Split('_');

            string orgName = split[0];
            Organization org = DataController.GetOrganization(orgName);
            if (org == null)
            {
                await DisplayAlert("Error", "Could not contact the server, please try again later.", "OK");
                return;
            }

            string proName = split[1];
            Constructivity.Core.Library lib = DataController.GetLibrary(orgName, proName);
            if (lib == null)
            {
                await DisplayAlert("Error", "Could not contact the server, please try again later.", "OK");
                return;
            }

            await Navigation.PushAsync(new ProjectContent(org, lib));
        }
        public class CustomOrganization : OrganizationReference
        {
            public ImageSource Source { get; set; }
        }
        public class CustomLibrary : LibraryReference
        {
            public ImageSource Source { get; set; }
        }
        public class CustomUser
        {
            public string Name { get; set; }
            public Guid UUID { get; set; }
            public CustomOrganization[] Organizations { get; set; }
            public CustomLibrary[] Libraries { get; set; }
        }
    }
}
