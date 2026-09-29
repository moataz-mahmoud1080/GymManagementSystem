using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.BLL.Service.Attstchment
{
    public class AttatchmentService : IAttatchmentService
    {
        private readonly long _maxLengthFile = 5 * 1024 * 1024; // 5 MB

        private readonly string[] _allowedExtention =
        {
            ".jpg",
            ".jpeg",
            ".png"
        };

        private readonly ILogger<AttatchmentService> _logger;
        private readonly IWebHostEnvironment _web;

        public AttatchmentService(
            ILogger<AttatchmentService> logger,
            IWebHostEnvironment web)
        {
            _logger = logger;
            _web = web;
        }


        public async Task<string?> UplodeAsync( Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {
            // Validate file
            if (fileStream is null || !fileStream.CanRead || fileStream.Length == 0)
                return null;

            // 1. Validate Max Size [5 MB]
            if (fileStream.Length > _maxLengthFile)
            {
                _logger.LogWarning( "Rejected Upload: File too large ({FileLength} Bytes).", fileStream.Length);
                return null;
            }

            // 2. Validate Extension
            var extension = Path.GetExtension(fileName);

            if (string.IsNullOrWhiteSpace(extension) ||!_allowedExtention.Contains( extension,StringComparer.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Rejected Upload: Extension ({Extension})",extension);

                return null;
            }

            // 3. Locate the folder
            var uploadsFolder = Path.Combine(
                _web.ContentRootPath,
                folderName);

            Directory.CreateDirectory(uploadsFolder);

            // 4. Make the name unique
            var storedFileName = $"{Guid.NewGuid()}_{fileName}";

            // 5. Build the full file path
            var filePath = Path.Combine(
                uploadsFolder,
                storedFileName);

            try
            {
                // 6. Open a file stream
                using var fs = new FileStream(
                    filePath,
                    FileMode.Create,
                    FileAccess.Write);

                // 7. Copy the file
                await fileStream.CopyToAsync(fs, ct);

                // 8. Return the file name to store in DB
                return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "File Upload Failed for {FileName}",
                    fileName);

                return null;
            }
        }

        public bool Delete(string fileName, string folderName)
        {
            try
            {
                var fullPath= Path.Combine(_web.ContentRootPath,folderName ,fileName);
                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"Failed To Delete {fileName}");
                return false;
            
            }

        }

        public (Stream stream, string countantType)? GetFile(string fileName, string folderName)
        {
            if(string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName)) return null;

            var fullPath = Path.Combine(_web.ContentRootPath, folderName, fileName);
            if (!File.Exists(fullPath)) return null;

            var fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var extention = Path.GetExtension(fileName).ToLowerInvariant();

            var contantType = extention switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpg",
                _ => "application/octet-stream"
            };
            return (fileStream, contantType);
        }


    }
}