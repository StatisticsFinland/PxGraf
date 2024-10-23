using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Tools.PxUtilsIntegrationTestTool
{
    internal class ResponseComparer : Command
    {
        private string _resultsLocation = string.Empty;
        private string[] _queries = [];
        private Dictionary<string, string> _rejected = [];
        private Dictionary<string, List<string>> _whitelist = [];
        private string _responseLocation = string.Empty;

        internal override async Task Start()
        {
            InitiateResults();
            _responseLocation = Program.Config.Paths.ResponseDirectory;
            _queries = await File.ReadAllLinesAsync(Program.Config.Paths.QueriesFile);

            await IterateResponses();
        }

        private void InitiateResults()
        {
            _resultsLocation = Program.Config.Paths.ResultsDirectory;
            if (string.IsNullOrEmpty(_resultsLocation))
            {
                throw new InvalidOperationException("Results directory is not set in the configuration file.");
            }

            ToolsUtilities.CreateDirectoryOrRemoveContents(_resultsLocation, true);

            string acceptedPath = Path.Combine(_resultsLocation, TokenConstants.ACCEPTED_FILE);
            string rejectedPath = Path.Combine(_resultsLocation, TokenConstants.REJECTED_FILE);
            string whitelistPath = Path.Combine(_resultsLocation, TokenConstants.WHITELIST_FILE);
            if (!File.Exists(acceptedPath))
                File.Create(acceptedPath).Close();
            if (!File.Exists(rejectedPath))
                File.Create(rejectedPath).Close();
            if (!File.Exists(whitelistPath))
                File.Create(whitelistPath).Close();
        }

        private async Task IterateResponses()
        {
            string acceptedPath = Path.Combine(_resultsLocation, TokenConstants.ACCEPTED_FILE);
            string rejectedPath = Path.Combine(_resultsLocation, TokenConstants.REJECTED_FILE);
            string whitelistPath = Path.Combine(_resultsLocation, TokenConstants.WHITELIST_FILE);

            string[] acceptedArray = await File.ReadAllLinesAsync(acceptedPath);
            List<string> _accepted = [.. acceptedArray];

            _rejected = JsonConvert.DeserializeObject<Dictionary<string, string>>(await File.ReadAllTextAsync(rejectedPath)) ?? [];
            _whitelist = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(await File.ReadAllTextAsync(whitelistPath)) ?? [];

            foreach (string query in _queries)
            {
                if (_accepted.Contains(query))
                {
                    Console.WriteLine($"Query: {query} has already been accepted.");
                    continue;
                }
                else if (_rejected.TryGetValue(query, out string? value))
                {
                    Console.WriteLine($"Query: {query} has been rejected. Reason: {value}. Do you want to re-evaluate this query? (y/n):");
                    if (!ToolsUtilities.GetBooleanAnswer())
                        continue;
                }

                if (CompareResults(query, TokenConstants.RESPONSE_SQ) &&
                CompareResults(query, TokenConstants.RESPONSE_SQMETA) &&
                CompareResults(query, TokenConstants.RESPONSE_SQVISUALIZATION))
                {                     
                    Console.WriteLine($"Query: {query} has been accepted.");
                    _accepted.Add(query);
                    await File.WriteAllLinesAsync(Path.Combine(_resultsLocation, TokenConstants.ACCEPTED_FILE), _accepted);

                    if(_rejected.ContainsKey(query))
                        _rejected.Remove(query);
                    await File.WriteAllTextAsync(Path.Combine(_resultsLocation, TokenConstants.REJECTED_FILE), JsonConvert.SerializeObject(_rejected));
                }
            }
        }

        private bool CompareResults(string query, string responseToken)
        {
            string pxUtilsPath = Path.Combine(_responseLocation, TokenConstants.DATASOURCE_PXUTILS, responseToken, query + ".json");
            string pxWebApiPath = Path.Combine(_responseLocation, TokenConstants.DATASOURCE_PXWEBAPI, responseToken, query + ".json");
            string pxWebOldPath = Path.Combine(_responseLocation, TokenConstants.DATASOURCE_OLD, responseToken, query + ".json");

            string pxUtilsResponse = File.ReadAllText(pxUtilsPath);
            string pxWebApiResponse = File.ReadAllText(pxWebApiPath);
            string pxWebOldResponse = File.ReadAllText(pxWebOldPath);

            KeyValuePair<string, string> pxUtils = new (TokenConstants.DATASOURCE_PXUTILS, pxUtilsResponse);
            KeyValuePair<string, string> pxWebApi = new (TokenConstants.DATASOURCE_PXWEBAPI, pxWebApiResponse);
            KeyValuePair<string, string> pxWebOld = new (TokenConstants.DATASOURCE_OLD, pxWebOldResponse);

            if (pxUtilsResponse != pxWebApiResponse &&
                CompareDeserializedObjects(pxUtils, pxWebApi, responseToken) > 0 &&
                DifferenceControls(query, pxUtilsPath, pxWebApiPath, responseToken))
            {
                return false;
            }

            if (pxUtilsResponse != pxWebOldResponse &&
                CompareDeserializedObjects(pxUtils, pxWebOld, responseToken) > 0 &&
                DifferenceControls(query, pxUtilsPath, pxWebOldPath, responseToken))
            {
                return false;
            }

            return true;
        }

        private bool DifferenceControls(string query, string resp1, string resp2, string responseToken)
        {
            Console.WriteLine($"Select an option to handle {responseToken} response for {query}:");
            Console.WriteLine("1. Open diff in VSCode editor");
            Console.WriteLine("2. Accept reviewed differences");
            Console.WriteLine("3. Reject the query");
            int option = ToolsUtilities.GetNumericAnswer(3);
            switch (option)
            {
                case 1:
                    OpenDiffInEditor(resp1, resp2);
                    return DifferenceControls(query, resp1, resp2, responseToken);
                case 2:
                    return false;
                case 3:
                    ReportRejection(query);
                    return true;
            }
            return true;
        }

        private void ReportRejection(string query)
        {
            Console.WriteLine("Enter a reason for rejecting the response: ");
            string? reason = Console.ReadLine();
            if (string.IsNullOrEmpty(reason))
            {
                Console.WriteLine("Invalid input. Try again.");
                ReportRejection(query);
            }
            if (!_rejected.TryAdd(query, reason))
            {
                _rejected[query] = reason;
            }
            File.WriteAllText(Path.Combine(_resultsLocation, TokenConstants.REJECTED_FILE), JsonConvert.SerializeObject(_rejected));
        }

        private static void OpenDiffInEditor(string resp1, string resp2)
        {
            string json1 = File.ReadAllText(resp1);
            string json2 = File.ReadAllText(resp2);

            string formattedJson1 = JToken.Parse(json1).ToString(Formatting.Indented);
            string formattedJson2 = JToken.Parse(json2).ToString(Formatting.Indented);

            string temp1 = $"diff1-temp.json";
            string temp2 = $"diff2-temp.json";

            File.WriteAllText(temp1, formattedJson1);
            File.WriteAllText(temp2, formattedJson2);

            var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "code",
                    Arguments = $"--diff \"{temp1}\" \"{temp2}\"",
                    RedirectStandardOutput = false,
                    UseShellExecute = true,
                    CreateNoWindow = false
                }
            };
            process.Start();

            process.WaitForExit();
            File.Delete(temp1);
            File.Delete(temp2);
        }

        private int CompareDeserializedObjects(KeyValuePair<string, string> resp1, KeyValuePair<string, string> resp2, string responseToken)
        {
            JObject obj1 = JObject.Parse(resp1.Value);
            JObject obj2 = JObject.Parse(resp2.Value);

            KeyValuePair<string, JToken> kvp1 = new (resp1.Key, obj1);
            KeyValuePair<string, JToken> kvp2 = new (resp2.Key, obj2);

            var differences = GetDifferences(kvp1, kvp2, responseToken);
            if (differences.Count > 0)
            {
                Console.WriteLine("Differences found:");
                foreach (var difference in differences)
                {
                    Console.WriteLine(difference);
                }
            }
            else
            {
                Console.WriteLine("No differences found.");
            }
            return differences.Count;
        }

        private List<string> GetDifferences(KeyValuePair<string, JToken> kvp1, KeyValuePair<string, JToken> kvp2, string responseToken)
        {
            List<string> differences = [];
            CompareJTokens(kvp1, kvp2, differences, "", responseToken);

            return differences;
        }

        private void CompareJTokens(KeyValuePair<string, JToken> token1, KeyValuePair<string, JToken> token2, List<string> differences, string path, string responseToken)
        {
            if (token1.Value.Type != token2.Value.Type)
            {
                differences.Add($"Type mismatch at {path}: {token1.Value.Type} vs {token2.Value.Type} when comparing {token1.Key} and {token2.Key}");
                return;
            }

            if (_whitelist.TryGetValue(responseToken, out List<string>? whitelist) && whitelist.Contains(path))
            {
                Console.WriteLine($"{path} a is whitelisted exception");
                return;
            }

            switch (token1.Value.Type)
            {
                case JTokenType.Object:
                    var obj1 = (JObject)token1.Value;
                    var obj2 = (JObject)token2.Value;
                    var allKeys = new HashSet<string>(obj1.Properties().Select(p => p.Name).Union(obj2.Properties().Select(p => p.Name)));
                    foreach (var key in allKeys)
                    {
                        var childPath = string.IsNullOrEmpty(path) ? key : $"{path}.{key}";
                        KeyValuePair<string, JToken> kvp1 = new (token1.Key, obj1[key]);
                        KeyValuePair<string, JToken> kvp2 = new (token2.Key, obj2[key]);
                        CompareJTokens(kvp1, kvp2, differences, childPath, responseToken);
                    }
                    break;

                case JTokenType.Array:
                    var arr1 = (JArray)token1.Value;
                    var arr2 = (JArray)token2.Value;
                    if (arr1.Count != arr2.Count)
                    {
                        if (!PromptWhitelisting(responseToken, path))
                        {
                            differences.Add($"Array length mismatch at {path}: {arr1.Count} vs {arr2.Count} when comparing {token1.Key} and {token2.Key}");
                        }
                    }
                    else
                    {
                        if (responseToken == TokenConstants.RESPONSE_SQVISUALIZATION && path == TokenConstants.DATA_PATH)
                        {
                            CompareData(arr1, arr2, differences);
                            break;
                        }
                        for (int i = 0; i < arr1.Count; i++)
                        {
                            KeyValuePair<string, JToken> kvp1 = new (token1.Key, arr1[i]);
                            KeyValuePair<string, JToken> kvp2 = new (token2.Key, arr2[i]);
                            CompareJTokens(kvp1, kvp2, differences, $"{path}[{i}]", responseToken);
                        }
                    }
                    break;

                default:
                    if (!JToken.DeepEquals(token1.Value, token2.Value) && !PromptWhitelisting(responseToken, path))
                    {
                        differences.Add($"Value mismatch at {path}: {token1.Value} vs {token2.Value} when comparing {token1.Key} and {token2.Key}");
                    }
                    break;
            }
        }

        private static void CompareData(JArray arr1, JArray arr2, List<string> differences)
        {
            for (int i = 0; i < arr1.Count; i++)
            {
                JToken item1 = arr1[i];
                JToken item2 = arr2[i];

                if (item1.Type != JTokenType.Float || item2.Type != JTokenType.Float)
                {
                    differences.Add($"Data type mismatch at {TokenConstants.DATA_PATH}[{i}]: {item1.Type} vs {item2.Type} when comparing {TokenConstants.RESPONSE_SQVISUALIZATION} and {TokenConstants.DATASOURCE_PXWEBAPI}");
                    return;
                }

                decimal value1 = item1.Value<decimal>();
                decimal value2 = item2.Value<decimal>();

                int decimalPlaces1 = BitConverter.GetBytes(decimal.GetBits(value1)[3])[2];
                int decimalPlaces2 = BitConverter.GetBytes(decimal.GetBits(value2)[3])[2];
                int decimalPlaces = Math.Min(decimalPlaces1, decimalPlaces2);

                decimal roundedValue1 = Math.Round(value1, decimalPlaces);
                decimal roundedValue2 = Math.Round(value2, decimalPlaces);

                if (roundedValue1 != roundedValue2)
                {
                    differences.Add($"Data mismatch at {TokenConstants.DATA_PATH}[{i}]: {value1} vs {value2} when comparing {TokenConstants.RESPONSE_SQVISUALIZATION} and {TokenConstants.DATASOURCE_PXWEBAPI}");
                }
            }
        }

        private bool PromptWhitelisting(string responseToken, string path)
        {
            Console.WriteLine($"Do you want to whitelist this property: {path} for {responseToken}? (y/n)");
            if (ToolsUtilities.GetBooleanAnswer())
            {
                if (!_whitelist.TryGetValue(responseToken, out List<string>? whitelist))
                {
                    whitelist = [];
                    _whitelist.Add(responseToken, whitelist);
                }
                whitelist.Add(path);
                File.WriteAllText(Path.Combine(_resultsLocation, TokenConstants.WHITELIST_FILE), JsonConvert.SerializeObject(_whitelist));
                return true;
            }
            return false;
        }
    }
}
