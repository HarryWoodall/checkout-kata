using System;
namespace checkout_kata.Models
{
    public class StockItem
    {
        public string SKU { get; set; } = "";
        public int UnitPrice { get; set; }
        public string SpecialPrice { get; set; } = "";
    }
}
