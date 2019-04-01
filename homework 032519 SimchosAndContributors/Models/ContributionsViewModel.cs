using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchosAndContributors.Data;

namespace homework_032519_SimchosAndContributors.Models
{
    public class ContributionsViewModel
    {
        public IEnumerable<Contribution> Contributions { get; set; }
        public Simcha Simcha { get; set; }
    }

}