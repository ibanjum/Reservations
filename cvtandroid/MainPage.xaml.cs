using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Xml;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace cvtandroid
{

    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }
        private async void Login_Pressed(object sender, EventArgs e)
        {
            string url = "https://www.constructivity.com/";

            string jsonTokenObject = getToken(url, UserId.Text, Password.Text);
            var TokenObject = JsonConvert.DeserializeObject<Token>(jsonTokenObject);

            if(!String.IsNullOrEmpty(TokenObject.error_description))
            {
                Error.Text = TokenObject.error_description;
                Error.IsVisible = true;
                return;
            }
            else
            {
                Application.Current.Properties["token"] = TokenObject.access_token;
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
