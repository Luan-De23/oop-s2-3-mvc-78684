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
    [Authorize(Roles = "Admin")]
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FacultyController> _logger;
        private readonly UserManager<IdentityUser> _userManager;


        public FacultyController(ApplicationDbContext context, ILogger<FacultyController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Faculty
        public async Task<IActionResult> Index()
        {
            return View(await _context.FacultyProfiles.ToListAsync());
        }

        // GET: Faculty/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facultyProfile == null)
            {
                return NotFound();
            }

            return View(facultyProfile);
        }

        // GET: Faculty/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var coursesFromDb = await _context.Courses.ToListAsync();

            var viewModel = new FacultyCreateViewModel
            {
                AvailableCourses = coursesFromDb.Select(c => new CourseSelectionViewModel
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    IsSelected = false
                }).ToList()
            };

            return View(viewModel);
            //return View();
        }

        // POST: Faculty/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacultyCreateViewModel model)
        {
            try
            {
                var Faculty = new IdentityUser
                {
                    UserName = model.Email, 
                    Email = model.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(Faculty, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(Faculty, "Faculty");
                    var facultyProfile = new VgcCollege.Library.FacultyProfile
                    {
                        Name = model.Name,
                        Email = model.Email,
                        PhoneNumber =  model.PhoneNumber,
                        IdentityUserId = Faculty.Id,
                        Courses = new List<Course>()
                    };
                    var selectedIds = model.AvailableCourses
                        .Where(x => x.IsSelected)
                        .Select(x => x.CourseId)
                        .ToList();
                    
                    var coursesToAdd = await _context.Courses
                        .Where(c => selectedIds.Contains(c.Id))
                        .ToListAsync();
                    foreach (var course in coursesToAdd)
                    {
                        facultyProfile.Courses.Add(course);
                    }
                    
                    
                    if (ModelState.IsValid)
                    {
                        _context.FacultyProfiles.Add(facultyProfile);
                        _context.Add(facultyProfile);
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

        // GET: Faculty/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculty = await _context.FacultyProfiles
                .Include(f => f.Courses) 
                .FirstOrDefaultAsync(f => f.Id == id);

            if (faculty == null) return NotFound();

            var allCourses = await _context.Courses.ToListAsync();

            var viewModel = new FacultyEditViewModel
            {
                Id = faculty.Id,
                IdentityUserId = faculty.IdentityUserId,
                Name = faculty.Name,
                Email = faculty.Email,
                PhoneNumber = faculty.PhoneNumber,

                AvailableCourses = allCourses.Select(c => new CourseSelectionViewModel
                {
                    CourseId = c.Id,
                    CourseName = c.Name,
                    IsSelected = faculty.Courses.Any(fc => fc.Id == c.Id)
                }).ToList()
            };
            return View(viewModel);
        }

        // POST: Faculty/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacultyEditViewModel model)        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var facultyToUpdate = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.Id == id);

                if (facultyToUpdate == null) return NotFound();

                facultyToUpdate.Name = model.Name;
                facultyToUpdate.PhoneNumber = model.PhoneNumber;

                facultyToUpdate.Courses.Clear();

                var selectedIds = model.AvailableCourses
                    .Where(x => x.IsSelected)
                    .Select(x => x.CourseId)
                    .ToList();

                var chosenCourses = await _context.Courses
                    .Where(c => selectedIds.Contains(c.Id))
                    .ToListAsync();

                foreach (var course in chosenCourses)
                {
                    facultyToUpdate.Courses.Add(course);
                }

                try
                {
                    _context.Update(facultyToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyProfileExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Faculty/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyProfile = await _context.FacultyProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facultyProfile == null)
            {
                return NotFound();
            }

            return View(facultyProfile);
        }

        // POST: Faculty/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile != null)
            {
                _context.FacultyProfiles.Remove(facultyProfile);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyProfileExists(int id)
        {
            return _context.FacultyProfiles.Any(e => e.Id == id);
        }
    }
}
