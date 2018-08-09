using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Model.Advisor
{
    public class BeAdvisorRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PreviousExperience { get; set; }
    }
}
