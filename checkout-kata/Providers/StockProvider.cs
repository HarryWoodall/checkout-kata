using checkout_kata.Constants;
using checkout_kata.Helpers;
using checkout_kata.Models;
using checkout_kata.Stores;
using System.Text.Json;

namespace checkout_kata.Providers
{
    public class StockProvider : IStockProvider
    {
        private readonly IStockPricesStore store;
        private readonly IMessageHelper messageHelper;
        public StockProvider(IStockPricesStore store, IMessageHelper messageHelper)
        {
            this.store = store;
            this.messageHelper = messageHelper;
        }

        public StockItem? GetStockItem(string sku)
        {
            try {
                var result = store.ReadData(FileConstants.DATA_FILE_NAME);
                var stockStore = JsonSerializer.Deserialize<StockItem[]>(result);

                if (stockStore is null) {
                    messageHelper.Print($"{ErrorConstants.STOCK_DATA_FORMAT_ERROR} - Failed to parse stock store");
                    return null;
                }

                var stockItem = stockStore.ToList().Find(_ => _.SKU.Equals(sku));

                if (stockItem is null) {
                    messageHelper.Print($"{ErrorConstants.ITEM_NOT_FOUND} - Item with SKU {sku} does not exist");
                    return null;
                }

                return stockItem;
            }
            catch (JsonException) {
                messageHelper.Print($"{ErrorConstants.STOCK_DATA_FORMAT_ERROR} - File {FileConstants.DATA_FILE_NAME} is incorrectly formatted");
                return null;
            }
            catch (FileNotFoundException) {
                messageHelper.Print($"{ErrorConstants.FILE_NOT_FOUND} - File {FileConstants.DATA_FILE_NAME} does not exist");
                return null;
            }
            catch (Exception e) {
                messageHelper.Print($"{ErrorConstants.GENERAL_ERROR}:: {e.Message}");
                return null;
            }
            
        }
    }
}
