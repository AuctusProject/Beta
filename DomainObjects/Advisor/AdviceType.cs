using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdviceType : IntType
    {
        public static readonly AdviceType Sell = new AdviceType(0);
        public static readonly AdviceType Buy = new AdviceType(1);
        public static readonly AdviceType ClosePosition = new AdviceType(2);

        private AdviceType(int type) : base(type)
        { }

        public static AdviceType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Sell;
                case 1:
                    return Buy;
                case 2:
                    return ClosePosition;
                default:
                    throw new ArgumentException("Invalid type.");
            }
        }
    }
}
