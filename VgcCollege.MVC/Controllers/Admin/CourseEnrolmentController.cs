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
using VgcCollege.MVC.Models;

namespace VgcCollege.MVC.Controllers.Admin
{
    public class CourseEnrolmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseEnrolmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CourseEnrolment 
        public async Task<IActionResult> Index()
        {
            string currentUserEmail = User.Identity.Name;

            if (User.IsInRole("Student"))
            {
                var studentEnrolments = await _context.CourseEnrolments
                    .Include(c => c.Course)
                    .Include(c => c.StudentProfile)
                    .Where(e => e.StudentProfile.Email == currentUserEmail)
                    .ToListAsync();
        
                return View("Index", studentEnrolments);
            }

            IQueryable<Course> coursesQuery = _context.Courses
                .Include(c => c.Branch)
                .Include(c => c.FacultyProfiles);

            if (User.IsInRole("Faculty"))
            {
                coursesQuery = coursesQuery.Where(c => c.FacultyProfiles.Any(f => f.Email == currentUserEmail));
            }

            var courses = await coursesQuery.ToListAsync();
            return View("IndexFaculty", courses);
        }
        
        // GET: CourseEnrolment/Details/5 
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseEnrolment = await _context.CourseEnrolments
                .Include(c => c.Course)
                .Include(c => c.StudentProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courseEnrolment == null)
            {
                return NotFound();
            }

            return View(courseEnrolment);
        }

        // GET: CourseEnrolment/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address");
            return View();
        }

        // POST: CourseEnrolment/Create 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,EnrolDate,Status,StudentProfileId,CourseId")]
            CourseEnrolment courseEnrolment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courseEnrolment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Name", courseEnrolment.CourseId);
            ViewData["StudentProfileId"] = new SelectList(_context.StudentProfiles, "Id", "Address",
                courseEnrolment.StudentProfileId);
            return View(courseEnrolment);
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Faculty")]
        public async Task<IActionResult> Attendance(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound("Not Found");

            DateOnly startDate = course.StartDate;
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            int currentWeek = ((today.DayNumber - startDate.DayNumber) / 7) + 1;
            if (currentWeek <= 0) currentWeek = 1;

            var enrolments = await _context.CourseEnrolments
                .Include(e => e.StudentProfile)
                .Where(e => e.CourseId == id).ToListAsync();

            var existingRecords = await _context.AttendaceRecords
                .Where(a => a.CourseId == id && a.WeekNumber == currentWeek).ToListAsync();

            var viewModel = new AttendanceViewModel
            {
                CourseId = id,
                CourseName = course.Name,
                WeekDate = currentWeek,
                Students = enrolments.Select(e => new StudentAttendanceItem
                {
                    EnrolmentId = e.Id,
                    StudentName = e.StudentProfile.Name,
                    SelectedStatus = existingRecords.FirstOrDefault(r => r.CourseEnrolmentId == e.Id)?.Present ?? AttendaceStatus.Absent
                }).ToList()
            };
            return View(viewModel);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAttendance(AttendanceViewModel model)
        {
            foreach (var item in model.Students)
            {
                var record = await _context.AttendaceRecords
                    .FirstOrDefaultAsync(a => a.CourseEnrolmentId == item.EnrolmentId && a.WeekNumber == model.WeekDate);

                if (record == null)
                {
                    var enrolment = await _context.CourseEnrolments.FindAsync(item.EnrolmentId);
                    var newRecord = new AttendaceRecord
                    {
                        CourseEnrolmentId = item.EnrolmentId,
                        StudentProfileId = enrolment.StudentProfileId,
                        WeekNumber = model.WeekDate,
                        CourseId = model.CourseId,
                        Present = item.SelectedStatus
                    };
                    _context.AttendaceRecords.Add(newRecord);
                }
                else
                {
                    record.Present = item.SelectedStatus;
                    _context.Update(record);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AttendanceHistory), new { id = model.CourseId });
        }
        
        [Authorize(Roles = "Student,Admin,Faculty")]
        public async Task<IActionResult> AttendanceHistory(int id)
        {
            if (id == 0) return NotFound("Invalid ID");

            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == id);

            if (course == null) return NotFound($"ID {id} Not Found.");

            ViewBag.CourseName = course.Name;
            ViewBag.CourseId = id;

            var weeksWithRecords = await _context.AttendaceRecords
                .Where(a => a.CourseId == id)
                .Select(a => a.WeekNumber)
                .Distinct()
                .OrderBy(w => w)
                .ToListAsync();

            return View(weeksWithRecords);
        }
        
        public async Task<IActionResult> ViewStatus(int id, int weekNumber)
        {
            string currentUserEmail = User.Identity.Name;

            var query = _context.AttendaceRecords
                .Include(a => a.CourseEnrolment.StudentProfile)
                .Where(a => a.CourseId == id && a.WeekNumber == weekNumber);

            if (User.IsInRole("Student"))
            {
                query = query.Where(a => a.StudentProfile.Email == currentUserEmail);
            }

            var records = await query.ToListAsync();

            if (!records.Any() && User.IsInRole("Student"))
            {
                TempData["Message"] = "Any register for this week.";
            }

            ViewBag.Week = weekNumber;
            return View(records);
        }
    }
}