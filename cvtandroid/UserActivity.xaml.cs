using Constructivity.Access;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace cvtandroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserActivity : Xamarin.Forms.TabbedPage
    {
        public UserActivity()
        {
            InitializeComponent();

            User user = DataController.GetUser();
            if (user == null)
            {
                user = DataController.CallUserApi();
            }
            Title = user.Name;

            OrganizationReference[] organizations = user.Organizations;
            List<CustomOrganization> newlist = new List<CustomOrganization>();

            this.ListViewOrganizations.ItemsSource = organizations;
            this.ListViewOrganizations.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);

            LibraryReference[] projects = user.Libraries;
            this.ListViewProjects.ItemsSource = projects;
            this.ListViewProjects.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);

            DataController.CallOrgIconApi(organizations, newlist, ListViewOrganizations);
        }
        private async void OnListViewOrganizationItemTapped(object sender, ItemTappedEventArgs e)
        {
            OrganizationReference orgRef = e.Item as OrganizationReference;

            Organization organization = DataController.GetOrganization(orgRef.Name);
            if(organization == null)
            {
                organization = DataController.CallOrganizationApi(orgRef.Name);
            }
            await Navigation.PushAsync(new OrganizationContent(organization));
        }
        public class CustomOrganization : OrganizationReference
        {
            public ImageSource Source { get; set; }
        }
    }
}
