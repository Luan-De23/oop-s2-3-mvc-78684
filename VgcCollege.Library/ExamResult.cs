namespace VgcCollege.Library;

public class ExamResult
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string Grade { get; set; }
    
    
    //PK's
    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    public int FacultyProfileId { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    
    public int StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    
    public int ExamResultId { get; set; }
    public ExamResult? Result { get; set; }
}