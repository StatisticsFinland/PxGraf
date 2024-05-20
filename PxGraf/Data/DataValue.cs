using System;
using System.Diagnostics;
using System.Globalization;

namespace PxGraf.Data
{
    public enum DataValueType
    {
        Exist = 0,
        Missing = 1,
        CannotRepresent = 2,
        Confidential = 3,
        NotAcquired = 4,
        NotAsked = 5, //or UnderConstruction / Error
        Empty = 6, //Actual usage unclear
        Nill = 7,  //Actual usage unclear
    }

    [DebuggerDisplay("{Type == DataValueType.Exist ? (object)Value : (object)Type}")]
    public readonly struct DataValue(double value, DataValueType type)
    {
        //Indexed by DataValueType with offset of one
        private static readonly string[] MissingValueDotCodes =
        [
            ".",
            "..",
            "...",
            "....",
            ".....",
            "......",
            "-",
        ];

        public readonly DataValueType Type = type;

        public readonly double UnsafeValue = value;

        public double Value
        {
            get
            {
                if (Type == DataValueType.Exist)
                {
                    return UnsafeValue;
                }
                else
                {
                    throw new InvalidOperationException("Value does not exist");
                }
            }
        }

        public static implicit operator double?(DataValue d) => d.Type == DataValueType.Exist ? d.Value : null;

        public static DataValue FromRaw(double value)
        {
            return new DataValue(value, DataValueType.Exist);
        }

        public static DataValue FromDotCode(string code, DataValueType defaultType)
        {
            var index = Array.IndexOf(MissingValueDotCodes, code);
            if (index == -1)
            {
                return new DataValue(0, defaultType);
            }
            else
            {
                return new DataValue(0, (DataValueType)(index + 1));
            }
        }

        public bool TryGetValue(out double value)
        {
            if (Type == DataValueType.Exist)
            {
                value = UnsafeValue;
                return true;
            }
            else
            {
                value = 0;
                return false;
            }
        }

        public double GetOrDefault(double defaultValue)
        {
            if (Type == DataValueType.Exist)
            {
                return UnsafeValue;
            }
            else
            {
                return defaultValue;
            }
        }

        public string ToHumanReadableString(int decimals, CultureInfo formatProvider)
        {
            if (TryGetValue(out double tryValue))
            {
                /*
                  .NET Core 2.1+ uses MidpointRounding.ToEven style rounding inside double.ToString("N#") so we need to use explicit rounding.
                  Examples:
                    ((double)10.5).ToString("0") => 11 // .NET Framework 4
                    ((double)10.5).ToString("0") => 11 // .NET Core 3.1
                    ((double)10.5).ToString("N0") => 11 // .NET Framework 4
                    ((double)10.5).ToString("N0") => 10 // .NET Core 3.1 !!!
                */
                var roundedValue = Math.Round(tryValue, decimals, MidpointRounding.AwayFromZero);
                return roundedValue.ToString("N" + decimals, formatProvider);
            }
            else
            {
                return MissingValueDotCodes[(int)Type - 1];
            }
        }

        public string ToMachineReadableString(int decimals)
        {
            if (TryGetValue(out double tryValue))
            {
                var formatString = "0." + new string('0', decimals);
                return tryValue.ToString(formatString, CultureInfo.InvariantCulture);
            }
            else
            {
                return MissingValueDotCodes[(int)Type - 1];
            }
        }
    }
}

