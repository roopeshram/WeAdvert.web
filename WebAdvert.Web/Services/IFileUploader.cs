using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace WebAdvert.Web.Services
{
    public interface IFileUploader
    {
        Task<bool> UploadFileAsync(string filename, Stream storagestream);
    }
}
