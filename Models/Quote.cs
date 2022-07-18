namespace QPlixInvestment.Models
{
    using System;
    using System.Collections.Generic;

    public class Quote
    {
        public string ISIN { get; set; }

        public DateTime Date { get; set; }

        public decimal PricePerShare { get; set; }

        public Quote(string share, DateTime date, decimal pricePerShare)
        {
            ISIN = share;
            Date = date;
            PricePerShare = pricePerShare;
        }

        public Quote(List<string> fields)
        {
            ISIN = fields[0];
            Date = DateTime.Parse(fields[1]);
            PricePerShare = decimal.Parse(fields[2]);
        }
        public override string ToString() => $"ISIN = {ISIN}, Date = {Date}, {PricePerShare}";
    }
}
