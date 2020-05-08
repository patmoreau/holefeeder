namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class AccountInfoDto
    {
        public string Id { get; }
        
        public string Name { get; }
        
        public AccountInfoDto(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
