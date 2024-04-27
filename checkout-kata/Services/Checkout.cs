using checkout_kata.Models;
using checkout_kata.Providers;
using checkout_kata.Helpers;

namespace checkout_kata.Services
{
    public class Checkout : ICheckout
    {
        private IStockProvider stockProvider;
        private IPrintMessage printMessage;

        public List<StockItem> scannedItems = new List<StockItem>();

        public Checkout(IStockProvider stockProvider, IPrintMessage printMessage)
        {
            this.stockProvider = stockProvider;
            this.printMessage = printMessage;
        }

        public int GetTotalPrice()
        {
            throw new NotImplementedException();
        }

        public async void Scan(string item)
        {
            throw new NotImplementedException();
        }
    }
}
