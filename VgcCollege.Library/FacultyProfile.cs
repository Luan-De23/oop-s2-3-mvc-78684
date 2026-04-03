namespace VgcCollege.Library;

public class FacultyProfile
{
    public int Id { get; set; }
    public string? IdentityUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    // PK's
    
    // Relations
    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    public ICollection<AssignmentResult> AssignmentResults { get; set; } = new List<AssignmentResult>();
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}

