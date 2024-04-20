using Microsoft.AspNetCore.Mvc.Rendering;

namespace Bookify.Web.Core.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //Category
            CreateMap<Category, CategoryViewModel>();
            CreateMap<Category, CategoryFormViewModel>().ReverseMap();
            CreateMap<Category, SelectListItem>()
                .ForMember(des => des.Value, op => op.MapFrom(src => src.Id))
                .ForMember(des => des.Text, op => op.MapFrom(src => src.Name));
            //Auther
            CreateMap<Author, AuthorViewModel>();
            CreateMap<Author, AuthorFormViewModel>().ReverseMap();
            CreateMap<Author, SelectListItem>()
                .ForMember(des => des.Value, op => op.MapFrom(src => src.Id))
                .ForMember(des => des.Text, op => op.MapFrom(src => src.Name));
            //Book
            CreateMap<BookFormViewModel,Book>()
                .ReverseMap()
                .ForMember(des=>des.Categories , op=>op.Ignore());
            CreateMap<Book,BookViewModel>()
                .ForMember(des=>des.Author,op=>op.MapFrom(src=>src.Author!.Name))
                .ForMember(des => des.Categories, op => op.MapFrom(src => src.Categories.Select(c=>c.Category!.Name).ToList()));

            //BookCopy
            CreateMap<BookCopy, BookCopyViewModel>()
               .ForMember(des => des.BookTitle, op => op.MapFrom(src => src.Book!.Title));
            CreateMap<BookCopy, BookCopyFormViewModel>()
                    .ReverseMap();

            //Users
            CreateMap<ApplicationUser, UserViewModel>();
            CreateMap<UserFormViewModel, ApplicationUser>()
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.UserName.ToUpper()))
                .ReverseMap();
        }
    }
}
