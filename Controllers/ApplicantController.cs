using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AplicatieCamine.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Http;
namespace AplicatieCamine
{
	public class ApplicantController : Controller
	{
		private readonly DBSistemContext _context;
		private int idAppl = 1;
		public ApplicantController(DBSistemContext context)
        {
            _context = context;
            var applicants = _context.Applicant.Select(a => a).AsEnumerable();
            idAppl = 1;
            if (applicants.Count() > 0)
            {
                idAppl = applicants.Last().IdApplicant + 1;
            }
        }
        public ActionResult Index()
		{
			return View("Inscriere");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Inscriere([Bind("IdApplicant,Nume,Prenume,Facultate,Varsta,Adresa,Email,An")] Applicant apl, IFormFile file)
		{
			if (ModelState.IsValid)
			{
				apl.Email = User.Identity.Name;
				apl.IdApplicant = idAppl;
				string path = Url.Content("wwwroot/UploadFiles/") + apl.Nume + "_" + apl.Prenume + "_" + idAppl.ToString() + ".pdf";
				using (FileStream stream = new FileStream(path, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				string fileName = apl.Nume + "_" + apl.Prenume + "_" + idAppl.ToString() + ".pdf";
				BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
				BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
				FileStream fl = new FileStream(path, FileMode.Open);
				await containerClient.UploadBlobAsync(fileName, fl);
				_context.Add(apl);
				await _context.SaveChangesAsync();
				idAppl++;

				var apiKey = "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";
				var client = new SendGridClient(apiKey);
				var from = new EmailAddress("florin.marut99@e-uvt.ro", "Florin");
				var to = new EmailAddress(apl.Email, apl.Prenume);
				var subject = "Cazarea ta a fost înregistrată cu succes!";
				var plainTextContent = "Salut, te informăm ca te-ai cazat cu success, blah, blah, blah...";
				var htmlContent = "<strong>Salut, te informăm ca te-ai cazat cu success, blah, blah, blah...</strong>";
				var msg = MailHelper.CreateSingleEmail(
					from,
					to,
					subject,
					plainTextContent,
					htmlContent
					);
				await client.SendEmailAsync(msg);
				System.Diagnostics.Debug.WriteLine("Email successfully sent!");
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
		public IEnumerable<BlobClient> GetAllBlobs(BlobContainerClient container, IEnumerable<string> applicants)
		{
			foreach (BlobItem blob in container.GetBlobs(BlobTraits.None, BlobStates.None, string.Empty))
			{
				yield return container.GetBlobClient(blob.Name);
			}
		}

		public async Task<IActionResult> Applicants()
		{
			var applicants = _context.Applicant.Select(apl => apl.Nume + "_" + apl.Prenume + "_" + apl.IdApplicant).AsEnumerable();
			BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
			BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
			var model = GetAllBlobs(containerClient, applicants);
			var files = System.IO.Directory.GetFiles(@"wwwroot/UploadFiles");
			System.Diagnostics.Debug.WriteLine(files.ToString());
			foreach (BlobItem blob in containerClient.GetBlobs(BlobTraits.None, BlobStates.None, string.Empty))
			{
				if (!files.Contains(blob.Name)){
					BlobClient bl = containerClient.GetBlobClient(blob.Name);
					var blobProperties = await bl.GetPropertiesAsync();
					using(FileStream file = new FileStream(Url.Content("wwwroot/UploadFiles/") + blob.Name, FileMode.Create))
					{
						await bl.DownloadToAsync(file);
					}
				}
			}
			return View(model);
		}
		[HttpPost]
		public IActionResult Accept()
		{
			string applNameID = Request.Form["file"];
			int toAccept = Int32.Parse(applNameID.Split(".")[0].Split("_")[2]);

			Applicant aplc = _context.Applicant.Where(apl => apl.IdApplicant == toAccept).First();
			
			return RedirectToAction("AcceptApplicant", "Student", new { 
				id = aplc.IdApplicant, nume = aplc.Nume, prenume = aplc.Prenume, facultate = aplc.Facultate,
				email = aplc.Email, adresa = aplc.Adresa, an = aplc.An, varsta = aplc.Varsta
			});
		}
		[HttpPost]
		public async Task<IActionResult> Refuse()
		{
			string applNameID = Request.Form["file"];
			int toAccept = Int32.Parse(applNameID.Split(".")[0].Split("_")[2]);

			var aplc = _context.Applicant.Where(apl => apl.IdApplicant == toAccept);
			if (aplc.Count() > 0)
			{
				_context.Remove(aplc.First());
			}
			await _context.SaveChangesAsync();
			BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
			BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
			containerClient.DeleteBlob(applNameID);
			return RedirectToAction("Applicants");
		}
	}
}
