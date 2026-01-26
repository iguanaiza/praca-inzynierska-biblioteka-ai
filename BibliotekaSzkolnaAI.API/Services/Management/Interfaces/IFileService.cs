namespace BibliotekaSzkolnaAI.API.Services.Management.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);

        void DeleteFile(string relativePath);
    }
}
