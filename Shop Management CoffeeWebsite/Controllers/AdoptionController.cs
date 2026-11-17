using CafeManagement.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    public class AdoptionController : Controller
    {
        private AdoptionDbContext db = new AdoptionDbContext();

        private const int MaxPhotoBytes = 3 * 1024 * 1024;
        private static readonly string[] AllowedExt = { ".jpg", ".jpeg", ".png", ".webp" };

        private bool IsAdmin
        {
            get { return (Session["UserRole"] as string) == "Admin"; }
        }

        // ===== TRANG NHẬN NUÔI: danh sách mèo =====
        public ActionResult Index()
        {
            var cats = db.Cats.AsNoTracking()
                         .Where(c => c.Status == "Available")
                         .OrderByDescending(c => c.CreatedAt)
                         .ToList();
            return View(cats);
        }

        // ===== ĐĂNG KÝ NHẬN NUÔI MỘT BÉ =====
        [HttpGet]
        public ActionResult Adopt(int id)
        {
            var cat = db.Cats.Find(id);
            if (cat == null || cat.Status != "Available")
            {
                TempData["Welcome"] = "Bé mèo này hiện không còn chờ nhận nuôi.";
                return RedirectToAction("Index");
            }

            ViewBag.Cat = cat;
            var model = new AdoptFormViewModel { CatId = id };
            PrefillAdopt(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Adopt(AdoptFormViewModel model)
        {
            var cat = db.Cats.Find(model.CatId);
            if (cat == null || cat.Status != "Available")
            {
                TempData["Welcome"] = "Bé mèo này hiện không còn chờ nhận nuôi.";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Cat = cat;
                return View(model);
            }

            db.AdoptionRequests.Add(new AdoptionRequest
            {
                CatId = cat.Id,
                FullName = model.FullName.Trim(),
                Phone = model.Phone.Trim(),
                Email = Clean(model.Email),
                Address = model.Address.Trim(),
                Note = Clean(model.Note),
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UserId = CurrentUserId()
            });
            db.SaveChanges();

            TempData["Welcome"] = "Mimi đã nhận đơn xin nhận nuôi bé " + cat.Name + "! Shop sẽ liên hệ bạn sớm nhé.";
            return RedirectToAction("Index");
        }

        // ===== GỬI MÈO CHO SHOP =====
        [HttpGet]
        public ActionResult Submit()
        {
            var model = new SubmitCatViewModel();
            PrefillSubmit(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Submit(SubmitCatViewModel model, HttpPostedFileBase photo)
        {
            string photoError;
            string photoPath = SavePhoto(photo, out photoError);
            if (photoError != null)
                ModelState.AddModelError("photo", photoError);

            if (!ModelState.IsValid)
                return View(model);

            db.CatSubmissions.Add(new CatSubmission
            {
                OwnerName = model.OwnerName.Trim(),
                Phone = model.Phone.Trim(),
                Email = Clean(model.Email),
                CatName = model.CatName.Trim(),
                AgeText = model.AgeText.Trim(),
                Gender = model.Gender,
                Breed = Clean(model.Breed),
                IsVaccinated = model.IsVaccinated,
                Description = model.Description.Trim(),
                Reason = Clean(model.Reason),
                ImageUrl = photoPath,
                Status = "Pending",
                CreatedAt = DateTime.Now,
                UserId = CurrentUserId()
            });
            db.SaveChanges();

            TempData["Welcome"] = "Cảm ơn bạn! Mimi đã nhận thông tin về bé " + model.CatName.Trim() + " và sẽ liên hệ để sắp xếp sớm nhất.";
            return RedirectToAction("Index");
        }

        // ===== QUẢN LÝ (chỉ Admin) =====
        public ActionResult Manage()
        {
            if (!IsAdmin)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Manage", "Adoption") });

            ViewBag.Submissions = db.CatSubmissions.AsNoTracking()
                                    .OrderBy(s => s.Status == "Pending" ? 0 : 1)
                                    .ThenByDescending(s => s.CreatedAt).ToList();

            var requests = db.AdoptionRequests.AsNoTracking()
                             .OrderBy(r => r.Status == "Pending" ? 0 : 1)
                             .ThenByDescending(r => r.CreatedAt).ToList();
            ViewBag.Requests = requests;

            ViewBag.CatNames = db.Cats.AsNoTracking()
                                 .ToDictionary(c => c.Id, c => c.Name);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApproveSubmission(int id)
        {
            if (!IsAdmin) return new HttpUnauthorizedResult();

            var s = db.CatSubmissions.Find(id);
            if (s != null && s.Status == "Pending")
            {
                db.Cats.Add(new Cat
                {
                    Name = s.CatName,
                    AgeText = s.AgeText,
                    Gender = s.Gender,
                    Breed = s.Breed,
                    Description = s.Description,
                    ImageUrl = s.ImageUrl,
                    IsVaccinated = s.IsVaccinated,
                    Status = "Available",
                    CreatedAt = DateTime.Now
                });
                s.Status = "Approved";
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectSubmission(int id)
        {
            if (!IsAdmin) return new HttpUnauthorizedResult();

            var s = db.CatSubmissions.Find(id);
            if (s != null && s.Status == "Pending")
            {
                s.Status = "Rejected";
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmAdoption(int id)
        {
            if (!IsAdmin) return new HttpUnauthorizedResult();

            var r = db.AdoptionRequests.Find(id);
            if (r != null && r.Status == "Pending")
            {
                r.Status = "Confirmed";

                var cat = db.Cats.Find(r.CatId);
                if (cat != null) cat.Status = "Adopted";

                // Các đơn khác cho cùng bé mèo coi như đã có người nhận
                var others = db.AdoptionRequests
                               .Where(x => x.CatId == r.CatId && x.Id != r.Id && x.Status == "Pending")
                               .ToList();
                foreach (var o in others) o.Status = "Rejected";

                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RejectAdoption(int id)
        {
            if (!IsAdmin) return new HttpUnauthorizedResult();

            var r = db.AdoptionRequests.Find(id);
            if (r != null && r.Status == "Pending")
            {
                r.Status = "Rejected";
                db.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        // ===== HÀM PHỤ =====
        private static string Clean(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private int? CurrentUserId()
        {
            return Session["UserId"] as int?;
        }

        private User CurrentUser()
        {
            var id = CurrentUserId();
            if (id == null) return null;
            using (var acc = new AccountDbContext())
            {
                return acc.Users.AsNoTracking().FirstOrDefault(u => u.Id == id.Value);
            }
        }

        private void PrefillAdopt(AdoptFormViewModel m)
        {
            var u = CurrentUser();
            if (u == null) return;
            m.FullName = u.FullName;
            m.Phone = u.Phone;
            m.Email = u.Email;
        }

        private void PrefillSubmit(SubmitCatViewModel m)
        {
            var u = CurrentUser();
            if (u == null) return;
            m.OwnerName = u.FullName;
            m.Phone = u.Phone;
            m.Email = u.Email;
        }

        private string SavePhoto(HttpPostedFileBase file, out string error)
        {
            error = null;
            if (file == null || file.ContentLength == 0) return null;

            if (file.ContentLength > MaxPhotoBytes)
            {
                error = "Ảnh quá lớn, vui lòng chọn ảnh dưới 3MB!";
                return null;
            }

            var ext = (Path.GetExtension(file.FileName) ?? "").ToLowerInvariant();
            if (!AllowedExt.Contains(ext) || file.ContentType == null || !file.ContentType.StartsWith("image/"))
            {
                error = "Chỉ nhận ảnh định dạng JPG, PNG hoặc WEBP!";
                return null;
            }

            var dir = Server.MapPath("~/image/cats");
            Directory.CreateDirectory(dir);

            var name = Guid.NewGuid().ToString("N") + ext;
            file.SaveAs(Path.Combine(dir, name));
            return "~/image/cats/" + name;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
