using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchosAndContributors.Data
{
    public class Contribution
    {
        public string Name { get; set; }
        public int ContributorId { get; set; }
        public decimal Amount { get; set; }
        public bool Contributed { get; set; }
        public decimal Balance { get; set; }
        public bool AlwaysInclude { get; set; }
        public Simcha Simcha { get; set; }
    }
}
