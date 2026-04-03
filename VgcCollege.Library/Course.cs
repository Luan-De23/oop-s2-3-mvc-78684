namespace VgcCollege.Library;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; } 
    
    // FK's
    public int BranchId { get; set; }
    public Branch? Branch { get; set; }
    
    // Relations
    public ICollection<CourseEnrolment> CourseEnrolments { get; set; } = new List<CourseEnrolment>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<FacultyProfile> FacultyProfiles { get; set; } = new List<FacultyProfile>();
}