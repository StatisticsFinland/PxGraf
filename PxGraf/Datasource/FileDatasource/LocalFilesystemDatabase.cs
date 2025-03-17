#nullable enable
using Px.Utils.Language;
using Px.Utils.ModelBuilders;
using Px.Utils.Models.Data.DataValue;
using Px.Utils.Models.Metadata.Dimensions;
using Px.Utils.Models.Metadata.Enums;
using Px.Utils.Models.Metadata.ExtensionMethods;
using Px.Utils.Models.Metadata.MetaProperties;
using Px.Utils.Models.Metadata;
using Px.Utils.Models;
using Px.Utils.PxFile.Data;
using Px.Utils.PxFile.Metadata;
using PxGraf.Models.Queries;
using PxGraf.Models.Responses.DatabaseItems;
using PxGraf.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System;

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
        /// <param name="groupHierarchy">Path to the group.</param>
        /// <returns>List of tables in the group.</returns>
        public async Task<List<PxTableReference>> GetTablesAsync(IReadOnlyList<string> groupHierarchy)
        {
            return await Task.Factory.StartNew(() => GetTables(groupHierarchy));
        }

        private List<PxTableReference> GetTables(IReadOnlyList<string> groupHierarchy)
        {
            List<PxTableReference> tables = [];
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, groupHierarchy);
            foreach (string pxFile in Directory.EnumerateFiles(path, PxSyntaxConstants.PX_FILE_FILTER))
            {
                tables.Add(new PxTableReference(Path.GetRelativePath(config.DatabaseRootPath, pxFile)));
            }
            return tables;
        }

        public Task<List<DatabaseGroupHeader>> GetGroupHeadersAsync(IReadOnlyList<string> groupHierarchy)
        {
            return Task.Factory.StartNew(() => GetGroupHeaders(groupHierarchy));
        }

        public List<DatabaseGroupHeader> GetGroupHeaders(IReadOnlyList<string> groupHierarchy)
        {
            List<DatabaseGroupHeader> headers = [];

            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, groupHierarchy);
            foreach (string directory in Directory.EnumerateDirectories(path))
            {
                string code = new DirectoryInfo(directory).Name;
                MultilanguageString alias = GetGroupName(directory);
                if (alias.Languages.Any()) // Only include groups with an alias file for one or more languages
                {
                    headers.Add(new DatabaseGroupHeader(code, [.. alias.Languages], alias));
                }
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
            string path = PathUtils.BuildAndSanitizePath(config.DatabaseRootPath, tableReference);
            using Stream readStream = File.OpenRead(path);
            PxFileMetadataReader metadataReader = new();
            Encoding encoding = await metadataReader.GetEncodingAsync(readStream);
            readStream.Position = 0;
            IAsyncEnumerable<KeyValuePair<string, string>> entries = metadataReader.ReadMetadataAsync(readStream, encoding);
            MatrixMetadataBuilder builder = new();
            MatrixMetadata meta = await builder.BuildAsync(entries);
            AssignOrdinalDimensionTypes(meta);
            AssignSourceToContentDimensionValues(meta);
            AssignLanguageToSingleLangProperties(meta, [PxSyntaxConstants.NOTE_KEY, PxSyntaxConstants.VALUENOTE_KEY]);
            return meta;
        }

        /// <summary>
        /// Assigns ordinal or nominal dimension types to dimensions that are either of unknown or other type based on their meta-id properties.
        /// </summary>
        /// <param name="meta">The matrix metadata to assign the dimension types to.</param>
        private static void AssignOrdinalDimensionTypes(MatrixMetadata meta)
        {
            for(int i = 0; i < meta.Dimensions.Count; i++)
            {
                DimensionType newType = GetDimensionType(meta.Dimensions[i]);
                if (newType == DimensionType.Ordinal || newType == DimensionType.Nominal)
                {
                    meta.Dimensions[i] = new(
                        meta.Dimensions[i].Code,
                        meta.Dimensions[i].Name,
                        meta.Dimensions[i].AdditionalProperties,
                        meta.Dimensions[i].Values,
                        newType);
                }
            }
        }

        /// <summary>
        /// Ensures that source property exists for all content dimension values in the given metadata.
        /// Prioritises the source property of the content dimension value over the source property of the dimension and the table.
        /// </summary>
        /// <param name="meta">Metadata object to be processed.</param>
        /// <exception cref="InvalidOperationException">Thrown if source property is not found from metadata.</exception>
        private static void AssignSourceToContentDimensionValues(MatrixMetadata meta)
        {
            if (meta.TryGetContentDimension(out ContentDimension? contentDimension))
            {
                string defaultLang = meta.DefaultLanguage;
                foreach (ContentDimensionValue cdv in contentDimension.Values)
                {
                    // Primarily use source information from the content dimension value.
                    if (cdv.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? valueProp))
                    {
                        cdv.AdditionalProperties[PxSyntaxConstants.SOURCE_KEY] = valueProp.AsMultiLanguageProperty(defaultLang);
                    }
                    // If the value has no source, use the source of the content dimension.
                    else if (contentDimension.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? dimProp))
                    {
                        cdv.AdditionalProperties.TryAdd(PxSyntaxConstants.SOURCE_KEY, dimProp.AsMultiLanguageProperty(defaultLang));
                    }
                    // If the dimension has no source, use the source of the table.
                    else if (meta.AdditionalProperties.TryGetValue(PxSyntaxConstants.SOURCE_KEY, out MetaProperty? tableProp))
                    {
                        cdv.AdditionalProperties.TryAdd(PxSyntaxConstants.SOURCE_KEY, tableProp.AsMultiLanguageProperty(defaultLang));
                    }
                    else
                    {
                        throw new InvalidOperationException("Source property not found from metadata.");
                    }
                }

                meta.AdditionalProperties.Remove(PxSyntaxConstants.SOURCE_KEY);
                contentDimension.AdditionalProperties.Remove(PxSyntaxConstants.SOURCE_KEY);
            }
        }

        private static void AssignLanguageToSingleLangProperties(MatrixMetadata meta, List<string> keys)
        {
            // Table level
            foreach(string key in keys)
            {
                if (meta.AdditionalProperties.TryGetValue(key, out MetaProperty? prop))
                {
                    meta.AdditionalProperties[key] = prop.AsMultiLanguageProperty(meta.DefaultLanguage);
                }
            }

            foreach (Dimension dim in meta.Dimensions)
            {
                // Dimension level
                foreach (string key in keys)
                {
                    if (dim.AdditionalProperties.TryGetValue(key, out MetaProperty? prop))
                    {
                        dim.AdditionalProperties[key] = prop.AsMultiLanguageProperty(meta.DefaultLanguage);
                    }
                }

                // Dimension value level
                foreach (DimensionValue val in dim.Values)
                {
                    foreach (string key in keys)
                    {
                        if (val.AdditionalProperties.TryGetValue(key, out MetaProperty? prop))
                        {
                            val.AdditionalProperties[key] = prop.AsMultiLanguageProperty(meta.DefaultLanguage);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Assigns a dimension type to the given dimension based on its meta-id property.
        /// </summary>
        /// <param name="dimension"></param>
        /// <returns></returns>
        private static DimensionType GetDimensionType(Dimension dimension)
        {
            // If the dimension already has a defining type, ordinality should not overrun it
            if (dimension.Type == DimensionType.Unknown ||
                dimension.Type == DimensionType.Other)
            {
                string propertyKey = PxSyntaxConstants.META_ID_KEY;
                if (dimension.AdditionalProperties.TryGetValue(propertyKey, out MetaProperty? prop) &&
                    prop is MultilanguageStringProperty mlsProp)
                {
                    dimension.AdditionalProperties.Remove(propertyKey); // OBS: Remove the property after retrieval
                    if (mlsProp.Value.UniformValue().Equals(PxSyntaxConstants.ORDINAL_VALUE)) return DimensionType.Ordinal;
                    else if (mlsProp.Value.UniformValue().Equals(PxSyntaxConstants.NOMINAL_VALUE)) return DimensionType.Nominal;
                }
            }
            return dimension.Type;
        }

        /// <summary>
        /// Assumes the alias files to be named Alias_[lang].txt where [lang] is the language code of the alias.
        /// </summary>
        /// <param name="path">Path to the folder containing the alias files.</param>
        /// <returns>Multilanguage string of the group name based on the alias files</returns>
        private MultilanguageString GetGroupName(string path)
        {
            Dictionary<string, string> translatedNames = [];
            IEnumerable<string> aliasFiles = Directory.GetFiles(path, PxSyntaxConstants.ALIAS_FILE_FILTER)
                .Where(p => Path.GetFileName(p).StartsWith(PxSyntaxConstants.ALIAS_FILE_PREFIX, StringComparison.OrdinalIgnoreCase));
            foreach (string aliasFile in aliasFiles)
            {
                int aliasFileSuffixLength = PxSyntaxConstants.ALIAS_FILE_PREFIX.Length + 1; // +1 for the underscore
                string lang = new([.. Path.GetFileNameWithoutExtension(aliasFile).Skip(aliasFileSuffixLength)]);
                string alias = GetAliasFromFile(aliasFile);
                translatedNames.Add(lang, alias.Trim());
            }
            return new MultilanguageString(translatedNames);
        }

        private string GetAliasFromFile(string path)
        {
            using FileStream? fs = File.OpenRead(path);
            Encoding encoding = config.Encoding;
            Ude.CharsetDetector cdet = new();
            cdet.Feed(fs);
            cdet.DataEnd();
            if (cdet.Charset != null)
            {
                encoding = Encoding.GetEncoding(cdet.Charset);
            }
            fs.Position = 0;
            using StreamReader sr = new(fs, encoding);
            return sr.ReadToEnd();
        }
    }
}
#nullable disable
