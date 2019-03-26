using System.Web;
using System.Web.Mvc;

namespace homework_032519_SimchosAndContributors
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
