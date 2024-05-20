using System;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Represents a range between two positive integers
    /// </summary>
    public class DimensionRange
    {
        /// <summary>
        /// Upper limit of the range
        /// </summary>
        public int Min { get; private set; }

        /// <summary>
        /// Lower limit of the range
        /// </summary>
        public int Max { get; private set; }

        /// <summary>
        /// Tells if the dimension corresponding this range is allowed.
        /// </summary>
        public bool DimensionRequired => Min > 0;

        /// <summary>
        /// Tells if the dimension corresponding this range is not allowed.
        /// </summary>
        public bool DimensionNotAllowed => Max == 0;

        /// <summary>
        /// Tells if this limit range should be ignored or not.
        /// </summary>
        public bool Ignore
        {
            get => Min == 0 && Max == int.MaxValue;
            private set
            {
                if (value)
                {
                    Min = 0;
                    Max = int.MaxValue;
                }
            }
        }

        /// <summary>
        /// Default construtor, the dimension related to this range will not be allowed by default.
        /// </summary>
        public DimensionRange()
        {
            Ignore = true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public DimensionRange(int min, int max)
        {
            CheckAndSet(min, max);
        }

        /// <summary>
        /// Builds a range based on input string.
        /// </summary>
        /// <param name="input">Must be in the format of: "min-max"</param>
        public DimensionRange(string input)
        {
            string trimmedInput = input.Replace(" ", "");

            if (trimmedInput.Equals("NOTALLOWED", StringComparison.CurrentCultureIgnoreCase) || trimmedInput == "0" || trimmedInput == "")
            {
                Min = 0;
                Max = 0;
                return;
            }

            if (trimmedInput.Equals("IGNORE", StringComparison.CurrentCultureIgnoreCase))
            {
                Min = 0;
                Max = int.MaxValue;
                return;
            }

            int min, max;

            if (trimmedInput.Contains('-'))
            {
                try
                {
                    string[] values = trimmedInput.Split('-');

                    if (values.Length == 1)
                    {
                        throw new ArgumentException($"{trimmedInput} is not valid range for limits", nameof(input));
                    }
                    else if (values.Length == 2)
                    {
                        min = int.Parse(values[0]);
                        max = int.Parse(values[1]);
                    }
                    else throw new ArgumentException("The input cannot contain more than two values separated by '-'");
                }
                catch (Exception e) // If parsing throws or lower < 0 || upper < lower
                {
                    throw new ArgumentException($"{trimmedInput} is not valid range for limits", nameof(input), e);
                }
            }
            else
            {
                try
                {
                    min = int.Parse(trimmedInput);
                    max = int.Parse(trimmedInput);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"{trimmedInput} is not valid input for generating a range", nameof(input), e);
                }
            }

            CheckAndSet(min, max);
        }

        private void CheckAndSet(int min, int max)
        {
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(min), $"The minumum value of the range cannot be negative, was {min}");
            if (max < 0) throw new ArgumentOutOfRangeException(nameof(min), $"The maximum value of the range cannot be negative, was {max}");
            if (min > max) throw new ArgumentOutOfRangeException(nameof(min), $"The maximum cannot be smaller than the minum, max was {max} and min was {min}");

            Min = min;
            Max = max;
        }

        /// <summary>
        /// Returns the range as a string, formatted Min-Max
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (DimensionNotAllowed) return "Not allowed";
            if (Ignore) return "Ignore";
            return $"{Min}-{Max}";
        }
    }
}
