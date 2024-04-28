using checkout_kata.Helpers;
using checkout_kata.Providers;
using checkout_kata.Stores;
using Moq;

namespace checkout_kata_tests.ProviderTests
{
    public class StockProviderTests
    {
        private readonly StockProvider stockProvider;
        private readonly Mock<IStockPricesStore> stockPricesStore;
        private readonly Mock<IPrintMessage> mockPrintMessage;

        public StockProviderTests()
        {
            stockPricesStore = new Mock<IStockPricesStore>();
            mockPrintMessage = new Mock<IPrintMessage>();
            stockProvider = new StockProvider(stockPricesStore.Object);

            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .ReturnsAsync("[{\"SKU\":\"A\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]");
        }

        [Fact]
        public async void StockProviderGetStockItem_ShouldPrintError_WhenStockPriceStoreThrows()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .ThrowsAsync(new Exception("error"));

            // Act
            var result = await stockProvider.GetStockItem("A");

            // Assert
            mockPrintMessage.Verify(_ => _.Print(It.IsAny<string>()), Times.Once); //TODO - put correct error message in
            Assert.Null(result);
        }

        [Theory]
        [InlineData("[{\"SKU\":\"A\",\"UnitPrice\":10}]")]
        [InlineData("[{\"SKU\":\"A\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]")]
        public async void StockProviderGetStockItem_SpecialPriceShouldBeEmptyString_WhenSpecialPriceInDataIsNullOrEmpty(string data)
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .ReturnsAsync(data);

            // Act
            var result = await stockProvider.GetStockItem("A");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result!.SpecialPrice);
        }

        [Fact]
        public async void StockProviderGetStockItem_ShouldReturnNull_WhenSKUIsNotFound()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .ReturnsAsync("[{\"SKU\":\"B\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]");

            // Act
            var result = await stockProvider.GetStockItem("A");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async void StockProviderGetStockItem_ShouldReturnStockItemObject_WhenGivenValidStockItemSKU()
        {
            // Act
            var result = await stockProvider.GetStockItem("A");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A", result!.SKU);
            Assert.Equal(10, result!.UnitPrice);
            Assert.Equal("", result!.SpecialPrice);
        }
    }
}
