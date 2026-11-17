using CafeManagement.Models;
using Shop_Management_CoffeeWebsite.Helpers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    public class AccountController : Controller
    {
        private AccountDbContext db = new AccountDbContext();

        // ===== ĐĂNG NHẬP =====
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "SanPham");

            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email.Trim().ToLower();
            var user = db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null || !PasswordHelper.Verify(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng!");
                return View(model);
            }

            SignIn(user);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "SanPham");
        }

        // ===== ĐĂNG KÝ =====
        [HttpGet]
        public ActionResult Register()
        {
            if (Session["UserId"] != null)
                return RedirectToAction("Index", "SanPham");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email.Trim().ToLower();

            if (db.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký!");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = email,
                Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim(),
                PasswordHash = PasswordHelper.Hash(model.Password),
                Role = "Customer",
                CreatedAt = DateTime.Now
            };

            db.Users.Add(user);
            db.SaveChanges();

            // Đăng ký xong tự đăng nhập luôn
            SignIn(user);
            TempData["Welcome"] = "Chào mừng " + user.FullName + " đến với Mimi Cat Cafe!";
            return RedirectToAction("Index", "SanPham");
        }

        // ===== ĐĂNG XUẤT =====
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "SanPham");
        }

        private void SignIn(User user)
        {
            Session["UserId"] = user.Id;
            Session["UserName"] = user.FullName;
            Session["UserRole"] = user.Role;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
