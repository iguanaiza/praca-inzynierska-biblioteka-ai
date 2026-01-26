using AutoMapper;
using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Favorites;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Loans;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Reservation;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Users;

namespace BibliotekaSzkolnaAI.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // --- KSIĄŻKI ---
            CreateMap<Book, BookGetBaseDto>()
                .ForMember(d => d.BookAuthor, o => o.MapFrom(s => FormatAuthor(s.BookAuthor)))
                .ForMember(d => d.BookPublisher, o => o.MapFrom(s => s.BookPublisher.Name))
                .ForMember(d => d.AvailableCopyCount, o => o.MapFrom(s => s.AvailableCopyCount));

            CreateMap<Book, BookGetForListDto>()
                .IncludeBase<Book, BookGetBaseDto>();

            CreateMap<Book, BookGetForHomepageDto>()
                .IncludeBase<Book, BookGetBaseDto>();

            CreateMap<Book, BookGetForCatalogDto>()
                .IncludeBase<Book, BookGetForListDto>() 
                .ForMember(d => d.BookCategory, o => o.MapFrom(s => s.BookCategory.Name))
                .ForMember(d => d.BookType, o => o.MapFrom(s => s.BookType.Title))
                .ForMember(d => d.BookSeries, o => o.MapFrom(s => s.BookSeries != null ? s.BookSeries.Title : null))
                .ForMember(d => d.BookGenres, o => o.MapFrom(s => s.BookBookGenres.Select(x => x.BookGenre.Title).ToList()))
                .ForMember(d => d.BookSpecialTags, o => o.MapFrom(s => s.BookBookSpecialTags.Select(x => x.BookSpecialTag.Title).ToList()))
                .ForMember(d => d.FavoriteCount, o => o.MapFrom(s => s.FavoriteCount))
                .ForMember(d => d.IsFavorite, o => o.MapFrom(s => false));

            CreateMap<Book, BookGetDto>()
                .IncludeBase<Book, BookGetForCatalogDto>()
                .ForMember(d => d.CopyCount, o => o.MapFrom(s => s.CopyCount))
                .ForMember(d => d.BookCopies, o => o.MapFrom(s => s.BookCopies));

            CreateMap<Book, BookGetForListDetailedDto>()
                .IncludeBase<Book, BookGetBaseDto>()
                .ForMember(d => d.BookSeries, o => o.MapFrom(s => s.BookSeries != null ? s.BookSeries.Title : null))
                .ForMember(d => d.CopyCount, o => o.MapFrom(s => s.CopyCount));

            CreateMap<Book, BookGetDetailedDto>()
                .IncludeBase<Book, BookGetDto>()
                .ForMember(d => d.BookGenreIds, o => o.MapFrom(s => s.BookBookGenres.Select(x => x.BookGenreId).ToList()))
                .ForMember(d => d.BookSpecialTagIds, o => o.MapFrom(s => s.BookBookSpecialTags.Select(x => x.BookSpecialTagId).ToList()));

            CreateMap<BookUpsertDto, Book>()
                .ForMember(d => d.BookBookGenres, o => o.Ignore())
                .ForMember(d => d.BookBookSpecialTags, o => o.Ignore())
                .IncludeAllDerived();

            CreateMap<BookCreateDto, Book>();
            CreateMap<BookEditDto, Book>();

            // --- KOSZYK ---
            CreateMap<BookReservationCart, ReservationGetDto>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.BookCopy.Book.Title))
                .ForMember(d => d.Author, o => o.MapFrom(s => $"{s.BookCopy.Book.BookAuthor.Surname}, {s.BookCopy.Book.BookAuthor.Name}"))
                .ForMember(d => d.CoverImageUrl, o => o.MapFrom(s => s.BookCopy.Book.ImageUrl))
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.BookCopy.BookId));

            // --- ULUBIONE ---
            CreateMap<FavoriteBook, FavoriteBookGetDto>()
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Book.Title))
                .ForMember(d => d.Author, o => o.MapFrom(s => $"{s.Book.BookAuthor.Surname}, {s.Book.BookAuthor.Name}"))
                .ForMember(d => d.Description, o => o.MapFrom(s => s.Book.Description))
                .ForMember(d => d.ImageUrl, o => o.MapFrom(s => s.Book.ImageUrl));

            // --- WYPOŻYCZENIA ---
            CreateMap<BookLoan, LoanGetDto>()
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.BookCopy.BookId))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.BookCopy.Book.Title))
                .ForMember(d => d.Author, o => o.MapFrom(s => $"{s.BookCopy.Book.BookAuthor.Surname}, {s.BookCopy.Book.BookAuthor.Name}"))
                .ForMember(d => d.BookPublisher, o => o.MapFrom(s => s.BookCopy.Book.BookPublisher.Name))
                .ForMember(d => d.BookPublicationYear, o => o.MapFrom(s => s.BookCopy.Book.Year));

            CreateMap<BookLoan, LoanManagementDto>()
                .IncludeBase<BookLoan, LoanGetDto>()
                .ForMember(d => d.UserFullName, o => o.MapFrom(s => s.User != null ? $"{s.User.LastName}, {s.User.FirstName}" : "Brak danych"))
                .ForMember(d => d.UserLibraryId, o => o.MapFrom(s => s.User != null ? s.User.LibraryId : 0))
                .ForMember(d => d.BookSignature, o => o.MapFrom(s => s.BookCopy.Signature));

            // --- USERS ---
            CreateMap<ApplicationUser, UserForListDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.LastName}, {s.FirstName}"))
                .ForMember(d => d.ActiveLoanCount, o => o.MapFrom(s =>
                    s.BookLoans != null
                    ? s.BookLoans.Count(l => l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue)
                    : 0));

            CreateMap<ApplicationUser, UserDetailedDto>()
                .IncludeBase<ApplicationUser, UserForListDto>()
                .ForMember(d => d.FullName, o => o.MapFrom(s => $"{s.FirstName} {s.LastName}"))
                .ForMember(d => d.Loans, o => o.MapFrom(s => s.BookLoans));

            CreateMap<UserEditDto, ApplicationUser>()
                .ForMember(d => d.Email, o => o.Ignore())
                .ForMember(d => d.UserName, o => o.Ignore());

            CreateMap<UserCreateDto, ApplicationUser>()
                .IncludeBase<UserEditDto, ApplicationUser>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.Email))
                .ForMember(d => d.DateAdded, o => o.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.FineAmount, o => o.Ignore())
                .ForMember(d => d.LibraryId, o => o.Ignore());

            // --- EGZEMPLARZE --
            CreateMap<BookCopy, CopyGetDetailedDto>()
                .ForMember(d => d.BookId, o => o.MapFrom(s => s.Book.Id))
                .ForMember(d => d.BookIsbn, o => o.MapFrom(s => s.Book.Isbn))
                .ForMember(d => d.BookTitle, o => o.MapFrom(s => s.Book.Title))
                .ForMember(d => d.AuthorName, o => o.MapFrom(s => $"{s.Book.BookAuthor.Surname}, {s.Book.BookAuthor.Name}"))
                .ForMember(d => d.PublisherName, o => o.MapFrom(s => $"{s.Book.BookPublisher.Name}"))
                .ForMember(d => d.YearOfPublication, o => o.MapFrom(s => $"{s.Book.Year}"));

            CreateMap<CopyCreateDto, BookCopy>()
                .ForMember(d => d.Available, o => o.MapFrom(s => true))
                .ForMember(d => d.Created, o => o.MapFrom(s => DateTime.UtcNow))
                .ForMember(d => d.IsDeleted, o => o.MapFrom(s => false));

            CreateMap<CopyEditDto, BookCopy>()
                .ForMember(d => d.Modified, o => o.MapFrom(s => DateTime.UtcNow));
        }

        private static string FormatAuthor(BookAuthor author)
        {
            return author != null ? $"{author.Surname}, {author.Name}" : string.Empty;
        }
    }
}
