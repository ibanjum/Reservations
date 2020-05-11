using System.Collections.Generic;
using Xamarin.Forms;
using Constructivity.Access;
using System.Reflection;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using System.Linq;

namespace cvtandroid
{
    public partial class ProjectContent : ContentPage
    {
        Organization _org;
        Constructivity.Core.Library _lib;

        public ProjectContent(Organization org, Constructivity.Core.Library lib)
        {
            InitializeComponent();

            if (org == null || lib == null)
                return;

            _org = org;
            _lib = lib;

            StackLayout titleview = Graphics.GetTitleView(_org.Name, _lib.Name);
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            PropertyInfo[] properties = lib.GetType().GetProperties();
            var products = new List<object>();
            foreach(PropertyInfo prop in properties)
            {
                products.Add(new { prop.Name });
            }

            this.ListViewproducts.ItemsSource = products;
            this.ListViewproducts.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
        private async void OnListViewProductItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            object ob = e.Item as object;
            string prodName = ob.GetType().GetProperty("Name").GetValue(ob).ToString();
            object prod = _lib.GetType().GetProperty(prodName).GetValue(_lib);
            await Navigation.PushAsync(new ProductContent(_org, _lib, prod, prodName));
        }
    }
}
