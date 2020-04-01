
using System;

namespace VeiligStallen.ApiClient.DataModel
{
    /// <summary>
    /// Transactions data holder for chart display
    /// </summary>
    public class TransactionsChartData
    {
        /// <summary>
        /// Date range start
        /// </summary>
        public DateTime? DateFrom { get; set; }

        /// <summary>
        /// Date range end
        /// </summary>
        public DateTime? DateTo { get; set; }

        /// <summary>
        /// Sampling interval
        /// </summary>
        public TimeSpan SamplingInterval { get; set; }

        /// <summary>
        /// Sampling interval friendly name if any
        /// </summary>
        public string SamplingIntervalName { get; set; }

        /// <summary>
        /// Time slots the data have been aggregated to
        /// </summary>
        public AggregationTimeSlot[] DataAggregationTimeSlots { get; set; }

        /// <summary>
        /// The actual aggregated data
        /// </summary>
        public TransactionsChartDataRecord[] Data { get; set; }
    }

    /// <summary>
    /// Transactions data single rec for the chart
    /// </summary>
    public class TransactionsChartDataRecord
    {
        /// <summary>
        /// sortable time stamp (ticks)
        /// </summary>
        public long TimeStampSort { get; set; }

        /// <summary>
        /// Time stamp
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// data aggregated for declared AggregationTimeSlots
        /// </summary>
        public int[] Data { get; set; }
    }

}
