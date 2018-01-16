using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoBot.Models
{
    public class Binance
    {
        public string prevClosePrice { get; set; }

        public string lastPrice { get; set; }

        public string priceChangePercent { get; set; }

        public string weightedAvgPrice { get; set; }
    }
}
