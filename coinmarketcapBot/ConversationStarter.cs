using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using coinmarketcapBot.Models;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace coinmarketcapBot
{
    //todo: need too rename
    public class ConversationStarter
    {
        //Note: Of course you don't want these here. Eventually you will need to save these in some table
        //Having them here as static variables means we can only remember one user :)
        public static string fromId;
        public static string fromName;
        public static string toId;
        public static string toName;
        public static string serviceUrl;
        public static string channelId;
        public static string conversationId;

        public static int percent;
        private static float money;
        public static Dictionary<string,List<Coin>> Portfolio = new Dictionary<string,List<Coin>>();


        public static bool ToPortfolio(string s,Coin c)
        {
            if (!Portfolio.ContainsKey(s))
            {
                Portfolio.Add(s, new List<Coin>(){c});
                return true;
            }

            float sum = 0;
            //todo: replace to for
            foreach (Coin coin in Portfolio[s])
            {
                sum += coin.Count;
            }
            if (sum - c.Count < 0)
                return false;

            Portfolio[s].Add(c);
            return true;
        }

        //This will send an adhoc message to the user
        public static async Task Resume(string conversationId, string channelId)
        {
            float sum1 = 0;

            //todo: replace to for
            foreach (string s in Portfolio.Keys)
            {
                
                float sum2 = 0;
                foreach (Coin coin in Portfolio[s])
                {
                    sum2 += coin.Count;
                }
                sum1 += sum2 * (float)Portfolio[s][0].Ticker.MarketCapUsd.Value;
            }
            if ((money * percent) + money < sum1)
            {
                money = sum1;
                Console.WriteLine("timerEvent false");
                return;
                
            }
            money = sum1;

            var userAccount = new ChannelAccount(toId, toName);
            var botAccount = new ChannelAccount(fromId, fromName);
            var connector = new ConnectorClient(new Uri(serviceUrl));

            IMessageActivity message = Activity.CreateMessageActivity();
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(channelId))
            {
                message.ChannelId = channelId;
            }
            else
            {
                conversationId = (await connector.Conversations.CreateDirectConversationAsync(botAccount, userAccount)).Id;
            }

            message.From = botAccount;
            message.Recipient = userAccount;
            message.Conversation = new ConversationAccount(id: conversationId);
            message.Text = $"Your portfolio grows more then {percent} percent";
            message.Locale = "en-Us";
            await connector.Conversations.SendToConversationAsync((Activity)message);
        }
    }
}