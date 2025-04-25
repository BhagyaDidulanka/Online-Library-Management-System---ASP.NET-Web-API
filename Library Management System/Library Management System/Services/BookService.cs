// Services/BookService.cs
using AutoMapper;
using Library_Management_System.Data;
using Library_Management_System.DTOs;
using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services
{
    public class BookService
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public BookService(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<BookDto>> GetAllBooks()
        {
            var books = await _context.Books
                .Include(b => b.Category)
                .ToListAsync();

            return _mapper.Map<List<BookDto>>(books);
        }

        public async Task<BookDto> GetBookById(int id)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                throw new Exception("Book not found.");

            return _mapper.Map<BookDto>(book);
        }

        public async Task<BookDto> CreateBook(BookCreateDto bookDto)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == bookDto.CategoryId);
            if (!categoryExists)
                throw new Exception("Category not found.");

            var book = _mapper.Map<Book>(bookDto);

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return await GetBookById(book.Id);
        }

        public async Task<BookDto> UpdateBook(int id, BookUpdateDto bookDto)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new Exception("Book not found.");

            // Calculate difference in copies
            int copiesDifference = bookDto.TotalCopies - book.TotalCopies;

            _mapper.Map(bookDto, book);

            // Update available copies
            book.AvailableCopies += copiesDifference;
            if (book.AvailableCopies < 0)
                book.AvailableCopies = 0;

            await _context.SaveChangesAsync();
            return await GetBookById(book.Id);
        }

        public async Task DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                throw new Exception("Book not found.");

            var hasActiveBorrowings = await _context.Borrowings
                .AnyAsync(b => b.BookId == id && (b.Status == "Approved" || b.Status == "Pending"));

            if (hasActiveBorrowings)
                throw new Exception("Cannot delete book with active borrowings.");

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<BookDto>> SearchBooks(string searchTerm)
        {
            var books = await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Title.Contains(searchTerm) ||
                           b.Author.Contains(searchTerm) ||
                           b.ISBN.Contains(searchTerm) ||
                           b.Category.Name.Contains(searchTerm))
                .ToListAsync();

            return _mapper.Map<List<BookDto>>(books);
        }
    }
}