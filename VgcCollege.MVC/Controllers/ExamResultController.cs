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
    public class ExamResultController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ExamResultController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExamResult
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ExamResults.Include(e => e.Exam).Include(e => e.FacultyProfile).Include(e => e.StudentProfile);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ExamResult/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.FacultyProfile)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResult == null)
            {
                return NotFound();
            }

            return View(examResult);
        }

        // GET: ExamResult/Create
        public IActionResult Create(int? examId)
        {
            if (examId == null) return NotFound();

            var exam = _context.Exams
                .Include(e => e.Course)
                .FirstOrDefault(e => e.Id == examId);

            if (exam == null) return NotFound();

            var studentsInCourse = _context.StudentProfiles
                .Where(s => s.CoursesEnrolments.Any(e => e.CourseId == exam.CourseId))
                .Select(s => new { Id = s.Id, Name = s.Name + " (" + s.Email + ")" })
                .ToList();

            ViewBag.StudentList = new SelectList(studentsInCourse, "Id", "Name");
            ViewBag.ExamId = examId;
            ViewBag.ExamTitle = exam.Title;

            return View();
        }
        
        // POST: ExamResult/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Score,Grade,ExamId,StudentProfileId")] ExamResult examResult)
        {
            var faculty = await _context.FacultyProfiles
                .FirstOrDefaultAsync(f => f.Email == User.Identity.Name);

            if (faculty != null)
            {
                examResult.FacultyProfileId = faculty.Id;
            }

            ModelState.Remove("FacultyProfile");
            ModelState.Remove("Exam");
            ModelState.Remove("StudentProfile");
            ModelState.Remove("Result"); 

            if (ModelState.IsValid)
            {
                _context.Add(examResult);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Exam");
            }
            var exam = await _context.Exams.FindAsync(examResult.ExamId);
            ViewBag.ExamTitle = exam?.Title;
            ViewBag.ExamId = examResult.ExamId;
            ViewBag.StudentList = new SelectList(_context.StudentProfiles, "Id", "Name", examResult.StudentProfileId);
    
            return View(examResult);
        }

        // GET: ExamResult/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult == null)
            {
                return NotFound();
            }
            ViewData["ExamId"] = new SelectList(_context.Exams, "Id", "Title", examResult.ExamId);
            ViewData["FacultyProfileId"] = new SelectList(_context.FacultyProfiles, "Id", "Email", examResult.FacultyProfileId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address", examResult.StudentProfileId);
            return View(examResult);
        }

        // POST: ExamResult/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Score,Grade,ExamId,FacultyProfileId,StudentProfileId")] ExamResult examResult)
        {
            if (id != examResult.Id) return NotFound();

            // 1. Validação da Nota Máxima (Reutilizando a lógica do Create)
            var exam = await _context.Exams.FindAsync(examResult.ExamId);
            if (exam != null && examResult.Score > exam.MaxScore)
            {
                ModelState.AddModelError("Score", $"The score cannot be higher than ({exam.MaxScore}).");
            }

            ModelState.Remove("Exam");
            ModelState.Remove("FacultyProfile");
            ModelState.Remove("StudentProfile");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamResultExists(examResult.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            examResult.Exam = await _context.Exams.FindAsync(examResult.ExamId);
            examResult.StudentProfile = await _context.StudentProfiles.FindAsync(examResult.StudentProfileId);
    
            return View(examResult);
        }

        // GET: ExamResult/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults
                .Include(e => e.Exam)
                .Include(e => e.FacultyProfile)
                .Include(e => e.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examResult == null)
            {
                return NotFound();
            }

            return View(examResult);
        }

        // POST: ExamResult/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examResult = await _context.ExamResults.FindAsync(id);
            if (examResult != null)
            {
                _context.ExamResults.Remove(examResult);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamResultExists(int id)
        {
            return _context.ExamResults.Any(e => e.Id == id);
        }
    }
}
