using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Messenger.Client.Objects;
using TransferBuddy.Service.Services;

namespace TransferBuddy.Service.Controllers
{
    /// <summary>
    /// The FacebookController class.
    /// </summary>
    [Route("api/webhook")]
    public class FacebookController : Controller
    {
        private readonly string verifyToken;

        private readonly ILogger<FacebookController> logger;

        private readonly MessageProcessorService processor;

        /// <summary>
        /// Initializes a new instance of the FacebookController class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="processor">The message processor.</param>
        public FacebookController(ILogger<FacebookController> logger, MessageProcessorService processor)
        {
            this.logger = logger;
            this.processor = processor;
            this.verifyToken = Environment.GetEnvironmentVariable("VERIFY_TOKEN");
            if (this.verifyToken == null) 
            {
                throw new Exception("Cannot find VERIFY_TOKEN in this env.");
            }     
        }

        /// <summary>
        /// Validates the sender.
        /// </summary>
        /// <returns>The response.</returns>
        [HttpGet, Produces("text/html")]
        public IActionResult Validate()
        {
            var challenge = Request.Query["hub.challenge"];
            var verifyToken = Request.Query["hub.verify_token"];

            if (verifyToken.Any() && verifyToken.First() == this.verifyToken)
            {
                return Ok(challenge.First());
            }

            return Ok();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="obj">The message object.</param>
        /// <returns>The response.</returns>
        [HttpPost]
        public async Task HandleMessage([FromBody] MessengerObject obj)
        {
            foreach (var entry in obj.Entries)
            {
                foreach (var messaging in entry.Messaging)
                {
                    if (messaging.Sender.Id == "1614435598861752")
                        continue;

                    if (messaging.Message == null && messaging.Postback == null)
                        continue;

                    if (messaging.Sender == null)
                    {
                        continue;
                    }
                    
                    var result = await processor.ProcessMessageAsync(messaging);

                    if (result == false)
                    {
                        logger.LogWarning("No handler found for the following message: " + messaging.ToString());
                    }
                }
            }
        }
    }
}
