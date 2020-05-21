using System;
using System.Collections.Generic;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class UpcomingDto
    {
        public string Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public decimal Amount { get; set; }
        
        public string Description { get; set; }

        public IList<string> Tags { get; set; }
        
        public CategoryInfoDto Category { get; set; }
        
        public AccountInfoDto Account { get; set; }
    }
}
