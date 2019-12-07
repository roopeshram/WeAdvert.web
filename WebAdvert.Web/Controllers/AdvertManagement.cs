using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;
using AutoMapper;
using AdvertApi.Models;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAdvertApiClient _advertapiclient;
        private readonly IMapper _mapper;
        

        public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient advertapiclient,IMapper mapper)
        {
            _fileUploader = fileUploader;
            _advertapiclient = advertapiclient;
            _mapper = mapper;
            
        }

        public IActionResult Create(CreateAdvertViewModel model)
        {
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {

                var createadvertmodel = _mapper.Map<CreateAdvertModel>(model);
                var apiCallResponse = await _advertapiclient.Create(createadvertmodel);
                var id = apiCallResponse.Id;

                var fileName = "";

                if (imageFile != null)
                {
                    fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    var filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                .ConfigureAwait(false);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please see the logs for details.");
                        }

                        var confirmmodel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Active
                        };
                        var canConfirm = await _advertapiclient.Confirm(confirmmodel);
                        if (!canConfirm)
                        {
                            throw new Exception($"Can not Confirm Advert of id {id}");
                        }
                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception e)
                    {
                        var confirmmodel = new ConfirmAdvertRequest()
                        {
                            Id = id,
                            FilePath = filePath,
                            Status = AdvertStatus.Pending
                        };
                        await _advertapiclient.Confirm(confirmmodel);
                        Console.WriteLine(e);
                    }
                    
                }

                    
            }

            return View(model);
        }
    }
}