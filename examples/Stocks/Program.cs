using SQLite.Net.Platform.Generic;
using System;
using System.IO;
using Path = System.IO.Path;

namespace Stocks.CommandLine
{
    class Program
    {
        public static void Main(string[] args)
        {
            new Program().Run();
        }


        void Run()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Stocks.db");

            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
            }

            Console.WriteLine("Creating database and valuation table...");

            var database = new Stocks.Database(dbPath, new SQLitePlatformGeneric());

            Console.WriteLine("Downloading data and inserting in to table...");

            database.UpdateStock("GE");

            Console.WriteLine("Getting data from database...");

            var data = database.GetData();

            foreach (var row in data.Data)
            {
                Console.WriteLine($"Price: {row["Price"]}");
            }

            Console.WriteLine("Done.");

            Console.Read();
        }
    }
}
