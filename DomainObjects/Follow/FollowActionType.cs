using Auctus.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Follow
{
    public class FollowActionType : IntType
    {
        public static readonly FollowActionType Unfollow = new FollowActionType(0);
        public static readonly FollowActionType Follow = new FollowActionType(1);

        private FollowActionType(int type) : base(type)
        { }

        public static FollowActionType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Unfollow;
                case 1:
                    return Follow;
                default:
                    throw new ArgumentException("Invalid type.");
            }
        }
    }
}
