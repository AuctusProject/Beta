using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class ReferralStatusType : IntType
    {
        public static readonly ReferralStatusType NotStarted = new ReferralStatusType(0);
        public static readonly ReferralStatusType InProgress = new ReferralStatusType(1);
        public static readonly ReferralStatusType Interrupted = new ReferralStatusType(2);
        public static readonly ReferralStatusType Finished = new ReferralStatusType(3);
        public static readonly ReferralStatusType Paid = new ReferralStatusType(4);

        private ReferralStatusType(int type) : base(type)
        { }

        public static ReferralStatusType Get(int? type)
        {
            if (!type.HasValue)
                return NotStarted;

            switch (type)
            {
                case 0:
                    return NotStarted;
                case 1:
                    return InProgress;
                case 2:
                    return Interrupted;
                case 3:
                    return Finished;
                case 4:
                    return Paid;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
