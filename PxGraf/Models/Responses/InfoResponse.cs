namespace PxGraf.Models.Responses
{
    /// <summary>
    /// Response model for the API info endpoint.
    /// </summary>
    /// <param name="Name">The application name.</param>
    /// <param name="Environment">The hosting environment name.</param>
    /// <param name="Version">The application version.</param>
    public record InfoResponse(string Name, string Environment, string Version);
}
