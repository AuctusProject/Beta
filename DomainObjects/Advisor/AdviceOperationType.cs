using Auctus.Util;
using Auctus.Util.Exceptions;

namespace Auctus.DomainObjects.Advisor
{
    public class AdviceOperationType : IntType
    {
        public static readonly AdviceOperationType Manual = new AdviceOperationType(0);
        public static readonly AdviceOperationType StopLoss = new AdviceOperationType(1);
        public static readonly AdviceOperationType TargetPrice = new AdviceOperationType(2);

        private AdviceOperationType(int type) : base(type)
        { }

        public static AdviceOperationType Get(int type)
        {
            switch (type)
            {
                case 0:
                    return Manual;
                case 1:
                    return StopLoss;
                case 2:
                    return TargetPrice;
                default:
                    throw new BusinessException("Invalid type.");
            }
        }
    }
}
