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
    public class AssignmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssignmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Assignment 
        public async Task<IActionResult> Index()
        {
            string currentUserEmail = User.Identity.Name;

            var query = _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.FacultyProfile)
                .AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                query = query.Where(a => a.Course.FacultyProfiles.Any(f => f.Email == currentUserEmail));
            }

            return View(await query.ToListAsync());
        }

        // GET: Assignment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .Include(a => a.FacultyProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null) return NotFound();

            // Some protection
            
            if (User.IsInRole("Faculty") && !IsCourseOwner(assignment.CourseId))
            {
                return Forbid();
            }

            return View(assignment);
        }

        // GET: Assignment/Create
        public IActionResult Create()
        {
            string currentUserEmail = User.Identity.Name;
            
            var coursesQuery = _context.Courses.AsQueryable();
            if (User.IsInRole("Faculty"))
            {
                coursesQuery = coursesQuery.Where(c => c.FacultyProfiles.Any(f => f.Email == currentUserEmail));
            }

            ViewData["CourseId"] = new SelectList(coursesQuery, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,MaxScore,DueDate,CourseId")] Assignment assignment)
        {
            var faculty = await _context.FacultyProfiles.FirstOrDefaultAsync(f => f.Email == User.Identity.Name);
            
            if (faculty != null)
            {
                assignment.FacultyProfileId = faculty.Id;
            }

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Faculty") && !IsCourseOwner(assignment.CourseId))
                {
                    return Forbid();
                }

                _context.Add(assignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }

        // GET: Assignment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment == null) return NotFound();

            if (User.IsInRole("Faculty") && !IsCourseOwner(assignment.CourseId))
            {
                return Forbid();
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", assignment.CourseId);
            return View(assignment);
        }

        // POST: Assignment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,MaxScore,DueDate,CourseId,FacultyProfileId")] Assignment assignment)
        {
            if (id != assignment.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Faculty") && !IsCourseOwner(assignment.CourseId))
                {
                    return Forbid();
                }

                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(assignment);
        }

        // GET: Assignment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var assignment = await _context.Assignments
                .Include(a => a.Course)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (assignment == null) return NotFound();

            if (User.IsInRole("Faculty") && !IsCourseOwner(assignment.CourseId))
            {
                return Forbid();
            }

            return View(assignment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.Assignments.FindAsync(id);
            if (assignment != null)
            {
                _context.Assignments.Remove(assignment);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool IsCourseOwner(int courseId)
        {
            return _context.Courses.Any(c => c.Id == courseId && c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
        }

        private bool AssignmentExists(int id)
        {
            return _context.Assignments.Any(e => e.Id == id);
        }
    }
}