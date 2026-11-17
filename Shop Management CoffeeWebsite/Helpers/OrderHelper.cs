using CafeManagement.Models;
using System;
using System.Linq;
using System.Web;

namespace Shop_Management_CoffeeWebsite.Helpers
{
    public static class OrderHelper
    {
        // Số món đang gọi (hóa đơn chưa thanh toán) để hiện trên thanh điều hướng.
        // Mọi lỗi đều được nuốt lại để không làm hỏng giao diện chung.
        public static string CartBadgeText(HttpSessionStateBase session)
        {
            try
            {
                var uid = session["UserId"] as int?;
                if (uid == null) return "";

                using (var db = new OrderDbContext())
                {
                    var n = (from i in db.OrderItems
                             where i.Order.UserId == uid.Value && i.Order.Status == "Open"
                             select (int?)i.Quantity).Sum() ?? 0;
                    return n > 0 ? n.ToString() : "";
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
