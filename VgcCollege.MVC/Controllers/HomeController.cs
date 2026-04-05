using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.MVC.Data;
using VgcCollege.Library;
using VgcCollege.MVC.Models;

namespace VgcCollege.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View();
            }

            var email = User.Identity.Name;

            // DASHBOARD ADMIN 
            if (User.IsInRole("Admin"))
            {
                var adminModel = new AdminDashboardViewModel
                {
                    TotalStudents = await _context.StudentProfiles.CountAsync(),
                    TotalFaculty = await _context.FacultyProfiles.CountAsync(),
                    TotalCourses = await _context.Courses.CountAsync(),
                    TotalBranches = await _context.Courses.Select(c => c.Branch).Distinct().CountAsync()
                };
                return View("AdminDashboard", adminModel);
            }

            // DASHBOARD FACULTY 
            if (User.IsInRole("Faculty"))
            {
                var faculty = await _context.FacultyProfiles
                    .Include(f => f.Courses)
                    .FirstOrDefaultAsync(f => f.Email == email);

                if (faculty == null) return View();

                var facultyModel = new FacultyDashboardViewModel
                {
                    MyCourses = faculty.Courses.ToList(),
                    PendingAssignmentsCount = await _context.Assignments
                        .Where(a => a.FacultyProfileId == faculty.Id && !a.AssignmentResults.Any())
                        .CountAsync()
                };
                return View("FacultyDashboard", facultyModel);
            }

            // DASHBOARD STUDENT
            if (User.IsInRole("Student"))
            {
                var student = await _context.StudentProfiles
                    .FirstOrDefaultAsync(s => s.Email == email);

                if (student == null) return View();

                var studentModel = new StudentDashboardViewModel
                {
                    MyEnrolments = await _context.CourseEnrolments
                        .Include(e => e.Course)
                        .Where(e => e.StudentProfileId == student.Id)
                        .ToListAsync(),
                    RecentResults = await _context.AssignmentResults
                        .Include(r => r.Assigment)
                        .Where(r => r.StudentProfileId == student.Id)
                        .OrderByDescending(r => r.Id)
                        .Take(5)
                        .ToListAsync()
                };
                return View("StudentDashboard", studentModel);
            }

            return View();
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}