using System.Net.Http;
using System.Web.Configuration;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using coinmarketcapBot.Models;
using CoinMarketCap;
using CoinMarketCap.Entities;

namespace coinmarketcapBot.Dialogs
{
    [Serializable]
    public static class CoinMarketCapService
    {
        private static CoinMarketCapClient client = CoinMarketCapClient.GetInstance();

        private static List<TickerEntity> TickersList=null;

        public static async Task<TickerEntity> GetTicker(string name)
        {
            try
            {
                var byname = await GetByName(name);
                return byname;
            }
            catch (Exception e)
            {
                if (e.HResult == -2146233088)
                {
                    
                    var byid= await GetBySymbol(name);
                    return byid;
                }
                Console.WriteLine(e);
                throw;
            }
        }

        private static async Task<TickerEntity> GetBySymbol(string Symbol)
        {
            try
            {
                if (TickersList == null)
                {
                    TickersList = await client.GetTickerListAsync();
                }
                var ticker = TickersList.FirstOrDefault(i => string.Equals(i.Symbol , Symbol, StringComparison.OrdinalIgnoreCase));
                return await GetByName(ticker?.Name);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new No​​Currency​​FoundException();
            }
        }

        private static async Task<TickerEntity> GetByName(string name)
        {
            return await client.GetTickerAsync(name);
        }
    }
}