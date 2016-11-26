using System.Collections.Generic;
using System.Threading.Tasks;
using Messenger.Client.Objects;

namespace TransferBuddy.Service.Services.Messenger
{
    /// <summary>
    /// Handles the help message.
    /// </summary>
    public class HelpMessageHandler : MessageHandler, IMessageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HelpMessageHandler" /> class.
        /// </summary>
        /// <param name="processor">The message processor.</param>
        public HelpMessageHandler (MessageProcessorService processor) 
            : base(processor)
        {
        }
        
        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="messageContainer">The message container.</param>
        /// <returns>The result of the operation.</returns>
        public async Task<bool> HandleMessageAsync(MessengerMessaging messageContainer)
        {
            var message = messageContainer.Message;
            var sender = messageContainer.Sender;

            if (message?.Text == "help" || messageContainer?.Postback.Payload == "help")
            {
                await this.SimulateTypingAsync(sender, 1000);
                
                var buttons = new List<MessengerButtonBase>();
                buttons.Add(new MessengerLinkButton("Configure", $"https://transfer-buddy.herokuapp.com/Configuration?userId={sender.Id}"));

                await this.SendTextWithButtonsAsync(sender, "these are the command that I support atm ;)" , buttons);

                return true;
            }

            return false;
        }
    }
}