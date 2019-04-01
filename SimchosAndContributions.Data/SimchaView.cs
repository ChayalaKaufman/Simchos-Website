using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchosAndContributors.Data
{
    public class SimchaView
    {
        public int TotalContributed { get; set; }
        public decimal Total { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Id { get; set; }
    }
}
