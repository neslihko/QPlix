using System;

namespace QPlixInvestment
{
    public class Query
    {
        public string InvestorId { get; set; }
        public DateTime Date { get; set; }

        public Query(string investorId, DateTime date)
        {
            InvestorId = investorId;
            Date = date;
        }
    }
}
