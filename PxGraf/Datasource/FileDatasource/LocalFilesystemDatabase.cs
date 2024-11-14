#nullable enable
using Px.Utils.Language;
using Px.Utils.ModelBuilders;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.PxFile.Data;
using Px.Utils.PxFile.Metadata;
using PxGraf.Datasource.DatabaseConnection;
using PxGraf.Models.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PxGraf.Datasource.FileDatasource
{
    /// <summary>
    /// Datasource for reading data from the local filesystem.
    /// </summary>
    /// <param name="config">Local file system database configuration.</param>
    [ExcludeFromCodeCoverage] // Methods consist mostly of filesystem IO
    public class LocalFilesystemDatabase(LocalFilesystemDatabaseConfig config) : IFileDatasource
    {
        /// <summary>
        /// Returns tables in a database group.
        /// </summary>
        /// <param name="groupHierarcy">Path to the group.</param>
        /// <returns>List of tables in the group.</returns>
        public async Task<List<PxTableReference>> GetTablesAsync(IReadOnlyList<string> groupHierarcy)
        {
            return await Task.Factory.StartNew(() => GetTables(groupHierarcy));
        }

        private List<PxTableReference> GetTables(IReadOnlyList<string> groupHierarcy)
        {
            List<PxTableReference> tables = [];
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, groupHierarcy);
            foreach (string pxFile in Directory.EnumerateFiles(path, PxSyntaxConstants.PX_FILE_FILTER))
            {
                tables.Add(new PxTableReference(pxFile.Remove(0, config.DatabaseRootPath.Length)));
            }
            return tables;
        }

        public Task<List<DatabaseGroupHeader>> GetGroupHeadersAsync(IReadOnlyList<string> groupHierarcy)
        {
            return Task.Factory.StartNew(() => GetGroupHeaders(groupHierarcy));
        }

        public List<DatabaseGroupHeader> GetGroupHeaders(IReadOnlyList<string> groupHierarcy)
        {
            List<DatabaseGroupHeader> headers = [];

            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, groupHierarcy);
            foreach (string directory in Directory.EnumerateDirectories(path))
            {
                string code = new DirectoryInfo(directory).Name;
                MultilanguageString alias = GetGroupName(directory);
                if (!alias.Languages.Any()) continue; // Skip folders without alias folder - they are not valid database groups
                headers.Add(new DatabaseGroupHeader(code, [.. alias.Languages], alias));
            }

            return headers;
        }

        /// <inheritdoc/>
        public DateTime GetLastWriteTime(PxTableReference tableReference)
        {
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference.Hierarchy);
            return Directory.GetLastWriteTime(path);
        }

        /// <inheritdoc/> 
        public async Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference)
        {
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference);
            return await Task.Factory.StartNew(() => Directory.GetLastWriteTime(path));
        }

        /// <inheritdoc/> 
        public Matrix<DecimalDataValue> GetMatrix(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, IMatrixMap completeMap)
        {
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference);
            DataIndexer indexer = new(completeMap, meta);
            Matrix<DecimalDataValue> output = new(meta, new DecimalDataValue[indexer.DataLength]);
            using Stream fileStream = File.OpenRead(path);
            PxFileStreamDataReader dataReader = new(fileStream);
            dataReader.ReadDecimalDataValues(output.Data, 0, meta, completeMap);
            return output;
        }

        /// <inheritdoc/> 
        public async Task<Matrix<DecimalDataValue>> GetMatrixAsync(
            PxTableReference tableReference,
            IReadOnlyMatrixMetadata meta,
            IMatrixMap completeTableMap,
            CancellationToken? cancellationToken = null
            )
        {
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference);
            if (!path.EndsWith(PxSyntaxConstants.PX_FILE_EXTENSION))
                path += PxSyntaxConstants.PX_FILE_EXTENSION;
            DataIndexer indexer = new(completeTableMap, meta);
            Matrix<DecimalDataValue> output = new(meta, new DecimalDataValue[indexer.DataLength]);
            using Stream fileStream = File.OpenRead(path);
            PxFileStreamDataReader dataReader = new(fileStream);
            if (cancellationToken is null) await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, meta, completeTableMap);
            else await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, meta, completeTableMap, cancellationToken.Value);
            return output;
        }

        /// <inheritdoc/> 
        public async Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference)
        {
            if (!tableReference.Name.EndsWith(PxSyntaxConstants.PX_FILE_EXTENSION))
                tableReference.Name += PxSyntaxConstants.PX_FILE_EXTENSION;

            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference);
            using Stream readStream = File.OpenRead(path);
            PxFileMetadataReader metadataReader = new();
            IAsyncEnumerable<KeyValuePair<string, string>> entries = metadataReader.ReadMetadataAsync(readStream, config.Encoding);
            MatrixMetadataBuilder builder = new();
            MatrixMetadata meta = await builder.BuildAsync(entries);
            meta = meta.AssignOrdinalDimensionTypes();
            meta.AssignSourceToContentDimensionValues();
            return meta;
        }

        /// <summary>
        /// Assumes the alias files to be named Alias_[lang].txt where [lang] is the language code of the alias.
        /// </summary>
        /// <param name="path">Path to the folder containing the allias files.</param>
        /// <returns>Multilanguage string of the alias</returns>
        private MultilanguageString GetGroupName(string path)
        {
            Dictionary<string, string> translatedNames = [];
            IEnumerable<string> aliasFiles = Directory.GetFiles(path, PxSyntaxConstants.ALIAS_FILE_FILTER)
                .Where(p => Path.GetFileName(p).StartsWith(PxSyntaxConstants.ALIAS_FILE_PREFIX, StringComparison.OrdinalIgnoreCase));
            foreach (string aliasFile in aliasFiles)
            {
                int aliasFileSuffixLength = PxSyntaxConstants.ALIAS_FILE_PREFIX.Length + 1; // +1 for the underscore
                string lang = new([.. Path.GetFileNameWithoutExtension(aliasFile).Skip(aliasFileSuffixLength)]);
                string alias = File.ReadAllText(aliasFile, config.Encoding);
                translatedNames.Add(lang, alias.Trim());
            }
            return new MultilanguageString(translatedNames);
        }

    }
}
#nullable disable
