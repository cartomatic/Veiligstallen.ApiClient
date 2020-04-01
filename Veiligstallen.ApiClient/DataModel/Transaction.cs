using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace VeiligStallen.ApiClient.DataModel
{
    /// <summary>
    /// VeiligStallen.nl transaction data response
    /// </summary>
    public class TransactionDataResponse : DataResponse
    {
        public TransactionRaw[] Data { get; set; }
    }

    /// <summary>
    /// Transaction model normalized for Trajan APIs
    /// </summary>
    public class Transaction
    {
        public DateTime? CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
    }

    /// <summary>
    /// Transaction model as output by the VeiligStallen.nl service
    /// </summary>
    public class TransactionRaw
    {
        [JsonProperty("checkindate")]
        public DateTime? CheckInDate { get; set; }

        [JsonProperty("checkoutdate")]
        public DateTime? CheckOutDate { get; set; }
    }

    public static class TransactionRawIncomingExtensions
    {
        public static Transaction AsTransaction(this TransactionRaw rawTransaction)
        {
            return new Transaction
            {
                CheckInDate = rawTransaction.CheckInDate,
                CheckOutDate = rawTransaction.CheckOutDate
            };
        }

        public static IEnumerable<Transaction> AsTransactions(this IEnumerable<TransactionRaw> rawTransactions)
        {
            return rawTransactions.Select(x => x.AsTransaction());
        }
    }
}
