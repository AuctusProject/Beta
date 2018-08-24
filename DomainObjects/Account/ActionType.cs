using Auctus.Util;
using Auctus.Util.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.DomainObjects.Account
{
    public class ActionType : IntType
    {
        public static readonly ActionType NewWallet = new ActionType(1);
        public static readonly ActionType NewLogin = new ActionType(2);
        public static readonly ActionType NewAucVerification = new ActionType(3);
        public static readonly ActionType JobVerification = new ActionType(4);

        private ActionType(int type) : base(type)
        { }

        public static ActionType Get(int type)
        {
            switch (type)
            {
                case 1:
                    return NewWallet;
                case 2:
                    return NewLogin;
                case 3:
                    return NewAucVerification;
                case 4:
                    return JobVerification;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
