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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faculty/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IdentityUserId,Name,Email,PhoneNumber,Password")] FacultyProfile facultyProfile)
        {
            try
            {
                var Student = new IdentityUser
                {
                    UserName = facultyProfile.Email, 
                    Email = facultyProfile.Email,
                    EmailConfirmed = true
                };
                var result = await _userManager.CreateAsync(Student, facultyProfile.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(Student, "Faculty");
                    facultyProfile.IdentityUserId = Student.Id;
                    
                    if (ModelState.IsValid)
                    {
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
                return View(facultyProfile);
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

            var facultyProfile = await _context.FacultyProfiles.FindAsync(id);
            if (facultyProfile == null)
            {
                return NotFound();
            }
            return View(facultyProfile);
        }

        // POST: Faculty/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdentityUserId,Name,Email,PhoneNumber")] FacultyProfile facultyProfile)
        {
            if (id != facultyProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyProfileExists(facultyProfile.Id))
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
            return View(facultyProfile);
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
