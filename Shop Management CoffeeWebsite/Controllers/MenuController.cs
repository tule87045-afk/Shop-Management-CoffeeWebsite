
using System.Collections.Generic;
using System.Web.Mvc;
using CafeManagement.Models;

namespace CafeManagement.Controllers
{
    public class MenuController : Controller
    {
        public ActionResult Index()
        {
            var products = new List<Product>
            {
                new Product
                {
                    Id = 1,
                    Name = "Matcha Latte",
                    Category = "Latte",
                    Price = 45000,
                    ImageUrl = "~/image/matcha.jpg",
                    Description = "Matcha kết hợp sữa tươi"
                },
                new Product
                {
                    Id = 2,
                    Name = "Trà đào cam sả",
                    Category = "Trà",
                    Price = 39000,
                    ImageUrl = "~/image/tra-dao.jpg",
                    Description = "Trà đào thơm mát"
                },
                new Product
                {
                    Id = 3,
                    Name = "Cà phê sữa đá",
                    Category = "Cà phê",
                    Price = 29000,
                    ImageUrl = "~/image/ca-phe-sua.jpg",
                    Description = "Cà phê sữa truyền thống"
                },
                new Product
                {
                    Id = 4,
                    Name = "Caramel Latte",
                    Category = "Latte",
                    Price = 45000,
                    ImageUrl = "~/image/caramel-latte.jpg",
                    Description = "Latte vị caramel"
                },
                new Product
                {
                    Id = 5,
                    Name = "Trà vải",
                    Category = "Trà",
                    Price = 35000,
                    ImageUrl = "~/image/tra-vai.jpg",
                    Description = "Trà vải thanh mát"
                },
                new Product
                {
                    Id = 6,
                    Name = "Chocolate đá xay",
                    Category = "Đá xay",
                    Price = 49000,
                    ImageUrl = "~/image/chocolate.jpg",
                    Description = "Chocolate xay cùng đá và sữa"
                }
            };

            return View(products);
        }
    }
}
