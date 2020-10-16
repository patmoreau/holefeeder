using System;

namespace DrifterApps.Holefeeder.Application.Models
{
    public class AccountInfoViewModel
    {
        public Guid Id { get; }
        
        public string Name { get; }

        public AccountInfoViewModel(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
