using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NoticeBoardSystem.Data;
using NoticeBoardSystem.Models;

namespace NoticeBoardSystem.Controllers
{
    [Authorize]
    public class NoticesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoticesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Notices.OrderByDescending(x => x.Dated).ToListAsync());
        }

        // GET: Notices/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notices = await _context.Notices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notices == null)
            {
                return NotFound();
            }

            return View(notices);
        }

        // GET: Notices/Create
        [Authorize (Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Notices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Title,Description")] Notices notices)
        {
            if (ModelState.IsValid)
            {
                notices.Id = Guid.NewGuid();
                notices.Dated = DateTime.Now;
                _context.Add(notices);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notices);
        }

        // GET: Notices/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notices = await _context.Notices.FindAsync(id);
            if (notices == null)
            {
                return NotFound();
            }
            return View(notices);
        }

        // POST: Notices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Description")] Notices notices)
        {
            if (id != notices.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    notices.Dated = DateTime.Now;
                    _context.Update(notices);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticesExists(notices.Id))
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
            return View(notices);
        }

        // GET: Notices/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notices = await _context.Notices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (notices == null)
            {
                return NotFound();
            }

            return View(notices);
        }

        // POST: Notices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var notices = await _context.Notices.FindAsync(id);
            _context.Notices.Remove(notices);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NoticesExists(Guid id)
        {
            return _context.Notices.Any(e => e.Id == id);
        }
    }
}
