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
    /// Datasource for reading data from file systems (local or remote).
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of FileDatasource.
    /// </remarks>
    /// <param name="fileSystem">File system implementation to use for data access.</param>
    /// <param name="rootPath">Root path for database operations. Can be empty for storage systems that don't use root paths.</param>
    [ExcludeFromCodeCoverage] // Methods consist mostly of file system IO
    public class FileDatasource(IFileSystem fileSystem, string rootPath = "") : IFileDatasource
    {
        private readonly IFileSystem fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        private readonly string rootPath = rootPath;

        /// <summary>
        /// Returns tables in a database group.
        /// </summary>
        /// <param name="groupHierarchy">Path to the group.</param>
        /// <returns>List of tables in the group.</returns>
        public async Task<List<PxTableReference>> GetTablesAsync(IReadOnlyList<string> groupHierarchy)
        {
            return await GetTables(groupHierarchy);
        }

        private async Task<List<PxTableReference>> GetTables(IReadOnlyList<string> groupHierarchy)
        {
            List<PxTableReference> tables = [];
            string path = PathUtils.BuildAndSanitizePath(rootPath, groupHierarchy);
            IEnumerable<string> pxFiles = await fileSystem.EnumerateFilesAsync(path, PxSyntaxConstants.PX_FILE_FILTER);

            foreach (string pxFile in pxFiles)
            {
                tables.Add(new PxTableReference(fileSystem.GetRelativePath(rootPath, pxFile)));
            }
            return tables;
        }

        /// <summary>
        /// Asynchronously lists the group header items from the specified level of the database.
        /// </summary>
        /// <param name="groupHierarchy">Defines the database level where to list the items from.</param>
        /// <returns>The subgroups from a level of a database.</returns>
        public async Task<List<DatabaseGroupHeader>> GetGroupHeadersAsync(IReadOnlyList<string> groupHierarchy)
        {
            List<DatabaseGroupHeader> headers = [];
            string path = PathUtils.BuildAndSanitizePath(rootPath, groupHierarchy);
            IEnumerable<string> directories = await fileSystem.EnumerateDirectoriesAsync(path);

            foreach (string directory in directories)
            {
                string code = fileSystem.GetDirectoryName(directory);
                MultilanguageString alias = await GetGroupNameAsync(directory);
                if (alias.Languages.Any()) // Only include groups with an alias file for one or more languages
                {
                    headers.Add(new DatabaseGroupHeader(code, [.. alias.Languages], alias));
                }
            }

            return headers;
        }

        /// <summary>
        /// Asynchronously gets the last time a table has been changed.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The last time the table has been changed.</returns>
        public async Task<DateTime> GetLastWriteTimeAsync(PxTableReference tableReference)
        {
            string path = PathUtils.BuildAndSanitizePath(rootPath, tableReference);
            return await fileSystem.GetLastWriteTimeAsync(path);
        }

        /// <summary>
        /// Asynchronously gets the metadata of a specified file.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <returns>The complete metadata from a file.</returns>
        public async Task<IReadOnlyMatrixMetadata> GetMatrixMetadataAsync(PxTableReference tableReference)
        {
            string path = PathUtils.BuildAndSanitizePath(rootPath, tableReference);
            using Stream readStream = await fileSystem.OpenReadAsync(path);
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
        /// Asynchronously gets the data matching the provided metadata object and builds a <see cref="Matrix{T}"/> from the result.
        /// </summary>
        /// <param name="tableReference">The source file identifier.</param>
        /// <param name="meta">Gets the data defined by this metadata object and uses this to build the result <see cref="Matrix{T}"/>.</param>
        /// <param name="completeTableMap">A complete map of the table metadata, file based implementations require this for reading.</param>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>A <see cref="Matrix{T}"/> build from the provided metadata and the related data.</returns>
        public async Task<Matrix<DecimalDataValue>> GetMatrixAsync(
            PxTableReference tableReference,
            IReadOnlyMatrixMetadata meta,
            IMatrixMap completeTableMap,
            CancellationToken? cancellationToken = null
        )
        {
            string path = PathUtils.BuildAndSanitizePath(rootPath, tableReference);
            DataIndexer indexer = new(completeTableMap, meta);
            Matrix<DecimalDataValue> output = new(meta, new DecimalDataValue[indexer.DataLength]);
            using Stream fileStream = await fileSystem.OpenReadAsync(path);
            PxFileStreamDataReader dataReader = new(fileStream);
            if (cancellationToken is null) await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, meta, completeTableMap);
            else await dataReader.ReadDecimalDataValuesAsync(output.Data, 0, meta, completeTableMap, cancellationToken.Value);
            return output;
        }

        /// <summary>
        /// Assigns ordinal or nominal dimension types to dimensions that are either of unknown or other type based on their meta-id properties.
        /// </summary>
        /// <param name="meta">The matrix metadata to assign the dimension types to.</param>
        private static void AssignOrdinalDimensionTypes(MatrixMetadata meta)
        {
            for (int i = 0; i < meta.Dimensions.Count; i++)
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

        /// <summary>
        /// Assigns appropriate language properties to single-language metadata properties.
        /// </summary>
        /// <param name="meta">The matrix metadata to assign language properties to.</param>
        /// <param name="keys">List of property keys to process.</param>
        private static void AssignLanguageToSingleLangProperties(MatrixMetadata meta, List<string> keys)
        {
            // Table level
            AssignLanguagePropertiesAtLevel(meta.AdditionalProperties, keys, meta.DefaultLanguage);

            foreach (Dimension dim in meta.Dimensions)
            {
                // Dimension level
                AssignLanguagePropertiesAtLevel(dim.AdditionalProperties, keys, meta.DefaultLanguage);

                // Dimension value level
                foreach (DimensionValue val in dim.Values)
                {
                    AssignLanguagePropertiesAtLevel(val.AdditionalProperties, keys, meta.DefaultLanguage);
                }
            }
        }

        /// <summary>
        /// Helper method to assign language properties at a specific metadata level.
        /// </summary>
        /// <param name="properties">The properties dictionary to process.</param>
        /// <param name="keys">List of property keys to process.</param>
        /// <param name="defaultLanguage">The default language to use.</param>
        private static void AssignLanguagePropertiesAtLevel(Dictionary<string, MetaProperty> properties, List<string> keys, string defaultLanguage)
        {
            foreach (string key in keys)
            {
                if (properties.TryGetValue(key, out MetaProperty? prop))
                {
                    properties[key] = prop.AsMultiLanguageProperty(defaultLanguage);
                }
            }
        }

        /// <summary>
        /// Assigns a dimension type to the given dimension based on its meta-id property.
        /// </summary>
        /// <param name="dimension">The dimension to analyze.</param>
        /// <returns>The appropriate dimension type.</returns>
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
        /// Gets the multilanguage group name from alias files.
        /// Assumes the alias files to be named Alias_[lang].txt where [lang] is the language code.
        /// </summary>
        /// <param name="directoryPath">Path to the directory containing the alias files.</param>
        /// <returns>Multilanguage string of the group name based on the alias files.</returns>
        private async Task<MultilanguageString> GetGroupNameAsync(string directoryPath)
        {
            Dictionary<string, string> translatedNames = [];
            IEnumerable<string> aliasFiles = await fileSystem.EnumerateFilesAsync(directoryPath, PxSyntaxConstants.ALIAS_FILE_FILTER);

            foreach (string aliasFile in aliasFiles)
            {
                string fileName = fileSystem.GetFileName(aliasFile);
                if (fileName.StartsWith(PxSyntaxConstants.ALIAS_FILE_PREFIX, StringComparison.OrdinalIgnoreCase))
                {
                    int aliasFileSuffixLength = PxSyntaxConstants.ALIAS_FILE_PREFIX.Length + 1; // +1 for the underscore
                    string lang = new([.. Path.GetFileNameWithoutExtension(fileName).Skip(aliasFileSuffixLength)]);
                    string alias = await fileSystem.ReadAllTextAsync(aliasFile);
                    translatedNames.Add(lang, alias.Trim());
                }
            }

            return new MultilanguageString(translatedNames);
        }
    }
}
#nullable disable