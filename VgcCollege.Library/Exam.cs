namespace VgcCollege.Library;

public class Exam
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public int MaxScore { get; set; }
    public bool ResultReleased { get; set; } = false;
    
    
    // Fk's
    public int? StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    
    public int FacultyProfileId { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    
    // Relations
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}