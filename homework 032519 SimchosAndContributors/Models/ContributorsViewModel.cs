using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchosAndContributors.Data;

namespace homework_032519_SimchosAndContributors.Models
{
    public class ContributorsViewModel
    {
        public IEnumerable<Contributor> Contributors { get; set; }
        public decimal Total { get; set; }
    }
}