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

    public class CourseSelectionViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}