using System;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util.DapperAttributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DapperCriticalRestrictionAttribute : Attribute
    {
        public enum Operation { PreviousValueIsEqual, PreviousValueIsNotEqual, PreviousValueIsGreater, PreviousValueIsLesser, PreviousValueIsGreaterOrEqual, PreviousValueIsLesserOrEqual };
        public string SqlOperation { get; private set; }

        public DapperCriticalRestrictionAttribute(Operation criticalRestriction)
        {
            switch(criticalRestriction)
            {
                case Operation.PreviousValueIsEqual:
                    SqlOperation = "=";
                    break;
                case Operation.PreviousValueIsNotEqual:
                    SqlOperation = "<>";
                    break;
                case Operation.PreviousValueIsGreater:
                    SqlOperation = ">";
                    break;
                case Operation.PreviousValueIsLesser:
                    SqlOperation = "<";
                    break;
                case Operation.PreviousValueIsGreaterOrEqual:
                    SqlOperation = ">=";
                    break;
                case Operation.PreviousValueIsLesserOrEqual:
                    SqlOperation = "<=";
                    break;
                default:
                    throw new ArgumentException("Invalid 'criticalRestriction'");
            }
        }
    }
}
