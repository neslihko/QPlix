using System;
using System.Configuration;

namespace QPlixInvestment
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Parsing files");
            var dataFolder = ConfigurationManager.AppSettings["DataPath"] ?? Environment.CurrentDirectory;

            var portfolio = new PortfolioModeler();
            var parseResult = portfolio.ParseFiles(dataFolder, "Investments.csv", "Quotes.csv", "Transactions.csv");

            if (!parseResult.Success)
            {
                Console.WriteLine(parseResult.Message);
                return;
            }

            Console.WriteLine("Done!");
            var separator = string.Concat(Environment.NewLine, new string('=', 80), Environment.NewLine);

            while (true)
            {
                Console.WriteLine(separator);
                Console.WriteLine("Enter 'Date;InvestorId' below: (enter to exit)");

                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine("Exiting...");
                    break;
                }

                var queryResult = ParseQuery(line);
                if (!queryResult.Success)
                {
                    Console.WriteLine($"\tERROR: {queryResult.Message}");
                    continue;
                }

                var query = queryResult.Data;
                var lookupResult = portfolio.GetWorthByDate(query.InvestorId, query.Date);

                if (!lookupResult.Success)
                {
                    Console.WriteLine($"\tERROR: {lookupResult.Message}");
                    continue;
                }

                Console.WriteLine($"Investor '{query.InvestorId}' is worth {lookupResult.Data:N2} by date {query.Date}");
            }


        }

        private static Result<Query> ParseQuery(string? line)
        {
            var input = line.Split(";");
            if (input.Length != 2)
            {
                return Result<Query>.Fail("Separator ';' not found");
            }

            if (!DateTime.TryParse(input[0], out var date))
            {
                return Result<Query>.Fail($"Can't parse {input[0]} into valid date");
            }

            var investorId = input[1];
            if (string.IsNullOrWhiteSpace(investorId))
            {
                return Result<Query>.Fail("InvestorId can't be null");
            }

            var query = new Query(investorId, date);

            return Result<Query>.OK(query);
        }
    }
}
