using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class SocialNetworkType : IntType
    {
        public static readonly SocialNetworkType Facebook = new SocialNetworkType(0);
        public static readonly SocialNetworkType Google = new SocialNetworkType(1);

        private SocialNetworkType(int type) : base(type)
        { }

        public static SocialNetworkType Get(int? type)
        {
            if (!type.HasValue)
                return null;

            switch (type)
            {
                case 0:
                    return Facebook;
                case 1:
                    return Google;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }

        public string GetDescription()
        {
            switch (Value)
            {
                case 0:
                    return "Facebook";
                case 1:
                    return "Google";
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
