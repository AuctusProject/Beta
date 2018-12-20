using System;
using System.Collections.Generic;
using System.Text;
using static Auctus.Model.AssetResponse;

namespace Auctus.Model
{
    public class OrderResponse
    {
        public int Id { get; set; }
        public DateTime CreationDate { get; set; }
        public int AdvisorId { get; set; }
        public string AdvisorName { get; set; }
        public string AdvisorDescription { get; set; }
        public string AdvisorGuid { get; set; }
        public int? AdvisorRanking { get; set; }
        public double? AdvisorRating { get; set; }
        public bool FollowingAdvisor { get; set; }
        public int AssetId { get; set; }
        public string AssetCode { get; set; }
        public string AssetName { get; set; }
        public bool? ShortSellingEnabled { get; set; }
        public PairResponse Pair { get; set; }
        public bool FollowingAsset { get; set; }
        public int ActionType { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public DateTime StatusDate { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public double Invested { get; set; }
        public double? TakeProfit { get; set; }
        public double? StopLoss { get; set; }
        public double? Profit { get; set; }
        public double? ProfitValue { get; set; }
        public double? ProfitWithoutFee { get; set; }
        public double? ProfitWithoutFeeValue { get; set; }
        public double? Fee { get; set; }
        public int? OrderId { get; set; }
        public DateTime? OpenDate { get; set; }
        public double? OpenPrice { get; set; }
        public double RemainingQuantity { get; set; }
        public bool CanBeEdited { get; set; }
    }
}
