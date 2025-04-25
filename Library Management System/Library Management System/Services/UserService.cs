using AutoMapper;
using Library_Management_System.Data;
using Library_Management_System.DTOs;
using Library_Management_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly LibraryContext _context;

        public UserService(UserManager<User> userManager, IMapper mapper, LibraryContext context)
        {
            _userManager = userManager;
            _mapper = mapper;
            _context = context;
        }

        public async Task<List<UserResponseDto>> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var userDto = _mapper.Map<UserResponseDto>(user);
                userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();
                userDtos.Add(userDto);
            }

            return userDtos;
        }

        public async Task<UserResponseDto> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            var userDto = _mapper.Map<UserResponseDto>(user);
            userDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return userDto;
        }

        public async Task<UserResponseDto> UpdateUser(string id, UserUpdateDto userDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception("User update failed.");

            // Update roles if provided
            if (userDto.Roles != null && userDto.Roles.Any())
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRolesAsync(user, userDto.Roles);
            }

            var updatedUserDto = _mapper.Map<UserResponseDto>(user);
            updatedUserDto.Roles = (await _userManager.GetRolesAsync(user)).ToList();

            return updatedUserDto;
        }

        public async Task DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            // Check if user has active borrowings
            var hasActiveBorrowings = await _context.Borrowings
                .AnyAsync(b => b.UserId == id && (b.Status == "Approved" || b.Status == "Pending"));

            if (hasActiveBorrowings)
                throw new Exception("Cannot delete user with active borrowings.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception("User deletion failed.");
        }

        public async Task ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new Exception("User not found.");

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
        }
    }

    public class UserUpdateDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public List<string> Roles { get; set; }
    }
}