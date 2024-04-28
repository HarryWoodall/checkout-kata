namespace checkout_kata.Stores
{
    public interface IStockPricesStore
    {
        public Task<string> ReadData(string fileName);
    }
}
