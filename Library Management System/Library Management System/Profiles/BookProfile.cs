
using AutoMapper;
using Library_Management_System.DTOs;
using Library_Management_System.Models;

namespace Library_Management_System.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>()
                .ForMember(dest => dest.CategoryName,
                         opt => opt.MapFrom(src => src.Category.Name));

            CreateMap<BookCreateDto, Book>()
                .ForMember(dest => dest.AvailableCopies,
                         opt => opt.MapFrom(src => src.TotalCopies))
                .ForMember(dest => dest.AddedDate,
                         opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<BookUpdateDto, Book>()
                .ForMember(dest => dest.AvailableCopies,
                         opt => opt.Ignore())
                .ForMember(dest => dest.AddedDate,
                         opt => opt.Ignore());
        }
    }
}