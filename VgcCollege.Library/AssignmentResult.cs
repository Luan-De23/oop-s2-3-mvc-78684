namespace VgcCollege.Library;

public class AssignmentResult
{
    public int Id { get; set; }
    public int Score { get; set; }
    public string FeedBack { get; set; } = string.Empty;
    
    
    
    //FK's
    public int AssigmentId { get; set; }
    public Assignment? Assigment { get; set; }
    
    public int StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }
    
    public int FacultyProfileId { get; set; }
    public FacultyProfile? FacultyProfile { get; set; }
    
}