using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Trade
{
    public class OrderActionType : IntType
    {
        public static readonly OrderActionType Market = new OrderActionType(0);
        public static readonly OrderActionType Limit = new OrderActionType(1);
        public static readonly OrderActionType Automated = new OrderActionType(2);
        public static readonly OrderActionType StopLoss = new OrderActionType(3);
        public static readonly OrderActionType TakeProfit = new OrderActionType(4);

        private OrderActionType(int type) : base(type)
        { }

        public static OrderActionType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Market;
                case 1:
                    return Limit;
                case 2:
                    return Automated;
                case 3:
                    return StopLoss;
                case 4:
                    return TakeProfit;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
