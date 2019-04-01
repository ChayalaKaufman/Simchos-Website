using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchosAndContributors.Data;

namespace homework_032519_SimchosAndContributors.Models
{
    public class HistoryViewModel
    {
        public IEnumerable<History> History { get; set; }
        public decimal Balance { get; set; }
        public string ContributorName { get; set; }
    }
}