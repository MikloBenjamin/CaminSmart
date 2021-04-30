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
		public async Task<IActionResult> Inscriere([Bind("IdApplicant,Nume,Prenume,Facultate,Varsta,Adresa,Email,An,FilePath")] Applicant apl)
		{
			System.Diagnostics.Debug.WriteLine(apl.Nume + " " + apl.Prenume + " " + apl.Adresa + " " + apl.FilePath);
			if (ModelState.IsValid)
			{
				apl.Email = User.Identity.Name;
				apl.IdApplicant = idAppl;
				idAppl++;
				BlobServiceClient serviceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=camineuvtstorage;AccountKey=s9ifIu1cH0Y9KXCFhQTNED+VmEy1eECvG5HAFrUHWtmsO5zLC9eV1V+vj4rG2yJPntm7gOHE0baigX5YW8dQ/A==;EndpointSuffix=core.windows.net");
				BlobContainerClient containerClient = serviceClient.GetBlobContainerClient("inscrieri");
				FileStream fl = new FileStream("UploadFiles/" + apl.FilePath, FileMode.Open);
				await containerClient.UploadBlobAsync("UploadFiles/" + Path.GetFileName(apl.FilePath), fl);
				_context.Add(apl);
				await _context.SaveChangesAsync();
				System.Diagnostics.Debug.WriteLine("Added applicant to DB and his file to Storage");

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
				var response = await client.SendEmailAsync(msg);
				return RedirectToAction("Index", "Home");
			}
			return View();
		}
	}
}
