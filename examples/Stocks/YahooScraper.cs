using System;
using System.Collections.Generic;
using System.Globalization;

namespace Stocks
{
    public class YahooScraper
    {
        public List<Valuation> GetValuations(Stock stock, DateTime start, DateTime end)
        {
            var retVal = new List<Valuation>();

#if (!NETSTANDARD1_4)

            var url = $"http://finance.yahoo.com/d/quotes.csv?s=" + stock.Symbol + "&f=snd1l1yr";

            var req = System.Net.WebRequest.Create(url);
            using (var resp = new System.IO.StreamReader(req.GetResponse().GetResponseStream()))
            {
                for (var line = resp.ReadLine(); line != null; line = resp.ReadLine())
                {
                    var parts = line.Split(',');

                    retVal.Add(new Valuation
                    {
                        StockId = stock.Id,
                        Price = decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                    });
                }
            }
#else
            retVal.Add(new Valuation { StockId = stock.Id, Price = 10 });
#endif

            return retVal;
        }
    }
}