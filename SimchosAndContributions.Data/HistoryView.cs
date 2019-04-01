using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchosAndContributors.Data
{
    public class HistoryView
    {
        public List<History> History { get; set; }
        public decimal Balance { get; set; }
        public string ContributorName { get; set; }
    }
}
