using System;
using CoinMarketCap.Entities;

namespace coinmarketcapBot.Models
{
    public class Coin
    {
        public TickerEntity Ticker { get; private set; }
        public float Count { get; private set; }
        public MyDateTime Time { get; private set; }

        public Coin(ulong c, TickerEntity t)
        {
            Count = c;
            Ticker = t;
            Time = new MyDateTime();
        }

        public Coin(string s, TickerEntity t)
        {
            float value = 0;
            if (float.TryParse(s, out value))
            {
                Count = value;
                Ticker = t;
                Time = new MyDateTime();
            }
        }

    }

}