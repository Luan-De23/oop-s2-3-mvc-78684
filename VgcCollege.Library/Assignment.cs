namespace VgcCollege.Library;

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public int MaxScore { get; set; } 
    public DateTime DueDate { get; set; }
    
    
    // FK's 
    public int FacultyProfileId { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    
    public int AssigmentResultId { get; set; }
    public AssignmentResult? AssigmentResult { get; set; }
    
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    
    // Relations
    public ICollection<AssignmentResult> ? AssignmentResults { get; set; }
}