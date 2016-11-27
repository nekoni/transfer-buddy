using System;
using System.ComponentModel.DataAnnotations;

namespace TransferBuddy.Models
{
    public class TransferConfig : Entity
    {
        [Display(Name = "Description")]  
        public string Description { get; set; }

        public string FacebookId { get; set; }

        [Display(Name = "Source Currency")]  
        public string Source { get; set; }

        [Display(Name = "Target Currency")] 
        public string Target { get; set; }

        [Display(Name = "Frequency (W or M)")] 
        public string Frequency { get; set; }

        [Display(Name = "Amount")] 
        public string Amount { get; set; }
    }
}