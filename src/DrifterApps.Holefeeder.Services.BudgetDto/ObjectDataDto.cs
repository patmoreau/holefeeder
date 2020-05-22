using System.ComponentModel.DataAnnotations;

namespace DrifterApps.Holefeeder.Services.BudgetDto
{
    public class ObjectDataDto
    {
        public string Id { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
