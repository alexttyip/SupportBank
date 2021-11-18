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
                    var fields = parser.ReadFields();
                    transactions.Add(new Transaction(fields));
                }
            }

            PrintListAll(transactions);
        }

        private static void PrintListAll(List<Transaction> transactions)
        {
            var people = new Dictionary<string, Person>();

            foreach (var transaction in transactions)
            {
                var from = transaction.From;
                var to = transaction.To;

                if (!people.ContainsKey(from))
                    people[from] = new Person(from);

                if (!people.ContainsKey(to))
                    people[to] = new Person(to);

                people[from].Owes += transaction.Amount;
                people[to].Owed += transaction.Amount;
            }

            foreach (var (name, person) in people)
            {
                var owe = person.Owes;
                var owed = person.Owed;

                Console.Out.WriteLine($"{name} owes {owe:F2} and is owed {owed:F2}");
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

    class Person
    {
        public string Name { get; }
        public double Owes { get; set; }
        public double Owed { get; set; }

        public Person(string name)
        {
            Name = name;
            Owes = 0;
            Owed = 0;
        }
    }
}