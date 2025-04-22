 
using DrillingCore.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DrillingCore.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _env;

        public FileStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
        {
            string uniqueFileName = "";
            try
            {
               
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), subDirectory);
                Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                var fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on saving file " + ex.ToString());
            }

            // Вернуть относительный путь, пригодный для хранения в БД
            return Path.Combine(subDirectory, uniqueFileName).Replace("\\", "/");
        }
    }
}
