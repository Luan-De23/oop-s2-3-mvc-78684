using VgcCollege.Library;
using VgcCollege.MVC.Models;

namespace VgcCollege.MVC.Models
{
    public class FacultyCreateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public List<CourseSelectionViewModel> AvailableCourses { get; set; } = new List<CourseSelectionViewModel>();
    }
    
    public class FacultyEditViewModel
    {
        public int Id { get; set; }
        public string IdentityUserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public List<CourseSelectionViewModel> AvailableCourses { get; set; } = new List<CourseSelectionViewModel>();
    }
    
    public class StudentCreateViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly DOB { get; set; } 

        public List<CourseEnrollSelectionViewModel> AvailableCourses { get; set; } = new List<CourseEnrollSelectionViewModel>();
    }
    
    public class StudentEditViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateOnly DOB { get; set; }
    
        public List<CourseEnrollSelectionViewModel> AvailableCourses { get; set; } = new();
    }
    
    

    public class CourseSelectionViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
    
    public class CourseEnrollSelectionViewModel
    {
        public int CourseId { get; set; }
        public Course? Course { get; set; }
        
        public string CourseName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
    
    public class AttendanceViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int WeekDate { get; set; }
        public List<StudentAttendanceItem> Students { get; set; } = new();
    }

    public class StudentAttendanceItem
    {
        public int EnrolmentId { get; set; }
        public string StudentName { get; set; }
        public AttendaceStatus SelectedStatus { get; set; } 
    }
    

   
}
