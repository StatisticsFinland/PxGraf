using Px.Utils.Language;
using System.Collections.Generic;

namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response object from publication webhook containing localized messages
    /// </summary>
    public class WebhookResponse
    {
        /// <summary>
        /// Localized messages from the webhook.
        /// </summary>
        public MultilanguageString Messages { get; set; } = new MultilanguageString([]);
    }
}