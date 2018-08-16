using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Advisor
{
    public class AdviceModeType : IntType
    {
        public static readonly AdviceModeType Initiate = new AdviceModeType(0);
        public static readonly AdviceModeType Reiterate = new AdviceModeType(1);
        public static readonly AdviceModeType Upgrade = new AdviceModeType(2);
        public static readonly AdviceModeType Downgrade = new AdviceModeType(3);

        private AdviceModeType(int type) : base(type)
        { }

        public static AdviceModeType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Initiate;
                case 1:
                    return Reiterate;
                case 2:
                    return Upgrade;
                case 3:
                    return Downgrade;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
