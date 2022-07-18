namespace QPlixInvestment
{
    using QPlixInvestment.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class PortfolioModeler
    {
        private Dictionary<string, List<Transaction>> MapTransactions { get; set; }
        private Dictionary<string, List<Investment>> MapInvestments { get; set; }
        private Dictionary<string, List<Quote>> MapStock { get; set; }

        public PortfolioModeler()
        {
            MapTransactions = new Dictionary<string, List<Transaction>>();
            MapInvestments = new Dictionary<string, List<Investment>>();
            MapStock = new Dictionary<string, List<Quote>>();
        }

        public Result<int> ParseFiles(string dataFolderPath, string investmentFileName, string quotesFileName, string transactionsFileName)
        {
            if (!Directory.Exists(dataFolderPath))
            {
                return Result<int>.Fail($"Can't find folder {dataFolderPath}");
            }
            var transactionsPath = Path.Combine(dataFolderPath, transactionsFileName);
            var investmentsPath = Path.Combine(dataFolderPath, investmentFileName);
            var quotesPath = Path.Combine(dataFolderPath, quotesFileName);

            if (!File.Exists(transactionsPath))
            {
                return Result<int>.Fail($"Can't find file {transactionsPath}");
            }

            if (!File.Exists(investmentsPath))
            {
                return Result<int>.Fail($"Can't find file {investmentsPath}");
            }

            if (!File.Exists(quotesPath))
            {
                return Result<int>.Fail($"Can't find file {quotesPath}");
            }

            MapTransactions = FileUtil.ParseLines(transactionsPath)
                .Select(fields => new Transaction(fields))
                .GroupBy(i => i.InvestmentId, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Date).ToList(), StringComparer.InvariantCultureIgnoreCase);

            MapInvestments = FileUtil.ParseLines(investmentsPath)
                .Select(fields => new Investment(fields))
                .GroupBy(i => i.InvestorId, StringComparer.InvariantCultureIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.InvariantCultureIgnoreCase);

            MapStock = FileUtil.ParseLines(quotesPath)
               .Select(fields => new Quote(fields))
               .GroupBy(i => i.ISIN, StringComparer.InvariantCultureIgnoreCase)
               .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Date).ToList(), StringComparer.InvariantCultureIgnoreCase);

            return Result<int>.OK(MapTransactions.Count);
        }

        private bool TryGetInvestments(string investorId, out List<Investment>? investments)
            => MapInvestments.TryGetValue(investorId, out investments);

        private decimal GetFondsSum(List<Transaction> transactions, string fundId, DateTime date)
        {
            var fundFound = TryGetInvestments(fundId, out var fundInvestments);
            if (!fundFound)
            {
                return 0;
            }

            if (!transactions.Any())
            {
                return 0;
            }

            var fundValue = GetInvestmentsSum(fundInvestments, date);
            var initialValue = transactions[0].Value;

            for (int i = 1; i < transactions.Count; i++)
            {
                initialValue *= (1 + transactions[i].Value / 100);
            }

            return initialValue * fundValue;
        }

        private decimal GetRealEstateSum(List<Transaction> transactions) => transactions.Sum(d => d.Value);

        private decimal GetStockSum(List<Transaction> transactions, string ISIN, DateTime date)
        {
            var totalShares = transactions.Sum(t => t.Value);
            var lastISINPrice = MapStock[ISIN].Where(d => d.Date < date).OrderByDescending(d => d.Date).FirstOrDefault()?.PricePerShare ?? 0;
            return lastISINPrice * totalShares;
        }

        public Result<decimal> GetWorthByDate(string investorId, DateTime date)
        {
            if (!TryGetInvestments(investorId, out var investments))
            {
                return Result<decimal>.Fail($"Investor {investorId} not found, please try another one.");
            }

            var sum = GetInvestmentsSum(investments, date);
            return Result<decimal>.OK(sum);
        }


        private decimal GetInvestmentsSum(List<Investment>? investments, DateTime date)
        {
            decimal sum = 0;

            foreach (var investment in investments)
            {
                var transactions = MapTransactions[investment.InvestmentId].FindAll(d => d.Date < date);

                switch (investment.InvestmentType)
                {
                    case InvestmentTypes.Fonds:
                        sum += GetFondsSum(transactions, investment.FondsInvestor, date);
                        break;

                    case InvestmentTypes.RealEstate:
                        sum += GetRealEstateSum(transactions);
                        break;

                    case InvestmentTypes.Stock:
                        sum += GetStockSum(transactions, investment.ISIN, date);
                        break;
                }
            }
            return sum;
        }
    }
}
