using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3267:Loops should be simplified with \"LINQ\" expressions", Justification = "Explicit loops are preferred for clarity.")]
[assembly: SuppressMessage("Performance", "CA1873:Avoid potentially expensive logging", Justification = "<Pending>", Scope = "member", Target = "~M:PxGraf.Services.PublicationWebhookService.TriggerWebhookAsync(System.String,PxGraf.Models.SavedQueries.SavedQuery,System.Collections.Generic.IReadOnlyDictionary{System.String,Px.Utils.Models.Metadata.MetaProperties.MetaProperty})~System.Threading.Tasks.Task{PxGraf.Services.WebhookPublicationResult}")]
