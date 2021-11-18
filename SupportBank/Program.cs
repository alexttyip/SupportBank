using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

namespace SupportBank
{
    class Program
    {
        private static readonly List<Transaction> Transactions = new();

        static void Main(string[] args)
        {
            string name = null;

            if (args.Length == 0)
            {
                Console.Out.WriteLine("Available commands are \"List All\" and \"List [Person]\".");
                return;
            }

            if (args[0] == "List")
            {
                if (args[1] != "All")
                    name = args[1];
            }
            else
            {
                Console.Out.WriteLine("Available commands are \"List All\" and \"List [Person]\".");
                return;
            }

            ReadCsv();

            if (name == null)
                PrintListAll();
            else
                try
                {
                    PrintPerson(name);
                }
                catch (UndefinedPersonException)
                {
                    Console.Out.WriteLine($"The person {name} is not found.");
                }
        }

        private static void ReadCsv()
        {
            using var parser = new TextFieldParser(@"C:\work\training\SupportBank\Transactions2014.csv");
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            // Skip header line
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                //Processing row
                var fields = parser.ReadFields();
                Transactions.Add(new Transaction(fields));
            }
        }

        private static void PrintListAll()
        {
            var people = new Dictionary<string, Person>();

            foreach (var transaction in Transactions)
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
                var owes = person.Owes;
                var owed = person.Owed;

                Console.Out.WriteLine($"{name} owes {owes:F2} and is owed {owed:F2}");
            }
        }

        private static void PrintPerson(string name)
        {
            var personTransactions =
                Transactions.FindAll(transaction => transaction.From == name || transaction.To == name);

            if (personTransactions.Count == 0)
                throw new UndefinedPersonException();

            foreach (var transaction in personTransactions)
            {
                Console.Out.WriteLine(
                    $"On {transaction.Date.ToString("d", CultureInfo.CurrentCulture)}, {transaction.From} owes {transaction.To} {transaction.Amount:F2} for {transaction.Narrative}.");
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
            DateTime.ParseExact(csvFields[0], "d", CultureInfo.CurrentCulture),
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

    public class UndefinedPersonException : Exception
    {
    }
}