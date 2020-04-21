using Newtonsoft.Json;
using Xamarin.Forms;
using Constructivity.Access;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Reflection;
using System;

namespace cvtandroid
{
    public partial class OrganizationContent : ContentPage
    {
        public Organization _org = null;

        public OrganizationContent(Organization organization)
        {
            InitializeComponent();
            _org = organization;
            StackLayout titleview = Graphics.GetTitleView(_org.Name, _org.Name);
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            this.ListViewLibraries.ItemsSource = organization.Libraries;
            this.ListViewLibraries.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
        protected async void OnListViewLibrariesItemTapped(object sender, ItemTappedEventArgs e)
        {
            LibraryReference libRef = e.Item as LibraryReference;

            Constructivity.Core.Library lib = DataController.GetLibrary(libRef.Name);
            if (lib == null)
            {
                lib = DataController.CallLibraryApi(_org.Name, libRef.Name);
            }
            await Navigation.PushAsync(new ProjectContent(_org, lib));
        }

    }
}
