using AutoMapper;
using Library_Management_System.Data;
using Library_Management_System.DTOs;
using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services
{
    public class BorrowingService
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;

        public BorrowingService(LibraryContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BorrowingDto> CreateBorrowing(string userId, BorrowingCreateDto borrowingDto)
        {
            var book = await _context.Books.FindAsync(borrowingDto.BookId);
            if (book == null)
                throw new Exception("Book not found.");

            if (book.AvailableCopies <= 0)
                throw new Exception("No available copies of this book.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var borrowing = new Borrowing
            {
                UserId = userId,
                BookId = borrowingDto.BookId,
                BorrowDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(borrowingDto.DaysToBorrow),
                Status = "Pending"
            };

            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            return _mapper.Map<BorrowingDto>(borrowing);
        }

        public async Task<BorrowingDto> UpdateBorrowingStatus(int id, BorrowingUpdateDto borrowingDto)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrowing == null)
                throw new Exception("Borrowing record not found.");

            if (borrowing.Status == "Returned")
                throw new Exception("Cannot modify a returned borrowing.");

            // If changing from Pending to Approved, decrease available copies
            if (borrowing.Status == "Pending" && borrowingDto.Status == "Approved")
            {
                borrowing.Book.AvailableCopies--;
                if (borrowing.Book.AvailableCopies < 0)
                    throw new Exception("No available copies left.");
            }
            // If changing from Approved to Rejected, increase available copies
            else if (borrowing.Status == "Approved" && (borrowingDto.Status == "Rejected" || borrowingDto.Status == "Returned"))
            {
                borrowing.Book.AvailableCopies++;
            }

            borrowing.Status = borrowingDto.Status;

            _context.Borrowings.Update(borrowing);
            await _context.SaveChangesAsync();

            return _mapper.Map<BorrowingDto>(borrowing);
        }

        public async Task<List<BorrowingDto>> GetUserBorrowings(string userId)
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return _mapper.Map<List<BorrowingDto>>(borrowings);
        }

        public async Task<List<BorrowingDto>> GetAllBorrowings()
        {
            var borrowings = await _context.Borrowings
                .Include(b => b.Book)
                .Include(b => b.User)
                .ToListAsync();

            return _mapper.Map<List<BorrowingDto>>(borrowings);
        }

        public async Task ReturnBook(int borrowingId, string userId)
        {
            var borrowing = await _context.Borrowings
                .Include(b => b.Book)
                .FirstOrDefaultAsync(b => b.Id == borrowingId && b.UserId == userId);

            if (borrowing == null)
                throw new Exception("Borrowing record not found.");

            if (borrowing.Status != "Approved")
                throw new Exception("Only approved borrowings can be returned.");

            borrowing.Status = "Returned";
            borrowing.ReturnDate = DateTime.UtcNow;
            borrowing.Book.AvailableCopies++;

            _context.Borrowings.Update(borrowing);
            await _context.SaveChangesAsync();
        }
    }
}