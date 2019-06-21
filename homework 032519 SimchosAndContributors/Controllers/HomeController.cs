using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using homework_032519_SimchosAndContributors.Models;
using SimchosAndContributors.Data;

namespace homework_032519_SimchosAndContributors.Controllers
{
    public class HomeController : Controller
    {
        DBManager db = new DBManager(Properties.Settings.Default.ConStr);

        public ActionResult Index()
        {
            SimchosViewModel vm = new SimchosViewModel();
            vm.Simchos = db.GetSimchos();
            vm.TotalContributors = db.GetTotalContributors();
            return View(vm);
        }

        public ActionResult Contributors(string searchText)
        {
            ContributorsViewModel vm = new ContributorsViewModel();
            
            vm.Contributors = db.GetContributors(searchText);
            vm.Total = db.GetTotalOnHand();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddContributor(Contributor c, decimal initialDeposit)
        {
            db.AddContributor(c, initialDeposit);
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public ActionResult EditContributor(Contributor c)
        {
            db.EditContributor(c);
            return Redirect("/home/contributors");
        }

        [HttpPost]
        public ActionResult AddSimcha(Simcha s)
        {
            db.AddSimcha(s);
            return Redirect("/");
        }

        public ActionResult Contributions(int id,string name)
        {
            ContributionsViewModel vm = new ContributionsViewModel();
            
            vm.Contributions = db.GetContributions(id);
            vm.Simcha = new Simcha
            {
                Name = name,
                Id = id
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddDeposit(Deposit d)
        {
            db.AddDeposit(d);
            return Redirect("/home/contributors");
        }

        public ActionResult History(int id,string name)
        {
            
            HistoryView history = db.GetHistory(id);
            history.ContributorName = name;

            return View(history);
        }

        [HttpPost]
        public ActionResult UpdateContributions(List<ContributionInclusion> conts, int SimchaId)
        {
            db.UpdateContributions(conts, SimchaId);
            return Redirect("/");
        }

        //[HttpPost]
        //public ActionResult UpdateContributions(List<ContributionInclusion> contributors, int simchaId)
        //{
        //    var mgr = new DBManager(Properties.Settings.Default.ConStr);
        //    mgr.UpdateContributions(simchaId, contributors);
        //    TempData["Message"] = "Simcha updated successfully";
        //    return RedirectToAction("Index");
        //}
    }
}
