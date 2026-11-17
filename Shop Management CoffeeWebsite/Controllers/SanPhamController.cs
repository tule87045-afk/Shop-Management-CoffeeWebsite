using CafeManagement.Models;
using System.Linq;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    public class SanPhamController : Controller
    {
        private CoffeeDbContext db = new CoffeeDbContext();

        public ActionResult Index()
        {
            var products = db.Products
                             .AsNoTracking()
                             .ToList();

            return View(products);
        }

        public ActionResult Details(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}