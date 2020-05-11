using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using Xamarin.Forms;
using System.Net;
using Newtonsoft.Json;
using Xamarin.Essentials;
using MonkeyCache.FileStore;

namespace cvtandroid
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();

            var tapForgotPassword = new TapGestureRecognizer();
            tapForgotPassword.Tapped += async (s, e) => {
                await Browser.OpenAsync("https://www.constructivity.com/Account/Forgot", BrowserLaunchMode.SystemPreferred);
            };
            LabelForgotPassword.GestureRecognizers.Add(tapForgotPassword);

            var tapRegister= new TapGestureRecognizer();
            tapRegister.Tapped += async (s, e) => {
                await Browser.OpenAsync("https://www.constructivity.com/Account/Register", BrowserLaunchMode.SystemPreferred);
            };
            LabelRegister.GestureRecognizers.Add(tapRegister);
        }
        private async void Login_Pressed(object sender, EventArgs e)
        {
            string url = "https://www.constructivity.com/";
            string jsonTokenObject = getToken(url, UserId.Text, Password.Text);
            try
            {
                var TokenObject = JsonConvert.DeserializeObject<Token>(jsonTokenObject);
                Application.Current.Properties["UserId"] = UserId.Text;
                await Application.Current.SavePropertiesAsync();

                Barrel.ApplicationId = "Constructivity" + UserId.Text;
                Barrel.Current.Add(key: "token", data: TokenObject, TimeSpan.FromDays(1));
            }
            catch
            {
                Error.Text = "Invalid Username or Password.";
                Error.IsVisible = true;
                return;
            }
            await Navigation.PushAsync(new Home());
         }
        private string getToken(string url, string UserName, string Password)
        {
            var pairs = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>( "grant_type", "password" ),
                        new KeyValuePair<string, string>( "username", UserName ),
                        new KeyValuePair<string, string> ( "Password", Password )
                    };
            var content = new FormUrlEncodedContent(pairs);
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

            using (var client = new HttpClient())
            {
                var response = client.PostAsync(url + "Token", content).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
