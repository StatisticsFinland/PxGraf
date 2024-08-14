using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models.Metadata;
using Px.Utils.Models.Metadata.ExtensionMethods;
using PxGraf.Data.MetaData;
using PxGraf.Datasource;
using PxGraf.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Settings;
using PxGraf.Utility;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    [ApiController]
    [Route("api/sq/meta")]
    public class QueryMetaController(ISqFileInterface sqFileInterface, ICachedDatasource cachedDatasource, ILogger<QueryMetaController> logger) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = cachedDatasource;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<QueryMetaController> _logger = logger;

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
                IReadOnlyMatrixMetadata meta = await GetMatrixMetadata(savedQuery, savedQueryId);
                MultilanguageString header = HeaderBuildingUtilities.CreateDefaultHeader(meta.Dimensions, savedQuery.Query, meta.AvailableLanguages);
                QueryMetaResponse queryMetaResponse = new()
                {
                    Header = HeaderBuildingUtilities.ReplaceTimePlaceholdersInHeader(header, meta.GetTimeDimension()),
                    HeaderWithPlaceholders = header,
                    Archived = savedQuery.Archived,
                    Selectable = savedQuery.Query.DimensionQueries.Any(q => q.Value.Selectable),
                    VisualizationType = savedQuery.Settings.VisualizationType,
                    TableId = savedQuery.Query.TableReference.Name,
                    Description = meta.GetMatrixProperty(PxSyntaxConstants.NOTE_KEY),
                    TableReference = savedQuery.Query.TableReference,
                    LastUpdated = meta.GetContentDimension().Values.Map(cdv => cdv.LastUpdated).Max().ToString(),
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

        private async Task<IReadOnlyMatrixMetadata> GetMatrixMetadata(SavedQuery savedQuery, string id)
        {
            if(savedQuery.Archived)
            {
                if (_sqFileInterface.ArchiveCubeExists(id, Configuration.Current.ArchiveFileDirectory))
                {
                    ArchiveCube archiveCube = await _sqFileInterface.ReadArchiveCubeFromFile(id, Configuration.Current.ArchiveFileDirectory);
                    return archiveCube.Meta.ToMatrixMetadata();
                }
                else
                {
                    throw new FileNotFoundException($"Could not find .sqa file matching the id {id}");
                }
            }
            else
            {
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);
                return meta.FilterDimensionValues(savedQuery.Query);
            }
        }
    }
}
