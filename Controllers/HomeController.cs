using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using label_generator.Models;
using System.IO;

namespace label_generator.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult PatientLabel()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneratePatientLabel ([Bind("FirstName,LastName,BookNumber,Pesel")] Patient patient)
        {
            string pdfFilename = patient.generate ();
            string downFilename = patient.getDownloadFilename();
            await Task.Delay (1000);
            return RedirectToAction ("GetPdfFile", new { filename = pdfFilename, downloadFilename = downFilename });
        }

        public IActionResult GetPdfFile (string filename, string downloadFilename)
        {
            const string contentType = "application/pdf";
            HttpContext.Response.ContentType = contentType;
            FileContentResult result = null;
            filename = "tmp/" + filename;

            try
            {
                result = new FileContentResult (System.IO.File.ReadAllBytes (filename), contentType)
                {
                    FileDownloadName = downloadFilename + ".pdf"
                };
                //Tools.deleteTempFiles (filename.Substring (4, 64));
                return result;
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine (e.Message);
                return NotFound ();
            }
        }
    }
}