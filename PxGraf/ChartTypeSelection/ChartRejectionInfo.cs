using PxGraf.Data.MetaData;
using PxGraf.Enums;
using System;

namespace PxGraf.ChartTypeSelection
{
    /// <summary>
    /// Contains infomation about why some chart type cannot be drawn from given data.
    /// </summary>
    public class ChartRejectionInfo : IComparable<ChartRejectionInfo>
    {
        /// <summary>
        /// Used to determine which rejection messages will be diplayed to the user.
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// The reason why the chart was rejected.
        /// </summary>
        public RejectionReason Reason { get; private set; }

        /// <summary>
        /// Type of the chart that can not be drawn from the data
        /// </summary>
        public VisualizationType ChartType { get; set; }

        /// <summary>
        /// The Name of the related dimension
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// The actual value measured
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Threshold for Value
        /// </summary>
        public int Threshold { get; }

        /// <summary>
        /// If error happens to be related to specific dimension value, here is the name of that
        /// </summary>
        public string InvalidValueName { get; internal set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartRejectionInfo(RejectionReason reason, int priority, VisualizationType chartType, int? value, int? threshold, VisualizationTypeSelectionObject.VariableInfo variable = null, string invalidValueName = null)
        {
            Reason = reason;
            Priority = priority;
            ChartType = chartType;
            VariableName = (variable != null) ? variable.Code : "Unknown";
            InvalidValueName = invalidValueName;

            //Do not care about nulls externally
            Threshold = threshold ?? -1;
            Value = value ?? -1;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ChartRejectionInfo(RejectionReason reason, int priority, VisualizationType chartType, int? value, int? threshold, Variable variable = null, string invalidValueName = null)
        {
            Reason = reason;
            Priority = priority;
            ChartType = chartType;
            VariableName = (variable != null) ? variable.Code : "Unknown";
            InvalidValueName = invalidValueName;

            //Do not care about nulls externally
            Threshold = threshold ?? -1;
            Value = value ?? -1;
        }

        /// <summary>
        /// Compares two instances of the ChartRejectionInfo.
        /// </summary>
        /// <param name="cri"></param>
        /// <returns>1 if the parameter object has a smaller priority, -1 of this has smaller priority, 0 if they are equal</returns>
        public int CompareTo(ChartRejectionInfo cri)
        {
            if (this.Priority > cri.Priority) return 1;
            if (this.Priority == cri.Priority) return 0;
            return -1;
        }

        /// <summary>
        /// Returns the reason as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Reason.ToString();
        }

        /// <summary>
        /// Standard smaller than implementation, uses priorities for comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            return left.CompareTo(right) < 0;
        }

        /// <summary>
        /// Standard smaller or equals, uses priorities for comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator <=(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            return left.CompareTo(right) <= 0;
        }

        /// <summary>
        /// Standard greater than implementation, uses priorities for comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            return left.CompareTo(right) > 0;
        }

        /// <summary>
        /// Standard greater or equal than implemntation, uses priorities for comparison
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator >=(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            return left.CompareTo(right) >= 0;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ChartRejectionInfo left, ChartRejectionInfo right)
        {
            return !(left == right);
        }
    }
}
