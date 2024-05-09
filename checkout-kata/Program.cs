using checkout_kata.Helpers;
using checkout_kata.Providers;
using checkout_kata.Services;
using checkout_kata.Stores;

MessageHelper messageHelper = new MessageHelper();
StockPriceStore stockPriceStore = new StockPriceStore();
StockProvider stockProvider = new StockProvider(stockPriceStore, messageHelper);

Checkout checkout = new Checkout(stockProvider, messageHelper);

checkout.Scan("A");
checkout.Scan("A");
checkout.Scan("A");

checkout.Scan("B");
checkout.Scan("B");

checkout.Scan("C");

checkout.Scan("D");

messageHelper.Print($"Total price: {checkout.GetTotalPrice()}");
