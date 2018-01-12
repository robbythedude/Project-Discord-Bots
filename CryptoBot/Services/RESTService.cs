using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Models;
using Newtonsoft.Json;

namespace CryptoBot.Services
{
    public class RESTService
    {
        private static HttpClient client = new HttpClient();

        public static async Task<string> GetPrice(string crypto)
        {
            HttpResponseMessage response = await client.GetAsync($"https://api.coinmarketcap.com/v1/ticker/{crypto}/");

            if (response.IsSuccessStatusCode)
            {
                string jsonResult = await response.Content.ReadAsStringAsync();

                List<CMC> deserializedObj = JsonConvert.DeserializeObject<List<CMC>>(jsonResult);
                CMC coin = deserializedObj[0];
                return coin.price_usd;
            }
            return "Put in a real coin you jagoff";
        }
    }
}
