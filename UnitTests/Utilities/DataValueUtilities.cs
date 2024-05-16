using PxGraf.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.Utilities
{
    public static class DataValueUtilities
    {
        /// <summary>
        /// Helper to generate DataValues from doubles for example expected data.
        /// </summary>
        /// <param name="pxValues">Double in px format.</param>
        public static DataValue[] Row(params double[] pxValues)
        {
            return pxValues.Select(DataValue.FromRaw).ToArray();
        }

        /// <summary>
        /// Helper to generate DataValues from doubles for example expected data.
        /// </summary>
        /// <param name="pxValues">Double in px format.</param>
        public static IReadOnlyList<DataValue> List(params double[] pxValues)
        {
            return pxValues.Select(DataValue.FromRaw).ToList();
        }

        /// <summary>
        /// Compares two jagged arrays of DataValues and returns if thay have the same content.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Compare(DataValue[][] expected, DataValue[][] actual, out string message, double delta = 0.0)
        {
            if (expected.Length != actual.Length)
            {
                message = $"The expected array length is {expected.Length}, but the actual length was {actual.Length}";
                return false;
            }
            else
            {
                for (int outer_i = 0; outer_i < expected.Length; outer_i++)
                {
                    if (expected[outer_i].Length != actual[outer_i].Length)
                    {
                        message = $"Expected length of expected[{outer_i}] is {expected[outer_i].Length}, but actual length was {actual[outer_i].Length}.";
                        return false;
                    }

                    for (int inner_i = 0; inner_i < expected[outer_i].Length; inner_i++)
                    {
                        if (expected[outer_i][inner_i].Type is DataValueType.Exist && actual[outer_i][inner_i].Type is DataValueType.Exist)
                        {
                            if (Math.Abs(expected[outer_i][inner_i].Value - actual[outer_i][inner_i].Value) > delta)
                            {
                                string deltaStr = "";
                                if (delta > 0) deltaStr = $" +/- {delta}";
                                message = $"The expected value at [{outer_i}][{inner_i}] is {expected[outer_i][inner_i].Value}{deltaStr}, but the actual value was {actual[outer_i][inner_i].Value}.";
                                return false;
                            }
                        }
                        else if (expected[outer_i][inner_i].Type != actual[outer_i][inner_i].Type)
                        {
                            message = $"The expected type at [{outer_i}][{inner_i}] is {expected[outer_i][inner_i].Type}, but the actual type was {actual[outer_i][inner_i].Type}.";
                            return false;
                        }
                    }
                }

                message = "OK";
                return true;
            }
        }

        /// <summary>
        /// Compares two jagged arrays of DataValues and returns if thay have the same content.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool Compare(IReadOnlyList<DataValue> expected, IReadOnlyList<DataValue> actual, out string message, double delta = 0.0)
        {
            if (expected.Count != actual.Count)
            {
                message = $"The expected list count is {expected.Count}, but the actual count was {actual.Count}";
                return false;
            }
            else
            {
                for (int i = 0; i < actual.Count; i++)
                {
                    if (expected[i].Type is DataValueType.Exist && actual[i].Type is DataValueType.Exist)
                    {
                        if (Math.Abs(expected[i].Value - actual[i].Value) > delta)
                        {
                            string deltaStr = "";
                            if (delta > 0) deltaStr = $" +/- {delta}";
                            message = $"The expected value at [{i}] is {expected[i].Value}{deltaStr}, but the actual value was {actual[i].Value}.";
                            return false;
                        }
                    }
                    else if (expected[i].Type != actual[i].Type)
                    {
                        message = $"The expected type at [{i}] is {expected[i].Type}, but the actual type was {actual[i].Type}.";
                        return false;
                    }
                }
            }

            message = "OK";
            return true;
        }
    }
}
