using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigFootIT_App_Demo.Data.Entities
{
    public class InteractionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime DateOn { get; set; }

        [Required]
        [StringLength(36)]
        public string UserId { get; set; } = Guid.NewGuid().ToString();

        public string? CustomerAddress { get; set; } = string.Empty;
        public Decimal DealValue { get; set; } = 0;

    }
}
