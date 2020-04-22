using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class ObjectDataDto
    {
        public string Id { get; }

        [Required]
        public string Code { get; }

        [Required]
        public string Data { get; }

        public ObjectDataDto(string id, string code, string data)
        {
            Id = id;
            Code = code;
            Data = data;
        }
    }
}
