﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchosAndContributors.Data
{
    public class ContributionInclusion
    {
        public bool Include { get; set; }
        public decimal Amount { get; set; }
        public int ContributorId { get; set; }
    }
}
