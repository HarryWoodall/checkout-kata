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
        private readonly IMessageHelper printMessage;
        public StockProvider(IStockPricesStore store, IMessageHelper printMessage)
        {
            this.store = store;
            this.printMessage = printMessage;
        }

        public StockItem? GetStockItem(string sku)
        {
            try {
                var result = store.ReadData(FileConstants.DATA_FILE_NAME);
                var stockStore = JsonSerializer.Deserialize<StockItem[]>(result);

                if (stockStore is null) {
                    printMessage.Print($"{ErrorConstants.STOCK_DATA_FORMAT_ERROR} - Failed to parse stock store");
                    return null;
                }

                var stockItem = stockStore.ToList().Find(_ => _.SKU.Equals(sku));

                if (stockItem is null) {
                    printMessage.Print($"{ErrorConstants.ITEM_NOT_FOUND} - Item with SKU {sku} does not exist");
                    return null;
                }

                return stockItem;
            }
            catch (JsonException) {
                printMessage.Print($"{ErrorConstants.STOCK_DATA_FORMAT_ERROR} - File {FileConstants.DATA_FILE_NAME} is incorrectly formatted");
                return null;
            }
            catch (FileNotFoundException) {
                printMessage.Print($"{ErrorConstants.FILE_NOT_FOUND} - File {FileConstants.DATA_FILE_NAME} does not exist");
                return null;
            }
            catch (Exception e) {
                printMessage.Print($"{ErrorConstants.GENERAL_ERROR}:: {e.Message}");
                return null;
            }
            
        }
    }
}
