using Px.Utils.Models.Metadata.MetaProperties;
using PxGraf.Models.SavedQueries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PxGraf.Services
{
    /// <summary>
    /// Service interface for publication webhooks.
    /// </summary>
    public interface IPublicationWebhookService
    {
        /// <summary>
        /// Triggers a webhook notification for a published query.
        /// </summary>
        /// <param name="queryId">The ID of the saved query.</param>
        /// <param name="savedQuery">The saved query object.</param>
        /// <param name="additionalProperties">The metadata additional properties dictionary.</param>
        /// <returns>A task representing the asynchronous operation with publication result.</returns>
        Task<WebhookPublicationResult> TriggerWebhookAsync(string queryId, SavedQuery savedQuery, IReadOnlyDictionary<string, MetaProperty> additionalProperties);
    }
}
