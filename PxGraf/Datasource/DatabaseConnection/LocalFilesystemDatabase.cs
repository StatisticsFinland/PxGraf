#nullable enable
using Px.Utils.Language;
using Px.Utils.ModelBuilders;
using Px.Utils.Models;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata;
using Px.Utils.PxFile.Data;
using Px.Utils.PxFile.Metadata;
using PxGraf.Datasource.DatabaseConnection.DatabaseItems;
using PxGraf.Models.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PxGraf.Datasource.DatabaseConnection
{
    public class LocalFilesystemDatabase(LocalFilesystemDatabaseConfig config, Func<PxTableReference, string> referenceToPathConverter) : IDatasource
    {
        private readonly Func<PxTableReference, string> _referenceToPath = referenceToPathConverter;

        /// <inheritdoc/>
        public DatabaseGroupContents GetGroupContents(IReadOnlyList<string> groupHierarcy)
        {
            List<DatabaseGroupHeader> headers = [];
            List<PxTableReference> tables = [];

            string path = Path.Combine(config.DatabaseRootPath, string.Join(Path.PathSeparator, groupHierarcy));
            foreach (string directory in Directory.EnumerateDirectories(path))
            {
                string code = new DirectoryInfo(directory).Name;
                MultilanguageString alias = GetGroupName(directory);
                headers.Add(new DatabaseGroupHeader(code, alias));
            }

            const string PX_FILE_FILTER = "*.px";
            foreach (string pxFile in Directory.EnumerateFiles(path, PX_FILE_FILTER))
            {
                tables.Add(new PxTableReference(pxFile.Remove(0, config.DatabaseRootPath.Length)));
            }

            return new DatabaseGroupContents(headers, tables);
        }

        /// <inheritdoc/>
        public async Task<DatabaseGroupContents> GetGroupContentsAsync(IReadOnlyList<string> groupHierarcy)
        {
            return await Task.Factory.StartNew(() => GetGroupContents(groupHierarcy));
        }

        /// <inheritdoc/>
        public DateTime GetLastWriteTime(PxTableReference tableReference)
        {
            string path = _referenceToPath(tableReference);
            return Directory.GetLastWriteTime(path);
        }

        /// <inheritdoc/> 
        public async Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference)
        {
            string path = _referenceToPath(tableReference);
            return await Task.Factory.StartNew(() => Directory.GetLastWriteTime(path));
        }

        /// <inheritdoc/> 
        public Matrix<DecimalDataValue> GetMatrix(PxTableReference tableReference, IReadOnlyMatrixMetadata meta, IMatrixMap completeMap)
        {
            string path = _referenceToPath(tableReference);
            DataIndexer indexer = new(completeMap, meta);
            Matrix<DecimalDataValue> output = new(meta, new DecimalDataValue[indexer.DataLength]);
            using Stream fileStream = File.OpenRead(path);
            PxFileStreamDataReader dataReader = new(fileStream);
            dataReader.ReadDecimalDataValues(output.Data, 0, indexer);
            return output;
        }

        /// <inheritdoc/> 
        public async Task<Matrix<DecimalDataValue>> GetMatrixAsync(
            PxTableReference tableReference,
            IReadOnlyMatrixMetadata meta,
            IMatrixMap completeMap,
            CancellationToken? cancellationToken = null
            )
        {
            string path = _referenceToPath(tableReference);
            DataIndexer indexer = new(completeMap, meta);
            Matrix<DecimalDataValue> output = new(meta, new DecimalDataValue[indexer.DataLength]);
            using Stream fileStream = File.OpenRead(path);
            PxFileStreamDataReader dataReader = new(fileStream);
            if (cancellationToken is null) await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, indexer); 
            else await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, indexer, cancellationToken.Value);
            return output;
        }

        /// <inheritdoc/> 
        public IReadOnlyMatrixMetadata GetMatrixMetadata(PxTableReference tableReference)
        {
            string path = _referenceToPath(tableReference);
            using Stream readStream = File.OpenRead(path);
            IEnumerable<KeyValuePair<string, string>> entries = PxFileMetadataReader.ReadMetadata(readStream, config.Encoding);
            MatrixMetadataBuilder builder = new();
            return builder.Build(entries);
        }

        /// <inheritdoc/> 
        public async Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference)
        {
            string path = _referenceToPath(tableReference);
            using Stream readStream = File.OpenRead(path);
            IAsyncEnumerable<KeyValuePair<string, string>> entries = PxFileMetadataReader.ReadMetadataAsync(readStream, config.Encoding);
            MatrixMetadataBuilder builder = new();
            return await builder.BuildAsync(entries);
        }

        /// <summary>
        /// Assumes the alias files to be named Alias_[lang].txt where [lang] is the language code of the alias.
        /// </summary>
        /// <param name="path">Path to the folder containing the allias files.</param>
        /// <returns>Multilanguage string of the alias</returns>
        private MultilanguageString GetGroupName(string path)
        {
            Dictionary<string, string> translatedNames = [];
            const string ALIAS_FILE_FILTER = "*.txt";
            IEnumerable<string> aliasFiles = Directory.GetFiles(path, ALIAS_FILE_FILTER)
                .Where(p => Path.GetFileName(p).StartsWith("alias", StringComparison.OrdinalIgnoreCase));
            foreach (string aliasFile in aliasFiles)
            {
                string lang = new([.. Path.GetFileName(aliasFile).Skip(6).TakeWhile(c => c != '.')]);
                string alias = File.ReadAllText(aliasFile, config.Encoding);
                translatedNames.Add(lang, alias.Trim());
            }
            return new MultilanguageString(translatedNames);
        }
    }
}
#nullable disable
