using System;
using System.Collections.Generic;
using Urho.Forms;

using Xamarin.Forms;

namespace cvtandroid
{
    public partial class UrhoSurface : ContentPage
    {
        public UrhoSurface()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            urhoSurface.Show<urhoApp>(new Urho.ApplicationOptions(assetsFolder: null) { Orientation = Urho.ApplicationOptions.OrientationType.LandscapeAndPortrait });
        }
    }
}
