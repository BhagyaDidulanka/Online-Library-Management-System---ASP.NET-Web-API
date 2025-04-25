namespace Library_Management_System.Models
{
    public class Borrowing
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int BookId { get; set; }

        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        public DateTime? ReturnDate { get; set; }

        public DateTime DueDate { get; set; }

        public string Status { get; set; } // "Pending", "Approved", "Rejected", "Returned", "Overdue"

        // Navigation properties
        public User User { get; set; }
        public Book Book { get; set; }
    }
}