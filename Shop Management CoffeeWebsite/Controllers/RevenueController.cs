using CafeManagement.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Shop_Management_CoffeeWebsite.Controllers
{
    // Thống kê doanh thu theo ngày / tuần / tháng / năm. Chỉ dành cho Admin.
    public class RevenueController : Controller
    {
        private OrderDbContext _db;
        private OrderDbContext db
        {
            get { return _db ?? (_db = new OrderDbContext()); }
        }

        private const int MaxOrderRows = 200;
        private static readonly string[] WeekDayNames = { "T2", "T3", "T4", "T5", "T6", "T7", "CN" };

        private bool IsAdmin
        {
            get { return (Session["UserRole"] as string) == "Admin"; }
        }

        // GET: /Revenue?period=day|week|month|year&date=yyyy-MM-dd
        public ActionResult Index(string period, string date)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Index", "Revenue") });

            if (!IsAdmin)
            {
                TempData["Welcome"] = "Chỉ quản trị viên mới xem được thống kê doanh thu.";
                return RedirectToAction("Index", "SanPham");
            }

            period = NormalizePeriod(period);

            DateTime selected;
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out selected))
                selected = DateTime.Today;
            selected = selected.Date;

            var model = new RevenueViewModel { Period = period, Date = selected };
            ComputeRange(model);

            // Đơn đã thanh toán trong kỳ đang xem
            var start = model.Start;
            var end = model.End;
            var orders = db.Orders.Include(o => o.Items).AsNoTracking()
                           .Where(o => o.Status == "Paid" && o.PaidAt >= start && o.PaidAt < end)
                           .ToList();

            BuildSummary(model, orders);
            BuildChart(model, orders);
            BuildBreakdowns(model, orders);
            BuildQuickOverview(model);
            BuildNavigation(model);

            var ordered = orders.OrderByDescending(o => o.PaidAt).ToList();
            model.OrdersTruncated = ordered.Count > MaxOrderRows;
            model.Orders = ordered.Take(MaxOrderRows).ToList();

            return View(model);
        }

        // ===== Khoảng thời gian =====
        private static string NormalizePeriod(string period)
        {
            switch ((period ?? "").ToLowerInvariant())
            {
                case "week": return "week";
                case "month": return "month";
                case "year": return "year";
                default: return "day";
            }
        }

        private static DateTime StartOfWeek(DateTime d)
        {
            int offset = ((int)d.DayOfWeek + 6) % 7; // tuần bắt đầu từ thứ Hai
            return d.Date.AddDays(-offset);
        }

        private static void ComputeRange(RevenueViewModel m)
        {
            var d = m.Date;
            switch (m.Period)
            {
                case "week":
                    m.Start = StartOfWeek(d);
                    m.End = m.Start.AddDays(7);
                    m.RangeTitle = "Tuần " + m.Start.ToString("dd/MM/yyyy") + " – " + m.End.AddDays(-1).ToString("dd/MM/yyyy");
                    m.ChartTitle = "Doanh thu từng ngày trong tuần";
                    break;
                case "month":
                    m.Start = new DateTime(d.Year, d.Month, 1);
                    m.End = m.Start.AddMonths(1);
                    m.RangeTitle = "Tháng " + d.Month + "/" + d.Year;
                    m.ChartTitle = "Doanh thu từng ngày trong tháng";
                    break;
                case "year":
                    m.Start = new DateTime(d.Year, 1, 1);
                    m.End = m.Start.AddYears(1);
                    m.RangeTitle = "Năm " + d.Year;
                    m.ChartTitle = "Doanh thu từng tháng trong năm";
                    break;
                default:
                    m.Start = d.Date;
                    m.End = m.Start.AddDays(1);
                    m.RangeTitle = "Ngày " + d.ToString("dd/MM/yyyy");
                    m.ChartTitle = "Doanh thu từng giờ trong ngày";
                    break;
            }
        }

        // ===== Số liệu tổng =====
        private static decimal AmountOf(Order o)
        {
            return o.PaidAmount ?? 0m;
        }

        private static void BuildSummary(RevenueViewModel m, List<Order> orders)
        {
            m.OrderCount = orders.Count;
            m.TotalRevenue = orders.Sum(o => AmountOf(o));
            m.ItemCount = orders.Sum(o => o.Items == null ? 0 : o.Items.Sum(i => i.Quantity));
            m.AverageOrder = m.OrderCount == 0 ? 0m : Math.Round(m.TotalRevenue / m.OrderCount, 0);
        }

        // ===== Biểu đồ =====
        private static void BuildChart(RevenueViewModel m, List<Order> orders)
        {
            var points = new List<RevenuePoint>();

            if (m.Period == "day")
            {
                for (int h = 0; h < 24; h++)
                {
                    int hour = h;
                    var inHour = orders.Where(o => o.PaidAt.Value.Hour == hour).ToList();
                    points.Add(MakePoint(hour + "h", "", hour + "h – " + (hour + 1) + "h", inHour));
                }
            }
            else if (m.Period == "week")
            {
                for (int i = 0; i < 7; i++)
                {
                    var day = m.Start.AddDays(i);
                    var inDay = orders.Where(o => o.PaidAt.Value.Date == day).ToList();
                    points.Add(MakePoint(WeekDayNames[i], day.ToString("dd/MM"), WeekDayNames[i] + " " + day.ToString("dd/MM/yyyy"), inDay));
                }
            }
            else if (m.Period == "month")
            {
                int days = DateTime.DaysInMonth(m.Start.Year, m.Start.Month);
                for (int i = 0; i < days; i++)
                {
                    var day = m.Start.AddDays(i);
                    var inDay = orders.Where(o => o.PaidAt.Value.Date == day).ToList();
                    points.Add(MakePoint(day.Day.ToString(), "", "Ngày " + day.ToString("dd/MM/yyyy"), inDay));
                }
            }
            else
            {
                for (int mo = 1; mo <= 12; mo++)
                {
                    int month = mo;
                    var inMonth = orders.Where(o => o.PaidAt.Value.Month == month).ToList();
                    points.Add(MakePoint("T" + month, "", "Tháng " + month + "/" + m.Start.Year, inMonth));
                }
            }

            m.Points = points;
            m.MaxAmount = points.Count == 0 ? 0m : points.Max(p => p.Amount);

            var best = points.Where(p => p.Amount > 0).OrderByDescending(p => p.Amount).FirstOrDefault();
            m.BestLabel = best == null ? "" : best.Tooltip;
        }

        private static RevenuePoint MakePoint(string label, string subLabel, string tooltip, List<Order> list)
        {
            var amount = list.Sum(o => AmountOf(o));
            return new RevenuePoint
            {
                Label = label,
                SubLabel = subLabel,
                Tooltip = tooltip + ": " + amount.ToString("N0") + " đ (" + list.Count + " hóa đơn)",
                Amount = amount,
                Orders = list.Count
            };
        }

        // ===== Món bán chạy + phương thức thanh toán =====
        private static void BuildBreakdowns(RevenueViewModel m, List<Order> orders)
        {
            m.TopProducts = orders.Where(o => o.Items != null)
                                  .SelectMany(o => o.Items)
                                  .GroupBy(i => i.ProductName)
                                  .Select(g => new RevenueTopProduct
                                  {
                                      Name = g.Key,
                                      Quantity = g.Sum(i => i.Quantity),
                                      Amount = g.Sum(i => i.UnitPrice * i.Quantity)
                                  })
                                  .OrderByDescending(p => p.Amount)
                                  .Take(5)
                                  .ToList();

            m.PayMethods = orders.GroupBy(o => string.IsNullOrEmpty(o.PaymentMethod) ? "Khác" : o.PaymentMethod)
                                 .Select(g => new RevenuePayMethod
                                 {
                                     Method = g.Key,
                                     Orders = g.Count(),
                                     Amount = g.Sum(o => AmountOf(o))
                                 })
                                 .OrderByDescending(p => p.Amount)
                                 .ToList();
        }

        // ===== Tổng quan nhanh: hôm nay / tuần này / tháng này / năm nay =====
        private void BuildQuickOverview(RevenueViewModel m)
        {
            var today = DateTime.Today;
            var weekStart = StartOfWeek(today);
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            m.TodayRevenue = SumPaid(today, today.AddDays(1));
            m.WeekRevenue = SumPaid(weekStart, weekStart.AddDays(7));
            m.MonthRevenue = SumPaid(monthStart, monthStart.AddMonths(1));
            m.YearRevenue = SumPaid(yearStart, yearStart.AddYears(1));
        }

        private decimal SumPaid(DateTime rangeStart, DateTime rangeEnd)
        {
            return db.Orders.Where(o => o.Status == "Paid" && o.PaidAt >= rangeStart && o.PaidAt < rangeEnd)
                            .Sum(o => (decimal?)o.PaidAmount) ?? 0m;
        }

        // ===== Nút kỳ trước / kỳ sau =====
        private static void BuildNavigation(RevenueViewModel m)
        {
            DateTime prev, next;
            switch (m.Period)
            {
                case "week":
                    prev = m.Start.AddDays(-7);
                    next = m.Start.AddDays(7);
                    break;
                case "month":
                    prev = m.Start.AddMonths(-1);
                    next = m.Start.AddMonths(1);
                    break;
                case "year":
                    prev = m.Start.AddYears(-1);
                    next = m.Start.AddYears(1);
                    break;
                default:
                    prev = m.Start.AddDays(-1);
                    next = m.Start.AddDays(1);
                    break;
            }

            m.PrevDate = prev.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            m.NextDate = next.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            m.HasNext = next.Date <= DateTime.Today;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null) _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
