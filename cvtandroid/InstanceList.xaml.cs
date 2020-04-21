using System;
using System.Collections;
using Constructivity.Access;
using System.Reflection;
using Xamarin.Forms;

namespace cvtandroid
{
    public partial class InstanceList : ContentPage
    {
        public Organization _org = null;
        public Constructivity.Core.Library _lib = null;
        public object _prod = null;
        public object _instance = null;
        public object _instancelist = null;

        public InstanceList(Organization org, Constructivity.Core.Library lib, object prod, object instance, object instancelist, string title)
        {
            InitializeComponent();

            _org = org;
            _lib = lib;
            _prod = prod;
            _instance = instance;
            _instancelist = instancelist;

            StackLayout titleview = Graphics.GetTitleView(_org.Name, title);
            NavigationPage.SetTitleView(this, titleview);

            var list = (IEnumerable)instancelist;
            StackLayout layout = new StackLayout();
            object valueobject = null;
            Type valuetype = null;
            Label name = null;
            int index = 0;
            Frame frame = null;

            foreach (var item in list)
            {
                StackLayout stackLayout = new StackLayout();
                if (item.GetType().IsClass)
                {
                    PropertyInfo[] properties = item.GetType().GetProperties();
                    foreach (PropertyInfo prop in properties)
                    {
                        valueobject = prop.GetValue(item);
                        if (prop.Name == "Name" && valueobject == null)
                        {
                            prop.SetValue(item, "--");
                            valueobject = prop.GetValue(item);
                        }
                        if (valueobject != null)
                        {
                            valuetype = valueobject.GetType();

                            if (valuetype.IsArray)
                            {
                                int count = ((Array)valueobject).Length;
                                name = new Label
                                {
                                    TextColor = Color.Black,
                                    Text = prop.Name + ":  " + count
                                };
                                stackLayout.Children.Add(name);
                            }
                            else
                            {
                                if (prop.Name == "Name")
                                {
                                    name = new Label
                                    {
                                        TextColor = Color.Black,
                                        Text = prop.GetValue(item).ToString(),
                                        FontAttributes = FontAttributes.Bold
                                    };
                                    BoxView boxview = new BoxView
                                    {
                                        Color = Color.Gray,
                                        HeightRequest = 1,
                                        HorizontalOptions = LayoutOptions.Fill
                                    };
                                    stackLayout.Children.Add(name);
                                    stackLayout.Children.Add(boxview);
                                }
                                else
                                {
                                    name = new Label
                                    {
                                        TextColor = Color.Black,
                                        Text = prop.Name + ":  " + prop.GetValue(item)
                                    };
                                    stackLayout.Children.Add(name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    name = new Label
                    {
                        TextColor = Color.Black,
                        Text = index + "-  " + item.ToString()
                    };
                    stackLayout.Children.Add(name);
                    index++;
                }
                stackLayout.Padding = 10;
                var tapGestureRecognizer = new TapGestureRecognizer();
                tapGestureRecognizer.Tapped += async (s, e) => {
                    await Navigation.PushAsync(new InstanceTab(_org, _lib, _prod, item));
                };
                frame = new Frame()
                {
                    BorderColor = Color.Gray,
                    CornerRadius = 10,
                    Margin = 8,
                    Padding = 8,
                    Content = stackLayout,
                    
                };
                frame.GestureRecognizers.Add(tapGestureRecognizer);
                layout.Children.Add(frame);
            }
            ScrollView scrollView = new ScrollView();
            scrollView.Content = layout;
            Content = scrollView;
        }
    }
}
