using System.ComponentModel.DataAnnotations;

namespace TestTableBinding.Models
{
    public class RowModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public bool Checked { get; set; }
    }
}
