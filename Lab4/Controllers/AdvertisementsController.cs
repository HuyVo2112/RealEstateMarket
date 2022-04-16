using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Assignment2.Data;
using Assignment2.Models;
using Assignment2.Models.ViewModels;
using Azure.Storage.Blobs;
using Azure;

namespace Assignment2.Controllers
{
    public class AdvertisementsController : Controller
    {
        private readonly MarketDbContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "advertisement";

        public AdvertisementsController(MarketDbContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        // GET: Advertisements
        public async Task<IActionResult> Index(string id)
        {
            var Brokerage = await _context.Brokerages.FindAsync(id);

            var viewModel = new AdsViewModel
            {
                Brokerage = Brokerage
            };

            var Advertisements = await _context.Advertisements.ToListAsync();

            var ads = (from advertisement in Advertisements where advertisement.BrokerageId == id select advertisement).ToList();
            viewModel.Advertisements = ads;


            return View(viewModel);
        }

        // GET: Advertisements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements
                .FirstOrDefaultAsync(m => m.AdvertisementId == id);
            if (advertisement == null)
            {
                return NotFound();
            }

            return View(advertisement);
        }

        // GET: Advertisements/Create
        //public async Task<IActionResult> Create(string id)
        //{
        //    var Brokerage = await _context.Brokerages.FindAsync(id);
        //    FileInputViewModel temp = new FileInputViewModel();
        //    if (Brokerage != null)
        //    {
                
        //        temp.BrokerageId = Brokerage.Id;
        //        temp.BrokerageTitle = Brokerage.Title;
        //    }
            

        //    return View(temp);
        //}

  

        public async Task<IActionResult> Create(string id)
        {

            var Brokerage = await _context.Brokerages.FindAsync(id);
            FileInputViewModel temp = new();
 
            if (Brokerage != null)
            {

                temp.BrokerageId = Brokerage.Id;
                temp.BrokerageTitle = Brokerage.Title;
            }
            return View(temp);
           
        }

        // POST: Advertisements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IFormFile file, string id)
        {
            BlobContainerClient containerClient;
            string randomFileName ;

            // Create the container and return a container client object
            try
            {
                containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName);
                // Give access to public
                containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            }
            catch (RequestFailedException)
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }

            try
            {
                randomFileName = Path.GetRandomFileName();
                // create the blob to hold the data
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }

                using (var memoryStream = new MemoryStream())
                {
                    // copy the file data into memory
                    await file.CopyToAsync(memoryStream);

                    // navigate back to the beginning of the memory stream
                    memoryStream.Position = 0;

                    // send the file to the cloud
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
            }

            Advertisement ad = new();

            ad.FileName = randomFileName;
            ad.BrokerageId = id;
            ad.Url = containerClient.GetBlobClient(randomFileName).Uri.ToString();

            _context.Advertisements.Add(ad);
            await _context.SaveChangesAsync();



            

            return RedirectToAction("Index", new { id = id});
        }
    

        // GET: Advertisements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var advertisement = await _context.Advertisements.FindAsync(id);
            if (advertisement == null)
            {
                return NotFound();
            }
            return View(advertisement);
        }

        // POST: Advertisements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdvertisementId,FileName,Url")] Advertisement advertisement)
        {
            if (id != advertisement.AdvertisementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(advertisement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdvertisementExists(advertisement.AdvertisementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(advertisement);
        }

        // GET: Advertisements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var Ad = await _context.Advertisements.FindAsync(id);
            var Brokerage = await _context.Brokerages.FindAsync(Ad.BrokerageId);

            var viewModel = new FileInputViewModel
            {
                Ad = Ad,
                BrokerageId = Ad.BrokerageId,
                BrokerageTitle = Brokerage.Title
            };

            return View(viewModel);
    
        }

        // POST: Advertisements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            Advertisement ad = await _context.Advertisements.FindAsync(id);

            BlobContainerClient containerClient;
            // Get the container and return a container client object
            try
            {
                containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
            }


            try
            {
                // Get the blob that holds the data
                var blockBlob = containerClient.GetBlobClient(ad.FileName);
                if (await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
            }
            catch (RequestFailedException)
            {
                return RedirectToPage("/Error");
            }

            if (ad != null)
            {
                _context.Advertisements.Remove(ad);
                await _context.SaveChangesAsync();
            }
   
            string broId = Request.Form["broId"].First();

            //return RedirectToAction("./Index");
            return RedirectToAction(nameof(Index), new { id = broId});
        }

        private bool AdvertisementExists(int id)
        {
            return _context.Advertisements.Any(e => e.AdvertisementId == id);
        }
    }
}
