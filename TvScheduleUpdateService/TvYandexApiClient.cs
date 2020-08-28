using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TvSchedule.Data.Data;
using TvScheduleUpdateService.YandexScheduleResponseModels;
using UAParser;

namespace TvScheduleUpdateService
{
    class TvYandexApiClient
    {
        private string _sessionKey;

        protected string SessionKey
        {
            get
            {
                if (_sessionKey is null)
                {
                    var page = GetMainPage().Result;

                    _sessionKey = GetSK(page);
                }

                return _sessionKey;
            }
            set => _sessionKey = value;
        }

        private HttpClient Client { get; set; }

        public TvYandexApiClient()
        {
            Client = new HttpClient();

            string uaString =
                "Mozilla/5.0(Windows NT 10.0; Win64; x64) AppleWebKit/537.36(KHTML, like Gecko) Chrome/80.0.3987.163 Safari/537.36";

            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", uaString);
        }




        public static string GetSK(string data)
        {
            var regex = new Regex(
                "window.__INITIAL_SK__\\s*=\\s*\\{\"key\":\"(?<json>.*?)\",\"expire\":(?<exp>\\d+)\\};");

            var match = regex.Match(data);

            if (!match.Success)
            {
                throw new ApplicationException("no matches");
            }

            return match.Groups[1].Value;
        }

        public async Task<YandexScheduleResponse> GetSchedule(DateTime date)
        {
            var apiUrl = $"https://tv.yandex.ru/api/39?date={date:yyyy-MM-dd}&period=all-day";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("X-TV-SK", SessionKey);
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(string.Empty);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await Client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();
            var ysrObj = JsonConvert.DeserializeObject<YandexScheduleResponse>(result);

            return ysrObj;
        }


        public async Task<TvChannelNode> GetSchedule(DateTime date, int apiChannelId) 
        {
            var apiUrl =
                $"https://tv.yandex.ru/api/39?date={date:yyyy-MM-dd}&period=all-day";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);

            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            request.Headers.Add("X-TV-SK", SessionKey);
            request.Headers.Add("Accept", "application/json");
            request.Content = new StringContent(string.Empty);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

            var response = await Client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string result = await response.Content.ReadAsStringAsync();

            var ysrObj = JsonConvert.DeserializeObject<YandexScheduleResponse>(result);

            TvChannelNode channelNode = null;
            foreach (var item in ysrObj.schedule.schedules)
            {
                if (item.channel.id == apiChannelId)
                {
                    channelNode = item;
                    break;
                }
            }

            return channelNode;
        }


        public async Task<string> GetMainPage()
        {
            await Client.GetStringAsync($"https://tv.yandex.ru");
            return await Client.GetStringAsync($"https://tv.yandex.ru/?date={DateTime.UtcNow:yyyy-MM-dd}&period=all-day");
        }


        public static void TvShowsDataContext_GetList()
        {
            var items = TvShowsDataContext.Instance.GetList(100, SqlFilter.Gt("UpdatedUtc", DateTime.UtcNow.AddHours(-2)));
            Console.WriteLine(items.ToString());
        }
    }
}
