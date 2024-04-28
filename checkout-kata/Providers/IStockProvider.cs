using checkout_kata.Models;
using System.Text.Json;

namespace checkout_kata.Providers
{
    public interface IStockProvider
    {
        StockItem? GetStockItem(string sku);
    }
}
