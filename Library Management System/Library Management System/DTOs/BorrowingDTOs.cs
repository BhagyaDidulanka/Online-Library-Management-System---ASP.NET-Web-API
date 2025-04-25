namespace Library_Management_System.DTOs
{
    public class BorrowingDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }

    public class BorrowingCreateDto
    {
        public int BookId { get; set; }
        public int DaysToBorrow { get; set; } = 14; // Default 2 weeks
    }

    public class BorrowingUpdateDto
    {
        public string Status { get; set; } // For admin to approve/reject
    }
}