using System;
using System.Collections.Generic;
using System.Globalization;

namespace Stocks
{
    public class YahooScraper
    {
        public IEnumerable<Valuation> GetValuations(Stock stock, DateTime start, DateTime end)
        {
            var url = $"http://finance.yahoo.com/d/quotes.csv?s=" + stock.Symbol + "&f=snd1l1yr";

            Console.WriteLine("GET {0}", url);
            var req = System.Net.WebRequest.Create(url);
            using (var resp = new System.IO.StreamReader(req.GetResponse().GetResponseStream()))
            {
                var first = true;
                var dateCol = 0;
                var priceCol = 6;
                for (var line = resp.ReadLine(); line != null; line = resp.ReadLine())
                {
                    var parts = line.Split(',');
                    if (first)
                    {
                        dateCol = Array.IndexOf(parts, "Date");
                        priceCol = Array.IndexOf(parts, "Adj Close");
                        first = false;
                    }
                    else
                    {
                        yield return new Valuation
                        {
                            StockId = stock.Id,
                            Price = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                        };
                    }
                }
            }
        }
    }
}