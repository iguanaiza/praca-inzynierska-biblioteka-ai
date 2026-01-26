using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Extensions;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Dictionaries;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.EntityFrameworkCore;
using FuzzySharp;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _context;

        public BookRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(List<Book> Items, int TotalCount)> GetBooksAsync(BookQueryParams query, bool includeHidden = false)
        {
            var dbQuery = _context.Books
                .AsNoTracking()
                .AsSplitQuery()
                .Where(b => !b.IsDeleted);

            if (!includeHidden)
            {
                dbQuery = dbQuery.Where(b => b.IsVisible);
            }
            else
            {
                if (query.IsVisible.HasValue)
                {
                    dbQuery = dbQuery.Where(b => b.IsVisible == query.IsVisible.Value);
                }
            }

            if (string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                dbQuery = AddIncludes(dbQuery);
                dbQuery = dbQuery.ApplyBookFiltersAndSort(query);

                var totalCount = await dbQuery.CountAsync();
                var items = await dbQuery
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync();

                return (items, totalCount);
            }

            else
            {
                var lightData = await dbQuery
                    .Select(b => new
                    {
                        b.Id,
                        b.Title,
                        b.Year, 
                        AuthorName = b.BookAuthor.Surname + ", " + b.BookAuthor.Name
                    })
                    .ToListAsync();

                var searchPattern = query.SearchTerm.ToLower();

                var scoredQuery = lightData
                    .Select(x => new
                    {
                        x.Id,
                        x.Title,
                        x.AuthorName,
                        x.Year,
                        Score = Math.Max(
                            Fuzz.PartialRatio(searchPattern, x.Title.ToLower()),
                            Fuzz.PartialRatio(searchPattern, x.AuthorName.ToLower())
                        )
                    })
                    .Where(x => x.Score > 65); 


                IEnumerable<dynamic> sortedResults; 

                if (string.IsNullOrEmpty(query.SortBy))
                {
                    sortedResults = scoredQuery.OrderByDescending(x => x.Score);
                }
                else
                {
                    bool isDesc = query.SortOrder?.ToLower() == "desc" || query.SortOrder?.ToLower() == "year_desc";

                    sortedResults = query.SortBy.ToLower() switch
                    {
                        "title" => isDesc
                            ? scoredQuery.OrderByDescending(x => x.Title)
                            : scoredQuery.OrderBy(x => x.Title),

                        "bookauthor" => isDesc
                            ? scoredQuery.OrderByDescending(x => x.AuthorName)
                            : scoredQuery.OrderBy(x => x.AuthorName),

                        "year" => isDesc
                            ? scoredQuery.OrderByDescending(x => x.Year)
                            : scoredQuery.OrderBy(x => x.Year),

                        _ => scoredQuery.OrderByDescending(x => x.Score)
                    };
                }

                var scoredResultsList = sortedResults.ToList();
                var totalCount = scoredResultsList.Count;

                var pagedIds = scoredResultsList
                    .Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(x => x.Id) 
                    .ToList();

                if (!pagedIds.Any())
                {
                    return (new List<Book>(), 0);
                }

                var finalQuery = _context.Books
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Where(b => pagedIds.Contains(b.Id));

                finalQuery = AddIncludes(finalQuery);

                var finalItems = await finalQuery.ToListAsync();

                var sortedFinalItems = finalItems
                    .OrderBy(item => pagedIds.IndexOf(item.Id))
                    .ToList();

                return (sortedFinalItems, totalCount);
            }
        }

        public async Task<List<Book>> GetBooksByTagsAsync(IEnumerable<string> tags)
        {
            return await _context.Books.AsNoTracking()
                .Where(b => !b.IsDeleted && b.IsVisible)
                .Where(b => b.BookBookSpecialTags.Any(t => tags.Contains(t.BookSpecialTag.Title)))
                .Include(b => b.BookAuthor)
                .Include(b => b.BookBookSpecialTags).ThenInclude(t => t.BookSpecialTag)
                .ToListAsync();
        }

        public async Task<List<Book>> GetBooksByCategoryIdAsync(int categoryId)
        {
            return await _context.Books.AsNoTracking()
                .Where(b => !b.IsDeleted && b.IsVisible && b.BookCategoryId == categoryId)
                .Include(b => b.BookCategory)
                .Include(b => b.BookAuthor)
                .Include(b => b.BookPublisher)
                .Include(b => b.BookCopies)
                .OrderBy(b => b.Class)
                    .ThenBy(b => b.BookAuthor.Surname)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books.AsNoTracking()
                .Include(b => b.BookAuthor)
                .Include(b => b.BookPublisher)
                .Include(b => b.BookSeries)
                .Include(b => b.BookCategory)
                .Include(b => b.BookType)
                .Include(b => b.BookBookGenres).ThenInclude(bg => bg.BookGenre)
                .Include(b => b.BookBookSpecialTags).ThenInclude(bt => bt.BookSpecialTag)
                .Include(b => b.BookCopies)
                .Include(b => b.FavoriteByUsers)
                .FirstOrDefaultAsync(b => b.Id == id && !b.IsDeleted);
        }

        public async Task<Book?> GetBookByIsbnAsync(string isbn)
        {
            return await _context.Books
                .Include(b => b.BookAuthor)
                .Include(b => b.BookPublisher)
                .FirstOrDefaultAsync(b => b.Isbn == isbn);
        }

        public async Task<Book?> GetByIdForEditAsync(int id)
        {
            return await _context.Books
                .Include(b => b.BookBookGenres)
                .Include(b => b.BookBookSpecialTags)
                .Include(b => b.BookCopies)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BookLookupsDto> GetBookLookupsAsync()
        {
            return new BookLookupsDto
            {
                Authors = await _context.BookAuthors.AsNoTracking()
                    .OrderBy(a => a.Surname)
                    .ThenBy(a => a.Name)
                    .Select(x => new LookupDto
                    {
                        Id = x.Id,
                        Name = x.Surname + ", " + x.Name
                    })
                    .ToListAsync(),

                Publishers = await _context.BookPublishers.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                Series = await _context.BookSeries.AsNoTracking()
                    .OrderBy(x => x.Title)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Title })
                    .ToListAsync(),

                Types = await _context.BookTypes.AsNoTracking()
                    .OrderBy(x => x.Title)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Title })
                    .ToListAsync(),

                Categories = await _context.BookCategories.AsNoTracking()
                    .OrderBy(x => x.Name)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Name })
                    .ToListAsync(),

                Genres = await _context.BookGenres.AsNoTracking()
                    .OrderBy(x => x.Title)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Title })
                    .ToListAsync(),

                SpecialTags = await _context.BookSpecialTags.AsNoTracking()
                    .OrderBy(x => x.Title)
                    .Select(x => new LookupDto { Id = x.Id, Name = x.Title })
                    .ToListAsync()
            };
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Delete(Book book)
        {
            _context.Books.Remove(book);
        }

        public async Task<FilterOptionsDto> GetFilterOptionsAsync()
        {
            var genres = await _context.BookGenres
                .AsNoTracking()
                .OrderBy(g => g.Title)
                .Select(g => g.Title)
                .ToListAsync();

            var categories = await _context.BookCategories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => c.Name)
                .ToListAsync();

            var types = await _context.BookTypes
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .Select(t => t.Title)
                .ToListAsync();

            var authors = await _context.Books
                .AsNoTracking()
                .Include(b => b.BookAuthor)
                .Where(b => b.BookAuthor != null)
                .Select(b => b.BookAuthor.Surname + ", " + b.BookAuthor.Name)
                .Distinct()
                .OrderBy(a => a)
                .ToListAsync();

            var publishers = await _context.Books
                .AsNoTracking()
                .Include(b => b.BookPublisher)
                .Where(b => b.BookPublisher != null)
                .Select(b => b.BookPublisher.Name)
                .Distinct()
                .OrderBy(p => p)
                .ToListAsync();

            var series = await _context.Books
                .AsNoTracking()
                .Include(b => b.BookSeries)
                .Where(b => b.BookSeries != null && !string.IsNullOrEmpty(b.BookSeries.Title) && b.BookSeries.Title != "--brak--")
                .Select(b => b.BookSeries.Title)
                .Distinct()
                .OrderBy(s => s)
                .ToListAsync();

            return new FilterOptionsDto
            {
                Genres = genres,
                Categories = categories,
                Types = types,
                Authors = authors,
                Publishers = publishers,
                Series = series
            };
        }

        private IQueryable<Book> AddIncludes(IQueryable<Book> query)
        {
            return query
                .Include(b => b.BookAuthor)
                .Include(b => b.BookPublisher)
                .Include(b => b.BookCategory)
                .Include(b => b.BookType)
                .Include(b => b.BookSeries)
                .Include(b => b.BookBookGenres).ThenInclude(bg => bg.BookGenre)
                .Include(b => b.BookBookSpecialTags).ThenInclude(bt => bt.BookSpecialTag)
                .Include(b => b.BookCopies)
                .Include(b => b.FavoriteByUsers);
        }

    }
}
