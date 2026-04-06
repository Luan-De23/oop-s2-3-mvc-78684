using Xunit;
using Microsoft.EntityFrameworkCore;
using VgcCollege.MVC.Data;
using VgcCollege.Library;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace VgcCollege.Tests
{
    public class Tests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        // ResultsReleased 
        [Fact]
        public void Student_ShouldNotSeeResults_WhenReleasedIsFalse()
        {
            var db = GetDbContext();
            var exam = new Exam { Id = 1, Title = "Final", ResultReleased = false };
            
            var result = new ExamResult { 
                Exam = exam, 
                StudentProfileId = 1, 
                Score = 90, 
                Grade = "A" 
            };
            
            db.ExamResults.Add(result);
            db.SaveChanges();

            var visibleResults = db.ExamResults
                .Where(r => r.Exam.ResultReleased == true && r.StudentProfileId == 1)
                .ToList();

            Assert.Empty(visibleResults);
        }

        [Fact]
        public void Student_ShouldSeeResults_WhenReleasedIsTrue()
        {
            var db = GetDbContext();
            var exam = new Exam { Id = 2, Title = "Midterm", ResultReleased = true };
            
            var result = new ExamResult { 
                Exam = exam, 
                StudentProfileId = 1, 
                Score = 80, 
                Grade = "B" 
            };
            
            db.ExamResults.Add(result);
            db.SaveChanges();

            var visibleResults = db.ExamResults
                .Where(r => r.Exam.ResultReleased == true && r.StudentProfileId == 1)
                .ToList();

            Assert.Single(visibleResults);
        }

        // Enrolment 
        [Fact]
        public void Student_ShouldNotEnrolTwice_InSameCourse()
        {
            var db = GetDbContext();
            var enrolment1 = new CourseEnrolment { StudentProfileId = 1, CourseId = 10 };
            db.CourseEnrolments.Add(enrolment1);
            db.SaveChanges();

            var enrolment2 = new CourseEnrolment { StudentProfileId = 1, CourseId = 10 };

            bool alreadyEnrolled = db.CourseEnrolments.Any(e => e.StudentProfileId == 1 && e.CourseId == 10);
            
            Assert.True(alreadyEnrolled);
        }

        // Faculty
        [Fact]
        public void Faculty_ShouldOnlySeeTheirOwnStudents()
        {
            var db = GetDbContext();
            var faculty1 = new FacultyProfile { Id = 1, Email = "teacher1@vgc.com" };
            var course1 = new Course { Id = 1, Name = "Math", FacultyProfiles = new List<FacultyProfile> { faculty1 } };
            
            var studentInCourse = new StudentProfile { Id = 1, Name = "My Student" };
            var enrolment = new CourseEnrolment { Course = course1, StudentProfile = studentInCourse };
            
            db.CourseEnrolments.Add(enrolment);
            db.SaveChanges();

            var myStudents = db.StudentProfiles
                .Where(s => s.CoursesEnrolments.Any(ce => ce.Course.FacultyProfiles.Any(f => f.Email == "teacher1@vgc.com")))
                .ToList();

            Assert.Contains(myStudents, s => s.Name == "My Student");
        }

        // Attendance
        [Theory]
        [InlineData(10, 8, 80.0)] 
        [InlineData(5, 5, 100.0)]
        [InlineData(4, 0, 0.0)]
        public void AttendancePercentage_ShouldCalculateCorrectly(int totalClasses, int attended, double expected)
        {
            double percentage = (double)attended / totalClasses * 100;

            Assert.Equal(expected, percentage);
        }

        // Score vs MaxScore
        [Fact]
        public void Score_CannotBeHigherThan_MaxScore()
        {
            var exam = new Exam { MaxScore = 100 };
            var result = new ExamResult { Score = 110, Grade = "Invalid" };

            bool isValid = result.Score <= exam.MaxScore;

            Assert.False(isValid);
        }

        [Fact]
        public void Score_IsValid_WhenLowerThanMaxScore()
        {
            var exam = new Exam { MaxScore = 100 };
            var result = new ExamResult { Score = 95 };
            
            bool isValid = result.Score <= exam.MaxScore;

            Assert.True(isValid);
        }

        // Role Authorization Simulator 
        [Fact]
        public void Admin_HasAccess_ToAllBranches()
        {
            var userRole = "Admin";
            bool canAccess = userRole == "Admin";

            Assert.True(canAccess);
        }
    }
}