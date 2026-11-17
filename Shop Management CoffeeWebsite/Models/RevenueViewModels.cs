using System;
using System.Collections.Generic;

namespace CafeManagement.Models
{
    // Một cột trên biểu đồ doanh thu (giờ / ngày / tháng tùy kỳ xem)
    public class RevenuePoint
    {
        public string Label { get; set; }
        public string SubLabel { get; set; }
        public string Tooltip { get; set; }
        public decimal Amount { get; set; }
        public int Orders { get; set; }
    }

    public class RevenueTopProduct
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class RevenuePayMethod
    {
        public string Method { get; set; }
        public int Orders { get; set; }
        public decimal Amount { get; set; }
    }

    public class RevenueViewModel
    {
        // day | week | month | year
        public string Period { get; set; }
        public DateTime Date { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }          // không bao gồm ngày End
        public string RangeTitle { get; set; }
        public string ChartTitle { get; set; }

        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }
        public int ItemCount { get; set; }
        public decimal AverageOrder { get; set; }
        public decimal MaxAmount { get; set; }
        public string BestLabel { get; set; }

        // Tổng quan nhanh (luôn hiển thị)
        public decimal TodayRevenue { get; set; }
        public decimal WeekRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }

        public string PrevDate { get; set; }
        public string NextDate { get; set; }
        public bool HasNext { get; set; }

        public List<RevenuePoint> Points { get; set; }
        public List<RevenueTopProduct> TopProducts { get; set; }
        public List<RevenuePayMethod> PayMethods { get; set; }
        public List<Order> Orders { get; set; }
        public bool OrdersTruncated { get; set; }

        public RevenueViewModel()
        {
            Points = new List<RevenuePoint>();
            TopProducts = new List<RevenueTopProduct>();
            PayMethods = new List<RevenuePayMethod>();
            Orders = new List<Order>();
        }
    }
}
