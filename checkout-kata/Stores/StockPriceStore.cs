using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace checkout_kata.Stores
{
    public class StockPriceStore : IStockPricesStore
    {
        public string ReadData(string fileName)
        {
            using FileStream stream = File.OpenRead(fileName);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
