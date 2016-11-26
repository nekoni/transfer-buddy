namespace TransferBuddy.Worker.Models
{
    using System;
    
    public class Rate : Entity
    {
        public decimal Value { get; set; }

        public DateTime Date { get; set; }
    }
}