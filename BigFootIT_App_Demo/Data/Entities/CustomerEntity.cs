using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BigFootIT_App_Demo.Data.Entities
{
    public class CustomerEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [StringLength(128)]
        public string? CustomerName { get; set; } = string.Empty;

        [StringLength(1024)]
        public string? CustomerAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(36)]
        public string UserId { get; set; } = Guid.NewGuid().ToString();
    }
}
