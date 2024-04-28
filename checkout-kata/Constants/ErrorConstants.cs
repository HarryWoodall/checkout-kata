using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace checkout_kata.Constants
{
    public static class ErrorConstants
    {
        public static readonly string GENERAL_ERROR = "An Error has occured";
        public static readonly string FILE_NOT_FOUND = "Data file not found";
        public static readonly string ITEM_NOT_FOUND = "Item not found";
        public static readonly string STOCK_DATA_FORMAT_ERROR = "Stock data is invalid";
        public static readonly string STOCK_ITEM_INVALID_PRICE = "Stock item invalid price";
        public static readonly string STOCK_ITEM_INVALID_SPECIAL_PRICE = "Stock item invalid special price";
    }
}
