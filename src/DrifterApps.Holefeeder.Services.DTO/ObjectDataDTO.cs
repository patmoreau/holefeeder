using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Services.DTO
{
    public class ObjectDataDTO
    {
        public string Id { get; }

        [Required]
        public string Code { get; }

        [Required]
        public string Data { get; }

        public ObjectDataDTO(string id, string code, string data)
        {
            Id = id;
            Code = code;
            Data = data;
        }
    }
}