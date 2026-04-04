namespace VgcCollege.Library;

public enum AttendaceStatus
{
    Present,
    Absent,
    Late
}

public class AttendaceRecord
{
    public int Id { get; set; }
    public int WeekNumber { get; set; }
    public AttendaceStatus Status { get; set; } 
    
    // FKs
    public int CourseEnrolmentId { get; set; }
    public CourseEnrolment CourseEnrolment { get; set; }
    
    public int StudentProfileId { get; set; }
    public StudentProfile? StudentProfile { get; set; }

    public int CourseId { get; set; }
    public AttendaceStatus Present { get; set; }
}