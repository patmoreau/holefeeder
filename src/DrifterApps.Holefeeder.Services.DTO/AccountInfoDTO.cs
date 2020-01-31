namespace DrifterApps.Holefeeder.Services.DTO
{
    public class AccountInfoDTO
    {
        public string Id { get; }
        
        public string Name { get; }
        
        public AccountInfoDTO(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}