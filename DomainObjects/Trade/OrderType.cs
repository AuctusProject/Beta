using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Trade
{
    public class OrderType : IntType
    {
        public static readonly OrderType Buy = new OrderType(1);
        public static readonly OrderType Sell = new OrderType(0);

        private OrderType(int type) : base(type)
        { }

        public static OrderType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Sell;
                case 1:
                    return Buy;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }

        public OrderType GetOppositeType()
        {
            return Value == Buy.Value ? OrderType.Sell : OrderType.Buy;
        }
    }
}
