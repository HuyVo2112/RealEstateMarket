#nullable disable
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

namespace Assignment2.Controllers
{
    public class ClientsController : Controller
    {
        private readonly MarketDbContext _context;

        public ClientsController(MarketDbContext context)
        {
            _context = context;
        }

        // GET: Clients
        public async Task<IActionResult> Index(int? id)
        {
            var viewModel = new BrokeragesViewModel
            {
                Clients = await _context.Clients
                 .Include(i => i.Subscriptions)
                 .AsNoTracking()
                 .OrderBy(i => i.FirstName)
                 .ToListAsync()
            };

            if (id != null)
            {
                //ViewData["ClientID"] = id;


                viewModel.Subscriptions = viewModel.Clients.Where(x => x.Id == id).Single().Subscriptions;

                var brokerageIDS = (from subscription in viewModel.Subscriptions select subscription.BrokerageId).ToList();

                var Brokerages = await _context.Brokerages.ToListAsync();
                viewModel.Brokerages = (from brokerage in Brokerages where brokerageIDS.Contains(brokerage.Id) select brokerage).ToList();

            }

            return View(viewModel);
            //return View(await _context.Clients.ToListAsync());
        }



        // GET: Clients/Details/5
        public async Task<IActionResult> EditSubscriptions(int id)
        {
            int tempNum = -1;
            try
            {
                tempNum = Int32.Parse(Request.Form["registerOrNot"].First());
            }
            catch(Exception ex)
            {

            }

            if (tempNum == 0)
            {
                string brokerageId = Request.Form["BrokerageId"].First();

                var subscription = await _context.Subscriptions.FindAsync(id, brokerageId);
                _context.Subscriptions.Remove(subscription);
                await _context.SaveChangesAsync();
            }

            if (tempNum == 1)
            {
                string brokerageId = Request.Form["BrokerageId"].First();

                var subscription = new Subscription { ClientId = id, BrokerageId = brokerageId };
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();
            }


            var viewModel = new ClientSubscriptionsViewModel();




            //ViewData["ClientID"] = id;

            var Clients = await _context.Clients.ToListAsync();
            viewModel.Client = (from c in Clients where c.Id == id select c).ToList().Single();

            var AllBrokerages = await _context.Brokerages.ToListAsync();

            var AllSubscriptions = await _context.Subscriptions.ToListAsync();

            var Subscriptions = (from subscription in AllSubscriptions where subscription.ClientId == id select subscription).ToList();
            List<String> brokerageIDS;
            List<Brokerage> SubBrokerages;

            if (Subscriptions != null)
            {
                brokerageIDS = (from subscription in Subscriptions select subscription.BrokerageId).ToList();
                SubBrokerages = (from brokerage in AllBrokerages where brokerageIDS.Contains(brokerage.Id) select brokerage).ToList();
            }
            else
            {
                SubBrokerages = new List<Brokerage>();
            }

            viewModel.Subscriptions = new List<BrokerageSubscriptionsViewModel>();
            foreach (var brokerage in AllBrokerages)
            {
                BrokerageSubscriptionsViewModel temp = new BrokerageSubscriptionsViewModel();
                temp.BrokerageId = brokerage.Id;
                temp.Title = brokerage.Title;

                if (SubBrokerages.Contains(brokerage))
                {
                    temp.IsMember = true;
                }
                else
                {
                    temp.IsMember = false;
                }
                viewModel.Subscriptions = viewModel.Subscriptions.Append(temp);
            }
            //


            return View(viewModel);
            //return View(await _context.Clients.ToListAsync());
        }



        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,BirthDate")] Client client)
        {
            if (id != client.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.Id))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.Id == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
