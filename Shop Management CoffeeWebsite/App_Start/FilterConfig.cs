using System.Web;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
