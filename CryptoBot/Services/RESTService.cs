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

        public static async Task<string> GetBinancePrice(string crypto)
        {
            string USDTString = await GetUSDTPrice();
            double USDTPrice = Convert.ToDouble(USDTString);

            string cryptoUpper = crypto.ToUpper();

            if (cryptoUpper == "BTC")
            {
                return USDTPrice.ToString();
            }

            HttpResponseMessage response = await client.GetAsync($"https://api.binance.com/api/v1/ticker/24hr?symbol={cryptoUpper}BTC");

            if (response.IsSuccessStatusCode)
            {
                string jsonResult = await response.Content.ReadAsStringAsync();

                Binance coin = JsonConvert.DeserializeObject<Binance>(jsonResult);

                double cryptoPrice = Convert.ToDouble(coin.lastPrice);

                double USDPrice = cryptoPrice * USDTPrice;
                string USD = USDPrice.ToString();
                return USD;
            }
            return "Put in a real coin you jagoff";
        }


        public static async Task<string> GetUSDTPrice()
        {
            HttpResponseMessage responseUSDT = await client.GetAsync($"https://api.binance.com/api/v1/ticker/24hr?symbol=BTCUSDT");

            if (responseUSDT.IsSuccessStatusCode)
            {
                string jsonResult = await responseUSDT.Content.ReadAsStringAsync();

                Binance coin = JsonConvert.DeserializeObject<Binance>(jsonResult);
                return coin.lastPrice;
            }

            return "0";
        }
    }
}
