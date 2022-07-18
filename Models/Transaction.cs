using System;
using System.Collections.Generic;

namespace QPlixInvestment.Models
{
    public class Transaction
    {
        public string InvestmentId { get; set; }

        public TransactionTypes TransactionType { get; set; }

        public DateTime Date { get; set; }

        public decimal Value { get; set; }

        public Transaction(string investmentId, TransactionTypes transactionType, DateTime date, decimal value)
        {
            InvestmentId = investmentId;
            TransactionType = transactionType;
            Date = date;
            Value = value;
        }

        public Transaction(List<string> fields)
        {
            InvestmentId = fields[0];
            TransactionType = Enum.Parse<TransactionTypes>(fields[1]);
            Date = DateTime.Parse(fields[2]);
            Value = decimal.Parse(fields[3]);
        }

        public override string ToString() => $"{TransactionType}: Date = {Date}; Value = {Value}";
    }
}
