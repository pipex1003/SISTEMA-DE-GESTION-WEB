namespace Sistema_Inventario_nick.Services
{
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Threading.Tasks;

    public class FileService
    {
        private readonly string _storagePath;

        public FileService(string storagePath)
        {
            _storagePath = storagePath;
        }

        public async Task<byte[]> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return null;

            string filePath = Path.Combine(_storagePath, Path.GetRandomFileName());

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }

}
