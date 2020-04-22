namespace DrifterApps.Holefeeder.Services.Dto
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
