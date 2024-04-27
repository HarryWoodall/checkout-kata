using checkout_kata.Services;
using checkout_kata.Providers;
using checkout_kata.Models;
using checkout_kata.Helpers;
using Moq;

namespace checkout_kata_tests.ServiceTests
{
    public class CheckoutTests
    {
        private readonly Mock<IStockProvider> mockStockProvider;
        private readonly Mock<IPrintMessage> mockPrintMessage;
        private readonly Checkout checkout;

        public CheckoutTests()
        {
            mockStockProvider = new Mock<IStockProvider>();
            mockPrintMessage = new Mock<IPrintMessage>();

            mockStockProvider
                .Setup(_ => _.GetStockItem(It.IsAny<string>()))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            checkout = new Checkout(mockStockProvider.Object, mockPrintMessage.Object);
        }

        [Fact]
        public void CheckoutScan_ShouldAddItemToScannedItems_IfItemExists()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            // Act
            checkout.Scan("A");

            // Assert
            Assert.Single(checkout.scannedItems);
            Assert.Equal("A", checkout.scannedItems[0].SKU);
        }

        [Fact]
        public void CheckoutScan_ShouldDisplayError_IfItemDoesNotExist()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem(It.IsAny<string>()))
                .Returns(Task.FromResult<StockItem?>(null));

            // Act
            checkout.Scan("A");

            // Assert
            mockPrintMessage.Verify(_ => _.Print(It.IsAny<string>()), Times.Once); //TODO - put correct error message in
            Assert.Empty(checkout.scannedItems);
        }

        [Fact]
        public void CheckoutScan_ShouldDisplayError_IfItemPriceIsInvalid()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = -1,
                    SpecialPrice = ""
                });

            // Act
            checkout.Scan("A");

            // Assert
            mockPrintMessage.Verify(_ => _.Print(It.IsAny<string>()), Times.Once); //TODO - put correct error message in
            Assert.Empty(checkout.scannedItems);
        }

        [Fact]
        public void CheckoutScan_ShouldDisplayError_IfItemSpecialPriceIsInvalid()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = "INVALID_SPECIAL_PRICE"
                });

            // Act
            checkout.Scan("A");

            // Assert
            mockPrintMessage.Verify(_ => _.Print(It.IsAny<string>()), Times.Once); //TODO - put correct error message in
            Assert.Empty(checkout.scannedItems);
        }

        [Fact]
        public void CheckoutGetTotalPrice_ShouldReturnZero_IfNoItemsInBasket()
        {
            // Act
            var result = checkout.GetTotalPrice();

            // Assert
            Assert.Equal(0, result);
        }


        [Theory]
        [InlineData(15, new string[] { "A", "A" })]
        [InlineData(25, new string[] { "A", "A", "A" })]
        [InlineData(30, new string[] { "A", "A", "A", "A", "A", "A" })]
        [InlineData(35, new string[] { "A", "A", "B", "B", "B" })]
        [InlineData(45, new string[] { "A", "B", "A", "B", "A", "B" })]
        public void CheckoutGetTotalPrice_ShouldCorrectlyCalculatePrice_IfItemHasSpecialPrice(int expectedPrice, string[] items)
        {
            // Assert
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = "2 for 15"
                });

            mockStockProvider
                .Setup(_ => _.GetStockItem("B"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "B",
                    UnitPrice = 12,
                    SpecialPrice = "3 for 20"
                });

            // Act
            items.ToList().ForEach(item => checkout.Scan(item));
            int result = checkout.GetTotalPrice();

            // Assert
            Assert.Equal(expectedPrice, result);
        }

        [Theory]
        [InlineData(20, new string[] { "A", "A" })]
        [InlineData(30, new string[] { "A", "A", "A" })]
        [InlineData(60, new string[] { "A", "A", "A", "A", "A", "A" })]
        [InlineData(56, new string[] { "A", "A", "B", "B", "B" })]
        [InlineData(66, new string[] { "A", "B", "A", "B", "A", "B" })]
        public void CheckoutGetTotalPrice_ShouldCorrectlyCalculatePrice_IfItemHasNoSpecialPrice(int expectedPrice, string[] items)
        {
            // Assert
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            mockStockProvider
                .Setup(_ => _.GetStockItem("B"))
                .ReturnsAsync(new StockItem()
                {
                    SKU = "B",
                    UnitPrice = 12,
                    SpecialPrice = ""
                });

            // Act
            items.ToList().ForEach(item => checkout.Scan(item));
            int result = checkout.GetTotalPrice();

            // Assert
            Assert.Equal(expectedPrice, result);
        }
    }
}