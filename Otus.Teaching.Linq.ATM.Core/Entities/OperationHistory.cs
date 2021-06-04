using System;

namespace Otus.Teaching.Linq.ATM.Core.Entities
{
    public class OperationsHistory
    {
        public int Id { get; set; }
        public DateTime OperationDate { get; set; }
        public OperationType OperationType { get; set; }
        public decimal CashSum { get; set; }
        public int AccountId { get; set; }
    }
}