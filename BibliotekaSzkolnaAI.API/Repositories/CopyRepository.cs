using BibliotekaSzkolnaAI.API.Data;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.Shared.Models.Params;
using Microsoft.EntityFrameworkCore;

namespace BibliotekaSzkolnaAI.API.Repositories
{
    public class CopyRepository(ApplicationDbContext context) : ICopyRepository
    {
        public async Task<(List<BookCopy> Items, int TotalCount)> GetCopiesAsync(CopyQueryParams query, bool includeDeleted)
        {
            var dbQuery = context.BookCopies
                .AsNoTracking()
                .Include(c => c.Book).ThenInclude(b => b.BookAuthor)
                .Include(c => c.Book).ThenInclude(b => b.BookPublisher)
                .AsQueryable();

            if (!includeDeleted)
            {
                dbQuery = dbQuery.Where(c => !c.IsDeleted);
            }
            else
            {
                dbQuery = dbQuery.Where(c => c.IsDeleted);
            }

            if (!string.IsNullOrWhiteSpace(query.Signature))
                dbQuery = dbQuery.Where(c => c.Signature.Contains(query.Signature));

            if (query.InventoryNum.HasValue)
                dbQuery = dbQuery.Where(c => c.InventoryNum == query.InventoryNum.Value);

            if (query.Available.HasValue)
                dbQuery = dbQuery.Where(c => c.Available == query.Available.Value);

            if (!string.IsNullOrWhiteSpace(query.BookTitle))
                dbQuery = dbQuery.Where(c => c.Book.Title.Contains(query.BookTitle));

            bool descending = string.Equals(query.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
            dbQuery = (query.SortBy?.ToLower()) switch
            {
                "signature" => descending ? dbQuery.OrderByDescending(c => c.Signature) : dbQuery.OrderBy(c => c.Signature),
                "inventorynum" => descending ? dbQuery.OrderByDescending(c => c.InventoryNum) : dbQuery.OrderBy(c => c.InventoryNum),
                _ => dbQuery.OrderBy(c => c.Signature)
            };

            var totalCount = await dbQuery.CountAsync();
            var items = await dbQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<BookCopy?> GetByIdAsync(int id)
        {
            return await context.BookCopies
                .Include(c => c.Book).ThenInclude(b => b.BookAuthor)
                .Include(c => c.Book).ThenInclude(b => b.BookPublisher)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<int> GetMaxInventoryNumberAsync()
        {
            return await context.BookCopies
                .MaxAsync(c => (int?)c.InventoryNum) ?? 0;
        }

        public async Task<bool> ExistsAsync(string signature, int inventoryNum, int? excludeId = null)
        {
            var query = context.BookCopies.AsQueryable();
            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync(c => c.Signature == signature || c.InventoryNum == inventoryNum);
        }

        public async Task AddAsync(BookCopy copy)
        {
            await context.BookCopies.AddAsync(copy);
        }

        public void Delete(BookCopy copy)
        {
            context.BookCopies.Remove(copy);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
