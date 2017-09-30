using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using System;
using System.Globalization;

namespace MSAContosoBank.Dialogs
{
    [LuisModel("2f44cb9b-7b5d-4c6d-97c2-9f0d984fd385", "48ed42041636478a9b3face6f4b51273")]
    [Serializable]
    public class Luis_Dialog : LuisDialog<object>
    {
        bool hasAccount = false;
        double balance = 0;
        string name = null;

        [LuisIntent("None")]
        public async Task NoneResponse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Sorry, I could not understand that.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("OpenAccount")]
        public async Task OpenAccountResponse(IDialogContext context, LuisResult result)
        {
            if (hasAccount)
            {
                await context.PostAsync("You already have an account; I can't open another one for you.");
            }
            else
            {
                await context.PostAsync("I have opened an account for you.");
                hasAccount = true;
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("CloseAccount")]
        public async Task CloseAccountResponse(IDialogContext context, LuisResult result)
        {
            if (hasAccount)
            {
                hasAccount = false;
                await context.PostAsync("I have closed your account.");
            }
            else
            {
                await context.PostAsync("You do not have an account that I can close.");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Deposit")]
        public async Task DepositResponse(IDialogContext context, LuisResult result)
        {
            if (!hasAccount)
            {
                await context.PostAsync("You do not have an account open.");
            }
            else
            {
                var entities = result.Entities;
                object amount;
                double value = 0;

                foreach (var entity in entities)
                {
                    if (entity.Type == "builtin.currency")
                    {
                        if (entity.Resolution.TryGetValue("value", out amount))
                        {
                            value += double.Parse((string)amount);
                        }
                    }
                }

                if (value > 0)
                {
                    balance += value;
                    await context.PostAsync($"You have deposited {value.ToString("C", new CultureInfo("en-NZ"))} into your account.");
                }
                else
                {
                    await context.PostAsync("Please enter a positive amount of money, correctly formatted.");
                }
            }
            
            context.Wait(MessageReceived);
        }

        [LuisIntent("Withdraw")]
        public async Task WithdrawResponse(IDialogContext context, LuisResult result)
        {
            if (!hasAccount)
            {
                await context.PostAsync("You do not have an account open.");
            }
            else
            {
                var entities = result.Entities;
                object amount;
                double value = 0;

                foreach (var entity in entities)
                {
                    if (entity.Type == "builtin.currency")
                    {
                        if (entity.Resolution.TryGetValue("value", out amount))
                        {
                            value += double.Parse((string)amount);
                        }
                    }
                }

                if (value > 0)
                {
                    balance -= value;
                    await context.PostAsync($"You have withdrawn {value.ToString("C", new CultureInfo("en-NZ"))} from your account.");
                }
                else
                {
                    await context.PostAsync("Please enter a positive amount of money, correctly formatted.");
                }
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("CheckBalance")]
        public async Task CheckBalanceResponse(IDialogContext context, LuisResult result)
        {
            if (!hasAccount)
            {
                await context.PostAsync("You do not have an account open.");
            }
            else
            {
                await context.PostAsync($"Your balance is {balance.ToString("C", new CultureInfo("en-NZ"))}.");
            }

            context.Wait(MessageReceived);
        }
    }
}