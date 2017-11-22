using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using CoinMarketCap;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

namespace coinmarketcapBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {

            await this.SendWelcomeMessageAsync(context);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await context.PostAsync("Hi, I'm the CoinMarketCapBot bot. Let's get started.");

            context.Call(new CoinDialog(), this.CoinDialogComplete);
        }

        private async Task CoinDialogComplete(IDialogContext context, IAwaitable<object> result)
        {
            throw new NotImplementedException();
        }
    }
}