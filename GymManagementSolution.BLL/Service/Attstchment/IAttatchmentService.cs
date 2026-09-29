using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementSystem.BLL.Service.Attstchment
{
    public interface IAttatchmentService
    {
        Task<string?> UplodeAsync(Stream fileStresm, string fileName, string folderName, CancellationToken ct = default);
        
        bool Delete (string fileName,string folderName);
        (Stream stream , string countantType)? GetFile(string fileName , string folderName);

    }
}
