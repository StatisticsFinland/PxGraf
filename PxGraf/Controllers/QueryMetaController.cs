using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Px.Utils.Language;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata;
using PxGraf.Datasource;
using PxGraf.Models.Metadata;
using PxGraf.Models.Responses;
using PxGraf.Models.SavedQueries;
using PxGraf.Services;
using PxGraf.Settings;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PxGraf.Controllers
{
    [ApiController]
    [Route("api/sq/meta")]
    public class QueryMetaController(ISqFileInterface sqFileInterface, ICachedDatasource cachedDatasource, ILogger<QueryMetaController> logger, IAuditLogService auditLogService) : ControllerBase
    {
        private readonly ICachedDatasource _cachedDatasource = cachedDatasource;
        private readonly ISqFileInterface _sqFileInterface = sqFileInterface;
        private readonly ILogger<QueryMetaController> _logger = logger;
        private readonly IAuditLogService _auditLogService = auditLogService;

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
            Dictionary<string, object> logScope = new()
            {
                [LoggerConstants.CONTROLLER] = nameof(QueryMetaController),
                [LoggerConstants.ACTION] = "api/sq/meta"
            };
            using (_logger.BeginScope(logScope))
            {
                _logger.LogDebug("Query meta requested GET: api/sq/meta/");
                if (_sqFileInterface.SavedQueryExists(savedQueryId, Configuration.Current.SavedQueryDirectory))
                {
                    using (_logger.BeginScope(new Dictionary<string, object> { [LoggerConstants.SQ_ID] = savedQueryId }))
                    {
                        _auditLogService.LogAuditEvent(
                            action: "api/sq/meta",
                            resource: savedQueryId
                        );
                        SavedQuery savedQuery = await _sqFileInterface.ReadSavedQueryFromFile(savedQueryId, Configuration.Current.SavedQueryDirectory);
                        IReadOnlyMatrixMetadata meta = await GetMatrixMetadata(savedQuery, savedQueryId);
                        if (meta == null)
                        {
                            _logger.LogWarning("The table for the saved query was not found.");
                            return NotFound();
                        }
                        IReadOnlyMatrixMetadata filteredMeta = meta.FilterDimensionValues(savedQuery.Query);
                        MultilanguageString header = HeaderBuildingUtilities.GetHeader(meta, savedQuery.Query, true);
                        QueryMetaResponse queryMetaResponse = new()
                        {
                            Header = HeaderBuildingUtilities.ReplaceTimePlaceholdersInHeader(header, filteredMeta.GetTimeDimension()),
                            HeaderWithPlaceholders = header,
                            Archived = savedQuery.Archived,
                            Selectable = savedQuery.Query.DimensionQueries.Any(q => q.Value.Selectable),
                            VisualizationType = savedQuery.Settings.VisualizationType,
                            TableId = savedQuery.Query.TableReference.Name,
                            Description = null, // This is kept here for compatibility with the old version, but we currently don't have a description for saved queries
                            TableReference = savedQuery.Query.TableReference,
                            LastUpdated = PxSyntaxConstants.FormatPxDateTime(filteredMeta.GetContentDimension().Values.Map(cdv => cdv.LastUpdated).Max())
                        };
                        _logger.LogDebug("Returning saved query metadata.");
                        return queryMetaResponse;
                    }
                }
                else
                {
                    _auditLogService.LogAuditEvent(
                        action: "api/sq/meta",
                        resource: LoggerConstants.INVALID_OR_MISSING_SQID
                    );

                    _logger.LogWarning("Could not find saved query with the provided id");
                    return NotFound();
                }
            }
        }

        private async Task<IReadOnlyMatrixMetadata> GetMatrixMetadata(SavedQuery savedQuery, string id)
        {
            if (savedQuery.Archived)
            {
                if (_sqFileInterface.ArchiveCubeExists(id, Configuration.Current.ArchiveFileDirectory))
                {
                    ArchiveCube archiveCube = await _sqFileInterface.ReadArchiveCubeFromFile(id, Configuration.Current.ArchiveFileDirectory);
                    return archiveCube.Meta;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                IReadOnlyMatrixMetadata meta = await _cachedDatasource.GetMatrixMetadataCachedAsync(savedQuery.Query.TableReference);
                if (meta == null) return null;
                return meta.FilterDimensionValues(savedQuery.Query);
            }
        }
    }
}
