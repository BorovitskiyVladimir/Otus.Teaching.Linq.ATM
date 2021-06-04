using System;

namespace Otus.Teaching.Linq.ATM.Core.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public DateTime OpeningDate { get; set; }
        public decimal CashAll { get; set; }
        public int UserId { get; set; }
    }
}