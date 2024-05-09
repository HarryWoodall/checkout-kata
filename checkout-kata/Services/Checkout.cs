using checkout_kata.Models;
using checkout_kata.Providers;
using checkout_kata.Helpers;
using checkout_kata.Constants;
using System.Text.RegularExpressions;

namespace checkout_kata.Services
{
    public class Checkout : ICheckout
    {
        private IStockProvider stockProvider;
        private IMessageHelper messageHelper;

        public List<StockItem> scannedItems = new List<StockItem>();

        public Checkout(IStockProvider stockProvider, IMessageHelper messageHelper)
        {
            this.stockProvider = stockProvider;
            this.messageHelper = messageHelper;
        }

        public int GetTotalPrice()
        {
            int total = 0;

            while (scannedItems.Count > 0) {
                StockItem currentItem = scannedItems[0];
                List<StockItem> itemsOfType = scannedItems.FindAll(_ => _.SKU.Equals(currentItem.SKU));
                total += CalculateCostOfItems(itemsOfType.Count, currentItem.UnitPrice, currentItem.SpecialPrice);
                scannedItems = scannedItems.FindAll(_ => !_.SKU.Equals(currentItem.SKU));
            }

            scannedItems.Clear();
            return total;
        }

        public void Scan(string item)
        {
            var stockItem = stockProvider.GetStockItem(item);

            if (stockItem is null) return;

            if (stockItem.UnitPrice < 0) {
                messageHelper.Print($"{ErrorConstants.STOCK_ITEM_INVALID_PRICE} - item {item} has invalid price of {stockItem.UnitPrice}");
                return;
            }

            string specialPriceRegex = @"^\d+ for \d+$";
            var match = Regex.Match(stockItem.SpecialPrice, specialPriceRegex);
            if (!stockItem.SpecialPrice.Equals("") && !match.Success) {
                messageHelper.Print($"{ErrorConstants.STOCK_ITEM_INVALID_SPECIAL_PRICE} - item {item} has invalid price of {stockItem.UnitPrice}");
                return;
            }

            scannedItems.Add(stockItem);
        }

        private int CalculateCostOfItems(int ammount, int cost, string specialOffer)
        {
            if (specialOffer.Equals("")) return ammount * cost;

            int offerAmmount = int.Parse(specialOffer.Split(" ")[0]);
            int offerPrice = int.Parse(specialOffer.Split(" ")[2]);

            return ammount / offerAmmount * offerPrice + ((ammount % offerAmmount) * cost);
        }
    }
}
