using checkout_kata.Services;
using checkout_kata.Providers;
using checkout_kata.Models;
using checkout_kata.Helpers;
using Moq;
using checkout_kata.Constants;

namespace checkout_kata_tests.ServiceTests
{
    public class CheckoutTests
    {
        private readonly Mock<IStockProvider> mockStockProvider;
        private readonly Mock<IMessageHelper> mockMessageHelper;
        private readonly Checkout checkout;

        public CheckoutTests()
        {
            mockStockProvider = new Mock<IStockProvider>();
            mockMessageHelper = new Mock<IMessageHelper>();

            mockStockProvider
                .Setup(_ => _.GetStockItem(It.IsAny<string>()))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            checkout = new Checkout(mockStockProvider.Object, mockMessageHelper.Object);
        }

        [Fact]
        public void CheckoutScan_ShouldAddItemToScannedItems_IfItemExists()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
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
        public void CheckoutScan_ShouldDisplayError_IfItemPriceIsInvalid()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = -1,
                    SpecialPrice = ""
                });

            // Act
            checkout.Scan("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.STOCK_ITEM_INVALID_PRICE))), Times.Once);
            Assert.Empty(checkout.scannedItems);
        }

        [Fact]
        public void CheckoutScan_ShouldDisplayError_IfItemSpecialPriceIsInvalid()
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = "INVALID_SPECIAL_PRICE"
                });

            // Act
            checkout.Scan("A");

            // Assert
            mockMessageHelper.Verify(_ => _.Print(It.Is<string>(_ => _.Contains(ErrorConstants.STOCK_ITEM_INVALID_SPECIAL_PRICE))), Times.Once);
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
        [InlineData(45, new string[] { "A", "A", "A", "A", "A", "A" })]
        [InlineData(35, new string[] { "A", "A", "B", "B", "B" })]
        [InlineData(45, new string[] { "A", "B", "A", "B", "A", "B" })]
        public void CheckoutGetTotalPrice_ShouldCorrectlyCalculatePrice_IfItemHasSpecialPrice(int expectedPrice, string[] items)
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = "2 for 15"
                });

            mockStockProvider
                .Setup(_ => _.GetStockItem("B"))
                .Returns(new StockItem()
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
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            mockStockProvider
                .Setup(_ => _.GetStockItem("B"))
                .Returns(new StockItem()
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

        [Theory]
        [InlineData(22, new string[] { "A", "B" })]
        [InlineData(27, new string[] { "A", "A", "B" })]
        [InlineData(54, new string[] { "A", "A", "A", "A", "B", "B" })]
        [InlineData(51, new string[] { "A", "A", "B", "B", "B" })]
        [InlineData(61, new string[] { "A", "B", "A", "B", "A", "B" })]
        public void CheckoutGetTotalPrice_ShouldCorrectlyCalculatePrice_IfSomeItemsHaveSpecialPrice(int expectedPrice, string[] items)
        {
            // Arrange
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = "2 for 15"
                });

            mockStockProvider
                .Setup(_ => _.GetStockItem("B"))
                .Returns(new StockItem()
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

        [Fact]
        public void CheckoutGetTotalPrice_ShouldEmptyBasket_AfterPriceHasBeenCalculated()
        {
            // Act
            checkout.Scan("A");
            checkout.Scan("A");
            checkout.Scan("A");

            checkout.GetTotalPrice();

            // Assert
            Assert.Empty(checkout.scannedItems);
        }

        [Fact]
        public void CheckoutGetTotalPrice_ShouldGiveDifferentPrice_IfPriceDataHasChangedBetweenCheckoutSessions()
        {
            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 10,
                    SpecialPrice = ""
                });

            checkout.Scan("A");
            checkout.Scan("A");
            checkout.Scan("A");

            var firstTotalPrice = checkout.GetTotalPrice();

            mockStockProvider
                .Setup(_ => _.GetStockItem("A"))
                .Returns(new StockItem()
                {
                    SKU = "A",
                    UnitPrice = 15,
                    SpecialPrice = ""
                });

            checkout.Scan("A");
            checkout.Scan("A");
            checkout.Scan("A");

            var secondTotalPrice = checkout.GetTotalPrice();

            Assert.Equal(30, firstTotalPrice);
            Assert.Equal(45, secondTotalPrice);
        }
    }
}