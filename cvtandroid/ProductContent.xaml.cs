using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Constructivity.Core;
using Xamarin.Forms;
using Constructivity.Access;
using System.Collections;

namespace cvtandroid
{
    public partial class ProductContent : ContentPage
    {
        Organization _org;
        Library _lib;
        object _prod;

        public ProductContent(Organization org, Library lib, object prod, string prodName)
        {
            InitializeComponent();

            if (lib == null || prod == null || prodName == null)
                return;

            _org = org;
            _lib = lib;
            _prod = prod;

            
            StackLayout titleview = Graphics.GetTitleView(_org.Name, prodName);
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            this.ListViewDefinition.ItemsSource = (IEnumerable)prod;
            this.ListViewDefinition.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
        private async void OnListViewDefinitionItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            await Navigation.PushAsync(new InstanceTab(_org, _lib, _prod, e.Item));
        }
    }
}
        