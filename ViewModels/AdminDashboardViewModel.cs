using System;
using System.Collections.Generic;

namespace TP2.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Basic statistics
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }

        // Sales statistics
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }

        // Product statistics
        public int LowStockProducts { get; set; }
        public int OutOfStockProducts { get; set; }

        // Chart data
        public List<SalesData> SalesData { get; set; } = new List<SalesData>();
        public List<CategoryData> CategoryDistribution { get; set; } = new List<CategoryData>();
        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public List<GrowthData> MonthlyGrowth { get; set; } = new List<GrowthData>();

        // User roles statistics
        public Dictionary<string, int> UserRoles { get; set; } = new Dictionary<string, int>();
    }

    public class SalesData
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }

    public class CategoryData
    {
        public string CategoryName { get; set; }
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
    }

    public class TopProduct
    {
        public string Name { get; set; }
        public int Sales { get; set; }
        public decimal Revenue { get; set; }
    }

    public class GrowthData
    {
        public string Month { get; set; }
        public decimal Revenue { get; set; }
        public decimal Growth { get; set; }
    }
}