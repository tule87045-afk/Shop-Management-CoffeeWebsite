using CafeManagement.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    public class OrderController : Controller
    {
        // Tạo DbContext khi thật sự cần, để thanh điều hướng không chạm database khi chưa đăng nhập
        private OrderDbContext _db;
        private OrderDbContext db
        {
            get { return _db ?? (_db = new OrderDbContext()); }
        }

        private const int MaxQtyPerAdd = 20;
        private const int MaxQtyPerLine = 50;
        private static readonly string[] PaymentMethods = { "Tiền mặt", "Chuyển khoản", "Thẻ / Ví điện tử" };

        private int? CurrentUserId()
        {
            return Session["UserId"] as int?;
        }

        private bool IsAdmin
        {
            get { return (Session["UserRole"] as string) == "Admin"; }
        }

        private Order GetOpenOrder(int userId)
        {
            return db.Orders.Include(o => o.Items)
                     .FirstOrDefault(o => o.UserId == userId && o.Status == "Open");
        }

        // ===== HÓA ĐƠN HIỆN TẠI =====
        public ActionResult Index()
        {
            var uid = CurrentUserId();
            if (uid == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Order") });

            var order = GetOpenOrder(uid.Value);
            ViewBag.PaymentMethods = PaymentMethods;
            return View(order);
        }

        // ===== ĐẶT MÓN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int productId, int? quantity, string returnUrl)
        {
            var uid = CurrentUserId();
            if (uid == null)
            {
                var loginUrl = Url.Action("Login", "Account", new { returnUrl = SafeReturn(returnUrl) });
                if (Request.IsAjaxRequest())
                    return Json(new { ok = false, login = true, loginUrl = loginUrl, message = "Vui lòng đăng nhập để đặt món." });

                TempData["Welcome"] = "Vui lòng đăng nhập để đặt món.";
                return Redirect(loginUrl);
            }

            int qty = quantity.HasValue ? quantity.Value : 1;
            if (qty < 1) qty = 1;
            if (qty > MaxQtyPerAdd) qty = MaxQtyPerAdd;

            string message;
            bool ok = true;

            // Luôn lấy tên và giá từ database, không tin dữ liệu từ trình duyệt
            Product product;
            using (var coffee = new CoffeeDbContext())
            {
                product = coffee.Products.AsNoTracking().FirstOrDefault(p => p.Id == productId);
            }

            if (product == null)
            {
                ok = false;
                message = "Không tìm thấy món này.";
            }
            else
            {
                var order = GetOpenOrder(uid.Value);
                if (order == null)
                {
                    order = new Order
                    {
                        UserId = uid.Value,
                        CustomerName = Session["UserName"] as string,
                        Status = "Open",
                        CreatedAt = DateTime.Now
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();
                }

                var line = db.OrderItems.FirstOrDefault(i => i.OrderId == order.Id && i.ProductId == product.Id);
                if (line == null)
                {
                    db.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = qty
                    });
                }
                else
                {
                    line.Quantity = Math.Min(line.Quantity + qty, MaxQtyPerLine);
                }
                db.SaveChanges();

                message = "Đã thêm " + qty + " × " + product.Name + " vào hóa đơn.";
            }

            if (Request.IsAjaxRequest())
                return Json(new { ok = ok, message = message, count = OpenItemCount(uid.Value) });

            TempData["Welcome"] = message;
            return Redirect(SafeReturn(returnUrl));
        }

        // ===== GIẢM SỐ LƯỢNG 1 MÓN (về 0 thì bỏ món) =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Decrease(int itemId)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Login", "Account");

            var item = FindOpenItem(itemId, uid.Value);
            if (item != null)
            {
                if (item.Quantity > 1) item.Quantity--;
                else db.OrderItems.Remove(item);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // ===== HỦY MÓN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int itemId)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Login", "Account");

            var item = FindOpenItem(itemId, uid.Value);
            if (item != null)
            {
                var name = item.ProductName;
                db.OrderItems.Remove(item);
                db.SaveChanges();
                TempData["Welcome"] = "Đã hủy món " + name + ".";
            }
            return RedirectToAction("Index");
        }

        // ===== HỦY TOÀN BỘ HÓA ĐƠN ĐANG GỌI =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CancelAll()
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Login", "Account");

            var order = GetOpenOrder(uid.Value);
            if (order != null)
            {
                db.Orders.Remove(order); // OrderItems xóa theo (ON DELETE CASCADE)
                db.SaveChanges();
                TempData["Welcome"] = "Đã hủy toàn bộ hóa đơn.";
            }
            return RedirectToAction("Index");
        }

        // ===== THANH TOÁN =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Pay(string paymentMethod)
        {
            var uid = CurrentUserId();
            if (uid == null) return RedirectToAction("Login", "Account");

            var order = GetOpenOrder(uid.Value);
            if (order == null || order.Items == null || order.Items.Count == 0)
            {
                TempData["Welcome"] = "Hóa đơn đang trống, hãy đặt món trước khi thanh toán nhé.";
                return RedirectToAction("Index");
            }

            if (!PaymentMethods.Contains(paymentMethod))
            {
                TempData["Welcome"] = "Vui lòng chọn phương thức thanh toán.";
                return RedirectToAction("Index");
            }

            order.PaidAmount = order.Items.Sum(i => i.UnitPrice * i.Quantity);
            order.PaymentMethod = paymentMethod;
            order.PaidAt = DateTime.Now;
            order.Status = "Paid";
            db.SaveChanges();

            TempData["Welcome"] = "Thanh toán thành công! Cảm ơn bạn đã ghé Mimi Cat Cafe.";
            return RedirectToAction("Receipt", new { id = order.Id });
        }

        // ===== HÓA ĐƠN ĐÃ THANH TOÁN =====
        public ActionResult Receipt(int id)
        {
            var uid = CurrentUserId();
            if (uid == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Receipt", "Order", new { id = id }) });

            var order = db.Orders.Include(o => o.Items).AsNoTracking().FirstOrDefault(o => o.Id == id);
            if (order == null || order.Status != "Paid" || (order.UserId != uid && !IsAdmin))
                return HttpNotFound();

            return View(order);
        }

        // ===== LỊCH SỬ HÓA ĐƠN =====
        public ActionResult History()
        {
            var uid = CurrentUserId();
            if (uid == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("History", "Order") });

            var query = db.Orders.Include(o => o.Items).AsNoTracking().Where(o => o.Status == "Paid");
            if (!IsAdmin)
                query = query.Where(o => o.UserId == uid);

            return View(query.OrderByDescending(o => o.PaidAt).ToList());
        }

        // ===== HÀM PHỤ =====
        private int OpenItemCount(int userId)
        {
            var q = from i in db.OrderItems
                    where i.Order.UserId == userId && i.Order.Status == "Open"
                    select (int?)i.Quantity;
            return q.Sum() ?? 0;
        }

        private OrderItem FindOpenItem(int itemId, int userId)
        {
            return db.OrderItems.Include(i => i.Order)
                     .FirstOrDefault(i => i.Id == itemId
                                          && i.Order.UserId == userId
                                          && i.Order.Status == "Open");
        }

        private string SafeReturn(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return returnUrl;
            return Url.Action("Index", "SanPham");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
