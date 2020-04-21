using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using static cvtandroid.UserActivity;
using Constructivity.Access;
using Newtonsoft.Json;
using Xamarin.Forms;
using System.Threading.Tasks;
using System;

namespace cvtandroid
{
    public static class DataController
    {
        public static User CallUserApi()
        {
            string url = "https://www.constructivity.com/api/login/json";
            string json = AuthorizedResponse(url);
            User user = JsonConvert.DeserializeObject<User>(json);
            SetUser(user);
            return GetUser();
        }
        public static Organization CallOrganizationApi(string orgName)
        {
            string url = "https://www.constructivity.com/api/data/json/" + orgName;
            string json = AuthorizedResponse(url);
            Organization organization = JsonConvert.DeserializeObject<Organization>(json);
            SetOrganization(organization);
            return GetOrganization(organization.Name);
        }
        public static async void CallOrgIconApi(OrganizationReference[] organizations, List<CustomOrganization> newlist, ListView listView)
        {
            ImageSource source = null;
            string token = Application.Current.Properties["token"].ToString();
            for (int i = 0; i < organizations.Length; i++)
            {
                string name = organizations[i].Name;
                string iconname = organizations[i].Name + "icon"; 
                string url = "https://www.constructivity.com/api/data/png/" + name;
                ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    }
                    byte[] bytes = await client.GetByteArrayAsync(url);
                    source = ImageSource.FromStream(() => new MemoryStream(bytes));
                }
                newlist.Add(new CustomOrganization { Name = name, Source = source });
                SetIcon(iconname, source);
            }
            listView.ItemsSource = newlist;
        }
        public static Constructivity.Core.Library CallLibraryApi(string orgName, string libName)
        {
            string url = "https://www.constructivity.com/api/data/json/" + orgName + "/" + libName;
            string json = AuthorizedResponse(url);
            Constructivity.Core.Library library = JsonConvert.DeserializeObject<Constructivity.Core.Library>(json);
            SetLibrary(library);
            return GetLibrary(library.Name);
        }
        private static string AuthorizedResponse(string url)
        {
            string token = Application.Current.Properties["token"].ToString();
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                {

                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                }
                var response = client.GetAsync(url).Result;
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        // Set Data from Api Call
        public static void SetUser(User user)
        {
            Application.Current.Properties["User"] = user;
        }
        public static void SetOrganization(Organization organization)
        {
            Application.Current.Properties[organization.Name] = organization;
        }
        public static void SetIcon(string name, ImageSource source)
        {
            Application.Current.Properties[name] = source;
        }
        public static void SetLibrary(Constructivity.Core.Library Library)
        {
            Application.Current.Properties[Library.Name] = Library;
        }

        // Get saved data
        public static User GetUser()
        {
            if (Application.Current.Properties.ContainsKey("User"))
            {
                return Application.Current.Properties["User"] as User;
            }
            return null;
        }
        public static Organization GetOrganization(string orgName)
        {
            if(Application.Current.Properties.ContainsKey(orgName))
            {
                return Application.Current.Properties[orgName] as Organization;
            }
            return null;
        }
        public static ImageSource GetIcon(string name)
        {
            if (Application.Current.Properties.ContainsKey(name))
            {
                return Application.Current.Properties[name] as ImageSource;
            }
            return null;
        }
        public static Constructivity.Core.Library GetLibrary(string libName)
        {
            if (Application.Current.Properties.ContainsKey(libName))
            {
                return Application.Current.Properties[libName] as Constructivity.Core.Library;
            }
            return null;
        }
    }
}
