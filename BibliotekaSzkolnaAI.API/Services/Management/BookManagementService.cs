using AutoMapper;
using BibliotekaSzkolnaAI.API.Models.Relations;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Management;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class BookManagementService(IBookRepository bookRepository, IMapper mapper) : IBookManagementService
    {
        public async Task<PagedResult<BookGetForListDetailedDto>> GetDetailedBooksAsync(BookQueryParams query)
        {
            var (books, totalCount) = await bookRepository.GetBooksAsync(query, includeHidden: true);
            var dtos = mapper.Map<List<BookGetForListDetailedDto>>(books);
            return new PagedResult<BookGetForListDetailedDto>(dtos, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<BookGetDetailedDto?> GetDetailedBookByIdAsync(int id)
        {
            var book = await bookRepository.GetByIdAsync(id);
            return book == null ? null : mapper.Map<BookGetDetailedDto>(book);
        }

        public async Task<List<BookGetForListDetailedDto>> GetDeletedBooksAsync()
        {
            return new List<BookGetForListDetailedDto>();
        }

        public async Task<BookLookupDto?> LookupBookByIsbnAsync(string isbn)
        {
            var book = await bookRepository.GetBookByIsbnAsync(isbn);

            if (book == null) return null;

            return new BookLookupDto
            {
                Id = book.Id,
                Title = book.Title,
                AuthorName = book.BookAuthor.Name + " " + book.BookAuthor.Surname,
                PublisherName = book.BookPublisher.Name,
                Year = book.Year
            };
        }

        public async Task<int> CreateBookAsync(BookUpsertDto dto)
        {
            var book = mapper.Map<Book>(dto);

            book.IsVisible = true;
            book.IsDeleted = false;
            book.Created = DateTime.Now;

            if (dto.BookGenreIds != null)
            {
                book.BookBookGenres = dto.BookGenreIds
                    .Select(id => new BookBookGenre { BookGenreId = id }).ToList();
            }

            if (dto.BookSpecialTagIds != null)
            {
                book.BookBookSpecialTags = dto.BookSpecialTagIds
                    .Select(id => new BookBookSpecialTag { BookSpecialTagId = id }).ToList();
            }

            await bookRepository.AddAsync(book);
            await bookRepository.SaveChangesAsync();

            return book.Id;
        }

        public async Task UpdateBookAsync(int id, BookUpsertDto dto)
        {
            var book = await bookRepository.GetByIdForEditAsync(id);
            if (book == null) throw new KeyNotFoundException($"Nie znaleziono książki o ID {id}");

            mapper.Map(dto, book);

            if (dto is BookEditDto editDto)
            {
                book.IsVisible = editDto.IsVisible;
            }

            book.Modified = DateTime.Now;

            book.BookBookGenres.Clear();
            if (dto.BookGenreIds != null)
            {
                foreach (var genreId in dto.BookGenreIds)
                {
                    book.BookBookGenres.Add(new BookBookGenre { BookId = id, BookGenreId = genreId });
                }
            }

            book.BookBookSpecialTags.Clear();
            if (dto.BookSpecialTagIds != null)
            {
                foreach (var tagId in dto.BookSpecialTagIds)
                {
                    book.BookBookSpecialTags.Add(new BookBookSpecialTag { BookId = id, BookSpecialTagId = tagId });
                }
            }

            await bookRepository.SaveChangesAsync();
        }
        public async Task SoftDeleteBookAsync(int id)
        {
            var book = await bookRepository.GetByIdForEditAsync(id);
            if (book == null) throw new KeyNotFoundException("Nie znaleziono książki");

            if (book.IsDeleted) throw new InvalidOperationException("Książka jest już w koszu.");

            book.IsDeleted = true;
            book.IsVisible = false;
            book.Modified = DateTime.Now;

            await bookRepository.SaveChangesAsync();
        }

        public async Task HardDeleteBookAsync(int id)
        {
            var book = await bookRepository.GetByIdForEditAsync(id);
            if (book == null) throw new KeyNotFoundException("Nie znaleziono książki");

            if (!book.IsDeleted)
            {
                throw new InvalidOperationException("Książka musi najpierw trafić do kosza.");
            }

            if (book.BookCopies != null && book.BookCopies.Count > 0)
            {
                throw new InvalidOperationException("Nie można usunąć książki, która ma egzemplarze.");
            }

            bookRepository.Delete(book);
            await bookRepository.SaveChangesAsync();
        }

        public async Task<BookLookupsDto> GetLookupsAsync()
        {
            return await bookRepository.GetBookLookupsAsync();
        }
    }
}
