using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Library;
using VgcCollege.MVC.Data;
using Microsoft.AspNetCore.Authorization;

namespace VgcCollege.MVC.Controllers
{
    [Authorize(Roles = "Admin, Faculty")]
    public class ExamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exam
        public async Task<IActionResult> Index()
        {
            var query = _context.Exams.Include(e => e.Course).Include(e => e.FacultyProfile).AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                query = query.Where(e => e.Course.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
            }

            return View(await query.ToListAsync());
        }

        // GET: Exam/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.FacultyProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exam == null) return NotFound();

            return View(exam);
        }

        // GET: Exam/Create
        public IActionResult Create()
        {
            var coursesQuery = _context.Courses.AsQueryable();

            if (User.IsInRole("Faculty"))
            {
                coursesQuery = coursesQuery.Where(c => c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
            }

            ViewData["CourseId"] = new SelectList(coursesQuery, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ExamDate,MaxScore,CourseId")] Exam exam)
        {
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.Email == User.Identity.Name);

            if (faculty == null)
            {
                faculty = new FacultyProfile 
                { 
                    Name = "Admin Profile", 
                    Email = User.Identity.Name,
                    PhoneNumber = "00000000" 
                };
                _context.FacultyProfiles.Add(faculty);
                await _context.SaveChangesAsync(); 
            }
            exam.FacultyProfileId = faculty.Id;

            //Avoid some id error
            ModelState.Remove("StudentProfile");
            ModelState.Remove("FacultyProfile");
            ModelState.Remove("Course");

            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync(); 
                return RedirectToAction(nameof(Index));
            }
    
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        // GET: Exam/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            var coursesQuery = _context.Courses.AsQueryable();
            if (User.IsInRole("Faculty"))
            {
                coursesQuery = coursesQuery.Where(c => c.FacultyProfiles.Any(f => f.Email == User.Identity.Name));
            }

            ViewData["CourseId"] = new SelectList(coursesQuery, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ExamDate,MaxScore,ResultReleased,CourseId,FacultyProfileId")] Exam exam)
        {
            if (id != exam.Id) return NotFound();

            // avoid some erros
            ModelState.Remove("FacultyProfile");
            ModelState.Remove("Course");
            ModelState.Remove("StudentProfile");

            if (ModelState.IsValid)
            {
                try
                {
                    var examInDb = await _context.Exams.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            
                    if (exam.CourseId == 0) exam.CourseId = examInDb.CourseId;
                    if (exam.FacultyProfileId == 0) exam.FacultyProfileId = examInDb.FacultyProfileId;

                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Error");
                }
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", exam.CourseId);
            return View(exam);
        }

        // GET: Exam/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.FacultyProfile)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (exam == null) return NotFound();

            return View(exam);
        }

        // POST: Exam/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ToggleResults(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            exam.ResultReleased = !exam.ResultReleased;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }
    }
}