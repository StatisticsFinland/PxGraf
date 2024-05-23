using Newtonsoft.Json;
using PxGraf.Language;
using PxGraf.Models.Queries;
using PxGraf.PxWebInterface.SerializationModels;
using PxGraf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Data.MetaData
{
    /// <summary>
    /// Information that defines the cube and gives meaning to the numeric data.
    /// </summary>
    public class CubeMeta : IReadOnlyCubeMeta
    {
        /// <summary>
        /// The multilanguage strings of this cube meta have all these languages.
        /// </summary>
        public List<string> Languages { get; private set; }

        IReadOnlyList<string> IReadOnlyCubeMeta.Languages => Languages;

        /// <summary>
        /// Header text of the query / visualization.
        /// </summary>
        public MultiLanguageString Header { get; private set; }

        IReadOnlyMultiLanguageString IReadOnlyCubeMeta.Header => Header;

        /// <summary>
        /// Some arbitrary information about the cube.
        /// </summary>
        public MultiLanguageString Note { get; private set; }

        IReadOnlyMultiLanguageString IReadOnlyCubeMeta.Note => Note;

        /// <summary>
        /// An ordered collection of the variables that define the cube.
        /// </summary>
        public List<Variable> Variables { get; private set; }

        IReadOnlyList<IReadOnlyVariable> IReadOnlyCubeMeta.Variables => Variables;

        /// <summary>
        /// Parameterless constructos udes when the cube meta is built in parts
        /// by appending information.
        /// </summary>
        public CubeMeta()
        {
            Languages = [];
            Variables = [];
            Header = null;
            Note = null;
        }

        /// <summary>
        /// Used when the cube meta can be built with all the required infomation available.
        /// </summary>
        [JsonConstructor]
        public CubeMeta(
            List<string> languages,
            MultiLanguageString header,
            MultiLanguageString note,
            List<Variable> variables
        )
        {
            Languages = languages;
            Variables = variables;
            Header = header;
            Note = note;
        }

        /// <summary>
        /// Returns a transformed deep copy of the metadata. This copy can be mutated.
        /// The copy has the same structure as the provided map,
        /// but contains all relevant metadata from the original.
        /// </summary>
        /// <param name="map"><see cref="VariableMap"/> object that defines the transformation.</param>
        /// <returns>A <see cref="CubeMeta"/> object based on the original variable map</returns>
        public CubeMeta GetTransform(IReadOnlyList<VariableMap> map)
        {
            var newVariables = new List<Variable>();
            foreach (var varMap in map)
            {
                Variable newVar = Variables.Find(v => v.Code == varMap.Code).GetTransform(varMap);
                newVariables.Add(newVar);
            }

            var newMeta = new CubeMeta(
                languages: new List<string>(Languages),
                header: Header.Clone(),
                note: Note?.Clone(), // Notes are also optional
                variables: newVariables
            );

            return newMeta;
        }

        /// <summary>
        /// Makes a deep copy of the object.
        /// </summary>
        /// <returns>A copied <see cref="CubeMeta"/> object.</returns>
        public CubeMeta Clone()
        {
            return new CubeMeta(
                languages: new List<string>(Languages),
                variables: Variables.Select(v => v.Clone()).ToList(),
                header: Header?.Clone(),
                note: Note?.Clone()
                );
        }

        /// <summary>
        /// Provides only the structure information about the cube meta.
        /// (variable codes and included variable value codes in order)
        /// </summary>
        /// <returns>A <see cref="CubeMap"/> object that represents the structure of the cube meta - its variable codes and their included value codes.</returns>
        public CubeMap BuildMap()
        {
            List<VariableMap> variableMap = Variables.Select(v => v.BuildVariableMap()).ToList();
            return new CubeMap(variableMap);
        }

        internal void AppendPxWebMetaData(string language, PxMetaResponse meta)
        {
            Languages.Add(language);
            foreach (var metaVar in meta.Variables)
            {
                if (Variables.Find(var => var.Code == metaVar.Code) is Variable v)
                {
                    v.AddTranslations(language, metaVar);
                }
                else
                {
                    Variables.Add(new Variable(language, metaVar));
                }
            }
        }

        internal void AppendPxWebDataResult(Dictionary<string, JsonStat2> dataResults)
        {
            string[] contentVariableCodes = dataResults.First().Value.Role.Metric;

            // Dodge null reference error in case of no content variable
            if (contentVariableCodes == null)
            {
                return;
            }

            foreach (string contentVariableCode in contentVariableCodes)
            {
                var source = dataResults.ToDictionary(
                    d => d.Key,
                    d => d.Value.Source ?? ""
                );

                var updated = dataResults.Select(
                    d => d.Value.Updated
                )
                .Distinct()
                .Single();

                foreach (var contValue in CodeToVar(contentVariableCode).IncludedValues)
                {
                    var unit = dataResults.ToDictionary(
                        d => d.Key,
                        d => d.Value.Dimensions[contentVariableCode].Category.Unit[contValue.Code].Base ?? ""
                    );

                    var decimals = dataResults.Select(
                        d => d.Value.Dimensions[contentVariableCode].Category.Unit[contValue.Code].Decimals
                    )
                    .Distinct()
                    .Single();

                    contValue.ContentComponent = new ContentComponent(
                        new MultiLanguageString(unit),
                        new MultiLanguageString(source),
                        decimals,
                        updated
                    );
                }
            }
        }

        /// <summary>
        /// Applies an edition to the cube meta based on the provided query.
        /// </summary>
        /// <param name="query"><see cref="CubeQuery"/> object that contains infomration about a query.</param>
        public void ApplyEditionFromQuery(CubeQuery query)
        {
            ValidateQuery(query);

            foreach (KeyValuePair<string, VariableQuery> pair in query.VariableQueries)
            {
                Variables.Single(v => v.Code == pair.Key).ApplyEditionsFromQuery(pair.Value);
            }

            Header = this.CreateDefaultChartHeader(query);
            Header.Edit(query.ChartHeaderEdit);
        }

        internal void SetVariableTypes(JsonStat2 jsonStat2)
        {
            foreach (var variable in Variables)
            {
                if (jsonStat2.VarHasOrdinalScaleType(variable.Code))
                {
                    variable.Type = Enums.VariableType.Ordinal;
                }
                else //Default to OtherClassificatory
                {
                    variable.Type = Enums.VariableType.OtherClassificatory;
                }
            }

            if (jsonStat2.Role.Geo is string[] geoVarCodes)
            {
                foreach (string code in geoVarCodes)
                {
                    CodeToVar(code).Type = Enums.VariableType.Geological;
                }
            }

            if (jsonStat2.Role.Time is string[] timeVarCodes)
            {
                foreach (string code in timeVarCodes)
                {
                    CodeToVar(code).Type = Enums.VariableType.Time;
                }
            }

            if (jsonStat2.Role.Metric is string[] contentVarCodes)
            {
                foreach (string code in contentVarCodes)
                {
                    CodeToVar(code).Type = Enums.VariableType.Content;
                }
            }
        }

        private Variable CodeToVar(string code)
        {
            if (Variables.Find(v => v.Code == code) is Variable var) return var;
            else throw new ArgumentException($"There is no variable matching the code {code}");
        }

        private void ValidateQuery(CubeQuery query)
        {
            List<string> mismatchingVarCodes =
            [
                .. query.VariableQueries.Where(qv => !Variables.Exists(v => v.Code == qv.Key))
                .Select(kvp => kvp.Key),
                .. Variables.Where(v => !query.VariableQueries.ContainsKey(v.Code))
                .Select(v => v.Code),
            ];

            if (mismatchingVarCodes.Count != 0)
            {
                throw new ArgumentException($"Mismatch between query and metadata. The following variable codes differ: {string.Join(",", mismatchingVarCodes)}.");
            }
        }
    }
}
