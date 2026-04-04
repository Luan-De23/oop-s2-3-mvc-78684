using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Library;
using VgcCollege.MVC.Data;

namespace VgcCollege.MVC.Controllers
{
    [Authorize(Roles = "Admin, Faculty")]
    public class AssignmentResultController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AssignmentResult
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AssignmentResults.Include(a => a.Assigment).Include(a => a.FacultyProfile).Include(a => a.StudentProfile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AssignmentResult/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assigment)
                .Include(a => a.FacultyProfile)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResult == null)
            {
                return NotFound();
            }

            return View(assignmentResult);
        }

        // GET: AssignmentResult/Create
        public IActionResult Create()
        {
            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title");
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Email");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address");
            return View();
        }

        // POST: AssignmentResult/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Score,FeedBack,AssigmentId,StudentProfileId,FacultyProfileId")] AssignmentResult assignmentResult)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Email", assignmentResult.FacultyProfileId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResult/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult == null)
            {
                return NotFound();
            }
            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Email", assignmentResult.FacultyProfileId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // POST: AssignmentResult/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Score,FeedBack,AssigmentId,StudentProfileId,FacultyProfileId")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignmentResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentResultExists(assignmentResult.Id))
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
            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Email", assignmentResult.FacultyProfileId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResult/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assigment)
                .Include(a => a.FacultyProfile)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (assignmentResult == null)
            {
                return NotFound();
            }

            return View(assignmentResult);
        }

        // POST: AssignmentResult/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignmentResult = await _context.AssignmentResults.FindAsync(id);
            if (assignmentResult != null)
            {
                _context.AssignmentResults.Remove(assignmentResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}
