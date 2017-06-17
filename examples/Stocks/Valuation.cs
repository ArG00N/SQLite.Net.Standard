using System;

namespace Stocks
{
    public class Valuation
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        //public DateTime Time { get; set; }
        public decimal Price { get; set; }

        public override string ToString()
        {
            return $"Stock: {StockId} Price: {Price}";
        }
    }
}