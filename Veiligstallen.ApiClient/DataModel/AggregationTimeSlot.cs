using System;

namespace VeiligStallen.ApiClient.DataModel
{
    public class AggregationTimeSlot
    {
        public AggregationTimeSlot(string name, TimeSpan? min, TimeSpan? max)
        {
            Name = name;
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Name of a slot
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Left enclosing time
        /// </summary>
        public TimeSpan? Min { get; set; }

        /// <summary>
        /// If true tests ts >= Min otherwise ts > Min; defaults to true
        /// </summary>
        public bool? MinGreaterThanOrEqual { get; set; }

        /// <summary>
        /// right enclosing time
        /// </summary>
        public TimeSpan? Max { get; set; }

        /// <summary>
        /// if true tests ts <= Max otherwise ts < Max; defaults to false
        /// </summary>
        public bool? MaxLowerThanOrEqual { get; set; }


        /// <summary>
        /// Whether or not slot defines at least one time edge
        /// </summary>
        public bool HasTimeRangeDefined => Min.HasValue || Max.HasValue;

        /// <summary>
        /// Whether or not the time span fits into this aggregation slot
        /// </summary>
        /// <param name="ts"></param>
        /// <returns></returns>
        public bool Fits(TimeSpan ts)
        {
            if (
                (!Min.HasValue || (MinGreaterThanOrEqual != false ? ts.Ticks >= Min?.Ticks : ts.Ticks > Min?.Ticks)) //MinGreaterThanOrEqual defaults to true! 
                &&
                (!Max.HasValue || (MaxLowerThanOrEqual == true ? ts.Ticks <= Max?.Ticks : ts.Ticks < Max?.Ticks))//MaxLowerThanOrEqual defaults to false!
            )
                return true;

            return false;
        }
    }

}
