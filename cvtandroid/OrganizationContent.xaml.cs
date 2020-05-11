using Xamarin.Forms;
using Constructivity.Access;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace cvtandroid
{
    public partial class OrganizationContent : ContentPage
    {
        Organization _org;

        public OrganizationContent(Organization organization)
        {
            InitializeComponent();

            if (organization == null)
                return;

            _org = organization;
            StackLayout titleview = Graphics.GetTitleView(_org.Name, _org.Name);
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            this.ListViewLibraries.ItemsSource = organization.Libraries;
            this.ListViewLibraries.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
        protected async void OnListViewLibrariesItemTapped(object sender, ItemTappedEventArgs e)
        {
            LibraryReference libRef = e.Item as LibraryReference;

            if (libRef == null)
                return;

            Constructivity.Core.Library lib = DataController.GetLibrary(_org.Name, libRef.Name);
            if (lib == null)
            {
                await DisplayAlert("Error", "Could not contact the server, please try again later.", "OK");
                return;
            }

            await Navigation.PushAsync(new ProjectContent(_org, lib));
        }

    }
}
