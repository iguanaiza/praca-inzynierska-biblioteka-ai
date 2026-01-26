using AutoMapper;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Catalog.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Books.Catalog;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Catalog
{
    public class BookCatalogService(IBookRepository bookRepository, IMapper mapper) : IBookCatalogService
    {
        public async Task<PagedResult<BookGetForCatalogDto>> GetCatalogBooksAsync(BookQueryParams query)
        {
            var (books, totalCount) = await bookRepository.GetBooksAsync(query, includeHidden: false);

            var dtos = mapper.Map<List<BookGetForCatalogDto>>(books);

            return new PagedResult<BookGetForCatalogDto>(dtos, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<Dictionary<string, List<BookGetForHomepageDto>>> GetHomepageBooksAsync()
        {
            var specialTags = new List<string> { "Polecane", "Nowości", "Popularne" };

            var books = await bookRepository.GetBooksByTagsAsync(specialTags);

            var result = new Dictionary<string, List<BookGetForHomepageDto>>();

            foreach (var tag in specialTags)
            {
                var booksForTag = books
                    .Where(b => b.BookBookSpecialTags.Any(t => t.BookSpecialTag.Title == tag))
                    .ToList();

                if (booksForTag.Any())
                {
                    result[tag] = mapper.Map<List<BookGetForHomepageDto>>(booksForTag);
                }
            }
            return result;
        }

        public async Task<List<BookGetForListDto>> GetSetBooksAsync()
        {
            // ID 1 = Lektura
            var books = await bookRepository.GetBooksByCategoryIdAsync(1);
            return mapper.Map<List<BookGetForListDto>>(books);
        }

        public async Task<List<BookGetForListDto>> GetTextBooksAsync()
        {
            // ID 2 = Podręcznik
            var books = await bookRepository.GetBooksByCategoryIdAsync(2);
            return mapper.Map<List<BookGetForListDto>>(books);
        }

        public async Task<BookGetDto?> GetBookDetailsAsync(int id)
        {
            var book = await bookRepository.GetByIdAsync(id);

            if (book == null || !book.IsVisible)
                return null;

            return mapper.Map<BookGetDto>(book);
        }

        public async Task<FilterOptionsDto> GetFilterOptionsAsync()
        {
            return await bookRepository.GetFilterOptionsAsync();
        }
    }
}
