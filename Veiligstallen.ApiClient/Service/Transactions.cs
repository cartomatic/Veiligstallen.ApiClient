using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VeiligStallen.ApiClient.DataModel;

namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Reads transactions data for given city & location and returns raw response; credentials supplied as user name & pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static async Task<TransactionDataResponse> GetTransactionsDataPassThroughAsync(
            string user, string pass, string cityCode, string locationId,
            DateTime? dateFrom, DateTime? dateTo)
        {
            return await GetTransactionsDataPassThroughInternalAsync(GetAuthorizationHeaderValue(user, pass), cityCode, locationId, dateFrom, dateTo);
        }

        /// <summary>
        /// Reads transactions data for given city & location and returns raw response; credentials supplied as authorization token
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public static async Task<TransactionDataResponse> GetTransactionsDataPassThroughAsync(
            string authToken, string cityCode, string locationId,
            DateTime? dateFrom, DateTime? dateTo)
        {
            return await GetTransactionsDataPassThroughInternalAsync(GetAuthorizationHeaderValue(authToken), cityCode, locationId, dateFrom, dateTo);
        }

        /// <summary>
        /// Reads transactions data for given city & location and returns raw response; credentials supplied as authorization header value
        /// </summary>
        /// <param name="authHdr"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        private static async Task<TransactionDataResponse> GetTransactionsDataPassThroughInternalAsync(string authHdr, string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo)
        {
            var cfg = ServiceConfig.Read();

            try
            {
                var dateFromFixed = GetDateFrom(dateFrom);
                var dateToFixed = GetDateTo(dateTo);
                if (dateToFixed == dateFromFixed)
                {
                    dateToFixed = new DateTime(dateToFixed.Year, dateToFixed.Month, dateToFixed.Day, 23, 59, 59);
                }

                var transactionsData = await ApiCall<TransactionDataResponse>(
                    cfg.Endpoint,
                    cfg.Routes.LocationTransactions
                        .Replace("{city_code}", cityCode)
                        .Replace("{location_id}", locationId)
                        .Replace("{date_from}", dateFromFixed.ToString("s"))
                        .Replace("{date_to}", dateToFixed.ToString("s")),
                    authHdr
                );

                if (transactionsData?.Data == null || !transactionsData.Data.Any())
                    return null;

                return transactionsData;
            }
            catch
            {
                //ignore
            }

            return null;
        }

        /// <summary>
        /// Returns transactions data aggregated as required for charts; credentials supplied as user name & pass
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dataSamplingInterval"></param>
        /// <param name="dataAggTimeSlots"></param>
        /// <returns></returns>
        public static async Task<TransactionsChartData> GetTransactionsChartDataAsync(string user, string pass, string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo,
            TimeSpan? dataSamplingInterval = null, AggregationTimeSlot[] dataAggTimeSlots = null)
        {
            return await GetTransactionsChartDataInternalAsync(GetAuthorizationHeaderValue(user, pass), cityCode,
                locationId, dateFrom, dateTo, dataSamplingInterval, dataAggTimeSlots);
        }

        /// <summary>
        /// Returns transactions data aggregated as required for charts; credentials supplied as authorization token
        /// </summary>
        /// <param name="authToken"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dataSamplingInterval"></param>
        /// <param name="dataAggTimeSlots"></param>
        /// <returns></returns>
        public static async Task<TransactionsChartData> GetTransactionsChartDataAsync(string authToken, string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo,
            TimeSpan? dataSamplingInterval = null, AggregationTimeSlot[] dataAggTimeSlots = null)
        {
            return await GetTransactionsChartDataInternalAsync(GetAuthorizationHeaderValue(authToken), cityCode,
                locationId, dateFrom, dateTo, dataSamplingInterval, dataAggTimeSlots);
        }

        /// <summary>
        /// Returns transactions data aggregated as required for charts; credentials supplied as authorization header value
        /// </summary>
        /// <param name="authHdr"></param>
        /// <param name="cityCode"></param>
        /// <param name="locationId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dataSamplingInterval"></param>
        /// <param name="dataAggTimeSlots"></param>
        /// <returns></returns>
        private static async Task<TransactionsChartData> GetTransactionsChartDataInternalAsync(string authHdr, string cityCode, string locationId, DateTime? dateFrom, DateTime? dateTo, TimeSpan? dataSamplingInterval = null, AggregationTimeSlot[] dataAggTimeSlots = null)
        {
            //auto default the dates, so can work out auto dataGranularity
            var dateFromFixed = GetDateFrom(dateFrom, snapToMidnightAm: true);
            var dateToFixed = GetDateTo(dateTo, snapToMidnightPm: true);

            //get raw data first
            var transactionsData =
                await GetTransactionsDataPassThroughInternalAsync(authHdr, cityCode, locationId, dateFromFixed,
                    dateToFixed);

            if (transactionsData?.Data == null || transactionsData.Data.Length == 0)
                return null;

            //key is snapped date (agg period)
            var (samplingInterval, samplingIntervalName) = GetDataSamplingInterval(dateFromFixed, dateToFixed, dataSamplingInterval);
            var aggTimeSlots = GetTransactionsDataAggregationSlots(dataAggTimeSlots);
            var agg = GetTransactionsDataAggCollector(dateFromFixed, dateToFixed, samplingInterval, aggTimeSlots);


            foreach (var transaction in transactionsData.Data.AsTransactions())
            {
                //discard all the transactions without checkin / checkout dates
                if (!transaction.CheckInDate.HasValue && !transaction.CheckOutDate.HasValue)
                    continue;

                foreach (var aggKey in agg.Keys)
                {
                    if (transaction.CheckInDate.HasValue && transaction.CheckOutDate.HasValue)
                    {
                        if (transaction.CheckInDate?.Ticks <= aggKey && transaction.CheckOutDate?.Ticks > aggKey)
                        {
                            foreach (var aggSlot in agg[aggKey].Keys.ToList())
                            {
                                if (aggSlot.HasTimeRangeDefined && aggSlot.Fits(new TimeSpan(transaction.CheckOutDate.Value.Ticks - transaction.CheckInDate.Value.Ticks)))
                                    agg[aggKey][aggSlot] += 1;
                            }
                        }
                    }
                    //check in date only means this bike is checked in for given period
                    else if (transaction.CheckInDate.HasValue && !transaction.CheckOutDate.HasValue)
                    {
                        if (transaction.CheckInDate?.Ticks <= aggKey)
                        {
                            //find 'check in' agg slot
                            var checkedInKey = agg[aggKey].Keys.FirstOrDefault(k => !k.HasTimeRangeDefined);

                            agg[aggKey][checkedInKey] += 1;
                        }
                    }
                }
            }

            //having sampled the data, need to output it in somewhat more usable form;

            return new TransactionsChartData
            {
                DateFrom = dateFromFixed,
                DateTo = dateToFixed,
                SamplingInterval = samplingInterval,
                SamplingIntervalName = samplingIntervalName,
                DataAggregationTimeSlots = aggTimeSlots,
                Data = agg.Select(kv => new TransactionsChartDataRecord
                {
                    TimeStampSort = kv.Key,
                    TimeStamp = new DateTime(kv.Key),
                    Data = kv.Value.Select(kv2 => kv2.Value).ToArray()
                }).ToArray()
            };
        }


        /// <summary>
        /// Max count of the graph data rows. used when auto calculating data snap period
        /// </summary>
        protected const int GraphDataMaxRecCount = 500;

        /// <summary>
        /// 5 minutes in ticks
        /// </summary>
        protected const long T5Minutes = 3000000000;

        /// <summary>
        /// 15 minutes in ticks
        /// </summary>
        protected const long T15Minutes = 9000000000;

        /// <summary>
        /// 1 hour in ticks
        /// </summary>
        protected const long T1Hour = T15Minutes * 4;

        /// <summary>
        /// 12 hrs in ticks
        /// </summary>
        protected const long T12Hours = T1Hour * 12;

        /// <summary>
        /// 1 day in ticks
        /// </summary>
        protected const long T1Day = T1Hour * 24;


        /// <summary>
        /// Gets an aggregation container for the transactions data
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="samplingInterval"></param>
        /// <param name="aggTimeSlots"></param>
        /// <returns></returns>
        protected static SortedDictionary<long, Dictionary<AggregationTimeSlot, int>> GetTransactionsDataAggCollector(DateTime dateFrom, DateTime dateTo, TimeSpan samplingInterval, AggregationTimeSlot[] aggTimeSlots)
        {
            var output = new SortedDictionary<long, Dictionary<AggregationTimeSlot, int>>();

            var currentSamplingTime = dateFrom.Ticks;
            while (currentSamplingTime < dateTo.Ticks)
            {
                output.Add(currentSamplingTime, aggTimeSlots.ToDictionary(x => x, x => 0));
                currentSamplingTime += samplingInterval.Ticks;
            }

            return output;
        }



        /// <summary>
        /// Gets data aggregation time slots for the transactions data processing; when no input is provided, returns default slots.
        /// </summary>
        /// <param name="aggSlots"></param>
        /// <returns></returns>
        public static AggregationTimeSlot[] GetTransactionsDataAggregationSlots(AggregationTimeSlot[] aggSlots)
        {
            if (aggSlots != null && aggSlots.Length > 0)
                return aggSlots;

            return new[]
            {
                new AggregationTimeSlot("0 - 1 hour", null, new TimeSpan(1,0,0)),
                new AggregationTimeSlot("1 - 3 hours", new TimeSpan(1,0,0), new TimeSpan(3,0,0)),
                new AggregationTimeSlot("3 - 6 hours", new TimeSpan(3,0,0), new TimeSpan(6,0,0)),
                new AggregationTimeSlot("6 - 9 hours", new TimeSpan(6,0,0), new TimeSpan(9,0,0)),
                new AggregationTimeSlot("9 - 13 hours", new TimeSpan(9,0,0), new TimeSpan(13,0,0)),
                new AggregationTimeSlot("13 - 18 hours", new TimeSpan(13,0,0), new TimeSpan(18,0,0)),
                new AggregationTimeSlot("18 - 24 hours", new TimeSpan(18,0,0), new TimeSpan(24,0,0)),
                new AggregationTimeSlot("24 - 36 hours", new TimeSpan(24,0,0), new TimeSpan(36,0,0)),
                new AggregationTimeSlot("36 - 48 hours", new TimeSpan(36,0,0), new TimeSpan(48,0,0)),
                new AggregationTimeSlot("48 hours - 1 week", new TimeSpan(48,0,0), new TimeSpan(7,0,0,0)),
                new AggregationTimeSlot("1 - 2 weeks", new TimeSpan(7,0,0,0), new TimeSpan(14,0,0,0)),
                new AggregationTimeSlot("2 - 3 weeks", new TimeSpan(14,0,0,0), new TimeSpan(21,0,0,0)),
                new AggregationTimeSlot("more than 3 weeks", new TimeSpan(21,0,0,0), null),
                new AggregationTimeSlot("checked in", null,null)
            };
        }

    }
}
