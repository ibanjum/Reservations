using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Constructivity.Core;
using Xamarin.Forms;
using Constructivity.Access;
using System.Collections;
using System.Reflection;
using System.Linq;

namespace cvtandroid
{
    public partial class ProductContent : ContentPage
    {
        public Organization _org = null;
        public static Constructivity.Core.Library _lib = null;
        public object _prod = null;

        public ProductContent(Organization org, Constructivity.Core.Library lib, object prod, string prodName)
        {
            InitializeComponent();

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
            await Navigation.PushAsync(new InstanceTab(_org, _lib, _prod, e.Item));
        }
    }
}
        