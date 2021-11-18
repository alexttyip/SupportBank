using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.FileIO;

namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            var transactions = new List<Transaction>();
            using (TextFieldParser parser = new TextFieldParser(@"C:\work\training\SupportBank\Transactions2014.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                // Skip header line
                parser.ReadLine();
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    transactions.Add(new Transaction(fields));
                }
            }

            foreach (var transaction in transactions)
            {
                Console.Out.WriteLine(transaction.From);
            }
        }
    }

    class Transaction
    {
        public DateTime Date { get; }
        public string From { get; }
        public string To { get; }
        public string Narrative { get; }
        public double Amount { get; }

        public Transaction(DateTime date, string from, string to, string narrative, double amount)
        {
            Date = date;
            From = from;
            To = to;
            Narrative = narrative;
            Amount = amount;
        }

        public Transaction(string[] csvFields) : this(
            DateTime.ParseExact(csvFields[0], "d", null),
            csvFields[1],
            csvFields[2],
            csvFields[3],
            double.Parse(csvFields[4])
        )
        {
        }
    }
}