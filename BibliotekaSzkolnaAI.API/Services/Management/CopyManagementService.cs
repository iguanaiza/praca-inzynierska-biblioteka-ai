using AutoMapper;
using BibliotekaSzkolnaAI.API.Models.Singles;
using BibliotekaSzkolnaAI.API.Repositories.Interfaces;
using BibliotekaSzkolnaAI.API.Services.Management.Interfaces;
using BibliotekaSzkolnaAI.Shared.Common;
using BibliotekaSzkolnaAI.Shared.DataTransferObjects.BookCopies;
using BibliotekaSzkolnaAI.Shared.Models.Params;

namespace BibliotekaSzkolnaAI.API.Services.Management
{
    public class CopyManagementService(ICopyRepository copyRepo, IMapper mapper) : ICopyManagementService
    {
        public async Task<PagedResult<CopyGetDetailedDto>> GetCopiesAsync(CopyQueryParams query)
        {
            var (items, totalCount) = await copyRepo.GetCopiesAsync(query, includeDeleted: false);
            var dtos = mapper.Map<List<CopyGetDetailedDto>>(items);
            return new PagedResult<CopyGetDetailedDto>(dtos, totalCount, query.PageNumber, query.PageSize);
        }

        public async Task<List<CopyGetDetailedDto>> GetDeletedCopiesAsync()
        {

            var query = new CopyQueryParams { PageSize = 1000 };
            var (items, _) = await copyRepo.GetCopiesAsync(query, includeDeleted: true);
            return mapper.Map<List<CopyGetDetailedDto>>(items);
        }

        public async Task<CopyGetDetailedDto?> GetCopyByIdAsync(int id)
        {
            var copy = await copyRepo.GetByIdAsync(id);
            return copy == null ? null : mapper.Map<CopyGetDetailedDto>(copy);
        }

        public async Task<int> GetNextInventoryNumberAsync()
        {
            var max = await copyRepo.GetMaxInventoryNumberAsync();
            return max + 1;
        }

        public async Task<CopyGetDetailedDto> CreateCopyAsync(CopyCreateDto dto)
        {
            if (await copyRepo.ExistsAsync(dto.Signature, dto.InventoryNum))
            {
                throw new InvalidOperationException("Egzemplarz o takiej sygnaturze lub numerze inwentarzowym już istnieje.");
            }

            var copy = mapper.Map<BookCopy>(dto);

            await copyRepo.AddAsync(copy);
            await copyRepo.SaveChangesAsync();

            var createdCopy = await copyRepo.GetByIdAsync(copy.Id);
            return mapper.Map<CopyGetDetailedDto>(createdCopy);
        }

        public async Task UpdateCopyAsync(int id, CopyEditDto dto)
        {
            var copy = await copyRepo.GetByIdAsync(id);
            if (copy == null || copy.IsDeleted) throw new KeyNotFoundException("Egzemplarz nie istnieje.");

            if (await copyRepo.ExistsAsync(dto.Signature, dto.InventoryNum, id))
            {
                throw new InvalidOperationException("Inny egzemplarz ma już taką sygnaturę lub numer.");
            }

            mapper.Map(dto, copy);
            await copyRepo.SaveChangesAsync();
        }

        public async Task SoftDeleteCopyAsync(int id)
        {
            var copy = await copyRepo.GetByIdAsync(id);
            if (copy == null || copy.IsDeleted) throw new KeyNotFoundException("Egzemplarz nie istnieje lub już jest w koszu.");

            copy.IsDeleted = true;
            copy.Modified = DateTime.UtcNow;
            await copyRepo.SaveChangesAsync();
        }

        public async Task HardDeleteCopyAsync(int id)
        {
            var copy = await copyRepo.GetByIdAsync(id);
            if (copy == null) throw new KeyNotFoundException("Egzemplarz nie istnieje.");

            if (!copy.IsDeleted)
            {
                throw new InvalidOperationException("Egzemplarz musi być najpierw przeniesiony do kosza.");
            }

            copyRepo.Delete(copy);
            await copyRepo.SaveChangesAsync();
        }
    }
}
