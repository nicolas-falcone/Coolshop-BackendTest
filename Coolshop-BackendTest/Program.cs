
using CsvHelper;
using System.Globalization;


/*
 *Backend Test:
   Si implementi uno script da eseguire da linea di comando, utilizzando a piacimento uno tra NodeJs e C#.
   
   Lo script riceve in input il percorso di un file csv da utilizzare, contenente la lista di ordini di un sito ecommerce.
   
   Es. file csv
   Id,Article Name,Quantity,Unit price,Percentage discount,Buyer
   1,Coke,10,1,0,Mario Rossi
   2,Coke,15,2,0,Luca Neri
   3,Fanta,5,3,2,Luca Neri
   4,Water,20,1,10,Mario Rossi
   5,Fanta,1,4,15,Andrea Bianchi
   
   Lo script deve dare in output i seguenti dati:
   Record con importo totale più alto
   Record con quantità più alta
   Record con maggior differenza tra totale senza sconto e totale con sconto
 */


namespace Coolshop_BackendTest
{
    internal class Program
    {
        public class Transaction
        {
            public int Id { get; set; }
            public string? ArticleName { get; set; }
            public int Quantity { get; set; } = 0;
            public double UnitPrice { get; set; } = 0;
            public int PercentageDiscount { get; set; } = 0;
            public string? Buyer { get; set; }
        }

        private static string RecordString(Transaction transaction)
        {
            return $"Id: {transaction.Id}, " +
                   $"ArticleName: {transaction.ArticleName}, " +
                   $"Quantity: {transaction.Quantity}, " +
                   $"UnitPrice: {transaction.UnitPrice}, " +
                   $"PercentageDiscount: {transaction.PercentageDiscount}, " +
                   $"Buyer: {transaction.Buyer}";
        }

        private static double TotalTransaction(Transaction transaction)
        {
            return transaction.UnitPrice * transaction.Quantity;
        }

        private static double DiscountedDifferencePrice(Transaction transaction)
        {
            if (transaction.PercentageDiscount != 0)
            {
                return TotalTransaction(transaction) - TotalTransaction(transaction) * (1 - transaction.PercentageDiscount / 100.0);
            }
            return 0.0;
        }

        public static void Main()
        {
            try
            {
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data.csv");
                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<Transaction>();
                var transactions = records.ToList();

                if (transactions.Count == 0)
                {
                    Console.WriteLine("No transaction found");
                    Console.ReadLine();
                    return;
                }

                var maxTotalTransaction = new Transaction();
                var maxQuantityTransaction = new Transaction();
                var maxDiscountDifferenceTransaction = new Transaction();

                foreach (var transaction in transactions)
                {
                    if (TotalTransaction(transaction) > TotalTransaction(maxTotalTransaction))
                    {
                        maxTotalTransaction = transaction;
                    }
                    if (transaction.Quantity > maxQuantityTransaction.Quantity)
                    {
                        maxQuantityTransaction = transaction;
                    }
                    if (DiscountedDifferencePrice(transaction) > DiscountedDifferencePrice(maxDiscountDifferenceTransaction))
                    {
                        maxDiscountDifferenceTransaction = transaction;
                    }
                }

                Console.WriteLine("Transaction with the highest total amount:");
                Console.WriteLine(RecordString(maxTotalTransaction));
                Console.WriteLine();

                Console.WriteLine("Transaction with the highest quantity:");
                Console.WriteLine(RecordString(maxQuantityTransaction));
                Console.WriteLine();

                Console.WriteLine("Transaction with the largest discount difference:");
                Console.WriteLine(RecordString(maxDiscountDifferenceTransaction));
                Console.WriteLine();

                Console.ReadLine();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found");
            }
            catch(CsvHelperException)
            {
                Console.WriteLine("Error reading csv file");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
