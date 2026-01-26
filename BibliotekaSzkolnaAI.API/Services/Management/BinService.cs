using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.Bin;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class BinService(
        ApplicationDbContext context,
        IBookManagementService bookService,
        ICopyManagementService copyService,
        IUserManagementService userService) : IBinService
    {
        public async Task<List<DeletedItemDto>> GetAllDeletedItemsAsync()
        {
            var result = new List<DeletedItemDto>();

            var books = await context.Books
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(b => b.IsDeleted)
                .Select(b => new DeletedItemDto
                {
                    Id = b.Id.ToString(),
                    Type = "Książka",
                    DisplayName = b.Title,
                    Details = $"ISBN: {b.Isbn}, Autor: {b.BookAuthor.Surname}",
                    DeletedAt = b.Modified
                }).ToListAsync();
            result.AddRange(books);

            var copies = await context.BookCopies
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(c => c.IsDeleted)
                .Include(c => c.Book)
                .Select(c => new DeletedItemDto
                {
                    Id = c.Id.ToString(),
                    Type = "Egzemplarz",
                    DisplayName = c.Book.Title,
                    Details = $"Sygnatura: {c.Signature}, Nr inw: {c.InventoryNum}",
                    DeletedAt = c.Modified
                }).ToListAsync();
            result.AddRange(copies);

            var users = await context.Users
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Where(u => u.IsDeleted)
                .Select(u => new DeletedItemDto
                {
                    Id = u.Id,
                    Type = "Użytkownik",
                    DisplayName = $"{u.LastName} {u.FirstName}",
                    Details = $"Karta: {u.LibraryId}, PESEL: {u.Pesel}",
                    DeletedAt = u.DateModified
                }).ToListAsync();
            result.AddRange(users);

            return result.OrderByDescending(x => x.DeletedAt).ToList();
        }

        public async Task RestoreItemAsync(string id, string type)
        {
            switch (type)
            {
                case "Książka":
                    var book = await context.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == int.Parse(id));
                    if (book != null) { book.IsDeleted = false; book.IsVisible = false; }
                    break;

                case "Egzemplarz":
                    var copy = await context.BookCopies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == int.Parse(id));
                    if (copy != null)
                    {
                        var parentBook = await context.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == copy.BookId);
                        if (parentBook != null && parentBook.IsDeleted)
                            throw new InvalidOperationException("Nie można przywrócić egzemplarza, bo jego książka jest w koszu.");

                        copy.IsDeleted = false;
                        copy.Available = true;
                    }
                    break;

                case "Użytkownik":
                    var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
                    if (user != null)
                    {
                        user.IsDeleted = false;
                        user.LockoutEnd = null;
                    }
                    break;

                default:
                    throw new ArgumentException("Nieznany typ elementu.");
            }

            await context.SaveChangesAsync();
        }

        public async Task HardDeleteItemAsync(string id, string type)
        {
            switch (type)
            {
                case "Książka":
                    var book = await context.Books.IgnoreQueryFilters().FirstOrDefaultAsync(b => b.Id == int.Parse(id));
                    if (book != null)
                    {
                        if (book.BookCopies.Any())
                            throw new InvalidOperationException("Nie można usunąć książki, która ma egzemplarze. Usuń je najpierw.");

                        context.Books.Remove(book);
                    }
                    break;

                case "Egzemplarz":
                    var copy = await context.BookCopies.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == int.Parse(id));
                    if (copy != null)
                    {
                        context.BookCopies.Remove(copy);
                    }
                    break;

                case "Użytkownik":
                    var user = await context.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Id == id);
                    if (user != null)
                    {
                        context.Users.Remove(user);
                    }
                    break;

                default:
                    throw new ArgumentException("Nieznany typ elementu.");
            }

            await context.SaveChangesAsync();
        }
    }
}