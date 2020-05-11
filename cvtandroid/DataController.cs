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
using MonkeyCache.FileStore;
using Xamarin.Essentials;

namespace cvtandroid
{
    public static class DataController
    {
        private static byte[] GetBytes(string endpoint, out string err)
        {
            err = null;
            if (!Barrel.Current.IsExpired(key: "token"))
            {
                Token token = Barrel.Current.Get<Token>(key: "token");

                HttpResponseMessage response;
                if (Barrel.Current.Exists(endpoint))
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        return Barrel.Current.Get<byte[]>(key: endpoint);

                    response = makeRequest(token, endpoint, true);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;

                        if (!Barrel.Current.IsExpired(key: endpoint))
                            Barrel.Current.Empty(key: endpoint);

                        Barrel.Current.Add(key: endpoint, data: bytes, TimeSpan.FromDays(1));
                        return bytes;
                    }
                    else if (response.StatusCode == HttpStatusCode.NotModified)
                        return Barrel.Current.Get<byte[]>(key: endpoint);
                    else
                        err = "Unable to contact server.";
                }
                response = makeRequest(token, endpoint, false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    byte[] bytes = response.Content.ReadAsByteArrayAsync().Result;
                    if (!Barrel.Current.IsExpired(key: endpoint))
                        Barrel.Current.Empty(key: endpoint);

                    Barrel.Current.Add(key: endpoint, data: bytes, TimeSpan.FromDays(1));
                    return bytes;
                }
                else
                    err = "Unable to contact server.";
            }
            return null;
        }
        private static string GetJson(string endpoint, out string err)
        {                     
            err = null;
            if (!Barrel.Current.IsExpired(key: "token"))
            {
                Token token = Barrel.Current.Get<Token>(key: "token");

                HttpResponseMessage response;
                if (Barrel.Current.Exists(endpoint))
                {
                    if (Connectivity.NetworkAccess != NetworkAccess.Internet)
                        return Barrel.Current.Get<string>(key: endpoint);

                    response = makeRequest(token, endpoint, true);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;

                        if (!Barrel.Current.IsExpired(key: endpoint))
                            Barrel.Current.Empty(key: endpoint);

                        Barrel.Current.Add(key: endpoint, data: json, TimeSpan.FromDays(1));
                        return json;
                    }
                    else if (response.StatusCode == HttpStatusCode.NotModified)
                        return Barrel.Current.Get<string>(key: endpoint);
                    else
                        err = "Unable to contact server.";
                }
                response = makeRequest(token, endpoint, false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string json = response.Content.ReadAsStringAsync().Result;

                    if (!Barrel.Current.IsExpired(key: endpoint))
                        Barrel.Current.Empty(key: endpoint);

                    Barrel.Current.Add(key: endpoint, data: json, TimeSpan.FromDays(1));
                    return json;
                }
                else
                    err = "Unable to contact server.";
            }
            return null;
        }
        private static HttpResponseMessage makeRequest(Token token, string endpoint, bool cached)
        {
            string baseURL = "https://www.constructivity.com/api/";

            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            using (var client = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token.access_token))
                {
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.access_token);
                    if (!Barrel.Current.IsExpired(key: "LastModified"))
                        client.DefaultRequestHeaders.Add("If-Modified-Since", Barrel.Current.Get<DateTimeOffset>(key: "LastModifed").ToString());
                    else
                        client.DefaultRequestHeaders.Add("test", DateTimeOffset.Now.ToUniversalTime().ToString("r"));

                    if (cached == true)
                    {
                        if (endpoint == "login/json")
                        {
                            string UserId = Application.Current.Properties["UserId"] as string;
                            client.DefaultRequestHeaders.Add("UserId", UserId);
                        }
                        else
                            client.DefaultRequestHeaders.Add("endpoint", endpoint);
                    }
                }
                var response = client.GetAsync(baseURL + endpoint).Result;
                DateTimeOffset lastmodified = response.Content.Headers.LastModified.GetValueOrDefault();
                Barrel.Current.Add(key: "LastModified", lastmodified, TimeSpan.FromDays(1));
                return response;
            }
        }
        public static User GetUser()
        {
            string err;
            string json = GetJson("login/json", out err);
            if (err != null)
                return null;

            User user = JsonConvert.DeserializeObject<User>(json);
            return user;
        }
        public static Organization GetOrganization(string identifier)
        {
            string err;
            string json = GetJson("data/json/" + identifier, out err);
            if(err != null)
                return null;

            Organization org = JsonConvert.DeserializeObject<Organization>(json);
            return org;
        }
        public static Constructivity.Core.Library GetLibrary(string orgIdentifier, string libIdentifer)
        {
            string err;
            string json = GetJson("data/json/" + orgIdentifier + "/" + libIdentifer, out err);
            if (err != null)
                return null;

            Constructivity.Core.Library lib = JsonConvert.DeserializeObject<Constructivity.Core.Library>(json);
            return lib;
        }
        public static async Task GetIcons
            (OrganizationReference[] organizations,
            LibraryReference[] libraries,
            List<CustomOrganization> orglist,
            List<CustomLibrary> liblist,
            ListView listVieworgs,
            ListView listViewlibs)
        {
            ImageSource source = null;

            for (int i = 0; i < organizations.Length; i++)
            {
                string name = organizations[i].Name;
                string iconname = organizations[i].Name + "icon";

                string err;
                byte[] data = GetBytes("data/png/" + name, out err);
                if (err != null)
                    break;

                source = ImageSource.FromStream(() => new MemoryStream(data));
                orglist.Add(new CustomOrganization { Name = name, Source = source });
                SetIcon(name, source);
            }
            for (int i = 0; i < libraries.Length; i++)
            {
                string[] split = libraries[i].Name.Split('_');
                string orgName = split[0];
                string libName = split[1];
                string name = libraries[i].Name;
                string iconname = libraries[i].Name + "icon";

                string err;
                byte[] data = GetBytes("data/png/" + name, out err);
                if (err != null)
                    break;

                source = ImageSource.FromStream(() => new MemoryStream(data));
                liblist.Add(new CustomLibrary { Name = name, Source = source });
                SetIcon(name, source);
            }
            listVieworgs.ItemsSource = orglist;
            listViewlibs.ItemsSource = liblist;
        }
        public static void SetIcon(string name, ImageSource source)
        {
            if (name != null && source != null)
            {
                if (!Barrel.Current.IsExpired(key: name))
                {
                    Barrel.Current.Empty(key: name);
                }
                Barrel.Current.Add(key: name, data: source, TimeSpan.FromDays(1));
            }
        }
        public static ImageSource GetIcon(string name)
        {
            if (!Barrel.Current.IsExpired(key: name))
            {
                return Barrel.Current.Get<ImageSource>(key: name);
            }
            return null;
        }
    }
}
