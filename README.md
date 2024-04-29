# Checkout kata

A soloution to the checkout kata written as a console application using C#.

### Design
The solution contains 2 projects, checkout-kata and checkout-kata-tests

The data is retrieved from a json file (`stockprices.json`) and transformed into a `StockItem` object within `StockProvider` 

During an item 'scan' the data is pulled through, validated and an object is appended to the `scannedItems` list

The `GetTotalPrice` will perform the necessary calculations, including any special offers, clear the list of scanned items and return the result.

![CheckoutKataDiagram](https://github.com/HarryWoodall/checkout-kata/assets/20969276/6e6c3113-ef7a-4ae2-a44c-67fe966a4ef0)
