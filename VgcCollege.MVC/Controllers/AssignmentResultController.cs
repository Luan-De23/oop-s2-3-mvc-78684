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
            string currentUserEmail = User.Identity.Name;

            var query = _context.AssignmentResults
                .Include(a => a.Assigment)
                .Include(a => a.FacultyProfile)
                .Include(a => a.StudentProfile)
                .AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                query = query.Where(r => r.Assigment.Course.FacultyProfiles.Any(f => f.Email == currentUserEmail));
            }

            return View(await query.ToListAsync());
        }

        // GET: AssignmentResult/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assigment)
                .Include(a => a.FacultyProfile)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            if (User.IsInRole("Faculty"))
            {
                var isOwner = await _context.Courses.AnyAsync(c => c.Id == assignmentResult.Assigment.CourseId && 
                               c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
                if (!isOwner) return Forbid();
            }

            return View(assignmentResult);
        }

        // GET: AssignmentResult/Create
        public IActionResult Create()
        {
            string currentUserEmail = User.Identity.Name;

            var assignmentsQuery = _context.Assignments.Include(a => a.Course).AsQueryable();
            if (User.IsInRole("Faculty"))
            {
                assignmentsQuery = assignmentsQuery.Where(a => a.Course.FacultyProfiles.Any(f => f.Email == currentUserEmail));
            }

            ViewData["AssigmentId"] = new SelectList(assignmentsQuery, "Id", "Title");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email");
            
            return View();
        }

        // POST: AssignmentResult/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Score,FeedBack,AssigmentId,StudentProfileId")] AssignmentResult assignmentResult)
        {
            var assignment = await _context.Assignments.FindAsync(assignmentResult.AssigmentId);
            if (assignment != null && assignmentResult.Score > assignment.MaxScore)
            {
                ModelState.AddModelError("Score", $"The score cannot be higher than ({assignment.MaxScore}).");
            }

            var faculty = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
            if (faculty != null)
            {
                assignmentResult.FacultyProfileId = faculty.Id;
            }

            if (ModelState.IsValid)
            {
                _context.Add(assignmentResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResult/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(r => r.Assigment)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (assignmentResult == null) return NotFound();

            if (User.IsInRole("Faculty"))
            {
                var isOwner = await _context.Courses.AnyAsync(c => c.Id == assignmentResult.Assigment.CourseId && 
                               c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
                if (!isOwner) return Forbid();
            }

            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // POST: AssignmentResult/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Score,FeedBack,AssigmentId,StudentProfileId,FacultyProfileId")] AssignmentResult assignmentResult)
        {
            if (id != assignmentResult.Id) return NotFound();

            var assignment = await _context.Assignments.FindAsync(assignmentResult.AssigmentId);
            if (assignment != null && assignmentResult.Score > assignment.MaxScore)
            {
                ModelState.AddModelError("Score", $"the score cannot be higher than ({assignment.MaxScore}).");
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
                    if (!AssignmentResultExists(assignmentResult.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AssigmentId"] = new SelectList(_context.Assignments, "Id", "Title", assignmentResult.AssigmentId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Email", assignmentResult.StudentProfileId);
            return View(assignmentResult);
        }

        // GET: AssignmentResult/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignmentResult = await _context.AssignmentResults
                .Include(a => a.Assigment)
                .Include(a => a.FacultyProfile)
                .Include(a => a.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignmentResult == null) return NotFound();

            if (User.IsInRole("Faculty"))
            {
                var isOwner = await _context.Courses.AnyAsync(c => c.Id == assignmentResult.Assigment.CourseId && 
                               c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
                if (!isOwner) return Forbid();
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
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentResultExists(int id)
        {
            return _context.AssignmentResults.Any(e => e.Id == id);
        }
    }
}