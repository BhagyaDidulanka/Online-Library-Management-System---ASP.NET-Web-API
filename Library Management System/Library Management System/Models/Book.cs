using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        public string Description { get; set; }

        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        public DateTime PublishedDate { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public int CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public ICollection<Borrowing> Borrowings { get; set; }
    }
}