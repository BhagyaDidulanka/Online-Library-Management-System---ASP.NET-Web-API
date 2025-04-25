// BookDTOs.cs
namespace Library_Management_System.DTOs
{
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime AddedDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class BookCreateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public int TotalCopies { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CategoryId { get; set; }
    }

    public class BookUpdateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int TotalCopies { get; set; }
        public int CategoryId { get; set; }
    }
}