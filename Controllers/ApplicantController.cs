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
			var applicant = _context.Applicant.Where(entry => entry.Email == User.Identity.Name).FirstOrDefault();
			bool is_applicant = applicant != null;
			if (is_applicant == true)
			{
				return RedirectToAction("Home", "Student");
			}
			return View("Inscriere", applicant);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Inscriere([Bind("IdApplicant,Nume,Prenume,Facultate,Varsta,Adresa,Email,An")] Applicant apl, IFormFile file)
		{
			if (ModelState.IsValid)
			{
				apl.Email = User.Identity.Name;
				apl.IdApplicant = idAppl;
				string fileName = apl.Nume + "_" + apl.Prenume + "_" + idAppl.ToString() + ".pdf";
				string path = Url.Content("wwwroot/UploadFiles/") + fileName;
				using (FileStream stream = new FileStream(path, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}
				FileStream fl = new FileStream(path, FileMode.Open);
				await GlobalVariables.BlobClient.GetBlobContainerClient("inscrieri").UploadBlobAsync(fileName, fl);
				_context.Add(apl);
				await _context.SaveChangesAsync();
				idAppl++;

				var client = new SendGridClient(GlobalVariables.SendGridApiKey);
				var from = new EmailAddress("florin.marut99@e-uvt.ro", "Florin");
				var to = new EmailAddress(apl.Email, apl.Prenume);
				var subject = "Cazarea ta a fost înregistrată cu succes!";
				var plainTextContent = @"Buna, drag Student! 
							Te informam ca te-ai inregistrat cu succes pe platforma Managment in camine si iti multumim ca ai aplicat pentru un loc in camin.
							Vom reveni cat de curand cu mai multe detalii despre statusul tau de cazare!
							Numai Bine!
							Asociatia Managementul in Camine UVT";
				var htmlContent = @"<b>Buna, drag Student!</b> <br>
							Te informam ca te-ai inregistrat cu succes pe platforma Managment in camine si iti multumim ca ai aplicat pentru un loc in camin. <br>
							Vom reveni cat de curand cu mai multe detalii despre statusul tau de cazare! <br>
							Numai Bine! <br>
							Asociatia Managementul in Camine UVT";
				var msg = MailHelper.CreateSingleEmail(
					from,
					to,
					subject,
					plainTextContent,
					htmlContent
					);
				await client.SendEmailAsync(msg);
				return RedirectToAction("Home", "Student");
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
			BlobContainerClient containerClient = GlobalVariables.BlobClient.GetBlobContainerClient("inscrieri");
			var model = GetAllBlobs(containerClient, applicants);
			var files = Directory.GetFiles(@"wwwroot/UploadFiles");
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
			int toAccept = int.Parse(applNameID.Split(".")[0].Split("_")[2]);

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
			int toAccept = int.Parse(applNameID.Split(".")[0].Split("_")[2]);

			var aplc = _context.Applicant.Where(apl => apl.IdApplicant == toAccept);
			if (aplc.Count() > 0)
			{
				_context.Remove(aplc.First());
				var client = new SendGridClient(GlobalVariables.SendGridApiKey);
				var from = new EmailAddress("florin.marut99@e-uvt.ro", "Florin");
				var to = new EmailAddress(aplc.First().Email, aplc.First().Prenume);
				var subject = "Cazare REFUZATA!";
				var plainTextContent = @"Buna, drag Student! 
								Ne cerem scuze pentru feedback-ul negativ, dar din pacate nu ai fost acceptat pentru locul in camin!
								Numai Bine!
								Asociatia Managementul in Camine UVT";
				var htmlContent = @"<b>Buna, drag Student!</b><br> 
								Ne cerem scuze pentru feedback-ul negativ, dar din pacate nu ai fost acceptat pentru locul in camin!<br>
								Numai Bine!<br>
								Asociatia Managementul in Camine UVT";
				var msg = MailHelper.CreateSingleEmail(
					from,
					to,
					subject,
					plainTextContent,
					htmlContent
					);
				await client.SendEmailAsync(msg);
			}
			await _context.SaveChangesAsync();
			GlobalVariables.BlobClient.GetBlobContainerClient("inscrieri").DeleteBlob(applNameID);

			return RedirectToAction("Applicants");
		}
	}
}
