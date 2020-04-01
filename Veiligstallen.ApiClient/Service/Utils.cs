using System;

namespace VeiligStallen.ApiClient
{
    public partial class Service
    {
        /// <summary>
        /// Gets a dateFrom date with a default value in a case input is null
        /// </summary>
        /// <param name="date"></param>
        /// <param name="snapToMidnightAm"></param>
        /// <returns></returns>
        protected static DateTime GetDateFrom(DateTime? date = null, bool snapToMidnightAm = false)
        {
            var ret = date ?? DateTime.Now.Date.AddDays(-8);

            if (snapToMidnightAm)
                ret = ret.Date; //discards time part, so snaps to 00:00:00

            return ret;
        }

        /// <summary>
        /// Gets a dateTo date with a default value in a case input is null
        /// </summary>
        /// <param name="date"></param>
        /// <param name="snapToMidnightPm"></param>
        /// <returns></returns>
        protected static DateTime GetDateTo(DateTime? date = null, bool snapToMidnightPm = false)
        {
            var ret = date ?? DateTime.Now.Date.AddDays(-1);

            if (snapToMidnightPm)
                ret = new DateTime(ret.Year, ret.Month, ret.Day, 23, 59, 59);

            return ret;
        }

        /// <summary>
        /// Gets a data sampling interval; when null is passed, works out an interval period so the anticipated amount od data does not exceed the graph data max rec count;
        /// starts with 5 minutes time slot and keeps on increasing it until the rec count fits into an expected range or the value of time slot reaches a week:
        /// 0:05, 0:10, 0:15, 0:30, 1:00, 2:00, 4:00, 8:00, 12:00, 24:00, 2 days, 4 days, 1 week
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="dataSamplingInterval"></param>
        /// <returns></returns>
        protected static (TimeSpan interval, string name) GetDataSamplingInterval(DateTime dateFrom, DateTime dateTo, TimeSpan? dataSamplingInterval)
        {
            if (dataSamplingInterval.HasValue &&
                (dateTo.Ticks - dateFrom.Ticks) / dataSamplingInterval.Value.Ticks <= GraphDataMaxRecCount)
                return (dataSamplingInterval.Value, string.Empty);

            var samplingInterval = T5Minutes;
            while ((dateTo.Ticks - dateFrom.Ticks) / samplingInterval > GraphDataMaxRecCount)
            {
                //when reached 10 minutes, jump to 15
                if (samplingInterval == T5Minutes * 2)
                {
                    samplingInterval = T15Minutes;
                    continue;
                }

                //when 8 hrs reached, jump to 12 hrs
                if (samplingInterval == T1Hour * 8)
                {
                    samplingInterval = T12Hours;
                    continue;
                }

                //when 4 days reached, jump to 1 week and break
                if (samplingInterval == T1Day * 4)
                {
                    samplingInterval = T1Day * 7;
                    break;
                }

                samplingInterval *= 2;
            }

            var si = new TimeSpan(samplingInterval);
            var name = string.Empty;

            if (si.Days == 7)
                name = "1 week";
            else if (si.Days >= 1 && si.Days < 7)
                name = $"{si.Days} day{(si.Days > 1 ? "s" : string.Empty)}";
            else if (si.Hours >= 1 && si.Hours < 24)
                name = $"{si.Hours} hour{(si.Hours > 1 ? "s" : string.Empty)}";
            else
                name = $"{si.Minutes} minutes";

            return (si, name);
        }
    }
}
