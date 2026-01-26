using BibliotekaSzkolnaAI.Shared.Models.Params;
using BibliotekaSzkolnaAI.API.Models.Singles;

namespace BibliotekaSzkolnaAI.API.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<Book> ApplyBookFiltersAndSort(this IQueryable<Book> booksQuery, BookQueryParams query)
        {
            #region Search
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var normalizedSearchTerm = query.SearchTerm.ToLower().Trim();

                booksQuery = booksQuery.Where(b =>

                    b.Title.ToLower().Contains(normalizedSearchTerm) ||


                    b.Description.ToLower().Contains(normalizedSearchTerm) ||

                    (
                        b.BookAuthor.Name.ToLower().Contains(normalizedSearchTerm) ||
                        b.BookAuthor.Surname.ToLower().Contains(normalizedSearchTerm)
                    )
                );
            }
            #endregion

            #region Filters
            // tytuł
            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                var term = query.Title.Trim().ToLower();
                booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(term));
            }

            // isbn
            if (!string.IsNullOrWhiteSpace(query.Isbn))
            {
                var term = query.Isbn.Trim().Replace("-", "").ToLower();
                booksQuery = booksQuery.Where(b => b.Isbn.ToLower().Contains(term));
            }

            // rok wydania
            if (query.Year.HasValue)
            {
                booksQuery = booksQuery.Where(b => b.Year == query.Year);
            }

            // autor
            if (query.BookAuthors != null && query.BookAuthors.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookAuthor != null &&
                    (query.BookAuthors.Contains(b.BookAuthor.Surname) ||
                     query.BookAuthors.Contains(b.BookAuthor.Surname + ", " + b.BookAuthor.Name))
                );
            }

            // wydawca
            if (query.BookPublishers != null && query.BookPublishers.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookPublisher != null &&
                    query.BookPublishers.Contains(b.BookPublisher.Name)
                );
            }

            // seria
            if (query.BookSeries != null && query.BookSeries.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookSeries != null &&
                    query.BookSeries.Contains(b.BookSeries.Title)
                );
            }

            // kategoria
            if (query.BookCategories != null && query.BookCategories.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookCategory != null &&
                    query.BookCategories.Contains(b.BookCategory.Name)
                );
            }

            // typ
            if (query.BookTypes != null && query.BookTypes.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookType != null &&
                    query.BookTypes.Contains(b.BookType.Title)
                );
            }

            // gatunek
            if (query.BookGenres != null && query.BookGenres.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookBookGenres.Any(bg =>
                        query.BookGenres.Contains(bg.BookGenre.Title)
                    )
                );
            }

            // tagi
            if (query.BookSpecialTags != null && query.BookSpecialTags.Any())
            {
                booksQuery = booksQuery.Where(b =>
                    b.BookBookSpecialTags.Any(bg =>
                        query.BookSpecialTags.Contains(bg.BookSpecialTag.Title)
                    )
                );
            }
            #endregion

            #region Sorting
            bool descending = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);

            booksQuery = (query.SortBy?.ToLower()) switch
            {
                "title" => descending
                    ? booksQuery.OrderByDescending(b => b.Title)
                    : booksQuery.OrderBy(b => b.Title),

                "year" => descending
                    ? booksQuery.OrderByDescending(b => b.Year)
                    : booksQuery.OrderBy(b => b.Year),

                "bookauthor" => descending
                    ? booksQuery.OrderByDescending(b => b.BookAuthor!.Surname)
                    : booksQuery.OrderBy(b => b.BookAuthor!.Surname),

                "bookpublisher" => descending
                    ? booksQuery.OrderByDescending(b => b.BookPublisher!.Name)
                    : booksQuery.OrderBy(b => b.BookPublisher!.Name),

                "bookseries" => descending
                    ? booksQuery.OrderByDescending(b => b.BookSeries!.Title)
                    : booksQuery.OrderBy(b => b.BookSeries!.Title),

                "category" => descending
                    ? booksQuery.OrderByDescending(b => b.BookCategory!.Name)
                    : booksQuery.OrderBy(b => b.BookCategory!.Name),

                "type" => descending
                    ? booksQuery.OrderByDescending(b => b.BookType!.Title)
                    : booksQuery.OrderBy(b => b.BookType!.Title),

                _ => booksQuery.OrderBy(b => b.Title)
            };
            #endregion

            return booksQuery;
        }
    }
}
