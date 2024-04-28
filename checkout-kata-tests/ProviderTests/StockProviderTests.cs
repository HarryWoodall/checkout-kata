using checkout_kata.Constants;
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
        private readonly Mock<IMessageHelper> mockMessageHelper;

        public StockProviderTests()
        {
            stockPricesStore = new Mock<IStockPricesStore>();
            mockMessageHelper = new Mock<IMessageHelper>();
            stockProvider = new StockProvider(stockPricesStore.Object, mockMessageHelper.Object);

            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Returns("[{\"SKU\":\"A\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]");
        }

        [Fact]
        public void StockProviderGetStockItem_ShouldPrintFileNotFoundError_WhenStockPriceStoreThrowsFileNotFoundException()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Throws(new FileNotFoundException("error"));

            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.FILE_NOT_FOUND))), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void StockProviderGetStockItem_ShouldPrintError_WhenStockPriceStoreThrows()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Throws(new Exception("error"));

            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.GENERAL_ERROR))), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void StockProviderGetStockItem_ShouldPrintError_WhenStockPriceStoreReturnsInvalidString()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Returns("{}");

            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.STOCK_DATA_FORMAT_ERROR))), Times.Once);
            Assert.Null(result);
        }

        [Theory]
        [InlineData("[{\"SKU\":\"A\",\"UnitPrice\":10}]")]
        [InlineData("[{\"SKU\":\"A\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]")]
        public void StockProviderGetStockItem_SpecialPriceShouldBeEmptyString_WhenSpecialPriceInDataIsNullOrEmpty(string data)
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Returns(data);

            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result!.SpecialPrice);
        }

        [Fact]
        public void StockProviderGetStockItem_ShouldReturnNull_WhenSKUIsNotFound()
        {
            // Arrange
            stockPricesStore
                .Setup(_ => _.ReadData(It.IsAny<string>()))
                .Returns("[{\"SKU\":\"B\",\"UnitPrice\":10,\"SpecialPrice\":\"\"}]");

            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.ITEM_NOT_FOUND))), Times.Once);
            Assert.Null(result);
        }

        [Fact]
        public void StockProviderGetStockItem_ShouldReturnStockItemObject_WhenGivenValidStockItemSKU()
        {
            // Act
            var result = stockProvider.GetStockItem("A");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A", result!.SKU);
            Assert.Equal(10, result!.UnitPrice);
            Assert.Equal("", result!.SpecialPrice);
        }
    }
}
