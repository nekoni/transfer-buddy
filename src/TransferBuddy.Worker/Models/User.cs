namespace TransferBuddy.Worker.Models
{
    using System;
    
    public class User : Entity
    {
        public string UserId { get; set; }

        public string FirstName { get; set; }

        public DateTime LastActivity { get; set; }
    } 
}