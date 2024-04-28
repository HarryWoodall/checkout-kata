using checkout_kata.Models;
using checkout_kata.Stores;

namespace checkout_kata.Providers
{
    public class StockProvider : IStockProvider
    {
        private IStockPricesStore store;
        public StockProvider(IStockPricesStore store)
        {
            this.store = store;
        }

        public async Task<StockItem?> GetStockItem(string sku)
        {
            throw new NotImplementedException();
        }
    }
}
