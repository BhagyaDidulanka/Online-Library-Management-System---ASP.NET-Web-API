using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        // Navigation properties
        public ICollection<Book> Books { get; set; }
    }
}