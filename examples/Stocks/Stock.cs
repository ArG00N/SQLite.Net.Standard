namespace Stocks
{
    public class Stock
	{
		public int Id { get; set; }
		public string Symbol { get; set; }

		public override string ToString ()
		{
			return Symbol;
		}
	}
}
