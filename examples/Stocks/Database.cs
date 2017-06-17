using SQLite.Net;
using SQLite.Net.Platform.Generic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Stocks
{
    public class Database : SQLiteConnection
    {
        public Database(string path) : base(new SQLitePlatformGeneric(), path)
        {

            //Execute(
            //"CREATE TABLE STOCK( " +
            //"    Id INT PRIMARY KEY     NOT NULL, " +
            //"    Symbol           TEXT NOT NULL );"
            //);

            Execute(
            "CREATE TABLE VALUATION( " +
            "    ID STRING PRIMARY KEY     NOT NULL, " +
            "    STOCKID           INT NOT NULL, " +
            //"    Time           TEXT NOT NULL, " +
            "    Price           TEXT NOT NULL " +
           ");");

            UpdateStock("BHP.AX");

        }

        public IEnumerable<Valuation> QueryValuations(Stock stock)
        {
            return null;
            //return Table<Valuation> ().Where(x => x.StockId == stock.Id);
        }

        public Valuation QueryLatestValuation(Stock stock)
        {
            return null;
            //return Table<Valuation> ().Where(x => x.StockId == stock.Id).OrderByDescending(x => x.Time).Take(1).FirstOrDefault();
        }


        public void UpdateStock(string stockSymbol)
        {
            var stock = new Stock { Symbol = stockSymbol };


            //
            // Get the latest valuations
            //
            try
            {
                var valuations = new YahooScraper().GetValuations(stock, DateTime.Now.AddYears(1), DateTime.Now);

                foreach (var valuation in valuations)
                {
                    Execute("INSERT INTO VALUATION (ID, STOCKID, Price) VALUES(@Param1, @Param2, @Param3);", new object[] { Guid.NewGuid().ToString(), valuation.StockId, valuation.Price });
                }

            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}