using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class ReferralStatusType : IntType
    {
        public static readonly ReferralStatusType InProgress = new ReferralStatusType(0);
        public static readonly ReferralStatusType Interrupted = new ReferralStatusType(1);
        public static readonly ReferralStatusType Finished = new ReferralStatusType(2);
        public static readonly ReferralStatusType Paid = new ReferralStatusType(3);

        private ReferralStatusType(int type) : base(type)
        { }

        public static ReferralStatusType Get(int? type)
        {
            if (!type.HasValue)
                return null;

            switch (type)
            {
                case 0:
                    return InProgress;
                case 1:
                    return Interrupted;
                case 2:
                    return Finished;
                case 3:
                    return Paid;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
