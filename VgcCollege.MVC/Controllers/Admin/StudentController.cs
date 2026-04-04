using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Library;
using VgcCollege.MVC.Data;
using VgcCollege.MVC.Models;

namespace VgcCollege.MVC.Controllers.Admin
{
    
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(ApplicationDbContext context, ILogger<StudentController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Student
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.StudentProfiles.ToListAsync());
        }

        // GET: Student/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // GET: Student/Create
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var coursesFromDb = await _context.Courses.ToListAsync();

            var viewModel = new StudentCreateViewModel
            {
                AvailableCourses = coursesFromDb.Select(c => new CourseEnrollSelectionViewModel
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Student/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StudentCreateViewModel model)
        {
            try
            {
                var Student = new IdentityUser
                {
                    UserName = model.Email, 
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(Student, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(Student, "Student");
                    var studentProfile = new VgcCollege.Library.StudentProfile()
                    {
                        Name = model.Name,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        DOB = model.DOB,
                        IdentityUserId = Student.Id,
                        Address = model.Address,
                        CoursesEnrolments = new List<CourseEnrolment>()

                    };
                    var selectedCourseIds = model.AvailableCourses
                        .Where(x => x.IsSelected)
                        .Select(x => x.CourseId)
                        .ToList();
                    
                    foreach (var courseId in selectedCourseIds)
                    {
                        studentProfile.CoursesEnrolments.Add(new CourseEnrolment()
                        {
                            CourseId = courseId,
                            EnrolDate = DateOnly.FromDateTime(DateTime.Now),
                        });
                    }
                    
                    if (ModelState.IsValid)
                    {
                        _context.Add(studentProfile);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(model);
            }
            catch (Exception e)
            {
                _logger.LogError("Unhandled exception occurred: {Message} by {User}", e.Message, User.Identity?.Name ?? "Anonymous");
                throw;
            }
        }

        // GET: Student/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var student = await _context.StudentProfiles
                .Include(s => s.CoursesEnrolments)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            
            var allCourses = await _context.Courses.ToListAsync();
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            var viewModel = new StudentEditViewModel
            {
                Id = student.Id,
                Name = student.Name,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                DOB = student.DOB,
                AvailableCourses = allCourses.Select(c => new CourseEnrollSelectionViewModel
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    IsSelected = student.CoursesEnrolments.Any(e => e.CourseId == c.Id)
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Student/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, StudentEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var studentToUpdate = await _context.StudentProfiles
                        .Include(s => s.CoursesEnrolments)
                        .FirstOrDefaultAsync(s => s.Id == id);
                    
                    if (studentToUpdate == null) return NotFound();
                    
                    studentToUpdate.Name = model.Name;
                    studentToUpdate.PhoneNumber = model.PhoneNumber;
                    studentToUpdate.Address = model.Address;
                    studentToUpdate.DOB = model.DOB;
                    _context.CourseEnrolments.RemoveRange(studentToUpdate.CoursesEnrolments);
                    
                    var selectedIds = model.AvailableCourses.Where(x => x.IsSelected).Select(x => x.CourseId);
                    foreach (var courseId in selectedIds)
                    {
                        studentToUpdate.CoursesEnrolments.Add(new CourseEnrolment 
                        { 
                            CourseId = courseId,
                            StudentProfileId = studentToUpdate.Id,
                            EnrolDate = DateOnly.FromDateTime(DateTime.Now), 
                        });
                    }
                    
                    await _context.SaveChangesAsync();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!StudentProfileExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError("Unhandled exception occurred: {Message} by {User}", e.Message, User.Identity?.Name ?? "Anonymous");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Student/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var studentProfile = await _context.StudentProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (studentProfile == null)
            {
                return NotFound();
            }

            return View(studentProfile);
        }

        // POST: Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var studentProfile = await _context.StudentProfiles.FindAsync(id);
            if (studentProfile != null)
            {
                _context.StudentProfiles.Remove(studentProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StudentProfileExists(int id)
        {
            return _context.StudentProfiles.Any(e => e.Id == id);
        }
        
        // Grades --Testing
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> MyGrades()
        {
            var currentUserEmail = User.Identity.Name;

            var myResults = await _context.AssignmentResults
                .Include(r => r.Assigment)
                .ThenInclude(a => a.Course)
                .Where(r => r.StudentProfile.Email == currentUserEmail)
                .OrderBy(r => r.Assigment.Course.Name)
                .ToListAsync();

            return View(myResults);
        }
    }
}
