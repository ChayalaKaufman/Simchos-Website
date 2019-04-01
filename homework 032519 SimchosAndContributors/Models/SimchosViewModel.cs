using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimchosAndContributors.Data;

namespace homework_032519_SimchosAndContributors.Models
{
    public class SimchosViewModel
    {
        public IEnumerable<SimchaView> Simchos { get; set; }
        public int TotalContributors { get; set; }
    }
}