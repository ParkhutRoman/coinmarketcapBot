using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using coinmarketcapBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace coinmarketcapBot.Dialogs
{
    [LuisModel(Constants.LuisModelId, Constants.LuisSubscriptionKey)]
    [Serializable]
    public class CoinDialog : LuisDialog<object>
    {
        
        string pattern = @"\b(\d+)|[a-zA-Z]+";

        public CoinDialog()
        {

        }
        [NonSerialized]
        private Timer t;

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("Help")]
        public async Task Help(IDialogContext context, LuisResult result)
        {
            var message = context.MakeMessage();
            var attachment = GetHeroCard();

            message.Attachments.Add(attachment);
            await context.PostAsync(message);

            context.Wait(ReplyReceivedAsync);
        }

        [LuisIntent("AddCoin")]
        public async Task AddCoin(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var val = result.Entities;

            //todo:get rid of 2 the same parts of code 
            try
            {
                var t = await CoinMarketCapService.GetTicker(val[1].Entity);
                var count = val[0].Entity;
                if (ConversationStarter.ToPortfolio(t.Id, new Coin(count, t)))
                    await context.PostAsync($"You added {count} {t.Name} to your portfolio");
              
            }
            catch (No​​Currency​​FoundException e)
            {
                await context.PostAsync("No ​​such currency​​ found or Luis don't get it");
            }
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("RemoveCoin")]
        public async Task RemoveCoin(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var val = result.Entities;
            try
            {
                var t = await CoinMarketCapService.GetTicker(val[1].Entity);
                var count = val[0].Entity;
                if (ConversationStarter.ToPortfolio(t.Id, new Coin("-"+count, t)))
                {
                    await context.PostAsync($"You remove {count} {t.Name} from your Portfolio");
                }
                else
                {
                    await context.PostAsync($"You don't have enough in your portfolio to remuve {count} {t.Name}");
                }
            }
            catch (No​​Currency​​FoundException e)
            {
                await context.PostAsync("No ​​such currency​​ found or luis don't get it");
            }

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("AddAlert")]
        public async Task AddAlert(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var val = result.Entities;
            var message = await activity;

            //todo: put inside
            ConversationStarter.toId = message.From.Id;
            ConversationStarter.toName = message.From.Name;
            ConversationStarter.fromId = message.Recipient.Id;
            ConversationStarter.fromName = message.Recipient.Name;
            ConversationStarter.serviceUrl = message.ServiceUrl;
            ConversationStarter.channelId = message.ChannelId;
            ConversationStarter.conversationId = message.Conversation.Id;

            int value = 0;
            if (int.TryParse(val[0].Entity, out value))
            {
                ConversationStarter.percent = value;
            }
            else
            {
                await context.PostAsync("Wrong format of percentage or Luis don't get it");
            }

            t = new Timer(new TimerCallback(timerEvent), null, 1000 * 60 * 5, Timeout.Infinite);
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("RemoveAlert")]
        public async Task RemoveAlert(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            t.Dispose();
            await context.PostAsync("Alert removed");
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("ShowPortfolio")]
        public async Task Alert(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Portfolio:");

            //todo: need replace to for
            foreach (var l in ConversationStarter.Portfolio.ToList())
            {
                float sum = 0;
                foreach (Coin c in l.Value)
                {
                    sum += c.Count;
                }
                await context.PostAsync($" {sum} {l.Key}'s = {sum*l.Value[0].Ticker.PriceUsd}$");
            }
        }

        public virtual async Task ReplyReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var msg = await argument;
            switch (msg.Text)
            {
                case "AddCoin":
                    await context.PostAsync("Pls type value and currency (2 btc)");
                    context.Wait(this.AddCoin);
                    break;
            }

        }


        public virtual async Task AddCoin(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
            var message = await result;
            var text = message.Text;

            var r = Regex.Matches(text, pattern, RegexOptions.IgnoreCase);

            //todo:get rid of 2 the same parts of code 
            var t = await CoinMarketCapService.GetTicker(r[1].Value);
            var count = r[0].Value;
            if (ConversationStarter.ToPortfolio(t.Id, new Coin(count, t)))
                await context.PostAsync($"You added {count} {t.Name} to your portfolio");

            }
            catch (No​​Currency​​FoundException e)
            {
                await context.PostAsync("No ​​such currency​​ found or Luis don't get it");
            }
        }


        private static Attachment GetHeroCard()
        {
            var heroCard = new HeroCard
            {
                Title = @"Hi! maybe?",
                
                Buttons = new List<CardAction>
                {
                    new CardAction(){Type = ActionTypes.PostBack, Value = "AddCoin", Title = "Add Coin"},
                    new CardAction(){Type = ActionTypes.PostBack, Value = "Portfolio", Title = "Show Portfolio"},
                },
                Subtitle = @"You can try ask Luis to add or remove some coins in to yours portfolio, or show it. And of course you can click buttons below"

            };

            return heroCard.ToAttachment();
        }

        private async void timerEvent(object target)
        {
            t.Dispose();
            await ConversationStarter.Resume(ConversationStarter.conversationId,
                ConversationStarter
                    .channelId);
           
            t = new Timer(new TimerCallback(timerEvent), null, 1000*60*5, Timeout.Infinite);
        }
     
    }
}
