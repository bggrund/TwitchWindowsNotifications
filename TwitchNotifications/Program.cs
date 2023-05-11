using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Threading;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Toolkit.Uwp.Notifications;

namespace TwitchNotifications
{
    class Program
    {
        static List<WatcherData> data;
        static string access_token;
        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            //List<WatcherData> temp = new List<WatcherData>();
            //temp.Add(new WatcherData() { channelName = "BarbarousKing", categoryContains = new List<string>() { "dungeon", "marbles" }, titleContains = new List<string>(new string[] { "dark", "marbles" }) });
            //string s = JsonSerializer.Serialize(temp);

            try
            {
                data = JsonSerializer.Deserialize<List<WatcherData>>(File.ReadAllText("config.json"));
            }
            catch(Exception e)
            {
                Console.WriteLine("Invalid config.json file");
                Console.WriteLine(e.Message);
                MinimizeTray.ShowConsole();
                Console.ReadKey();
            }

            Task.Factory.StartNew(MinimizeTray.MinimizeToTray);

            Task.Factory.StartNew(MainAsync);

            while (true)
            {
                if (Console.ReadKey().Key == ConsoleKey.Enter) { Console.WriteLine("Enter pressed"); }
                Thread.Sleep(1000);
            }
        }

        static async void MainAsync()
        {
            await GetAccessToken();
            GetNotifications();
        }

        //static async void GetData_default()
        //{
        //    while (true)
        //    {
        //        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/channels?broadcaster_id={user.data[0].id}");

        //        request.Headers.Add("Authorization", $"Bearer {access_token}");
        //        request.Headers.Add("Client-Id", "omjlejqc1z2abez03s6p5sdt2xmpf2");

        //        HttpResponseMessage response = await client.SendAsync(request);
        //        response.EnsureSuccessStatusCode();
        //        string responseBody = await response.Content.ReadAsStringAsync();

        //        ChannelResponse chan = JsonSerializer.Deserialize<ChannelResponse>(responseBody);

        //        Console.WriteLine(chan.data[0].game_name);

        //        if (string.Equals(chan.data[0].game_name, "Marbles on Stream", StringComparison.OrdinalIgnoreCase))
        //        {
        //            new ToastContentBuilder().AddText("Barb is streaming marbles!").Show();
        //        }
        //        if (chan.data[0].title.ToLower().Replace(" ", "").Contains("marbles"))
        //        {
        //            new ToastContentBuilder().AddText("Barb has marbles in title!").Show();
        //        }

        //        Thread.Sleep(20000);
        //    }
        //}

        static async Task GetAccessToken()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");
            request.Content = new StringContent($"client_id=omjlejqc1z2abez03s6p5sdt2xmpf2&client_secret={Token.secret}&grant_type=client_credentials");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            TokenResponse tok = JsonSerializer.Deserialize<TokenResponse>(responseBody);
            
            access_token = tok.access_token;
        }

        static async Task<string> GetID(string channelName)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/users?login={channelName}");

            request.Headers.Add("Authorization", $"Bearer {access_token}");
            request.Headers.Add("Client-Id", "omjlejqc1z2abez03s6p5sdt2xmpf2");

            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            UserResponse user = JsonSerializer.Deserialize<UserResponse>(responseBody);

            return user.data[0].id;
        }

        static async void GetNotifications()
        {
            while (true)
            {
                foreach (WatcherData w in data)
                {
                    string id = await GetID(w.channelName);

                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://api.twitch.tv/helix/channels?broadcaster_id={id}");

                    request.Headers.Add("Authorization", $"Bearer {access_token}");
                    request.Headers.Add("Client-Id", "omjlejqc1z2abez03s6p5sdt2xmpf2");

                    HttpResponseMessage response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    ChannelResponse chan = JsonSerializer.Deserialize<ChannelResponse>(responseBody);

                    ChannelResponseData chanData = chan.data[0];

                    Console.WriteLine($"{w.channelName} was last streaming {chanData.game_name} -- \"{chanData.title}\"");

                    foreach (string s in w.titleContains)
                    {
                        if (chanData.title.ToLower().Replace(" ", "").Contains(s.ToLower().Replace(" ", "")))
                        {
                            new ToastContentBuilder().AddText($"{w.channelName} has title \"{chanData.title}\"!").Show();
                        }
                    }

                    foreach (string s in w.categoryContains)
                    {
                        if (chanData.game_name.ToLower().Replace(" ", "").Contains(s.ToLower().Replace(" ", "")))
                        {
                            new ToastContentBuilder().AddText($"{w.channelName} is streaming {chanData.game_name}!").Show();
                        }
                    }

                    //if (string.Equals(chan.data[0].game_name, "Marbles on Stream", StringComparison.OrdinalIgnoreCase))
                    //{
                    //    new ToastContentBuilder().AddText("Barb is streaming marbles!").Show();
                    //}
                    //if (chan.data[0].title.ToLower().Replace(" ", "").Contains("marbles"))
                    //{
                    //    new ToastContentBuilder().AddText("Barb has marbles in title!").Show();
                    //}
                }

                // Check every 20 seconds
                Thread.Sleep(20000);
            }
        }
    }
    
    //class WatcherData
    //{
    //    public List<WatcherChannelData> data;
    //}
    class WatcherData
    {
        public string channelName { get; set; }
        public List<string> titleContains { get; set; }
        public List<string> categoryContains { get; set; }
    }

    class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
    }

    class UserResponseData
    {
        public string id { get; set; }
    }
    class UserResponse
    {
        public List<UserResponseData> data { get; set; }
    }
    class ChannelResponseData
    {
        public string game_name { get; set; }
        public string title { get; set; }
    }
    class ChannelResponse
    {
        public List<ChannelResponseData> data { get; set; }
    }
}
