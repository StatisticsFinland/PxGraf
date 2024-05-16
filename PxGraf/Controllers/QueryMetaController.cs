using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PxGraf.Data.MetaData;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.PxWebInterface;
using PxGraf.Settings;
using PxGraf.Utility;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    /// <summary>
    /// Handles requests for saved query metadata requests.
    /// </summary>
    [ApiController]
    [Route("api/sq/meta")]
    public class QueryMetaController : ControllerBase
    {
        private readonly ICachedPxWebConnection _cachedPxWebConnection;
        private readonly ISqFileInterface _sqFileInterface;
        private readonly ILogger<QueryMetaController> _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sqFileInterface">Instance of a <see cref="ISqFileInterface"/> object. Used for interacting with saved queries.</param>
        /// <param name="cachedPxWebConnection">Instance of a <see cref="ICachedPxWebConnection"/> object. Used to interact with PxWeb API and cache data.</param>
        /// <param name="logger">Instance of the <see cref="ILogger"/> used for logging API calls.</param>
        public QueryMetaController(ISqFileInterface sqFileInterface, ICachedPxWebConnection cachedPxWebConnection, ILogger<QueryMetaController> logger)
        {
            _cachedPxWebConnection = cachedPxWebConnection;
            _sqFileInterface = sqFileInterface;
            _logger = logger;
        }

        /// <summary>
        /// Returns metadata about a saved query.
        /// </summary>
        /// <param name="savedQueryId">ID of the saved query.</param>
        /// <returns>
        /// <see cref="QueryMetaResponse"/> object that contains saved query metadata including its header, archived status and visualization settings.
        /// If no query for the given ID is found, "Not Found" response is returned.
        /// </returns>
        [HttpGet("{savedQueryId}")]
        public async Task<ActionResult<QueryMetaResponse>> GetQueryMeta([FromRoute] string savedQueryId)
        {
            _logger.LogDebug("Query meta requested GET: api/sq/meta/{SavedQueryId}", savedQueryId);
            if (_sqFileInterface.SavedQueryExists(savedQueryId, Configuration.Current.SavedQueryDirectory))
            {
                SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(savedQueryId, Configuration.Current.SavedQueryDirectory);
                IReadOnlyCubeMeta readOnlyTableMeta = await GetCubeMeta(savedQuery, savedQueryId);
                CubeMeta tableMeta = readOnlyTableMeta.Clone();
                tableMeta.ApplyEditionFromQuery(savedQuery.Query);

                QueryMetaResponse queryMetaResponse = new ()
                {
                    Header = tableMeta.GetHeaderWithoutTimePlaceholders(),
                    HeaderWithPlaceholders = tableMeta.Header,
                    Archived = savedQuery.Archived,
                    Selectable = savedQuery.Query.VariableQueries.Any(q => q.Value.Selectable),
                    VisualizationType = savedQuery.Settings.VisualizationType,
                    TableId = savedQuery.Query.TableReference.Name,
                    Description = tableMeta.Note,
                    TableReference = savedQuery.Query.TableReference,
                    LastUpdated = tableMeta.GetLastUpdated()
                };
                _logger.LogInformation("{SavedQueryId} result: {QueryMetaResponse}", savedQueryId, queryMetaResponse);
                return queryMetaResponse;
            }
            else
            {
                _logger.LogWarning("Could not find saved query with id {SavedQueryId}", savedQueryId);
                return NotFound();
            }
        }

        private async Task<IReadOnlyCubeMeta> GetCubeMeta(SavedQuery savedQuery, string id)
        {
            if(savedQuery.Archived)
            {
                if (_sqFileInterface.ArchiveCubeExists(id, Configuration.Current.ArchiveFileDirectory))
                {
                    ArchiveCube archiveCube = await _sqFileInterface.ReadArchiveCubeFromFile(id, Configuration.Current.ArchiveFileDirectory);
                    return archiveCube.Meta;
                }
                else
                {
                    throw new FileNotFoundException($"Could not find .sqa file matching the id {id}");
                }
            }
            else
            {
                return await _cachedPxWebConnection.GetCubeMetaCachedAsync(savedQuery.Query.TableReference);
            }
        }
    }
}
