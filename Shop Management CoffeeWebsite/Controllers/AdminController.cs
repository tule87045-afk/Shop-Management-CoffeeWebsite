using CafeManagement.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    public class AdminController : Controller
    {
        // DANH SÁCH DÙNG CHUNG TOÀN BỘ WEBSITE (KHO 26 MÓN)
        public static List<Product> products = new List<Product>
        {
            // ===== 6 MÓN BAN ĐẦU =====
            new Product { Id = 1, Name = "Matcha Latte", Category = "Latte", Price = 45000, ImageUrl = "~/image/matcha.jpg", Description = "Matcha kết hợp sữa tươi", Size = "M, L", Ingredients = "Matcha, sữa tươi, đá", PreparationTime = "5-7 phút", Note = "Có thể điều chỉnh lượng đường." },
            new Product { Id = 2, Name = "Trà đào cam sả", Category = "Trà", Price = 39000, ImageUrl = "~/image/tra-dao.jpg", Description = "Trà đào thơm mát", Size = "M, L", Ingredients = "Trà, đào, sả", PreparationTime = "5 phút", Note = "Ngon hơn khi uống lạnh." },
            new Product { Id = 3, Name = "Cà phê sữa đá", Category = "Cà phê", Price = 29000, ImageUrl = "~/image/ca-phe-sua.jpg", Description = "Cà phê sữa truyền thống", Size = "M, L", Ingredients = "Cà phê, sữa đặc, đá", PreparationTime = "3-5 phút", Note = "Có thể điều chỉnh lượng sữa." },
            new Product { Id = 4, Name = "Caramel Latte", Category = "Latte", Price = 45000, ImageUrl = "~/image/caramel-latte.jpg", Description = "Latte vị caramel", Size = "M, L", Ingredients = "Cà phê, sữa tươi, caramel", PreparationTime = "3-5 phút", Note = "Vị ngọt béo thơm ngon." },
            new Product { Id = 5, Name = "Trà vải", Category = "Trà", Price = 35000, ImageUrl = "~/image/tra-vai.jpg", Description = "Trà vải thanh mát", Size = "M, L", Ingredients = "Trà, vải, thạch", PreparationTime = "5-7 phút", Note = "Có thể điều chỉnh lượng đường." },
            new Product { Id = 6, Name = "Chocolate đá xay", Category = "Đá xay", Price = 49000, ImageUrl = "~/image/chocolate.jpg", Description = "Chocolate đá xay thơm béo", Size = "M, L", Ingredients = "Chocolate, sữa tươi, đá", PreparationTime = "5-7 phút", Note = "Có thể chọn mức đường." },

            // ===== 5 MÓN CÀ PHÊ =====
            new Product { Id = 7, Name = "Cà phê đen đá", Category = "Cà phê", Price = 25000, ImageUrl = "~/image/ca-phe-sua.jpg", Description = "Cà phê đậm vị truyền thống", Size = "M, L", Ingredients = "Cà phê, đá", PreparationTime = "3-5 phút", Note = "Phù hợp người thích vị đắng." },
            new Product { Id = 8, Name = "Bạc xỉu", Category = "Cà phê", Price = 32000, ImageUrl = "~/image/ca-phe-sua.jpg", Description = "Sữa thơm béo hòa quyện cà phê", Size = "M, L", Ingredients = "Cà phê, sữa đặc, sữa tươi", PreparationTime = "3-5 phút", Note = "Vị sữa nổi bật." },
            new Product { Id = 9, Name = "Americano", Category = "Cà phê", Price = 35000, ImageUrl = "~/image/ca-phe-sua.jpg", Description = "Cà phê espresso pha nước", Size = "M, L", Ingredients = "Espresso, nước", PreparationTime = "3-5 phút", Note = "Có thể chọn nóng hoặc lạnh." },
            new Product { Id = 10, Name = "Espresso", Category = "Cà phê", Price = 30000, ImageUrl = "~/image/Espresso.jpg", Description = "Cà phê espresso đậm đặc", Size = "S", Ingredients = "Cà phê espresso", PreparationTime = "2-3 phút", Note = "Dành cho người thích cà phê đậm." },
            new Product { Id = 11, Name = "Cappuccino", Category = "Cà phê", Price = 42000, ImageUrl = "~/image/caramel-latte.jpg", Description = "Cà phê kết hợp sữa và bọt sữa", Size = "M, L", Ingredients = "Espresso, sữa tươi, bọt sữa", PreparationTime = "4-6 phút", Note = "Thưởng thức nóng sẽ ngon." },

            // ===== 5 MÓN TRÀ =====
            new Product { Id = 12, Name = "Trà chanh mật ong", Category = "Trà", Price = 30000, ImageUrl = "~/image/tra-dao.jpg", Description = "Trà chanh chua ngọt dịu nhẹ", Size = "M, L", Ingredients = "Trà, chanh, mật ong", PreparationTime = "4-5 phút", Note = "Có thể giảm lượng đường." },
            new Product { Id = 13, Name = "Trà tắc", Category = "Trà", Price = 25000, ImageUrl = "~/image/tra-vai.jpg", Description = "Trà tắc thanh mát", Size = "M, L", Ingredients = "Trà, tắc, đường", PreparationTime = "3-5 phút", Note = "Thích hợp uống lạnh." },
            new Product { Id = 14, Name = "Trà dâu", Category = "Trà", Price = 39000, ImageUrl = "~/image/tra-vai.jpg", Description = "Trà trái cây vị dâu", Size = "M, L", Ingredients = "Trà, dâu, siro dâu", PreparationTime = "5-7 phút", Note = "Vị trái cây ngọt dịu." },
            new Product { Id = 15, Name = "Trà vải hoa hồng", Category = "Trà", Price = 42000, ImageUrl = "~/image/tra-vai.jpg", Description = "Trà vải thơm hương hoa hồng", Size = "M, L", Ingredients = "Trà, vải, siro hoa hồng", PreparationTime = "5-7 phút", Note = "Hương thơm nhẹ nhàng." },
            new Product { Id = 16, Name = "Trà ô long đào", Category = "Trà", Price = 40000, ImageUrl = "~/image/tra-dao.jpg", Description = "Ô long kết hợp đào ngọt thơm", Size = "M, L", Ingredients = "Trà ô long, đào", PreparationTime = "5-7 phút", Note = "Vị trà thơm dịu." },

            // ===== 4 MÓN LATTE =====
            new Product { Id = 17, Name = "Vanilla Latte", Category = "Latte", Price = 45000, ImageUrl = "~/image/caramel-latte.jpg", Description = "Latte thơm hương vanilla", Size = "M, L", Ingredients = "Espresso, sữa tươi, vanilla", PreparationTime = "4-6 phút", Note = "Có thể chọn nóng hoặc lạnh." },
            new Product { Id = 18, Name = "Hazelnut Latte", Category = "Latte", Price = 47000, ImageUrl = "~/image/caramel-latte.jpg", Description = "Latte hương hạt phỉ", Size = "M, L", Ingredients = "Espresso, sữa, siro hạt phỉ", PreparationTime = "4-6 phút", Note = "Hương hạt phỉ thơm béo." },
            new Product { Id = 19, Name = "Oat Milk Latte", Category = "Latte", Price = 49000, ImageUrl = "~/image/caramel-latte.jpg", Description = "Latte kết hợp sữa yến mạch", Size = "M, L", Ingredients = "Espresso, sữa yến mạch", PreparationTime = "4-6 phút", Note = "Hương vị dịu nhẹ." },
            new Product { Id = 20, Name = "Mocha Latte", Category = "Latte", Price = 48000, ImageUrl = "~/image/chocolate.jpg", Description = "Cà phê sữa kết hợp chocolate", Size = "M, L", Ingredients = "Espresso, chocolate, sữa", PreparationTime = "4-6 phút", Note = "Vị cà phê và chocolate hòa quyện." },

            // ===== 6 MÓN SODA =====
            new Product { Id = 21, Name = "Soda Blue Ocean", Category = "Soda", Price = 35000, ImageUrl = "~/image/tra-dao.jpg", Description = "Soda xanh mát sảng khoái", Size = "M, L", Ingredients = "Soda, siro blue, chanh", PreparationTime = "3-5 phút", Note = "Thức uống có gas, dùng lạnh." },
            new Product { Id = 22, Name = "Soda Dâu", Category = "Soda", Price = 35000, ImageUrl = "~/image/tra-vai.jpg", Description = "Soda dâu ngọt dịu", Size = "M, L", Ingredients = "Soda, siro dâu, đá", PreparationTime = "3-5 phút", Note = "Vị dâu thơm ngọt." },
            new Product { Id = 23, Name = "Soda Chanh", Category = "Soda", Price = 30000, ImageUrl = "~/image/tra-dao.jpg", Description = "Soda chanh chua ngọt", Size = "M, L", Ingredients = "Soda, chanh, đường", PreparationTime = "3-5 phút", Note = "Vị chanh tươi mát." },
            new Product { Id = 24, Name = "Soda Việt Quất", Category = "Soda", Price = 38000, ImageUrl = "~/image/soda.jpg", Description = "Soda việt quất thơm trái cây", Size = "M, L", Ingredients = "Soda, siro việt quất", PreparationTime = "3-5 phút", Note = "Vị ngọt chua nhẹ." },
            new Product { Id = 25, Name = "Soda Đào", Category = "Soda", Price = 36000, ImageUrl = "~/image/tra-dao.jpg", Description = "Soda đào thơm ngọt", Size = "M, L", Ingredients = "Soda, siro đào, đào", PreparationTime = "3-5 phút", Note = "Dùng lạnh để ngon hơn." },
            new Product { Id = 26, Name = "Soda Mojito Chanh", Category = "Soda", Price = 39000, ImageUrl = "~/image/tra-dao.jpg", Description = "Soda chanh bạc hà sảng khoái", Size = "M, L", Ingredients = "Soda, chanh, bạc hà", PreparationTime = "3-5 phút", Note = "Vị chanh bạc hà thanh mát." }
        };
        private CoffeeDbContext db = new CoffeeDbContext();

        public AdminController()
        {
            // Chỉ chuyển 26 món mẫu vào SQL khi bảng đang trống
            if (!db.Products.Any())
            {
                db.Products.AddRange(products);
                db.SaveChanges();
            }
        }

        // 1. Xem danh sách sản phẩm
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }

        // 2. Thêm sản phẩm - GET
        public ActionResult Create()
        {
            return View();
        }

        // 2. Thêm sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // 3. Sửa sản phẩm - GET
        public ActionResult Edit(int id)
        {
            var product = db.Products.Find(id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }

        // 3. Sửa sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State =
                    System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

        // 4. Xóa sản phẩm
        public ActionResult Delete(int id)
        {
            var product = db.Products.Find(id);

            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
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