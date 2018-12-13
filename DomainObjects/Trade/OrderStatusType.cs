using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Trade
{
    public class OrderStatusType : IntType
    {
        public static readonly OrderStatusType Open = new OrderStatusType(0);
        public static readonly OrderStatusType Executed = new OrderStatusType(1);
        public static readonly OrderStatusType Canceled = new OrderStatusType(2);
        public static readonly OrderStatusType Close = new OrderStatusType(3);
        public static readonly OrderStatusType Finished = new OrderStatusType(4);

        private OrderStatusType(int type) : base(type)
        { }

        public static OrderStatusType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Open;
                case 1:
                    return Executed;
                case 2:
                    return Canceled;
                case 3:
                    return Close;
                case 4:
                    return Finished;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
