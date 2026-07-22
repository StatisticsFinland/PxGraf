#nullable enable
using PxGraf.Models.Queries;
using System.Collections.Generic;
using System.Linq;

namespace PxGraf.Services
{
    /// <summary>
    /// Validates a list of <see cref="VirtualValueDefinition"/> objects against the real dimension value codes
    /// for a given dimension.
    /// </summary>
    public interface IVirtualValueValidationService
    {
        /// <summary>
        /// Validates a list of VirtualValueDefinitions against the real dimension value codes.
        /// Returns true if valid; false and sets <paramref name="errorMessage"/> to the first error found.
        /// </summary>
        bool Validate(
            List<VirtualValueDefinition> definitions,
            IEnumerable<string> realValueCodes,
            out string? errorMessage);
    }

    /// <summary>
    /// Default implementation of <see cref="IVirtualValueValidationService"/>.
    /// </summary>
    public class VirtualValueValidationService : IVirtualValueValidationService
    {
        /// <inheritdoc/>
        public bool Validate(
            List<VirtualValueDefinition> definitions,
            IEnumerable<string> realValueCodes,
            out string? errorMessage)
        {
            HashSet<string> realCodes = [.. realValueCodes];
            HashSet<string> seenVirtualCodes = [];

            foreach (VirtualValueDefinition def in definitions)
            {
                if (string.IsNullOrEmpty(def.Code))
                {
                    errorMessage = "A virtual value definition has a null or empty Code.";
                    return false;
                }

                if (!seenVirtualCodes.Add(def.Code))
                {
                    errorMessage = $"Duplicate virtual value code: '{def.Code}'.";
                    return false;
                }

                if (realCodes.Contains(def.Code))
                {
                    errorMessage = $"Virtual value code '{def.Code}' collides with a real dimension value code.";
                    return false;
                }
            }

            // Validate operand references and minimum operand count for sum.
            // Operands may reference real codes or any declared virtual codes (order-independent).
            // Cycles are caught separately by DetectCycle below.
            HashSet<string> availableCodes = [.. realCodes, .. seenVirtualCodes];
            foreach (VirtualValueDefinition def in definitions)
            {
                foreach (string operand in def.GetOperandCodes())
                {
                    if (!availableCodes.Contains(operand))
                    {
                        errorMessage = $"Virtual value '{def.Code}' references unknown operand code '{operand}'.";
                        return false;
                    }
                }

                if (def is SumDefinition sumDef)
                {
                    int count = sumDef.OperandCodes?.Count ?? 0;
                    int minRequired = sumDef.Constant.HasValue ? 1 : 2;
                    if (count < minRequired)
                    {
                        errorMessage = $"Virtual value '{def.Code}': 'sum' requires at least {minRequired} operand code(s) (got {count}).";
                        return false;
                    }
                }

                if (def is DivisionByConstantDefinition divByConstant && System.Math.Abs(divByConstant.Constant) <= double.Epsilon)
                {
                    errorMessage = $"Virtual value '{def.Code}': division by a zero constant is not allowed.";
                    return false;
                }
            }

            errorMessage = DetectCycle(definitions) ? "Virtual values contain a Circular dependency." : null;
            return errorMessage is null;
        }

        private static bool DetectCycle(List<VirtualValueDefinition> definitions)
        {
            HashSet<string> virtualCodes = [.. definitions
                .Where(d => !string.IsNullOrEmpty(d.Code))
                .Select(d => d.Code)];

            // Build adjacency: virtual code → set of virtual codes it directly depends on.
            Dictionary<string, HashSet<string>> dependencies = [];
            Dictionary<string, int> inDegree = [];

            foreach (VirtualValueDefinition def in definitions)
            {
                dependencies[def.Code] = [];
                inDegree[def.Code] = 0;

                foreach (string operand in def.GetOperandCodes())
                {
                    if (virtualCodes.Contains(operand))
                    {
                        if (dependencies[def.Code].Add(operand))
                        {
                            inDegree[def.Code]++;
                        }
                    }
                }
            }

            // Start with nodes that have no incoming edges.
            Queue<string> queue = new(inDegree.Where(kv => kv.Value == 0).Select(kv => kv.Key));
            int processed = 0;

            while (queue.Count > 0)
            {
                string node = queue.Dequeue();
                processed++;

                // Find all definitions that depend on this node and reduce their in-degree.
                foreach (VirtualValueDefinition def in definitions)
                {
                    if (!dependencies.ContainsKey(def.Code)) continue;
                    if (!dependencies[def.Code].Contains(node)) continue;

                    inDegree[def.Code]--;
                    if (inDegree[def.Code] == 0)
                    {
                        queue.Enqueue(def.Code);
                    }
                }
            }

            return processed < virtualCodes.Count;
        }
    }
}
