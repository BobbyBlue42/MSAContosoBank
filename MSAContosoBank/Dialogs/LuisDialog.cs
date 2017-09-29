using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;

namespace MSAContosoBank
{
    [LuisModel("2f44cb9b-7b5d-4c6d-97c2-9f0d984fd385", "48ed42041636478a9b3face6f4b51273")]
    public class Luis_Dialog : LuisDialog<object>
    {

        [LuisIntent("OpenAccount")]
        public async Task OpenAccountResponse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("An account has been opened for you.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("CloseAccount")]
        public async Task CloseAccountResponse(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Your account has been closed.");

            context.Wait(MessageReceived);
        }

        [LuisIntent("Deposit")]
        public async Task DepositResponse(IDialogContext context, LuisResult result)
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
                // Adjust state
                await context.PostAsync($"You have deposited ${value} into your account.");
            }
            else
            {
                await context.PostAsync("Please enter a positive amount of money, correctly formatted.");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Withdraw")]
        public async Task WithdrawResponse(IDialogContext context, LuisResult result)
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
                // Adjust state
                await context.PostAsync($"You have withdrawn ${value} from your account.");
            }
            else
            {
                await context.PostAsync("Please enter a positive amount of money, correctly formatted.");
            }

            context.Wait(MessageReceived);
        }
    }
}