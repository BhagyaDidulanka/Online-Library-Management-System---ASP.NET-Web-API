using Library_Management_System.DTOs;
using Library_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Library_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BorrowingsController : ControllerBase
    {
        private readonly BorrowingService _borrowingService;

        public BorrowingsController(BorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBorrowings()
        {
            var borrowings = await _borrowingService.GetAllBorrowings();
            return Ok(borrowings);
        }

        [HttpGet("my-borrowings")]
        public async Task<IActionResult> GetUserBorrowings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowings = await _borrowingService.GetUserBorrowings(userId);
            return Ok(borrowings);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBorrowing([FromBody] BorrowingCreateDto borrowingDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowing = await _borrowingService.CreateBorrowing(userId, borrowingDto);
            return CreatedAtAction(nameof(GetUserBorrowings), borrowing);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBorrowingStatus(int id, [FromBody] BorrowingUpdateDto borrowingDto)
        {
            var borrowing = await _borrowingService.UpdateBorrowingStatus(id, borrowingDto);
            return Ok(borrowing);
        }

        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnBook(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _borrowingService.ReturnBook(id, userId);
            return NoContent();
        }
    }
}