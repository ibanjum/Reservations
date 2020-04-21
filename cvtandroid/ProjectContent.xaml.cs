using System;
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
        public Organization _org = null;
        public Constructivity.Core.Library _lib = null;

        public ProjectContent(Organization org, Constructivity.Core.Library lib)
        {
            InitializeComponent();

            _org = org;
            _lib = lib;

            StackLayout titleview = Graphics.GetTitleView(_org.Name, _lib.Name);
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            PropertyInfo[] properties = lib.GetType().GetProperties();
            List<string> products = properties.Select(x => x.Name).ToList();

            this.ListViewproducts.ItemsSource = products;
            this.ListViewproducts.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);
        }
        private async void OnListViewProductItemTapped(object sender, ItemTappedEventArgs e)
        {
            string prodName = e.Item as string;
            object prod = _lib.GetType().GetProperty(prodName).GetValue(_lib);

            await Navigation.PushAsync(new ProductContent(_org, _lib, prod, prodName));
        }
    }
}
