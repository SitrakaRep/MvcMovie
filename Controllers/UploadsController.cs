using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Controllers
{
    public class UploadsController : Controller
    {
        private readonly MvcMovieContext _context;
        private readonly IHostingEnvironment _host;

        public UploadsController(MvcMovieContext context, IHostingEnvironment host)
        {
            this._host = host;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(IFormFile files)
        {
            long size = files.Length;

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            string name = files.FileName;

            long imageBytes = files.Length;
            Console.WriteLine("BBBBYYTTEES ==> {0}", imageBytes);
            var newPhotoBase64 = string.Format(@"Customer-{0}", Guid.NewGuid());

            var otherfilePath = _host.WebRootPath + "/" + newPhotoBase64 + ".jpeg";
            Console.WriteLine("FILE PATH ==> {0}", otherfilePath);
            if (files.Length > 0)
            {
                using (var stream = new FileStream(otherfilePath, FileMode.Create))
                {
                    await files.CopyToAsync(stream);
                    stream.Flush();
                }
            }

            // process upload files
            // Don't rely on or trust the FileName property without validation

            return Ok(new { size, filePath });
        }

        [HttpPost("UploadFiles")]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            long size = files.Sum(f => f.Length);
            // full path to file in temp location
            var filePath = Path.GetTempFileName();
            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok(new { count = files.Count, size, filePath });
        }
    }
}