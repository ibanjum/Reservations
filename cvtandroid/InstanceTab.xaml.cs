using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Constructivity.Access;
using Constructivity.Core;
namespace cvtandroid
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InstanceTab : Xamarin.Forms.TabbedPage
    {
        Organization _org;
        Library _lib;
        object _prod;
        object _instance;

        public InstanceTab(Organization org, Library lib, object prod, object instance)
        {
            InitializeComponent();

            _org = org;
            _lib = lib;
            _prod = prod;
            _instance = instance;

            StackLayout titleview = Graphics.GetTitleView(_org.Name, _instance.ToString());
            Xamarin.Forms.NavigationPage.SetTitleView(this, titleview);

            PropertyInfo[] instanceproperties = instance.GetType().GetProperties();
            // load up instances
            var list = new List<object>();
            object valueobject = null;
            Type valuetype = null;
            foreach (PropertyInfo prop in instanceproperties)
            {
                if (prop.GetIndexParameters().Length == 0 && prop.Name != "Name" && prop.Name != "UUID")
                {
                    valueobject = prop.GetValue(instance);
                    if (valueobject != null)
                    {
                        valuetype = valueobject.GetType();
                        if (valuetype.IsArray)
                        {
                            int count = ((Array)valueobject).Length;
                            list.Add(new { prop.Name, Count = count, Valueobject = valueobject });
                        }
                        else
                        {
                            list.Add(new { prop.Name, Value = valueobject });
                        }
                    }
                }
            }
            this.ListViewInstances.ItemsSource = list;
            this.ListViewInstances.On<iOS>().SetSeparatorStyle(SeparatorStyle.FullWidth);

           if(_instance.GetType() == typeof(Product) || _instance.GetType() == typeof(Mesh))
            {
                this.UrhoButton.IsVisible = true;
            }
            else
            {
                this.CanvasView.PaintSurface += OnCanvasViewPaintSurface;
                ViewPage.Content = this.CanvasView;
            }
        }
        protected void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            // draw svg
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            Graphics.CreateSVG(_instance, canvas, info, 50, 50);
        }
        protected void OnListViewPropertiesItemTapped(object sender, ItemTappedEventArgs e)
        {
            object Value = e.Item as object;
            PropertyInfo valueobject = Value.GetType().GetProperty("Valueobject");
            if (valueobject != null)
            {
                object instancelist = valueobject.GetValue(Value);
                string title = Value.GetType().GetProperty("Name").GetValue(Value).ToString() + " (" + ((Array)instancelist).Length + ")";
                Navigation.PushAsync(new InstanceList(_org, _lib, _prod, _instance, instancelist, title));
            }
        }
        protected async void UrhoClicked(object sender, EventArgs e)
        {
            if(_instance.GetType() == typeof(Product) || _instance.GetType() == typeof(Mesh))
            {
                Xamarin.Forms.Application.Current.Properties["Meshes"] = _instance;
                await Navigation.PushAsync(new UrhoSurface());
            }
        }
    }
}
