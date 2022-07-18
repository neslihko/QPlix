using System;
using System.Collections.Generic;

namespace QPlixInvestment.Models
{
    public class Investment
    {
        public string InvestorId { get; set; }

        public string InvestmentId { get; set; }

        public InvestmentTypes InvestmentType { get; set; }

        public string ISIN { get; set; }

        public string City { get; set; }

        public string FondsInvestor { get; set; }

        public Investment(string investorId, string investmentId, InvestmentTypes investmentType, string isin, string city, string fondsInvestor)
        {
            InvestorId = investorId;
            InvestmentId = investmentId;
            InvestmentType = investmentType;
            ISIN = isin;
            City = city;
            FondsInvestor = fondsInvestor;
        }

        public Investment(List<string> fields)
            : this(fields[0], fields[1], Enum.Parse<InvestmentTypes>(fields[2]), fields[3], fields[4], fields[5])
        {
        }

        public override string ToString() => $"{InvestmentType}: {InvestmentId}, ISIN = {ISIN}, Investor = {FondsInvestor}";
    }
}
